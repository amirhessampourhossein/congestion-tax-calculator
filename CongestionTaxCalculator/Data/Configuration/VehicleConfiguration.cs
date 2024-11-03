using CongestionTaxCalculator.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CongestionTaxCalculator.Data.Configuration;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasMany(x => x.TaxRecords)
            .WithOne(x => x.Vehicle)
            .HasForeignKey(x => x.VehicleId);
    }
}
