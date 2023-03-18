using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DAL.Entities;

namespace DAL.Context;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Category> Category { get; set; }
    public DbSet<Item> Item { get; set; }

    public DbSet<RefreshToken> RefreshToken { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

		#region CATEGORY
		modelBuilder.Entity<Category>().HasKey(x => x.CategoryID);
        modelBuilder.Entity<Category>().Property(x => x.Name).HasColumnType("varchar(64)");
		#endregion

		#region ITEM
		modelBuilder.Entity<Item>().HasKey(x => x.ID);
        modelBuilder.Entity<Item>().Property(x => x.Name).HasColumnType("varchar(128)");
		modelBuilder.Entity<Item>().Property(x => x.ImagePath).HasColumnType("varchar(256)");
		modelBuilder.Entity<Item>()
                    .HasOne(x => x.Category)
                    .WithMany(x => x.Items)
                    .HasForeignKey(x => x.CategoryID);
		#endregion

		#region REFRESHTOKEN
		modelBuilder.Entity<RefreshToken>().HasKey(x => x.UserId);
        modelBuilder.Entity<RefreshToken>().Property(x => x.Token).HasColumnType("varchar(512)");
        modelBuilder.Entity<ApplicationUser>()
                    .HasOne(x => x.RefreshToken)
                    .WithOne(x => x.User)
                    .HasForeignKey<RefreshToken>(x => x.UserId);
		#endregion
	}
}
