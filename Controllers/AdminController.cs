using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolProj.DAL.NoticeRepository;

namespace SchoolProj.Controllers
{
    public class AdminController : Controller
    {
        private readonly INoticeRepo noticeRepo;

        public AdminController(INoticeRepo noticeRepo)
        {
            this.noticeRepo = noticeRepo;
            
        }

        
        public IActionResult AdminDashBoard()
        {
            return View();
        }
    }
}
