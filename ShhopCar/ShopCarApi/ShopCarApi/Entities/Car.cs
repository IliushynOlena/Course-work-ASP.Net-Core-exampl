using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopCarApi.Entities
{
    [Table("tblCars")]
    public class Car
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string UniqueName { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Decimal Price { get; set; }

        [Required]
        public int Count { get; set; }

        public virtual ICollection<Filter> Filtres { get; set; }
    }
}
