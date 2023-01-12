using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PiHealth.Service.UserAccounts;
using PiHealth.Web.MappingExtention;
using System.Linq;
using System.Threading.Tasks;
 

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
 
        private readonly IAppUserService _userService;
        public MessageController(IAppUserService usersService)
        {
            _userService = usersService;
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
