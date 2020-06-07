using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using AlohaService.Entities;

namespace AlohaService
{
    public class AlohaDb : DbContext
    {
        public AlohaDb() : base()
        {
            //Database.SetInitializer(new DropCreateDatabaseAlways<AlohaDb>());
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<AlohaDb>());

            //Database.SetInitializer(new CreateDatabaseIfNotExists<AlohaDb>());

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<AlohaDb, Migrations.Configuration>());

            // Get the ObjectContext related to this DbContext
            var objectContext = (this as IObjectContextAdapter).ObjectContext;

            // Sets the command timeout for all the commands
            objectContext.CommandTimeout = 300;

            //this.tim
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            /*
            modelBuilder.Entity<OrderFlight>().Map(m =>
            {
                m.MapInheritedProperties();
            });

            modelBuilder.Entity<OrderToGo>().Map(m =>
            {
                m.MapInheritedProperties();
            });
            */

        }

        public DbSet<User> Users { get; set; }
        public DbSet<AirCompany> AirCompanies { get; set; }
        public DbSet<LogItem> LogItems { get; set; }
        public DbSet<Curier> Curiers { get; set; }
        public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
        public DbSet<DeliveryPlace> DeliveryPlace { get; set; }
        public DbSet<MarketingChannel> MarketingChannel { get; set; }
        public DbSet<Dish> Dish { get; set; }
        public DbSet<Driver> Driver { get; set; }
        public DbSet<DishPackageFlightOrder> DishPackagesFlightOrder { get; set; }
        public DbSet<DishPackageToGoOrder> DishPackagesToGoOrder { get; set; }
        public DbSet<OrderCustomer> OrderCustomers { get; set; }
        public DbSet<OrderFlight> OrderFlight { get; set; }
        public DbSet<OrderToGo> OrderToGo { get; set; }

        public DbSet<ItemLabelInfo> ItemLabelInfos { get; set; }

        public DbSet<ContactPerson> ContactPersons { get; set; }

        public DbSet<UserFunc> UserFuncs { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupAccess> UserGroupAccesses { get; set; }
        public DbSet<UserUserGroup> UserUserGroups { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentGroup> PaymentGroups { get; set; }

        public DbSet<Discount> Discounts { get; set; }
        public DbSet<DiscountRange> DiscountRanges { get; set; }
        public DbSet<DiscountDiscountRangeLink> DiscountDiscountRangeLinks { get; set; }
        public DbSet<Alert> Alerts { get; set; }

        public DbSet<DishKitchenGroup> DishKitchenGroups { get; set; }

        public DbSet<DishLogicGroup> DishLogicGroups { get; set; }

        public DbSet<OrderCustomerPhone> OrderCustomerPhones { get; set; }

        public DbSet<OrderCustomerAddress> OrderCustomerAddresses { get; set; }
        public DbSet<TransactionTime> TransactionTime { get; set; }

        public DbSet<OrderCustomerInfo> OrderCustomerInfo { get; set; }

        public DbSet<DishExternalLinks> DishExternalLinks { get; set; }
        
    }
}