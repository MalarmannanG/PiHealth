﻿using PiHealth.DataModel;
using PiHealth.DataModel.Entity;
using PiHealth.DataModel.Entity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiHealth.Services.Master
{
    public class PrescriptionMasterService
    {
        public readonly IRepository<PrescriptionMaster> _repository;
        public readonly IRepository<Prescription> _repositoryPrescription;

        public PrescriptionMasterService(IRepository<PrescriptionMaster> repository, IRepository<Prescription> repositoryPrescription)
        {
            _repository = repository;
            _repositoryPrescription = repositoryPrescription;
        }

        public virtual IQueryable<PrescriptionMaster> GetAll(string name = null)
        {
            var data = _repository.TableNoTracking.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(name))
            {
                data = data.Where(a => a.MedicineName.Contains(name) || a.GenericName.Contains(name));
            }

            return data;
        }

        public virtual async Task<PrescriptionMaster> Update(PrescriptionMaster entity)
        {
            return await _repository.UpdateAsync(entity);
        }

        public virtual async Task<PrescriptionMaster> Create(PrescriptionMaster entity)
        {
            return await _repository.InsertAsync(entity);
        }

        public virtual async Task Delete(PrescriptionMaster entity)
        {
            entity.IsDeleted = true;
            await _repository.UpdateAsync(entity);
        }

        public virtual async Task<PrescriptionMaster> Get(long id)
        {
            return await _repository.GetByIdAsync(id);
        }
        public virtual IQueryable<string> GetInstructions()
        {
            return _repository.Table.Where(a => !string.IsNullOrEmpty(a.Instructions)).Select(a => a.Instructions).Distinct();
        }
        public virtual void UpdatePatientPrescription(long patientProfileId)
        {
            var data = _repositoryPrescription.TableNoTracking.Where(a =>  a.PatientProfileId == patientProfileId).ToList();
            _repositoryPrescription.Delete(data);
        }
    }
}
