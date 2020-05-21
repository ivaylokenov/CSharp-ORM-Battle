namespace QueryOptimizations.Models
{
    using System;
    using LinqToDB.Mapping;

    [Table(Name = "Cats")]
    public class Cat
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public int Age { get; set; }

        [Column]
        public DateTime BirthDate { get; set; }

        [Column]
        public string Color { get; set; }

        [Column]
        public int OwnerId { get; set; }

        public virtual Owner Owner { get; set; }
    }
}
