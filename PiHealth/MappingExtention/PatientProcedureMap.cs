using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiHealth.DataModel.Entity;
using PiHealth.Web.Model.PatientProcedureModel;
using PiHealth.Web.Model.PatientProfile;

namespace PiHealth.Web.MappingExtention
{
    public static class PatientProcedureMap
    {
        public static PatientProcedure ToEntity(this PatientProcedureModel model, PatientProcedure entity)
        {
            entity.Id = model.id;
            entity.DoctorMasterId = model.referedBy;
            entity.Date = model.date;
            entity.Description = model.description;
            entity.Diagnosis = model.diagnosis;
            entity.Others = model.others;
            entity.Procedurename = model.procedurename;
            entity.ActualCost = model.actualCost;
            entity.Anesthesia = model.anesthesia;
            entity.Complication = model.complication;
            
            return entity;
        }

        public static PatientProcedureModel ToModel(this PatientProcedure entity, PatientProcedureModel model)
        {
            model.id = entity.Id;
            model.referedBy = entity.DoctorMasterId;
            model.date = entity.Date;
            model.description = entity.Description;
            model.diagnosis = entity.Diagnosis;
            model.others = entity.Others;
            model.procedurename = entity.Procedurename;
            model.actualCost = entity.ActualCost;
            model.anesthesia = entity.Anesthesia;
            model.complication = entity.Complication;
            model.createdBy = entity.CreatedBy;
            model.createdOn = entity.CreatedDate;
            return model;
        }

        public static ProcedureModel ToModel1(this PatientProcedure entity, ProcedureModel model)
        {
            model.id = entity.Id;
            model.referedBy = entity.DoctorMasterId;
            model.date = entity.Date;
            model.description = entity.Description;
            model.diagnosis = entity.Diagnosis;
            model.others = entity.Others;
            model.name = entity.Procedurename;
            model.actualCost = entity.ActualCost;
            model.anesthesia = entity.Anesthesia;
            model.complication = entity.Complication;
            model.createdBy = entity.CreatedBy;
            model.createdDate = entity.CreatedDate;
            return model;
        }
    }
}
