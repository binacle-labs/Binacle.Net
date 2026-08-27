using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Binacle.Net.ServiceModule.Domain.Subscriptions.Models;
using Binacle.Net.ServiceModule.IntegrationTests.ExtensionMethods;
using Binacle.Net.ServiceModule.IntegrationTests.Models;
using Binacle.Net.ServiceModule.v0.Contracts.Admin;

namespace Binacle.Net.ServiceModule.IntegrationTests.Endpoints.Admin.Subscription;

[Trait("Endpoint Tests", "Endpoint Integration tests")]
public class Patch : AdminEndpointsTestsBase
{
	private readonly AccountCredentialsWithSubscription accountCredentialsUnderTest;

	public Patch(BinacleApi sut) : base(sut)
	{
		this.accountCredentialsUnderTest = new AccountCredentialsWithSubscription(
			Guid.Parse("76D9513B-1893-4866-BDF8-3D754E337758"),
			"subscriptionpatchuser@test.binacle.net",
			"subscriptionpatchuser@test.binacle.net",
			"SubscriptionP4tchUs3ersP@ssw0rd",
			Guid.Parse("6FA0D0B1-6C83-4393-BA44-A99CB6344AD0")
		);
	}

	private const string routePath = "/api/admin/account/{id}/subscription";

	#region 401 Unauthorized

	[Fact(DisplayName = $"PATCH {routePath}. Without Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithoutBearerToken_Returns_401Unauthorized()
		=> this.Action_WithoutBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionPatchRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Client.PatchAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"PATCH {routePath}. With Expired Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithExpiredBearerToken_Returns_401Unauthorized()
		=> this.Action_WithExpiredBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionPatchRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Client.PatchAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});

	[Fact(DisplayName = $"PATCH {routePath}. With Wrong Issuer Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithWrongIssuerBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionPatchRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Client.PatchAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"PATCH {routePath}. With Wrong Audience Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithWrongAudienceBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWrongIssuerBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionPatchRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Client.PatchAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});


	[Fact(DisplayName = $"PATCH {routePath}. With Wrongly Signed Bearer Token Returns 401 Unauthorized")]
	public Task Patch_WithWronglySignedBearerToken_Returns_401Unauthorized()
		=> this.Action_WithWronglySignedBearerToken_Returns_401Unauthorized(async () =>
		{
			var request = new SubscriptionPatchRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Client.PatchAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 403 Forbidden

	[Fact(DisplayName = $"PATCH {routePath}. Without Admin Bearer Token Returns 403 Forbidden")]
	public Task Patch_WithoutAdminBearerToken_Returns_403Forbidden()
		=> this.Action_WithoutAdminBearerToken_Returns_403Forbidden(async () =>
		{
			var request = new SubscriptionPatchRequest()
			{
				Type = SubscriptionType.Normal,
				Status = SubscriptionStatus.Active
			};
			var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

			return await this.Client.PatchAsJsonAsync(
				url,
				request,
				this.Sut.JsonSerializerOptions, 
				TestContext.Current.CancellationToken
			);
		});

	#endregion

	#region 204 No Content

	[Fact(DisplayName = $"PATCH {routePath}. With Full Valid Request Returns 204 No Content")]
	public async Task Patch_WithFullValidRequest_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var request = new SubscriptionPatchRequest()
		{
			Type = SubscriptionType.Normal,
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Client.PatchAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
	}
	
	
	[Fact(DisplayName = $"PATCH {routePath}. With Status Only Returns 204 No Content")]
	public async Task Patch_WithStatusOnly_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var request = new SubscriptionPatchRequest()
		{
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Client.PatchAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
	}
	
	[Fact(DisplayName = $"PATCH {routePath}. With Type Only Returns 204 No Content")]
	public async Task Patch_WithTypeOnly_Returns_204NoContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var request = new SubscriptionPatchRequest()
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Client.PatchAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
	}

	#endregion

	#region 404 Not Found

	[Fact(DisplayName = $"PATCH {routePath}. For Non Existing Account Returns 404 Not Found")]
	public async Task Patch_ForNonExistingAccount_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var request = new SubscriptionPatchRequest()
		{
			Type = SubscriptionType.Normal,
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", this.Sut.NonExistentId.ToString());

		var response = await this.Client.PatchAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	[Fact(DisplayName = $"PATCH {routePath}. For Account Without Subscription Returns 404 Not Found")]
	public async Task Patch_ForAccountWithoutSubscription_Returns_404NotFound()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);

		var request = new SubscriptionPatchRequest()
		{
			Type = SubscriptionType.Normal,
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", this.Sut.ExistingAccountCredentials.Id.ToString());

		var response = await this.Client.PatchAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
	}

	#endregion

	#region 422 Unprocessable Content

	[Fact(DisplayName = $"PATCH {routePath}. With Invalid Id Returns 422 Unprocessable Content")]
	public async Task Patch_WithInvalidId_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var request = new SubscriptionPatchRequest()
		{
			Type = SubscriptionType.Normal,
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", "invalid");

		var response = await this.Client.PatchAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"PATCH {routePath}. With Invalid Subscription Type Returns 422 Unprocessable Content")]
	public async Task Patch_WithInvalidSubscriptionType_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var request = new SubscriptionPatchRequest
		{
			Type = null,
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Client.PatchAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}
	
