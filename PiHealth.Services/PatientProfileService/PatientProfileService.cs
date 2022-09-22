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
            var data = _repository.Table.Where(a => !a.IsDeleted)
                .Include(a => a.Appointment).Include(a => a.Prescriptions)
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
        /*
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
        }    */
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
