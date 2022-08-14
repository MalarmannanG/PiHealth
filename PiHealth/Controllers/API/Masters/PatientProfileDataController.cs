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
using PiHealth.Model.PatientProfileData;
using PiHealth.MappingExtention;

namespace PiHealth.Controllers.API.Masters
{
    [Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class PatientProfileDataController : BaseApiController
    {
        private readonly PatientProfileDataService _patientProfileDataService;
      
        private readonly AuditLogServices _auditLogService;

        public PatientProfileDataController(
            PatientProfileDataService patientProfileDataService,
            AuditLogServices auditLogServices)
        {
            _patientProfileDataService = patientProfileDataService;
            _auditLogService = auditLogServices;
           
        }

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var entities = await _patientProfileDataService.Get(id);
            var result = entities.ToModel(new PatientProfileDataModel());
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery] PatientProfileDataQueryModel model)
        {
            var entities = _patientProfileDataService.GetAll(model.description, model.key);
            var total = entities.Count();
            entities = entities.OrderByDescending(a => a.CreatedDate).Skip(model.skip);
            if (model.take > 0)
            {
                entities = entities.Take(model.take);
            }
            var result = entities.ToList()?.Select(a => a.ToModel(new PatientProfileDataModel())).ToList();
            return Ok(new { result, total });
        }

        [HttpPost]
        [Route("Create")]
        public async Task<PatientProfileDataModel> Create([FromBody] PatientProfileDataModel model)
        {
            var patientProfileData = model.ToEntity(new PatientProfileData());
            patientProfileData.CreatedDate = DateTime.Now;
            patientProfileData.CreatedBy = ActiveUser.Id;
            await _patientProfileDataService.Create(patientProfileData);
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");
            return model;
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] PatientProfileDataModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            var entity = await _patientProfileDataService.Get(model.id);

            if (entity == null)
            {
                return BadRequest();
            }

            entity = model.ToEntity(entity);
            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedBy = ActiveUser.Id;

            await _patientProfileDataService.Update(entity);
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");
            return Ok(model);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _patientProfileDataService.Get(id);

            if (entity == null)
            {
                return BadRequest();
            }

            entity.IsDeleted = true;
            entity.ModifiedDate = DateTime.Now;
            entity.ModifiedBy = ActiveUser.Id;

            await _patientProfileDataService.Update(entity);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok();
        }
    }
}
