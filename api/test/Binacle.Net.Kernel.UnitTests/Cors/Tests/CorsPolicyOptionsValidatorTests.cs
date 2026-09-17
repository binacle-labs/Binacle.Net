using Binacle.Net.Kernel.Cors;

namespace Binacle.Net.Kernel.UnitTests.Cors;

// A bad origin never fails at runtime. The app starts, and the browser silently blocks the request in
// someone else's console. Startup is the only place an operator finds out.
[Trait("Behavioral Tests", "Ensures a CORS policy's origins are validated as expected")]
public class CorsPolicyOptionsValidatorTests
{
	private readonly CorsPolicyOptionsValidator validator = new();

	private static CorsPolicyOptions OptionsWith(params string[]? allowedOrigins)
		=> new() { AllowedOrigins = allowedOrigins };

	// A closed policy is a valid choice, so absent and empty both stay valid.
	[Fact]
	public void Absent_Or_Empty_Origins_Are_Valid()
	{
		this.validator.Validate(new CorsPolicyOptions()).IsValid.ShouldBeTrue();
		this.validator.Validate(OptionsWith()).IsValid.ShouldBeTrue();
	}

	[Theory]
	[InlineData("https://example.com")]
	[InlineData("http://localhost:5173")]
	[InlineData("https://sub.example.com:8443")]
	[InlineData("*")]
	public void A_Matchable_Origin_Is_Accepted(string origin)
	{
		this.validator.Validate(OptionsWith(origin)).IsValid.ShouldBeTrue();
	}

	// Each of these is something a browser compares against and never matches, so the request is blocked with no
	// hint as to why.
	[Theory]
	[InlineData("https://example.com/")] // trailing slash
	[InlineData("https://example.com/app")] // path
	[InlineData("https://example.com?q=1")] // query
	[InlineData("example.com")] // no scheme
	[InlineData("ftp://example.com")] // not a browser origin
	[InlineData("")]
	[InlineData("   ")]
	public void An_Unmatchable_Origin_Fails_Validation(string origin)
	{
		this.validator.Validate(OptionsWith(origin)).IsValid.ShouldBeFalse();
	}

	[Fact]
	public void An_Unmatchable_Origin_Message_Names_The_Entry_And_Shows_A_Working_One()
	{
		var result = this.validator.Validate(OptionsWith("https://example.com/"));

		var message = result.Errors.Single().ErrorMessage;
		message.ShouldContain("https://example.com/");
		message.ShouldContain("No trailing slash");
	}

	[Fact]
	public void Only_The_Bad_Entry_Is_Reported()
	{
		var result = this.validator.Validate(OptionsWith("https://good.example.com", "https://bad.example.com/"));

		result.Errors.Count.ShouldBe(1);
		result.Errors.Single().ErrorMessage.ShouldContain("bad.example.com");
	}
}
