using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SchoolProj.Auth_Filters
{
    public class SessionAuthorizeAttribute:ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if the user session exists
            var userId = context.HttpContext.Session.GetInt32("UserID");
            var userTypeId = context.HttpContext.Session.GetString("UserTypeID");
            // Get the current controller and action names
            var controllerName = (string)context.RouteData.Values["controller"]!;
            var actionName = (string)context.RouteData.Values["action"]!;

            // Exclude Login and AccessDenied pages from the session check
            if (controllerName == "Accounts" && (actionName == "Login" || actionName == "AccessDenied"))
            {
                base.OnActionExecuting(context);  // Let the request continue as usual
                return;  // Do not apply the session check here
            }

            // Allow access to all action methods of the Home controller without session check
            if (controllerName == "Home")
            {
                base.OnActionExecuting(context);  // Let the request continue as usual
                return;  // Do not apply the session check here
            }

            if (controllerName == "Public")
            {
                base.OnActionExecuting(context);  // Let the request continue as usual
                return;  // Do not apply the session check here
            }

            // If session is null (i.e., user not logged in), redirect to login page
            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Accounts", null);
            }
            else
            {
                // If logged in, check UserTypeID // give access for any any basis on the UserTypeId
                if (userTypeId != "1")
                {
                    // Redirect to AccessDenied page if UserTypeID is not 2
                    context.Result = new RedirectToActionResult("AccessDenied", "Accounts", null);
                }
            }
            base.OnActionExecuting(context);  // Continue executing the action if no redirect
        }
    }
}
