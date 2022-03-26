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
using PiHealth.Web.Model.DiagnosisMaster;
using PiHealth.Controllers;
using PiHealth.Model.ProcedureMaster;
using PiHealth.MappingExtention;

namespace PiHealth.Controllers.API.Masters
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcedureMasterController : BaseApiController
    {
        private readonly ProcedureMasterService _procedureMasterService;
        private readonly IAppUserService _appUserService;
        private readonly AuditLogServices _auditLogService;

        public ProcedureMasterController(
            ProcedureMasterService procedureMasterService,
            IAppUserService appUserService,
            AuditLogServices auditLogServices)
        {
            _procedureMasterService = procedureMasterService;
            _appUserService = appUserService;
            _auditLogService = auditLogServices;
        }

        #region  Diagnosis Master

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _procedureMasterService.Get(id);
            var diagnosis = result.ToModel(new ProcedureMasterModel());
            return Ok(diagnosis);
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery] ProcedureMasterQueryModel model)
        {
            var entities = _procedureMasterService.GetAll(name: model.name);
            var total = entities.Count();
            entities = entities.OrderByDescending(a => a.CreatedDate).Skip(model.skip);
            if (model.take > 0)
            {
                entities = entities.Take(model.take);
            }
            var result = entities.ToList()?.Select(a => a.ToModel(new ProcedureMasterModel())).ToList();
            return Ok(new { result, total });
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] ProcedureMasterModel model)
        {
            var procedure = model.ToEntity(new ProcedureMaster());
            procedure.CreatedDate = DateTime.Now;
            procedure.CreatedBy = ActiveUser.Id;
            long templateId = -1;
            var _templates = await _procedureMasterService.GetByName(model.procedurename, 0);
            if (_templates == null)
            {
                await _procedureMasterService.Create(procedure);
                templateId = procedure.Id;
            }
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok(templateId);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] ProcedureMasterModel model)
        {
            long templateId = -1;
            if (model == null)
                return BadRequest();
            var _templates = await _procedureMasterService.GetByName(model.procedurename, model.id);
            if (_templates == null)
            {
                var procedure = await _procedureMasterService.Get(model.id);

                if (procedure == null)
                    return BadRequest();
                procedure.Procedurename = model.procedurename;
                procedure.Description = model.description;
                procedure.IsDeleted = model.isDeleted;
                procedure.ModifiedDate = DateTime.Now;
                procedure.ModifiedBy = ActiveUser.Id;
                procedure.ActualCost = model.actualCost;
                procedure.Anesthesia = model.anesthesia;
                procedure.Complication = model.complication;
                procedure.Others = model.others;
                procedure.Diagnosis = model.diagnosis;
                procedure.Description = procedure.Description;
                await _procedureMasterService.Update(procedure);
                _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");
            }
            else
                model.id = templateId;
            return Ok(model);

        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {

            var procedure = await _procedureMasterService.Get(id);

            if (procedure == null)
                return BadRequest();

            procedure.IsDeleted = true;
            procedure.ModifiedDate = DateTime.Now;
            procedure.ModifiedBy = ActiveUser.Id;

            await _procedureMasterService.Update(procedure);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok();
        }

        #endregion Procedure Ends

    }
}
