using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PiHealth.Service.UserAccounts;
using PiHealth.Web.MappingExtention;
using System.Linq;
using System.Threading.Tasks;
using WhatsappBusiness.CloudApi.Interfaces;
using WhatsappBusiness.CloudApi.Messages.Requests;

namespace PiHealth.Controllers.API
{
    [AllowAnonymous]
    [Route("Api/[Controller]")]
    [Produces("application/json")]
    [ApiController]
    public class MessageController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        const string VerfifyToken = "1234";
        private readonly IWhatsAppBusinessClient _whatsAppBusinessClient;
        private readonly IAppUserService _userService;
        public MessageController(IAppUserService usersService, IWhatsAppBusinessClient whatsAppBusinessClient)
        {
            _whatsAppBusinessClient = whatsAppBusinessClient;
            _userService = usersService;
        }
        [HttpGet("webhook")]
        public ActionResult<string> SetupWebHook([FromQuery(Name = "hub.mode")] string hubMode,
                                                 [FromQuery(Name = "hub.challenge")] int hubChallenge,
                                                 [FromQuery(Name = "hub.verify_token")] string hubVerifyToken)
        {
            log.Error(hubChallenge);
            if (!hubVerifyToken.Equals(VerfifyToken))
            {
                return Forbid("VerifyToken doesn't match");
            }
            log.Error("SetupWebHook");
            return Ok(hubChallenge);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> ReceiveNotification([FromBody] string data)
        {
            TextMessageRequest textMessageRequest = new TextMessageRequest();
            textMessageRequest.To = "919677010957";
            textMessageRequest.Text = new WhatsAppText();
            textMessageRequest.Text.Body = "Message Body";
            textMessageRequest.Text.PreviewUrl = false;

            var results = await _whatsAppBusinessClient.SendTextMessageAsync(textMessageRequest);
            log.Error("ReceiveNotification");
            return Ok();
        }

        [HttpGet]
        [Route("GetAllDoctors")]
        public async Task<ActionResult> getAllDoctors()
        {
            var user = _userService.GetAll(userType:"Doctor");
            var result = user.Select(a => a.ToModel()).ToList();
            return Ok(result);
        }
    }
}
