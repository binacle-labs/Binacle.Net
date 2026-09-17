using Binacle.Net.Kernel.Cors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AspNetCorsOptions = Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions;

namespace Binacle.Net.Kernel.UnitTests.Cors;

// The Cors section is one dictionary any owner may add a key to. An owner names its policy and gets it built
// from the section; validation on start rejects a bad origin under a registered key and looks at nothing else.
[Trait("Behavioral Tests", "Ensures Cors policies are bound from the section and validated on start")]
public class CorsExtensionsTests
{
	private static IServiceProvider ProviderWith(string[] policies, params (string Key, string Value)[] entries)
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(entries.ToDictionary(x => x.Key, x => (string?)x.Value))
			.Build();
		var services = new ServiceCollection().AddSingleton<IConfiguration>(configuration);

		foreach (var policy in policies)
			services.AddCorsPolicy(policy);

		return services.BuildServiceProvider();
	}

	[Fact]
	public void A_Policy_Is_Built_From_Its_Key()
	{
		var provider = ProviderWith(
			["CoreApi", "ServiceApi"],
			("Cors:CoreApi:AllowedOrigins:0", "https://a.example.com"),
			("Cors:ServiceApi:AllowedOrigins:0", "https://b.example.com")
		);

		var cors = provider.GetRequiredService<IOptions<AspNetCorsOptions>>().Value;

		cors.GetPolicy("CoreApi")!.Origins.ShouldBe(["https://a.example.com"]);
		cors.GetPolicy("ServiceApi")!.Origins.ShouldBe(["https://b.example.com"]);
	}

	[Fact]
	public void A_Missing_Key_Is_A_Closed_Policy_Not_An_Error()
	{
		var provider = ProviderWith(["CoreApi"]);

		var cors = provider.GetRequiredService<IOptions<AspNetCorsOptions>>().Value;

		cors.GetPolicy("CoreApi")!.Origins.ShouldBeEmpty();
	}

	[Fact]
	public void A_Bad_Origin_Fails_Validation_And_Names_The_Key()
	{
		var provider = ProviderWith(["ServiceApi"], ("Cors:ServiceApi:AllowedOrigins:0", "https://example.com/"));

		var exception = Should.Throw<OptionsValidationException>(() => provider.GetRequiredService<IOptions<CorsOptions>>().Value);

		exception.Message.ShouldContain("'ServiceApi'");
		exception.Message.ShouldContain("https://example.com/");
	}

	[Fact]
	public void A_Key_No_Owner_Registered_Is_Ignored_Even_When_Bad()
	{
		var provider = ProviderWith(["CoreApi"], ("Cors:SomethingElse:AllowedOrigins:0", "https://example.com/"));

		var options = provider.GetRequiredService<IOptions<CorsOptions>>().Value;

		options.Policy("CoreApi").ShouldBeNull();
	}

	[Fact]
	public void A_Registered_Key_With_No_Origins_Is_Valid()
	{
		var provider = ProviderWith(["CoreApi"], ("Cors:CoreApi:AllowedOrigins", ""));

		var options = provider.GetRequiredService<IOptions<CorsOptions>>().Value;

		options.Policy("CoreApi").ShouldNotBeNull();
	}

	[Fact]
	public void Registered_Keys_Pass_Whatever_Their_Case()
	{
		var provider = ProviderWith(["CoreApi"], ("Cors:coreapi:AllowedOrigins:0", "https://example.com"));

		var options = provider.GetRequiredService<IOptions<CorsOptions>>().Value;

		options.Policy("CoreApi")!.AllowedOrigins.ShouldBe(["https://example.com"]);
	}

	[Fact]
	public void The_Section_Is_Validated_Once_However_Many_Owners()
	{
		var provider = ProviderWith(["CoreApi", "ServiceApi"]);

		provider.GetServices<IValidateOptions<CorsOptions>>().Count().ShouldBe(1);
		provider.GetRequiredService<IOptions<CorsRegisteredPolicies>>().Value.Names.Count.ShouldBe(2);
	}
}
