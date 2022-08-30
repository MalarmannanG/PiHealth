using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using PiHealth.DataModel.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace PiHealth.Services.Master
{
    public class PatientProfileDataService
    {
        public readonly IRepository<PatientProfileData> _repository;

        public PatientProfileDataService(IRepository<PatientProfileData> repository)
        {
            _repository = repository;
        }

        public virtual IQueryable<PatientProfileData> GetAll(string description = null, long id = 0)
        {
            var data = _repository.Table.Where(a => !a.IsDeleted).WhereIf(id > 0, a => a.Key == id).WhereIf(!string.IsNullOrEmpty(description), a => a.Description.Contains(description));
            return data;
        }
        public virtual async Task<PatientProfileData> Create(PatientProfileData entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public virtual async Task<PatientProfileData> Update(PatientProfileData entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<PatientProfileData> Get(long? id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public virtual async Task<PatientProfileData> GetPatientProfileData(string description,long key, long id)
        {
            return await _repository.Table.Where(a => a.Description.ToLower() == description.ToLower() && a.Key == key && !a.IsDeleted && a.Id != id).FirstOrDefaultAsync();
        }
    }
}
