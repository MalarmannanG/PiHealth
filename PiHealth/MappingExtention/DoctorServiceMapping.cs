using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiHealth.DataModel.Entity;
using PiHealth.Web.Model.DoctorService;

namespace PiHealth.Web.MappingExtention
{
    public static class DoctorServiceMapping
    {
        public static DoctorService ToEntity(this DoctorServiceModel model, DoctorService entity)
        {
            entity.ServiceName = model.serviceName;
            entity.AppUserId = model.userId;
            entity.Fees = model.fees;
            entity.IsDeleted = model.isDeleted;
            entity.Id = model.id;
            return entity;
        }

        public static DoctorServiceModel ToModel(this DoctorService entity, DoctorServiceModel model)
        {
            model.serviceName = entity.ServiceName;
            model.userId = entity.AppUserId;
            model.fees = entity.Fees;
            model.doctorName = entity?.AppUser?.Name;
            model.isDeleted = entity.IsDeleted;
            model.id = entity.Id;
            return model;
        }
    }
}
