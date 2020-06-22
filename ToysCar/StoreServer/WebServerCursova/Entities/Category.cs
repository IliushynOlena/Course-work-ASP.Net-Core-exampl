using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServerCursova.Entities
{
    [Table("tblCategories")]
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [ForeignKey("Children")]
        public int? ParentId { get; set; }
        public Category Children { get; set; }

        public virtual ICollection<DbProduct> Products { get; set; }
    }
}
