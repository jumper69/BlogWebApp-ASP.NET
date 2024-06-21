using AspNetCoreHero.ToastNotification.Abstractions;
using BlogWebApp.Models;
using BlogWebApp.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notification;
        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, INotyfService notyfService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notification = notyfService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View(new LoginVM());
        }

        //[HttpGet("Login")]
        //public IActionResult Login(LoginVM vm)
        //{
        //    return View(vm);
        //}
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid) { return View(vm); }
            var existingUser = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == vm.Username);
            if (existingUser == null)
            {
                _notification.Error("Username does not exist");
                return View(vm);
            }
            var verifyPassword = await _userManager.CheckPasswordAsync(existingUser, vm.Password);
            if (!verifyPassword)
            {
                _notification.Error("Password does not match");
                return View(vm);
            }
            await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberMe, true);
            _notification.Success("Login Successful");
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }
    }
}
