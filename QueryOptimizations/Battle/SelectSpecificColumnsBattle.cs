namespace QueryOptimizations.Battle
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using Dapper;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Results;

    public static class SelectSpecificColumnsBattle
    {
        private static string GetAllOwnerNamesWithCat()
        {
            using var db = new CatsDbContext();

            var owners = db.Owners
                .Where(o => o.Cats.Any())
                .ToList();

            var names = new List<string>();

            foreach (var owner in owners)
            {
                names.Add(owner.Name);
            }

            return string.Join(", ", names);
        }

        private static string GetAllOwnerNamesWithCatSelect()
        {
            using var db = new CatsDbContext();

            var owners = db.Owners
                .Where(o => o.Cats.Any())
                .Select(o => o.Name)
                .ToList();

            return string.Join(", ", owners);
        }

        public static void Fight()
        {
            Console.WriteLine("Select Specific Columns Battle");

            var stopWatch = Stopwatch.StartNew();

            // EF Core Getting All Columns
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Owner.Cats.Count > 1)
                    .AsNoTracking()
                    .ToDictionary(c => c.Name, c => c.Age);

                Console.WriteLine($"EF Core Getting All Columns: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Getting All Columns Cached
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Owner.Cats.Count > 1)
                    .AsNoTracking()
                    .ToDictionary(c => c.Name, c => c.Age);

                Console.WriteLine($"EF Core Getting All Columns Cached: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Getting Only The Columns We Need
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Owner.Cats.Count > 1)
                    .Select(c => new CatResult
                    {
                        Name = c.Name,
                        Age = c.Age
                    })
                    .ToDictionary(c => c.Name, c => c.Age);

                Console.WriteLine($"EF Core Getting Only The Columns We Need: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Getting Only The Columns We Need Cached
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Where(c => c.Owner.Cats.Count > 1)
                    .Select(c => new CatResult
                    {
                        Name = c.Name,
                        Age = c.Age
                    })
                    .ToDictionary(c => c.Name, c => c.Age);

                Console.WriteLine($"EF Core Getting Only The Columns We Need Cached: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Using Joins
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .Join(
                        db.Owners.Where(o => o.Cats.Count > 1),
                        c => c.OwnerId,
                        o => o.Id,
                        (c, o) => new CatResult
                        {
                            Name = c.Name,
                            Age = c.Age
                        })
                    .ToDictionary(c => c.Name, c => c.Age);

                Console.WriteLine($"EF Core Using Joins: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Getting All Columns
            using (var db = new CatsDataConnection())
            {
                var cats = (from cat in db.Cats
                            join owner in db.Owners
                                on cat.OwnerId equals owner.Id
                            join ownerCat in db.Cats
                                on owner.Id equals ownerCat.OwnerId
                                into ownersWithCats
                            where ownersWithCats.Count() > 1
                            select cat).ToDictionary(c => c.Name, c => c.Age);

                Console.WriteLine($"LINQ to DB Getting All Columns: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Getting All Columns Cached
            using (var db = new CatsDataConnection())
            {
                var cats = (from cat in db.Cats
                            join owner in db.Owners
                                on cat.OwnerId equals owner.Id
                            join ownerCat in db.Cats
                                on owner.Id equals ownerCat.OwnerId
                                into ownersWithCats
                            where ownersWithCats.Count() > 1
                            select cat).ToDictionary(c => c.Name, c => c.Age);

                Console.WriteLine($"LINQ to DB Getting All Columns Cached: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Getting Only The Columns We Need
            using (var db = new CatsDataConnection())
            {
                var cats = (from cat in db.Cats
                            join owner in db.Owners
                                on cat.OwnerId equals owner.Id
                            join ownerCat in db.Cats
                                on owner.Id equals ownerCat.OwnerId
                                into ownersWithCats
                            where ownersWithCats.Count() > 1
                            select new CatResult
                            {
                                Name = cat.Name,
                                Age = cat.Age
                            }).ToDictionary(c => c.Name, c => c.Age);

                Console.WriteLine($"LINQ to DB Getting Only The Columns We Need: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Getting Only The Columns We Need Cached
            using (var db = new CatsDataConnection())
            {
                var cats = (from cat in db.Cats
                            join owner in db.Owners
                                on cat.OwnerId equals owner.Id
                            join ownerCat in db.Cats
                                on owner.Id equals ownerCat.OwnerId
                                into ownersWithCats
                            where ownersWithCats.Count() > 1
                            select new CatResult
                            {
                                Name = cat.Name,
                                Age = cat.Age
                            }).ToDictionary(c => c.Name, c => c.Age);

                Console.WriteLine($"LINQ to DB Getting Only The Columns We Need Cached: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper Getting All Columns
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT *
                    FROM [Cats] AS [c]
                    INNER JOIN [Owners] AS [o] ON [c].[OwnerId] = [o].[Id]
                    WHERE (
                        SELECT COUNT(*)
                        FROM [Cats] AS [c0]
                        WHERE [o].[Id] = [c0].[OwnerId]) > 1");

                Console.WriteLine($"Dapper Getting All Columns: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper Getting All Columns Cached
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT *
                    FROM [Cats] AS [c]
                    INNER JOIN [Owners] AS [o] ON [c].[OwnerId] = [o].[Id]
                    WHERE (
                        SELECT COUNT(*)
                        FROM [Cats] AS [c0]
                        WHERE [o].[Id] = [c0].[OwnerId]) > 1");

                Console.WriteLine($"Dapper Getting All Columns Cached: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper Getting Only The Columns We Need
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<CatResult>(
                    @"SELECT [c].[Name], [c].[Age]
                    FROM [Cats] AS [c]
                    INNER JOIN [Owners] AS [o] ON [c].[OwnerId] = [o].[Id]
                    WHERE (
                        SELECT COUNT(*)
                        FROM [Cats] AS [c0]
                        WHERE [o].[Id] = [c0].[OwnerId]) > 1");

                Console.WriteLine($"Dapper Getting Only The Columns We Need: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper Getting Only The Columns We Need Cached
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<CatResult>(
                    @"SELECT [c].[Name], [c].[Age]
                    FROM [Cats] AS [c]
                    INNER JOIN [Owners] AS [o] ON [c].[OwnerId] = [o].[Id]
                    WHERE (
                        SELECT COUNT(*)
                        FROM [Cats] AS [c0]
                        WHERE [o].[Id] = [c0].[OwnerId]) > 1");

                Console.WriteLine($"Dapper Getting Only The Columns We Need Cached: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Raw SQL Query
            using (var db = new CatsDbContext())
            {
                // EF Core cannot translate this query.

                try
                {
                    var cats = db.Cats
                        .FromSqlRaw(
                            @"SELECT *
                            FROM [Cats] AS [c]
                            INNER JOIN [Owners] AS [o] ON [c].[OwnerId] = [o].[Id]
                            WHERE (
                                SELECT COUNT(*)
                                FROM [Cats] AS [c0]
                                WHERE [o].[Id] = [c0].[OwnerId]) > 1")
                        .ToList();

                    Console.WriteLine($"EF Core Raw SQL Query: {stopWatch.Elapsed} - {cats.Count} Results");
                }
                catch
                {
                    Console.WriteLine("EF Core Raw SQL Query: CANNOT EXECUTE.");
                }
            }

            Console.WriteLine(new string('-', 50));
        }
    }
}
