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
    public class TemplateMasterDataMapService
    {
        public readonly IRepository<TemplateMasterDataMapping> _repository;
        public TemplateMasterDataMapService(IRepository<TemplateMasterDataMapping> repository)
        {
            _repository = repository;
        }

        public virtual IQueryable<TemplateMasterDataMapping> GetAll(long templateMasterId = 0)
        {
            var data = _repository.TableNoTracking.WhereIf(templateMasterId > 0, a => a.TemplateMasterId == templateMasterId).Include(a => a.PatientProfileData).AsQueryable();
            return data;
        }


        public virtual async Task<TemplateMasterDataMapping> Update(TemplateMasterDataMapping entity)
        {

            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task Create(List<TemplateMasterDataMapping> entities)
        {
            await _repository.InsertAsync(entities);
        }

        public virtual async Task Delete(TemplateMasterDataMapping entity)
        {
            _repository.Delete(entity);
        }
        public virtual async Task DeleteByTemplateId(long templateMasterId)
        {
            var items = _repository.Table.Where(a => a.TemplateMasterId == templateMasterId).AsQueryable();
            _repository.Delete(items);

        }
    }
       
}
