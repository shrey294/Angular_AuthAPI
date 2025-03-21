using Angular_AuthAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Angular_AuthAPI.Context
{
	public class AppDbContext:DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
		{
			
		}
		public DbSet<User> Users { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>().ToTable("users");
		}
	}
}
