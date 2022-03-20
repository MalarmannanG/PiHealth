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
using PiHealth.Services.PatientProfileService;
using PiHealth.Web.MappingExtention;
using PiHealth.Web.Model.Appointment;
using PiHealth.Web.Model.PatientProfile;
using PiHealth.Web.Model.VitalsReport;
using PiHealth.Controllers;
using PiHealth.Web.Model.PatientProcedureModel;

namespace PiHealth.Web.Controllers.API
{
    [Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class PatientProfileController : BaseApiController
    {
        private readonly PatientProfileService _patientProfileService;
        private readonly IAppUserService _appUserService;
        private readonly AuditLogServices _auditLogService;
        private readonly AppointmentService _appointmentService;
        private readonly PatientProcedureService _patientProcedureService;

        public PatientProfileController(
            PatientProfileService patientProfileService,
            IAppUserService appUserService,
            AppointmentService appointmentService,
            PatientProcedureService patientProcedureService,
            AuditLogServices auditLogServices)
        {
            _patientProfileService = patientProfileService;
            _appUserService = appUserService;
            _appointmentService = appointmentService;
            _auditLogService = auditLogServices;
            _patientProcedureService = patientProcedureService;
        }

        #region  PatientProfile Master

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _patientProfileService.Get(id);
            var patientProfile = result?.ToModel(new PatientProfileModel()) ?? new PatientProfileModel();
            //patientProfile.followUp.HasValue
            if (result == null)
            {
                var appointment = await _appointmentService.Get(id);
                patientProfile.patientModel = appointment?.Patient?.ToModel(new Model.Patient.PatientModel());
                patientProfile.appointment = appointment?.ToModel(new AppointmentModel());
                patientProfile.appointment.patientFiles = appointment?.PatientFiles?.Select(a => a.ToModel(new PatientFilesModel())).ToList();                
                patientProfile.appointmentId = appointment.Id;
                patientProfile.patientId = appointment.PatientId ?? 0;
                patientProfile.procedureModel = new ProcedureModel();
                var entity = await _patientProcedureService.GetByProfileId(patientProfile.id);
                if (entity != null)
                {
                    patientProfile.procedureModel.id = entity.Id;
                    patientProfile.procedureModel.referedBy = entity.DoctorMasterId;
                    patientProfile.procedureModel.referedByName = entity.DoctorMaster.Name;
                    patientProfile.procedureModel.date = entity.Date;
                    patientProfile.procedureModel.description = entity.Description;
                    patientProfile.procedureModel.diagnosis = entity.Diagnosis;
                    patientProfile.procedureModel.others = entity.Others;
                    patientProfile.procedureModel.procedurename = entity.Procedurename;
                    patientProfile.procedureModel.actualCost = entity.ActualCost;
                    patientProfile.procedureModel.anesthesia = entity.Anesthesia;
                    patientProfile.procedureModel.complication = entity.Complication;
                    patientProfile.procedureModel.createdBy = entity.CreatedBy;
                    patientProfile.procedureModel.createdDate = entity.CreatedDate;
                }

            }
            else
            {
                patientProfile.procedureModel = new ProcedureModel();
                var entity = await _patientProcedureService.GetByProfileId(patientProfile.id);
                if (entity != null)
                {
                    patientProfile.procedureModel.id = entity.Id;
                    patientProfile.procedureModel.referedBy = entity.DoctorMasterId;
                    patientProfile.procedureModel.referedByName = entity.DoctorMaster.Name;
                    patientProfile.procedureModel.date = entity.Date;
                    patientProfile.procedureModel.description = entity.Description;
                    patientProfile.procedureModel.diagnosis = entity.Diagnosis;
                    patientProfile.procedureModel.others = entity.Others;
                    patientProfile.procedureModel.procedurename = entity.Procedurename;
                    patientProfile.procedureModel.actualCost = entity.ActualCost;
                    patientProfile.procedureModel.anesthesia = entity.Anesthesia;
                    patientProfile.procedureModel.complication = entity.Complication;
                    patientProfile.procedureModel.createdBy = entity.CreatedBy;
                    patientProfile.procedureModel.createdDate = entity.CreatedDate;
                }

            }
            return Ok(patientProfile);
        }

        [HttpGet]
        [Route("GetByPatient/{id}")]
        public async Task<IActionResult> GetByPatient(long id)
        {
            var result = await _patientProfileService.GetByPatient(id);
            var patientProfile = result?.ToModel(new PatientProfileModel()) ?? new PatientProfileModel();
            var prescriptios = await _patientProfileService.GetPrescriptions(result.Id);
            patientProfile.prescriptionModel = prescriptios?.Prescriptions.Select(a => new PrescriptionModel() { beforeFood = a.BeforeFood, categoryName = a.CategoryName, genericName=a.GenericName,
             medicineName = a.MedicineName, morning=a.Morning, night=a.Night, noOfDays=a.NoOfDays, noon=a.Noon, remarks=a.Remarks, strength=a.Strength,units=a.Units }).ToList();
            if (result == null)
            {
                var appointment = await _appointmentService.Get(result.AppointmentId);
                patientProfile.patientModel = appointment?.Patient?.ToModel(new Model.Patient.PatientModel());
                patientProfile.appointment = appointment?.ToModel(new AppointmentModel());
                patientProfile.appointment.patientFiles = appointment?.PatientFiles?.Select(a => a.ToModel(new PatientFilesModel())).ToList();
                patientProfile.appointmentId = appointment.Id;
                patientProfile.patientId = appointment.PatientId ?? 0;
                
            }
            return Ok(patientProfile);
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery] PatientProfileQueryModel model)
        {
            var appointments = _appointmentService.GetAllInActive().Where(a => a.PatientId == model.PatientId && a.AppointmentDateTime <= model.appointmentDate).ToList();
            var appointmentIds = appointments.Select(a => a.Id).ToArray();
            var patientProfiles = _patientProfileService.GetAll(patientId: model.PatientId, appointmentIds: appointmentIds).OrderByDescending(a => a.CreatedDate).ToList().Select(a => a.ToModel(new PatientProfileModel())).ToList();
            return Ok(patientProfiles);
        }

