using Binacle.Net.ServiceModule.Infrastructure.Services;

namespace Binacle.Net.ServiceModule.UnitTests.Tests;

// Known answers, not a round trip. A round trip passes whatever the algorithm is, so it would not notice the
// hash changing - and every password already stored would stop matching.
public class Sha256PasswordHasherTests
{
	// base64 of the bytes 0 to 15.
	private const string Salt = "AAECAwQFBgcICQoLDA0ODw==";
	private const string Password = "hunter2";

	[Fact]
	public void Create_without_a_salt_hashes_the_password_alone()
	{
		var hasher = new Sha256PasswordHasher();

		var password = hasher.Create(Password);

		password.Type.ShouldBe("SHA256");
		password.Salt.ShouldBeNull();
		password.Hash.ShouldBe("9S+9MrKzuG/4jvbEkGKChfSCrxXdyylUH5S89Saj9sc=");
	}

	[Fact]
	public void Create_with_a_salt_hashes_the_password_then_the_salt()
	{
		var hasher = new Sha256PasswordHasher();

		var password = hasher.Create(Password, Salt);

		password.Type.ShouldBe("SHA256");
		password.Salt.ShouldBe(Salt);
		password.Hash.ShouldBe("8y6r8IvvkSoryC9GYE7mSHUKm98Z0CNjORkKxugeJJg=");
	}

	[Fact]
	public void Create_treats_an_empty_salt_as_no_salt()
	{
		var hasher = new Sha256PasswordHasher();

		hasher.Create(Password, "").Hash.ShouldBe(hasher.Create(Password).Hash);
	}

	[Fact]
	public void GenerateSalt_returns_16_fresh_bytes()
	{
		var hasher = new Sha256PasswordHasher();

		var salt = hasher.GenerateSalt();

		Convert.FromBase64String(salt).Length.ShouldBe(16);
		salt.ShouldNotBe(hasher.GenerateSalt());
	}
}
