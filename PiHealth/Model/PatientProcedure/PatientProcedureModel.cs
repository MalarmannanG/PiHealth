using System;

namespace PiHealth.Web.Model.PatientProcedureModel
{
    public class PatientProcedureModel
    {
        public long id { get; set; }
        public string diagnosis { get; set; }
        public string date { get; set; }
        public string procedurename { get; set; }
        public long? referedBy { get; set; }
        public string referedByName { get; set; }
        public string anesthesia { get; set; }
        public string description { get; set; }
        public string complication { get; set; }
        public string others { get; set; }
        public double? actualCost { get; set; }
        public long createdBy { get; set; }
        public DateTime? createdOn { get; set; }
        public long modifiedBy { get; set; }
        public DateTime? modifiedOn { get; set; }

    }
}
