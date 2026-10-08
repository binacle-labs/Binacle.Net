using FluentValidation;

namespace Binacle.Net.Kernel.Cors;

// One named CORS policy as it is written in a config file. The owner - the core or a module - decides the
// policy's name and which file it comes from; this is the shape under that key.
public class CorsPolicyOptions
{
	public string[]? AllowedOrigins { get; set; }
}

// A bad origin never fails at runtime - the app starts and the browser silently blocks the request in
// someone else's console - so startup is the only place it shows.
public class CorsPolicyOptionsValidator : AbstractValidator<CorsPolicyOptions>
{
	public CorsPolicyOptionsValidator()
	{
		When(x => x.AllowedOrigins is not null, () =>
		{
			RuleForEach(x => x.AllowedOrigins)
				.Must(BeAMatchableOrigin)
				.WithMessage(
					"'{PropertyValue}' is not a usable origin. Use scheme, host and optional port with nothing "
					+ "after it, such as https://example.com or http://localhost:5173. No trailing slash, path "
					+ "or query. Use '*' to allow any origin."
				);
		});
	}

	// The browser compares an origin as an exact string, so a trailing slash or a path never matches anything.
	private static bool BeAMatchableOrigin(string? origin)
	{
		if (string.IsNullOrWhiteSpace(origin))
		{
			return false;
		}

		if (origin == "*")
		{
			return true;
		}

		if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri))
		{
			return false;
		}

		return (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
			&& uri.PathAndQuery == "/"
			&& !origin.EndsWith('/')
			&& string.IsNullOrEmpty(uri.Fragment);
	}
}
