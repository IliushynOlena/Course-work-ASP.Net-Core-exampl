using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace WebServerCursova.Entities
{
    public class DbRole : IdentityRole<int>
    {
        public ICollection<DbUserRole> UserRoles { get; set; }
    }
}

