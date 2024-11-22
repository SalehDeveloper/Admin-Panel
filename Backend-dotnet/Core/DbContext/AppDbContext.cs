using Backend_dotnet.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend_dotnet.Core.DbContext
{
    public class AppDbContext :IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
         :base(options)
        {
        }


        public DbSet<Log> Logs { get; set; }    

        public DbSet<Message> Messages { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //1
            builder.Entity<ApplicationUser>(e => { e.ToTable("Users"); });
            //2
            builder.Entity<IdentityUserClaim<string>>(e => { e.ToTable("UserClaims"); });
            //3
            builder.Entity<IdentityUserLogin<string>>(e => { e.ToTable("UserLogins"); });
            //4
            builder.Entity<IdentityUserToken<string>>(e => { e.ToTable("UserTokens"); });
            //5
            builder.Entity<IdentityRole>(e => { e.ToTable("Roles"); });
            //6
            builder.Entity<IdentityRoleClaim<string>>(e => { e.ToTable("RoleClaims"); });
            //7
            builder.Entity<IdentityUserRole<string>>(e => { e.ToTable("UserRoles"); });





        }

    }
}
