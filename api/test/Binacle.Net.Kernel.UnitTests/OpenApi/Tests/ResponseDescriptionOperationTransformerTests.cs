using Binacle.Net.Kernel.OpenApi.Helpers;
using Binacle.Net.Kernel.OpenApi.Models;
using Binacle.Net.Kernel.OpenApi.Transformers;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// The prose against each response code. SDK generators dump these verbatim into generated code, so a description
// that never reaches the document is a client with a blank doc comment.
[Trait("Behavioral Tests", "Ensures declared response descriptions reach the document")]
public class ResponseDescriptionOperationTransformerTests
{
	private static async Task<OpenApiOperation> Transform(
		OpenApiOperation operation,
		params object[] endpointMetadata
	)
	{
		await new ResponseDescriptionOperationTransformer().TransformAsync(
			operation,
			TransformerContexts.ForOperation(endpointMetadata),
			CancellationToken.None
		);
		return operation;
	}

	[Fact]
	public async Task A_Description_Replaces_The_One_On_An_Existing_Response()
	{
		var operation = new OpenApiOperation
		{
			Responses = new OpenApiResponses
			{
				["200"] = new OpenApiResponse { Description = "Whatever the framework wrote" },
			},
		};

		await Transform(operation, new ResponseDescriptionMetadata(200, "The bin fits."));

		operation.Responses!["200"].Description.ShouldBe(ResponseDescription.Format(200, "The bin fits."));
	}

	[Fact]
	public async Task A_Description_For_A_Code_That_Is_Not_There_Adds_The_Response()
	{
		var operation = new OpenApiOperation { Responses = new OpenApiResponses() };

		await Transform(operation, new ResponseDescriptionMetadata(404, "No such preset."));

		operation.Responses!.ShouldContainKey("404");
		operation.Responses["404"].Description.ShouldBe(ResponseDescription.Format(404, "No such preset."));
	}

	[Fact]
	public async Task An_Endpoint_That_Declares_Nothing_Is_Left_Alone()
	{
		var operation = new OpenApiOperation
		{
			Responses = new OpenApiResponses
			{
				["200"] = new OpenApiResponse { Description = "Untouched" },
			},
		};

		await Transform(operation);

		operation.Responses!["200"].Description.ShouldBe("Untouched");
	}
}
