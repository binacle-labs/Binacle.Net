using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin;

public abstract partial class AdminEndpointsTestsBase :  IAsyncLifetime
{
	protected readonly BinacleApi Sut;
	protected readonly HttpClient Client;

	public AdminEndpointsTestsBase(BinacleApi sut)
	{
		this.Sut = sut;
		this.Client = sut.CreateClient();
	}

	public virtual ValueTask InitializeAsync()
	{
		return ValueTask.CompletedTask;
	}

	public virtual ValueTask DisposeAsync()
	{
		this.Client.Dispose();
		return ValueTask.CompletedTask;
	}
	
	// A negative test written as an anonymous object keeps compiling after the contract is renamed, still gets
	// its 422, and passes on nothing from then on. Serialize the typed request instead and replace one field
	// with the raw JSON a client can send: nameof fails the build when the property moves, and the key check
	// fails the test when the wire name does.
	protected async Task<HttpResponseMessage> SendWithFieldValueAsync<TRequest>(
		HttpMethod method,
		string url,
		TRequest request,
		string propertyName,
		JsonNode? value
	)
	{
		var payload = JsonSerializer.SerializeToNode(request, this.Sut.JsonSerializerOptions)!.AsObject();
		var wireName = this.Sut.JsonSerializerOptions.PropertyNamingPolicy?.ConvertName(propertyName)
		               ?? propertyName;

		payload.ContainsKey(wireName).ShouldBeTrue($"'{wireName}' is not on the serialized request");
		payload[wireName] = value;

		return await this.Client.SendAsync(
			new HttpRequestMessage(method, url)
			{
				Content = new StringContent(
					payload.ToJsonString(),
					Encoding.UTF8,
					MediaTypeNames.Application.Json
				)
			},
			TestContext.Current.CancellationToken
		);
	}

	protected static Guid GetCreatedId(HttpResponseMessage response)
	{
		var location = response.Headers.Location!.ToString();
		var parts = location.Split(["/"], StringSplitOptions.RemoveEmptyEntries);
		var id = Guid.Parse(parts.Last());
		return id;
	}
}
