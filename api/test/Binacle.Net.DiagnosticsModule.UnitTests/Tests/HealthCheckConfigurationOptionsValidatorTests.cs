using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Configuration.Validators;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// Startup validation keeps a bad allow-list away from the middleware, so an unsupported entry fails here
// rather than at the first health request. Which spellings are unsupported is covered exhaustively in the
// Kernel tests; these rows prove the validator refuses what IPEntry refuses.
[Trait("Behavioral Tests", "Ensures health check configuration is validated as expected")]
public class HealthCheckConfigurationOptionsValidatorTests
{
	private readonly HealthCheckConfigurationOptionsValidator validator = new();

	// The element is nullable because a null entry is one of the cases under test - binding gives RestrictedIPs a
	// null element, which is what a stray comma in the JSON file produces.
	private static HealthCheckConfigurationOptions OptionsWith(string? path, params string?[]? restrictedIPs)
		=> new() { Enabled = true, Path = path, RestrictedIPs = restrictedIPs! };

	[Theory]
	[InlineData("192.168.1.0/24")]
	[InlineData("192.168.1.1")]
	[InlineData("2001:db8::/32")]
	[InlineData("192.168.1.1/24")] // host bits set, meaning the whole 192.168.1.0/24
	[InlineData("  10.0.0.1  ")] // padded by hand in the JSON file; both parsers reject it untrimmed
	public void An_Address_Or_Prefix_Entry_Is_Accepted(string entry)
	{
		var result = this.validator.Validate(OptionsWith("/_health", entry));

		result.IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData("192.168.1.1-192.168.1.9")] // the range form, gone since v3.0.0
	[InlineData("010.10.10.10")] // octal, admits 8.10.10.10
	[InlineData("10.1")] // shorthand, admits 10.0.0.1
	[InlineData("not-an-address")]
	[InlineData("")]
	[InlineData("   ")]
	[InlineData(null)]
	public void An_Unsupported_Entry_Fails_Validation(string? entry)
	{
		var result = this.validator.Validate(OptionsWith("/_health", entry));

		result.IsValid.ShouldBeFalse();
	}

	// A null arrives from a stray comma in the JSON list. It has no value to name in the message, so the row has
	// to be findable by index.
	[Fact]
	public void A_Null_Entry_Is_Reported_By_Its_Position_In_The_List()
	{
		var result = this.validator.Validate(OptionsWith("/_health", "192.168.1.0/24", null));

		result.IsValid.ShouldBeFalse();
		result.Errors.Single().PropertyName.ShouldBe("RestrictedIPs[1]");
	}

	[Fact]
	public void No_Restricted_IPs_Is_Valid()
	{
		this.validator.Validate(OptionsWith("/_health")).IsValid.ShouldBeTrue();
		this.validator.Validate(OptionsWith("/_health", null)).IsValid.ShouldBeTrue();
	}

	// A missing Path once threw out of the validator instead of failing it, so the app died on a stack trace
	// rather than reporting the setting.
	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("_health")]
	public void An_Unusable_Path_Fails_Validation(string? path)
	{
		var result = this.validator.Validate(OptionsWith(path, "192.168.1.0/24"));

		result.IsValid.ShouldBeFalse();
	}

	[Fact]
	public void A_Missing_Path_Is_Reported_Once()
	{
		var result = this.validator.Validate(OptionsWith(null, "192.168.1.0/24"));

		result.Errors.Count.ShouldBe(1);
	}

	// The entry has to appear in the message, or a list of ten with one bad line is unreadable.
	[Fact]
	public void An_Unsupported_Entry_Message_Names_The_Entry_And_The_Removed_Form()
	{
		var result = this.validator.Validate(OptionsWith("/_health", "192.168.1.0/24", "10.0.0.1-10.0.0.9"));

		var message = result.Errors.Single().ErrorMessage;
		message.ShouldContain("10.0.0.1-10.0.0.9");
		message.ShouldContain("prefix length");
	}

	private static HealthCheckConfigurationOptions OptionsWithKey(string? accessKey)
		=> new() { Enabled = true, Path = "/_health", AccessKey = accessKey };

	[Theory]
	[InlineData(null)]
	[InlineData("")]
	[InlineData("0123456789abcdef0123456789abcdef")]
	[InlineData("A1b2-C3d4_E5f6.G7h8~I9j0+K1l2/M3n4=")]
	public void A_Missing_Or_Usable_Access_Key_Is_Accepted(string? accessKey)
	{
		this.validator.Validate(OptionsWithKey(accessKey)).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData("too-short")]
	[InlineData("0123456789abcdef 0123456789abcdef")]
	[InlineData(" 0123456789abcdef0123456789abcdef")]
	[InlineData("0123456789abcdef0123456789abcdef\u00e9")]
	public void An_Access_Key_That_Is_Short_Or_Cannot_Travel_In_A_Header_Fails_Validation(string accessKey)
	{
		var result = this.validator.Validate(OptionsWithKey(accessKey));

		result.IsValid.ShouldBeFalse();
		result.Errors.Count.ShouldBe(1);
	}
}
