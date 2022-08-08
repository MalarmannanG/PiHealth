using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.DataModel.Entity
{
    public class PatientProfileData : BaseEntity
    {
        public PatientProfileData()
        {

        }

        public long Key { get; set; }
        public string Description { get; set; }
    }
}
