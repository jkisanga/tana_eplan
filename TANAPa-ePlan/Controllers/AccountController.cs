using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using TANAPa_ePlan.Models;
using TANAPa_ePlan.Filters;
using System.Data.Entity;
using System.IO;
using System.Data;
using System.Net.Mail;
using System.Data.OleDb;

namespace MvcLoginPage.Controllers
{


    public class AccountController : Controller
    {
        ContextModel db = new ContextModel();

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
        public ActionResult Login(LoginModel model, string returnUrl)
        {

            var username = model.UserName;
            if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password)) 
            {
                ViewBag.Message2 = "You must fill the username and password";
                return View("Login");
            }

            var userDetail = db.UserProfiles.Where(x => x.UserName == model.UserName).FirstOrDefault();
            if (userDetail == null) 
            {
                ViewBag.Message3 = "wrong username and/or password";
                return View("Login");
            }
            
                if (userDetail.Status == 1)
                {
                    ViewBag.Message = "Sorry you are no longer allow to login. Please contact system administrator or Head of Department.";
                    return View("Login");
                }
                if (userDetail.isLogin == 1)
                {
                    ViewBag.Message1 = "Yor are still login to another device. Please logout first and re-login. ";
                    return View("Login");
                }
            
            if (ModelState.IsValid && WebSecurity.Login(model.UserName, model.Password, persistCookie: model.RememberMe))
            {
                var data = db.UserProfiles.Where(x => x.UserName == model.UserName).FirstOrDefault();

                FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                userDetail.isLogin = 1;
                db.Entry(userDetail).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToDefault(returnUrl,  model.UserName);
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "The user name or password provided is incorrect.");
            return View(model);
        }


        private ActionResult RedirectToDefault(string returnUrl, string UserName)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
             
              String[] roles = Roles.GetRolesForUser(UserName);
                if (roles.Contains("Admin"))
                {
                    
                    return RedirectToAction("Index", "Admin");
                }
                else if (roles.Contains("User"))
                {
                    return RedirectToAction("Index", "Officer");
                }
                else if (roles.Contains("Manager"))
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Unauthorised");
                }
            }
        }


        //
        // POST: /Account/LogOff

        [HttpPost]
        public ActionResult LogOff()
        {

            var userDetail = db.UserProfiles.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            userDetail.isLogin = 0;
            db.Entry(userDetail).State = EntityState.Modified;
            db.SaveChanges();
            WebSecurity.Logout();

            Session.Abandon();

            // clear authentication cookie
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);

            // clear session cookie (not necessary for your current problem but i would recommend you do it anyway)
            HttpCookie cookie2 = new HttpCookie("ASP.NET_SessionId", "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            
           
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Register
        [Access(Roles = "admin")]
        [AllowAnonymous]
        public ActionResult Register()
        {
            ViewBag.DeptId = new SelectList(db.Departments, "DeptId", "DeptName");
            return View();
        }

        //
        // POST: /Account/Register
        [Access(Roles = "admin")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string register)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Users = from u in db.UserProfiles
                                select new UserListView
                                {
                                    UserName = u.UserName,
                                    FirstName = u.Firstname,
                                    LastName = u.Surname,
                                    Email = u.Email
                                };

                // Attempt to register the user
                try
                {
                    WebSecurity.CreateUserAndAccount(model.UserName, model.Password,
                        new
                        {

                            FirstName = model.Firstname,
                            MiddleName = model.Othername,
                            LastName = model.Surname,
                            Email = model.Email,
                            
                            DeptId = model.DeptId
                        });
                    //WebSecurity.Login(model.UserName, model.Password);

                    return RedirectToAction("Register", "Account");

                }

                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }


            // If we got this far, something failed, redisplay form

            return View(model);
        }

        [Access(Roles = "admin")]
        public ActionResult Edit(int id)
        {
            var user = db.UserProfiles.Find(id);
            if (user != null)
            {
                return View(user);
            }
            ModelState.AddModelError("", "User Not Found");
            return View("Register");
        }


        [Access(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Register");
            }
            return View(model);
        }


        [Access(Roles = "admin")]
        public ActionResult RoleCreate()
        {
            return View();
        }

        [Access(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleCreate(string RoleName)
        {

            Roles.CreateRole(Request.Form["RoleName"]);
            // ViewBag.ResultMessage = "Role created successfully !";

            return RedirectToAction("RoleIndex", "Account");
        }

        [Access(Roles = "admin")]
        public ActionResult RoleIndex()
        {
            var roles = Roles.GetAllRoles();
            return View(roles);
        }
        [Access(Roles = "admin")]
        public ActionResult RoleDelete(string RoleName)
        {

            Roles.DeleteRole(RoleName);
            ViewBag.UserName = new SelectList(db.UserProfiles, "UserName", "UserName");
            ViewBag.ResultMessage = "Role deleted succesfully !";


            return RedirectToAction("RoleIndex", "Account");
        }


        [Access(Roles = "admin")]
        public ActionResult RoleAddToUser()
        {
            var UserList = new List<string>();
            ViewBag.UserName = new SelectList(db.UserProfiles, "UserName", "UserName");
            var UserQry = from u in db.UserProfiles orderby u.UserName select u.UserName;
            UserList.AddRange(UserQry.Distinct());
            ViewBag.Users = new SelectList(UserList);

            SelectList list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list;

            return View();
        }

        [Access(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RoleAddToUser(string RoleName, string UserName)
        {
            var UserList = new List<string>();
            var UserQry = from u in db.UserProfiles orderby u.UserName select u.UserName;
            UserList.AddRange(UserQry.Distinct());
            ViewBag.Users = new SelectList(UserList);

            if (Roles.IsUserInRole(UserName, RoleName))
            {
                ViewBag.UserName = new SelectList(db.UserProfiles, "UserName", "UserName");
                ViewBag.ResultMessage = "This user already has the role specified !";
            }
            else
            {
                ViewBag.UserName = new SelectList(db.UserProfiles, "UserName", "UserName");
                Roles.AddUserToRole(UserName, RoleName);
                ViewBag.ResultMessage = "Username added to the role succesfully !";
            }

            //ViewBag.UserId = new SelectList(db.UserProfiles, "UserName", "UserName");
            SelectList list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list;
            return View();
        }

        /// <summary>
        /// Get all the roles for a particular user
        /// </summary>
        /// <param name="UserName"></param>
        /// <returns></returns>
        /// 

        [Access(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRoles(string UserName)
        {
            ViewBag.UserName = new SelectList(db.UserProfiles, "UserName", "UserName");
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                ViewBag.RolesForThisUser = Roles.GetRolesForUser(UserName);
                SelectList list = new SelectList(Roles.GetAllRoles());
                ViewBag.Roles = list;
            }
            return View("RoleAddToUser");
        }


        [HttpPost]
        [Access(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteRoleForUser(string UserName, string RoleName)
        {
            ViewBag.UserName = new SelectList(db.UserProfiles, "UserName", "UserName");

            if (Roles.IsUserInRole(UserName, RoleName))
            {
                Roles.RemoveUserFromRole(UserName, RoleName);
                ViewBag.ResultMessage = "Role removed from this user successfully !";
            }
            else
            {
                ViewBag.ResultMessage = "This user doesn't belong to selected role.";
            }
            ViewBag.RolesForThisUser = Roles.GetRolesForUser(UserName);
            SelectList list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list;


            return View("RoleAddToUser");
        }





        //
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string UserName)
        {
            //check user existance
            var user = Membership.GetUser(UserName);
            if (user == null)
            {
                TempData["Message"] = "User Not exist.";
            }
            else
            {
                //generate password token
                var token = WebSecurity.GeneratePasswordResetToken(UserName);
                //create url with above token
                var resetLink = "<a href='" + Url.Action("ResetPassword", "Account", new { un = UserName, rt = token }, "http") + "'>Reset Password</a>";
                //get user emailid
                UsersContext db = new UsersContext();
                var emailid = (from i in db.UserProfiles
                               where i.UserName == UserName
                               select i.Email).FirstOrDefault();
                //send mail
                string subject = "Password Reset Token";
                string body = "<b>Please find the Password Reset Token</b><br/>" + resetLink; //edit it
                try
                {
                    SendEMail(emailid, subject, body);
                    TempData["Message"] = "Mail Sent.";
                }
                catch (Exception ex)
                {
                    TempData["Message"] = "Error occured while sending email." + ex.Message;
                }
                //only for testing
                TempData["Message"] = resetLink;
            }

            return View();
        }



        //rabder users
        //get
        public ActionResult UserProfile()
        {
            var UsersList = from u in db.UserProfiles
                            select new UserListView
                            {
                                UserName = u.UserName,
                                FirstName = u.Firstname,
                                LastName = u.Surname,
                                Email = u.Email
                            };


            return View(UsersList);
        }

        public ActionResult UserList()
        {
            var Users = from u in db.UserProfiles
                        where u.UserName != "admin"
                        select new UserListView
                        {
                            UserName = u.UserName,
                            FirstName = u.Firstname,
                            LastName = u.Surname,
                            Email = u.Email,
                            UserId = u.UserId
                        };

            return PartialView("_UserList", Users);
        }

        private ContextModel ContextModel()
        {
            throw new NotImplementedException();
        }

        [Access(Roles = "admin")]
        public ActionResult ImportEmployee()
        {
            ViewBag.DeptId = new SelectList(db.Departments,"DeptId", "DeptName");
            return View();
        }
        [Access(Roles = "admin")]
        [HttpPost]
        public ActionResult ImportEmployee(HttpPostedFileBase fileupload, string Employee, int Dept)
        {

    
                if (fileupload.ContentLength > 0)
                {

                    string extension = System.IO.Path.GetExtension(fileupload.FileName);
                    string path = string.Format("{0}/{1}", Server.MapPath("~/App_Data/EmployeeUpload"), fileupload.FileName);
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);

                    fileupload.SaveAs(path);

                    //Create connection string to Excel work book
                    string excelConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;Persist Security Info=False";
                    //Create Connection to Excel work book
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    //Create OleDbCommand to fetch data from Excel
                    OleDbCommand cmd = new OleDbCommand("Select * from [Sheet1$]", excelConnection);

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);

                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    da.Dispose();
                    excelConnection.Close();
                    excelConnection.Dispose();

                    // Import to Database

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        //string checkNo = dr["checkNumber"].ToString(); // will use check number to check if user exists
                        // var exists = db.UserProfiles.Where(a => a.EmployeeID.Equals(empID)).FirstOrDefault();
                        //  CheckNo	LastName	FirstName	MiddleName	Position	Gender	MarStatus	Age

                        //var firstChar = firstname.Substring(0, 1);
                        var Surname = dr["Surname"].ToString();
                        var EmployeeCode = dr["EmployeeCode"].ToString();
                        var Firstname = dr["Firstname"].ToString();
                        var UserName = Firstname.ToLower() +"."+ Surname.ToLower();

                        var existingUser = db.UserProfiles.Where(x => x.EmployeeCode == EmployeeCode).Where(x => x.UserName == EmployeeCode.ToLower() + Surname.ToLower()).FirstOrDefault();
                        if (existingUser == null)
                        {
                            WebSecurity.CreateUserAndAccount(UserName, "tanapa123",
                                    new
                                    {
                                        EmployeeCode = dr["EmployeeCode"].ToString(),
                                        Firstname = dr["Firstname"].ToString(),
                                        Othername = dr["Othername"].ToString(),
                                        Surname = dr["Surname"].ToString(),
                                        Branch = dr["Branch"].ToString(),
                                        DeptId = dr["Department"].ToString(),
                                        Section = dr["Section"].ToString(),
                                        Unit = dr["Unit"].ToString(),
                                        Job = dr["Job"].ToString(),
                                        CostCenter = dr["CostCenter"].ToString(),

                                        GradeGroup = dr["GradeGroup"].ToString(),
                                        Grade = dr["Grade"].ToString(),
                                        GradeLevel = dr["GradeLevel"].ToString(),
                                        Increment = dr["Increment"].ToString(),
                                        EmploymentType = dr["EmploymentType"].ToString(),
                                        DateHired = dr["DateHired"].ToString(),
                                        Birthdate = dr["Birthdate"].ToString(),
                                        Gender = dr["Gender"].ToString(),
                                        MaritalStatus = dr["MaritalStatus"].ToString(),
                                        Religion = dr["Religion"].ToString(),


                                        Telephone = dr["Telephone"].ToString(),
                                        PresentAddress = dr["PresentAddress"].ToString(),
                                        Email = dr["Email"].ToString(),
                                        EmergencyContact = dr["EmergencyContact"].ToString(),
                                        Bank = dr["Bank"].ToString(),
                                        Membership = dr["Membership"].ToString(),
                                        DepartmentGroup = dr["DepartmentGroup"].ToString(),
                                        SectionGroup = dr["SectionGroup"].ToString(),
                                        UnitGroup = dr["UnitGroup"].ToString(),
                                        Team = dr["Team"].ToString(),

                                        JobGroup = dr["JobGroup"].ToString(),
                                        ClassGroup = dr["ClassGroup"].ToString(),
                                        Class = dr["Class"].ToString(),
                                        AnniversaryDate = dr["AnniversaryDate"].ToString(),                                     
                                        ConfirmationDate = dr["ConfirmationDate"].ToString(),
                                        ReinstatementDate = dr["ReinstatementDate"].ToString(),
                                        ProbationStartDate = dr["ProbationStartDate"].ToString(),
                                        SuspendedFromDate = dr["SuspendedFromDate"].ToString(),
                                        ProbationEndDate = dr["ProbationEndDate"].ToString(),
                                        SuspendedToDate = dr["SuspendedToDate"].ToString(),

                                        EndOfContract = dr["EndOfContract"].ToString()
                                       

                                    });

                            Roles.AddUserToRole(UserName, "User");
                        } 
                    }

                }
            
            return RedirectToAction("Index", "Admin");
            
        }
        [Access(Roles = "admin")]
        public void AddUserToDefaulRole(string Username)
        {
            Roles.AddUserToRole(Username, "User");
        }

        //
        // GET: /Account/Manage

        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed successful."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : "";
            ViewBag.HasLocalPassword = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage

        [HttpPost]
        public ActionResult Manage(LocalPasswordModel model)
        {
            bool hasLocalAccount = OAuthWebSecurity.HasLocalAccount(WebSecurity.GetUserId(User.Identity.Name));
            ViewBag.HasLocalPassword = hasLocalAccount;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasLocalAccount)
            {
                if (ModelState.IsValid)
                {
                    // ChangePassword will throw an exception rather than return false in certain failure scenarios.
                    bool changePasswordSucceeded;
                    try
                    {
                        changePasswordSucceeded = WebSecurity.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }

                    if (changePasswordSucceeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                    }
                }
            }
            else
            {
                // User does not have a local password so remove any validation errors caused by a missing
                // OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        WebSecurity.CreateAccount(User.Identity.Name, model.NewPassword);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError("", String.Format("Unable to create local account. An account with the name \"{0}\" may already exist.", User.Identity.Name));
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        public ActionResult CompleteProfile()
        {
            var userId = WebSecurity.CurrentUserId;
            var data = db.UserProfiles.Where(y => y.UserId == userId).FirstOrDefault();

            return View(data);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompleteProfile(UserProfile userprofile, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    string targetFolder = HttpContext.Server.MapPath("~/Images/profiles"); //files's folder on the server
                    string fileName = Path.GetFileName(Image.FileName); //capture file name
                    string path = Path.Combine(targetFolder, fileName); // file path location

                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.Message = "The File Already Exists in System";
                    }
                    else
                    {
                        Image.SaveAs(path);
                    }
                }

                db.Entry(userprofile).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(userprofile);

        }
        public ActionResult ProfileImage()
        {
            var user = db.UserProfiles.Where(x => x.UserId == WebSecurity.CurrentUserId).FirstOrDefault();
            if (user != null)
            {
                ViewBag.fName = user.Firstname;
                ViewBag.lName = user.Surname;
                return PartialView("_ProfileImage", user);
            }
            return new EmptyResult();
        }

        [Access(Roles = "admin")]
        public ActionResult ResetPassword(int id)
        {
            var user = (from i in db.UserProfiles where i.UserId == id select i).FirstOrDefault();
            if (user != null)
            {
                //generate password token           
                var token = WebSecurity.GeneratePasswordResetToken(user.UserName);
                ViewBag.token = token;
                return View(user);
            }
            return RedirectToAction("Register");
        }
        //
        // POST: /Account/ResetPassword
        [Access(Roles = "admin")]
        [HttpPost]
        public ActionResult ResetPassword(string UserName, string rt)
        {
            UsersContext _UsersContext = new UsersContext();
            //TODO: Check the un and rt matching and then perform following
            //get userid of received username
            var user = (from i in db.UserProfiles
                        where i.UserName == UserName
                        select i).FirstOrDefault();
            //check userid and token matches
            //bool any = (from j in db.webpages_Membership
            //            where (j.UserId == user.UserId)
            //            && (j.PasswordVerificationToken == rt)
            //            //&& (j.PasswordVerificationTokenExpirationDate < DateTime.Now)
            //            select j).Any();

            //if (any == true)
            //{
            //    //generate random password
            //    string newpassword = GenerateRandomPassword(6);
            //    //reset password
            //    bool response = WebSecurity.ResetPassword(rt, newpassword);
            //    if (response == true)
            //    {

            //        //get new password

            //        TempData["Message"] = "Success! New Password Is " + newpassword;
            //        return View(user);
            //    }
            //    else
            //    {
            //        TempData["Message"] = "Hey, avoid random request on this page.";
            //    }
            //}
            //else
            //{
            //    TempData["Message"] = "Username and token not maching.";
            //}

            return View(user);
        }


        //
        private void SendEMail(string emailid, string subject, string body)
        {
            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.EnableSsl = true;
            client.Host = "smtp.gmail.com";
            client.Port = 587;

            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("tzchoice@gmail.com", "Pas!1224");
            client.UseDefaultCredentials = false;
            client.Credentials = credentials;

            MailMessage msg = new MailMessage();
            msg.From = new MailAddress("tzchoice@gmail.com");
            msg.To.Add(new MailAddress(emailid));

            msg.Subject = subject;
            msg.IsBodyHtml = true;
            msg.Body = body;

            client.Send(msg);
        }

        #region Helpers

        private string GenerateRandomPassword(int length)
        {
            string allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789!@$?_-*&#+";
            char[] chars = new char[length];
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
            }
            return new string(chars);
        }
        

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion


        
    }
}
