namespace QueryOptimizations
{
    using System.Collections.Generic;
    using System.Linq;
    using LinqToDB;
    using LinqToDB.Configuration;
    using LinqToDB.Data;
    using Models;

    public class CatsDataConnection : DataConnection
    {
        public CatsDataConnection()
            : base("CatsDemoDb")
        {
        }

        public ITable<Cat> Cats => this.GetTable<Cat>();

        public ITable<Owner> Owners => this.GetTable<Owner>();

        public class ConnectionStringSettings : IConnectionStringSettings
        {
            public string ConnectionString { get; set; }
            public string Name { get; set; }
            public string ProviderName { get; set; }
            public bool IsGlobal => false;
        }

        public class DataSettings : ILinqToDBSettings
        {
            public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

            public string DefaultConfiguration => "SqlServer";

            public string DefaultDataProvider => "SqlServer";

            public IEnumerable<IConnectionStringSettings> ConnectionStrings
            {
                get
                {
                    yield return
                        new ConnectionStringSettings
                        {
                            Name = "CatsDemoDb",
                            ProviderName = "SqlServer",
                            ConnectionString = Settings.ConnectionString
                        };
                }
            }
        }
    }
}
