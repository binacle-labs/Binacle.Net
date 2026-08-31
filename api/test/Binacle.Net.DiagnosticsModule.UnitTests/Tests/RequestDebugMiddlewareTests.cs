using System.Net;
using System.Text;
using Binacle.Net.DiagnosticsModule.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;

namespace Binacle.Net.DiagnosticsModule.UnitTests;

// /_debug echoes the caller's own request back at them. What it must never echo is the one thing worth testing:
// the Cookie header is dropped, and everything else is shown exactly as it arrived.
[Trait("Behavioral Tests", "Ensures the debug endpoint echoes the request and withholds the Cookie header")]
public class RequestDebugMiddlewareTests
{
	private const string DebugPath = "/_debug";
	private static readonly DateTimeOffset Now = new(2030, 1, 1, 0, 0, 0, TimeSpan.Zero);

	private sealed record Call(string Body, bool ReachedNext, HttpContext Context);

	private static async Task<Call> Invoke(
		string path,
		Action<HttpContext>? arrangeRequest = null,
		Action<FeatureOptions>? arrangeFeatures = null,
		Action<ReservedPathOptions>? arrangeReservedPaths = null
	)
	{
		var context = new DefaultHttpContext();
		context.Request.Path = path;
		// A bare DefaultHttpContext has no protocol; a real server always does.
		context.Request.Protocol = "HTTP/1.1";
		context.Connection.RemoteIpAddress = IPAddress.Parse("203.0.113.7");
		context.Connection.RemotePort = 44321;
		arrangeRequest?.Invoke(context);

		var body = new MemoryStream();
		context.Response.Body = body;

		var features = new FeatureOptions();
		arrangeFeatures?.Invoke(features);

		var reservedPaths = new ReservedPathOptions();
		arrangeReservedPaths?.Invoke(reservedPaths);

		var reachedNext = false;
		var middleware = new RequestDebugMiddleware(
			_ =>
			{
				reachedNext = true;
				return Task.CompletedTask;
			},
			new FakeHostEnvironment { EnvironmentName = "Test" },
			Options.Create(features),
			Options.Create(reservedPaths),
			new FakeTimeProvider(Now)
		);

		await middleware.InvokeAsync(context);

		return new Call(Encoding.UTF8.GetString(body.ToArray()), reachedNext, context);
	}

	// Each section runs to the next blank line, so a test can assert on one without matching the whole page.
	private static string[] Section(string body, string name)
		=> body.Split('\n')
			.Select(line => line.TrimEnd('\r'))
			.SkipWhile(line => line != $"[{name}]")
			.Skip(1)
			.TakeWhile(line => line.Length > 0)
			.ToArray();

	[Fact]
	public async Task Any_other_path_passes_through_untouched()
	{
		var call = await Invoke("/v3/pack");

		call.ReachedNext.ShouldBeTrue();
		call.Body.ShouldBeEmpty();
	}

	// StartsWithSegments matches whole segments, so a path that merely begins with the same letters is somebody
	// else's route and must not be answered here.
	[Fact]
	public async Task A_path_that_only_starts_with_the_same_letters_is_not_the_debug_path()
	{
		var call = await Invoke("/_debugger");

		call.ReachedNext.ShouldBeTrue();
		call.Body.ShouldBeEmpty();
	}

	[Fact]
	public async Task The_debug_path_answers_instead_of_calling_the_rest_of_the_pipeline()
	{
		var call = await Invoke(DebugPath);

		call.ReachedNext.ShouldBeFalse();
		call.Context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
		call.Context.Response.ContentType.ShouldBe("text/plain; charset=utf-8");
	}

	[Fact]
	public async Task Anything_under_the_debug_path_answers_too()
	{
		var call = await Invoke($"{DebugPath}/whatever");

		call.ReachedNext.ShouldBeFalse();
		call.Context.Response.StatusCode.ShouldBe(StatusCodes.Status200OK);
	}

	// The output is the caller's own request. The warning is the only thing telling them not to paste it into an
	// issue, so it opens the page.
	[Fact]
	public async Task The_output_opens_with_the_warning_about_pasting_it_anywhere()
	{
		var call = await Invoke(DebugPath);

		call.Body.ShouldStartWith("Binacle.Net debug.");
		call.Body.ShouldContain("Do not paste it anywhere public.");
	}

