﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebApiInsight.Administrator.Models;

namespace WebApiInsight.Administrator.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private UserManager _userManager;

        public ManageController()
        {
        }

        public ManageController(UserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public UserManager UserManager
        {
            get
            {
                return _userManager ?? new UserManager();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "Your phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                //PhoneNumber = await UserManager.GetPhoneNumberAsync(userId),
                //Logins = await UserManager.GetLoginsAsync(userId),
                //BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(userId)
            };
            return View(model);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            var message = string.Empty;
            //var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            //if (result.Succeeded)
            //{
            //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //    if (user != null)
            //    {
            //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            //    }
            //    message = ManageMessageId.RemoveLoginSuccess;
            //}
            //else
            //{
            //    message = ManageMessageId.Error;
            //}
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Manage/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }
        
        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            throw new NotImplementedException();
            //if (!ModelState.IsValid)
            //{
            //    return View(model);
            //}
            //var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            //if (result.Succeeded)
            //{
            //    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //    if (user != null)
            //    {
            //        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            //    }
            //    return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            //}
            //AddErrors(result);
            //return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            throw new NotImplementedException();
            //if (ModelState.IsValid)
            //{
            //    var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
            //    if (result.Succeeded)
            //    {
            //        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //        if (user != null)
            //        {
            //            await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            //        }
            //        return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
            //    }
            //    AddErrors(result);
            //}

            //// If we got this far, something failed, redisplay form
            //return View(model);
        }

        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            throw new NotImplementedException();
            //ViewBag.StatusMessage =
            //    message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
            //    : message == ManageMessageId.Error ? "An error has occurred."
            //    : "";
            //var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            //if (user == null)
            //{
            //    return View("Error");
            //}
            //var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            //var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            //ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            //return View(new ManageLoginsViewModel
            //{
            //    CurrentLogins = userLogins,
            //    OtherLogins = otherLogins
            //});
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

#region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            //var user = UserManager.FindById(User.Identity.GetUserId());
            //if (user != null)
            //{
            //    return user.PasswordHash != null;
            //}
            return false;
        }
        
        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

#endregion
    }
}