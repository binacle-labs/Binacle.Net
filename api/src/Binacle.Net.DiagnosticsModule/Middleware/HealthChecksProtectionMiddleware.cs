using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Binacle.Net.DiagnosticsModule.Configuration.Models;
using Binacle.Net.Kernel.Network;

namespace Binacle.Net.DiagnosticsModule.Middleware;

internal class HealthChecksProtectionMiddleware
{
	internal const string AccessKeyHeader = "X-Health-Key";

	private readonly RequestDelegate next;
	private readonly ILogger<HealthChecksProtectionMiddleware> logger;
	private readonly IOptions<HealthCheckConfigurationOptions> options;
	private readonly HealthCheckAllowList allowList;
	// Hashed so the fixed-time compare also hides the key's length.
	private readonly byte[]? accessKeyHash;

	public HealthChecksProtectionMiddleware(
		RequestDelegate next,
		ILogger<HealthChecksProtectionMiddleware> logger,
		IOptions<HealthCheckConfigurationOptions> options
	)
	{
		this.next = next;
		this.logger = logger;
		this.options = options;
		// Built once, because HealthChecks.json is not reloaded.
		this.allowList = this.BuildAllowList(options.Value.RestrictedIPs);
		this.accessKeyHash = string.IsNullOrEmpty(options.Value.AccessKey)
			? null
			: SHA256.HashData(Encoding.UTF8.GetBytes(options.Value.AccessKey));
	}

	private bool AccessKeyMatches(string? sent)
	{
		var sentHash = SHA256.HashData(Encoding.UTF8.GetBytes(sent ?? string.Empty));
		return CryptographicOperations.FixedTimeEquals(sentHash, this.accessKeyHash);
	}

	internal HealthCheckAllowList BuildAllowList(string?[]? entries)
	{
		if (entries is null || entries.Length == 0)
		{
			return new HealthCheckAllowList([]);
		}

		var networks = new IPNetwork[entries.Length];

		for (var index = 0; index < entries.Length; index++)
		{
			var entry = entries[index];

			// Startup validation rejects a malformed entry, so reaching this is a bug. Throwing beats keeping the
			// default network, which has no base address and would error every health request, and beats
			// dropping the entry, which would quietly widen or narrow the allow-list. The position is in the
			// message because the entry itself can be null or blank.
			if (!IPEntry.TryParse(entry, out var network))
			{
				throw new InvalidOperationException(
					$"Invalid health check RestrictedIPs entry at position {index}: '{entry}'. Use a single address " +
					"such as 192.168.1.1, or CIDR notation such as 192.168.1.0/24"
				);
			}

			networks[index] = network;

			// "192.168.1.1/24" is the whole 192.168.1.0/24, 256 hosts. Every parser in .NET masks the host bits
			// off in silence; this one says what it ended up with.
			if (entry!.Contains('/') && entry.Trim() != network.ToString())
			{
				this.logger.LogWarning(
					"Health check RestrictedIPs entry {Entry} covers the whole network {Network}", entry, network
				);
			}
		}

		return new HealthCheckAllowList(networks);
	}

	public async Task InvokeAsync(HttpContext context)
	{
		// Ignore if the request is not for the health checks path
		if (!context.Request.Path.StartsWithSegments(this.options.Value.Path))
		{
			await next(context);
			return;
		}

		if (!this.IsAllowed(context))
		{
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			return;
		}

		await next(context);
	}

	private bool IsAllowed(HttpContext context)
	{
		if (this.accessKeyHash is null && this.allowList.RestrictsNobody)
		{
			return true;
		}

		// With no key configured the header is ignored.
		if (this.accessKeyHash is not null && context.Request.Headers.TryGetValue(AccessKeyHeader, out var sentKey))
		{
			return this.KeyAllows(sentKey, context.Connection.RemoteIpAddress);
		}

		return this.AddressAllows(context.Connection.RemoteIpAddress);
	}

	// A wrong key is refused even from an allowed address.
	private bool KeyAllows(StringValues sentKey, IPAddress? remoteIp)
	{
		if (sentKey.Count == 1 && this.AccessKeyMatches(sentKey[0]))
		{
			return true;
		}

		logger.LogWarning("Health check request from {RemoteIp} sent a wrong access key", remoteIp);
		return false;
	}

	// Behind a proxy this is the caller only once the forwarded headers middleware has resolved it; without
	// that it is the proxy, and no operator address will ever match.
	private bool AddressAllows(IPAddress? remoteIp)
	{
		if (remoteIp is null)
		{
			logger.LogWarning("Health check request from unknown remote IP");
			return false;
		}

		if (!this.allowList.Allows(remoteIp))
		{
			logger.LogWarning("Health check request from {RemoteIp} is not allowed", remoteIp);
			return false;
		}

		return true;
	}

	// The parsed RestrictedIPs. Nested rather than file-scoped: CS9051 forbids a file-local type in a member
	// signature of a non-file-local type, which the field and BuildAllowList both are.
	internal sealed class HealthCheckAllowList
	{
		private readonly IPNetwork[] networks;

		public HealthCheckAllowList(IPNetwork[] networks)
		{
			this.networks = networks;
		}

		// An empty list is not "allow nobody" - it is the default deployment, where the health check is open.
		public bool RestrictsNobody => this.networks.Length == 0;

		public bool Allows(IPAddress caller)
		{
			var callerAddress = IPEntry.Normalize(caller);

			foreach (var network in this.networks)
			{
				if (network.Contains(callerAddress))
				{
					return true;
				}
			}

			return false;
		}
	}
}
