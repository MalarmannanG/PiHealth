using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PiHealth.DataModel.Entity;
using PiHealth.Services.PiHealthPatients;
using PiHealth.Web.Helper;
using PiHealth.Web.MappingExtention;
using PiHealth.Web.Model.Patient;
using PiHealth.Controllers;
using PiHealth.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PiHealth.Web.Model.Prefix;
using PiHealth.Web.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using PiHealth.Service.UserAccounts;
using PiHealth.Services.Master;

namespace PiHealth.Web.Controllers.API.Masters
{
    [Authorize]
    [Route("Api/[Controller]")]
    [Produces("application/json")]
    public class PatientMasterController : BaseApiController
    {     
        private readonly PatientService _patientService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AuditLogServices _auditLogService;
        private readonly IConfiguration _configuration;
        private readonly IOptionsSnapshot<PrefixOption> _prefixOption;
        private readonly IAppUserService _userService;
        private readonly DoctorMasterService _doctorMasterService;

        public PatientMasterController(
            IHostingEnvironment hostingEnvironment,
            PatientService patientService,
            AuditLogServices auditLogServices,
            IConfiguration configuration,
             IOptionsSnapshot<PrefixOption> prefixOption,
             IAppUserService userService,
             DoctorMasterService doctorMasterService)
        {          
            _patientService = patientService;
            _hostingEnvironment = hostingEnvironment;
            _auditLogService = auditLogServices;
            _configuration = configuration;
            _prefixOption = prefixOption;
            _userService = userService;
            _doctorMasterService = doctorMasterService;
        }

        #region  Patient Master
        
        [HttpGet]
        [Route("Get/{id}")]        
        public async Task<IActionResult> Get(long id)
        {
            var entity = await _patientService.Get(id); 
            if (entity == null)
                return BadRequest();
            var model = entity.ToModel(new PatientModel());

            return Ok(model);
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery] PatientQueryModel model)
        {
            var entities = _patientService.GetAll(isTodayPatients: model.isTodayPatients, name: model?.name, fromDate: model.fromDate, toDate: model.toDate);
            var total = entities.Count();
            var orderBy = string.IsNullOrEmpty(model?.order_by) ? "CreatedDate" : model.order_by;
            entities = entities?.Include(a => a.DoctorMaster).OrderByDescending(a => a.CreatedDate).Skip(model.skip);
            if (model.take > 0)
            {
                entities = entities.Take(model.take);
            }
            //var result = entities.ToList()?.Select(a => a.ToModel(new PatientModel())).ToList();
            var result = entities.Select(a => new
            {
                id = a.Id,
                //hrno = a.HrNo,
                initial = a.Initial,
                patientName = a.PatientName,
                //dob = a.DOB,
                age = a.Age,
                gender = a.PatientGender,
                ulId = a.UlID,
                mobileNumber = a.MobileNumber,
                //referedBy = a.DoctorMasterId,
                referedByName = a.DoctorMaster.Name,
                referedDoctorName = a.DoctorMaster.ClinicName,
                address = a.Address
            }).ToList();
            return Ok(new { result, total });
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] PatientModel model)
        {
            
            if (model == null)
                return BadRequest();

            var ulIdPrefix = (_prefixOption?.Value?.ULIDPrefix ?? "");
            var entity = model.ToEntity(new Patient());
            entity.HrNo = _patientService.NewULID();
            entity.HrNo = ulIdPrefix + entity.HrNo;
            entity.CreatedBy = ActiveUser.Id;
            entity.CreatedDate = DateTime.Now;
            var _templates = await _patientService.GetByName(model.patientName, 0, model.mobileNumber);
            if (_templates == null)
            {
                var patient = _patientService.Create(entity);
                model = patient?.ToModel(new PatientModel());
            }
            else
                model.id = -1;
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: ActiveUser.Id, RequestIP: RequestIP, value1: "Success");

            return Ok(model);
        }
                
        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] PatientModel model)
        {
            if (model == null)
                return BadRequest();

            var entity = await _patientService.Get(model.id);

            if (entity == null)
                return BadRequest();

            
            var _templates = await _patientService.GetByName(model.patientName, model.id, model.mobileNumber);
            if (_templates == null)
            {
                entity = model.ToEntity(entity);
                _patientService.Update(entity);
                model.id = entity.Id;
            }
            else
                model.id = -1;
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: ActiveUser.Id, RequestIP: RequestIP, value1: "Success");
            return Ok(model);
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var entity = await _patientService.Get(id);

            entity.ModifiedBy = ActiveUser.Id;
            entity.ModifiedDate = DateTime.Now;
            entity.IsDeleted = true;

            _patientService.Update(entity);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: ActiveUser.Id, RequestIP: RequestIP, value1: "Success");

            return Ok(true);
        }

        [HttpGet]
        [Route("AutoComplete")]
        public IActionResult AutoComplete(string ulId = null)
        {
            if (!string.IsNullOrEmpty(ulId) && !string.IsNullOrWhiteSpace(ulId))
            {
                var patients = _patientService.AutoComplete(ulId: ulId).ToList().Select(a => a.ToModel(new PatientModel())).ToList();
                return Ok(patients);
            }

            return BadRequest();
        }

        [HttpGet]
        [Route("GetDoctorsAndFacilities")]
        public IActionResult GetDoctorsAndFacilities()
        {
            var doctors = _userService.GetAll(userType: "Doctor").Select(a => new { id = a.Id, name = a.Name })
                .OrderBy(a => a.name).ToList();
            var facilities = _doctorMasterService.GetAll().Select(a => new {id=a.Id,name=a.Name,doctorName=a.ClinicName})
                .OrderBy(a=>a.name).ToList();
            return Ok(new { doctors, facilities });
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("Migration")]
        public IActionResult Migration(bool go = false)
        {
            var entities = _patientService.GetAllWithTracking().ToList();
            foreach (var entity in entities)
            {
                entity.DOB = Helper.Helper.NumberToDate(entity.Age, 0, 0);
                //_patientService.Update(entity);
            }

            //for finished test only invoice paid, pending = invoice pending, cancelld = invoice dispute and paymnets are cancelled


            //insert doctor master from doctor name
            //add ref id from doctor id
            //Created date patient test created date
            // tax = 0
            //persentage = defult = 0
            //created by = current user

            //insert invoice master from patient test 
            // patient test id is report id
            //create invoice no
            //Created date patient test created date
            //ref commision default = 0            
            // tax = 0
            //total amount  = (actAmount > 0) ? actAmount : testCost;
            //defult amount = test cost
            //Created date patient test created date, raised on and paid on
            //created by = current user
            //if cancelled test = dispute status
            //if pendng test or process= pending
            //if verfied = paid

            // line item from invoice master
            // test id and package id from patient test packages  where package id != null are test line item with use the test id rest are package (group and insert)
            // amount and total amount test or package amount from master.
            // tax = 0
            //Created date invoice master created date
            //created by = current user

            //payment from invoice
            // invoice id from master
            //amount and tptal from invoice master
            //description paid
            //if dispute invoice = isdeleted
            //Created date invoice master created date
            //created by = current user
            //no payment for pending and dispute or processing
            
            return Ok();
        }

        #endregion Patient Master Ends
    }
}