	[Fact(DisplayName = $"PATCH {routePath}. With Invalid Subscription Status Returns 422 Unprocessable Content")]
	public async Task Patch_WithInvalidSubscriptionStatus_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var request = new SubscriptionPatchRequest
		{
			Status = null
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.Client.PatchAsJsonAsync(
			url,
			request,
			this.Sut.JsonSerializerOptions, 
			TestContext.Current.CancellationToken
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	[Fact(DisplayName = $"PATCH {routePath}. With Unknown Subscription Type Returns 422 Unprocessable Content")]
	public async Task Patch_WithUnknownSubscriptionType_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var request = new SubscriptionPatchRequest
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.SendWithFieldValueAsync(
			HttpMethod.Patch,
			url,
			request,
			nameof(SubscriptionPatchRequest.Type),
			JsonValue.Create("invalid")
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	[Fact(DisplayName = $"PATCH {routePath}. With Unknown Subscription Status Returns 422 Unprocessable Content")]
	public async Task Patch_WithUnknownSubscriptionStatus_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var request = new SubscriptionPatchRequest
		{
			Status = SubscriptionStatus.Active
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.SendWithFieldValueAsync(
			HttpMethod.Patch,
			url,
			request,
			nameof(SubscriptionPatchRequest.Status),
			JsonValue.Create("invalid")
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	[Fact(DisplayName = $"PATCH {routePath}. With Numeric Subscription Type Returns 422 Unprocessable Content")]
	public async Task Patch_WithNumericSubscriptionType_Returns_422UnprocessableContent()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var request = new SubscriptionPatchRequest
		{
			Type = SubscriptionType.Normal
		};
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var response = await this.SendWithFieldValueAsync(
			HttpMethod.Patch,
			url,
			request,
			nameof(SubscriptionPatchRequest.Type),
			JsonValue.Create(1)
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);
	}

	// Read back rather than trusting the status code: the valid field alone would satisfy the validator, and a
	// patch that applied with the type dropped would still answer 422 on the field it dropped.
	[Fact(DisplayName = $"PATCH {routePath}. With Unknown Type And Valid Status, Returns 422 And Leaves The Subscription Unchanged")]
	public async Task Patch_WithUnknownTypeAndValidStatus_Returns_422AndLeavesTheSubscriptionUnchanged()
	{
		await using var scope = this.Sut.StartAuthenticationScope(this.Client, this.Sut.Admin);
		var url = routePath.Replace("{id}", this.accountCredentialsUnderTest.Id.ToString());

		var before = await this.GetSubscription(url);

		var request = new SubscriptionPatchRequest
		{
			Type = SubscriptionType.Normal,
			Status = SubscriptionStatus.Suspended
		};

		var response = await this.SendWithFieldValueAsync(
			HttpMethod.Patch,
			url,
			request,
			nameof(SubscriptionPatchRequest.Type),
			JsonValue.Create("invalid")
		);
		response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableContent);

		var after = await this.GetSubscription(url);
		after.Type.ShouldBe(before.Type);
		after.Status.ShouldBe(before.Status);
	}

	#endregion
	
	public override async ValueTask InitializeAsync()
	{
		await this.Sut.EnsureAccountExistsWithSubscription(this.accountCredentialsUnderTest);
		await base.InitializeAsync();
	}

	public override async ValueTask DisposeAsync()
	{
		await this.Sut.EnsureAccountWithSubscriptionDoesNotExist(this.accountCredentialsUnderTest);
		await base.DisposeAsync();
	}

	private async Task<SubscriptionGetResponse> GetSubscription(string url)
	{
		var response = await this.Client.GetAsync(url, TestContext.Current.CancellationToken);
		response.StatusCode.ShouldBe(HttpStatusCode.OK);

		var subscription = await response.Content.ReadFromJsonAsync<SubscriptionGetResponse>(
			this.Sut.JsonSerializerOptions,
			TestContext.Current.CancellationToken
		);
		subscription.ShouldNotBeNull();
		return subscription;
	}
}
