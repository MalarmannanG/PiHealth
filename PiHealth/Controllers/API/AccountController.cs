using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PiHealth.Service.UserAccounts;
using PiHealth.Services.UserAccounts;
using PiHealth.Web.Model;
using PiHealth.Controllers;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using PiHealth.Web.Model.Account;
using PiHealth.Services;
using PiHealth.Services.AccessServices;
using PiHealth.Web.Model.AccessRights;
using Microsoft.EntityFrameworkCore;
using PiHealth.DataModel.Options;
using PiHealth.Web.Helper;
using PiHealth.Web.Filter;
using PiHealth.Web.Model.Patient;
using PiHealth.DataModel.Entity;
using System;
using PiHealth.Web.MappingExtention;
using PiHealth.Services.PiHealthPatients;
using PiHealth.Web.Model.Prefix;
using Microsoft.Extensions.Options;
using Abp.Json;
using StackExchange.Profiling.Internal;
using Newtonsoft.Json;
using System.Net.Http;

namespace PiHealth.Web.Controllers
{

    [Route("Api/[Controller]")]
    [Produces("application/json")]
    public class AccountController : BaseApiController
    {
        private readonly IAppUserService _usersService;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly SecurityService _securityService;
        private readonly AuditLogServices _auditLogService;
        private readonly AccessFunctionService _accessFunctionService;
        private readonly AccessModuleService _accessModuleService;
        private readonly AccessRoleFunctionService _accessRoleService;
        private readonly IOptionsSnapshot<PrefixOption> _prefixOption;
        private readonly PatientService _patientService;

        public AccountController(
            AccessFunctionService accessFunctionService,
            AccessModuleService accessModuleService,
            AccessRoleFunctionService accessRoleService,
            IAppUserService usersService,
            ITokenStoreService tokenStoreService,
            SecurityService securityService,
            AuditLogServices auditLogServices,
            IOptionsSnapshot<PrefixOption> prefixOption,
            PatientService patientService)

        {
            _accessFunctionService = accessFunctionService;
            _accessModuleService = accessModuleService;
            _accessRoleService = accessRoleService;
            _usersService = usersService;
            _usersService.CheckArgumentIsNull(nameof(usersService));
            _securityService = securityService;
            _auditLogService = auditLogServices;
            _tokenStoreService = tokenStoreService;
            _tokenStoreService.CheckArgumentIsNull(nameof(_tokenStoreService));
            _prefixOption = prefixOption;
            _patientService = patientService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Signup")]
        [CustomExceptionFilter]
        public async Task<IActionResult> Signup([FromBody] SignupModel signupModel)
        {
            UserModel userModel = new UserModel();
            userModel.userName = signupModel.name;
            userModel.name = signupModel.name;
            userModel.email = signupModel.email;
            userModel.phoneNo = signupModel.mobileNumber;
            userModel.userType = "Patient";
            userModel.gender = signupModel.gender;
            userModel.address = signupModel.address;
            PatientModel patientModel = new PatientModel();
            patientModel.patientName = signupModel.name;
            patientModel.initial = signupModel.initial;
            patientModel.gender = signupModel.gender;
            patientModel.mobileNumber = signupModel.mobileNumber;
            patientModel.age = signupModel.age;
            patientModel.address = signupModel.address;
            if (userModel == null || patientModel == null)
            {
                return BadRequest();
            }

            var emailExist = _usersService.EmailAlreadyExit(signupModel.email);
            var _templates = await _patientService.GetByName(patientModel.patientName, 0, patientModel.mobileNumber);

            if (!emailExist && _templates == null)
            {
                //inserting into AppUser table
                var user = userModel.ToEntity(new AppUser());
                user.SerialNumber = new Guid().ToString();
                user.CreatedDate = DateTime.UtcNow;
                user.Password = _securityService.GetSha256Hash(signupModel.password);
                if (userModel.specializationId.HasValue)
                {
                    user.SpecializationId = userModel.specializationId.Value;
                }
                user.RegistrationNo = userModel.registrationNo;
                user.IsActive = true;
                user = await _usersService.Create(user);
                userModel = user.ToModel(new UserModel());

                //inserting into Patient Table
                var ulIdPrefix = (_prefixOption?.Value?.ULIDPrefix ?? "");
                var entity = patientModel.ToEntity(new Patient());
                entity.HrNo = _patientService.NewULID();
                entity.HrNo = ulIdPrefix + entity.HrNo;
                entity.CreatedBy = 0;
                entity.CreatedDate = DateTime.Now;
                var patient = _patientService.Create(entity);
                patientModel = patient?.ToModel(new PatientModel());
            }
            else if (emailExist)
            {
                return Ok(-1);
            }
            else if(_templates != null)
            {
                return Ok(-2);
            }

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, RequestIP: RequestIP, userid: 0, value1: "Success", value2: "Create");
            return Ok(new { userModel = userModel, patientModel = patientModel });
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        [CustomExceptionFilter]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginUser)
        {

            if (loginUser == null)
            {
                return BadRequest();
            }

            var user = await _usersService.FindUserAsync(loginUser.Email.Trim(), loginUser.Password).ConfigureAwait(false);

            if (!user.IsActive)
            {
                _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: user.Id, RequestIP: RequestIP, value1: $"Email-{loginUser.Email} Is Inactive", value2: "Failed");
                return BadRequest();
            }

