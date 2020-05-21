namespace QueryOptimizations.Models
{
    using System.Collections.Generic;

    using LinqToDB.Mapping;

    [Table(Name = "Owners")]
    public class Owner
    {
        [PrimaryKey, Identity]
        public int Id { get; set; }

        [Column]
        public string Name { get; set; }

        // public byte[] ProfilePicture { get; set; }

        public virtual ICollection<Cat> Cats { get; set; } = new HashSet<Cat>();
    }
}
