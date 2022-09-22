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
    public class DoctorDataServices
    {
        public readonly IRepository<DoctorService> _repository;
        public DoctorDataServices(IRepository<DoctorService> repository)
        {
            _repository = repository;
        }

        public virtual IQueryable<DoctorService> GetAll(long userId, string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted).WhereIf(!string.IsNullOrEmpty(name), e => e.ServiceName.Contains(name))
                   .WhereIf(userId > 0, e => e.AppUserId == userId).Include(a=>a.AppUser);
            return data;
        }
        public virtual async Task<DoctorService> Get(long id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public virtual async Task<DoctorService> Update(DoctorService entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<DoctorService> Create(DoctorService entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public virtual async Task Delete(DoctorService entity)
        {
            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
        }

        public virtual async Task<DoctorService> GetByName(string name, long userId, long id)
        {
            return await _repository.Table.Where(a => a.ServiceName.ToLower() == name.ToLower() && !a.IsDeleted && a.Id != id && a.AppUserId == userId).FirstOrDefaultAsync();
        }
        public virtual async Task<DoctorService> UpdateGet(long id)
        {
            return await _repository.Table.Where(a => a.Id == id).FirstOrDefaultAsync();
        }
    }
}
