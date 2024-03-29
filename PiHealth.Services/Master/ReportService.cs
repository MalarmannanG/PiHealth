﻿using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using PiHealth.Services.AppConstants;
using PiHealth.Services.Model;
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
        //public readonly IRepository<PatientDiagnosis> _repositoryPatientDiagnosis;
        //public readonly IRepository<DiagnosisMaster> _repositoryDiagnosisMaster;
        public readonly IRepository<DoctorService> _repositoryDoctorService;
        public readonly IRepository<PatientProfileData> _repositoryPatientProfileData;
        public readonly IRepository<PatientProfileDataMapping> _repositoryPatientProfileDataMapping;

        public ReportService(IRepository<Patient> repositoryPatient,
            IRepository<Appointment> repositoryAppointment,
            IRepository<PatientProfile> repositoryPatientProfile,
            //IRepository<PatientDiagnosis> repositoryPatientDiagnosis,
            //IRepository<DiagnosisMaster> repositoryDiagnosisMaster,
            IRepository<DoctorService> repositoryDoctorService,
            IRepository<PatientProfileDataMapping> repositoryPatientProfileDataMapping,
            IRepository<PatientProfileData> repositoryPatientProfileData)
        {
            _repositoryPatient = repositoryPatient;
            _repositoryAppointment = repositoryAppointment;
            _repositoryPatientProfile = repositoryPatientProfile;
            //_repositoryPatientDiagnosis = repositoryPatientDiagnosis;
            //_repositoryDiagnosisMaster = repositoryDiagnosisMaster;
            _repositoryDoctorService = repositoryDoctorService;
            _repositoryPatientProfileData = repositoryPatientProfileData;
            _repositoryPatientProfileDataMapping = repositoryPatientProfileDataMapping;
        }


        public virtual List<FeesDataModel> GetFeesData(string name = null, long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            List<FeesDataModel> response = new List<FeesDataModel>();
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join p in _repositoryPatient.Table on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table on pp.AppointmentId equals app.Id
                        join ds in _repositoryDoctorService.Table on pp.DoctorServiceId equals ds.Id
                        where DateOnly.FromDateTime(app.AppointmentDateTime) >= DateOnly.FromDateTime((DateTime)fromDate)
                        && DateOnly.FromDateTime(app.AppointmentDateTime) <= DateOnly.FromDateTime((DateTime)toDate)
                        && app.IsDeleted == false && app.AppUserId == doctorId && p.IsDeleted == false
                        && pp.IsDeleted == false && ds.AppUserId == doctorId & ds.IsDeleted == false
                        select new { date = app.AppointmentDateTime, p.PatientName, ds.ServiceName, ds.Fees };

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(e => e.PatientName.ToLower().Contains(name.ToLower()) || e.ServiceName.ToLower().Contains(name.ToLower()));
            }

            response = query.Select(f => new FeesDataModel()
            {
                date = f.date.ToString("dd/MM/yyyy"),
                patientName = f.PatientName,
                serviceName = f.ServiceName,
                fees = f.Fees,
            }).ToList();
            return response;
        }

        public virtual List<ImpressionDataModel> GetImpressionData(string name = null, long doctorId = 0,
           DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join pt in _repositoryPatient.Table.ToList() on pp.PatientId equals pt.Id
                        join pd in _repositoryPatientProfileDataMapping.Table.ToList() on pp.Id equals pd.PatientProfileId
                        join dm in _repositoryPatientProfileData.Table.ToList() on pd.PatientProfileDataId equals dm.Id
                        join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
                        where DateOnly.FromDateTime(app.AppointmentDateTime) >= DateOnly.FromDateTime((DateTime)fromDate)
                        && DateOnly.FromDateTime(app.AppointmentDateTime) <= DateOnly.FromDateTime((DateTime)toDate)
                        && app.AppUserId == doctorId && app.IsDeleted == false 
                        && pp.IsDeleted == false && dm.IsDeleted == false
                        && dm.Key == (long)ProfileDataEnum.Impression
                        //group pp by new { dm.Description, app.AppointmentDateTime } into grp
                        select new ImpressionDataModel
                        {
                            impressionName = dm.Description,
                            date =  app.AppointmentDateTime,
                            patientAge = (int)pt.Age,
                            patientName = pt.PatientName,
                        };
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(e => e.patientName.ToLower().Contains(name.ToLower()) || e.impressionName.ToLower().Contains(name.ToLower()));
            }

            return query.ToList();
        }
        public virtual ResponseData GetGenderRatio(long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join p in _repositoryPatient.Table.ToList() on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
                        where DateOnly.FromDateTime(app.AppointmentDateTime) >= DateOnly.FromDateTime((DateTime)fromDate)
                        && DateOnly.FromDateTime(app.AppointmentDateTime) <= DateOnly.FromDateTime((DateTime)toDate)
                        && app.AppUserId == doctorId && app.IsDeleted == false && p.IsDeleted == false
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
                date = f.date.ToString("dd-MMM"),
                count = f.count
            }).OrderBy(a => a.date).ToList();

            var _obj = _grpData.Select(a => a.date).Distinct().ToList();
            var _objName = _grpData.Select(a => a.data).Distinct().ToList();

            ResponseData response = new ResponseData();
            var _datas = _grpData.Select(a => a.data).Distinct().ToList();
            var _dates = _grpData.Select(a => a.date).Distinct().ToList();
            response.dataItems = _datas.Select(a => new DataFormat()
            {
                name = a,
                data = new List<int>()
            }).ToList();
            response.dates = _dates;
            int i = 0;
            foreach (var item in _dates)
            {
                response.dataItems.ForEach
                    (a => a.data.Add(_grpData.Where(g => g.date == item && g.data == a.name).Sum(a => a.count)));
                i++;
            }
            return response;
        }

        public virtual ResponseData GetDiseaseCategories(long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            //var query = from pp in _repositoryPatientProfile.Table.ToList()
            //            join pd in _repositoryPatientDiagnosis.Table.ToList() on pp.Id equals pd.PatientProfileId
            //            join dm in _repositoryDiagnosisMaster.Table.ToList() on pd.DiagnosisMasterId equals dm.Id
            //            join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
            //            where DateOnly.FromDateTime(app.AppointmentDateTime) >= DateOnly.FromDateTime((DateTime)fromDate)
            //            && DateOnly.FromDateTime(app.AppointmentDateTime) <= DateOnly.FromDateTime((DateTime)toDate)
            //            && app.AppUserId == doctorId && app.IsDeleted == false && pp.IsDeleted == false
            //            && pd.IsDeleted == false && dm.IsDeleted == false
            //            group pp by new { dm.Name, app.AppointmentDateTime } into grp
            //            select new
            //            {
            //                diagnosisName = grp.Key.Name,
            //                date = grp.Key.AppointmentDateTime,
            //                count = grp.Count()
            //            };
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join pd in _repositoryPatientProfileDataMapping.Table.ToList() on pp.Id equals pd.PatientProfileId
                        join dm in _repositoryPatientProfileData.Table.ToList() on pd.PatientProfileDataId equals dm.Id
                        join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
                        where DateOnly.FromDateTime(app.AppointmentDateTime) >= DateOnly.FromDateTime((DateTime)fromDate)
                        && DateOnly.FromDateTime(app.AppointmentDateTime) <= DateOnly.FromDateTime((DateTime)toDate)
                        && app.AppUserId == doctorId && app.IsDeleted == false && pp.IsDeleted == false && dm.IsDeleted == false
                        && dm.Key == (long)ProfileDataEnum.Impression
                        group pp by new { dm.Description, app.AppointmentDateTime } into grp
                        select new
                        {
                            impressionName = grp.Key.Description,
                            date = grp.Key.AppointmentDateTime,
                            count = grp.Count()
                        };
            var _grpData = query.Select(f => new Report()
            {
                data = f.impressionName,
                date = f.date.ToString("dd-MMM"),
                count = f.count,

            }).OrderBy(a => a.date).ToList();

            var _obj = _grpData.Select(a => a.date).Distinct().ToList();
            var _objName = _grpData.Select(a => a.data).Distinct().ToList();

            ResponseData response = new ResponseData();
            var _datas = _grpData.Select(a => a.data).Distinct().ToList();
            var _dates = _grpData.Select(a => a.date).Distinct().ToList();
            response.dataItems = _datas.Select(a => new DataFormat()
            {
                name = a,
                data = new List<int>()
            }).ToList();
            response.dates = _dates;
            int i = 0;
            foreach (var item in _dates)
            {
                response.dataItems.ForEach
                    (a => a.data.Add(_grpData.Where(g => g.date == item && g.data == a.name).Sum(a => a.count)));
                i++;
            }
            return response;
        }

        public virtual ResponseData GetDoctorReferrals(long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join p in _repositoryPatient.Table.ToList() on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
                        where DateOnly.FromDateTime(app.AppointmentDateTime) >= DateOnly.FromDateTime((DateTime)fromDate)
                        && DateOnly.FromDateTime(app.AppointmentDateTime) <= DateOnly.FromDateTime((DateTime)toDate)
                        && app.ReferredBy != null && app.AppUserId == doctorId && app.IsDeleted == false
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
                date = f.date.ToString("dd-MMM"),
                count = f.count
            }).OrderBy(a => a.date).ToList();

            var _obj = _grpData.Select(a => a.date).Distinct().ToList();
            var _objName = _grpData.Select(a => a.data).Distinct().ToList();

            ResponseData response = new ResponseData();
            var _datas = _grpData.Select(a => a.data).Distinct().ToList();
            var _dates = _grpData.Select(a => a.date).Distinct().ToList();
            response.dataItems = _datas.Select(a => new DataFormat()
            {
                name = a,
                data = new List<int>()
            }).ToList();
            response.dates = _dates;
            int i = 0;
            foreach (var item in _dates)
            {
                response.dataItems.ForEach(a => a.data.Add(_grpData.Where(g => g.date == item && g.data == a.name)
                    .Sum(a => a.count)));
                i++;
            }
            return response;
        }

        public virtual ResponseData GetNewAndOldPatients(long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join p in _repositoryPatient.Table.ToList() on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
                        where DateOnly.FromDateTime(app.AppointmentDateTime) >= DateOnly.FromDateTime((DateTime)fromDate)
                        && DateOnly.FromDateTime(app.AppointmentDateTime) <= DateOnly.FromDateTime((DateTime)toDate)
                        && app.AppUserId == doctorId && app.IsDeleted == false && p.IsDeleted == false
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
                date = a.date.ToString("dd-MMM"),
                count = a.count
            }).OrderBy(a => a.date).ToList();
            var _obj = _grpData.Select(a => a.date).Distinct().ToList();
            ResponseData response = new ResponseData();
            var _datas = _grpData.Select(a => a.data).Distinct().ToList();
            var _dates = _grpData.Select(a => a.date).Distinct().ToList();
            response.dataItems = _datas.Select(a => new DataFormat()
            {
                name = a,
                data = new List<int>()
            }).ToList();
            response.dates = _dates;
            int i = 0;
            foreach (var item in _dates)
            {
                response.dataItems.ForEach(a => a.data.Add(_grpData.Where(g => g.date == item && g.data == a.name)
                    .Sum(a => a.count)));
                i++;
            }
            return response;
        }


        public virtual PieResponseData GetAgeRangeReport(long doctorId = 0,
          DateTime? fromDate = null, DateTime? toDate = null)
        {
            /*
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join p in _repositoryPatient.Table.ToList() on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
                        where DateOnly.FromDateTime(app.AppointmentDateTime) >= DateOnly.FromDateTime((DateTime)fromDate)
                        && DateOnly.FromDateTime(app.AppointmentDateTime) <= DateOnly.FromDateTime((DateTime)toDate)
                        && app.AppUserId == doctorId && app.IsDeleted == false && p.IsDeleted == false
                        && pp.IsDeleted == false
                        group pp by new { p.Age, app.AppointmentDateTime } into grp
                        select new
                        {
                            age = grp.Key.Age,
                            date = grp.Key.AppointmentDateTime,
                            count = grp.Count()
                        };


            var _grpData = query.Select(f => new AgeReport()
            {
                data = (int)f.age,
                date = f.date.ToString("dd-MMM"),
                count = f.count
            }).OrderBy(a => a.date).ToList();

            var _obj = _grpData.Select(a => a.date).Distinct().ToList();
            var _objName = new List<string>() { "19-30", "31-40", "41-50", "51-60", "61+" };

            ResponseData response = new ResponseData();
            var _datas = _grpData.Select(a => a.data).Distinct().ToList();
            response.dates = _grpData.Select(a => a.date).Distinct().ToList();

            response.dataItems = _objName.Select(a => new DataFormat()
            {
                name = a,
                data = new List<int>()
            }).ToList();

            foreach (var item in response.dates)
            {

                response.dataItems[0].data.Add(_grpData.Where(g => g.date == item && (g.data > 18 && g.data < 31)).Sum(a => a.count));
                response.dataItems[1].data.Add(_grpData.Where(g => g.date == item && (g.data > 30 && g.data < 41)).Sum(a => a.count));
                response.dataItems[2].data.Add(_grpData.Where(g => g.date == item && (g.data > 40 && g.data < 51)).Sum(a => a.count));
                response.dataItems[3].data.Add(_grpData.Where(g => g.date == item && (g.data > 50 && g.data < 61)).Sum(a => a.count));
                response.dataItems[4].data.Add(_grpData.Where(g => g.date == item && (g.data > 60)).Sum(a => a.count));
            }
            return response;
            */
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join p in _repositoryPatient.Table.ToList() on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
                        where DateOnly.FromDateTime(app.AppointmentDateTime) >= DateOnly.FromDateTime((DateTime)fromDate)
                        && DateOnly.FromDateTime(app.AppointmentDateTime) <= DateOnly.FromDateTime((DateTime)toDate)
                        && app.AppUserId == doctorId && app.IsDeleted == false && p.IsDeleted == false
                        && pp.IsDeleted == false
                        select new
                        {
                            age = p.Age
                        };


            PieResponseData response = new PieResponseData();

            response.labels = new List<string>() { "19-30", "31-40", "41-50", "51-60", "61+" };
            response.dataItems = new List<int>();
            foreach (var item in response.labels)
            {
                switch (item)
                {
                    case "19-30":
                        response.dataItems.Add(query.Where(g => (g.age > 18 && g.age < 31)).Count());
                        break;

                    case "31-40":
                        response.dataItems.Add(query.Where(g => (g.age > 30 && g.age < 41)).Count());
                        break;

                    case "41-50":
                        response.dataItems.Add(query.Where(g => (g.age > 40 && g.age < 51)).Count());
                        break;

                    case "51-60":
                        response.dataItems.Add(query.Where(g => (g.age > 50 && g.age < 61)).Count());
                        break;
                    case "61+":
                        response.dataItems.Add(query.Where(g => (g.age > 60)).Count());
                        break;
                }

            }
            return response;
        }


    }


}
