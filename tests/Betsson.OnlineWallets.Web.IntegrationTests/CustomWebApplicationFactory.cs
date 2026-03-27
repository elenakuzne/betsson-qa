using Betsson.OnlineWallets.Data;
using Betsson.OnlineWallets.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Betsson.OnlineWallets.Web.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<OnlineWalletContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            services.AddDbContext<OnlineWalletContext>(options =>
                options.UseInMemoryDatabase("IntegrationTests"));
        });
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<OnlineWalletContext>();
        context.Database.EnsureDeleted();
    }
}
