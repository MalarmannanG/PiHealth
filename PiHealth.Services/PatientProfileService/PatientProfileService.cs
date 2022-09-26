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
        //public readonly IRepository<PatientProcedure> _repositoryProcedure;
        public readonly IRepository<Prescription> _repositoryPrescription;
        public readonly IRepository<PatientDiagnosis> _repositoryPd;
        public readonly IRepository<PatientTest> _repositoryTest;
        //public readonly IRepository<DiagnosisMaster> _repositoryDM;
        //public readonly IRepository<Appointment> _repositoryApp;
        public PatientProfileService(IRepository<PatientProfile> repository,
            //IRepository<PatientProcedure> repositoryProcedure,
            IRepository<Prescription> repositoryPrescription,
             IRepository<PatientTest> repositoryTest,
             IRepository<PatientDiagnosis> repositoryPd)
            // IRepository<Appointment> repositoryApp)
        {
            _repository = repository;
            //_repositoryProcedure = repositoryProcedure;
            //_repositoryPrescription = repositoryPrescription;
            _repositoryPrescription = repositoryPrescription;
            _repositoryTest = repositoryTest;
            _repositoryPd = repositoryPd;
            //_repositoryApp = repositoryApp;
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
        

        }
}
