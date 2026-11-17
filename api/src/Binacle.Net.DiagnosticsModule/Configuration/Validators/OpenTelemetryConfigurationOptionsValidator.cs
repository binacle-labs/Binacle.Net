using Binacle.Net.DiagnosticsModule.Configuration.Models;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Binacle.Net.DiagnosticsModule.Configuration.Validators;

internal class OpenTelemetryConfigurationOptionsValidator : AbstractValidator<OpenTelemetryConfigurationOptions>
{
	public OpenTelemetryConfigurationOptionsValidator()
	{
		RuleFor(x => x.Otlp)
			.SetValidator(x => new OtlpExporterConfigurationsOptionsValidator());

		RuleFor(x => x.AzureMonitor)
			.SetValidator(x => new AzureMonitorConfigurationOptionsValidator());
	}

	// For options read while services are registered, which is before ValidateOnStart runs.
	public static void ThrowIfInvalid(OpenTelemetryConfigurationOptions options)
	{
		var result = new OpenTelemetryConfigurationOptionsValidator().Validate(options);
		if (result.IsValid)
		{
			return;
		}

		var errors = result.Errors
			.Select(x => $"Options validation failed of '{x.PropertyName}' with error: '{x.ErrorMessage}'.");
		throw new OptionsValidationException(
			OpenTelemetryConfigurationOptions.SectionName,
			typeof(OpenTelemetryConfigurationOptions),
			errors
		);
	}
}

internal class OtlpExporterConfigurationsOptionsValidator : AbstractValidator<OtlpExporterConfigurationOptions>
{
	public OtlpExporterConfigurationsOptionsValidator()
	{
		When(x =>  !string.IsNullOrEmpty(x.Protocol), () =>
		{
			RuleFor(x => x.Protocol)
				.Must(x => x is "grpc" or "http/protobuf" or "httpProtobuf")
				.WithMessage("The protocol must be either 'grpc' or 'http/protobuf'");
		});

		When(x => x.Enabled && !string.IsNullOrEmpty(x.Endpoint), () =>
		{
			RuleFor(x => x.Endpoint)
				.Must(BeAnHttpUrl)
				.WithMessage("'{PropertyName}' must be an absolute http or https URL, for example 'http://localhost:4317'.");
		});
	}

	// "localhost:4317" parses as a URI with the scheme "localhost", so the scheme has to be checked.
	private static bool BeAnHttpUrl(string? endpoint)
		=> Uri.TryCreate(endpoint, UriKind.Absolute, out var uri)
		   && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
}

internal class AzureMonitorConfigurationOptionsValidator : AbstractValidator<AzureMonitorConfigurationOptions?>
{
	public AzureMonitorConfigurationOptionsValidator()
	{
		When(x => x is not null, () =>
		{
			RuleFor(x => x!.SamplingRatio)
				.InclusiveBetween(0.1f, 1.0f);
		});
	}
}
