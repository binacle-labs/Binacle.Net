using Binacle.Net.Kernel.OpenApi.Models;
using Binacle.Net.Kernel.OpenApi.Transformers;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi.Tests;

// The operationId is what a generated client names its method. It is carried as metadata rather than taken from
// the endpoint name, because v3 and v4 both want the same unqualified id and endpoint names must be unique
// across the whole app.
[Trait("Behavioral Tests", "Ensures the declared operationId reaches the document")]
public class OperationIdOperationTransformerTests
{
	private static async Task<OpenApiOperation> Transform(params object[] endpointMetadata)
	{
		var operation = new OpenApiOperation { OperationId = "fromTheFramework" };
		await new OperationIdOperationTransformer().TransformAsync(
			operation,
			TransformerContexts.ForOperation(endpointMetadata),
			CancellationToken.None
		);
		return operation;
	}

	[Fact]
	public async Task The_Declared_Id_Replaces_The_Frameworks()
	{
		var operation = await Transform(new OperationIdMetadata("listPresets"));

		operation.OperationId.ShouldBe("listPresets");
	}

	// Metadata accumulates down the route builder, so the one added last is the one meant.
	[Fact]
	public async Task The_Last_Declared_Id_Wins()
	{
		var operation = await Transform(
			new OperationIdMetadata("first"),
			new OperationIdMetadata("last")
		);

		operation.OperationId.ShouldBe("last");
	}

	[Fact]
	public async Task An_Endpoint_That_Declares_Nothing_Keeps_The_Frameworks_Id()
	{
		var operation = await Transform();

		operation.OperationId.ShouldBe("fromTheFramework");
	}
}