            if (user == null)
            {
                _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: user.Id, RequestIP: RequestIP, value1: $"Email-{loginUser.Email}", value2: "Failed");
                return BadRequest();
            }

            var (accessToken, refreshToken) = await _tokenStoreService.CreateJwtTokens(user).ConfigureAwait(false);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: user.Id, RequestIP: RequestIP, value1: $"Email-{loginUser.Email}", value2: "Success");

            return Ok(new { token = accessToken, refresh_token = refreshToken, role = user.UserType, username = user.Name, id = user.Id, name = user.Name, registrationNo = user.RegistrationNo });
        }


        [AllowAnonymous]
        [HttpPost]
        [Route("PatientLogin")]
        [CustomExceptionFilter]
        public async Task<IActionResult> PatientLogin([FromBody]PatientSignInModel model)
        {
            if(model == null)
            {
                return BadRequest();
            }

            var userPatient = await _usersService.GetUserPatientByMobileNumber(model.mobileNumber);

            if (userPatient == null)
            {
                return Ok(-1);
            }

            if(userPatient.Otp != model.otp)
            {
                _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: userPatient.Id, RequestIP: RequestIP, value1: $"OTP-{userPatient.Otp} Not Matched", value2: "Failed");
                return Ok(-2);
            }

            var (accessToken, refreshToken) = await _tokenStoreService.CreateJwtTokens(userPatient).ConfigureAwait(false);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: userPatient.Id, RequestIP: RequestIP, value1: $"Phone No-{userPatient.PhoneNo}", value2: "Success");

            return Ok(new { token = accessToken, refresh_token = refreshToken, role = userPatient.UserType, username = userPatient.Name, id = userPatient.Id, name = userPatient.Name, registrationNo = userPatient.RegistrationNo });
        }


        //[AllowAnonymous]
        //[HttpPost]
        //[Route("RefreshToken")]
        //public async Task<IActionResult> RefreshToken([FromBody] JToken jsonBody)
        //{
        //    var refreshToken = jsonBody.Value<string>("refreshToken");
        //    if (string.IsNullOrWhiteSpace(refreshToken))
        //    {
        //        return BadRequest("refreshToken is not set.");
        //    }

        //    var token = await _tokenStoreService.FindTokenAsync(refreshToken);
        //    if (token == null)
        //    {
        //        return Unauthorized();
        //    }

        //    var (accessToken, newRefreshToken) = await _tokenStoreService.CreateJwtTokens(token.AppUser).ConfigureAwait(false);
        //    _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: ActiveUser.Id, RequestIP: RequestIP, value1: "Success", value2: "Token Refreshed for the user " + token.AppUser.Name + ". New Token: " + refreshToken);
        //    return Ok(new { access_token = accessToken, refresh_token = newRefreshToken, username = token.AppUser.Name });
        //}

        [AllowAnonymous]
        [HttpGet]
        [Route("GetAccess/{roleName}")]
        public async Task<AccessRightsModel> GetAccess(string roleName)
        {

            var moduleFunctions = await _accessFunctionService.GetAll().Include(a => a.AccessModule).AsQueryable().ToListAsync();

            var data = new AccessRightsModel();

            data.roles = typeof(UserType).ToSelectListItems().Select(a => new RoleDetails()
            {
                name = a.Text,
                value = a.Value

            }).ToList();

            data.role = string.IsNullOrEmpty(roleName) ? UserType.Admin.ToString() : roleName;

            var functions = _accessRoleService.GetAll(role: roleName).Select(a => a.FunctionID).ToList();

            foreach (var each in moduleFunctions.GroupBy(a => a.ModuleID))
            {
                var menus = new ModuleDetails();

                menus.code = each.FirstOrDefault().AccessModule.ModuleCode;

                menus.id = each.Key;

                menus.name = each.FirstOrDefault().AccessModule.Name;

                menus.functions = each.Select(a => new FunctionDetails()
                {
                    id = a.Id,
                    code = a.FuctionCode,
                    haveAccess = functions.Contains(a.Id),
                    name = a.Description
                }).ToList();

                data.modules.Add(menus);
            }

            return data;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("Logout")]
        public async Task<bool> Logout()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;

            var userIdValue = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // The Jwt implementation does not support "revoke OAuth token" (logout) by design.
            // Delete the user's tokens from the database (revoke its bearer token)
            if (!string.IsNullOrWhiteSpace(userIdValue) && int.TryParse(userIdValue, out int userId))
            {
                await _tokenStoreService.InvalidateUserTokensAsync(userId).ConfigureAwait(false);
            }

            await _tokenStoreService.DeleteExpiredTokensAsync().ConfigureAwait(false);

            return true;
        }

        [HttpPost]
        [Route("IsAuthenthenticated")]
        public bool IsAuthenthenticated()
        {
            return User.Identity.IsAuthenticated;
        }

        [HttpGet]
        [Route("GetUserInfo")]
        public IActionResult GetUserInfo()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            return Ok(new { Username = claimsIdentity.Name });
        }

        [Authorize]
        [HttpPost]
        [Route("ChangePassword")]
        public IActionResult ChangePassword([FromBody] ChangePasswordModel changePassword)
        {
            if (changePassword == null)
                return BadRequest();

            var oUser = ActiveUser;
            var hashed = _securityService.GetSha256Hash(changePassword.oldPassword);
            var oldPassworWrong = hashed != oUser.Password;

            if (oldPassworWrong)
            {
                return Ok(new { status = false });
            }

            oUser.Password = _securityService.GetSha256Hash(changePassword.newPassword);

            _usersService.Update(oUser);

            _auditLogService.InsertLog(ControllerName: ControllerName, ActionName: ActionName, UserAgent: UserAgent, userid: ActiveUser.Id, RequestIP: RequestIP, value1: "Success", value2: "Password changed from " + changePassword.oldPassword + " to " + changePassword.newPassword);

            return Ok(new { status = true });

        }

        [AllowAnonymous]
        [HttpPost]
        [Route("ForgotPassword")]
        [CustomExceptionFilter]
        public async Task<IActionResult> ForgotPassword([FromBody] LoginRequestModel forgotPasswordUser)
        {
            if(forgotPasswordUser == null)
            {
                return BadRequest();
            }

            var user = await _usersService.GetUserByEmail(forgotPasswordUser.Email);

            if(user == null)
            {
                return Ok(-1);
            }
            else
            {
                user.Password = _securityService.GetSha256Hash(forgotPasswordUser.Password);
                await _usersService.Update(user);
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("SendOTP")]
        [CustomExceptionFilter]
        public async Task<IActionResult> SendOTP([FromBody] PatientSignInModel patientSignInModel)
        {
            if (patientSignInModel == null)
            {
                return BadRequest();
            }

            var userPatient = await _usersService.GetUserPatientByMobileNumber(patientSignInModel.mobileNumber);

            if (userPatient == null)
            {
                return Ok(-1);
            }

            userPatient.Otp = _usersService.GenerateOTP();
            //var smsResponse = _usersService.SendOTP(userPatient.PhoneNo, userPatient.Otp);
            //var result = JsonConvert.DeserializeObject(smsResponse);
            //var result = _usersService.SendOTP(userPatient.PhoneNo, userPatient.Otp);
            var url = "http://online.chennaisms.com/api/mt/SendSMS?user=bulksms6&password=Bulksms@9&senderid=NKLGSM&channel=Trans&DCS=0&flashsms=0&number=91"+ userPatient.PhoneNo+"&text=Hello%2C%20Your%20OTP%20for%20Ganga%20Supermarket%20account%20is%20"+ userPatient.Otp + "%20Regards%2C%20GSM%20%28Ganga%20Supermarket%29&route=6";
            var client = new HttpClient();
            var result = await client.GetAsync(url);
            if (result.IsSuccessStatusCode)
            {
                userPatient.OtpSentDateTime = DateTime.Now;
                await _usersService.Update(userPatient);
            }
            //return Ok(new {userPatient, smsResponse });
            return Ok(new {userPatient,result});
            //return Ok(new { result });
        }
    }
}
