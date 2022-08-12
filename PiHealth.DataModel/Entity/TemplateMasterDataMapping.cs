using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.DataModel.Entity
{
    public class TemplateMasterDataMapping : BaseEntity
    {
        public TemplateMasterDataMapping()
        {

        }
        public long TemplateMasterId { get; set; }
        public virtual TemplateMaster TemplateMaster { get; set; }
        public long PatientProfileDataId { get; set; }
        public virtual PatientProfileData PatientProfileData { get; set; }
    }
}
