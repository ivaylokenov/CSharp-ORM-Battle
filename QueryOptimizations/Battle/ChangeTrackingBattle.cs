namespace QueryOptimizations.Battle
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Results;

    public static class ChangeTrackingBattle
    {
        public static void Fight()
        {
            Console.WriteLine("Change Tracking Battle");

            // EF Core - Warm-Up
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .ToList();

                Console.WriteLine($"EF Core Warming Up - {cats.Count} Results");
            }

            var stopWatch = Stopwatch.StartNew();

            // EF Core - Default Change Tracking
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .ToList();

                Console.WriteLine($"EF Core Default Tracking - {stopWatch.Elapsed} - {cats.Count} Results - {db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - No Tracking on a DbContext Instance Level
            using (var db = new CatsDbContext())
            {
                db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

                var cats = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .ToList();

                Console.WriteLine($"EF Core Context No Tracking - {stopWatch.Elapsed} - {cats.Count} Results - {db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - No Tracking per Single Query
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .AsNoTracking()
                    .Where(c => c.Id % 10 == 0)
                    .ToList();

                Console.WriteLine($"EF Core Query No Tracking - {stopWatch.Elapsed} - {cats.Count} Results - {db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - Tracking in Projection Without a Data Model
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        Owner = c.Owner.Name
                    })
                    .ToList();

                Console.WriteLine($"EF Core Select Tracking With No Data Model - {stopWatch.Elapsed} - {cats.Count} Results - {db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - Tracking in Projection With a Data Model
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Owner
                    })
                    .ToList();

                Console.WriteLine($"EF Core Select Tracking With Data Model - {stopWatch.Elapsed} - {cats.Count} Results - {db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - No Tracking in Projection With a Data Model
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .AsNoTracking()
                    .Where(c => c.Id % 10 == 0)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Owner
                    })
                    .ToList();

                Console.WriteLine($"EF Core Select No Tracking With Data Model - {stopWatch.Elapsed} - {cats.Count} Results - {db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - No Tracking in Projection With a DTO Model
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Id % 10 == 0)
                    .Select(c => new CatResult
                    {
                        Name = c.Name,
                        Age = c.Age
                    })
                    .ToList();

                Console.WriteLine($"EF Core Select Tracking With DTO Model - {stopWatch.Elapsed} - {cats.Count} Results - {db.ChangeTracker.Entries().Count()} Tracked Entities.");
            }

            Console.WriteLine(new string('-', 50));
        }
    }
}
