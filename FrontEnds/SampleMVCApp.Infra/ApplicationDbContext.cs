using System;
using Microsoft.EntityFrameworkCore;
using SampleMVCApp.Domain;

namespace SampleMVCApp.Infra
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<MenuDTO> Menus { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            builder.Entity<MenuDTO>().HasKey(itm => itm.Key);
            builder.Entity<MenuDTO>().Property(itm => itm.Visible).HasDefaultValue<bool>(true);
            
        }
    }
}
