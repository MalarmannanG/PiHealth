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
using PiHealth.Web.Model.DoctorService;

namespace PiHealth.Controllers.API.Masters
{
    [Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class DoctorServicsController : BaseApiController
    {
        private readonly DoctorDataServices _doctorService;
        private readonly IAppUserService _appUserService;
        private readonly AuditLogServices _auditLogService;

        public DoctorServicsController(
           DoctorDataServices doctorService,
           IAppUserService appUserService,
           AuditLogServices auditLogServices)
        {
            _doctorService = doctorService;
            _appUserService = appUserService;
            _auditLogService = auditLogServices;
        }

        [HttpGet]
        [Route("GetAll")]
        public async Task<PagedResultDto<DoctorServiceModel>> GetAll([FromQuery] DoctorServiceInput model)
        {
            var entities = _doctorService.GetAll(model.userId, name: model.name);
            var total = entities.Count();
            entities = entities.OrderBy(a => model.Sorting ?? "id asc").Skip(model.SkipCount);
            if (model.MaxResultCount > 0)
            {
                entities = entities.Take(model.MaxResultCount);
            }
            var result = entities.ToList()?.Select(a => a.ToModel(new DoctorServiceModel())).ToList();

            //return Ok(new { result, total });
            return new PagedResultDto<DoctorServiceModel>(
                total,
                result
            );
        }

        [HttpGet]
        [Route("Get")]
        public async Task<DoctorServiceModel> Get([FromQuery] long id)
        {
            var entity = await _doctorService.Get(id);
            DoctorServiceModel model =  entity.ToModel(new DoctorServiceModel());
            return model;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<DoctorServiceModel> Create([FromBody] DoctorServiceModel model)
        {
            var doctorService = model.ToEntity(new DoctorService());
            doctorService.CreatedDate = DateTime.Now;
            doctorService.CreatedBy = ActiveUser.Id;
            var _templates = await _doctorService.GetByName(doctorService.ServiceName, ActiveUser.Id, 0);
            if (_templates == null)
            {
                await _doctorService.Create(doctorService);

            }
            else
            {
                doctorService.Id = -1;
            }
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");
            return  doctorService.ToModel(new DoctorServiceModel());
        }

        [HttpPut]
        [Route("Update")]
        public async Task<DoctorServiceModel> Update([FromBody] DoctorServiceModel model)
        {
            var templateMaster = await _doctorService.UpdateGet(model.id);
            var doctorService = model.ToEntity(templateMaster);
            doctorService.ModifiedDate = DateTime.Now;
            doctorService.ModifiedBy = ActiveUser.Id;
            var _templates = await _doctorService.GetByName(doctorService.ServiceName, ActiveUser.Id, model.id);
            if (_templates == null)
            {
                await _doctorService.Update(doctorService);

            }
            else
            {
                doctorService.Id = -1;
            }
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");
            return model;

        }

        [HttpDelete]
        [Route("Delete")]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            var templateMaster = await _doctorService.UpdateGet(id);

            if (templateMaster == null)
                return BadRequest();

            templateMaster.IsDeleted = true;
            templateMaster.ModifiedDate = DateTime.Now;
            templateMaster.ModifiedBy = ActiveUser.Id;

            await _doctorService.Update(templateMaster);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok();
        }
    }
}
