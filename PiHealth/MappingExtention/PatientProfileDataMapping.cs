using PiHealth.DataModel.Entity;
using PiHealth.Model.PatientProfileData;

namespace PiHealth.MappingExtention
{
    public static class PatientProfileDataMapping
    {
        public static PatientProfileDataModel ToModel(this PatientProfileData entity,PatientProfileDataModel model)
        {
            model.id = entity.Id;
            model.key = entity.Key;
            model.description = entity.Description;
            model.isDeleted = entity.IsDeleted;
            model.createdDate = entity.CreatedDate;
            model.modifiedDate = entity.ModifiedDate;
            model.createdBy = entity.CreatedBy;
            model.modifiedBy = entity.ModifiedBy;
            return model;
        }

        public static PatientProfileData ToEntity(this PatientProfileDataModel model, PatientProfileData entity)
        {
            entity.Id = model.id ?? 0;
            entity.Key = model.key; 
            entity.Description = model.description;
            entity.IsDeleted = model.isDeleted;
            entity.CreatedDate = model.createdDate;
            entity.ModifiedDate = model.modifiedDate;
            entity.CreatedBy = model.createdBy;
            entity.ModifiedBy = model.modifiedBy;
            return entity;
        }
    }
}
