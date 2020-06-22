using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebServerCursova.Entities
{
    public class DbUser : IdentityUser<int>
    {
        public ICollection<DbUserRole> UserRoles { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