        [HttpGet]
        [Route("GetAllInActive")]
        public IActionResult GetAllInActive([FromQuery] PatientProfileQueryModel model)
        {
            var appointments = _appointmentService.GetAllInActive().Where(a => a.PatientId == model.PatientId).ToList();
            var appointmentIds = appointments.Select(a => a.Id).ToArray();
            var patientProfiles = _patientProfileService.GetAll(patientId: model.PatientId, appointmentIds: appointmentIds).OrderByDescending(a => a.CreatedDate).ToList().Select(a => a.ToModel(new PatientProfileModel())).ToList();
            return Ok(patientProfiles);
        }
        [HttpGet]
        [Route("GetAllComplaints")]
        public IActionResult GetAllComplaints()
        {
            var complaints = _patientProfileService.GetComplaints();
            return Ok(complaints);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] PatientProfileModel model)
        {
            var patientProfile = model.ToEntity(new PatientProfile());
            patientProfile.CreatedDate = DateTime.Now;
            patientProfile.CreatedBy = ActiveUser.Id;
            await _patientProfileService.Create(patientProfile);
            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok(patientProfile);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] PatientProfileModel model)
        {
            if (model == null)
                return BadRequest();

            var patientProfile = await _patientProfileService.Get(model.appointmentId);
            var _appoinment = await _appointmentService.Get(model.appointmentId);
            try
            {
                if (patientProfile == null)
                {                    
                    patientProfile = model.ToEntity(new PatientProfile());
                    patientProfile.CreatedDate = DateTime.Now;
                    patientProfile.CreatedBy = ActiveUser.Id;
                    if (!model.appointment.isActive)
                    {
                        _appoinment.IsActive = false;
                        _appoinment.UpdatedBy = ActiveUser.Id;
                        _appoinment.UpdatedDate = DateTime.Now;
                        await _appointmentService.Update(_appoinment);
                    }
                    
                    await _patientProfileService.Create(patientProfile);
                }
                else
                {

                    patientProfile = model.ToEntity(patientProfile);
                    patientProfile.IsDeleted = model.isDeleted;
                    patientProfile.ModifiedDate = DateTime.Now;
                    patientProfile.ModifiedBy = ActiveUser.Id;
                      if (!model.appointment.isActive)
                    {
                        _appoinment.IsActive = false;
                        _appoinment.UpdatedBy = ActiveUser.Id;
                        _appoinment.UpdatedDate = DateTime.Now;
                        await _appointmentService.Update(_appoinment);
                    }
                    await _patientProfileService.Update(patientProfile);
                    _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

                }
                if(model.procedureModel != null)
                {
                    if (model.procedureModel.id > 0)
                    {
                        var patientProcedure = await _patientProcedureService.Get(model.procedureModel.id);
                        var entity = model.procedureModel.ToEntity(patientProcedure);
                        await _patientProcedureService.Update(entity);

                    }
                    else
                    {
                        var entity = model.procedureModel.ToEntity(new PatientProcedure());
                        var _model = model.procedureModel;
                        entity.DoctorMasterId = _model.referedBy;
                        entity.Date = _model.date;
                        entity.Description = _model.description;
                        entity.Diagnosis = _model.diagnosis;
                        entity.Others = _model.others;
                        entity.Procedurename = _model.procedurename;
                        entity.ActualCost = _model.actualCost;
                        entity.Anesthesia = _model.anesthesia;
                        entity.Complication = _model.complication;
                        entity.PatientProfileId = patientProfile.Id;
                        await _patientProcedureService.Create(entity);
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            try
            {
                if (model.isfollowUpNeed && !model.appointment.isActive)
                {
                    _appoinment.Id = 0;
                    _appoinment.AppUserId = ActiveUser.Id;
                    _appoinment.AppointmentDateTime = model.followUp.Value;
                    _appoinment.IsActive = true;
                    await _appointmentService.Create(_appoinment);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ok(model);

        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {

            var patientProfile = await _patientProfileService.Get(id);

            if (patientProfile == null)
                return BadRequest();

            patientProfile.IsDeleted = true;

            patientProfile.ModifiedDate = DateTime.Now;
            patientProfile.ModifiedBy = ActiveUser.Id;

            await _patientProfileService.Update(patientProfile);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok();
        }

        #endregion PatientProfile Ends
    }
}