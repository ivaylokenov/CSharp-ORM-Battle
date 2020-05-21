namespace QueryOptimizations.Results
{
    using System.Collections.Generic;

    public class CatSiblingsResult
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<string> Siblings { get; set; }
    }
}
