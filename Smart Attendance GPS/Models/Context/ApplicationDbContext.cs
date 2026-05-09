using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Smart_Attendance_GPS.Models.Context
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Specify precision and scale for the decimal properties
            builder.Entity<Company>()
                .Property(c => c.CompanyLatitude)
                .HasColumnType("decimal(9, 6)"); 

            builder.Entity<Company>()
                .Property(c => c.CompanyLongitude)
                .HasColumnType("decimal(9, 6)");

            
            builder.Entity<UserCompany>()
            .HasKey(uc => new { uc.UserId, uc.CompanyId });
            builder.Entity<UserCompany>()
            .HasOne(uc => uc.User)
            .WithMany(u => u.UserCompanies)
            .HasForeignKey(uc => uc.UserId);

            builder.Entity<UserCompany>()
                .HasOne(uc => uc.Company)
                .WithMany(c => c.UserCompanies)
                .HasForeignKey(uc => uc.CompanyId);

            #region Renaming IdentityTables
            // Renaming the Identity tables
            builder.Entity<ApplicationUser>(entity =>{entity.ToTable(name: "Users");});

            builder.Entity<IdentityRole>(entity =>{entity.ToTable(name: "Roles");});

            builder.Entity<IdentityUserRole<string>>(entity =>{entity.ToTable(name: "UserRoles");});

            builder.Entity<IdentityUserClaim<string>>(entity =>{entity.ToTable(name: "UserClaims");});

            builder.Entity<IdentityUserLogin<string>>(entity =>{entity.ToTable(name: "UserLogins");});

            builder.Entity<IdentityRoleClaim<string>>(entity =>{entity.ToTable(name: "RoleClaims");});

            builder.Entity<IdentityUserToken<string>>(entity =>{entity.ToTable(name: "UserTokens");});
            #endregion
        }
        public DbSet<UserCompany> UserCompanies { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
    }
}
