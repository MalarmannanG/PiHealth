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

namespace PiHealth.Web.Controllers.API
{
    [Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class AppointmentController : BaseApiController
    {
        private readonly AppointmentService _appointmentService;
        private readonly IAppUserService _appUserService;
        private readonly AuditLogServices _auditLogService;
        private readonly PatientService _patientService;

        public AppointmentController(
            PatientService patientService,
            AppointmentService appointmentService,
            IAppUserService appUserService,
            AuditLogServices auditLogServices)
        {
            _appointmentService = appointmentService;
            _appUserService = appUserService;
            _auditLogService = auditLogServices;
            _patientService = patientService;
        }

        #region  Appointment Master

        [HttpGet]
        [Route("Dashboard")]
        public async Task<IActionResult> Dasboard(long id)
        {
            DateTime date = DateTime.Now.Date;
            TimeSpan frtime = new TimeSpan(0, 0, 0, 0);
            DateTime fromDate = date.Add(frtime);
            TimeSpan totime = new TimeSpan(0, 23, 59, 59);
            DateTime toDate = date.Add(totime);
            var todaysPatients = _appointmentService.GetAll(fromDate: fromDate, toDate: toDate).Distinct().Count();
            var todaysProcedures = _appointmentService.GetAll(fromDate: fromDate, toDate: toDate, isProcedure: true).Count();
            var todaysAppointments = _appointmentService.GetAll(fromDate: fromDate, toDate: toDate, isProcedure: false).Count();
            return Ok(new { todaysPatients, todaysAppointments, todaysProcedures });
        }

        [HttpGet]
        [Route("Get/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var appointment = await _appointmentService.Get(id);
            var result = appointment.ToModel(new AppointmentModel());
            return Ok(result);
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll([FromQuery] AppointmentQueryModel model)
        {
            var patients1 = new List<long>();
            var patients2 = new List<long>();
            long[] patientIds = null;
            long[] doctorIds = null;

            if (model.todayPatients)
            {
                patients1 = _patientService.GetAll(isTodayPatients: model.todayPatients).Select(a => a.Id).ToList();
                if (patients1?.Count() == 0)
                {
                    return Ok(new { result = new List<AppointmentModel>(), total = 0 });
                }
            }

            //if (!string.IsNullOrEmpty(model.patientName))
            //{
                patients2 = _patientService.GetAll(name: model.patientName, clinicName:model.clinicName).Select(a => a.Id).ToList();
            //}

            if (!string.IsNullOrEmpty(model.doctorName))
            {
                doctorIds = _appUserService.GetIdByDoctorName(name: model.doctorName).Select(a => a.Id).ToArray();
            }

            if (patients1 != null  && patients2 != null)
            {
                patients1.AddRange(patients2);
                patientIds = patients1.ToArray();
            }
            else if (patients1 != null)
            {
                patientIds = patients1.ToArray();
            }
            else if (patients2 != null)
            {
                patientIds = patients2.ToArray();
            }
            DateTime? fromDate = string.IsNullOrEmpty(model.fromDate) ? null : DateTime.Parse(model.fromDate);
            DateTime? toDate = string.IsNullOrEmpty(model.toDate) ? null : DateTime.Parse(model.toDate);
            var appointments = _appointmentService.GetAll(patientIds: patientIds, doctorIds: doctorIds, isProcedure: model?.isProcedure, fromDate: fromDate, toDate: toDate);
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

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromBody] AppointmentModel model)
        {
            var appointment = model.ToEntity(new Appointment());
            appointment.CreatedDate = DateTime.Now;
            appointment.CreatedBy = ActiveUser.Id;
            appointment.IsActive = true;
            var FormattedDate = DateTime.ParseExact(model.appointmentISOString, "ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            appointment.AppointmentDateTime = FormattedDate;

            var appts = _appointmentService.AppointmentAlreadyExist(model.patientId.Value, model.consultingDoctorID.Value, FormattedDate,0);
            if (appts.Count() < 1)
            {
                await _appointmentService.Create(appointment);
                _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");
            }
            else
            {
                appointment = appts.First();
                model.appointmentDateTime = appointment.AppointmentDateTime;
                model.id = -1;
            }
            return Ok(model);
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update([FromBody] AppointmentModel model)
        {
            if (model == null)
                return BadRequest();

            var appointment = await _appointmentService.Get(model.id);

            if (appointment == null)
                return BadRequest();
            var updated = model.ToEntity(appointment);
            var FormattedDate = DateTime.ParseExact(model.appointmentISOString, "ddd MMM dd yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            appointment.AppointmentDateTime = FormattedDate;
            var appts = _appointmentService.AppointmentAlreadyExist(model.patientId.Value, model.consultingDoctorID.Value, FormattedDate, model.id);
            if (appts.Count() < 1)
            {
                await _appointmentService.Update(appointment);
                _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");
            }
            else
            {
                appointment = appts.First();
                model.appointmentDateTime = appointment.AppointmentDateTime;
                model.id = -1;
            }
            return Ok(model);

        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var appointment = await _appointmentService.Get(id);

            if (appointment == null)
                return BadRequest();

            appointment.IsActive = false;
            appointment.UpdatedDate = DateTime.Now;
            appointment.UpdatedBy = ActiveUser.Id;

            await _appointmentService.Update(appointment);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: ActiveUser.Id, value1: "Success");

            return Ok();
        }

        #endregion Appointment Ends
    }
}