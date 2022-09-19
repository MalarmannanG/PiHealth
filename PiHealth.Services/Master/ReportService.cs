﻿using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PiHealth.Services.Master
{
    public class ReportService
    {
        public readonly IRepository<Patient> _repositoryPatient;
        public readonly IRepository<Appointment> _repositoryAppointment;
        public readonly IRepository<PatientProfile> _repositoryPatientProfile;
        public readonly IRepository<PatientDiagnosis> _repositoryPatientDiagnosis;
        public readonly IRepository<DiagnosisMaster> _repositoryDiagnosisMaster;
        public readonly IRepository<DoctorService> _repositoryDoctorService;

        public ReportService(IRepository<Patient> repositoryPatient,
            IRepository<Appointment> repositoryAppointment,
            IRepository<PatientProfile> repositoryPatientProfile,
            IRepository<PatientDiagnosis> repositoryPatientDiagnosis,
            IRepository<DiagnosisMaster> repositoryDiagnosisMaster,
            IRepository<DoctorService> repositoryDoctorService)
        {
            _repositoryPatient = repositoryPatient;
            _repositoryAppointment = repositoryAppointment;
            _repositoryPatientProfile = repositoryPatientProfile;
            _repositoryPatientDiagnosis = repositoryPatientDiagnosis;
            _repositoryDiagnosisMaster = repositoryDiagnosisMaster;
            _repositoryDoctorService = repositoryDoctorService;
        }

        public class Report
        {
            public string data { get; set; }
            public int count { get; set; }
            public string date { get; set; }
        }

        public class DataFormat
        {
            public string data { get; set; }
            public int itemCount { get; set; }
        }
        public class ResponseData
        {
            public string date { get; set; }
            public List<DataFormat> dataItems { get; set; }
        }

        public class FeesDataModel
        {
            public string date { get; set; }
            public string patientName { get; set; }
            public string serviceName { get; set; }
            public long fees { get; set; }
        }

        public virtual List<FeesDataModel> GetFeesData(string name=null, long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<FeesDataModel> response = new List<FeesDataModel>();
            if(name == null)
            {
                var query = from pp in _repositoryPatientProfile.Table
                            join p in _repositoryPatient.Table on pp.PatientId equals p.Id
                            join app in _repositoryAppointment.Table on pp.AppointmentId equals app.Id
                            join ds in _repositoryDoctorService.Table on pp.DoctorServiceId equals ds.Id
                            where app.AppointmentDateTime > fromDate && app.AppointmentDateTime < toDate
                            select new { date = app.AppointmentDateTime, p.PatientName, ds.ServiceName, ds.Fees };
                response = query.Select(f => new FeesDataModel()
                {
                    date = f.date.ToString("dd/MM/yyyy"),
                    patientName = f.PatientName,
                    serviceName = f.ServiceName,
                    fees = f.Fees,
                }).ToList();
            }
            if (name != null && name != "")
            {
                var query = from pp in _repositoryPatientProfile.Table
                            join p in _repositoryPatient.Table on pp.PatientId equals p.Id
                            join app in _repositoryAppointment.Table on pp.AppointmentId equals app.Id
                            join ds in _repositoryDoctorService.Table on pp.DoctorServiceId equals ds.Id
                            where app.AppointmentDateTime > fromDate && app.AppointmentDateTime < toDate
                            //&& p.PatientName.Contains(name) || ds.ServiceName.Contains(name)
                            select new { date = app.AppointmentDateTime, p.PatientName, ds.ServiceName, ds.Fees };
                response = query.Select(f => new FeesDataModel()
                {
                    date = f.date.ToString("dd/MM/yyyy"),
                    patientName = f.PatientName,
                    serviceName = f.ServiceName,
                    fees = f.Fees,
                }).ToList();
            }
            return response;
        }

        public virtual List<ResponseData> GetGenderRatio(long doctorId = 0, 
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from pp in _repositoryPatientProfile.Table
                        join p in _repositoryPatient.Table on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table on pp.AppointmentId equals app.Id
                        where app.AppointmentDateTime > fromDate && app.AppointmentDateTime < toDate 
                        && app.AppUserId == doctorId && app.IsActive == true && p.IsDeleted == false
                        && pp.IsDeleted == false
                        group pp by new { p.PatientGender, app.AppointmentDateTime } into grp
                        select new
                        {
                            gender = grp.Key.PatientGender,
                            date = grp.Key.AppointmentDateTime,
                            count = grp.Count()
                        };

            var _grpData = query.Select(f => new Report()
            {
                data = f.gender,
                date = f.date.ToString("dd/MM/yyyy"),
                count = f.count
            }).ToList();

            var _obj = _grpData.Select(a => a.date).Distinct().ToList();

            List<ResponseData> _response = new List<ResponseData>();
            foreach (var item in _obj)
            {
                ResponseData response = new ResponseData();
                response.date = item;
                response.dataItems = new List<DataFormat>();
                response.dataItems = _grpData.Where(a => a.date == item).
                    GroupBy(f => new
                    {
                        data = f.data,
                        count = f.count
                    }).Select(a => new DataFormat()
                    {
                        data = a.Key.data,
                        itemCount = a.Key.count
                    }).ToList();
                _response.Add(response);
            }
            return _response;
        }

        public virtual List<ResponseData> GetDiseaseCategories(long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from pp in _repositoryPatientProfile.Table
                        join pd in _repositoryPatientDiagnosis.Table on pp.Id equals pd.PatientProfileId
                        join dm in _repositoryDiagnosisMaster.Table on pd.DiagnosisMasterId equals dm.Id
                        join app in _repositoryAppointment.Table on pp.AppointmentId equals app.Id
                        where app.AppointmentDateTime > fromDate && app.AppointmentDateTime < toDate
                        && app.AppUserId == doctorId && app.IsActive == true && pp.IsDeleted == false
                        && pd.IsDeleted == false
                        group pp by new { dm.Name, app.AppointmentDateTime } into grp
                        select new
                        {
                            diagnosisName = grp.Key.Name,
                            date = grp.Key.AppointmentDateTime,
                            count = grp.Count()
                        };
            var _grpData = query.Select(f => new Report()
            {
                data = f.diagnosisName,
                date = f.date.ToString("dd/MM/yyyy"),
                count = f.count,

            }).ToList();

            var _obj = _grpData.Select(a => a.date).Distinct().ToList();

            List<ResponseData> _response = new List<ResponseData>();
            foreach (var item in _obj)
            {
                ResponseData response = new ResponseData();
                response.date = item;
                response.dataItems = new List<DataFormat>();
                response.dataItems = _grpData.Where(a => a.date == item).
                    GroupBy(a => new
                    {
                        data = a.data,
                        count = a.count
                    }).Select(f => new DataFormat()
                    {
                        data = f.Key.data,
                        itemCount = f.Key.count
                    }).ToList();
                _response.Add(response);
            }
            return _response;
        }

        public virtual List<ResponseData> GetDoctorReferrals(long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from pp in _repositoryPatientProfile.Table
                        join p in _repositoryPatient.Table on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table on pp.AppointmentId equals app.Id
                        where app.ReferredBy != null && app.AppointmentDateTime > fromDate && 
                        app.AppointmentDateTime < toDate && app.AppUserId == doctorId  && app.IsActive == true 
                        && pp.IsDeleted == false && p.IsDeleted == false
                        group pp by new { app.ReferredBy, app.AppointmentDateTime } into grp
                        select new
                        {
                            referredBy = grp.Key.ReferredBy,
                            date = grp.Key.AppointmentDateTime,
                            count = grp.Count()
                        };
            var _grpData = query.Select(f => new Report()
            {
                data = f.referredBy,
                date = f.date.ToString("dd/MM/yyyy"),
                count = f.count
            }).ToList() ;

            var _obj = _grpData.Select(a => a.date).Distinct().ToList();
            List<ResponseData> _response = new List<ResponseData>();
            foreach (var item in _obj)
            {
                ResponseData response = new ResponseData();
                response.date = item;
                response.dataItems = new List<DataFormat>();
                response.dataItems = _grpData.Where(a => a.date == item).
                    GroupBy(a => new
                    {
                        data = a.data,
                        count = a.count
                    }).Select(f => new DataFormat()
                    {
                        data = f.Key.data,
                        itemCount = f.Key.count
                    }).ToList();
                _response.Add(response);
            }
            return _response;
        }

        public virtual List<ResponseData> GetNewAndOldPatients(long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from pp in _repositoryPatientProfile.Table
                        join p in _repositoryPatient.Table on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table on pp.AppointmentId equals app.Id
                        where app.AppointmentDateTime > fromDate && app.AppointmentDateTime < toDate
                        && app.AppUserId == doctorId && app.IsActive == true && p.IsDeleted == false
                        && pp.IsDeleted == false
                        group pp by new { app.VisitType, app.AppointmentDateTime } into grp
                        select new
                        {
                            visitType = grp.Key.VisitType,
                            date = grp.Key.AppointmentDateTime,
                            count = grp.Count()
                        };
            var _grpData = query.Select(a => new Report()
            {
                data = a.visitType,
                date = a.date.ToString("dd/MM/yyyy"),
                count = a.count
            }).ToList();
            var _obj = _grpData.Select(a => a.date).Distinct().ToList();
            List<ResponseData> _response = new List<ResponseData>();
            foreach (var item in _obj)
            {
                ResponseData response = new ResponseData();
                response.date = item;
                response.dataItems = new List<DataFormat>();
                response.dataItems = _grpData.Where(a => a.date == item).
                    GroupBy(f => new
                    {
                        data = f.data,
                        count = f.count
                    }).Select(f => new DataFormat()
                    {
                        data = f.Key.data,
                        itemCount = f.Key.count
                    }).ToList();
                _response.Add(response);
            }
            return _response;
        }
    }


}