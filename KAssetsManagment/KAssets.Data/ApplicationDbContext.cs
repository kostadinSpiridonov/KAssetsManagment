using KAssets.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KAssets.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public DbSet<SecurityGroup> SecurityGroups { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Organisation> Organisations { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Asset> Assets { get; set; }
        public DbSet<RequestToAcquireItems> RequestsToAcquireItems { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<CountItems> CountItems { get; set; }
        public DbSet<AssetHistory> AssetHistories { get; set; }
        public DbSet<HistoryRow> HistoryRows { get; set; }
        public DbSet<RequestForScrapping> RequestsForScrapping { get; set; }
        public DbSet<RequestForRelocation> RequestsForRelocation { get; set; }
        public DbSet<RequestForRenovation> RequestsFоrRenovation { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<PackingSlip> PackingSlips { get; set; }
        public DbSet<RequestForAsset> RequestForAssets { get; set; }
        public DbSet<RequestToProvider> RequestsToProvider { get; set; }
        public DbSet<ProviderItemOffer> ProviderItemOffers { get; set; }
        public DbSet<Currency> Currency { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<Accident> Accidents { get; set; }

        /// <summary>
        /// Define the relations between tables
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.SecurityGroups).WithMany(x => x.ApplicationUsers);
            modelBuilder.Entity<ApplicationUser>().HasMany(x => x.Events);

            modelBuilder.Entity<SecurityGroup>().HasMany(x => x.Rights).WithMany();

            modelBuilder.Entity<Organisation>().HasMany(x => x.Items);
            modelBuilder.Entity<Organisation>().HasMany(x => x.Sites);

            modelBuilder.Entity<RequestToAcquireItems>().HasMany(x => x.Items).WithMany();
            modelBuilder.Entity<RequestToAcquireItems>().HasMany(x => x.CountSelectedItems).WithOptional().WillCascadeOnDelete(true);

            modelBuilder.Entity<AssetHistory>().HasKey(x => x.AssetId);
            modelBuilder.Entity<AssetHistory>().HasMany(x => x.Rows);


            modelBuilder.Entity<Site>().HasMany(x => x.Assets);

            modelBuilder.Entity<PackingSlip>().Property(f => f.DateOfGiven).HasColumnType("datetime2");
            modelBuilder.Entity<PackingSlip>().Property(f => f.DateOfReceived).HasColumnType("datetime2");

            
            modelBuilder.Entity<RequestForAsset>().HasMany(x => x.Assets).WithMany();
            modelBuilder.Entity<RequestForAsset>().HasMany(x => x.ApprovedAssets).WithMany().Map(x => x.ToTable("RequestForAssetApprovedAssets"));
            modelBuilder.Entity<RequestForAsset>().HasMany(x => x.GivenAssets).WithMany().Map(x => x.ToTable("RequestForAssetGivenAssets"));

            modelBuilder.Entity<RequestToProvider>().HasMany(x => x.SendOffers);
            modelBuilder.Entity<RequestToProvider>().HasMany(x => x.WantItems).WithMany();
            modelBuilder.Entity<RequestToProvider>().HasMany(x => x.CountItems);
            modelBuilder.Entity<RequestToProvider>().HasMany(x => x.GiveItems).WithMany().Map(x => x.ToTable("RequesToProviderGiveItems"));

            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<Invoice>().HasMany(x => x.Items).WithMany();
            modelBuilder.Entity<Invoice>().Property(f => f.DateOfApproving).HasColumnType("datetime2");
            modelBuilder.Entity<Invoice>().Property(f => f.DateOfCreation).HasColumnType("datetime2");
            modelBuilder.Entity<Invoice>().Property(f => f.DateOfPayment).HasColumnType("datetime2");
            modelBuilder.Entity<Invoice>().Property(f => f.PaymentPeriod).HasColumnType("datetime2");


            modelBuilder.Entity<Accident>().Property(f => f.DateOfSend).HasColumnType("datetime2");
            modelBuilder.Entity<Accident>().Property(f => f.ReplyingDate).HasColumnType("datetime2");

            modelBuilder.Entity<Provider>().HasRequired(x => x.Organisation).WithMany()
            .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<Currency>().HasRequired(x => x.Organisation).WithMany()
            .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<Location>().HasRequired(x => x.Organisation).WithMany()
            .WillCascadeOnDelete(false);
           
            modelBuilder.Entity<ExchangeRate>().HasRequired(x => x.Organisation).WithMany()
            .WillCascadeOnDelete(false);
           

            base.OnModelCreating(modelBuilder);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}
