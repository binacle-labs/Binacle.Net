using Binacle.Net.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Kernel.UnitTests.OpenApi;

// The two JWT transformers take the scheme provider as an optional dependency and read one thing off it: whether
// a scheme called "Bearer" is registered. The real provider is used rather than a double, so the registration
// path is the app's.
internal static class AuthenticationSchemeProviders
{
	public static IOptionalDependency<IAuthenticationSchemeProvider> Absent()
		=> new OptionalDependency<IAuthenticationSchemeProvider>(
			new ServiceCollection().BuildServiceProvider()
		);

	public static IOptionalDependency<IAuthenticationSchemeProvider> With(params string[] schemeNames)
	{
		var options = new AuthenticationOptions();
		foreach (var name in schemeNames)
		{
			options.AddScheme(name, builder => builder.HandlerType = typeof(NoopHandler));
		}

		var services = new ServiceCollection();
		services.AddSingleton<IAuthenticationSchemeProvider>(
			new AuthenticationSchemeProvider(Options.Create(options))
		);

		return new OptionalDependency<IAuthenticationSchemeProvider>(services.BuildServiceProvider());
	}

	// A scheme needs a handler type to register. Nothing ever calls it.
	private sealed class NoopHandler : IAuthenticationHandler
	{
		public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context) => Task.CompletedTask;
		public Task<AuthenticateResult> AuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());
		public Task ChallengeAsync(AuthenticationProperties? properties) => Task.CompletedTask;
		public Task ForbidAsync(AuthenticationProperties? properties) => Task.CompletedTask;
	}
}