	// The one header that is withheld. Everything else about the request is echoed, so this is the line that
	// keeps /_debug from handing a session back over the wire.
	[Fact]
	public async Task The_cookie_header_is_not_echoed()
	{
		var call = await Invoke(DebugPath, context =>
		{
			context.Request.Headers["Cookie"] = "session=secret";
			context.Request.Headers["User-Agent"] = "curl/8.0";
		});

		Section(call.Body, "headers").ShouldContain("User-Agent: curl/8.0");
		Section(call.Body, "headers").ShouldNotContain(header => header.StartsWith("Cookie:"));
	}

	// X-Forwarded-For can arrive as several headers rather than one comma separated list, and which it was
	// changes how it should be read - so they are listed one per line rather than joined.
	[Fact]
	public async Task A_header_sent_more_than_once_is_listed_once_per_value()
	{
		var call = await Invoke(DebugPath, context =>
			context.Request.Headers["X-Forwarded-For"] = new[] { "198.51.100.1", "198.51.100.2" });

		Section(call.Body, "headers").ShouldContain("X-Forwarded-For: 198.51.100.1");
		Section(call.Body, "headers").ShouldContain("X-Forwarded-For: 198.51.100.2");
	}

	[Fact]
	public async Task A_request_with_no_cookies_says_so()
	{
		var call = await Invoke(DebugPath);

		Section(call.Body, "cookies").ShouldBe(["(none)"]);
	}

	// The cookies are still listed under their own heading. Dropping the raw header is about the header, not
	// about hiding the values from the caller who sent them.
	[Fact]
	public async Task Cookies_are_listed_under_their_own_heading()
	{
		var call = await Invoke(DebugPath, context =>
			context.Request.Headers["Cookie"] = "first=one; second=two");

		Section(call.Body, "cookies").ShouldBe(["first: one", "second: two"]);
	}

	[Fact]
	public async Task The_request_line_and_the_socket_peer_are_echoed()
	{
		var call = await Invoke(DebugPath, context =>
		{
			context.Request.Method = "POST";
			context.Request.QueryString = new QueryString("?verbose=true");
			context.Request.Scheme = "https";
			context.Request.Host = new HostString("api.example.test");
		});

		Section(call.Body, "connection").ShouldContain("RemoteAddr:   203.0.113.7:44321");
		Section(call.Body, "connection").ShouldContain("Protocol:     HTTP/1.1");
		Section(call.Body, "connection").ShouldContain("IsHttps:      True");
		Section(call.Body, "request").ShouldContain("POST /_debug?verbose=true HTTP/1.1");
		Section(call.Body, "request").ShouldContain("Scheme: https");
		Section(call.Body, "request").ShouldContain("Host:   api.example.test");
	}

	[Fact]
	public async Task The_server_section_reports_the_environment_and_the_current_time()
	{
		var call = await Invoke(DebugPath);

		Section(call.Body, "server").ShouldContain("Environment: Test");
		Section(call.Body, "server").ShouldContain($"UtcNow:      {Now:O}");
	}

	[Fact]
	public async Task Features_and_reserved_paths_say_so_when_there_are_none()
	{
		var call = await Invoke(DebugPath);

		Section(call.Body, "features").ShouldBe(["(none)"]);
		Section(call.Body, "reservedPaths").ShouldBe(["(none)"]);
	}

	// Sorted, so two hosts running the same build produce output that can be diffed.
	[Fact]
	public async Task Features_and_reserved_paths_are_listed_in_order()
	{
		var call = await Invoke(
			DebugPath,
			arrangeFeatures: features =>
			{
				features.AddFeature("UI_MODULE");
				features.AddFeature("SERVICE_MODULE");
			},
			arrangeReservedPaths: reservedPaths =>
			{
				reservedPaths.AddPrefix("/_health");
				reservedPaths.AddPrefix("/api");
			}
		);

		Section(call.Body, "features").ShouldBe(["SERVICE_MODULE", "UI_MODULE"]);
		Section(call.Body, "reservedPaths").ShouldBe(["/_health", "/api"]);
	}
}
