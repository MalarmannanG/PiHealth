using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using PiHealth.DataModel.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PiHealth.Services.Master
{
    public class PatientProfileDataMapService
    {
        public readonly IRepository<PatientProfileDataMapping> _repository;

        public PatientProfileDataMapService(IRepository<PatientProfileDataMapping> repository)
        {
            _repository = repository;
        }

        public virtual IQueryable<PatientProfileDataMapping> GetAll(long patientProfileId = 0)
        {
            var data = _repository.TableNoTracking.WhereIf(patientProfileId > 0, a => a.PatientProfileId == patientProfileId).Include(a => a.PatientProfileData).AsQueryable();
            return data;
        }

        public virtual async Task<PatientProfileDataMapping> Update(PatientProfileDataMapping entity)
        {
            return await _repository.UpdateAsync(entity);
        }
        public virtual async Task Create(List<PatientProfileDataMapping> entities)
        {
            await _repository.InsertAsync(entities);
        }

        public virtual async Task Delete(PatientProfileDataMapping entity)
        {
            _repository.Delete(entity);
        }
        public virtual async Task DeleteByPatientProfileId(long patientProfileId)
        {
            var items = _repository.Table.Where(a => a.PatientProfileId == patientProfileId).AsQueryable();
            _repository.Delete(items);
        }
    }
}
