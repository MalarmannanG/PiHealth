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
    [Route("api/[controller]")]
    [ApiController]
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


        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> IntialDataLoad(long doctorId)
        {
            var patientsquery = _patientService.GetAll();
            patientsquery = patientsquery?.Include(a => a.DoctorMaster).OrderByDescending(a => a.PatientName);
            var patients = patientsquery.ToList()?.Select(a => a.ToModel(new Web.Model.Patient.PatientModel())).ToList();

            var doctorsquery = _userService.GetAll(-1).OrderByDescending(a => a.Name);
            var doctors = doctorsquery.Select(a => a.ToModel()).ToList();

            var facilityQuery = _doctorMasterService.GetAll();
            facilityQuery = facilityQuery?.OrderByDescending(a => a.ClinicName);
            var facilities = facilityQuery?.ToList().Select(a => a.ToModel(new Web.Model.DoctorMaster.DoctorMasterModel())).ToList();

            return Ok(new { patients, doctors, facilities });
        }

        [HttpGet]
        [Route("GetAllAppointment")]
        public IActionResult GetAllAppointment([FromQuery] DashboardQueryModel model)
        {
            DateTime? fromDate = string.IsNullOrEmpty(model.fromDate) ? null : DateTime.Parse(model.fromDate);
            DateTime? toDate = string.IsNullOrEmpty(model.toDate) ? null : DateTime.Parse(model.toDate);

            var appointments = _appointmentService.GetDashboardAll(patientId: model.patientId, doctorId: model.doctorId, clinicId: model.clinicId, fromDate: fromDate, toDate: toDate, isProcedure: model?.isProcedure);
            var total = appointments?.Count();
            var orderBy = string.IsNullOrEmpty(model?.order_by) ? "AppointmentDateTime" : model.order_by;
            appointments = appointments?.OrderByDescending(a => a.AppointmentDateTime).Skip(model.skip);

            if (model.take > 0)
            {
                appointments = appointments.Take(model.take);
            }

            var result = appointments.ToList().Select(a => a.ToModel(new AppointmentModel())).OrderByDescending(a => a.appointmentDateTime).ToList() ?? new List<AppointmentModel>();
            return Ok(new { result, total });
        }
    }
}
