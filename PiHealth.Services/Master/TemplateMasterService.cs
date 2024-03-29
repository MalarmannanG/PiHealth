﻿using PiHealth.DataModel;
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
    public class TemplateMasterService
    {
        public readonly IRepository<TemplateMaster> _repository;
        //public readonly IRepository<TemplateMasterDataMapService> _dataRepository;
        public TemplateMasterService(IRepository<TemplateMaster> repository)
        {
            _repository = repository;           
        }      

        public virtual IQueryable<TemplateMaster> GetAll(string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted).Include(a => a.TemplatePrescriptions).ThenInclude(a=>a.PrescriptionMaster).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                //data = data.Where(a => a.Name.Contains(name));
                data = data.WhereIf(!string.IsNullOrWhiteSpace(name), e => false || e.Name.Contains(name) || e.Compliants.Contains(name) || e.Examination.Contains(name) || e.Impression.Contains(name) || e.Advice.Contains(name) || e.Plan.Contains(name) || e.FollowUp.Contains(name));
            }

            return data;
        }
        public virtual IQueryable<TemplateMaster> AutoComplete(string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(name) && !string.IsNullOrWhiteSpace(name))
            {
                data = data.Where(a => a.Name.Contains(name));
            }

            return data;
        }

        public virtual async Task<TemplateMaster> Update(TemplateMaster entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<TemplateMaster> Create(TemplateMaster entity)
        {
           return await _repository.InsertAsync(entity);
        }

        public virtual async Task Delete(TemplateMaster entity)
        {
            entity.IsDeleted = true;
            //return await _repository.UpdateAsync(entity);
            _repository.Delete(entity);
        }

        public virtual async Task<TemplateMaster> Get(long id)
        {
            return await _repository.TableNoTracking.Where(a => a.Id == id).Include(a => a.TemplatePrescriptions)?.ThenInclude(a=>a.PrescriptionMaster).FirstOrDefaultAsync();
        }
        public virtual async Task<TemplateMaster> GetByName(string TemplateName, long id)
        {
            return await _repository.Table.Where(a => a.Name.ToLower() == TemplateName.ToLower() && !a.IsDeleted && a.Id != id).FirstOrDefaultAsync();
        }
        public virtual async Task<TemplateMaster> UpdateGet(long id)
        {
            return await _repository.Table.Where(a=> a.Id == id).Include(a => a.TemplatePrescriptions).FirstOrDefaultAsync();
        }

        public virtual IQueryable<TemplateMaster> GetAllTemplates()
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted).AsQueryable();
            return data;
        }
    }
}
