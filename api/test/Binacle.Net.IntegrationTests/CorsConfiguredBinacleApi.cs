using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Binacle.Net.IntegrationTests;

// A host with one CORS origin configured. BinacleApi carries no Cors.json on purpose, to prove the closed
// default, so a test that needs an allowed origin needs its own host instead.
public class CorsConfiguredBinacleApi : WebApplicationFactory<IApiMarker>
{
	public const string AllowedOrigin = "https://allowed.example.com";

	public CorsConfiguredBinacleApi()
	{
		this.Client = this.CreateClient();
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		var preBuildConfigurationValues = new Dictionary<string, string?>
		{
			{ "Cors:CoreApi:AllowedOrigins:0", AllowedOrigin }
		};
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(preBuildConfigurationValues)
			.Build();

		builder
			.UseEnvironment("Test")
			// Program.cs reads the Cors section while the host builds, before app.Build() runs.
			.UseConfiguration(configuration)
			.ConfigureAppConfiguration(configurationBuilder =>
			{
				configurationBuilder.AddInMemoryCollection(preBuildConfigurationValues);
			});

		builder.ConfigureTestServices(services =>
		{
			services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
		});
	}

	public HttpClient Client { get; init; }
}
