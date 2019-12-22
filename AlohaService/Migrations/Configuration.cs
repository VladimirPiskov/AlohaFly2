using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using AlohaService;

namespace AlohaService.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AlohaDb>
    {
        public Configuration()
        {
           // AutomaticMigrationsEnabled = false;
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(AlohaDb context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
