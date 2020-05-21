namespace QueryOptimizations.Battle
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Results;

    public static class InnerQueryBattle
    {
        public static void Fight()
        {
            Console.WriteLine("Inner Query Battle");

            var stopWatch = Stopwatch.StartNew();

            // EF Core Inner Query (Depends On EF Version - Optimized After 3.0)
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Select(c => new CatSiblingsResult
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Siblings = c.Owner.Cats
                            .Where(s => s.Id != c.Id)
                            .Select(s => s.Name)
                            .ToList()
                    })
                    .ToList();

                Console.WriteLine($"EF Core Inner Query: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Inner Query Cached (Depends On EF Version - Optimized After 3.0)
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Select(c => new CatSiblingsResult
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Siblings = c.Owner.Cats
                            .Where(s => s.Id != c.Id)
                            .Select(s => s.Name)
                            .ToList()
                    })
                    .ToList();

                Console.WriteLine($"EF Core Inner Query Cached: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core No Inner Query - In Memory Calculation
            using (var db = new CatsDbContext())
            {
                var dbCats = db.Cats
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.OwnerId
                    })
                    .ToList();

                // Evaluated in memory.
                var cats = dbCats
                    .GroupJoin(
                        dbCats,
                        c => c.OwnerId,
                        s => s.OwnerId,
                        (f, s) => new CatSiblingsResult
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Siblings = s
                                .Where(si => si.Id != f.Id)
                                .Select(si => si.Name)
                                .ToList()
                        })
                    .ToList();

                Console.WriteLine($"EF Core No Inner Query - In Memory Calculation: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Does Not Allow Inner Queries
            using (var db = new CatsDataConnection())
            {
                var dbCats = db.Cats
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.OwnerId
                    })
                    .ToList();

                // Evaluated in memory.
                var cats = dbCats
                    .GroupJoin(
                        dbCats,
                        c => c.OwnerId,
                        s => s.OwnerId,
                        (f, s) => new CatSiblingsResult
                        {
                            Id = f.Id,
                            Name = f.Name,
                            Siblings = s
                                .Where(si => si.Id != f.Id)
                                .Select(si => si.Name)
                                .ToList()
                        })
                    .ToList();

                Console.WriteLine($"LINQ to DB No Inner Query - In Memory Calculation: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            Console.WriteLine(new string('-', 50));
        }
    }
}
