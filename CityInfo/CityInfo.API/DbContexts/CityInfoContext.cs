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

        /// <summary>
        /// 1) This allows to manually construct our model, if the conventions we used until
        /// now were not sufficient or if we preffered to be more explicit.
        /// 2) It can also be used to provide data for seeding the database.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // The 'HasData' is used to seed data to the database
            _ = modelBuilder.Entity<City>()
                .HasData(
                new City("NYC")
                {
                    Id = 1,
                    Description = "City with a lot of people",
                },
                new City("Athens")
                {
                    Id = 2,
                    Description = "Ancient City",
                },
                new City("Paris")
                {
                    Id = 3,
                    Description = "City with Eiffel Tower",
                });

            _ = modelBuilder.Entity<PointOfInterest>()
               .HasData(
               new PointOfInterest("Central Park")
               {
                   Id = 1,
                   CityId = 1,
                   Description = "Big park",
               },
               new PointOfInterest("Empire State")
               {
                   Id = 2,
                   CityId = 1,
                   Description = "Tall building",
               },
               new PointOfInterest("Parthenon")
               {
                   Id = 3,
                   CityId = 2,
                   Description = "Acropolis",
               },
               new PointOfInterest("Empire State")
               {
                   Id = 4,
                   CityId = 2,
                   Description = "Center of the city",
               },
               new PointOfInterest("Eiffel Tower")
               {
                   Id = 5,
                   CityId = 3,
                   Description = "Tall tower",
               },
               new PointOfInterest("Louvre Museum")
               {
                   Id = 6,
                   CityId = 3,
                   Description = "World's biggest museum",
               });

            base.OnModelCreating(modelBuilder);
        }

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
