using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.DataModel.Entity
{
    public class ProcedureMaster : BaseEntity
    {
        public ProcedureMaster()
        {
            PatientProcedure = new List<PatientProcedure>();
        }
        public string Diagnosis { get; set; }
        public string Date { get; set; }
        public string Procedurename { get; set; }
        public string Anesthesia { get; set; }
        public string Description { get; set; }
        public string Complication { get; set; }
        public string Others { get; set; }
        public long? ActualCost { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public long CreatedBy { get; set; }
        public long? ModifiedBy { get; set; }
        public virtual ICollection<PatientProcedure> PatientProcedure { get; set; }
    }
}
