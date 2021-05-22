using CloudPic.BAL.Interfaces;
using CloudPic.BAL.Logger;
using CloudPic.DAL.Interfaces;
using CloudPic.Models.Builders;
using CloudPic.Models.DTO;
using CloudPic.Models.Enums;
using CloudPic.Models.VM;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CloudPic.BAL.Logger.Logger;

namespace CloudPic.Web.Controllers
{
    [Log]
    [Route("account")]
    public class AccountController : _BaseController
    {
        #region Fields

        private readonly string ERROR_MESSAGE = "Error while sending a request to account service.";

        private readonly IUserRepo _userRepo;
        private readonly IPlanRepo _planRepo;
        private readonly ILogService _logService;
        private readonly BAL.Logger.Logger _logger;

        #endregion

        #region Constructors

        public AccountController(IUserRepo userRepo,
                                 IPlanRepo planRepo,
                                 ILogService logService)
        {
            _userRepo = userRepo;
            _planRepo = planRepo;
            _logService = logService;

            _logger = new ConsoleLogger(LogSeverity.Info,
                            new ConsoleLogger(LogSeverity.Warning,
                            new ConsoleLogger(LogSeverity.Error,
                            new FileLogger(LogSeverity.Error))));
        }


        #endregion

        #region Endpoints

        [Route("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (!IsUserLogged)
                    return RedirectToAction(nameof(Login));

                var lastPackage = await _planRepo.GetLastPackage(UserId);
                var plan = await _planRepo.GetPlan(lastPackage.PlanId);
                ViewBag.CurrentPlan = plan;
                ViewBag.CurrentPackage = lastPackage;
                ViewBag.DisableChange = DateTime.Today.AddDays(1).Date == lastPackage.DateFrom.Date;

                var weekConsumation = new List<ConsumationOnDateVM>();
                for (int i = 0; i < 7; i++)
                    weekConsumation.Add(await _planRepo.GetConsumeOnDate(UserId, DateTime.Today.AddDays(-i)));

                var storageUsed = (int)((double)weekConsumation.First().Storage / plan.StorageSize * 100);
                ViewBag.StorageUsed = storageUsed <= 100 ? storageUsed : 100;

                ViewBag.WeekConsumation = weekConsumation;
                return View();
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }


        #endregion

        #region Package

        [Route("package")]
        public async Task<IActionResult> ChoosePackage()
        {
            try
            {
                if (!IsUserLogged)
                    return RedirectToAction(nameof(Login));

                var lastPackage = await _planRepo.GetLastPackage(UserId);
                if (DateTime.Today.AddDays(1).Date == lastPackage.DateFrom.Date)
                    return RedirectToAction(nameof(Login));

                ViewBag.LastPackage = lastPackage;

                return View();
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }


        [HttpPost]
        [Route("package")]
        public async Task<IActionResult> ChoosePackage(int packageId)
        {
            try
            {
                if (!IsUserLogged)
                    return RedirectToAction(nameof(Login));

                var lastPackage = await _planRepo.GetLastPackage(UserId);
                if (DateTime.Today.AddDays(1).Date == lastPackage.DateFrom.Date)
                    return RedirectToAction(nameof(Login));

                await _planRepo.ChangePackageAsync(UserId, packageId);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }


        #endregion

        #region Login/Logout

        [HttpGet]
        [Route("login")]
        public IActionResult Login()
        {
            try
            {
                if (IsUserLogged)
                    return RedirectToAction("Explore", "Home");

                return View();
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        public class LoginUserVM
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginUserVM loginUserVM)
        {
            try
            {
                var user = await _userRepo.LoginUserAsync(loginUserVM.Email, loginUserVM.Password);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "Incorrect password. Please try again.";
                    return View();
                }

                HttpContext.Session.SetInt32("UserId", user.Id);

                await _logService.InsertLog(new Log(user.Id, LogType.LoggedIn, user.Id));
                TempData["SuccessMessage"] = $"Login successful. Welcome {user.Name}!";
                return RedirectToAction("Explore", "Home");
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        [HttpGet]
        [Route("oauth/github")]
        public IActionResult GithubLogin()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = Url.Action(nameof(OAuthGithubResponse)) }, "Github");
        }

        [HttpGet]
        [Route("oauth/google")]
        public IActionResult GoogleLogin()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = Url.Action(nameof(OAuthGoogleResponse)) }, "Google");
        }

        [HttpGet]
        [Route("oauth/github-response")]
        public async Task<IActionResult> OAuthGithubResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            (string email, string name) = GetDataFromOAuthResult(result);

