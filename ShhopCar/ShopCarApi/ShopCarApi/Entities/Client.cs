using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  ShopCarApi.Entities
{
    [Table("tblClients")]
    public class Client
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [Required]
        public string UniqueName { get; set; }
        [Required]
        public string Phone { get; set; }

        [Required]
        public string Email { get; set; }

        public float Total_price { get; set; }
    }
}
