using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace Binacle.Net.UIModule.Pages;

internal class ErrorModel : PageModel
{
	private readonly IWebHostEnvironment environment;

	public ErrorModel(IWebHostEnvironment environment)
	{
		this.environment = environment;
	}

	public string Title { get; private set; } = "Error";
	public string Message { get; private set; } = "Something went wrong while handling your request.";
	public string? RequestId { get; private set; }

	public void OnGet(string? errorCode)
	{
		// 400 to 599 is what the re-execute can send here, and the only range worth answering with. Anything
		// else is someone typing in the address bar, and a made-up status code is not a reply.
		if (int.TryParse(errorCode, out var statusCode) && statusCode is >= 400 and <= 599)
		{
			this.Title = $"Error {statusCode}";
			this.Message = MessageFor(statusCode);

			// A direct hit used to answer 200, so a monitor pointed at this address was told all was well.
			// On the re-execute path the status is already this, and setting it again changes nothing.
			this.Response.StatusCode = statusCode;
		}

		// Only in development: the trace id is a detail of this instance, not something a visitor needs.
		if (this.environment.IsDevelopment())
		{
			this.RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier;
		}
	}

	private static string MessageFor(int statusCode) => statusCode switch
	{
		404 => "That page does not exist. Check the address, or start again from the home page.",
		403 => "You do not have access to that page.",
		500 => "Something went wrong on this instance. Try again, and check the server log if it keeps happening.",
		_ => "Something went wrong while handling your request."
	};
}
