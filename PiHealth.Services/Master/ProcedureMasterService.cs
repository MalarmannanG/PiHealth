using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace PiHealth.Services.Master
{
    public class ProcedureMasterService
    {
        public readonly IRepository<ProcedureMaster> _repository;
        public ProcedureMasterService(IRepository<ProcedureMaster> repository)
        {
            _repository = repository;
        }

        public virtual IQueryable<ProcedureMaster> GetAll(string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(name))
            {
                //data = data.Where(a => a.Name.Contains(name));
                data = data.WhereIf(!string.IsNullOrWhiteSpace(name), e => false || e.Procedurename.Contains(name) || e.Description.Contains(name));
            }

            return data;
        }
        public virtual IQueryable<ProcedureMaster> AutoComplete(string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
            {
                data = data.Where(a => a.Procedurename.Contains(name));
            }

            return data;
        }

        public virtual async Task<ProcedureMaster> Update(ProcedureMaster entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<ProcedureMaster> Create(ProcedureMaster entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public virtual async Task Delete(ProcedureMaster entity)
        {
            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
        }

        public virtual async Task<ProcedureMaster> Get(long id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public virtual async Task<ProcedureMaster> GetByName(string diagName, long Id)
        {
            return _repository.Table.Where(a => a.Procedurename.ToLower() == diagName.ToLower() && !a.IsDeleted && a.Id != Id).FirstOrDefault();
        }
    }
}
