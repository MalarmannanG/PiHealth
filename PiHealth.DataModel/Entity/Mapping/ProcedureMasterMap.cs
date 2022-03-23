using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace PiHealth.DataModel.Entity.Mapping
{
    public class ProcedureMasterMap : EntityTypeConfiguration<ProcedureMaster>
    {
        public override void Map(EntityTypeBuilder<ProcedureMaster> builder)
        {
            builder.HasKey(t => t.Id);
            // Properties
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.ToTable("ProcedureMaster");

        }
    }
}
