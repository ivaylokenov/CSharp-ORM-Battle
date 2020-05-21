namespace DataLayerApproaches.Models
{
    using System.Collections.Generic;

    public class CatWithOwnerResponseModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Age { get; set; }

        public string Owner { get; set; }
    }
}
