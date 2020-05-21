namespace QueryOptimizations
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Results;

    public class CatQueries
    {
        public static Func<CatsDbContext, int, string, IEnumerable<CatFamilyResult>> CatQuery
            => EF.CompileQuery((CatsDbContext db, int age, string nameStart) =>
                db.Cats
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
                    }));
    }
}
