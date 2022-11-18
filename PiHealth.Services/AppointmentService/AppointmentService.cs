using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using PiHealth.DataModel.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace PiHealth.Services.Master
{
    public class AppointmentService
    {
        public readonly IRepository<Appointment> _repository;
        public readonly IRepository<PatientFiles> _repositoryPF;
        public AppointmentService(IRepository<Appointment> repository, IRepository<PatientFiles> repositoryPF)
        {
            _repository = repository;
            _repositoryPF = repositoryPF;
        }

        public virtual IQueryable<Appointment> GetAll(long[] patientIds = null, long[] doctorIds = null, bool? isProcedure = null, DateTime? fromDate = null, DateTime? toDate = null)
        {

            var data = _repository.Table.Where(a => a.IsActive && !a.IsDeleted).Include(a => a.Patient).ThenInclude(a => a.DoctorMaster).AsQueryable();


            if (patientIds != null )
            {
                data = data.Where(a => a.PatientId != null && patientIds.Contains(a.PatientId.Value));
            }

            if (doctorIds != null )
            {
                data = data.Where(a => a.AppUserId != null && doctorIds.Contains(a.AppUserId.Value));
            }

            if (isProcedure != null)
            {
                if (isProcedure == true)
                {
                    data = data.Where(a => a.VisitType == "Procedure");
                }
                else
                {
                    data = data.Where(a => a.VisitType != "Procedure");
                }
            }

            if (fromDate != null)
            {
                var date = fromDate.Value;
                TimeSpan time = new TimeSpan(0, 0, 0, 0);
                DateTime combined = date.Add(time);
                data = data.Where(a => a.AppointmentDateTime >= combined);
            }


            if (toDate != null)
            {
                var date = toDate.Value;
                TimeSpan time = new TimeSpan(0, 23, 59, 59);
                DateTime combined = date.Add(time);
                data = data.Where(a => a.AppointmentDateTime <= combined);
            }

            return data.Include(a => a.VitalsReport).Include(a => a.AppUser);
        }

        public virtual IQueryable<Appointment> GetAll(string patientName, string doctorName, string clinicName, DateTime? fromDate, DateTime? toDate, bool? isProcedure)
        {

            var data = _repository.Table.Where(a => a.IsActive && !a.IsDeleted).Include(a => a.Patient).ThenInclude(a => a.DoctorMaster).Include(a => a.VitalsReport).Include(a => a.AppUser).AsQueryable();

            //data = data.WhereIf(!string.IsNullOrEmpty(clinicName), a => a.Patient.DoctorMaster.Name.Contains(clinicName))
            //     .WhereIf(!string.IsNullOrEmpty(patientName), e => e.Patient.PatientName.Contains(patientName))
            //     .WhereIf(!string.IsNullOrEmpty(doctorName), e => e.AppUser.Name.Contains(doctorName));



            if (fromDate != null)
            {
                var date = fromDate.Value;
                TimeSpan time = new TimeSpan(0, 0, 0, 0);
                DateTime combined = date.Add(time);
                //data = data.Where(a => a.AppointmentDateTime >= combined);
            }


            if (toDate != null)
            {
                var date = toDate.Value;
                TimeSpan time = new TimeSpan(0, 23, 59, 59);
                DateTime combined = date.Add(time);
                //data = data.Where(a => a.AppointmentDateTime <= combined);
            }

            return data;
        }

        public virtual IQueryable<Appointment> GetDashboardAll(long patientId, long doctorId, 
            string clinicName, DateTime? fromDate, DateTime? toDate, bool? isProcedure)
        {

            var data = _repository.Table.Where(a => a.IsActive && !a.IsDeleted).Include(a => a.Patient).
                ThenInclude(a => a.DoctorMaster).Include(a => a.VitalsReport).Include(a => a.AppUser).AsQueryable();

            data = data.WhereIf(!string.IsNullOrEmpty(clinicName), a => a.Patient.DoctorMaster.Name.Contains(clinicName))
                 .WhereIf(patientId > 0, e => e.Patient.Id == patientId)
                 .WhereIf(doctorId > 0, e => e.AppUser.Id == doctorId );

            if (isProcedure != null)
            {
                if (isProcedure == true)
                {
                    data = data.Where(a => a.VisitType == "Procedure");
                }
                else
                {
                    data = data.Where(a => a.VisitType != "Procedure");
                }
            }

            if (fromDate != null)
            {
                var date = fromDate.Value;
                TimeSpan time = new TimeSpan(0, 0, 0, 0);
                DateTime combined = date.Add(time);
                data = data.Where(a => a.AppointmentDateTime >= combined);
            }


            if (toDate != null)
            {
                var date = toDate.Value;
                TimeSpan time = new TimeSpan(0, 23, 59, 59);
                DateTime combined = date.Add(time);
                data = data.Where(a => a.AppointmentDateTime <= combined);
            }

            return data;
        }

        public virtual IQueryable<Appointment> GetDashboardCardCount(long patientId, long doctorId, long clinicId, DateTime? fromDate, DateTime? toDate )
        {

            var data = _repository.Table.Where(a => a.IsActive && !a.IsDeleted).Include(a => a.Patient).
                ThenInclude(a => a.DoctorMaster).Include(a => a.AppUser).AsQueryable();

            data = data.WhereIf(clinicId > 0, a => a.Patient.DoctorMaster.Id == clinicId)
                 .WhereIf(patientId > 0, e => e.Patient.Id == patientId)
                 .WhereIf(doctorId > 0, e => e.AppUser.Id == doctorId);

            if (fromDate != null)
            {
                var date = fromDate.Value;
                TimeSpan time = new TimeSpan(0, 0, 0, 0);
                DateTime combined = date.Add(time);
                data = data.Where(a => a.AppointmentDateTime >= combined);
            }


            if (toDate != null)
            {
                var date = toDate.Value;
                TimeSpan time = new TimeSpan(0, 23, 59, 59);
                DateTime combined = date.Add(time);
                data = data.Where(a => a.AppointmentDateTime <= combined);
            }

            return data;
        }


        public virtual IQueryable<Appointment> GetAllInActive(long[] patientIds = null, long[] doctorIds = null, bool? isProcedure = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var data = _repository.Table.Include(a => a.Patient).ThenInclude(a => a.DoctorMaster).AsQueryable();
            if (patientIds?.Count() > 0)
            {
                data = data.Where(a => a.PatientId != null && patientIds.Contains(a.PatientId.Value));
            }
            if (doctorIds?.Count() > 0)
            {
                data = data.Where(a => a.AppUserId != null && doctorIds.Contains(a.AppUserId.Value));
            }
            if (isProcedure != null)
            {
                if (isProcedure == true)
                {
                    data = data.Where(a => a.VisitType == "Procedure");
                }
                else
                {
                    data = data.Where(a => a.VisitType != "Procedure");
                }
            }
            if (fromDate != null)
            {
                data = data.Where(a => a.AppointmentDateTime.Date >= fromDate.Value.Date);
            }
            if (toDate != null)
            {
                data = data.Where(a => a.AppointmentDateTime.Date <= toDate.Value.Date);
            }

            return data.Include(a => a.VitalsReport).Include(a => a.AppUser);
        }

        public virtual async Task<Appointment> Update(Appointment entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<Appointment> Create(Appointment entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public virtual async Task Delete(Appointment entity)
        {
            //entity.IsActive = false;
            await _repository.UpdateAsync(entity);
        }

        public virtual async Task<Appointment> Get(long id)
        {
            return await _repository.Table.Where(a => a.Id == id)?.Include(a => a.VitalsReport).Include(a => a.AppUser).Include(a => a.PatientFiles).Include(a => a.Patient).ThenInclude(a => a.DoctorMaster)?.FirstOrDefaultAsync();
        }
        public virtual IQueryable<Appointment> AppointmentAlreadyExist(long PatientId, long DoctorId, DateTime appoinmentDate, long? Id)
        {
            var data = _repository.Table.Where(a => a.PatientId == PatientId && a.AppUserId == DoctorId).WhereIf(Id > 0, e => e.Id != Id).AsQueryable();
            data = data.Where(a => a.AppointmentDateTime >= appoinmentDate.AddMinutes(-15));
            data = data.Where(a => a.AppointmentDateTime <= appoinmentDate.AddMinutes(15));
            return data;
        }

        public virtual IQueryable<Appointment> GetAlreadyExistPatientAppointment(long PatientId, 
            DateTime appoinmentDate, long? Id)
        {
            var data = _repository.Table.Where(a => !a.IsDeleted).WhereIf(Id > 0, e => e.Id != Id).AsQueryable();
            data = data.Where(a => a.PatientId == PatientId && a.AppointmentDateTime <= appoinmentDate.AddMinutes(15)
            && a.AppointmentDateTime >= appoinmentDate.AddMinutes(-15));
            return data;
        }

        public virtual IQueryable<Appointment> GetAlreadyExistDoctorAppointment(long DoctorId,
           DateTime appoinmentDate, long? Id)
        {
            var data = _repository.Table.Where(a => !a.IsDeleted).WhereIf(Id > 0, e => e.Id != Id).AsQueryable();
            data = data.Where(a => a.AppUserId == DoctorId && a.AppointmentDateTime <= appoinmentDate.AddMinutes(15)
            && a.AppointmentDateTime >= appoinmentDate.AddMinutes(-15));
            return data;
        }

        private DateTime getDateTimeFormate(DateTime? fromDate)
        {
            var date = fromDate.Value;
            TimeSpan time = new TimeSpan(0, 0, 0, 0);
            return date.Add(time);
             
        }

        public virtual async Task<List<PatientFiles>> GetPatientFiles(long id)
        {
            return await _repositoryPF.Table.Where(a => a.AppointmentID == id).Select(a=>a).ToListAsync();
        }

        public virtual async Task<VitalsReport> GetVitalReports(long id)
        {
            return await _repository.Table.Where(a=>a.Id == id).Select(a=>a.VitalsReport).FirstOrDefaultAsync();
        }
    }
}
