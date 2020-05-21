namespace QueryOptimizations.Battle
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Results;

    public static class CompiledQueriesBattle
    {
        public static void Fight()
        {
            Console.WriteLine("Compiled Queries Battle");

            var stopWatch = Stopwatch.StartNew();

            // EF Core - First Query Is Cold
            using (var db = new CatsDbContext())
            {
                var cats = CatQuery(db, 5, "C");

                Console.WriteLine($"EF Core - Normal Query Cold: {stopWatch.Elapsed} - {cats} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - Second Query Is Warm
            using (var db = new CatsDbContext())
            {
                var cats = CatQuery(db, 5, "C");

                Console.WriteLine($"EF Core - Normal Query Warm: {stopWatch.Elapsed} - {cats} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - Same Query Compiled Cold
            using (var db = new CatsDbContext())
            {
                var cats = CatQueries
                    .CatQuery(db, 5, "C")
                    .Count();

                Console.WriteLine($"EF Core - Compiled Query Cold: {stopWatch.Elapsed} - {cats} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - Same Query Compiled Warm
            using (var db = new CatsDbContext())
            {
                var cats = CatQueries
                    .CatQuery(db, 5, "C")
                    .Count();

                Console.WriteLine($"EF Core - Compiled Query Warm: {stopWatch.Elapsed} - {cats} Results");
            }

            Console.WriteLine(new string('-', 50));
        }

        private static int CatQuery(CatsDbContext db, int age, string nameStart)
        {
            var cats = db.Cats
                .Where(c =>
                    c.BirthDate.Year > 2019 &&
                    c.Color.Contains("B") &&
                    c.Owner.Cats.Any(cat => cat.Age < age) &&
                    c.Owner.Cats.Count(cat => cat.Name.Length > 3) > 3)
                .Select(c => new CatFamilyResult
                {
                    Name = c.Name,
                    Cats = c.Owner
                        .Cats
                        .Count(cat =>
                            cat.Age < age &&
                            cat.Name.StartsWith(nameStart))
                })
                .ToList();

            return cats.Count;
        }
    }
}
