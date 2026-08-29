namespace Binacle.FluxResults.UnitTests;

public class TypedResultTests
{
	public static TheoryData<ITypedResult> ErrorResults =>
	[
		TypedResult.Conflict,
		TypedResult.NotFound,
		TypedResult.ValidationError,
		TypedResult.Unauthorized,
		TypedResult.Forbidden,
		TypedResult.UnexpectedError
	];

	[Fact]
	public void A_bare_result_carries_no_message()
	{
		TypedResult.Success.Message.ShouldBeNull();
		TypedResult.NotFound.Message.ShouldBeNull();
		TypedResult.Conflict.Message.ShouldBeNull();
	}

	[Fact]
	public void A_with_result_carries_the_message()
	{
		TypedResult.SuccessWith("saved").Message.ShouldBe("saved");
		TypedResult.NotFoundWith("no such account").Message.ShouldBe("no such account");
		TypedResult.ConflictWith("already exists").Message.ShouldBe("already exists");
	}

	[Fact]
	public void Two_results_with_the_same_message_are_equal()
	{
		TypedResult.NotFound.ShouldBe(new NotFound(null));
		TypedResult.NotFoundWith("gone").ShouldBe(new NotFound("gone"));
		TypedResult.NotFoundWith("gone").ShouldNotBe(new NotFound("missing"));
	}

	[Theory]
	[MemberData(nameof(ErrorResults))]
	public void An_error_result_is_an_IErrorTypedResult(ITypedResult result)
	{
		result.ShouldBeAssignableTo<IErrorTypedResult>();
	}

	[Fact]
	public void A_successful_result_is_an_ISuccessfulTypedResult()
	{
		TypedResult.Success.ShouldBeAssignableTo<ISuccessfulTypedResult>();
		TypedResult.Created.ShouldBeAssignableTo<ISuccessfulTypedResult>();
		TypedResult.Deleted.ShouldBeAssignableTo<ISuccessfulTypedResult>();
		TypedResult.Updated.ShouldBeAssignableTo<ISuccessfulTypedResult>();
	}

	[Fact]
	public void A_successful_result_is_not_an_error()
	{
		TypedResult.Success.ShouldNotBeAssignableTo<IErrorTypedResult>();
		TypedResult.Created.ShouldNotBeAssignableTo<IErrorTypedResult>();
		TypedResult.Deleted.ShouldNotBeAssignableTo<IErrorTypedResult>();
		TypedResult.Updated.ShouldNotBeAssignableTo<IErrorTypedResult>();
	}
}
