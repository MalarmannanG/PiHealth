using System;
using System.Collections.Generic;
using System.Text;

namespace PiHealth.DataModel.Entity
{
    public class PrescriptionMaster : BaseEntity
    {
        public PrescriptionMaster()
        {

        }
        public string GenericName { get; set; }
        public string CategoryName { get; set; }
        public string MedicineName { get; set; }
        public string Strength { get; set; }
        public string Units { get; set; }
        public string Remarks { get; set; }
        public string Instructions { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int NoOfDays { get; set; }
        public bool BeforeFood { get; set; }
        public string Morning { get; set; }
        public string Noon { get; set; }
        public string Night { get; set; }
        public bool Sos { get; set; }
        public bool Stat { get; set; }
    }
}
