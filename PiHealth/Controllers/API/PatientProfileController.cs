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
using PiHealth.Services.PiHealthPatients;
using System.Globalization;
using PiHealth.Services.AppConstants;

namespace PiHealth.Web.Controllers.API
{
    //[Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class PatientProfileController : BaseApiController
    {
        private readonly PatientProfileService _patientProfileService;
        private readonly PatientProfileDataMapService _patientProfileDataMapService;
        private readonly IAppUserService _appUserService;
        private readonly AuditLogServices _auditLogService;
        private readonly AppointmentService _appointmentService;
        private readonly PatientProcedureService _patientProcedureService;
        private readonly PatientService _patientService;
        private readonly DoctorMasterService _doctorService;
        private readonly PrescriptionMasterService _prescriptionMasterService;
        private readonly TestMasterService _testMasterService;
        private readonly DiagnosisMasterService _diagnosisMasterService;
        private readonly TemplateMasterService _templateMasterService;
        private readonly ProcedureMasterService _procedureMasterService;

        public PatientProfileController(
            PatientProfileService patientProfileService,
            PatientProfileDataMapService patientProfileDataMapService,
            IAppUserService appUserService,
            AppointmentService appointmentService,
            PatientProcedureService patientProcedureService,
            PatientService patientService,
            DoctorMasterService doctorService,
            PrescriptionMasterService prescriptionMaster,
             TestMasterService testMaster,
             DiagnosisMasterService diagnosisMaster,
             TemplateMasterService templateMaster,
             ProcedureMasterService procedureMaster,
            AuditLogServices auditLogServices)
        {
            _patientProfileService = patientProfileService;
            _patientProfileDataMapService = patientProfileDataMapService;
            _appUserService = appUserService;
            _appointmentService = appointmentService;
            _auditLogService = auditLogServices;
            _patientProcedureService = patientProcedureService;
            _patientService = patientService;
            _doctorService = doctorService;
            _prescriptionMasterService = prescriptionMaster;
            _testMasterService = testMaster;
            _diagnosisMasterService = diagnosisMaster;
            _templateMasterService = templateMaster;
            _procedureMasterService = procedureMaster;
        }

        #region  PatientProfile Master


        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> Get(long id)
        {

            var result = await _patientProfileService.Get(id);
            var patientProfile = result?.ToModel(new PatientProfileModel()) ?? new PatientProfileModel();
            var _patientProfileData = _patientProfileDataMapService.GetAll(patientProfile.id).Select(a => new PatientProfileDataMapMdl() { key = a.PatientProfileData.Key, description = a.PatientProfileData.Description, patientProfileDataId = a.PatientProfileDataId, patientProfileId = a.PatientProfileId }).ToList();
            if (patientProfile.id > 0)
            {
                patientProfile.patientComplaints = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Complaints).Select(a => a).ToList();
                patientProfile.patientImpressions = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Impression).Select(a => a).ToList();
                patientProfile.patientAdvices = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Advice).Select(a => a).ToList();
                patientProfile.patientPlans = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Plan).Select(a => a).ToList();
                patientProfile.patientExaminations = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Examination).Select(a => a).ToList();
                patientProfile.patientInvestigationResults = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.InvestigationResult).Select(a => a).ToList();
            }
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
            }
            else
            {
                patientProfile.procedureModel = new ProcedureModel();
                var entity = await _patientProcedureService.GetByProfileId(patientProfile.id);
                if (entity != null)
                {
                    patientProfile.procedureModel.id = entity.Id;
                    patientProfile.procedureModel.referedBy = entity.DoctorMasterId;
                    if (entity.DoctorMasterId > 0)
                    {
                        var doctorentity = await _doctorService.Get(entity.DoctorMasterId.Value);
                        patientProfile.procedureModel.referedByName = doctorentity.Name;
                    }
                    patientProfile.procedureModel.date = entity.Date;
                    patientProfile.procedureModel.description = entity.Description;
                    patientProfile.procedureModel.diagnosis = entity.Diagnosis;
                    patientProfile.procedureModel.others = entity.Others;
                    patientProfile.procedureModel.name = entity.Procedurename;
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
            var _patientProfileData = _patientProfileDataMapService.GetAll(patientProfile.id).Select(a => new PatientProfileDataMapMdl() { key = a.PatientProfileData.Key, description = a.PatientProfileData.Description, patientProfileDataId = a.PatientProfileDataId, patientProfileId = a.PatientProfileId });
            if (patientProfile.id > 0)
            {
                patientProfile.patientComplaints = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Complaints).Select(a => a).ToList();
                patientProfile.patientImpressions = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Impression).Select(a => a).ToList();
                patientProfile.patientAdvices = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Advice).Select(a => a).ToList();
                patientProfile.patientPlans = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Plan).Select(a => a).ToList();
                patientProfile.patientExaminations = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Examination).Select(a => a).ToList();
                patientProfile.patientInvestigationResults = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.InvestigationResult).Select(a => a).ToList();
            }

            if (result == null)
            {
                patientProfile.appointment = new AppointmentModel();
                patientProfile.appointment.patientFiles = new System.Collections.Generic.List<PatientFilesModel>();
                patientProfile.patientModel = new Model.Patient.PatientModel();
                patientProfile.procedureModel = new ProcedureModel();
            }
            else
            {
                var prescriptios = await _patientProfileService.GetPrescriptions(result.Id);
                patientProfile.prescriptionModel = prescriptios?.Prescriptions.Select(a => a.ToModel(new PrescriptionModel())).ToList();

                patientProfile.procedureModel = new ProcedureModel();
                var entity = await _patientProcedureService.GetByProfileId(patientProfile.id);
                if (entity != null)
                {
                    patientProfile.procedureModel.id = entity.Id;
                    patientProfile.procedureModel.referedBy = entity.DoctorMasterId;
                    if (entity.DoctorMasterId > 0)
                    {
                        var doctorentity = await _doctorService.Get(entity.DoctorMasterId.Value);
                        patientProfile.procedureModel.referedByName = doctorentity.Name;
                    }
                    patientProfile.procedureModel.date = entity.Date;
                    patientProfile.procedureModel.description = entity.Description;
                    patientProfile.procedureModel.diagnosis = entity.Diagnosis;
                    patientProfile.procedureModel.others = entity.Others;
                    patientProfile.procedureModel.name = entity.Procedurename;
                    patientProfile.procedureModel.actualCost = entity.ActualCost;
                    patientProfile.procedureModel.anesthesia = entity.Anesthesia;
                    patientProfile.procedureModel.complication = entity.Complication;
                    patientProfile.procedureModel.createdBy = entity.CreatedBy;
                    patientProfile.procedureModel.createdDate = entity.CreatedDate;
                }
                else
                {
                    var patient = await _patientService.Get(result.Patient.Id);
                    patientProfile.procedureModel.referedByName = patient.DoctorMaster.Name;
                    patientProfile.procedureModel.referedBy = result.Patient.DoctorMasterId;
                }

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
            for (int i = 0; i < patientProfiles.Count; i++)
            {
                var _patientProfileData = _patientProfileDataMapService.GetAll(patientProfiles[i].id).Select(a => new PatientProfileDataMapMdl() { key = a.PatientProfileData.Key, description = a.PatientProfileData.Description, patientProfileDataId = a.PatientProfileDataId, patientProfileId = a.PatientProfileId });
                {
                    patientProfiles[i].patientComplaints = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Complaints).Select(a => a).ToList();
                    patientProfiles[i].patientImpressions = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Impression).Select(a => a).ToList();
                    patientProfiles[i].patientAdvices = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Advice).Select(a => a).ToList();
                    patientProfiles[i].patientPlans = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Plan).Select(a => a).ToList();
                    patientProfiles[i].patientExaminations = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.Examination).Select(a => a).ToList();
                    patientProfiles[i].patientInvestigationResults = _patientProfileData.Where(a => a.key == (int)ProfileDataEnum.InvestigationResult).Select(a => a).ToList();
                }
            }
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
        [Route("GetAllInstructions")]
        public IActionResult GetAllInstructions()
        {
            //var _complaints = _patientProfileService.GetComplaints();
            //var _advices = _patientProfileService.GetAdvice();
            //var _plans = _patientProfileService.GetPlan();
            //var _impressions = _patientProfileService.GetImpression();
            var _instructions = _patientProfileService.GetInstructions();
            //return Ok(new { complaints = _complaints, advices = _advices, plans = _plans, impressions = _impressions, instructions = _instructions });
            return Ok(new { instructions = _instructions });
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] PatientProfileModel model)
        {
            var patientProfile = model.ToEntity(new PatientProfile());
            var _appoinment = await _appointmentService.Get(model.appointmentId);
            patientProfile.CreatedDate = DateTime.Now;
            patientProfile.CreatedBy = ActiveUser.Id;
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
            if (model.procedureModel != null)
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
                    entity.PatientProfileId = patientProfile.Id;
                    var _model = entity.ToModel(model.procedureModel);
                    if (entity.DoctorMasterId > 0)
                        await _patientProcedureService.Create(_model);
                }
            }
            await _patientProfileService.Create(patientProfile);
            await _patientProfileDataMapService.DeleteByPatientProfileId(patientProfile.Id);
            var _patientProfileData = model.patientComplaints.Concat(model.patientImpressions).Concat(model.patientPlans)
                .Concat(model.patientAdvices).Concat(model.patientExaminations).Concat(model.patientInvestigationResults).ToList();
            await _patientProfileDataMapService.Create(_patientProfileData.Select(a => new PatientProfileDataMapping() { PatientProfileDataId = a.patientProfileDataId, PatientProfileId = patientProfile.Id }).ToList());
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
                    patientProfile.DoctorServiceId = model.doctorServiceId;
                    if (!model.appointment.isActive)
                    {
                        _appoinment.IsActive = false;
                        _appoinment.UpdatedBy = ActiveUser.Id;
                        _appoinment.UpdatedDate = DateTime.Now;
                        await _appointmentService.Update(_appoinment);
                    }

                    await _patientProfileService.Create(patientProfile);
                    await _patientProfileDataMapService.DeleteByPatientProfileId(patientProfile.Id);
                    var _patientProfileData = model.patientComplaints.Concat(model.patientImpressions).Concat(model.patientPlans)
                .Concat(model.patientAdvices).Concat(model.patientExaminations).Concat(model.patientInvestigationResults).ToList();
                    await _patientProfileDataMapService.Create(_patientProfileData.Select(a => new PatientProfileDataMapping() { PatientProfileDataId = a.patientProfileDataId, PatientProfileId = patientProfile.Id }).ToList());
                }
                else
                {

                    patientProfile = model.ToEntity(patientProfile);
                    patientProfile.IsDeleted = model.isDeleted;
                    patientProfile.ModifiedDate = DateTime.Now;
                    patientProfile.ModifiedBy = ActiveUser.Id;
                    patientProfile.DoctorServiceId = model.doctorServiceId;
                    if (!model.appointment.isActive)
                    {
                        _appoinment.IsActive = false;
                        _appoinment.UpdatedBy = ActiveUser.Id;
                        _appoinment.UpdatedDate = DateTime.Now;
                        await _appointmentService.Update(_appoinment);
                    }
                    await _patientProfileService.Update(patientProfile);
                    await _patientProfileDataMapService.DeleteByPatientProfileId(patientProfile.Id);
                    var _patientProfileData = model.patientComplaints.Concat(model.patientImpressions).Concat(model.patientPlans)
                .Concat(model.patientAdvices).Concat(model.patientExaminations).Concat(model.patientInvestigationResults).ToList();
                    await _patientProfileDataMapService.Create(_patientProfileData.Select(a => new PatientProfileDataMapping() { PatientProfileDataId = a.patientProfileDataId, PatientProfileId = patientProfile.Id }).ToList());
                    _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

                }
                if (model.procedureModel != null)
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
                    Appointment _model = new Appointment();
                    _model.PatientId = _appoinment.PatientId;
                    _model.AppUserId = ActiveUser.Id;
                    _model.VisitType = "Follow Up";
                    _model.CreatedDate = DateTime.Now;
                    _model.CreatedBy = ActiveUser.Id;
                    _model.UpdatedDate = DateTime.Now;
                    _model.UpdatedBy = ActiveUser.Id;
                    _model.AppointmentDateTime = DateTime.ParseExact(model.followUpDate, "ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    _model.IsActive = true;
                    await _appointmentService.Create(_model);
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
            await _patientProfileDataMapService.DeleteByPatientProfileId(patientProfile.Id);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok();
        }

        [HttpGet]
        [Route("PastVisitCount/{id}")]
        public async Task<IActionResult> PastVisitCount(long id)
        {
            var result = await _patientProfileService.GetByPatient(id);
            int a = 0;
            if (result != null)
                a = 1;
            return Ok(a);
        }


        [HttpGet]
        [Route("LastVisit/{id}")]
        public async Task<IActionResult> LastVisit(long id)
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
            }
            else
            {
                patientProfile.procedureModel = new ProcedureModel();
                var entity = await _patientProcedureService.GetByProfileId(patientProfile.id);
                if (entity != null)
                {
                    patientProfile.procedureModel.id = entity.Id;
                    patientProfile.procedureModel.referedBy = entity.DoctorMasterId;
                    if (entity.DoctorMasterId > 0)
                    {
                        var doctorentity = await _doctorService.Get(entity.DoctorMasterId.Value);
                        patientProfile.procedureModel.referedByName = doctorentity.Name;
                    }
                    patientProfile.procedureModel.date = entity.Date;
                    patientProfile.procedureModel.description = entity.Description;
                    patientProfile.procedureModel.diagnosis = entity.Diagnosis;
                    patientProfile.procedureModel.others = entity.Others;
                    patientProfile.procedureModel.name = entity.Procedurename;
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
        [Route("getInitialLoad")]
        public IActionResult getInitialLoad()
        {
            var prescriptions = _prescriptionMasterService.GetAll().ToList();
            var testMasters = _testMasterService.GetAll().ToList();
            var diagnosis = _diagnosisMasterService.GetAll().ToList();
            var templates = _templateMasterService.GetAllTemplates().ToList();
            var procedures = _procedureMasterService.GetAll().ToList();
            var doctors = _doctorService.GetAll().ToList();
            //var patientProfileDatas = _patientProfileDataMapService.GetAll(id).ToList();
            return Ok(new
            {
                prescriptions = prescriptions,
                testMasters = testMasters,
                diagnosis = diagnosis,
                templates = templates,
                procedures = procedures,
                doctors = doctors
            });
        }

        #endregion PatientProfile Ends
    }
}