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

    public static class LazyLoadingTooManyQueriesBattle
    {
        public static void Fight()
        {
            Console.WriteLine("Lazy Loading Too Many Queries Battle");

            var stopWatch = Stopwatch.StartNew();

            // EF Core N+1 Lazy Loading
            using (var db = new CatsDbContext(true))
            {
                var cats = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .ToList();

                var ownerNames = new List<string>();

                // Usually in another method
                foreach (var cat in cats)
                {
                    var ownerName = cat.Owner.Name;
                    ownerNames.Add(ownerName);
                }

                Console.WriteLine($"EF Core N+1 Lazy Loading: {stopWatch.Elapsed} - {ownerNames.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Using Join
            using (var db = new CatsDbContext())
            {
                var ownerNames = db.Cats
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Owners,
                        c => c.OwnerId,
                        o => o.Id, (c, o) => o.Name)
                    .ToList();

                Console.WriteLine($"EF Core Join: {stopWatch.Elapsed} - {ownerNames.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Using Select
            using (var db = new CatsDbContext())
            {
                var ownerNames = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .Select(c => c.Owner.Name)
                    .ToList();

                Console.WriteLine($"EF Core Using Select: {stopWatch.Elapsed} - {ownerNames.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Using Select Cached
            using (var db = new CatsDbContext())
            {
                var ownerNames = db.Cats
                    .Where(c => c.Name.Contains("1"))
                    .Select(c => c.Owner.Name)
                    .ToList();

                Console.WriteLine($"EF Core Using Select Cached: {stopWatch.Elapsed} - {ownerNames.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB
            using (var db = new CatsDataConnection())
            {
                // LINQ to DB does not support Select joins
                // You need to join your tables explicitly

                var ownerNames = db.Cats
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Owners,
                        c => c.OwnerId,
                        o => o.Id, (c, o) => o.Name)
                    .ToList();

                Console.WriteLine($"LINQ to DB Join: {stopWatch.Elapsed} - {ownerNames.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Cached
            using (var db = new CatsDataConnection())
            {
                // LINQ to DB does not support Select joins
                // You need to join your tables explicitly

                var ownerNames = db.Cats
                    .Where(o => o.Name.Contains("1"))
                    .Join(db.Owners,
                        c => c.OwnerId,
                        o => o.Id, (c, o) => o.Name)
                    .ToList();

                Console.WriteLine($"LINQ to DB Join Cached: {stopWatch.Elapsed} - {ownerNames.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // RepoDB Query And In Memory Join
            using (var connection = new SqlConnection(Settings.ConnectionString).EnsureOpen())
            {
                var dbOwners = connection.QueryAll<Owner>();
                var dbCats = connection.Query<Cat>(c => c.Name.Contains("1"));

                var cats = dbOwners
                    .Join(dbCats,
                        o => o.Id,
                        c => c.OwnerId, (o, c) => c.Name)
                    .ToList();

                Console.WriteLine($"RepoDB Query And In Memory Join: {stopWatch.Elapsed} - {cats.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper With EF Query
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var ownerNames = connection.Query<string>(
                    @"SELECT [o].[Name]
                    FROM [Cats] AS [c]
                    INNER JOIN [Owners] AS [o] ON [c].[OwnerId] = [o].[Id]
                    WHERE CHARINDEX(N'1', [c].[Name]) > 0");

                Console.WriteLine($"Dapper (EF): {stopWatch.Elapsed} - {ownerNames.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper With EF Query Cached
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<string>(
                    @"SELECT [o].[Name]
                    FROM [Cats] AS [c]
                    INNER JOIN [Owners] AS [o] ON [c].[OwnerId] = [o].[Id]
                    WHERE CHARINDEX(N'1', [c].[Name]) > 0");

                Console.WriteLine($"Dapper (EF) Cached: {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper With LINQ to DB Query
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<string>(
                    @"SELECT
	                    [o_1].[Name]
                    FROM
	                    [Cats] [o_2]
		                    INNER JOIN [Owners] [o_1] ON [o_2].[OwnerId] = [o_1].[Id]
                    WHERE
	                    [o_2].[Name] LIKE N'%1%'");

                Console.WriteLine($"Dapper (LINQ to DB): {stopWatch.Elapsed} - {cats.Count()} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper With LINQ to DB Query Cached
            using (var connection = new SqlConnection(Settings.ConnectionString))
            {
                var cats = connection.Query<string>(
                    @"SELECT
	                    [o_1].[Name]
                    FROM
	                    [Cats] [o_2]
		                    INNER JOIN [Owners] [o_1] ON [o_2].[OwnerId] = [o_1].[Id]
                    WHERE
	                    [o_2].[Name] LIKE N'%1%'");

                Console.WriteLine($"Dapper (LINQ to DB) cached: {stopWatch.Elapsed} - {cats.Count()} Results");
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
                            FROM
	                            [Cats] [o_2]
		                            INNER JOIN [Owners] [o_1] ON [o_2].[OwnerId] = [o_1].[Id]
                            WHERE
	                            [o_2].[Name] LIKE N'%1%'")
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
