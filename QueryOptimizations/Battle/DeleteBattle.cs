namespace QueryOptimizations.Battle
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Linq;
    using Dapper;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using RepoDb;
    using Z.EntityFramework.Plus;
    
    public static class DeleteBattle
    {
        public static void Fight()
        {
            Console.WriteLine("Delete Battle");

            var stopWatch = Stopwatch.StartNew();

            // EF Core Makes 2 Queries - One Read and One Delete
            using (var db = new CatsDbContext())
            {
                var cat = db.Cats.Find(1);

                db.Remove(cat);

                db.SaveChanges();

                Console.WriteLine($"EF Core - 2 Queries: {stopWatch.Elapsed}");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Makes One Query - Only Delete
            using (var db = new CatsDbContext())
            {
                var cat = new Cat { Id = 2 };

                db.Remove(cat);

                db.SaveChanges();

                Console.WriteLine($"EF Core - 1 Query: {stopWatch.Elapsed}");
            }

            Console.WriteLine(new string('-', 20));

            stopWatch = Stopwatch.StartNew();

            // EF Core Delete Multiple Rows - Slow
            using (var db = new CatsDbContext())
            {
                var catsToDelete = db.Cats
                    .Where(c => c.Age == 1)
                    .Select(c => c.Id)
                    .ToList();

                db.RemoveRange(catsToDelete.Select(id => new Cat { Id = id }));

                db.SaveChanges();

                Console.WriteLine($"EF Core Delete Multiple Rows - Remove Range: {stopWatch.Elapsed} - {catsToDelete.Count} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Core Delete Multiple Rows - Fast with SQL
            using (var db = new CatsDbContext())
            {
                var deleted = db.Database.ExecuteSqlInterpolated($"DELETE FROM Cats WHERE Age = {2}");

                Console.WriteLine($"EF Core Delete Multiple Rows - SQL: {stopWatch.Elapsed} - {deleted} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // EF Plus
            using (var db = new CatsDbContext())
            {
                var catsToDelete = db.Cats
                    .Where(c => c.Age == 3)
                    .Delete();

                Console.WriteLine($"EF Plus: {stopWatch.Elapsed} - {catsToDelete} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // LINQ to DB Delete Multiple Rows
            using (var db = new CatsDataConnection())
            {
                var deleted = LinqToDB.LinqExtensions.Delete(db.Cats, c => c.Age == 4);

                Console.WriteLine($"LINQ to DB: {stopWatch.Elapsed} - {deleted} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // RepoDB Delete Multiple Rows
            using (var sqlConnection = new SqlConnection(Settings.ConnectionString).EnsureOpen())
            {
                var deleted = sqlConnection.Delete<Cat>(c => c.Age == 5);

                Console.WriteLine($"RepoDB Delete Multiple Rows: {stopWatch.Elapsed} - {deleted} Results");
            }

            stopWatch = Stopwatch.StartNew();

            // Dapper 
            using var connection = new SqlConnection(Settings.ConnectionString);

            var dapperDeleted = connection.Execute("DELETE FROM Cats WHERE Age = @Id", new { Id = 6 });

            Console.WriteLine($"Dapper: {stopWatch.Elapsed} - {dapperDeleted} Results");

            Console.WriteLine(new string('-', 50));
        }
    }
}
