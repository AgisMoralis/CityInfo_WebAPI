using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        // NOTE: This is the 2nd option on how to configure the DbContext
        // in order to be able to use it with an Sqlite database, by creating
        // an overloaded constructor on our DbContext class, that calls the
        // overloaded base method, passing the "DbContextOptions" during the construction
        public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options) 
        { 
        }

        // LINQ queries against a 'DbSet' will be
        // translated into queries against the Database
        public DbSet<City> Cities { get; set; }
        public DbSet<PointOfInterest> PointsOfInterest { get; set; }

        // NOTE: This is the 1st option on how to configure the DbContext
        // NOTE: in order to be able to use it with an Sqlite database
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     // This configuration indicates that this 'CityInfo'
        //     // DbContext is going to be used to connect to an Sqlite database
        //     optionsBuilder.UseSqlite("connectionstring");
        // 
        //     base.OnConfiguring(optionsBuilder);
        // }
    }
}
