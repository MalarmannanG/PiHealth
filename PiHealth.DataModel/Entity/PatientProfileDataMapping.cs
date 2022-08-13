using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.DataModel.Entity
{
    public class PatientProfileDataMapping : BaseEntity
    {
        public PatientProfileDataMapping()
        {

        }

        public long PatientProfileId { get; set; }
        public virtual PatientProfile PatientProfile { get; set; }
        public long PatientProfileDataId { get; set; }
        public virtual PatientProfileData PatientProfileData { get; set; }
    }
}
