using Auction.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auction.Data
{
    public class AuctionDbContext : IdentityDbContext
    {
       

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<Account>(entity => { entity.ToTable("Accounts"); });
            //builder.Entity<Customer>(entity => { entity.ToTable("Customers"); });
            //builder.Entity<Professional>(entity => { entity.ToTable("Professionals"); });
        }
        public AuctionDbContext(DbContextOptions<AuctionDbContext> options) : base(options)
        {
        }

        public DbSet<ProductViewModel> Products { get; set; }
        public DbSet<WalletViewModel> Wallets{ get; set; }
        public DbSet<AuctionViewModel> Auctions { get; set; }
        public DbSet<BidViewModel> Bids { get; set; }
       
       

    }
   
}
