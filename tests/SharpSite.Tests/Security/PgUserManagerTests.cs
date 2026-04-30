using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SharpSite.Abstractions.Security;
using SharpSite.Security.Postgres;
using System.Security.Claims;
using Xunit;

namespace SharpSite.Tests.Security;

public class PgUserManagerTests
{
    [Fact]
    public async Task CreateUser_Success()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IUserManager<ISharpSiteUser>, PgUserManager>();
        services.AddScoped<UserManager<PgSharpSiteUser>>();
        var provider = services.BuildServiceProvider();

        var userManager = provider.GetRequiredService<IUserManager<ISharpSiteUser>>();

        // Act
        var user = new PgSharpSiteUser 
        { 
            DisplayName = "Test User",
            UserName = "test@test.com",
            Email = "test@test.com"
        };

        var result = await userManager.CreateAsync((ISharpSiteUser)user, "TestPass123!");

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task GetUser_ReturnsPgSharpSiteUser()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<IUserManager<ISharpSiteUser>, PgUserManager>();
        services.AddScoped<UserManager<PgSharpSiteUser>>();
        var provider = services.BuildServiceProvider();

        var userManager = provider.GetRequiredService<IUserManager<ISharpSiteUser>>();

        var claims = new List<Claim> 
        {
            new Claim(ClaimTypes.Name, "test@test.com")  
        };
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Act
        var user = await userManager.GetUserAsync(principal);

        // Assert
        Assert.NotNull(user);
        Assert.IsType<PgSharpSiteUser>(user);
    }
}
