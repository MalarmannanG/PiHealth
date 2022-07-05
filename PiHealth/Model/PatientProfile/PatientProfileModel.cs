using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiHealth.Web.Model.Appointment;
using PiHealth.Web.Model.Patient;

namespace PiHealth.Web.Model.PatientProfile
{
    public class PatientProfileModel
    {
        public PatientProfileModel()
        {
            prescriptionModel = new List<PrescriptionModel>();
            patientDiagnosisModel = new List<PatientDiagnosisModel>();
            patientTestModel = new List<PatientTestModel>();
        }
        public long id { get; set; }
        public long? templateMasterId { get; set; }
        public long? procedureMasterId { get; set; }
        public long patientId { get; set; }
        public long doctorId { get; set; }
        public long appointmentId { get; set; }
        public string compliants { get; set; }
        public string examination { get; set; }
        public string pastHistory { get; set; }
        public string investigationResults { get; set; }
        public string impression { get; set; }
        public string advice { get; set; }
        public string plan { get; set; }
        public long fees { get; set; }
        public bool isfollowUpNeed { get; set; }
        public DateTime? followUp { get; set; }
        public string followUpDate { get; set; }
        public bool isDeleted { get; set; }
        public string referredDoctor { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public long createdBy { get; set; }
        public long? modifiedBy { get; set; }
        public long? doctorServiceId { get; set; }
        public DoctorService.DoctorServiceModel doctorService { get; set; }
        public PatientModel patientModel { get; set; }
        public AppointmentModel appointment { get; set; }
        public ProcedureModel procedureModel { get; set; }
        public List<PatientDiagnosisModel> patientDiagnosisModel { get; set; }
        public List<PrescriptionModel> prescriptionModel { get; set; }
        public List<PatientTestModel> patientTestModel { get; set; }
    }
    public class PatientProfileQueryModel
    {
        public long? PatientId { get; set; }
        public DateTime? appointmentDate { get; set; }
    }

    public class PatientTestModel
    {
        public PatientTestModel()
        {
        }
        public long id { get; set; }
        public long patientProfileId { get; set; }
        public long testMasterId { get; set; }
        public string testMasterName { get; set; }
        public string remarks { get; set; }
        public bool isDeleted { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public long createdBy { get; set; }
        public long? modifiedBy { get; set; }
    }



    public class PatientDiagnosisModel
    {
        public PatientDiagnosisModel()
        {

        }

        public long id { get; set; }
        public long patientProfileId { get; set; }
        public long diagnosisMasterId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool isDeleted { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public long createdBy { get; set; }
        public long? modifiedBy { get; set; }

    }

    public class PrescriptionModel
    {
        public PrescriptionModel()
        {

        }

        public long id { get; set; }
        public long patientProfileId { get; set; }
        public string medicineName { get; set; }
        public string categoryName { get; set; }
        public string genericName { get; set; }
        public string strength { get; set; }
        public string units { get; set; }
        public string remarks { get; set; }
        public string instructions { get; set; }
        public bool beforeFood { get; set; }
        public string morning { get; set; }
        public string noon { get; set; }
        public string night { get; set; }
        public int noOfDays { get; set; }
        public bool isDeleted { get; set; } 
        public bool sos { get; set; }
        public bool stat { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public long createdBy { get; set; }
        public long? modifiedBy { get; set; }
    }

    public class ProcedureModel
    {
        public ProcedureModel()
        {

        }
        public long id { get; set; }
        public string diagnosis { get; set; }
        public string date { get; set; }
        public string name { get; set; }
        public long? referedBy { get; set; }
        public string referedByName { get; set; }
        public string anesthesia { get; set; }
        public string description { get; set; }
        public string complication { get; set; }
        public string others { get; set; }
        public double? actualCost { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? modifiedDate { get; set; }
        public long createdBy { get; set; }
        public long? modifiedBy { get; set; }
    }
}
