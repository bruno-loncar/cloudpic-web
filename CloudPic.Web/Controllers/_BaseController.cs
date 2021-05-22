using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CloudPic.Web.Models;
using Microsoft.AspNetCore.Authentication;
using CloudPic.Models.VM;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace CloudPic.Web.Controllers
{
    public class _BaseController : Controller
    {
        public bool IsUserLogged => HttpContext.Session.GetInt32("UserId") > 0;
        public bool IsUserAdmin => HttpContext.Session.GetInt32("Admin") > 0;
        public int UserId => HttpContext.Session.GetInt32("UserId").Value;

    }
}
