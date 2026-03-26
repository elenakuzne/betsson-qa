using Betsson.OnlineWallets.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Betsson.OnlineWallets.Web.IntegrationTests")]

namespace Betsson.OnlineWallets.Data
{
    internal class OnlineWalletContext : DbContext
    {
        public DbSet<OnlineWalletEntry> Transactions { get; set; }

        public OnlineWalletContext(DbContextOptions<OnlineWalletContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new OnlineWalletEntryConfiguration());
        }
    }
}
