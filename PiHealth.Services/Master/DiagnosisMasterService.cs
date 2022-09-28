using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using PiHealth.DataModel.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.Services.Master
{
    public class DiagnosisMasterService
    {
        public readonly IRepository<DiagnosisMaster> _repository;
        public readonly IRepository<PatientDiagnosis> _repositoryPatientDiag;
        public DiagnosisMasterService(IRepository<DiagnosisMaster> repository, IRepository<PatientDiagnosis> repositoryPatientDiag)
        {
            _repository = repository;
            _repositoryPatientDiag = repositoryPatientDiag;
        }

        public virtual IQueryable<DiagnosisMaster> GetAll(string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(name))
            {
                //data = data.Where(a => a.Name.Contains(name));
                data = data.WhereIf(!string.IsNullOrWhiteSpace(name), e => false || e.Name.Contains(name) || e.Description.Contains(name));
            }

            return data;
        }
        public virtual IQueryable<DiagnosisMaster> AutoComplete(string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
            {
                data = data.Where(a => a.Name.Contains(name));
            }

            return data;
        }

        public virtual async Task<DiagnosisMaster> Update(DiagnosisMaster entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<DiagnosisMaster> Create(DiagnosisMaster entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public virtual async Task Delete(DiagnosisMaster entity)
        {
            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
        }

        public virtual async Task<DiagnosisMaster> Get(long id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public virtual async Task<DiagnosisMaster> GetByName(string diagName, long Id)
        {
            return _repository.Table.Where(a => a.Name.ToLower() == diagName.ToLower() && !a.IsDeleted && a.Id != Id).FirstOrDefault();
        }
        public virtual void UpdatePatientDiagnosis(long id)
        {
            var data = _repositoryPatientDiag.TableNoTracking.Where(a => !a.IsDeleted).ToList();
            _repositoryPatientDiag.Delete(data);
        }

    }
}
