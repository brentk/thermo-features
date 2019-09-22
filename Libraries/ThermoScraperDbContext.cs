using System;
using Microsoft.EntityFrameworkCore;

namespace ThermoScrape {
    public class ThermoScraperDbContext: DbContext {

        public DbSet<ThermostatLog> ThermostatLogs {get; set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.UseSqlite("Data Source=local.db");
    }

    public class ThermostatLog {
        public string Id {get; set;}
        public string Json {get; set;}
        public DateTimeOffset Stamp {get;set;}
    }
}