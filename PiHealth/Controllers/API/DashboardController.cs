using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PiHealth.Web.Filters;
using PiHealth.DataModel.Entity;
using PiHealth.Service.UserAccounts;
using PiHealth.Services;
using PiHealth.Services.Master;
using PiHealth.Services.PiHealthPatients;
using PiHealth.Web.MappingExtention;
using PiHealth.Web.Model.Appointment;
using PiHealth.Controllers;
using System.Globalization;
using System.ComponentModel;
using PiHealth.DataModel.Options;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;

namespace PiHealth.Controllers.API
{
    [Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class DashboardController : BaseApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly AppointmentService _appointmentService;
        private readonly IAppUserService _appUserService;
        private readonly AuditLogServices _auditLogService;
        private readonly PatientService _patientService;
        private readonly DoctorMasterService _doctorMasterService;
        private readonly IAppUserService _userService;
        private readonly IDistributedCache _distributedCache;

        public DashboardController(
            PatientService patientService,
            AppointmentService appointmentService,
            IAppUserService appUserService,
            AuditLogServices auditLogServices,
            DoctorMasterService doctorMasterService,
           IAppUserService userService,
           IDistributedCache distributedCache)
        {
            _appointmentService = appointmentService;
            _appUserService = appUserService;
            _auditLogService = auditLogServices;
            _patientService = patientService;
            _userService = userService;
            _doctorMasterService = doctorMasterService;
            _distributedCache = distributedCache;
        }

        [HttpPost]
        [Route("CardData")]
        public async Task<IActionResult> CardData([FromBody] DashboardQueryModel model)
        {
            var startdate = DateTime.Now;
            DateTime? fromDate = string.IsNullOrEmpty(model.fromDate) ? null : DateTime.Parse(model.fromDate);
            DateTime? toDate = string.IsNullOrEmpty(model.toDate) ? null : DateTime.Parse(model.toDate);
            var data = _appointmentService.GetDashboardCardCount(patientId: model.patientId, doctorId: model.doctorId, clinicId: model.clinicId, fromDate: fromDate, toDate: toDate);
            int _appointmentCount = data.Where(a => a.VisitType.ToLower() != "procedure").Count();
            int _procedureCount = data.Where(a => a.VisitType.ToLower() == "procedure").Count();
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for Dashboard Card {0}", diff.TotalSeconds.ToString()));
            return Ok(new { Appointments = _appointmentCount, Procedures = _procedureCount });
        }

        [HttpGet]
        [Route("InitialLoad/{id}")]
        public IActionResult InitialLoad(long id)
        {
            var patientsCount = _patientService.GetPatientsCount();

            var doctorsquery = _userService.GetAll(id, userType: "Doctor").OrderByDescending(a => a.Name);
            var doctors = doctorsquery.Select(a => new AppUser { Name = a.Name, Id = a.Id }).ToList();


            var facilities = _doctorMasterService.GetAll().OrderByDescending(a => a.Name).Select(a => a.Name).ToList();

            return Ok(new { patientsCount, doctors, facilities });
        }

        //Redis implementation

        //public async Task<IActionResult> InitialLoad(long id)
        //{
        //    //Redis Start
        //    var cacheKey = "facilities";
        //    string serializedFacilitiesList;
        //    List<string> facilitiesList = new List<string>();
        //    var redisFacilitiesList = await _distributedCache.GetAsync(cacheKey);
        //    if(redisFacilitiesList != null)
        //    {
        //        serializedFacilitiesList = Encoding.UTF8.GetString(redisFacilitiesList);
        //        facilitiesList = JsonConvert.DeserializeObject<List<string>>(serializedFacilitiesList);
        //    }
        //    else
        //    {
        //        facilitiesList = _doctorMasterService.GetAll().OrderByDescending(a => a.Name).Select(a => a.Name).ToList();
        //        serializedFacilitiesList = JsonConvert.SerializeObject(facilitiesList);
        //        redisFacilitiesList = Encoding.UTF8.GetBytes(serializedFacilitiesList);
        //        var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(10)).SetSlidingExpiration(TimeSpan.FromMinutes(2));
        //        await _distributedCache.SetAsync(cacheKey, redisFacilitiesList, options);
        //    }
        //    //Redis End

        //    var patientsCount = _patientService.GetPatientsCount();

        //    var doctorsquery = _userService.GetAll(id,userType:"Doctor").OrderByDescending(a => a.Name);
        //    var doctors = doctorsquery.Select(a => new AppUser{ Name=a.Name, Id=a.Id }).ToList();


        //    var facilities = _doctorMasterService.GetAll().OrderByDescending(a => a.Name).Select(a => a.Name).ToList();

        //    return Ok(new { patientsCount, doctors, facilities, facilitiesList });
        //}
<<<<<<< HEAD
=======
        public async Task<IActionResult> InitialLoad(long id)
        {
            var startdate = DateTime.Now;
            //Redis Start
            var patientsCount = _patientService.GetPatientsCount();

            var doctorsquery = _userService.GetAll(id).OrderByDescending(a => a.Name);
            var doctors = doctorsquery.Select(a => a.ToModel()).ToList();

            var facilityQuery = _doctorMasterService.GetAll();
            facilityQuery = facilityQuery?.OrderByDescending(a => a.ClinicName);
            var facilities = facilityQuery?.ToList().Select(a => a.ToModel(new Web.Model.DoctorMaster.DoctorMasterModel())).ToList();

            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for Dashboard InitialLoad {0}", diff.TotalSeconds.ToString()));
            return Ok(new { patientsCount, doctors, facilities });
        }
>>>>>>> 0750c47ce879a239fc8f41f3a5089f5bedec9293

        [HttpPost]
        [Route("GetAllAppointment")]
        public IActionResult GetAllAppointment([FromBody] DashboardQueryModel model)
        {
            var startdate = DateTime.Now;
            DateTime? fromDate = string.IsNullOrEmpty(model.fromDate) ? null : DateTime.Parse(model.fromDate);
            DateTime? toDate = string.IsNullOrEmpty(model.toDate) ? null : DateTime.Parse(model.toDate);

            var appointments = _appointmentService.GetDashboardAll(patientId: model.patientId, doctorId: model.doctorId, clinicId: model.clinicId, fromDate: fromDate, toDate: toDate, isProcedure: model?.isProcedure);
            var total = appointments?.Count();

            appointments = appointments?.OrderByDescending(a => a.AppointmentDateTime);

            //if (model.take > 0)
            //{
            //    appointments = appointments.Take(model.take);
            //}

            var result = appointments.ToList().Select(a => a.ToModel(new AppointmentModel())).OrderByDescending(a => a.appointmentDateTime).ToList() ?? new List<AppointmentModel>();
            TimeSpan diff = DateTime.Now - startdate;
            log.Error(string.Format("Time taken for Dashboard GetAllAppointment {0}", diff.TotalSeconds.ToString()));
            return Ok(new { result, total });
        }
    }
}
