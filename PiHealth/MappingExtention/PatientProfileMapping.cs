using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using PiHealth.Web.Model.Appointment;
using PiHealth.Web.Model.PatientProfile;

namespace PiHealth.Web.MappingExtention
{
    public static class PatientProfileMapping
    {
        public static PatientProfileModel ToModel(this PatientProfile entity, PatientProfileModel model)
        {
            model.id = entity.Id;
            model.patientId = entity.PatientId;
            model.doctorId = entity.DoctorId;
            model.appointmentId = entity.AppointmentId;
            model.compliants = entity.Compliants;
            model.examination = entity.Examination;
            model.impression = entity.Impression;
            model.advice = entity.Advice;
            model.templateMasterId = entity.TemplateMasterId;
            model.procedureMasterId = entity.ProcedureMasterId;
            model.pastHistory = entity.PastHistory;
            model.investigationResults = entity.InvestigationResults;
            model.plan = entity.Plan;
            model.fees = entity.Fees;
            model.isfollowUpNeed = entity.IsfollowUpNeed;
            model.followUp = entity.FollowUp;
            model.isDeleted = entity.IsDeleted;
            model.createdBy = entity.CreatedBy;
            model.createdDate = entity.CreatedDate;
            model.modifiedBy = entity.ModifiedBy;
            model.modifiedDate = entity.ModifiedDate;
            model.referredDoctor = entity.ReferredDoctor;
            model.doctorServiceId = entity.DoctorServiceId;
            model.patientModel = entity.Patient?.ToModel(new Model.Patient.PatientModel());
            model.appointment = entity.Appointment?.ToModel(new AppointmentModel());
            model.prescriptionModel = entity.Prescriptions?.ToList()?.Select(a => a.ToModel(new PrescriptionModel())).ToList() ?? new List<PrescriptionModel>();
            model.patientDiagnosisModel = entity.PatientDiagnosis?.ToList()?.Select(a => a.ToModel(new PatientDiagnosisModel())).ToList() ?? new List<PatientDiagnosisModel>();
            model.patientTestModel = entity.PatientTests?.ToList()?.Select(a => a.ToModel(new PatientTestModel())).ToList() ?? new List<PatientTestModel>();
            return model;
        }

        public static PatientProfile ToEntity(this PatientProfileModel model, PatientProfile entity)
        {
            entity.Compliants = model.compliants;
            entity.Examination = model.examination;
            entity.PastHistory = model.pastHistory;
            entity.InvestigationResults = model.investigationResults;
            entity.Impression = model.impression;
            entity.TemplateMasterId = model.templateMasterId;
            entity.ProcedureMasterId = model.procedureMasterId;
            entity.Advice = model.advice;
            entity.Plan = model.plan;
            entity.IsfollowUpNeed = model.isfollowUpNeed;
            entity.FollowUp = model.followUp;
            entity.IsDeleted = model.isDeleted;
            entity.Fees = model.fees;
            entity.CreatedBy =model.createdBy;            
            entity.CreatedDate = model.createdDate;
            entity.ModifiedBy = model.modifiedBy;
            entity.ModifiedDate = model.modifiedDate;
            entity.PatientId = model.patientId;
            entity.AppointmentId = model.appointmentId;
            entity.ReferredDoctor = model.referredDoctor;
            entity.DoctorServiceId = model.doctorServiceId;
            entity.Prescriptions = model.prescriptionModel?.ToList()?.Select(a => a.ToEntity(new Prescription())).ToList() ?? new List<Prescription>();
            entity.PatientDiagnosis = model.patientDiagnosisModel?.ToList()?.Select(a => a.ToEntity(new PatientDiagnosis())).ToList() ?? new List<PatientDiagnosis>();
            entity.PatientTests = model.patientTestModel?.ToList()?.Select(a => a.ToEntity(new PatientTest())).ToList() ?? new List<PatientTest>();
            return entity;
        }


        public static PrescriptionModel ToModel(this Prescription entity, PrescriptionModel model)
        {
            model.id = entity.Id;
            model.patientProfileId = entity.PatientProfileId;
            model.medicineName = entity.MedicineName;
            model.genericName = entity.GenericName;
            model.categoryName = entity.CategoryName;
            model.units = entity.Units;
            model.strength = entity.Strength;
            model.beforeFood = entity.BeforeFood;
            if (!string.IsNullOrEmpty(entity.Remarks))
            {
                model.beforeFood = entity.Remarks.ToLower().Contains("before") ? true : false;
                model.beforeFood = entity.Remarks.ToLower().Contains("after") ? false : true;
            }
            model.afterFood = entity.AfterFood;
            model.morning = entity.Morning;
            model.noon = entity.Noon;
            model.night = entity.Night;
            model.remarks = entity.Remarks;
            model.instructions = entity.Instructions;
            model.noOfDays = entity.NoOfDays;
            model.isDeleted = entity.IsDeleted;
            model.createdBy = entity.CreatedBy;
            model.createdDate = entity.CreatedDate;
            model.modifiedBy = entity.ModifiedBy;
            model.modifiedDate = entity.ModifiedDate;
            model.sos = entity.Sos;
            model.stat = entity.Stat;
            return model;
        }

