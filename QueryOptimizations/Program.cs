namespace QueryOptimizations
{
    using System;
    using System.Linq;
    using Battle;
    using LinqToDB.Data;
    using Models;
    using RepoDb;
    using Results;

    public class Program
    {
        // DISCLAIMER: I am using System.Diagnostics.Stopwatch to measure performance as it is 
        // faster and it suits my presentation purposes. For real-life scenarios, I suggest using
        // Benchmark.NET.
        //
        // I am also using the latest preview versions of all frameworks.
        // These may perform differently compared to stable releases.
        //
        // You may find a better ORM performance comparison here:
        // https://github.com/FransBouma/RawDataAccessBencher/

        public static void Main()
        {
            // LINQ to DB
            DataConnection.DefaultSettings = new CatsDataConnection.DataSettings();

            // RepoDB
            SqlServerBootstrap.Initialize();
            ClassMapper.Add<Cat>("[dbo].[Cats]");
            ClassMapper.Add<Owner>("[dbo].[Owners]");

            // DataSeeder.Seed();

            TooManyQueriesBattle.Fight();
            // LazyLoadingTooManyQueriesBattle.Fight();
            // SelectSpecificColumnsBattle.Fight();
            // IncorrectDataLoadingBattle.Fight();
            // InnerQueryBattle.Fight();
            // DeleteBattle.Fight();
            // DataSeeder.DeleteAndSeed();
            // ColdAndWarmQueriesBattle.Fight();
            // CompiledQueriesBattle.Fight();
            // ChangeTrackingBattle.Fight();
        }
    }
}
