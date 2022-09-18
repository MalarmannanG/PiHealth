

using Abp.Application.Services.Dto;
using PiHealth.Web.Model.PrescriptionMaster;
//using PiHealth.Extention;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiHealth.Web.Model.TemplateMaster
{
    public class TemplateMasterModel
    {
        public TemplateMasterModel()
        {
            templatePrescriptionModel = new List<TemplatePrescriptionModel>();
            templateComplaints = new List<PatientProfileDataMapModel>();
            templatePlans = new List<PatientProfileDataMapModel>();
            templateImpressions = new List<PatientProfileDataMapModel>();
            templateAdvices = new List<PatientProfileDataMapModel>();
            templateExaminations = new List<PatientProfileDataMapModel>();
        }
        public long id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string compliants { get; set; }
        public string examination { get; set; }
        public string impression { get; set; }
        public string advice { get; set; }
        public string plan { get; set; }
        public string followUp { get; set; }
        public bool isDeleted { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public long createdBy { get; set; }
        public long? modifiedBy { get; set; }
        public List<TemplatePrescriptionModel> templatePrescriptionModel { get; set; }
        public List<PatientProfileDataMapModel> templateComplaints { get; set; }
        public List<PatientProfileDataMapModel> templatePlans { get; set; }
        public List<PatientProfileDataMapModel> templateImpressions { get; set; }
        public List<PatientProfileDataMapModel> templateAdvices { get; set; }
        public List<PatientProfileDataMapModel> templateExaminations { get; set; }


    }

    public class TemplatePrescriptionModel
    {
        public TemplatePrescriptionModel()
        {

        }

        public long id { get; set; }
        public long templateMasterId { get; set; }
        public string medicineName { get; set; }
        public string strength { get; set; }
        public bool beforeFood { get; set; }
        public bool afterFood { get; set; }
        public string morning { get; set; }
        public string noon { get; set; }
        public string night { get; set; }
        public string remarks { get; set; }
        public string instructions { get; set; }
        public int noOfDays { get; set; }
        public bool isDeleted { get; set; }
        public string units { get; set; }
        public bool sos { get; set; }
        public bool stat { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public long createdBy { get; set; }
        public long? modifiedBy { get; set; }
        public long? prescriptionMasterId { get; set; }
        public PrescriptionMasterModel presciptionMaster { get; set; }
    }

    public class TemplateQueryModel   
    {
        public string name { get; set; }
        public string orderBy { get; set; }
        public int skip { get; set; }
        public int take { get; set; }
    }

    public class PatientProfileDataMapModel
    {
        public long templateMasterId { get; set; }
        public long patientProfileDataId { get; set; }
        public string description { get; set; }
        public long key { get; set; }
    }
}
