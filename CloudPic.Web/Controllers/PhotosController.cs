using CloudPic.BAL.Interfaces;
using CloudPic.BAL.Logger;
using CloudPic.Models.DTO;
using CloudPic.Models.Enums;
using CloudPic.Models.VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Threading.Tasks;
using static CloudPic.BAL.Logger.Logger;

namespace CloudPic.Web.Controllers
{
    [Log]
    [Route("photos")]
    public class PhotosController : _BaseController
    {

        #region Fields

        private readonly string ERROR_MESSAGE = "Error while sending a request to photos service.";

        private readonly IPhotoService _photoService;
        private readonly ILogService _logService;
        private readonly ConsoleLogger _logger;

        #endregion

        #region Constructor

        public PhotosController(IPhotoService photoService,
            ILogService logService)
        {
            _photoService = photoService;
            _logService = logService;
            this._logger = new ConsoleLogger(LogSeverity.Info,
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
                    return RedirectToAction("Login", "Account");

                var photosWithHashtags = await _photoService.GetPhotosDetailedForUser(UserId);

                return View(photosWithHashtags);
            }
            catch (Exception e)
            {
                _logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        [Route("upload")]
        public IActionResult UploadPhoto()
        {
            try
            {
                if (!IsUserLogged)
                    return RedirectToAction("Login", "Account");

                return View(new PhotoInsertVM());
            }
            catch (Exception e)
            {
                _logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }


        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto(PhotoInsertVM photoInsertVM)
        {
            try
            {
                (int, int) result = await _photoService.UploadPhoto(photoInsertVM, UserId);

                switch (result.Item1)
                {
                    case -1:
                        TempData["ErrorMessage"] = "You have reached your daily Mb upload limit.";
                        return RedirectToAction(nameof(UploadPhoto));
                    case -2:
                        TempData["ErrorMessage"] = "You have reached your daily photo count limit.";
                        return RedirectToAction(nameof(UploadPhoto));
                    case -3:
                        TempData["ErrorMessage"] = "You have reached your storage limit.";
                        return RedirectToAction(nameof(UploadPhoto));
                    default:
                        await _logService.InsertLog(new Log(UserId, LogType.UploadedPhoto, result.Item2));
                        TempData["SuccessMessage"] = "Photo has been uploaded.";
                        return RedirectToAction("Index");
                }
            }
            catch (Exception e)
            {
                _logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }


        [HttpGet]
        [Route("delete")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            try
            {
                var photo = await _photoService.GetPhoto(id);
                if (!IsUserLogged || photo.UserId != UserId)
                    return RedirectToAction("Login", "Account");

                await _photoService.DeletePhoto(id);

                await _logService.InsertLog(new Log(UserId, LogType.DeletedPhoto, id));
                TempData["SuccessMessage"] = "Photo has been deleted.";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        [HttpPost]
        [Route("update")]
        public async Task<IActionResult> UpdatePhoto(PhotoUpdateVM photoUpdateVM)
        {
            try
            {
                var photo = await _photoService.GetPhoto(photoUpdateVM.Id);
                if (!IsUserLogged || photo.UserId != UserId)
                    return RedirectToAction("Login", "Account");

                await _photoService.UpdatePhoto(photoUpdateVM);

                await _logService.InsertLog(new Log(UserId, LogType.UploadedPhoto, photoUpdateVM.Id));
                TempData["SuccessMessage"] = "Photo has been updated.";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                _logger.Log(LogSeverity.Error, e.Message);
                return Json(ERROR_MESSAGE);
            }
        }

        #endregion

    }
}