            var _user = await _userRepo.GetUserAsync(email);
            if (_user == null)
            {
                var user = new UserBuilder()
                    .SetLoginProvider(LoginProvider.Github)
                    .SetEmail(email)
                    .SetName(name)
                    .Build();

                var userId = int.Parse((await _userRepo.InsertUserAsync(user)).ToString());

                var userPlan = new UserPlan(userId, 1, DateTime.Today, null);
                _ = await _planRepo.InsertUserPlanAsync(userPlan);

                HttpContext.Session.SetInt32("UserId", userId);

                await _logService.InsertLog(new Log(userId, LogType.LoggedIn, userId));
                TempData["SuccessMessage"] = $"Registration successful. Welcome {name}!";
                return RedirectToAction("Explore", "Home");
            }
            else
            {
                HttpContext.Session.SetInt32("UserId", _user.Id);
                await _logService.InsertLog(new Log(_user.Id, LogType.LoggedIn, _user.Id));
                TempData["SuccessMessage"] = $"Login successful. Welcome {name}!";
                return RedirectToAction("Explore", "Home");
            }
        }

        [HttpGet]
        [Route("oauth/google-response")]
        public async Task<IActionResult> OAuthGoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            (string email, string name) = GetDataFromOAuthResult(result);

            var _user = await _userRepo.GetUserAsync(email);
            if (_user == null)
            {
                var user = new UserBuilder()
                    .SetLoginProvider(LoginProvider.Google)
                    .SetEmail(email)
                    .SetName(name)
                    .Build();

                var userId = int.Parse((await _userRepo.InsertUserAsync(user)).ToString());

                var userPlan = new UserPlan(userId, 1, DateTime.Today, null);
                _ = await _planRepo.InsertUserPlanAsync(userPlan);

                HttpContext.Session.SetInt32("UserId", userId);

                await _logService.InsertLog(new Log(userId, LogType.LoggedIn, userId));
                TempData["SuccessMessage"] = $"Registration successful. Welcome {name}!";
                return RedirectToAction("Explore", "Home");
            }
            else
            {
                HttpContext.Session.SetInt32("UserId", _user.Id);
                HttpContext.Session.SetInt32("Admin", _user.Email != "brunoubica@gmail.com" ? 0 : 1);

                await _logService.InsertLog(new Log(_user.Id, LogType.LoggedIn, _user.Id));
                TempData["SuccessMessage"] = $"Login successful. Welcome {name}!";
                return RedirectToAction("Explore", "Home");
            }
        }


        [Route("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (!IsUserLogged)
                    return RedirectToAction(nameof(Login));

                var userId = HttpContext.Session.GetInt32("UserId");
                HttpContext.Session.Clear();

                if (userId.HasValue)
                {
                    await _logService.InsertLog(new Log(userId.Value, LogType.LoggedOut, userId.Value));
                    TempData["SuccessMessage"] = $"You have been logged out";
                }
                
                return RedirectToAction(nameof(Login));
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        #endregion

        #region Registration

        [HttpGet]
        [Route("register")]
        public IActionResult Register()
        {
            if (IsUserLogged)
                return RedirectToAction("Explore", "Home");

            return View();
        }

        public class RegisterUserVM
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Package { get; set; }
            public string FullName { get; set; }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(RegisterUserVM registerUserVM)
        {
            try
            {
                var user = new UserBuilder()
                .SetLoginProvider(LoginProvider.Local)
                .SetEmail(registerUserVM.Email)
                .SetPassword(registerUserVM.Password)
                .SetName(registerUserVM.FullName)
                .Build();

                var userId = int.Parse((await _userRepo.InsertUserAsync(user)).ToString());

                var userPlan = new UserPlan(userId, Plan.Parse(registerUserVM.Package), DateTime.Today, null);
                _ = await _planRepo.InsertUserPlanAsync(userPlan);

                HttpContext.Session.SetInt32("UserId", userId);
                HttpContext.Session.SetInt32("Rola", 2);

                await _logService.InsertLog(new Log(userId, LogType.LoggedIn, userId));
                TempData["SuccessMessage"] = $"Registration successful. Welcome {user.Name}!";
                return RedirectToAction("Explore", "Home");
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        #endregion

        #region Helpers
        private static (string email, string name) GetDataFromOAuthResult(AuthenticateResult result)
        {
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim =>
                new { claim.Issuer, claim.OriginalIssuer, claim.Type, claim.Value });

            var email = claims.Where(c => c.Type.Contains("email")).FirstOrDefault().Value;
            var name = claims.Where(c => c.Type.EndsWith("name")).FirstOrDefault().Value;

            return (email, name);
        }

        #endregion

    }
}
