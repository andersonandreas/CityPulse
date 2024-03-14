using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityPulse.Server.Data.Models.EntityConfigs
{
	public class CityEntityTypeConfig : IEntityTypeConfiguration<City>
	{
		public void Configure(EntityTypeBuilder<City> builder)
		{

			builder.ToTable("Cities");
			builder.HasKey(b => b.Id);
			builder.Property(b => b.Id).IsRequired();
			builder.HasOne(b => b.Country)
				.WithMany(b => b.Cities)
				.HasForeignKey(b => b.CountryId);
			builder.Property(b => b.Lat).HasColumnType("decimal(7,4)");
			builder.Property(b => b.Lon).HasColumnType("decimal(7,4)");
			builder.HasIndex(b => b.Name);
			builder.HasIndex(b => b.Lat);
			builder.HasIndex(b => b.Lon);

		}
	}
}
