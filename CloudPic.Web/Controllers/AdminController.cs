using CloudPic.BAL.Interfaces;
using CloudPic.BAL.Logger;
using CloudPic.BAL.Patterns.Factory;
using CloudPic.DAL.Interfaces;
using CloudPic.Models.DTO;
using CloudPic.Models.Enums;
using CloudPic.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    [Route("admin")]
    public class AdminController : _BaseController
    {
        private readonly string ERROR_MESSAGE = "Error while sending a request to admin service.";

        private readonly IUserRepo _userRepo;
        private readonly IPlanRepo _planRepo;
        private readonly IPhotoRepo _photoRepo;
        private readonly IFSRepo _fsRepo;
        private readonly IUserService _userService;
        private readonly IPhotoService _photoService;
        private readonly ILogService _logService;
        private readonly BAL.Logger.Logger _logger;
        private readonly ILogger<ILogger> _nLogger;

        public AdminController(
            IConfiguration configuration,
            IUserRepo userRepo,
            IPlanRepo planRepo,
            IUserService userService,
            IPhotoService photoService,
            ILogService logService,
            IPhotoRepo photoRepo, 
            ILogger<ILogger> nLogger)
        {
            _userRepo = userRepo;
            _planRepo = planRepo;
            _fsRepo = this._fsRepo = FileShareFactory.GetFSRepo(FSType.AzureFS, configuration);
            _userService = userService;
            _photoService = photoService;
            _logService = logService;
            _photoRepo = photoRepo;
            _nLogger = nLogger;

            this._logger = new ConsoleLogger(LogSeverity.Info,
                new ConsoleLogger(LogSeverity.Warning,
                new ConsoleLogger(LogSeverity.Error,
                new FileLogger(LogSeverity.Error))));
            _nLogger = nLogger;
        }

        [Route("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                if (!IsUserLogged || !IsUserAdmin)
                    return RedirectToAction("Login", "Account");

                ViewBag.AllUsers = (await _userRepo.GetAllUsers())
                    .OrderByDescending(x => x.RegisterDate);

                ViewBag.AllLogs = (await _logService.GetAllLogs())
                    .OrderByDescending(x => x.LogDate);


                return View();
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        [Route("user/delete")]
        [HttpGet]
        public async Task<IActionResult> DeleteUser([FromQuery] int id)
        {
            try
            {
                if (!IsUserLogged || !IsUserAdmin)
                    return RedirectToAction("Login", "Account");

                await _userService.DeleteUser(id);

                await _logService.InsertLog(new Log(UserId, LogType.DeletedUser, id));
                TempData["SuccessMessage"] = "User has been deleted.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        [Route("user/details")]
        [HttpGet]
        public async Task<IActionResult> AccountOverview([FromQuery] int id)
        {
            try
            {
                if (!IsUserLogged || !IsUserAdmin)
                    return RedirectToAction("Login", "Account");

                _nLogger.LogError("INDEX HIT" + Environment.NewLine);

                var user = await _userRepo.GetUserAsync(id);
                ViewBag.User = user;

                var lastPackage = await _planRepo.GetLastPackage(id);
                var plan = await _planRepo.GetPlan(lastPackage.PlanId);
                ViewBag.CurrentPlan = plan;
                ViewBag.CurrentPackage = lastPackage;
                ViewBag.DisableChange = DateTime.Today.AddDays(1).Date == lastPackage.DateFrom.Date;

                var weekConsumation = new List<ConsumationOnDateVM>();
                for (int i = 0; i < 3; i++)
                    weekConsumation.Add(await _planRepo.GetConsumeOnDate(id, DateTime.Today.AddDays(-i)));

                var storageUsed = (int)((double)weekConsumation.First().Storage / plan.StorageSize * 100);
                ViewBag.StorageUsed = storageUsed <= 100 ? storageUsed : 100;

                ViewBag.WeekConsumation = weekConsumation;

                var photosWithHashtags = await _photoService.GetPhotosDetailedForUser(id);
                return View(photosWithHashtags);
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        [HttpPost]
        [Route("photo/update")]
        public async Task<IActionResult> UpdatePhoto(PhotoUpdateVM photoUpdateVM)
        {
            try
            {
                if (!IsUserLogged || !IsUserAdmin)
                    return RedirectToAction("Login", "Account");

                var photo = await _photoService.GetPhoto(photoUpdateVM.Id);
                await _photoService.UpdatePhoto(photoUpdateVM);

                await _logService.InsertLog(new Log(UserId, LogType.EditedPhoto, photoUpdateVM.Id));
                TempData["SuccessMessage"] = "Photo has been updated.";
                return RedirectToAction(nameof(AccountOverview), new { id = photo.UserId });
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            try
            {
                if (!IsUserLogged || !IsUserAdmin)
                    return RedirectToAction("Login", "Account");

                var photo = await _photoService.GetPhoto(id);
                await _photoService.DeletePhoto(id);

                await _logService.InsertLog(new Log(UserId, LogType.DeletedPhoto, id));
                TempData["SuccessMessage"] = "Photo has been deleted.";
                return RedirectToAction(nameof(AccountOverview), new { id = photo.UserId });
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }


        [HttpGet]
        [Route("user/package")]
        public async Task<IActionResult> ChangePackage([FromQuery] int id)
        {
            try
            {
                if (!IsUserLogged || !IsUserAdmin)
                    return RedirectToAction("Login", "Account");

                ViewBag.User = await _userRepo.GetUserAsync(id);
                var lastPackage = await _planRepo.GetLastPackage(id);
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
        [Route("user/package")]
        public async Task<IActionResult> ChangePackage(int userId, int packageId)
        {
            try
            {
                if (!IsUserLogged || !IsUserAdmin)
                    return RedirectToAction("Login", "Account");

                await _planRepo.ChangePackageAsync(userId, packageId);
                await _logService.InsertLog(new Log(UserId, LogType.ChangedPackage, userId));
                TempData["SuccessMessage"] = "Package has been changed.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                this._logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }
    }
}
