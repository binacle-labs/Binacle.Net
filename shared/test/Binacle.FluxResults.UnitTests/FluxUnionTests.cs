namespace Binacle.FluxResults.UnitTests;

public class FluxUnionTests
{
	private sealed record Account(string Id);

	private static readonly Account TheAccount = new("acc-1");

	[Fact]
	public void An_arm_converts_into_the_union_implicitly()
	{
		FluxUnion<Account, NotFound> found = TheAccount;
		FluxUnion<Account, NotFound> missing = TypedResult.NotFound;

		found.GetValueType().ShouldBe(typeof(Account));
		found.GetValue().ShouldBe(TheAccount);

		missing.GetValueType().ShouldBe(typeof(NotFound));
		missing.GetValue().ShouldBe(TypedResult.NotFound);
	}

	[Fact]
	public void Match_runs_the_arm_the_union_holds()
	{
		FluxUnion<Account, NotFound> found = TheAccount;
		FluxUnion<Account, NotFound> missing = TypedResult.NotFoundWith("gone");

		found.Match(a => a.Id, n => n.Message ?? "none").ShouldBe("acc-1");
		missing.Match(a => a.Id, n => n.Message ?? "none").ShouldBe("gone");
	}

	[Fact]
	public void Is_compares_the_exact_type_only()
	{
		FluxUnion<Account, NotFound> missing = TypedResult.NotFound;

		missing.Is<NotFound>().ShouldBeTrue();
		missing.Is<Account>().ShouldBeFalse();
		missing.Is<object>().ShouldBeFalse();
		missing.Is<IErrorTypedResult>().ShouldBeFalse();
	}

	[Fact]
	public void As_returns_the_value_it_holds()
	{
		FluxUnion<Account, NotFound> found = TheAccount;

		found.As<Account>().ShouldBe(TheAccount);
	}

	// A struct arm's miss looks like a real value. TryGetValue is what tells them apart.
	[Fact]
	public void As_returns_default_on_a_miss()
	{
		FluxUnion<Account, NotFound> found = TheAccount;

		found.As<NotFound>().ShouldBe(default(NotFound));
		found.As<string>().ShouldBeNull();
	}

	[Fact]
	public void TryGetValue_says_which_arm_it_is()
	{
		FluxUnion<Account, NotFound> found = TheAccount;

		found.TryGetValue<Account>(out var account).ShouldBeTrue();
		account.ShouldBe(TheAccount);

		found.TryGetValue<NotFound>(out var notFound).ShouldBeFalse();
		notFound.ShouldBe(default(NotFound));
	}

	[Fact]
	public void Unwrap_returns_the_value_it_holds()
	{
		FluxUnion<Account, NotFound> found = TheAccount;

		found.Unwrap<Account>().ShouldBe(TheAccount);
	}

	[Fact]
	public void Unwrap_throws_on_the_wrong_arm_and_names_both_types()
	{
		FluxUnion<Account, NotFound> found = TheAccount;

		var exception = Should.Throw<InvalidOperationException>(() => found.Unwrap<NotFound>());

		exception.Message.ShouldContain(nameof(Account));
		exception.Message.ShouldContain(nameof(NotFound));
	}

	// The flux enum defaults to 0, so an unset union claims T0 and hands back null.
	[Fact]
	public void A_default_union_claims_the_first_arm_and_holds_null()
	{
		var union = default(FluxUnion<Account, NotFound>);

		union.GetValueType().ShouldBe(typeof(Account));
		union.GetValue().ShouldBeNull();
		union.Is<Account>().ShouldBeTrue();
		union.As<Account>().ShouldBeNull();
	}
}
