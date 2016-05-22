using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using TheWorld.Models;
using TheWorld.Services;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AppController : Controller
    {
        public AppController(IWorldRepository worldRepository, IMailService mailService)
        {
            _worldRepository = worldRepository;
            _mailService = mailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Trips()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = Startup.Configuration["AppSettings:SiteEmailAddress"];

                if (string.IsNullOrWhiteSpace(email))
                {
                    ModelState.AddModelError("", "Could not send email, configuration problem.");
                }
                else
                {
                    if (_mailService.SendMail(email, email,
                        $"Contact Page from {model.Name} ({model.Email})",
                        model.Message))
                    {
                        ModelState.Clear();
                        ViewBag.MessageSuccess = "Mail Sent. Thanks!";
                    }
                }
            }

            return View();
        }

        #region Fields

        private IWorldRepository _worldRepository;
        private IMailService _mailService;

        #endregion

    }
}
