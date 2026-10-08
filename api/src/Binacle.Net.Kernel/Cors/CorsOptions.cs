using FluentValidation;
using Microsoft.Extensions.Options;

namespace Binacle.Net.Kernel.Cors;

// The Cors section as a whole: one entry per policy, keyed by the policy's name. Any file, environment
// variable or test may contribute a key; an owner asks for the one it registers.
public class CorsOptions : Dictionary<string, CorsPolicyOptions?>
{
	public const string SectionName = "Cors";

	public CorsOptions() : base(StringComparer.OrdinalIgnoreCase) {}

	public CorsPolicyOptions? Policy(string name)
	{
		return this.GetValueOrDefault(name);
	}
}

// Runs on start, after every owner has registered. Only a registered key is checked, and only if it has
// origins: each must be one a browser can match. Absent or empty is a closed policy and valid. A key nobody
// registered is ignored.
internal sealed class CorsOptionsValidator : AbstractValidator<CorsOptions>
{
	public CorsOptionsValidator(IOptions<CorsRegisteredPolicies> registered)
	{
		var policyValidator = new CorsPolicyOptionsValidator();

		RuleFor(x => x).Custom((options, context) =>
		{
			foreach (var name in registered.Value.Names)
			{
				var policy = options.Policy(name);
				if (policy is null)
				{
					continue;
				}

				foreach (var error in policyValidator.Validate(policy).Errors)
				{
					context.AddFailure($"Cors policy '{name}': {error.ErrorMessage}");
				}
			}
		});
	}
}
