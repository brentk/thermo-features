using System;
using Microsoft.EntityFrameworkCore;

namespace ThermoFeatures {
    public class ThermoFeaturesDbContext: DbContext {

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