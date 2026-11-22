using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net;

public static class DecorateServiceCollectionExtensions
{
	// Wraps the last registration of TService and keeps its lifetime. Each call adds an outer layer.
	public static IServiceCollection Decorate<TService, TDecorator>(this IServiceCollection services)
		where TService : class
		where TDecorator : class, TService
	{
		var index = FindLast(services, typeof(TService));
		var inner = services[index];

		services[index] = ServiceDescriptor.Describe(
			typeof(TService),
			sp => ActivatorUtilities.CreateInstance<TDecorator>(sp, CreateInner(sp, inner)),
			inner.Lifetime
		);

		return services;
	}

	private static int FindLast(IServiceCollection services, Type serviceType)
	{
		for (var i = services.Count - 1; i >= 0; i--)
		{
			if (services[i].ServiceType == serviceType && !services[i].IsKeyedService)
			{
				return i;
			}
		}

		throw new InvalidOperationException($"{serviceType.Name} is not registered, so it cannot be decorated.");
	}

	// The container does not dispose an inner instance built here.
	private static object CreateInner(IServiceProvider sp, ServiceDescriptor inner)
	{
		if (inner.ImplementationInstance is not null)
		{
			return inner.ImplementationInstance;
		}

		if (inner.ImplementationFactory is not null)
		{
			return inner.ImplementationFactory(sp);
		}

		return ActivatorUtilities.CreateInstance(sp, inner.ImplementationType!);
	}
}
