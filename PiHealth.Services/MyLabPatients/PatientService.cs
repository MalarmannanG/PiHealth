using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PiHealth.DataModel.Entity;
using PiHealth.DataModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PiHealth.Services.PiHealthPatients
{
    public class PatientService
    {
        public readonly IRepository<Patient> _repository;
        public PatientService(IRepository<Patient> repository)
        {
            _repository = repository;
        }

        public async Task<Patient> Get(long id)
        {
            return await _repository.Table.AsQueryable().Where(a => a.Id == id).Include(a => a.DoctorMaster).FirstOrDefaultAsync();
        }

        public async Task<Patient> Get(string ulId)
        {
            return await _repository.Table.AsQueryable().Where(a => a.UlID == ulId).FirstOrDefaultAsync();
        }

        public IQueryable<Patient> AutoComplete(string ulId)
        {
            var data = _repository.Table.Where(a => a.UlID.Contains(ulId)).AsQueryable();
            return data;
        }


        public virtual IQueryable<Patient> GetAll(bool isTodayPatients = false, long[] ids=null, string name = null, long? referredBy = null, bool? isDeleted = false, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var data = _repository.TableNoTracking.AsQueryable().Where(a => !a.IsDeleted);

            if (fromDate != null)
            {
                data = data.Where(a => a.CreatedDate != null && a.CreatedDate.Date >= fromDate.Value.ToLocalTime());
            }

            if (toDate != null)
            {
                data = data.Where(a => a.CreatedDate != null && a.CreatedDate.Date >= toDate.Value.ToLocalTime());
            }

            if (ids != null)
            {
                data = data.Where(a => ids.Contains(a.Id));
            }

            if(isTodayPatients)
            {
                data = data.Where(a => a.CreatedDate != null && a.CreatedDate.Date == DateTime.Now.Date);
            }

            if (!string.IsNullOrEmpty(name))
            {
                //data = data.Where(a => a.PatientName.Contains(name));
                data = data.WhereIf(!string.IsNullOrWhiteSpace(name), e => false || e.PatientName.Contains(name) || e.Age.ToString() == name || e.Gender.ToString() == name || e.Address.Contains(name) || e.MobileNumber.Contains(name) || e.DoctorMaster.Name.Contains(name));
            }

            data = data.AsQueryable();

            return data;
        }

        public virtual IQueryable<Patient> GetAllWithTracking(long[] ids = null, string name = null, long? referredBy = null, bool? isDeleted = false)
        {
            var data = _repository.Table.AsQueryable();

            return data;
        }

        public virtual Patient Update(Patient entity)
        {
            return _repository.Update(entity);
        }

        public async Task<Patient> UpdateAsync(Patient entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual Patient Create(Patient entity)
        {
            return _repository.Insert(entity);
        }

        public virtual void Delete(Patient entity)
        {
            _repository.Delete(entity);
        }

        public virtual int GetPatientsCount(DateTime? date = null)
        {
            var data = GetAll();

            if (date != null)
            {
                data = data.Where(a => a.CreatedDate.Date == date.Value.Date);
            }
            
            return data.Count();
        }
        
        public virtual async Task<Patient> GetByName(string patientName, long Id, string mobileNumber)
        {
            return _repository.Table.Where(a => a.PatientName.ToLower() == patientName.ToLower() && a.MobileNumber == mobileNumber && !a.IsDeleted && a.Id != Id).FirstOrDefault();
        }

        public string NewULID()
        {
            var id = (_repository.Table.ToList().LastOrDefault()?.Id ?? 0) + 1;
            var ulId = id.ToString("D7");
            return ulId;
        }

        
    }
}
