using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountsController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        public async Task<IActionResult> SignUp()
        {
            var model = new SignUpModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpModel model)
        {
            if(ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if(user.Status!=null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists.");
                    return View(model);
                }
                //user.Attributes.Add(CognitoAttributesConstants.Name, model.Email);
                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Password);
                if(createdUser.Succeeded)
                {
                    RedirectToAction("Confirm");
                }
            }
            return View();
        }

        public async Task<IActionResult> Confirm()
        {
            var model = new ConfirmModel();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Confirm_Post(ConfirmModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if(user==null)
                {
                    ModelState.AddModelError("NotFound", "User is not found.");
                    return View(model);
                }
                var result = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmEmailAsync(user, model.Code);
                if(result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                }
            }
                
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> LogIn(LogInModel model)
        {
            //var model = new LogInModel();
            return View(model);
        }
        [HttpPost]
        [ActionName("LogIn")]
        public async Task<IActionResult> LogIn_Post(LogInModel model)
        {
            if (ModelState.IsValid)
            {

 
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                  
                        ModelState.AddModelError("Log In Error", "Email and password do not match.");
                    
                }
            }

            return View("LogIn",model);
        }
    }
}
