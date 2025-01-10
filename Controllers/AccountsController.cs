using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using SchoolProj.DAL.AccountRepository;
using SchoolProj.Models.AccountDTO;

namespace SchoolProj.Controllers
{
    public class AccountsController : Controller
    {
        private readonly IAccountRepo accountRepo;

        public AccountsController(IAccountRepo accountRepo)
        {
            this.accountRepo = accountRepo;
        }

        [Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (ModelState.IsValid)
            {
                // Call the repository to log in and get the response
                var response = await accountRepo.Login(loginDTO);

                if (response.userTypeID >=0) // Success
                {
                    // Clear existing session values
                    HttpContext.Session.Clear();
                   // var userid = response.userTypeID;
                    // Set session values for userID, userTypeID, and any other data you need
                    HttpContext.Session.SetInt32("UserID", Convert.ToInt32(response.UserID));
                    HttpContext.Session.SetString("UserTypeID", response.userTypeID.ToString());
                    HttpContext.Session.SetString("UserName", response.UserName.ToString());
                    //HttpContext.Session.SetString("jwt_token", response.Token!);
                    //var token = HttpContext.Session.GetString("jwt_token");
                    //if (string.IsNullOrEmpty(token))
                    //{
                    //    // Log a message or handle the case where the token is not found
                    //    throw new Exception("JWT token is missing in the session.");
                    //}
                   var id = HttpContext.Session.GetString("UserID");
                    return RedirectToAction("AdminDashBoard", "Admin");
                }
                else
                {
                    // Invalid login attempt
                    // Invalid login attempt
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your credentials.");
                    return View(loginDTO);
                }
                
            }

            // If we get to this point, something went wrong, redisplay the form
            return View(loginDTO);
        }


        [AllowAnonymous]
        [Route("Accounts/Logout")]
        public IActionResult Logout()
        {
            // Sign out the user from cookie-based authentication
            // await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Clear session data
            HttpContext.Session.Clear();
            return Redirect("/home"); // Home controllers Index method
        }


        [AllowAnonymous]
        // [Route("accessdenied")]
        public IActionResult AccessDenied()
        {
            HttpContext.Session.Clear();
            return View();
        }




    }
}
