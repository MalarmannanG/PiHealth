using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.DataModel.Entity.Mapping
{
    public class PatientProfileDataMappingMap : EntityTypeConfiguration<PatientProfileDataMapping>
    {
        public override void Map(EntityTypeBuilder<PatientProfileDataMapping> builder)
        {
            builder.HasKey(t => t.Id);
            // Properties
            builder.Property(t => t.Id).ValueGeneratedOnAdd();

            builder.ToTable("PatientProfileDataMapping");

        }
    }
}
