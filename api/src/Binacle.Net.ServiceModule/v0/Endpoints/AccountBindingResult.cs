using Binacle.Net.Kernel.Endpoints;
using Binacle.Net.ServiceModule.Domain.Accounts.Entities;
using Binacle.Net.ServiceModule.Domain.Accounts.Services;
using Binacle.Net.ServiceModule.v0.Contracts.Common;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.ServiceModule.v0.Endpoints;

internal class AccountBindingResult<T>
{
	private readonly IServiceProvider serviceProvider;
	private readonly T? request;
	private readonly Exception? exception;
	private readonly CancellationToken cancellationToken;

	private AccountBindingResult(
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

	public async Task<IResult> ValidateAsync(
		AccountId id, 
		Func<T, Domain.Accounts.Entities.Account, Task<IResult>> handleRequest
		)
	{
		var accountIdValidator = this.serviceProvider.GetRequiredService<IValidator<AccountId>>();

		
		var accountIdValidationResult = await accountIdValidator.ValidateAsync(id, this.cancellationToken);
		if (!accountIdValidationResult.IsValid)
		{
			return Results.ValidationProblem(
				accountIdValidationResult.GetValidationSummary(),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}
		
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

		var accountRepository = this.serviceProvider.GetRequiredService<IAccountRepository>();
		var accountResult = await accountRepository.GetByIdAsync(
			id.Value,
			cancellationToken:this.cancellationToken
		);
		
		var result = accountResult.Match(
			account => handleRequest(this.request, account),
			notFound => Task.FromResult<IResult>(Results.NotFound())
		);
		return await result;
	}


	public static async ValueTask<AccountBindingResult<T>> BindAsync(HttpContext httpContext)
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


