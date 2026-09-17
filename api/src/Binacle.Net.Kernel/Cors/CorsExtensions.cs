using Binacle.Net.Kernel.Validation.Services;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using AspNetCorsOptions = Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions;

namespace Binacle.Net.Kernel.Cors;

public static class CorsExtensions
{
	// Adds a CORS file and its environment variant to the configuration. Optional: no file is a closed policy.
	// Every file feeds the one Cors section, so an owner's keys sit beside the others'.
	public static TBuilder AddCorsFile<TBuilder>(this TBuilder builder, string filePath)
		where TBuilder : IHostApplicationBuilder
	{
		var extension = Path.GetExtension(filePath);
		var environmentFilePath = Path.ChangeExtension(filePath, $"{builder.Environment.EnvironmentName}{extension}");

		builder.Configuration.AddJsonFile(filePath, optional: true, reloadOnChange: false);
		builder.Configuration.AddJsonFile(environmentFilePath, optional: true, reloadOnChange: false);
		builder.Configuration.AddEnvironmentVariables();

		return builder;
	}

	// Registers one named policy, built from the Cors section when first used. A missing key is a policy that
	// allows nothing - a valid closed default, not an error. The section is bound and validated on start once,
	// however many owners call this.
	public static IServiceCollection AddCorsPolicy(this IServiceCollection services, string name)
	{
		services.Configure<CorsRegisteredPolicies>(registered => registered.Names.Add(name));

		// Each added once, by implementation type. The options-builder chain has no TryAdd form, so it is
		// spelled out.
		services.TryAddSingleton<IValidator<CorsOptions>, CorsOptionsValidator>();
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<CorsOptions>, ConfigureFromConfigurationOptions<CorsOptions>>(BindCorsSection));
		services.TryAddEnumerable(ServiceDescriptor.Singleton<IValidateOptions<CorsOptions>, FluentValidationOptions<CorsOptions>>(ValidateCorsOptions));
		services.AddOptions<CorsOptions>().ValidateOnStart();

		services.AddCors();
		services
			.AddOptions<AspNetCorsOptions>()
			.Configure<IOptions<CorsOptions>>((cors, bound) =>
			{
				cors.AddPolicy(name, policy =>
				{
					policy.WithOrigins(bound.Value.Policy(name)?.AllowedOrigins ?? [])
						.AllowAnyHeader()
						.AllowAnyMethod();
				});
			});

		return services;
	}

	private static ConfigureFromConfigurationOptions<CorsOptions> BindCorsSection(IServiceProvider provider)
	{
		var section = provider.GetRequiredService<IConfiguration>().GetSection(CorsOptions.SectionName);
		return new ConfigureFromConfigurationOptions<CorsOptions>(section);
	}

	private static FluentValidationOptions<CorsOptions> ValidateCorsOptions(IServiceProvider provider)
	{
		var validator = provider.GetRequiredService<IValidator<CorsOptions>>();
		return new FluentValidationOptions<CorsOptions>(Options.DefaultName, validator);
	}
}
