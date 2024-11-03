using CongestionTaxCalculator.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CongestionTaxCalculator.Data.Configuration;

public class TaxRecordConfiguration : IEntityTypeConfiguration<TaxRecord>
{
    public void Configure(EntityTypeBuilder<TaxRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder
            .HasOne(x => x.Vehicle)
            .WithMany(x => x.TaxRecords)
            .HasForeignKey(x => x.VehicleId);
    }
}
