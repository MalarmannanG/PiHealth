using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace PiHealth.DataModel.Entity.Mapping
{
    public class PatientProcedureMap : EntityTypeConfiguration<PatientProcedure>
    {
        public override void Map(EntityTypeBuilder<PatientProcedure> builder)
        {
            builder.HasKey(t => t.Id);
            // Properties
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.Property(t => t.Description)
              .IsRequired(false);

            builder.ToTable("PatientProcedure");
 
        }
    }
}
