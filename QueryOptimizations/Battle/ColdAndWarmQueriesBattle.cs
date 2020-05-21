namespace QueryOptimizations.Battle
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using Dapper;
    using Models;

    public static class ColdAndWarmQueriesBattle
    {
        public static void Fight()
        {
            Console.WriteLine("Cold And Warm Queries Battle");

            var stopWatch = Stopwatch.StartNew();

            // EF Core - First Query Will Be Slow
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c =>
                        c.BirthDate.Year > 2019 &&
                        c.Color.Contains("B") &&
                        c.Owner.Cats.Any(cat => cat.Age < 5) &&
                        c.Owner.Cats.Count(cat => cat.Name.Length > 3) > 3)
                    .Select(c => new
                    {
                        c.Name,
                        Cats = c.Owner
                            .Cats
                            .Count(cat =>
                                cat.Age < 5 &&
                                cat.Name.StartsWith("C"))
                    })
                    .ToList();

                Console.WriteLine($"EF Core - First Query: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core - Second Query Will Be Fast
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c =>
                        c.BirthDate.Year > 2019 &&
                        c.Color.Contains("B") &&
                        c.Owner.Cats.Any(cat => cat.Age < 5) &&
                        c.Owner.Cats.Count(cat => cat.Name.Length > 3) > 3)
                    .Select(c => new
                    {
                        c.Name,
                        Cats = c.Owner
                            .Cats
                            .Count(cat =>
                                cat.Age < 5 &&
                                cat.Name.StartsWith("C"))
                    })
                    .ToList();

                Console.WriteLine($"EF Core - Second Query: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB - First Query Will Be Slow 
            using (var db = new CatsDataConnection())
            {
                var cats = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Cats.Where(c => c.Name.Contains("1")),
                        o => o.Id,
                        c => c.OwnerId, (o, c) => c)
                    .ToList();

                Console.WriteLine($"LINQ to DB - First Query: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB - Second Query Will Be Fast
            using (var db = new CatsDataConnection())
            {
                var cats = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Cats.Where(c => c.Name.Contains("1")),
                        o => o.Id,
                        c => c.OwnerId, (o, c) => c)
                    .ToList();

                Console.WriteLine($"LINQ to DB - Second Query: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper - First Query
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT [c].[Name], [c].[Age]
                    FROM [Cats] AS [c]
                    INNER JOIN [Owners] AS [o] ON [c].[OwnerId] = [o].[Id]
                    WHERE (
                        SELECT COUNT(*)
                        FROM [Cats] AS [c0]
                        WHERE [o].[Id] = [c0].[OwnerId]) > 1");

                Console.WriteLine($"Dapper - First Query: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper - Second Query
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT [c].[Name], [c].[Age]
                    FROM [Cats] AS [c]
                    INNER JOIN [Owners] AS [o] ON [c].[OwnerId] = [o].[Id]
                    WHERE (
                        SELECT COUNT(*)
                        FROM [Cats] AS [c0]
                        WHERE [o].[Id] = [c0].[OwnerId]) > 1");

                Console.WriteLine($"Dapper - First Query: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            Console.WriteLine(new string('-', 50));
        }
    }
}
