using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PiHealth.Model.Report;
using PiHealth.Services;
using PiHealth.Services.Master;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PiHealth.Controllers.API.Masters
{
    [Authorize]
    [Route("Api/[controller]")]
    [Produces("application/json")]
    public class ReportController : BaseApiController
    {
        public readonly ReportService _reportService;
        public readonly AuditLogServices _auditLogServices;

        public ReportController(ReportService reportService,AuditLogServices auditLogServices)
        {
            _reportService = reportService;
            _auditLogServices = auditLogServices;
        }

        [HttpGet]
        [Route("GetGenderRatio")]
        public async Task<IActionResult> GetGenderRatio([FromQuery] ReportQueryModel model)
        {
            model.doctorId = ActiveUser.Id;
            var result = _reportService.GetGenderRatio(doctorId: model.doctorId,
                fromDate: model.fromDate, toDate: model.toDate);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetDiseaseCategories")]
        public async Task<ActionResult> GetDiseaseCategories([FromQuery] ReportQueryModel model)
        {
            model.doctorId = ActiveUser.Id;
            var result = _reportService.GetDiseaseCategories(doctorId: model.doctorId,
                fromDate: model.fromDate, toDate: model.toDate);
            return Ok(result);
        }
        [HttpGet]
        [Route("GetImpressionData")]
        public IActionResult GetImpressionData([FromQuery] ReportQueryModel model)
        {
            model.doctorId = ActiveUser.Id;
            var entities = _reportService.GetImpressionData(name: model.name, doctorId: model.doctorId,
                fromDate: model.fromDate, toDate: model.toDate);
            var total = entities.Count;
            entities = entities.OrderBy(a => a.impressionName).ToList();
             
            var result = entities;
            return Ok(new { result, total });
        }

        [HttpGet]
        [Route("GetDoctorReferrals")]
        public async Task<ActionResult> GetDoctorReferrals([FromQuery] ReportQueryModel model)
        {
            model.doctorId = ActiveUser.Id;
            var result = _reportService.GetDoctorReferrals(doctorId: model.doctorId,
                fromDate: model.fromDate, toDate: model.toDate);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetNewAndOldPatients")]
        public async Task<ActionResult> GetNewAndOldPatients([FromQuery] ReportQueryModel model)
        {
            model.doctorId = ActiveUser.Id;
            var result = _reportService.GetNewAndOldPatients(doctorId: model.doctorId,
                fromDate: model.fromDate, toDate: model.toDate);
            return Ok(result);
        }

        [HttpGet]
        [Route("GetFeesData")]
        public IActionResult GetFeesData([FromQuery] ReportQueryModel model)
        {
            model.doctorId = ActiveUser.Id;
            var entities = _reportService.GetFeesData(name: model.name,doctorId: model.doctorId,
                fromDate:model.fromDate,toDate:model.toDate);
            var total = entities.Count;
            entities = entities.OrderBy(a => a.date).Skip(model.skip).ToList();
            if (model.take > 0)
            {
                entities = entities.Take(model.take).ToList();
            }
            var result = entities;
            return Ok(new { result, total });
        }
        [HttpGet]
        [Route("GetAgeRangeReport")]
        public async Task<ActionResult> GetAgeRangeReport([FromQuery] ReportQueryModel model)
        {
            model.doctorId = ActiveUser.Id;
            var result = _reportService.GetAgeRangeReport(doctorId: model.doctorId,
                fromDate: model.fromDate, toDate: model.toDate);
            return Ok(result);
        }

    }
}
