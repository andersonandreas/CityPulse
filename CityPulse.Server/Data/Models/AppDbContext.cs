using Microsoft.EntityFrameworkCore;

namespace CityPulse.Server.Data.Models
{
	public class AppDbContext : DbContext
	{
		public DbSet<Country> Countries => Set<Country>();
		public DbSet<City> Cities => Set<City>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfigurationsFromAssembly(
				typeof(AppDbContext).Assembly);
		}


	}
}
