using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace Binacle.Net.Kernel.UnitTests.OpenApi;

// The three transformer contexts the framework hands in. All of them are sealed with init-only properties and no
// factory, so a test builds its own.
internal static class TransformerContexts
{
	private static readonly IServiceProvider EmptyServices = new ServiceCollection().BuildServiceProvider();

	public static OpenApiDocumentTransformerContext ForDocument(IServiceProvider? services = null)
		=> new()
		{
			DocumentName = "test",
			DescriptionGroups = [],
			ApplicationServices = services ?? EmptyServices,
		};

	public static OpenApiOperationTransformerContext ForOperation(
		params object[] endpointMetadata
	) => new()
	{
		DocumentName = "test",
		Description = new ApiDescription
		{
			ActionDescriptor = new ActionDescriptor { EndpointMetadata = endpointMetadata },
		},
		ApplicationServices = EmptyServices,
		Document = new OpenApiDocument(),
	};

	// The schema transformers read the property off JsonPropertyInfo, so the context has to carry a real one.
	public static OpenApiSchemaTransformerContext ForProperty<T>(string propertyName)
	{
		var typeInfo = (JsonTypeInfo)JsonSerializerOptions.Default.GetTypeInfo(typeof(T));
		var property = typeInfo.Properties.Single(p => p.Name == propertyName);

		return new()
		{
			DocumentName = "test",
			ParameterDescription = null!,
			JsonTypeInfo = typeInfo,
			JsonPropertyInfo = property,
			ApplicationServices = EmptyServices,
			Document = new OpenApiDocument(),
		};
	}

	public static OpenApiSchemaTransformerContext ForType<T>()
		=> new()
		{
			DocumentName = "test",
			ParameterDescription = null!,
			JsonTypeInfo = JsonSerializerOptions.Default.GetTypeInfo(typeof(T)),
			JsonPropertyInfo = null!,
			ApplicationServices = EmptyServices,
			Document = new OpenApiDocument(),
		};
}
