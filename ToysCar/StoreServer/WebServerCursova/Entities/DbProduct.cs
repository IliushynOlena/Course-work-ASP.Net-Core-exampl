using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServerCursova.Entities
{
    [Table("tblProducts")]
    public class DbProduct
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(maximumLength:255)]
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime DateCreate { get; set; }
        public string PhotoName { get; set; }
        public virtual ICollection<Filter> Filters { get; set; }

        [ForeignKey("Category")]
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
