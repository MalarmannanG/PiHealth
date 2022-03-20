using PiHealth.DataModel.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.DataModel.Entity
{
    public class PatientProcedure : BaseEntity
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Diagnosis { get; set; }
        public string Date { get; set; }
        public string Procedurename { get; set; }
        public long? DoctorMasterId { get; set; }
        public DoctorMaster DoctorMaster { get; set; }
        public string Anesthesia { get; set; }
        public string Description { get; set; }
        public string Complication { get; set; }
        public string Others { get; set; }
        public double? ActualCost { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        [ForeignKey("PatientProcedure")]
        public long PatientProfileId { get; set; }
        public virtual PatientProfile PatientProfile { get; set; }
    }
}
