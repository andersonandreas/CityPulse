using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CityPulse.Server.Data.Models.EntityConfigs
{
    public class CountryEntityTypeConfig : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("Countries");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).IsRequired();
            builder.HasIndex(b => b.Name);
            builder.HasIndex(b => b.ISO2);
            builder.HasIndex(b => b.ISO3);

        }
    }
}
