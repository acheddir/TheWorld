using System.Threading.Tasks;
using Microsoft.AspNet.Http.Authentication;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Web
{
    public class AuthController : Controller
    {
        public AuthController(SignInManager<WorldUser> signInManager, UserManager<WorldUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Trips", "App");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(vm.Username, vm.Pwd, true, false);

                if (signInResult.Succeeded)
                {
                    if (string.IsNullOrWhiteSpace(returnUrl))
                        return RedirectToAction("Trips", "App");
                    return Redirect(returnUrl);
                }
                ModelState.AddModelError("", "Username or password incorrect");
            }

            return View();
        }

        public IActionResult Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Trips", "App");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = new WorldUser
                {
                    UserName = vm.Username,
                    Email = vm.Email,
                    PhoneNumber = vm.PhoneNumber
                };
                var creationResult = await _userManager.CreateAsync(user, vm.Pwd);

                if (creationResult.Succeeded)
                {
                    await _signInManager.SignInAsync(user, new AuthenticationProperties
                    {
                        IsPersistent = false
                    });

                    if (string.IsNullOrWhiteSpace(returnUrl))
                        return RedirectToAction("Trips", "App");
                    return Redirect(returnUrl);
                }
                foreach (var error in creationResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Index", "App");
        }

        #region Fields

        private SignInManager<WorldUser> _signInManager;
        private UserManager<WorldUser> _userManager;

        #endregion
    }
}