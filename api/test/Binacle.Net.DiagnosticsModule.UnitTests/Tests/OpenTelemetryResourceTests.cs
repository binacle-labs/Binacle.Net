using Binacle.Net.DiagnosticsModule.ExtensionMethods;
using OpenTelemetry.Resources;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

[Trait("Behavioral Tests", "Ensures the resource carries the host and process")]
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
}
