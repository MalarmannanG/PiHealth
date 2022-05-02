using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PiHealth.DataModel.Entity.Mapping
{
    public class DoctorServiceMap : EntityTypeConfiguration<DoctorService>
    {
        public override void Map(EntityTypeBuilder<DoctorService> builder)
        {
            builder.HasKey(t => t.Id);
            // Properties
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.ServiceName)
              .IsRequired(true);

            builder.Property(t => t.Fees)
              .IsRequired(true);

            builder.ToTable("DoctorService");

        }
    }
}
