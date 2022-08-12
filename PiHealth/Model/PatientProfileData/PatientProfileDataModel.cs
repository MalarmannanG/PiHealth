using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiHealth.Model.PatientProfileData
{
    public class PatientProfileDataModel
    {
        public long? id { get; set; }
        public long key { get; set; }
        public string description { get; set; }
        public bool isDeleted { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public long createdBy { get; set; }
        public long? modifiedBy { get; set; }
    }

    public class PatientProfileDataQueryModel
    {
        public long key { get; set; }
        public string description { get; set; }
        public string orderBy { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
    }
}
