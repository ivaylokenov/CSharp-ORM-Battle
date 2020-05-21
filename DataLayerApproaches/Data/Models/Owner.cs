namespace DataLayerApproaches.Data.Models
{
    using System.Collections.Generic;

    public class Owner
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // public byte[] ProfilePicture { get; set; }

        public virtual ICollection<Cat> Cats { get; set; } = new HashSet<Cat>();
    }
}