        public static Prescription ToEntity(this PrescriptionModel model, Prescription entity)
        {
            entity.PatientProfileId = model.patientProfileId;
            entity.MedicineName = model.medicineName;
            entity.GenericName = model.genericName;
            entity.Strength = model.strength;
            entity.Units = model.units;
            entity.CategoryName = model.categoryName;
            entity.BeforeFood = model.beforeFood;
            entity.AfterFood = model.afterFood;
            entity.Morning = model.morning;
            entity.Noon = model.noon;
            entity.Night = model.night;
            entity.Remarks = model.remarks;
            entity.Instructions = model.instructions;
            entity.NoOfDays = model.noOfDays;
            entity.IsDeleted = model.isDeleted;
            entity.CreatedBy = model.createdBy;
            entity.CreatedDate = model.createdDate;
            entity.ModifiedBy = model.modifiedBy;
            entity.ModifiedDate = model.modifiedDate;
            entity.Sos = model.sos;
            entity.Stat = model.stat;
            return entity;
        }

        public static PatientTestModel ToModel(this PatientTest entity, PatientTestModel model)
        {
            model.id = entity.Id;
            model.patientProfileId = entity.PatientProfileId;
            model.testMasterId = entity.TestMasterId;
            model.testMasterName = entity.TestMaster?.Name;            
            model.remarks = entity.Remarks;
            model.isDeleted = entity.IsDeleted;
            model.createdBy = entity.CreatedBy;
            model.createdDate = entity.CreatedDate;
            model.modifiedBy = entity.ModifiedBy;
            model.modifiedDate = entity.ModifiedDate;
            return model;
        }

        public static PatientTest ToEntity(this PatientTestModel model, PatientTest entity)
        {
            entity.PatientProfileId = model.patientProfileId;
            entity.TestMasterId = model.testMasterId;
            entity.Remarks = model.remarks;
            entity.IsDeleted = model.isDeleted;
            entity.CreatedBy = model.createdBy;
            entity.CreatedDate = model.createdDate;
            entity.ModifiedBy = model.modifiedBy;
            entity.ModifiedDate = model.modifiedDate;
            return entity;
        }

        public static PatientDiagnosisModel ToModel(this PatientDiagnosis entity, PatientDiagnosisModel model)
        {
            model.id = entity.Id;
            model.patientProfileId = entity.PatientProfileId;
            model.diagnosisMasterId = entity.DiagnosisMasterId;
            model.name = entity.DiagnosisMaster?.Name;            
            model.description = entity.Description;
            model.isDeleted = entity.IsDeleted;
            model.createdBy = entity.CreatedBy;
            model.createdDate = entity.CreatedDate;
            model.modifiedBy = entity.ModifiedBy;
            model.modifiedDate = entity.ModifiedDate;
            return model;
        }

        public static PatientDiagnosis ToEntity(this PatientDiagnosisModel model, PatientDiagnosis entity)
        {
            entity.PatientProfileId = model.patientProfileId;
            entity.DiagnosisMasterId = model.diagnosisMasterId;
            entity.Description = model.description;
            entity.IsDeleted = model.isDeleted;
            entity.CreatedBy = model.createdBy;
            entity.CreatedDate = model.createdDate;
            entity.ModifiedBy = model.modifiedBy;
            entity.ModifiedDate = model.modifiedDate;
            return entity;
        }

        public static PatientProcedure ToEntity(this ProcedureModel model, PatientProcedure entity)
        {
            entity.ActualCost = model.actualCost;
            entity.Description = model.description;
            entity.Anesthesia = model.anesthesia;
            entity.Complication = model.complication;
            entity.Date = model.date;
            entity.Diagnosis = model.diagnosis;
            entity.Procedurename = model.name;
            entity.DoctorMasterId = model.referedBy;
            entity.Others = model.others;
            entity.CreatedBy = model.createdBy;
            entity.CreatedDate = model.createdDate;
            entity.ModifiedBy = model.modifiedBy;
            entity.ModifiedDate = model.modifiedDate;
            return entity;
        }
        public static PatientProcedure ToModel(this PatientProcedure entity, ProcedureModel model)
        {
            model.actualCost = entity.ActualCost;
             model.description = entity.Description;
            model.anesthesia = entity.Anesthesia;
            model.complication = entity.Complication;
            model.date = entity.Date ;
            model.diagnosis = entity.Diagnosis;
            model.name = entity.Procedurename ;
            model.referedBy = entity.DoctorMasterId;
            model.others = entity.Others ;
            model.createdBy = entity.CreatedBy;
            model.createdDate = entity.CreatedDate ;
            model.modifiedBy = entity.ModifiedBy;
            model.modifiedDate = entity.ModifiedDate;
            return entity;
        }
    }
}
