using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http;

namespace Binacle.Net.IntegrationTests;

// A negative test written as an anonymous object keeps compiling after the contract is renamed, still gets its
// 422, and passes on nothing from then on. Serialize the typed request instead and replace one field with the
// raw JSON a client can send. nameof at the call site fails the build when a property is renamed; the key
// check fails the test when only the wire name moves.
internal static class NegativeRequest
{
	public static async Task Post_WithFieldValue_Returns_422UnprocessableContent<TRequest>(
		BinacleApi sut,
		string url,
		TRequest request,
		JsonNode? value,
		string[] propertyPath
	)
	{
		// params takes an empty array happily, and the test would then post an untouched valid request and
		// assert nothing.
		propertyPath.ShouldNotBeEmpty();

		var payload = JsonSerializer.SerializeToNode(request, sut.JsonSerializerOptions)!.AsObject();

		var parent = payload;
		for (var i = 0; i < propertyPath.Length - 1; i++)
		{
			parent = Field(parent, sut, propertyPath[i]).AsObject();
		}

		var leaf = WireName(sut, propertyPath[^1]);
		parent.ContainsKey(leaf).ShouldBeTrue($"'{leaf}' is not on the serialized request");
		parent[leaf] = value;

		var response = await sut.Client.PostAsync(
			url,
			new StringContent(payload.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json),
			TestContext.Current.CancellationToken
		);

		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);

		// The key an absent value produces. A client that gets a second key for the same field cannot tell the
		// two apart, and only one of them is documented.
		var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(
			sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);

		problem.ShouldNotBeNull();
		problem.Errors.ShouldContainKey(string.Join('.', propertyPath));
	}

	private static JsonNode Field(JsonObject payload, BinacleApi sut, string propertyName)
	{
		var name = WireName(sut, propertyName);

		payload.ContainsKey(name).ShouldBeTrue($"'{name}' is not on the serialized request");
		return payload[name]!;
	}

	private static string WireName(BinacleApi sut, string propertyName)
	{
		return sut.JsonSerializerOptions.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName;
	}
}
