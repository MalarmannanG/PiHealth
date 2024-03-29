﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiHealth.DataModel.Entity;
using PiHealth.Web.Model.Patient;

namespace PiHealth.Web.MappingExtention
{
    public static class PatientMapping
    {
        public static Patient ToEntity(this PatientModel model, Patient entity)
        {
            entity.UlID = model.ulId;
            entity.Age = model.age ?? 0;
            entity.Address = model.address;
            entity.DOB = model.dob;
            entity.PatientGender = model.gender;
            entity.PatientName = model.patientName;
            entity.Initial = model.initial;
            entity.MobileNumber = model.mobileNumber;
            entity.HrNo = model.hrno;
            entity.AttendarName = model.attendarName;
            entity.DoctorMasterId = model.referedBy;            
            entity.IsDeleted = model.isDeleted;

            return entity;
        }

        public static PatientModel ToModel(this Patient entity, PatientModel model)
        {
            model.id = entity.Id;
            model.ulId = entity.UlID;
            model.address = entity.Address;
            model.dob = entity.DOB;
            model.age = entity.DOB.HasValue  ? CalculateAge(entity.DOB.Value) : entity.Age;
            model.gender = entity.PatientGender;
            model.patientName = entity.PatientName;
            model.initial = entity.Initial;
            model.mobileNumber = entity.MobileNumber;
            model.referedBy = entity.DoctorMasterId;
            model.referedByName = entity.DoctorMaster?.Name;
            model.isDeleted = entity.IsDeleted;            
            model.hrno = entity.HrNo;            
            model.attendarName = entity.AttendarName;
            model.createdBy = entity.CreatedBy;
            model.createdOn = entity.CreatedDate;

            return model;
        }
    
        public static int CalculateAge(DateTime bob)
        {

            int year = DateTime.Now.Year - bob.Year;
            int _month = DateTime.Now.Month - bob.Month;
            if (_month < 0 || (_month == 0 && DateTime.Now.Day < bob.Day))
                year--;
            return year;

        }
    }
}
