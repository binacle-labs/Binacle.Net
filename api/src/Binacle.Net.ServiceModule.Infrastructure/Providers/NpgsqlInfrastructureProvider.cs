using System.Data;
using Binacle.Net.Kernel.Configuration.Models;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Services;
using Binacle.Net.ServiceModule.Infrastructure.Accounts.Services;
using Binacle.Net.ServiceModule.Infrastructure.Common;
using Binacle.Net.ServiceModule.Infrastructure.HealthChecks;
using Binacle.Net.ServiceModule.Infrastructure.StartupTasks;
using Binacle.Net.ServiceModule.Infrastructure.Subscriptions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Npgsql;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Binacle.Net.ServiceModule.Infrastructure.Providers;

internal class NpgsqlInfrastructureProvider : IInfrastructureProvider
{
	private static readonly string[] ServiceTags = ["Service"];

	internal const string TelemetryName = "Npgsql";

	public string ConnectionStringName => "Postgres";

	public void Register(IHostApplicationBuilder builder, ConnectionString connectionString)
	{
		builder.Services.AddSingleton<NpgsqlDataSource>(sp =>
		{
			var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
			return dataSourceBuilder.Build();
		});

		builder.Services.AddTransient<NpgsqlConnection>(sp => sp.GetRequiredService<NpgsqlDataSource>().CreateConnection());
		builder.Services.AddTransient<IDbConnection>(sp => sp.GetRequiredService<NpgsqlDataSource>().CreateConnection());
		builder.Services
			.AddScoped<IAccountRepository, NpgsqlAccountRepository>()
			.AddScoped<ISubscriptionRepository, NpgsqlSubscriptionRepository>();

		builder.Services.AddHealthCheck<NpgsqlHealthCheck>(
			"Database",
			HealthStatus.Unhealthy,
			ServiceTags
		);

		builder.Services.AddStartupTask<EnsureRequiredNpgsqlTablesExistStartupTask>();

		// Does nothing unless OpenTelemetry is on.
		builder.Services
			.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddSource(TelemetryName))
			.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddMeter(TelemetryName));
	}
}
