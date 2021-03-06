﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Examonitor.Models;
using Postal;
using System.Configuration;

namespace Examonitor.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private MyDbContext db;
        private UserManager<MyUser> UserManager;
        private RoleManager<IdentityRole> RoleManager;

        public AccountController()
        {
            db = new MyDbContext();
            UserManager = new UserManager<MyUser>(new UserStore<MyUser>(db));
            RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            UserManager.UserValidator = new UserValidator<MyUser>(UserManager) { AllowOnlyAlphanumericUserNames = false };
        }

        //
        // GET: /Account/Activate
        [AllowAnonymous]
        public ActionResult Activate(string c, string i)
        {
            if (c != null && i != null)
            {
                try
                {
                    var result = UserManager.ConfirmUser(i, c);

                    if (result.Succeeded)
                    {
                        ViewBag.StatusMessage = "Account geactiveerd, U kan nu inloggen met uw email en wachtwoord.";
                    }
                    else
                    {
                        ModelState.AddModelError("", "Foutieve bevestigingscode");
                    }
                }
                catch(InvalidOperationException)
                {
                    ModelState.AddModelError("", "Foutieve email");
                }
               
            }
            else if(c == null && i == null)
            {
                ViewBag.StatusMessage = "Gelieve de instructies in de activatie-email te volgen";
            }
            else
            {
                ModelState.AddModelError("", "Foutieve bevestiginscode");
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ResetPassword(string userName)
        {
            var user = UserManager.FindByName(userName);
            string adminEmail = ConfigurationManager.AppSettings["AdminEmail"].ToString();

            if (user != null)
            {
                String url = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                if (!Request.ApplicationPath.TrimEnd('/').Equals(""))
                {
                    url += Request.ApplicationPath.TrimEnd('/');
                }
                dynamic email = new Email("ResetPassword");
                email.Url = url;
                email.To = user.Email;
                email.From = adminEmail;
                email.UserName = user.UserName;
                email.UserId = user.Id;
                email.ConfirmationToken = UserManager.GetPasswordResetToken(user.Id);
                email.Send();

                ViewBag.StatusMessage = "Gelieve de instructies in de email te volgen";
            }
            else
            {
                ModelState.AddModelError("", "Foutieve email");

            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult ResetAdminPassword()
        {
            var user = UserManager.FindByName("Admin");
            string adminEmail = ConfigurationManager.AppSettings["AdminEmail"].ToString();

            if (user != null)
            {
                String url = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                if(!Request.ApplicationPath.TrimEnd('/').Equals(""))
                {
                    url += Request.ApplicationPath.TrimEnd('/');
                }

                dynamic email = new Email("ResetPassword");
                email.Url = url;
                email.To = adminEmail;
                email.From = adminEmail;
                email.UserName = user.UserName;
                email.UserId = user.Id;
                email.ConfirmationToken = UserManager.GetPasswordResetToken(user.Id);
                email.Send();

                ViewBag.StatusMessage = "Gelieve de instructies in de email te volgen";
            }
            else
            {
                ModelState.AddModelError("", "Foutieve email");

            }

            return View("Login");
        }

        [AllowAnonymous]
        public ActionResult ResetPasswordConfirm(string c, string i)
        {
            var user = UserManager.FindById(i);
            string adminEmail = ConfigurationManager.AppSettings["AdminEmail"].ToString();

            if (user != null)
            {
                string newPassword = System.Web.Security.Membership.GeneratePassword(14, 4);
                var result = UserManager.ResetPassword(i, c, newPassword);

                if (result.Succeeded)
                {
                    String url = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                    if (!Request.ApplicationPath.TrimEnd('/').Equals(""))
                    {
                        url += Request.ApplicationPath.TrimEnd('/');
                    }
                    if (user.UserName.Equals("Admin"))
                    {
                        user.Email = adminEmail;
                    }

                    dynamic email = new Email("NewPassword");
                    email.Url = url;
                    email.To = user.Email;
                    email.From = adminEmail;
                    email.UserName = user.UserName;
                    email.Password = newPassword;
                    email.Send();

                    ViewBag.StatusMessage = "Er werd voor U een nieuw wachtwoord gegenereerd. Gelieve de instructies in de email te volgen";
                }
                else
                {
                    ModelState.AddModelError("", "Foutieve bevestiginscode");
                }
            }
            else
            {
                ModelState.AddModelError("", "Foutieve email");
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult ResendActivation()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult ResendActivation(string userName)
        {
            var mailUser = UserManager.FindByName(userName);
            string adminEmail = ConfigurationManager.AppSettings["AdminEmail"].ToString();

            if (mailUser != null && !mailUser.IsConfirmed)
            {
                String url = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                if (!Request.ApplicationPath.TrimEnd('/').Equals(""))
                {
                    url += Request.ApplicationPath.TrimEnd('/');
                }
                dynamic email = new Email("Activate");
                email.Url = url;
                email.To = mailUser.Email;
                email.From = adminEmail;
                email.UserName = mailUser.UserName;
                email.UserId = mailUser.Id;
                email.ConfirmationToken = UserManager.GetConfirmationToken(mailUser.Id);
                email.Send();

                return RedirectToAction("Activate");
            }
            else if(mailUser != null && mailUser.IsConfirmed)
            {
                ModelState.AddModelError("", "Deze account is al geactiveerd");

            }
            else
            {
                ModelState.AddModelError("", "Foutieve email");
            }

            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null && user.IsConfirmed == true)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToAction("Index", "MonitorBeurt");
                }
                else if(user != null)
                {
                    ModelState.AddModelError("", "Account moet eerst worden geactiveerd!");
                }
                else
                {
                    ModelState.AddModelError("", "Foutieve email/wachtwoord combinatie");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            string mail;
            string splitmail;

            if (ModelState.IsValid)
            {
                ValidateModel(model);
                mail = model.Email;
                splitmail = mail.Split('@')[1];

                if(splitmail.Equals("ap.be"))
                {
                    var user = new MyUser() { UserName = model.UserName };
                    user.Email = mail;
                    var result = await UserManager.CreateAsync(user, model.Password);
                    string adminEmail = ConfigurationManager.AppSettings["AdminEmail"].ToString();

                    if (result.Succeeded)
                    {
                        String url = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host + (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port);
                        if (!Request.ApplicationPath.TrimEnd('/').Equals(""))
                        {
                            url += Request.ApplicationPath.TrimEnd('/');
                        }
                        UserManager.AddToRole(user.Id, "Docent");
                        var mailUser = UserManager.FindByName(model.UserName);

                        dynamic email = new Email("Activate");
                        email.Url = url;
                        email.To = mailUser.UserName;
                        email.From = adminEmail;
                        email.UserName = mailUser.UserName;
                        email.UserId = mailUser.Id;
                        email.ConfirmationToken = UserManager.GetConfirmationToken(mailUser.Id);
                        email.Send();

                        //Don't log in, activate account first
                        //await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Activate");
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Foutieve email");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Uw wachtwoord werd veranderd"
                : message == ManageMessageId.SetPasswordSuccess ? "Uw wachtwoord werd veranderd"
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "Er heeft zich een fout opgetreden"
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new MyUser() { UserName = model.UserName };
                
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "MonitorBeurt");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
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

        private async Task SignInAsync(MyUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
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
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}