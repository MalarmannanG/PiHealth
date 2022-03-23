using PiHealth.DataModel.Entity;
using PiHealth.Model.ProcedureMaster;

namespace PiHealth.MappingExtention
{
    public static class ProcedureMasterMapping
    {
        public static ProcedureMasterModel ToModel(this ProcedureMaster entity, ProcedureMasterModel model)
        {
            model.actualCost = entity.ActualCost;
            model.procedurename = entity.Procedurename;
            model.diagnosis = entity.Diagnosis;
            model.description = entity.Description;
            model.isDeleted = entity.IsDeleted;
            model.id = entity.Id;
            model.createdBy = entity.CreatedBy;
            model.createdDate = entity.CreatedDate;
            model.updatedBy = entity.ModifiedBy;
            model.updatedDate = entity.ModifiedDate;
            model.others = entity.Others;
            model.date = entity.Date;
            model.anesthesia = entity.Anesthesia;
            model.complication = entity.Complication;
            return model;
        }

        public static ProcedureMaster ToEntity(this ProcedureMasterModel model, ProcedureMaster entity)
        {
           entity.ActualCost = model.actualCost;
            entity.Procedurename = model.procedurename;
            entity.Diagnosis = model.diagnosis;
            entity.Description = model.description;
            entity.IsDeleted = model.isDeleted;
            entity.Id = model.id ;
            entity.CreatedBy  = model.createdBy;
            entity.CreatedDate = model.createdDate;
            entity.ModifiedBy = model.updatedBy;
            entity.ModifiedDate = model.updatedDate;
            entity.Others = model.others;
            entity.Date = model.date ;
            entity.Anesthesia = model.anesthesia;
            entity.Complication = model.complication;
            return entity;
        }
    }
}
