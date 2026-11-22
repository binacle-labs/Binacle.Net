using Microsoft.Extensions.DependencyInjection;

namespace Binacle.Net.Kernel.UnitTests.Dependencies;

[Trait("Behavioral Tests", "Ensures Decorate wraps a registration in order and keeps its lifetime")]
public class DecorateTests
{
	[Fact]
	public void The_last_decorator_is_the_outermost()
	{
		var services = new ServiceCollection()
			.AddSingleton<IGreeter, Greeter>()
			.Decorate<IGreeter, Inner>()
			.Decorate<IGreeter, Outer>()
			.BuildServiceProvider();

		services.GetRequiredService<IGreeter>().Greet().ShouldBe("outer(inner(hello))");
	}

	[Fact]
	public void A_decorator_gets_its_other_arguments_from_the_container()
	{
		var services = new ServiceCollection()
			.AddSingleton(new Suffix("!"))
			.AddSingleton<IGreeter, Greeter>()
			.Decorate<IGreeter, Suffixed>()
			.BuildServiceProvider();

		services.GetRequiredService<IGreeter>().Greet().ShouldBe("hello!");
	}

	[Theory]
	[InlineData(ServiceLifetime.Singleton)]
	[InlineData(ServiceLifetime.Scoped)]
	[InlineData(ServiceLifetime.Transient)]
	public void The_lifetime_is_kept(ServiceLifetime lifetime)
	{
		IServiceCollection services = new ServiceCollection();
		services.Add(ServiceDescriptor.Describe(typeof(IGreeter), typeof(Greeter), lifetime));

		services.Decorate<IGreeter, Inner>();

		services.ShouldHaveSingleItem().Lifetime.ShouldBe(lifetime);
	}

	[Fact]
	public void A_factory_registration_can_be_decorated()
	{
		var services = new ServiceCollection()
			.AddSingleton<IGreeter>(_ => new Greeter())
			.Decorate<IGreeter, Inner>()
			.BuildServiceProvider();

		services.GetRequiredService<IGreeter>().Greet().ShouldBe("inner(hello)");
	}

	[Fact]
	public void A_missing_registration_throws()
	{
		var services = new ServiceCollection();

		Should.Throw<InvalidOperationException>(() => services.Decorate<IGreeter, Inner>())
			.Message.ShouldContain(nameof(IGreeter));
	}

	public interface IGreeter
	{
		string Greet();
	}

	private sealed class Greeter : IGreeter
	{
		public string Greet() => "hello";
	}

	private sealed class Inner(IGreeter inner) : IGreeter
	{
		public string Greet() => $"inner({inner.Greet()})";
	}

	private sealed class Outer(IGreeter inner) : IGreeter
	{
		public string Greet() => $"outer({inner.Greet()})";
	}

	public sealed record Suffix(string Value);

	private sealed class Suffixed(IGreeter inner, Suffix suffix) : IGreeter
	{
		public string Greet() => inner.Greet() + suffix.Value;
	}
}
