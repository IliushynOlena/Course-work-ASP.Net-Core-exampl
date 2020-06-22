using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebServerCursova.Entities
{
    public class EFDbContext : IdentityDbContext<DbUser, DbRole, int, IdentityUserClaim<int>,
    DbUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {  
        //Models
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<DbProduct> Products { get; set; }

        //Filters
        public DbSet<FilterName> FilterNames { get; set; }
        public DbSet<FilterValue> FilterValues { get; set; }
        public DbSet<FilterNameGroup> FilterNameGroups { get; set; }
        public DbSet<Filter> Filters { get; set; }

        //Categories
        public DbSet<Category> Categories { get; set; }

        public EFDbContext(DbContextOptions<EFDbContext> options)
         : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<DbUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();
                userRole.HasOne(ur => ur.User)
                     .WithMany(r => r.UserRoles)
                     .HasForeignKey(ur => ur.UserId)
                     .IsRequired();
            });

            builder.Entity<Filter>(filter =>
            {
                filter.HasKey(f => new { f.ProductId, f.FilterValueId, f.FilterNameId });
                filter.HasOne(ur => ur.FilterNameOf)
                    .WithMany(r => r.Filters)
                    .HasForeignKey(ur => ur.FilterNameId)
                    .IsRequired();
                filter.HasOne(ur => ur.FilterValueOf)
                    .WithMany(r => r.Filters)
                    .HasForeignKey(ur => ur.FilterValueId)
                    .IsRequired();
                filter.HasOne(ur => ur.ProductOf)
                    .WithMany(r => r.Filters)
                    .HasForeignKey(ur => ur.ProductId)
                    .IsRequired();
            });

            builder.Entity<FilterNameGroup>(fng =>
            {
                fng.HasKey(f => new { f.FilterValueId, f.FilterNameId });
                fng.HasOne(ur => ur.FilterNameOf)
                    .WithMany(r => r.FilterNameGroups)
                    .HasForeignKey(ur => ur.FilterNameId)
                    .IsRequired();
                fng.HasOne(ur => ur.FilterValueOf)
                    .WithMany(r => r.FilterNameGroups)
                    .HasForeignKey(ur => ur.FilterValueId)
                    .IsRequired();
            });
        }
    }
}
