using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Kernel.Endpoints;

public class BindingResult<T>
{
	private readonly IServiceProvider serviceProvider;
	private readonly T? request;
	private readonly Exception? exception;
	private readonly CancellationToken cancellationToken;

	private BindingResult(
		IServiceProvider serviceProvider,
		T? request,
		Exception? exception,
		CancellationToken cancellationToken = default
	)
	{
		this.serviceProvider = serviceProvider;
		this.request = request;
		this.exception = exception;
		this.cancellationToken = cancellationToken;
	}

	public async Task<IResult> ValidateAsync(Func<T, Task<IResult>> handleValidRequest)
	{
		if (this.exception is not null)
		{
			return BindingProblem.For(this.exception, typeof(T));
		}

		if (this.request is null)
		{
			return BindingProblem.MalformedRequest();
		}

		var validator = this.serviceProvider.GetRequiredService<IValidator<T>>();
		var validationResult = await validator.ValidateAsync(this.request, this.cancellationToken);

		if (!validationResult.IsValid)
		{
			return Results.ValidationProblem(
				validationResult.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}

		return await handleValidRequest(this.request);
	}

	public static async ValueTask<BindingResult<T>> BindAsync(HttpContext httpContext)
	{
		try
		{
			var request = await httpContext.Request.ReadFromJsonAsync<T>(httpContext.RequestAborted);
			return new(httpContext.RequestServices, request, null, httpContext.RequestAborted);
		}
		catch (Exception ex)
		{
			return new(httpContext.RequestServices, default, ex, httpContext.RequestAborted);
		}
	}
}
