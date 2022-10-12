using PiHealth.DataModel;
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
            public string name { get; set; }
            public List<int> data { get; set; }
        }
        public class ResponseData
        {
            public List<string> dates { get; set; }
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
                var query = from pp in _repositoryPatientProfile.Table.ToList()
                            join p in _repositoryPatient.Table on pp.PatientId equals p.Id
                            join app in _repositoryAppointment.Table on pp.AppointmentId equals app.Id
                            join ds in _repositoryDoctorService.Table on pp.DoctorServiceId equals ds.Id
                            where app.AppointmentDateTime > fromDate && app.AppointmentDateTime < toDate
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

        public virtual ResponseData GetGenderRatio(long doctorId = 0,
            DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join p in _repositoryPatient.Table.ToList() on pp.PatientId equals p.Id
                        join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
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
            var query = from pp in _repositoryPatientProfile.Table.ToList()
                        join pd in _repositoryPatientDiagnosis.Table.ToList() on pp.Id equals pd.PatientProfileId
                        join dm in _repositoryDiagnosisMaster.Table.ToList() on pd.DiagnosisMasterId equals dm.Id
                        join app in _repositoryAppointment.Table.ToList() on pp.AppointmentId equals app.Id
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
                        where app.ReferredBy != null && app.AppointmentDateTime > fromDate &&
                        app.AppointmentDateTime < toDate && app.AppUserId == doctorId && app.IsActive == true
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
            }).ToList();

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
    }


}
