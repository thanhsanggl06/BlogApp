using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Data
{
    public class AuthDbContext : IdentityDbContext
    {
        private readonly IConfiguration configuration;

        public AuthDbContext(DbContextOptions<AuthDbContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = configuration.GetConnectionString("MyBlogConnectionString");
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            var readerRoleId = "4f1619d7-e0dd-4292-90d0-1e10a048599c";
            var writerRoleID = "7bd33c7e-f8e8-48cf-94c7-9372fdc4ed2d";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = readerRoleId,
                    Name = "Reader",
                    NormalizedName = "Reader".ToUpper(),
                    ConcurrencyStamp = readerRoleId
                },
                new IdentityRole
                {
                    Id = writerRoleID,
                    Name = "Writer",
                    NormalizedName = "Writer".ToUpper(),
                    ConcurrencyStamp = writerRoleID
                }
            };


            //Seed the role
            builder.Entity<IdentityRole>().HasData(roles);

            //Create an Admin user
            var adminUserId = "8e4d163e-575c-48f0-b5ec-956d1ed1b71b";
            var admin = new IdentityUser
            {
                Id = adminUserId,
                UserName = "admin@blogts.com",
                Email = "admin@blogts.com",
                NormalizedEmail = "admin@blogts.com".ToUpper(),
                NormalizedUserName = "admin@blogts.com".ToUpper(),
            };

            admin.PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(admin, "admin");

            builder.Entity<IdentityUser>().HasData(admin);

            //Give Roles to admin
            var adminRoles = new List<IdentityUserRole<string>>()
            {
                new()
                {
                    UserId = adminUserId,
                    RoleId = readerRoleId
                },
                new()
                {
                    UserId = adminUserId,
                    RoleId = writerRoleID
                }
            };

            builder.Entity<IdentityUserRole<string>>().HasData(adminRoles);
        }
    }
}
