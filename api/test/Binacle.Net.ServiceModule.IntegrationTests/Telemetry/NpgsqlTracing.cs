using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using Binacle.Net.ServiceModule.Domain;
using Binacle.Net.ServiceModule.Infrastructure.Providers;
using Binacle.Net.ServiceModule.v0.Contracts.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Binacle.Net.ServiceModule.IntegrationTests.Telemetry;

[Trait("Telemetry Tests", "Ensures the Npgsql spans carry no password")]
public class NpgsqlTracing
{
	private readonly BinacleApi sut;

	public NpgsqlTracing(BinacleApi sut)
	{
		this.sut = sut;
	}

	private const string routePath = "/api/auth/token";

	[Fact(DisplayName = $"POST {routePath}. Makes Npgsql spans with no password in them")]
	public async Task ATokenRequest_Makes_NpgsqlSpans_WithNoPassword()
	{
		var dataSource = this.sut.Services.GetService<NpgsqlDataSource>();
		Assert.SkipWhen(dataSource is null, "Postgres is not the configured backend for this run.");

		var connectionString = this.sut.Services.GetRequiredService<IConfiguration>().GetConnectionString("Postgres");
		var password = new NpgsqlConnectionStringBuilder(connectionString).Password;
		password.ShouldNotBeNullOrEmpty();

		var spans = new ConcurrentQueue<Activity>();
		using var listener = new ActivityListener
		{
			ShouldListenTo = source => source.Name == NpgsqlInfrastructureProvider.TelemetryName,
			Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
			ActivityStopped = spans.Enqueue
		};
		ActivitySource.AddActivityListener(listener);

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

		response.StatusCode.ShouldBe(HttpStatusCode.OK);
		spans.ShouldNotBeEmpty();
		foreach (var span in spans)
		{
			span.DisplayName.ShouldNotContain(password);
			foreach (var tag in span.TagObjects)
			{
				(tag.Value?.ToString() ?? string.Empty).ShouldNotContain(password, customMessage: tag.Key);
			}
		}
	}
}
