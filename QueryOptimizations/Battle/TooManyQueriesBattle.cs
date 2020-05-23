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
    using RepoDb;
    using Results;

    public static class TooManyQueriesBattle
    {
        private static IEnumerable<Cat> GetOldCats(bool printCats)
        {
            using var db = new CatsDbContext();

            var oldCats = db.Cats
                .Where(c => c.Age > 5)
                .ToList();

            if (printCats)
            {
                PrintOldCats(oldCats);
            }

            return oldCats;
        }

        private static void PrintOldCats(IEnumerable<Cat> oldCats)
        {
            foreach (var oldCat in oldCats)
            {
                // In case of lazy loading enabled
                Console.WriteLine($"{oldCat.Name} - {oldCat.Age} - {oldCat.Owner.Name}");
            }
        }

        private static IEnumerable<OldCatResult> GetOldCatsSelect(bool printCats)
        {
            using var db = new CatsDbContext();

            var oldCats = db.Cats
                .Where(c => c.Age > 5)
                .Select(c => new OldCatResult
                {
                    Name = c.Name,
                    Age = c.Age,
                    Owner = c.Owner.Name
                })
                .ToList();

            if (printCats)
            {
                PrintOldCatsSelect(oldCats);
            }

            return oldCats;
        }

        private static void PrintOldCatsSelect(IEnumerable<OldCatResult> oldCats)
        {
            foreach (var oldCat in oldCats)
            {
                // In case of lazy loading enabled
                Console.WriteLine($"{oldCat.Name} - {oldCat.Age} - {oldCat.Owner}");
            }
        }

        public static void Fight()
        {
            Console.WriteLine("Too Many Queries Battle");

            var stopWatch = Stopwatch.StartNew();

            // EF Core N+1
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .ToList();

                var total = 0;

                foreach (var owner in owners)
                {
                    var cats = db.Cats
                        .Where(c => c.OwnerId == owner.Id && c.Name.Contains("1"))
                        .ToList();

                    total += cats.Count;
                }

                Console.WriteLine($"EF Core N+1: {stopWatch.Elapsed} - {total} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Using Include
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Include(o => o.Cats)
                    .ToList();

                var total = 0;

                foreach (var owner in owners)
                {
                    var cats = owner.Cats
                        .Where(c => c.Name.Contains("1"))
                        .ToList();

                    total += cats.Count;
                }

                Console.WriteLine($"EF Core Include: {stopWatch.Elapsed} - {total} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Using Filtered Include - Introduced in EF Core 5.0
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Include(o => o.Cats
                        .Where(c => c.Name.Contains("1")))
                    .ToList();

                Console.WriteLine($"EF Core Filtered Include: {stopWatch.Elapsed} - {owners.SelectMany(o => o.Cats).Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Using Select
            using (var db = new CatsDbContext())
            {
                var owners = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Select(o => new
                    {
                        Cats = o.Cats
                            .Where(c => c.Name.Contains("1"))
                    })
                    .ToList();

                Console.WriteLine($"EF Core Select: {stopWatch.Elapsed} - {owners.SelectMany(o => o.Cats).Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Using Join
            using (var db = new CatsDbContext())
            {
                var cats = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Cats.Where(c => c.Name.Contains("1")),
                        o => o.Id,
                        c => c.OwnerId, (o, c) => c)
                    .ToList();

                Console.WriteLine($"EF Core Join: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB
            using (var db = new CatsDataConnection())
            {
                // LINQ to DB does not support Select joins
                // You need to join your tables explicitly

                var cats = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Cats.Where(c => c.Name.Contains("1")), 
                        o => o.Id, 
                        c => c.OwnerId, (o, c) => c)
                    .ToList();

                Console.WriteLine($"LINQ to DB Join: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Cached
            using (var db = new CatsDataConnection())
            {
                var cats = db.Owners
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Cats.Where(c => c.Name.Contains("1")),
                        o => o.Id,
                        c => c.OwnerId, (o, c) => c)
                    .ToList();

                Console.WriteLine($"LINQ to DB Join Cached: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // RepoDB Query And In Memory Join
            using (var connection = new SqlConnection(Settings.ConnectionString).EnsureOpen())
            {
                var dbOwners = connection.Query<Owner>(o => o.Name.Contains("1"));
                var dbCats = connection.Query<Cat>(c => c.Name.Contains("1"));

                // In Memory
                var cats = dbOwners
                    .Join(dbCats,
                        o => o.Id,
                        c => c.OwnerId, (o, c) => c)
                    .ToList();

                Console.WriteLine($"RepoDB Query And In Memory Join: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // RepoDB Query Multiple And In Memory Join
            using (var connection = new SqlConnection(Settings.ConnectionString).EnsureOpen())
            {
                var result = connection.QueryMultiple<Owner, Cat>(
                    o => o.Name.Contains("1"), 
                    c => c.Name.Contains("1"));

                var dbOwners = result.Item1.AsList();
                var dbCats = result.Item2.AsList();

                var cats = dbOwners
                    .Join(dbCats,
                        o => o.Id,
                        c => c.OwnerId, (o, c) => c)
                    .ToList();

                Console.WriteLine($"RepoDB Query Multiple And In Memory Join: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper With EF Query
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT [o].[Id], [t].[Id], [t].[Age], [t].[BirthDate], [t].[Color], [t].[Name], [t].[OwnerId]
                    FROM[Owners] AS[o]
                    LEFT JOIN(
                        SELECT[c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
                    FROM[Cats] AS[c]
                    WHERE CHARINDEX(N'1', [c].[Name]) > 0
                        ) AS[t] ON[o].[Id] = [t].[OwnerId]
                    WHERE CHARINDEX(N'1', [o].[Name]) > 0
                    ORDER BY[o].[Id], [t].[Id]");

                Console.WriteLine($"Dapper (EF): {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper With EF Query Cached
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT [o].[Id], [t].[Id], [t].[Age], [t].[BirthDate], [t].[Color], [t].[Name], [t].[OwnerId]
                    FROM[Owners] AS[o]
                    LEFT JOIN(
                        SELECT[c].[Id], [c].[Age], [c].[BirthDate], [c].[Color], [c].[Name], [c].[OwnerId]
                    FROM[Cats] AS[c]
                    WHERE CHARINDEX(N'1', [c].[Name]) > 0
                        ) AS[t] ON[o].[Id] = [t].[OwnerId]
                    WHERE CHARINDEX(N'1', [o].[Name]) > 0
                    ORDER BY[o].[Id], [t].[Id]");

                Console.WriteLine($"Dapper (EF) Cached: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper With LINQ to DB Query
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT
	                    [c_1].[Id],
	                    [c_1].[Name],
	                    [c_1].[Age],
	                    [c_1].[BirthDate],
	                    [c_1].[Color],
	                    [c_1].[OwnerId]
                    FROM
	                    [Owners] [o]
		                    INNER JOIN [Cats] [c_1] ON [o].[Id] = [c_1].[OwnerId]
                    WHERE
	                    [c_1].[Name] LIKE N'%1%' AND [o].[Name] LIKE N'%1%'");

                Console.WriteLine($"Dapper (LINQ to DB): {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper With LINQ to DB Query Cached
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<Cat>(
                    @"SELECT
	                    [c_1].[Id],
	                    [c_1].[Name],
	                    [c_1].[Age],
	                    [c_1].[BirthDate],
	                    [c_1].[Color],
	                    [c_1].[OwnerId]
                    FROM
	                    [Owners] [o]
		                    INNER JOIN [Cats] [c_1] ON [o].[Id] = [c_1].[OwnerId]
                    WHERE
	                    [c_1].[Name] LIKE N'%1%' AND [o].[Name] LIKE N'%1%'");

                Console.WriteLine($"Dapper (LINQ to DB) cached: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Raw SQL Query
            using (var db = new CatsDbContext())
            {
                var cats = db.Cats
                    .FromSqlRaw(
                        @"SELECT
	                        [c_1].[Id],
	                        [c_1].[Name],
	                        [c_1].[Age],
	                        [c_1].[BirthDate],
	                        [c_1].[Color],
	                        [c_1].[OwnerId]
                        FROM
	                        [Owners] [o]
		                        INNER JOIN [Cats] [c_1] ON [o].[Id] = [c_1].[OwnerId]
                        WHERE
	                        [c_1].[Name] LIKE N'%1%' AND [o].[Name] LIKE N'%1%'")
                    .AsNoTracking()
                    .ToList();

                Console.WriteLine($"EF Core Raw SQL Query: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            Console.WriteLine(new string('-', 50));
        }
    }
}
