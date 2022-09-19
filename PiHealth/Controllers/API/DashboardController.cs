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

namespace PiHealth.Controllers.API
{
    //[Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class DashboardController : BaseApiController
    {

        private readonly AppointmentService _appointmentService;
        private readonly IAppUserService _appUserService;
        private readonly AuditLogServices _auditLogService;
        private readonly PatientService _patientService;
        private readonly DoctorMasterService _doctorMasterService;
        private readonly IAppUserService _userService;

        public DashboardController(
            PatientService patientService,
            AppointmentService appointmentService,
            IAppUserService appUserService,
            AuditLogServices auditLogServices,
            DoctorMasterService doctorMasterService,
           IAppUserService userService)
        {
            _appointmentService = appointmentService;
            _appUserService = appUserService;
            _auditLogService = auditLogServices;
            _patientService = patientService;
            _userService = userService;
            _doctorMasterService = doctorMasterService;
        }

        [HttpPost]
        [Route("CardData")]
        public async Task<IActionResult> CardData([FromBody] DashboardQueryModel model)
        {
            DateTime? fromDate = string.IsNullOrEmpty(model.fromDate) ? null : DateTime.Parse(model.fromDate);
            DateTime? toDate = string.IsNullOrEmpty(model.toDate) ? null : DateTime.Parse(model.toDate);
            var data = _appointmentService.GetDashboardCardCount(patientId: model.patientId, doctorId: model.doctorId, clinicId: model.clinicId, fromDate: fromDate, toDate: toDate);
            int _appointmentCount = data.Where(a=>a.VisitType.ToLower() != "procedure").Count();
            int _procedureCount = data.Where(a => a.VisitType.ToLower() == "procedure").Count();
            return Ok(new { Appointments = _appointmentCount, Procedures = _procedureCount  });
        }

        [HttpGet]
        [Route("InitialLoad/{id}")]
        public IActionResult InitialLoad(long id)
        {
            var patientsquery = _patientService.GetAll();
            patientsquery = patientsquery?.Include(a => a.DoctorMaster).OrderByDescending(a => a.PatientName);
            var patients = patientsquery.ToList()?.Select(a => a.ToModel(new Web.Model.Patient.PatientModel())).ToList();

            var doctorsquery = _userService.GetAll(id).OrderByDescending(a => a.Name);
            var doctors = doctorsquery.Select(a => a.ToModel()).ToList();

            var facilityQuery = _doctorMasterService.GetAll();
            facilityQuery = facilityQuery?.OrderByDescending(a => a.ClinicName);
            var facilities = facilityQuery?.ToList().Select(a => a.ToModel(new Web.Model.DoctorMaster.DoctorMasterModel())).ToList();

            return Ok(new { patients, doctors, facilities });
        }

        [HttpPost]
        [Route("GetAllAppointment")]
        public IActionResult GetAllAppointment([FromBody] DashboardQueryModel model)
            {
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
            return Ok(new { result, total });
        }
    }
}
