using System.Text.Json;
using Binacle.Net.Kernel.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Binacle.Net.Kernel.Endpoints;

// The one place a failed bind turns into a response, so an unknown enum cannot answer 422 on one route and 400
// on the next. JsonEnumValueException is a JsonException, so it has to be matched first.
public static class BindingProblem
{
	// Read as JSON but came back null - "null" as the whole body, or a payload that maps to nothing.
	public static IResult MalformedRequest()
	{
		return Results.Problem(new ProblemDetails
		{
			Status = StatusCodes.Status400BadRequest,
			Title = "Malformed Request",
			Detail = "The server could not process the request because it is malformed or contains invalid data. Please verify the request format and try again.",
		});
	}

	public static IResult For(Exception exception, Type requestType)
	{
		if (exception is JsonEnumValueException enumException)
		{
			return Results.ValidationProblem(
				enumException.GetValidationSummary(requestType),
				statusCode: StatusCodes.Status422UnprocessableEntity
			);
		}

		if (exception is JsonException jsonException)
		{
			return Results.Problem(new ProblemDetails
			{
				Status = StatusCodes.Status400BadRequest,
				Title = "Invalid JSON Format",
				Detail = jsonException.Message,
			});
		}

		var problemDetails = new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "Unexpected Server Error",
			Detail = "An unexpected error occurred while processing your request. Please try again later or contact support.",
		};

		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			problemDetails.Extensions.TryAdd("exception", exception.GetType().Name);
			problemDetails.Extensions.TryAdd("message", exception.Message);
			problemDetails.Extensions.TryAdd("stackTrace", exception.StackTrace);
		}

		return Results.Problem(problemDetails);
	}
}
