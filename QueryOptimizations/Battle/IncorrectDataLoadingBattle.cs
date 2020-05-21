namespace QueryOptimizations.Battle
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    public static class IncorrectDataLoadingBattle
    {
        private static void DataNoLoaded()
        {
            // Methods which load data from the database
            // .ToList(), .ToArray(), .ToDictionary(),
            // .Any(), .All(), Count(), .Contains()
            // .First...(), .Last...(), .Single...()
            // .Max(), Min(), Average(), Sum()
            // All Async versions and more

            using var db = new CatsDbContext();

            // Database will not be queried in the cats variable.
            var cats = db.Cats
                .Where(c => c.Name.Contains("1"))
                .OrderBy(c => c.Name)
                .Select(c => c.Name);

            // Database will be queried here.
            Console.WriteLine(cats.Count());
        }

        public static void Fight()
        {
            Console.WriteLine("Incorrect Data Loading Battle");

            var stopWatch = Stopwatch.StartNew();

            // EF Core Getting All Rows
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .ToList()
                    .Where(c => c.Name.Contains("1")); // This Where will be executed in memory.

                Console.WriteLine($"EF Core Getting All Rows: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Getting Only The Rows We Need
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    // .AsNoTracking()
                    .ToList();

                Console.WriteLine($"EF Core Getting Only The Rows We Need: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();
             
            // EF Core Getting Only The Rows We Need Cached
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    // .AsNoTracking()
                    .ToList();

                Console.WriteLine($"EF Core Getting Only The Rows We Need Cached: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Getting All Rows
            using (var db = new CatsDataConnection())
            {
                var cats = db.Cats
                    .ToList()
                    .Where(c => c.Name.Contains("1")); // This Where will be executed in memory.

                Console.WriteLine($"LINQ to DB Getting All Rows: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Getting Only The Rows We Need
            using (var db = new CatsDataConnection())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .ToList();

                Console.WriteLine($"LINQ to DB Getting Only The Rows We Need: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Getting Only The Rows We Need Cached
            using (var db = new CatsDataConnection())
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .ToList();

                Console.WriteLine($"LINQ to DB Getting Only The Rows We Need Cached: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            Console.WriteLine(new string('-', 50));
        }
    }
}
