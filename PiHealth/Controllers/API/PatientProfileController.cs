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
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Newtonsoft.Json;

namespace PiHealth.Web.Controllers.API
{
    //[Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class PatientProfileController : BaseApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly PatientProfileService _patientProfileService;
        private readonly PatientProfileDataMapService _patientProfileDataMapService;
        private readonly IAppUserService _appUserService;
        //private readonly AuditLogServices _auditLogService;
        private readonly AppointmentService _appointmentService;
        private readonly PatientProcedureService _patientProcedureService;
        private readonly PatientService _patientService;
        private readonly DoctorMasterService _doctorService;
        private readonly PrescriptionMasterService _prescriptionMasterService;
        private readonly TestMasterService _testMasterService;
        private readonly DiagnosisMasterService _diagnosisMasterService;
        private readonly TemplateMasterService _templateMasterService;
        private readonly ProcedureMasterService _procedureMasterService;
        private readonly IDistributedCache _distributedCache;

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
            AuditLogServices auditLogServices,
            IDistributedCache distributedCache)
        {
            _patientProfileService = patientProfileService;
            _patientProfileDataMapService = patientProfileDataMapService;
            _appUserService = appUserService;
            _appointmentService = appointmentService;
            //_auditLogService = auditLogServices;
            _patientProcedureService = patientProcedureService;
            _patientService = patientService;
            _doctorService = doctorService;
            _prescriptionMasterService = prescriptionMaster;
            _testMasterService = testMaster;
            _diagnosisMasterService = diagnosisMaster;
            _templateMasterService = templateMaster;
            _procedureMasterService = procedureMaster;
            _distributedCache = distributedCache;
        }

        #region  PatientProfile Master

        [HttpGet]
        [Route("Test")]
        public async Task<IActionResult> Test()
        {

            return Ok(_patientProfileService.test());
        }
       

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var startdate = DateTime.Now;
            var result = await _patientProfileService.GetByAppointment(id);

            var patientProfile = result?.ToModel(new PatientProfileModel()) ?? new PatientProfileModel();
            var _patientProfileData = _patientProfileDataMapService.GetAll(patientProfile.id).Select(a => new PatientProfileDataMapMdl() { key = a.PatientProfileData.Key, description = a.PatientProfileData.Description, patientProfileDataId = a.PatientProfileDataId, patientProfileId = a.PatientProfileId }).ToList();
            if (patientProfile.id > 0)
            {
                var _patientDiagnosis = await _patientProfileService.getDiagnosis(patientProfile.id);
                patientProfile.patientDiagnosisModel = _patientDiagnosis.Select(a => a.ToModel(new PatientDiagnosisModel())).ToList();
                var _patientTest = await _patientProfileService.getTestValues(patientProfile.id);
                patientProfile.patientTestModel = _patientTest.Select(a => a.ToModel(new PatientTestModel())).ToList();
                var _patientPrescription = await _patientProfileService.getPrescriptions(patientProfile.id);
                patientProfile.prescriptionModel = _patientPrescription.Select(a => a.ToModel(new PrescriptionModel())).ToList();
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
                    patientProfile.procedureModel = entity.ToModel1(new ProcedureModel());
                    if (entity.DoctorMasterId > 0)
                    {
                        var doctorentity = await _doctorService.Get(entity.DoctorMasterId.Value);
                        patientProfile.procedureModel.referedByName = doctorentity.Name;
                    }
                }
            }
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile Get {0}", diff.TotalSeconds.ToString()));
            return Ok(patientProfile);
        }
         
        [HttpGet]
        [Route("GetByPatient/{id}")]
        public async Task<IActionResult> GetByPatient(long id)
        {
            var startdate = DateTime.Now;
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
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile GetByPatient {0}", diff.TotalSeconds.ToString()));
            return Ok(patientProfile);
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery] PatientProfileQueryModel model)
        {
            var startdate = DateTime.Now;
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
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile GetAll {0}", diff.TotalSeconds.ToString()));
            return Ok(patientProfiles);
        }

        [HttpGet]
        [Route("GetAllInActive")]
        public IActionResult GetAllInActive([FromQuery] PatientProfileQueryModel model)
        {
            var startdate = DateTime.Now;
            var appointments = _appointmentService.GetAllInActive().Where(a => a.PatientId == model.PatientId).ToList();
            var appointmentIds = appointments.Select(a => a.Id).ToArray();
            var patientProfiles = _patientProfileService.GetAll(patientId: model.PatientId, appointmentIds: appointmentIds).OrderByDescending(a => a.CreatedDate).ToList().Select(a => a.ToModel(new PatientProfileModel())).ToList();
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile AllInactive {0}", diff.TotalSeconds.ToString()));
            return Ok(patientProfiles);
        }

        [HttpGet]
        [Route("GetAllInstructions")]
        public IActionResult GetAllInstructions()
        {
            var startdate = DateTime.Now;
            var _instructions = _prescriptionMasterService.GetInstructions();
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile GetAllInstructions {0}", diff.TotalSeconds.ToString()));
            return Ok(new { instructions = _instructions });
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] PatientProfileModel model)
        {

            var startdate = DateTime.Now;
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

            if (model.procedureMasterId.HasValue)
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

            await _patientProfileService.Create(patientProfile);
            await _patientProfileDataMapService.DeleteByPatientProfileId(patientProfile.Id);
            var _patientProfileData = model.patientComplaints.Concat(model.patientImpressions).Concat(model.patientPlans)
                .Concat(model.patientAdvices).Concat(model.patientExaminations).Concat(model.patientInvestigationResults).ToList();
            await _patientProfileDataMapService.Create(_patientProfileData.Select(a => new PatientProfileDataMapping() { PatientProfileDataId = a.patientProfileDataId, PatientProfileId = patientProfile.Id }).ToList());
            //_auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile Create PatiPro {0}", diff.TotalSeconds.ToString()));

            return Ok(patientProfile);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] PatientProfileModel model)
        {
            var startdate = DateTime.Now;
            if (model == null)
                return BadRequest();

            var patientProfile = await _patientProfileService.GetByAppointment(model.appointmentId);
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
                    _prescriptionMasterService.UpdatePatientPrescription(patientProfile.Id);
                    _diagnosisMasterService.UpdatePatientDiagnosis(patientProfile.Id);
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
                    if (!string.IsNullOrEmpty(model.appointment.referredBy) && 
                        _appoinment.ReferredBy != model.appointment.referredBy)
                    {
                        _appoinment.ReferredBy = model.appointment.referredBy;
                        await _appointmentService.Update(_appoinment);
                    }
                    await _patientProfileDataMapService.DeleteByPatientProfileId(patientProfile.Id);
                    await _patientProfileService.Update(patientProfile);
                    var _patientProfileData = model.patientComplaints.Concat(model.patientImpressions).Concat(model.patientPlans)
                                                                     .Concat(model.patientAdvices).Concat(model.patientExaminations)
                                                                     .Concat(model.patientInvestigationResults).ToList();
                    await _patientProfileDataMapService.Create(_patientProfileData.Select(a => new PatientProfileDataMapping() { PatientProfileDataId = a.patientProfileDataId, PatientProfileId = patientProfile.Id }).ToList());
                    //_auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

                }
                if (model.procedureMasterId.HasValue)
                {
                    var patientProcedure = await _patientProcedureService.Get(model.procedureModel.id);
                    var entity = model.procedureModel.ToEntity(patientProcedure??new PatientProcedure());
                    await _patientProcedureService.Update(entity);

                }
                else
                {
                    var entity = model.procedureModel.ToEntity(new PatientProcedure());
                    entity.PatientProfileId = patientProfile.Id;
                    await _patientProcedureService.Create(entity);
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
                    _model.Description = model.appointment.description;
                    _model.VisitType = "Follow Up";
                    _model.CreatedDate = DateTime.Now;
                    _model.CreatedBy = ActiveUser.Id;
                    _model.UpdatedDate = DateTime.Now;
                    _model.UpdatedBy = ActiveUser.Id;
                    _model.AppointmentDateTime = DateTime.ParseExact(model.followUpDate, "ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                    _model.IsActive = true;
                    _model.ReferredBy = model.appointment.referredBy;
                    await _appointmentService.Create(_model);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile Update PatientP {0}", diff.TotalSeconds.ToString()));

            return Ok(model);

        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {

            var patientProfile = await _patientProfileService.GetByAppointment(id);

            if (patientProfile == null)
                return BadRequest();

            patientProfile.IsDeleted = true;

            patientProfile.ModifiedDate = DateTime.Now;
            patientProfile.ModifiedBy = ActiveUser.Id;

            await _patientProfileService.Update(patientProfile);
            await _patientProfileDataMapService.DeleteByPatientProfileId(patientProfile.Id);

            //_auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok();
        }

        [HttpGet]
        [Route("PastVisitCount/{id}")]
        public async Task<IActionResult> PastVisitCount(long id)
        {
            var startdate = DateTime.Now;
            var result = await _patientProfileService.GetByPatient(id);
            int a = 0;
            if (result != null)
                a = 1;
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile Past Visits {0}", diff.TotalSeconds.ToString()));
            return Ok(a);
        }


        [HttpGet]
        [Route("LastVisit/{id}")]
        public async Task<IActionResult> LastVisit(long id)
        {
            var startdate = DateTime.Now;
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
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile GetAllInstructions {0}", diff.TotalSeconds.ToString()));
            return Ok(patientProfile);
        }


        [HttpGet]
        [Route("getInitialLoad")]
        public IActionResult getInitialLoad()
        {
            var startdate = DateTime.Now;
            var prescriptions = _prescriptionMasterService.GetAll().ToList();
            var testMasters = _testMasterService.GetAll().ToList();
            var diagnosis = _diagnosisMasterService.GetAll().ToList();
            var templates = _templateMasterService.GetAllTemplates().ToList();
            //var procedures = _procedureMasterService.GetAll().ToList();
            var procedures = _procedureMasterService.GetAll()
                .Select(a => new
                {
                    actualCost = a.ActualCost,
                    anesthesia = a.Anesthesia,
                    complication = a.Complication,
                    date = a.Date,
                    description = a.Description,
                    diagnosis = a.Diagnosis,
                    id = a.Id,
                    others = a.Others,
                    name = a.Procedurename
                }).ToList();
            var doctors = _doctorService.GetAll().ToList();
            //var patientProfileDatas = _patientProfileDataMapService.GetAll(id).ToList();
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile Initial Load {0}", diff.TotalSeconds.ToString()));
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

        //Redis implementation

        //[HttpGet]
        //[Route("getRedisPrescriptionData")]
        //public async Task<IActionResult> getRedisPrescriptionData()
        //{
        //    var cacheKey = "prescriptions";
        //    string serializedPrescriptionsList;
        //    List<PrescriptionMaster> prescriptionList = new List<PrescriptionMaster>();
        //    var redisPrescriptionList = await _distributedCache.GetAsync(cacheKey);
        //    if(redisPrescriptionList != null)
        //    {
        //        serializedPrescriptionsList = Encoding.UTF8.GetString(redisPrescriptionList);
        //        prescriptionList = JsonConvert.DeserializeObject<List<PrescriptionMaster>>(serializedPrescriptionsList);
        //    }
        //    else
        //    {
        //        prescriptionList = _prescriptionMasterService.GetAll().ToList();
        //        serializedPrescriptionsList = JsonConvert.SerializeObject(prescriptionList);
        //        redisPrescriptionList = Encoding.UTF8.GetBytes(serializedPrescriptionsList);
        //        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));
        //        await _distributedCache.SetAsync(cacheKey, redisPrescriptionList, options);
        //    }
        //    return Ok(new
        //    {
        //        prescriptionsList = prescriptionList,
        //    });
        //}

        [HttpGet]
        [Route("GetPatientFiles/{id}")]
        public IActionResult GetPatientFiles(long id)
        {
            var startdate = DateTime.Now;
            var _patientFiles = _appointmentService.GetPatientFiles(id);
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile GetPatientFiles {0}", diff.TotalSeconds.ToString()));
            return Ok(new { PatientFiles = _patientFiles });
        }

        [HttpGet]
        [Route("GetVitalReports/{id}")]
        public async Task<IActionResult> GetVitalReports(long id)
        {
            var startdate = DateTime.Now;
            var result = await _appointmentService.GetVitalReports(id);
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for PatienProfile GetVitalReports {0}", diff.TotalSeconds.ToString()));
            if (result != null)
                return Ok(result.ToModel(new VitalsReportModel()));
            else
                return Ok(new VitalsReportModel());
        }

        #endregion PatientProfile Ends
    }
}