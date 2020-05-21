namespace QueryOptimizations
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Models;

    public static class DataSeeder
    {
        public static void Seed()
        {
            using var db = new CatsDbContext();

            db.Database.Migrate();

            db.ChangeTracker.AutoDetectChangesEnabled = false;

            for (int i = 1; i <= 10000; i++)
            {
                var owner = new Owner
                {
                    Name = $"Owner {i}"
                };

                for (int j = 1; j <= 10; j++)
                {
                    owner.Cats.Add(new Cat
                    {
                        Name = $"Cat {i} {j}",
                        Color = j % 2 == 0 ? "Black" : "White",
                        BirthDate = DateTime.Now.AddDays(-j),
                        Age = j
                    });
                }

                db.Owners.Add(owner);

                if (i % 200 == 0)
                {
                    db.SaveChanges();
                    Console.Write(".");
                }
            }
        }

        public static void DeleteAndSeed()
        {
            using var db = new CatsDbContext();

            db.Database.EnsureDeleted();

            Seed();
        }
    }
}
