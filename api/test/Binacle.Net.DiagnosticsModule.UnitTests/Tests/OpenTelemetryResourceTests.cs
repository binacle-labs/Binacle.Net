using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.ExtensionMethods;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

[Trait("Behavioral Tests", "Ensures the resource carries the host and process, and who can override what")]
public class OpenTelemetryResourceTests
{
	[Fact]
	public void The_resource_has_the_host_name_and_process_id()
	{
		var attributes = ResourceBuilder.CreateEmpty()
			.AddHostAndProcess()
			.Build()
			.Attributes
			.ToDictionary();

		attributes["host.name"].ShouldBe(Environment.MachineName);
		Convert.ToInt64(attributes["process.pid"]).ShouldBe(Environment.ProcessId);
	}

	[Fact]
	public void A_user_attribute_added_later_wins()
	{
		var userAttributes = new Dictionary<string, object>
		{
			["host.name"] = "custom"
		};

		var attributes = ResourceBuilder.CreateEmpty()
			.AddHostAndProcess()
			.AddAttributes(userAttributes)
			.Build()
			.Attributes
			.ToDictionary();

		attributes["host.name"].ShouldBe("custom");
	}

	[Fact]
	public void With_no_environment_variables_the_defaults_are_used()
	{
		var attributes = ResourceFor(new OpenTelemetryConfigurationOptions(), new Dictionary<string, string?>());

		attributes["service.name"].ShouldBe("Binacle.Net");
		attributes["host.name"].ShouldBe(Environment.MachineName);
	}

	[Fact]
	public void The_resource_has_the_host_environment_lowercased()
	{
		var attributes = ResourceFor(new OpenTelemetryConfigurationOptions(), new Dictionary<string, string?>());

		attributes["deployment.environment.name"].ShouldBe("staging");
	}

	[Fact]
	public void The_resource_attributes_variable_beats_the_host_environment()
	{
		var environment = new Dictionary<string, string?>
		{
			["OTEL_RESOURCE_ATTRIBUTES"] = "deployment.environment.name=from-env"
		};

		var attributes = ResourceFor(new OpenTelemetryConfigurationOptions(), environment);

		attributes["deployment.environment.name"].ShouldBe("from-env");
	}

	[Fact]
	public void Additional_attributes_beat_the_host_environment()
	{
		var options = new OpenTelemetryConfigurationOptions
		{
			AdditionalAttributes = new Dictionary<string, object> { ["deployment.environment.name"] = "from-config" }
		};

		var attributes = ResourceFor(options, new Dictionary<string, string?>());

		attributes["deployment.environment.name"].ShouldBe("from-config");
	}

	[Fact]
	public void The_resource_attributes_variable_beats_the_defaults_and_the_instance_id_key()
	{
		var options = new OpenTelemetryConfigurationOptions { ServiceInstanceId = "from-key" };
		var environment = new Dictionary<string, string?>
		{
			["OTEL_RESOURCE_ATTRIBUTES"] = "host.name=from-env,process.pid=7,service.instance.id=from-env"
		};

		var attributes = ResourceFor(options, environment);

		attributes["host.name"].ShouldBe("from-env");
		attributes["process.pid"].ShouldBe("7");
		attributes["service.instance.id"].ShouldBe("from-env");
	}

	[Fact]
	public void The_service_name_variable_renames_the_service()
	{
		var environment = new Dictionary<string, string?>
		{
			["OTEL_SERVICE_NAME"] = "from-env"
		};

		var attributes = ResourceFor(new OpenTelemetryConfigurationOptions(), environment);

		attributes["service.name"].ShouldBe("from-env");
	}

	[Fact]
	public void Additional_attributes_beat_the_resource_attributes_variable()
	{
		var options = new OpenTelemetryConfigurationOptions
		{
			AdditionalAttributes = new Dictionary<string, object> { ["host.name"] = "from-config" }
		};
		var environment = new Dictionary<string, string?>
		{
			["OTEL_RESOURCE_ATTRIBUTES"] = "host.name=from-env"
		};

		var attributes = ResourceFor(options, environment);

		attributes["host.name"].ShouldBe("from-config");
	}

	// The detector reads the IConfiguration in the container, so no real environment variable is set.
	private static Dictionary<string, object> ResourceFor(
		OpenTelemetryConfigurationOptions options,
		Dictionary<string, string?> environment
	)
	{
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(environment)
			.Build();

		var services = new ServiceCollection();
		services.AddSingleton<IConfiguration>(configuration);

		var builder = services
			.AddOpenTelemetry()
			.WithLogger(new LoggerConfiguration().CreateLogger())
			.ConfigureResource(options, "Staging");

		builder.Builder.WithTracing(_ => { });

		using var provider = services.BuildServiceProvider();

		return provider.GetRequiredService<TracerProvider>()
			.GetResource()
			.Attributes
			.ToDictionary();
	}
}
