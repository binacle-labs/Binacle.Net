using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain;
using Binacle.Net.ServiceModule.Infrastructure.Providers;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Binacle.Net.ServiceModule.IntegrationTests.Telemetry;

[Trait("Telemetry Tests", "Ensures the Npgsql spans and metrics reach an OpenTelemetry pipeline")]
public class NpgsqlOpenTelemetry
{
	private readonly BinacleApi sut;

	public NpgsqlOpenTelemetry(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/auth/token";

	[Fact(DisplayName = $"POST {routePath}. Exports Npgsql spans and metrics")]
	public async Task ATokenRequest_Exports_NpgsqlSpansAndMetrics()
	{
		var dataSource = this.sut.Services.GetService<NpgsqlDataSource>();
		Assert.SkipWhen(dataSource is null, "Postgres is not the configured backend for this run.");

		var options = this.sut.Services.GetRequiredService<IOptions<ServiceModuleOptions>>();
		var defaultAdmin = ServiceModuleOptions.ParseAccountCredentials(options.Value.DefaultAdminAccount);
		var request = new TokenRequest()
		{
			Username = defaultAdmin.Username,
			Password = defaultAdmin.Password
		};

		var response = await this.sut.Client.PostAsJsonAsync(
			routePath,
			request,
			this.sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		this.sut.FlushMetrics();

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		this.sut.Spans.Snapshot()
			.ShouldContain(span => span.Source.Name == NpgsqlInfrastructureProvider.TelemetryName);
		this.sut.Metrics.Snapshot()
			.ShouldContain(metric => metric.MeterName == NpgsqlInfrastructureProvider.TelemetryName);
	}
}
