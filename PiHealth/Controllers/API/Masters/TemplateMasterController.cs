using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using PiHealth.Web.Filters;
using PiHealth.DataModel.Entity;
using PiHealth.Service.UserAccounts;
using PiHealth.Services;
using PiHealth.Services.Master;
using PiHealth.Web.MappingExtention;
using PiHealth.Web.Model.TemplateMaster;
using PiHealth.Controllers;
using Abp.Application.Services.Dto;
using System.Collections.Generic;
using PiHealth.Services.AppConstants;

//using Abp.Application.Services.Dto;

namespace PiHealth.Web.Controllers.API.Masters
{
    [Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class TemplateMasterController : BaseApiController
    {
        private readonly TemplateMasterService _templateMasterService;
        private readonly TemplateMasterDataMapService _templateMasterDataMapService;
        private readonly IAppUserService _appUserService;
        private readonly AuditLogServices _auditLogService;

        public TemplateMasterController(
            TemplateMasterService templateMasterService,
            TemplateMasterDataMapService templateMasterDataMapService,
            IAppUserService appUserService,
            AuditLogServices auditLogServices)
        {
            _templateMasterService = templateMasterService;
            _appUserService = appUserService;
            _templateMasterDataMapService = templateMasterDataMapService;
            _auditLogService = auditLogServices;
        }

        #region  Template Master

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _templateMasterService.Get(id);
            var templateMaster = result.ToModel(new TemplateMasterModel());
            var _patientProfileData = _templateMasterDataMapService.GetAll(id).Select(a => new PatientProfileDataMapModel() { key = a.PatientProfileData.Key, description = a.PatientProfileData.Description, patientProfileDataId = a.PatientProfileDataId, templateMasterId = a.TemplateMasterId });
            templateMaster.templateComplaints = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Complaints).Select(a => a).ToList();
            templateMaster.templateImpressions = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Impression).Select(a => a).ToList();
            templateMaster.templateAdvices = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Advice).Select(a => a).ToList();
            templateMaster.templatePlans = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Plan).Select(a => a).ToList();
            return Ok(templateMaster);
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery] TemplateQueryModel model)
        {
            var entities = _templateMasterService.GetAll(name: model.name);
            var total = entities.Count();
            entities = entities.OrderByDescending(a => a.CreatedDate).Skip(model.skip);
            if (model.take > 0)
            {
                entities = entities.Take(model.take);
            }
            var result = entities.ToList()?.Select(a => a.ToModel(new TemplateMasterModel())).ToList();
            return Ok(new { result, total });
        }

        //[HttpGet]
        //[Route("GetAll")]
        //public async Task<PagedResultDto<TemplateMasterModel>> GetAll([FromQuery] TemplateQueryModel model)
        //{
        //    var entities = _templateMasterService.GetAll(name: model.name);
        //    var total = entities.Count();
        //    entities = entities.OrderBy(a => model.Sorting ?? "id asc").Skip(model.SkipCount);
        //    if (model.MaxResultCount > 0)
        //    {
        //        entities = entities.Take(model.MaxResultCount);
        //    }
        //    var result = entities.ToList()?.Select(a => a.ToModel(new TemplateMasterModel())).ToList();
        //    //return Ok(new { result, total });
        //    return new PagedResultDto<TemplateMasterModel>(
        //        total,
        //        result
        //    );
        //}

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] TemplateMasterModel model)
        {
            long templateId = -1;
            var templateMaster = model.ToEntity(new TemplateMaster());
            templateMaster.CreatedDate = DateTime.Now;
            templateMaster.CreatedBy = ActiveUser.Id;
            var _templates = await _templateMasterService.GetByName(templateMaster.Name, 0);
            if (_templates == null)
            {
                await _templateMasterService.Create(templateMaster);
                templateId = templateMaster.Id;
                await _templateMasterDataMapService.DeleteByTemplateId(templateMaster.Id);
                var _patientProfileData = model.templateComplaints.Concat(model.templateImpressions).Concat(model.templatePlans).Concat(model.templateAdvices).ToList();
                await _templateMasterDataMapService.Create(_patientProfileData.Select(a => new TemplateMasterDataMapping() { PatientProfileDataId = a.patientProfileDataId, TemplateMasterId = templateId }).ToList());
                
            }
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");
            return Ok(templateId);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] TemplateMasterModel model)
        {
            long templateId = -1;
            var _templates = await _templateMasterService.GetByName(model.name, model.id);
            if (_templates == null)
            {
                if (model == null)
                    return BadRequest();

                var templateMaster = await _templateMasterService.UpdateGet(model.id);

                if (templateMaster == null)
                    return BadRequest();

                templateMaster = model.ToEntity(templateMaster);
                templateMaster.IsDeleted = model.isDeleted;
                templateMaster.ModifiedDate = DateTime.Now;
                templateMaster.ModifiedBy = ActiveUser.Id;

                await _templateMasterService.Update(templateMaster);
                await _templateMasterDataMapService.DeleteByTemplateId(templateMaster.Id);
                var _patientProfileData = model.templateComplaints.Concat(model.templateImpressions).Concat(model.templatePlans).Concat(model.templateAdvices).ToList();
                await _templateMasterDataMapService.Create(_patientProfileData.Select(a => new TemplateMasterDataMapping() { PatientProfileDataId = a.patientProfileDataId, TemplateMasterId = a.templateMasterId }).ToList());
            }
            else
                model.id = templateId;
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");
            
            return Ok(model);

        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var templateMaster = await _templateMasterService.UpdateGet(id);

            if (templateMaster == null)
                return BadRequest();

            templateMaster.IsDeleted = true;
            templateMaster.ModifiedDate = DateTime.Now;
            templateMaster.ModifiedBy = ActiveUser.Id;

            await _templateMasterService.Update(templateMaster);
            await _templateMasterDataMapService.DeleteByTemplateId(templateMaster.Id);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok();
        }

        #endregion Template Master Ends
    }
}