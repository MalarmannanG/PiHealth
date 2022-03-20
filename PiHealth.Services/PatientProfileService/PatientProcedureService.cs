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
    public class PatientProcedureService
    {
        public readonly IRepository<PatientProcedure> _repository;
        public PatientProcedureService(IRepository<PatientProcedure> repository)
        {
            _repository = repository;
        }
        public virtual async Task<PatientProcedure> Get(long id)
        {
            return await _repository.Table.Where(a => a.Id == id).Include(a => a.PatientProfile).FirstOrDefaultAsync();
        }
        public virtual async Task<PatientProcedure> GetByProfileId(long id)
        {
            return await _repository.Table.Where(a => a.PatientProfileId == id).Include(a=>a.DoctorMaster).FirstOrDefaultAsync();
        }
        
        public virtual async Task<PatientProcedure> Update(PatientProcedure entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<PatientProcedure> Create(PatientProcedure entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public virtual async Task Delete(PatientProcedure entity)
        {
            await _repository.UpdateAsync(entity);
        }
    }
}
