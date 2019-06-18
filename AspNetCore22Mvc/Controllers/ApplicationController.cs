using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore22Mvc.Data;
using AspNetCore22Mvc.Extensions;
using AspNetCore22Mvc.Models.CustomModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AspNetCore22Mvc.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ApplicationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Application/Setting
        [Authorize]
        public IActionResult Settings()
        {
            return View(GetAdminSettings.GetSettings());
        }

        // POST: Application/SaveSettings
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveSettings(Admin_Settings admin_Settings)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(@"App_Data/adminSettings.json");
                    if (fileInfo.Exists)
                    {
                        fileInfo.Delete();
                    }
                }
                catch (Exception) { }
                finally
                {
                    using (StreamWriter sw = new StreamWriter(@"App_Data/adminSettings.json"))
                    {
                        sw.Write(JsonConvert.SerializeObject(admin_Settings));
                        sw.Flush();
                    }
                }
                return RedirectToAction(nameof(Settings));
            }
            return View("Settings", admin_Settings);
        }
    }
}