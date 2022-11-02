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
        public readonly IRepository<PatientTest> _repositoryTest;
        public readonly IRepository<DiagnosisMaster> _repositoryDM;
        public readonly IRepository<Appointment> _repositoryApp;
        public PatientProfileService(IRepository<PatientProfile> repository,
            IRepository<PatientProcedure> repositoryProcedure,
            IRepository<Prescription> repositoryPrescription,
             IRepository<PatientTest> repositoryTest,
             IRepository<PatientDiagnosis> repositoryPd,
             IRepository<DiagnosisMaster> repositoryDM,
             IRepository<Appointment> repositoryApp)
        {
            _repository = repository;
            _repositoryProcedure = repositoryProcedure;
            _repositoryPrescription = repositoryPrescription;
            _repositoryDM = repositoryDM;
            _repositoryTest = repositoryTest;
            _repositoryPd = repositoryPd;
            _repositoryApp = repositoryApp;
        }

        public virtual IQueryable<PatientProfile> GetAll(string name = null, long? patientId = null, long[] appointmentIds = null)
        {
            var data = _repository.Table.Where(a => !a.IsDeleted)
                .Include(a => a.Appointment).ThenInclude(a=>a.AppUser).Include(a => a.Prescriptions)
                .ThenInclude(a => a.PrescriptionMaster).AsQueryable();

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
            //return await _repository.Table.Where(a => a.AppointmentId == id).Include(a => a.Patient)
            //    .Include(a => a.PatientDiagnosis).Include(a => a.PatientTests)
            //    .Include(a => a.Prescriptions)
            //    .Include(a => a.Appointment).ThenInclude(a => a.AppUser).Include(a => a.Appointment.PatientFiles)
            //    .Include(a => a.Appointment.VitalsReport).Include(a => a.DoctorService).FirstOrDefaultAsync();
            return await _repository.Table.Where(a => a.AppointmentId == id).Include(a => a.Patient)
                .Include(a => a.PatientDiagnosis).ThenInclude(a => a.DiagnosisMaster).Include(a => a.PatientTests)
                .Include(a => a.Prescriptions).ThenInclude(a => a.PrescriptionMaster)
                .Include(a => a.Appointment).ThenInclude(a => a.AppUser).Include(a => a.Appointment.PatientFiles)
                .Include(a => a.Appointment.VitalsReport).Include(a => a.DoctorService).FirstOrDefaultAsync();
        }


        public virtual async Task<PatientProfile> GetByAppointment(long id)
        {
            return await _repository.Table.Where(a => a.AppointmentId == id).Include(a => a.Patient).Include(a => a.Appointment.AppUser).FirstOrDefaultAsync();
        }
        public virtual async Task<List<Prescription>> getPrescriptions(long patientProfileId)
        {
            return await _repositoryPrescription.Table.Where(a => a.PatientProfileId == patientProfileId).Include(a => a.PrescriptionMaster).ToListAsync();
        }
        public virtual async Task<List<PatientDiagnosis>> getDiagnosis(long patientProfileId)
        {
            return await _repositoryPd.Table.Where(a => a.PatientProfileId == patientProfileId).Include(a => a.DiagnosisMaster).ToListAsync();
        }
        public virtual async Task<List<PatientTest>> getTestValues(long patientProfileId)
        {
            return await _repositoryTest.Table.Where(a => a.PatientProfileId == patientProfileId).Include(a => a.TestMaster).ToListAsync();
        }
        public virtual async Task<PatientProfile> GetByPatient(long id)
        {

            return await _repository.Table.Where(a => a.Patient.Id == id).Include(a => a.Patient)
                .Include(a => a.PatientDiagnosis).ThenInclude(a => a.DiagnosisMaster)
                .Include(a => a.PatientTests)
                .Include(a => a.Appointment).ThenInclude(a => a.AppUser).Include(a => a.Appointment.PatientFiles)
                .Include(a => a.Appointment.VitalsReport).OrderByDescending(a => a.Id).FirstOrDefaultAsync();
        }

        public virtual async Task<PatientProfile> GetPrescriptions(long id)
        {
            return await _repository.Table.Where(a => a.Id == id && !a.IsDeleted)
                .Include(a => a.Prescriptions).ThenInclude(a => a.PrescriptionMaster).FirstOrDefaultAsync();
        }
        public virtual List<long> getHistory(long patientId, long appointmentId)
        {
            return _repository.Table.Where(a => a.PatientId == patientId && a.AppointmentId != appointmentId).Select(a => a.Id).ToList();

        }
        // public List<ResponseData> test1()
        //{

        //    var results = from r in _repository.Table
        //                  join pd in _repositoryPd.Table on r.Id equals pd.PatientProfileId
        //                  join dm in _repositoryDM.Table on pd.DiagnosisMasterId equals dm.Id
        //                  join apt in _repositoryApp.Table on r.AppointmentId equals apt.Id
        //                  where apt.IsActive == true && apt.AppointmentDateTime > DateTime.Now.AddDays(-100)
        //                  group r by new { Name = dm.Name, Date = apt.AppointmentDateTime.Date } into grp
        //                  select new { name = grp.Key.Name, date = grp.Key.Date, rowcount = grp.Count() };

        //    var _grpData = results.ToList().Select(f => new DiagnosisBasedGraph() { Name = f.name, Date = f.date.ToString(), ItemCount = f.rowcount }).ToList();

        //    var _obj = _grpData.Select(a => a.Date).Distinct().ToList();

        //    List<ResponseData> _response = new List<ResponseData>();
        //    ResponseForChart resChart = new ResponseForChart();
        //    foreach (var item in _obj)
        //    {
        //        ResponseData res = new ResponseData();
        //        res.Date = item;
        //        res.dataItems = new List<DataFormat>();
        //        res.dataItems = _grpData.Where(a => a.Date == item).GroupBy(a => new { name = a.Name, cnt = a.ItemCount }).Select(a => new DataFormat() { Name = a.Key.name, ItemCount = a.Key.cnt }).ToList();
        //        _response.Add(res);
        //    }

        //    //return SetChartData(_response);
        //    return _response;

        //}


        public ResponseForChart test()
        {

            var results = from r in _repository.Table.ToList()
                          join pd in _repositoryPd.Table.ToList() on r.Id equals pd.PatientProfileId
                          join dm in _repositoryDM.Table.ToList() on pd.DiagnosisMasterId equals dm.Id
                          join apt in _repositoryApp.Table.ToList() on r.AppointmentId equals apt.Id
                          where apt.IsActive == true && apt.AppointmentDateTime > DateTime.Now.AddDays(-100)
                          group r by new { Name = dm.Name, Date = apt.AppointmentDateTime.Date.ToShortDateString() } into grp
                          select new { name = grp.Key.Name, date = grp.Key.Date, rowcount = grp.Count() };

            var _grpData = results.ToList().Select(f => new DiagnosisBasedGraph() { Name = f.name, Date = f.date, ItemCount = f.rowcount }).ToList();

            var _obj = _grpData.Select(a => a.Date).Distinct().ToList();
            var _objName = _grpData.Select(a => a.Name).Distinct().ToList();

            ResponseForChart resChart = new ResponseForChart();
            var _names = _grpData.Select(a=>a.Name).Distinct().ToList();
            var _dates = _grpData.Select(a => a.Date).Distinct().ToList();
            resChart.categories = _names.Select(a => new DataFormatChart() { Name = a, values = new List<int>() }).ToList();
            resChart.dates = _dates;
            int i = 0;
            foreach (var item in _dates)
            {
                resChart.categories.ForEach(a => a.values.Add(_grpData.Where(g => g.Date == item && g.Name == a.Name).Sum(a => a.ItemCount)));
                i++;
                
            }
            return resChart;

        }
        public class DiagnosisBasedGraph
        {
            public string Name { get; set; }
            public string Date { get; set; }
            public int ItemCount { get; set; }
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
        //public class DataFormat
        //{
        //    public string Name { get; set; }
        //    public int ItemCount { get; set; }
        //}
        //public class ResponseData
        //{
        //    public string  Date  { get; set; }
        //    public List<DataFormat> dataItems { get; set; }
        //}

    }
}
