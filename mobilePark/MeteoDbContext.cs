using Microsoft.EntityFrameworkCore;
using mobilePark.Models;
using System.Collections.Generic;

namespace mobilePark
{
	public class MeteoDbContext : DbContext
	{
		public MeteoDbContext(DbContextOptions<MeteoDbContext> options) : base(options)
		{
			Database.EnsureCreated();
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

		}

		public DbSet<MeteoDataDbModel> MeteoDataPackages => Set<MeteoDataDbModel>();
	}
}
