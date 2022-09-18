using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using PiHealth.DataModel.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PiHealth.Services.PatientProfileService
{
    public class PatientProfileService
    {
        public readonly IRepository<PatientProfile> _repository;
        public readonly IRepository<PatientProcedure> _repositoryProcedure;
        public readonly IRepository<Prescription> _repositoryPrescription;
        public readonly IRepository<PatientDiagnosis> _repositoryPd;
        public readonly IRepository<DiagnosisMaster> _repositoryDM;
        public readonly IRepository<Appointment> _repositoryApp;
        public PatientProfileService(IRepository<PatientProfile> repository,
            IRepository<PatientProcedure> repositoryProcedure,
            IRepository<Prescription> repositoryPrescription,
             IRepository<DiagnosisMaster> repositoryDM,
             IRepository<PatientDiagnosis> repositoryPd,
             IRepository<Appointment> repositoryApp)
        {
            _repository = repository;
            _repositoryProcedure = repositoryProcedure;
            _repositoryPrescription = repositoryPrescription;
            _repositoryPd = repositoryPd;
            _repositoryDM = repositoryDM;
            _repositoryApp = repositoryApp;
        }

        public virtual IQueryable<PatientProfile> GetAll(string name = null, long? patientId = null, long[] appointmentIds = null)
        {
            var data = _repository.Table.Where(a => !a.IsDeleted).Include(a => a.Appointment).Include(a => a.Prescriptions).ThenInclude(a => a.PrescriptionMaster).AsQueryable();

            if (patientId != null)
            {
                data = data.Where(a => a.PatientId == patientId);
            }

            if (appointmentIds != null && appointmentIds.Count() > 0)
            {
                data = data.Where(a => appointmentIds.Contains(a.AppointmentId));
            }


            return data;
        }

        public virtual IQueryable<PatientProfile> AutoComplete(string name = null)
        {
            var data = _repository.Table.Where(a => !a.IsDeleted);


            return data;
        }
        public virtual IQueryable<string> GetComplaints()
        {
            return _repository.Table.Where(a => !string.IsNullOrEmpty(a.Compliants)).Select(a => a.Compliants).Distinct();
        }
        public virtual IQueryable<string> GetAdvice()
        {
            return _repository.Table.Where(a => !string.IsNullOrEmpty(a.Advice)).Select(a => a.Advice).Distinct();
        }
        public virtual IQueryable<string> GetPlan()
        {
            return _repository.Table.Where(a => !string.IsNullOrEmpty(a.Plan)).Select(a => a.Plan).Distinct();
        }
        public virtual IQueryable<string> GetImpression()
        {
            return _repository.Table.Where(a => !string.IsNullOrEmpty(a.Impression)).Select(a => a.Impression).Distinct();
        }
        public virtual IQueryable<string> GetInstructions()
        {
            return _repositoryPrescription.Table.Where(a => !string.IsNullOrEmpty(a.Instructions)).Select(a => a.Instructions).Distinct();
        }
        public virtual async Task<PatientProfile> Update(PatientProfile entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<PatientProfile> Create(PatientProfile entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public virtual async Task Delete(PatientProfile entity)
        {
            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
        }

        public virtual async Task<PatientProfile> Get(long id)
        {
            return await _repository.Table.Where(a => a.AppointmentId == id).Include(a => a.Patient).Include(a => a.PatientDiagnosis).ThenInclude(a => a.DiagnosisMaster).Include(a => a.PatientTests).ThenInclude(a => a.TestMaster).Include(a => a.Prescriptions).ThenInclude(a => a.PrescriptionMaster).Include(a => a.Appointment).ThenInclude(a => a.AppUser).Include(a => a.Appointment.PatientFiles).Include(a => a.Appointment.VitalsReport).Include(a => a.DoctorService).FirstOrDefaultAsync();
        }

        public virtual async Task<PatientProfile> GetByPatient(long id)
        {
            return await _repository.Table.Where(a => a.Patient.Id == id).Include(a => a.Patient).Include(a => a.PatientDiagnosis).ThenInclude(a => a.DiagnosisMaster).Include(a => a.PatientTests).ThenInclude(a => a.TestMaster).Include(a => a.Appointment).ThenInclude(a => a.AppUser).Include(a => a.Appointment.PatientFiles).Include(a => a.Appointment.VitalsReport).OrderByDescending(a => a.Id).OrderByDescending(a => a.Id).FirstOrDefaultAsync();
        }

        public virtual async Task<PatientProfile> GetPrescriptions(long id)
        {
            return await _repository.Table.Where(a => a.Id == id && !a.IsDeleted).Include(a => a.Prescriptions).ThenInclude(a => a.PrescriptionMaster).FirstOrDefaultAsync();
        }


        public List<ResponseData> test1()
        {

            var results = from r in _repository.Table
                          join pd in _repositoryPd.Table on r.Id equals pd.PatientProfileId
                          join dm in _repositoryDM.Table on pd.DiagnosisMasterId equals dm.Id
                          join apt in _repositoryApp.Table on r.AppointmentId equals apt.Id
                          where apt.IsActive == true && apt.AppointmentDateTime > DateTime.Now.AddDays(-100)
                          group r by new { Name = dm.Name, Date = apt.AppointmentDateTime.Date } into grp
                          select new { name = grp.Key.Name, date = grp.Key.Date, rowcount = grp.Count() };

            var _grpData = results.ToList().Select(f => new DiagnosisBasedGraph() { Name = f.name, Date = f.date.ToString(), ItemCount = f.rowcount }).ToList();

            var _obj = _grpData.Select(a => a.Date).Distinct().ToList();

            List<ResponseData> _response = new List<ResponseData>();
            ResponseForChart resChart = new ResponseForChart();
            foreach (var item in _obj)
            {
                ResponseData res = new ResponseData();
                res.Date = item;
                res.dataItems = new List<DataFormat>();
                res.dataItems = _grpData.Where(a => a.Date == item).GroupBy(a => new { name = a.Name, cnt = a.ItemCount }).Select(a => new DataFormat() { Name = a.Key.name, ItemCount = a.Key.cnt }).ToList();
                _response.Add(res);
            }

            //return SetChartData(_response);
            return _response;

        }


        public ResponseForChart test()
        {

            var results = from r in _repository.Table
                          join pd in _repositoryPd.Table on r.Id equals pd.PatientProfileId
                          join dm in _repositoryDM.Table on pd.DiagnosisMasterId equals dm.Id
                          join apt in _repositoryApp.Table on r.AppointmentId equals apt.Id
                          where apt.IsActive == true && apt.AppointmentDateTime > DateTime.Now.AddDays(-100)
                          group r by new { Name = dm.Name, Date = apt.AppointmentDateTime.Date } into grp
                          select new { name = grp.Key.Name, date = grp.Key.Date, rowcount = grp.Count() };

            var _grpData = results.ToList().Select(f => new DiagnosisBasedGraph() { Name = f.name, Date = f.date.ToString(), ItemCount = f.rowcount }).ToList();

            var _obj = _grpData.Select(a => a.Date).Distinct().ToList();
            var _objName = _grpData.Select(a => a.Name).Distinct().ToList();

            ResponseForChart resChart = new ResponseForChart();
            var _names = _grpData.Select(a=>a.Name).Distinct().ToList();
            resChart.categories = _names.Select(a => new DataFormatChart() { Name = a, values = new List<int>() }).ToList();
            resChart.dates = new List<string>();
            foreach (var item in _grpData)
            {
                resChart.dates.Add(item.Date);
                resChart.categories.Where(a => a.Name == item.Name).ToList().ForEach(a => a.values.Add(item.ItemCount));
                resChart.categories.Where(a => a.Name != item.Name).ToList().ForEach(a => a.values.Add(0));
            }
            return resChart;

        }

        private ResponseForChart SetChartData(List<ResponseData> items)
        {
            ResponseForChart resChart = new ResponseForChart();
            resChart.dates= new List<string>();
            resChart.categories = new List<DataFormatChart>();
            foreach (var item in items)
            {
                resChart.dates.Add(item.Date);
                foreach (var _innerItem in item.dataItems)
                {
                    var existing = resChart.categories.FirstOrDefault(c => c.Name == _innerItem.Name);
                    if (existing != null)
                    {
                        resChart.categories.Where(a => a.Name == _innerItem.Name).ToList().ForEach(c => c.values.Add(_innerItem.ItemCount));
                    }
                    else
                        resChart.categories.Add(new DataFormatChart() { Name=_innerItem.Name, values = new List<int>() { _innerItem.ItemCount } });
                }
               
            }
            return resChart;
        }


        public class DiagnosisBasedGraph
        {
            public string Name { get; set; }
            public string Date { get; set; }
            public int ItemCount { get; set; }
        }

        public class DataFormat
        {
            public string Name { get; set; }
            public int ItemCount { get; set; }
        }
        public class ResponseData
        {
            public string  Date  { get; set; }
            public List<DataFormat> dataItems { get; set; }
        }

        public class DataFormatChart
        {
            public string Name { get; set; }
            public List<int> values { get; set; }
        }
        public class ResponseForChart
        {
            public List<string> dates { get; set; }
            public List<DataFormatChart> categories { get; set; }
        }
    }
}
