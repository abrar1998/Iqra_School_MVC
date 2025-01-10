using Microsoft.AspNetCore.Mvc;
using SchoolProj.DAL.NoticeRepository;
using SchoolProj.DAL.SliderRepository;
using SchoolProj.General;

namespace SchoolProj.Controllers
{
    public class PublicController : Controller
    {
        private readonly INoticeRepo noticeRepo;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly ISliderRepo sliderRepo;

        public PublicController(INoticeRepo noticeRepo, IHttpContextAccessor httpContextAccessor, ISliderRepo sliderRepo)
        {
            this.noticeRepo = noticeRepo;
            this.httpContextAccessor = httpContextAccessor;
            this.sliderRepo = sliderRepo;
        }

        
        [Route("Public/Notices")]
        [HttpGet]
        public async Task<IActionResult> Notices()
        {
            try
            {
                var noticeList = await sliderRepo.GetNoticeListWithFilesAsync();
                return View(noticeList.ResponseData);

               
            }
            catch (Exception ex)
            {
                // _logger.LogError(ex, "An error occurred while retrieving notifications.");
                return BadRequest(ex.Message);
            }
        }

        
        [Route("/Home/GetNotice/{id}")]
        
        public async Task<IActionResult> GetNotice(string id)
        {
            try
            {
                var noticeResponse = await noticeRepo.GetNoticeListWithFilesAsync();
                var notice = noticeResponse.ResponseData.FirstOrDefault(n => n.Nid == id);
                return View(notice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
