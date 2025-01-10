using Microsoft.AspNetCore.Mvc;
using SchoolProj.DAL.UserRepository;
using SchoolProj.Models.AccountDTO;

namespace SchoolProj.Controllers
{
    public class ManageUserController : Controller
    {
        private readonly IUserRepo userRepo;

        public ManageUserController(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
        }

        [HttpPost]
        [Route("PasswordChange")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordChangeDTO model)
        {

            if (ModelState.IsValid)
            {
                var userResponse = await userRepo.PasswordChangeAsync(model);
                if (userResponse.IsSuccess == true && userResponse.Status == 1)
                {
                    return Json(new { success = true, message = userResponse.Message });
                }
                else if (userResponse.IsSuccess == true && userResponse.Status == 0)
                {
                    return Json(new { success = false, message = userResponse.Message });
                }
                else if (userResponse.IsSuccess == false && userResponse.Status == -1)
                {
                    return Json(new { success = false, message = $"{userResponse.Message} \n {userResponse.Error}" });
                }
                //return Json(new { success = true, message = "password changed successfully" });

            }
            // Collect model errors and return them in the response
            var errors = ModelState.Values.SelectMany(v => v.Errors)
                                           .Select(e => e.ErrorMessage)
                                           .ToList();
            return BadRequest(errors);

        }


    }
}
