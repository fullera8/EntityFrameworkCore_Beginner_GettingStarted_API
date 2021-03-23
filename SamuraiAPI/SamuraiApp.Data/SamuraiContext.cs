using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SamuraiApp.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamuraiApp.Data
{
    public class SamuraiContext : DbContext
    {
        public DbSet<Samurai> Samurais { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Battle> Battles { get; set; }
        public DbSet<SamuraiBattleStat> SamuraiBattleStats { get; set; }

        /// <summary>
        /// Now required to tell the app specifically what to connect to. Abstracted connections are now gone.
        /// </summary>
        /// <param name="optionsBuilder">Default Db context options builder, defines db connection properties.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer("Data Source= (localdb)\\MSSQLLocalDB; Initial Catalog=SamuraiAppData") //How to connect to sql server
                .LogTo(Console.Write,
                    new[] {DbLoggerCategory.Database.Command.Name},//Logs everything to the profiler
                    LogLevel.Information) //Filter down to DB commands only (along with meta data about their execution)
                .EnableSensitiveDataLogging(); //Unhides paramters passed between methods

        }

        /// <summary>
        /// Explicitly defines the relationship between battles and samurais many to many relationship
        /// and uses the BattleSamurai class as the controller for the SQL table
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Samurai>()
                .HasMany(s => s.Battles)
                .WithMany(b => b.Samurais)
                .UsingEntity<BattleSamurai> //Defines the related table (one to many on each side)
                    (bs => bs.HasOne<Battle>().WithMany(),
                    bs => bs.HasOne<Samurai>().WithMany())
                .Property(bs => bs.DateJoined)
                .HasDefaultValueSql("getdate()");//This property will need to populate by default so we need to give it specific behavior

            modelBuilder.Entity<Horse>().ToTable("Horses");
            modelBuilder.Entity<SamuraiBattleStat>().HasNoKey().ToView("SamuraiBattleStats"); //This is required when there is no PK or FK
        }
    }
}
