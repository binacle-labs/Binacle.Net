using System.Net;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.DiagnosticsModule.Middleware;
using Binacle.Net.DiagnosticsModule.UnitTests.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// The middleware reads only the connection's remote address, so a DefaultHttpContext drives it fully: no test
// host, no server.
[Trait("Behavioral Tests", "Ensures health check protection behaves as expected")]
public class HealthChecksProtectionMiddlewareTests
{
	private const string healthPath = "/_health";

	private static HttpContext ContextFor(string path, string? remoteIpAddress)
	{
		var context = new DefaultHttpContext();
		context.Request.Path = path;
		context.Connection.RemoteIpAddress = remoteIpAddress is null ? null : IPAddress.Parse(remoteIpAddress);
		return context;
	}

	private static (HealthChecksProtectionMiddleware Middleware, Func<bool> ReachedHealthCheck) MiddlewareWith(
		params string[]? restrictedIPs
	) => MiddlewareWithKey(null, restrictedIPs);

	private static (HealthChecksProtectionMiddleware Middleware, Func<bool> ReachedHealthCheck) MiddlewareWithKey(
		string? accessKey,
		params string[]? restrictedIPs
	)
	{
		var reachedHealthCheck = false;
		var options = Options.Create(new HealthCheckConfigurationOptions
		{
			Enabled = true,
			Path = healthPath,
			RestrictedIPs = restrictedIPs,
			AccessKey = accessKey
		});

		var middleware = new HealthChecksProtectionMiddleware(
			_ =>
			{
				reachedHealthCheck = true;
				return Task.CompletedTask;
			},
			NullLogger<HealthChecksProtectionMiddleware>.Instance,
			options
		);

		return (middleware, () => reachedHealthCheck);
	}

	[Theory]
	[ClassData(typeof(HealthCheckCallerProvider))]
	public async Task A_Caller_Reaches_The_Health_Check_Only_When_The_Allow_List_Covers_It(
		string entry,
		string callerAddress,
		bool expectedAllowed
	)
	{
		var (middleware, reachedHealthCheck) = MiddlewareWith(entry);
		var context = ContextFor(healthPath, callerAddress);

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBe(expectedAllowed);
		context.Response.StatusCode.ShouldBe(
			expectedAllowed ? StatusCodes.Status200OK : StatusCodes.Status403Forbidden
		);
	}

	[Fact]
	public async Task An_Unknown_Remote_Address_Is_Refused()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWith("192.168.1.0/24");
		var context = ContextFor(healthPath, null);

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeFalse();
		context.Response.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
	}

	[Fact]
	public async Task An_Empty_Allow_List_Restricts_Nobody()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWith();
		var context = ContextFor(healthPath, "10.0.0.1");

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeTrue();
	}

	[Fact]
	public async Task Requests_Off_The_Health_Path_Are_Never_Restricted()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWith("192.168.1.0/24");
		var context = ContextFor("/api/v4/pack/bin", "10.0.0.1");

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeTrue();
	}

	// CIDR notation masks host bits off in every .NET parser, so "192.168.1.1/24" admits 256 addresses. Accepted,
	// because that is what the notation means, but not in silence.
	[Fact]
	public void An_Entry_Whose_Host_Bits_Were_Masked_Off_Is_Reported()
	{
		var logger = new CapturingLogger<HealthChecksProtectionMiddleware>();
		var options = Options.Create(new HealthCheckConfigurationOptions
		{
			Enabled = true,
			Path = healthPath,
			RestrictedIPs = ["192.168.1.1/24", "10.0.0.1", "172.16.0.0/12"]
		});

		_ = new HealthChecksProtectionMiddleware(_ => Task.CompletedTask, logger, options);

		var warning = logger.Warnings.ShouldHaveSingleItem();
		warning.ShouldContain("192.168.1.1/24");
		warning.ShouldContain("192.168.1.0/24");
	}

	// The message has to name the position: the entry itself can be null or blank.
	[Fact]
	public void A_Malformed_Entry_That_Slipped_Past_Validation_Fails_On_Construction()
	{
		var exception = Should.Throw<InvalidOperationException>(
			() => MiddlewareWith("192.168.1.0/24", "not-an-address")
		);

		exception.Message.ShouldContain("position 1");
		exception.Message.ShouldContain("not-an-address");
	}

	private const string accessKey = "0123456789abcdef0123456789abcdef";

	private static HttpContext ContextWithKey(string? remoteIpAddress, string sentKey)
	{
		var context = ContextFor(healthPath, remoteIpAddress);
		context.Request.Headers[HealthChecksProtectionMiddleware.AccessKeyHeader] = sentKey;
		return context;
	}

	[Fact]
	public async Task The_Right_Key_Is_Let_In_From_An_Address_Off_The_Allow_List()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWithKey(accessKey, "192.168.1.0/24");
		var context = ContextWithKey("10.0.0.1", accessKey);

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeTrue();
	}

	[Theory]
	[InlineData("wrong-key")]
	[InlineData("")]
	[InlineData("0123456789abcdef0123456789abcdeF")]
	public async Task A_Wrong_Key_Is_Refused_Even_From_An_Allowed_Address(string sentKey)
	{
		var (middleware, reachedHealthCheck) = MiddlewareWithKey(accessKey, "192.168.1.0/24");
		var context = ContextWithKey("192.168.1.10", sentKey);

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeFalse();
		context.Response.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
	}

	[Fact]
	public async Task The_Key_Sent_Twice_Is_Refused()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWithKey(accessKey);
		var context = ContextFor(healthPath, "10.0.0.1");
		context.Request.Headers[HealthChecksProtectionMiddleware.AccessKeyHeader] =
			new Microsoft.Extensions.Primitives.StringValues([accessKey, accessKey]);

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeFalse();
	}

	[Fact]
	public async Task A_Key_Alone_Protects_The_Health_Check()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWithKey(accessKey);
		var context = ContextFor(healthPath, "10.0.0.1");

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeFalse();
		context.Response.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
	}

	[Fact]
	public async Task Without_A_Key_Sent_An_Allowed_Address_Still_Gets_In()
	{
		var (middleware, reachedHealthCheck) = MiddlewareWithKey(accessKey, "192.168.1.0/24");
		var context = ContextFor(healthPath, "192.168.1.10");

		await middleware.InvokeAsync(context);

		reachedHealthCheck().ShouldBeTrue();
	}

	[Fact]
	public async Task A_Key_Sent_When_None_Is_Configured_Is_Ignored()
	{
		var (openMiddleware, reachedOpen) = MiddlewareWith();
		await openMiddleware.InvokeAsync(ContextWithKey("10.0.0.1", "anything"));
		reachedOpen().ShouldBeTrue();

		var (restrictedMiddleware, reachedRestricted) = MiddlewareWith("192.168.1.0/24");
		var refused = ContextWithKey("10.0.0.1", "anything");
		await restrictedMiddleware.InvokeAsync(refused);
		reachedRestricted().ShouldBeFalse();
		refused.Response.StatusCode.ShouldBe(StatusCodes.Status403Forbidden);
	}
}
