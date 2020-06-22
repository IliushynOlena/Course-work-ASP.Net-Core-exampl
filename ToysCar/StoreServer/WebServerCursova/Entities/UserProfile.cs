using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebServerCursova.Entities
{
    [Table("tblUserProfile")]
    public class UserProfile
    {
        [Key]
        [ForeignKey("DbUser")]
        public int DbUserId { get; set; }

        [Required, StringLength(maximumLength: 255)]
        public string FirstName { get; set; }

        [Required, StringLength(maximumLength: 255)]
        public string LastName { get; set; }

        [Required, StringLength(maximumLength: 255)]
        public string Phone { get; set; }

        public virtual DbUser DbUser { get; set; }
    }
}
