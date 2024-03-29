﻿using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using PiHealth.DataModel.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PiHealth.Services;

namespace PiHealth.Services.Master
{   
    public class DoctorMasterService
    {
        public readonly IRepository<DoctorMaster> _repository;     
        public DoctorMasterService(IRepository<DoctorMaster> repository)
        {
            _repository = repository;           
        }      

        public virtual IQueryable<DoctorMaster> GetAll(string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(name))
            {
                //data = data.Where(a => a.Name.Contains(name));
                data = data.WhereIf(!string.IsNullOrWhiteSpace(name), e => false || e.Name.Contains(name) || e.ClinicName.Contains(name) || e.PhoneNo1.Contains(name) || e.Email.Contains(name) || e.PinCode.ToString().Contains(name));
            }

            return data;
        }
        public virtual IQueryable<DoctorMaster> AutoComplete(string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
            {
                data = data.Where(a => a.Name.Contains(name));
            }

            return data;
        }

        public virtual async Task<DoctorMaster> Update(DoctorMaster entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<DoctorMaster> Create(DoctorMaster entity)
        {
           return await _repository.InsertAsync(entity);
        }

        public virtual async Task Delete(DoctorMaster entity)
        {
            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
        }

        public virtual async Task<DoctorMaster> Get(long id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public virtual async Task<DoctorMaster> GetByName(string doctorName, long Id, string clinicName)
        {
            return _repository.Table.Where(a => a.Name.ToLower() == doctorName.ToLower() && a.ClinicName.ToLower() == clinicName.ToLower() && !a.IsDeleted && a.Id != Id).FirstOrDefault();
        }
    }
}
