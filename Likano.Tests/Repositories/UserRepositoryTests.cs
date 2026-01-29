using Likano.Domain.Entities;
using Likano.Domain.Enums.User;
using Likano.Infrastructure.Data;
using Likano.Infrastructure.Repositories;
using Likano.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Likano.Tests.Repositories
{
    [TestFixture]
    public class UserRepositoryTests
    {
        ApplicationDbContext _db = null!;
        UserRepository _repo = null!;
        IHttpContextAccessor _httpContextAccessor = null!;
        int _userId;

        [SetUp]
        public async Task SetUp()
        {
            _db = DbContextHelper.GetInMemoryDbContext();
            _httpContextAccessor = new HttpContextAccessor();
            _repo = new UserRepository(_db, _httpContextAccessor);

            var user = new User
            {
                UserName = "testuser",
                Password = "hashedpassword123",
                UserType = UserType.User,
                RefreshToken = null,
                RefreshTokenExpiry = null
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            _userId = user.Id;
        }

        [TearDown]
        public void TearDown()
        {
            _db.Dispose();
        }

        #region UserNameExists Tests

        [Test]
        public async Task UserNameExists_ExistingUser_ReturnsTrue()
        {
            var result = await _repo.UserNameExists("testuser");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UserNameExists_CaseInsensitive_ReturnsTrue()
        {
            var result = await _repo.UserNameExists("TESTUSER");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UserNameExists_MixedCase_ReturnsTrue()
        {
            var result = await _repo.UserNameExists("TestUser");

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UserNameExists_NonExistingUser_ReturnsFalse()
        {
            var result = await _repo.UserNameExists("nonexistinguser");

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UserNameExists_EmptyString_ReturnsFalse()
        {
            var result = await _repo.UserNameExists("");

            Assert.That(result, Is.False);
        }

        #endregion

        #region GetByIdAsync Tests

        [Test]
        public async Task GetByIdAsync_ExistingUser_ReturnsUser()
        {
            var result = await _repo.GetByIdAsync(_userId);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserName, Is.EqualTo("testuser"));
            Assert.That(result.UserType, Is.EqualTo(UserType.User));
        }

        [Test]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var result = await _repo.GetByIdAsync(9999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdAsync_ZeroId_ReturnsNull()
        {
            var result = await _repo.GetByIdAsync(0);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdAsync_NegativeId_ReturnsNull()
        {
            var result = await _repo.GetByIdAsync(-1);

            Assert.That(result, Is.Null);
        }

        #endregion

        #region GetByUserNameAsync Tests

        [Test]
        public async Task GetByUserNameAsync_ExistingUser_ReturnsUser()
        {
            var result = await _repo.GetByUserNameAsync("testuser");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserName, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task GetByUserNameAsync_CaseInsensitive_ReturnsUser()
        {
            var result = await _repo.GetByUserNameAsync("TESTUSER");

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserName, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task GetByUserNameAsync_NonExistingUser_ReturnsNull()
        {
            var result = await _repo.GetByUserNameAsync("nonexisting");

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByUserNameAsync_EmptyString_ReturnsNull()
        {
            var result = await _repo.GetByUserNameAsync("");

            Assert.That(result, Is.Null);
        }

        #endregion

        #region Register Tests

        [Test]
        public async Task Register_ValidUser_ReturnsNewUser()
        {
            var newUser = new User
            {
                UserName = "newuser",
                Password = "hashedpassword456"
            };

            var result = await _repo.Register(newUser);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.UserName, Is.EqualTo("newuser"));
            Assert.That(result.Password, Is.EqualTo("hashedpassword456"));
            Assert.That(result.UserType, Is.EqualTo(UserType.User));
            Assert.That(result.RefreshToken, Is.Null);
            Assert.That(result.RefreshTokenExpiry, Is.Null);
        }

        [Test]
        public async Task Register_ValidUser_SavesUserToDatabase()
        {
            var newUser = new User
            {
                UserName = "anotheruser",
                Password = "password789"
            };

            var result = await _repo.Register(newUser);

            var savedUser = await _db.Users.FindAsync(result!.Id);
            Assert.That(savedUser, Is.Not.Null);
            Assert.That(savedUser!.UserName, Is.EqualTo("anotheruser"));
        }

        [Test]
        public async Task Register_SetsDefaultUserType()
        {
            var newUser = new User
            {
                UserName = "defaultuser",
                Password = "password",
                UserType = UserType.Admin // This should be overridden
            };

            var result = await _repo.Register(newUser);

            Assert.That(result!.UserType, Is.EqualTo(UserType.User));
        }

        #endregion

        #region UpdateRefreshTokenAsync Tests

        [Test]
        public async Task UpdateRefreshTokenAsync_UpdatesTokenAndExpiry()
        {
            var user = await _db.Users.FindAsync(_userId);
            var refreshToken = "new_refresh_token";
            var expiry = DateTime.UtcNow.AddDays(7);

            await _repo.UpdateRefreshTokenAsync(user!, refreshToken, expiry);

            var updatedUser = await _db.Users.FindAsync(_userId);
            Assert.That(updatedUser!.RefreshToken, Is.EqualTo(refreshToken));
            Assert.That(updatedUser.RefreshTokenExpiry, Is.EqualTo(expiry));
        }

        [Test]
        public async Task UpdateRefreshTokenAsync_CanUpdateMultipleTimes()
        {
            var user = await _db.Users.FindAsync(_userId);

            await _repo.UpdateRefreshTokenAsync(user!, "token1", DateTime.UtcNow.AddDays(1));
            await _repo.UpdateRefreshTokenAsync(user!, "token2", DateTime.UtcNow.AddDays(2));

            var updatedUser = await _db.Users.FindAsync(_userId);
            Assert.That(updatedUser!.RefreshToken, Is.EqualTo("token2"));
        }

        #endregion

        #region GetUser Tests

        [Test]
        public void GetUser_NoHttpContext_ReturnsNull()
        {
            var result = _repo.GetUser();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUser_WithHttpContext_ReturnsClaimsPrincipal()
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = principal };
            _httpContextAccessor.HttpContext = httpContext;

            var result = _repo.GetUser();

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Identity!.IsAuthenticated, Is.True);
        }

        #endregion

        #region GetUserId Tests

        [Test]
        public void GetUserId_NoHttpContext_ReturnsNull()
        {
            var result = _repo.GetUserId();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserId_NotAuthenticated_ReturnsNull()
        {
            var httpContext = new DefaultHttpContext();
            _httpContextAccessor.HttpContext = httpContext;

            var result = _repo.GetUserId();

            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserId_WithNameIdentifierClaim_ReturnsUserId()
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, _userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = principal };
            _httpContextAccessor.HttpContext = httpContext;

            var result = _repo.GetUserId();

            Assert.That(result, Is.EqualTo(_userId));
        }

        [Test]
        public void GetUserId_WithSubClaim_ReturnsUserId()
        {
            var claims = new[] { new Claim("sub", _userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = principal };
            _httpContextAccessor.HttpContext = httpContext;

            var result = _repo.GetUserId();

            Assert.That(result, Is.EqualTo(_userId));
        }

        [Test]
        public void GetUserId_WithUserIdClaim_ReturnsUserId()
        {
            var claims = new[] { new Claim("userId", _userId.ToString()) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = principal };
            _httpContextAccessor.HttpContext = httpContext;

            var result = _repo.GetUserId();

            Assert.That(result, Is.EqualTo(_userId));
        }

        [Test]
        public void GetUserId_InvalidClaimValue_ReturnsNull()
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "invalid") };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext { User = principal };
            _httpContextAccessor.HttpContext = httpContext;

            var result = _repo.GetUserId();

            Assert.That(result, Is.Null);
        }

        #endregion

        #region RevokeRefreshTokenAsync Tests

        [Test]
        public async Task RevokeRefreshTokenAsync_ClearsTokenAndExpiry()
        {
            var user = await _db.Users.FindAsync(_userId);
            user!.RefreshToken = "token";
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _db.SaveChangesAsync();

            await _repo.RevokeRefreshTokenAsync(user);

            var updatedUser = await _db.Users.FindAsync(_userId);
            Assert.That(updatedUser!.RefreshToken, Is.Null);
            Assert.That(updatedUser.RefreshTokenExpiry, Is.Null);
        }

        [Test]
        public async Task RevokeRefreshTokenAsync_NonExistingUser_DoesNotThrow()
        {
            var nonExistingUser = new User
            {
                Id = 9999,
                UserName = "ghost",
                Password = "password"
            };

            Assert.DoesNotThrowAsync(async () => await _repo.RevokeRefreshTokenAsync(nonExistingUser));
        }

        [Test]
        public async Task RevokeRefreshTokenAsync_AlreadyNullTokens_DoesNotThrow()
        {
            var user = await _db.Users.FindAsync(_userId);
            user!.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _db.SaveChangesAsync();

            Assert.DoesNotThrowAsync(async () => await _repo.RevokeRefreshTokenAsync(user));
        }

        #endregion
    }
}