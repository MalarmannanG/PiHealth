using System;

namespace PiHealth.Model.ProcedureMaster
{
    public class ProcedureMasterModel
    {
        public long id { get; set; }
        public string diagnosis { get; set; }
        public string date { get; set; }
        public string procedurename { get; set; }
        public string anesthesia { get; set; }
        public string description { get; set; }
        public string complication { get; set; }
        public string others { get; set; }
        public long actualCost { get; set; }
        public bool isDeleted { get; set; }
        public DateTime createdDate { get; set; }
        public long createdBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public long? updatedBy { get; set; }
    }
    public class ProcedureMasterQueryModel
    {
        public string name { get; set; }
        public string orderBy { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
    }
}

