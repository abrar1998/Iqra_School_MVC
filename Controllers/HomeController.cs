using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using SchoolProj.DAL.DownloadRepository;
using SchoolProj.DAL.GalleryRepository;
using SchoolProj.DAL.NoticeRepository;
using SchoolProj.DAL.PageLoadService;
using SchoolProj.DAL.SliderRepository;
using SchoolProj.General;
using SchoolProj.Models;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SchoolProj.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INoticeRepo noticeRepo;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPageLoadRepo pageLoadRepo;
        private readonly ISliderRepo sliderRepo;
        private readonly IDownloadRepo downloadRepo;
        private readonly IGalleryRepo galleryRepo;

        public HomeController(ILogger<HomeController> logger, INoticeRepo noticeRepo,
            IHttpContextAccessor httpContextAccessor, IPageLoadRepo pageLoadRepo, ISliderRepo sliderRepo, IDownloadRepo downloadRepo,
            IGalleryRepo galleryRepo)
        {
            _logger = logger;
            this.noticeRepo = noticeRepo;
            this.httpContextAccessor = httpContextAccessor;
            this.pageLoadRepo = pageLoadRepo;
            this.sliderRepo = sliderRepo;
            this.downloadRepo = downloadRepo;
            this.galleryRepo = galleryRepo;
        }
        [Route("/")]
        [Route("home")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var indexPageData = new IndexPageViewModel
                {
                    Categories = new List<Photos>(),  // Initialize to avoid null reference issues
                    Sliders = new List<Slider>()       // Initialize Sliders to avoid null reference issues
                };
                ViewBag.Album = false;
                var catResponse = await galleryRepo.GetPhotoCategoriesAsync();
                if (catResponse != null && catResponse.IsSuccess && catResponse.Status == 1 && catResponse.ResponseData != null)
                {
                    if (catResponse.ResponseData.Any())  // Use Any() instead of Count() for better performance
                    {
                        catResponse.ResponseData = galleryRepo.GeneratePhotosUrl(catResponse.ResponseData);
                        indexPageData.Categories = catResponse.ResponseData.Take(4).ToList();
                        ViewBag.Album = true;
                    }
                }

                var slides = await sliderRepo.GetSlidesAsync();
                if (slides != null && slides.ResponseData != null && slides.ResponseData.Any())
                {
                    foreach (var slide in slides.ResponseData)
                    {
                        slide.SliderImage = FileUploadHelper.GenerateFileUrl(slide.SliderImage, httpContextAccessor.HttpContext!.Request);
                    }
                    indexPageData.Sliders = slides.ResponseData;
                    ViewBag.IndexCarousel = false;
                }
                else
                {
                    ViewBag.IndexCarousel = true;
                }

                var director = await pageLoadRepo.GetPageByIdAsync("20011");
                if(director != null)
                {
                    director.PagePic = FileUploadHelper.GenerateFileUrl(director.PagePic!, httpContextAccessor.HttpContext!.Request);
                    indexPageData.DirectorMessage = director;
                    ViewBag.Director = false;
                }
                else
                {
                    ViewBag.Director = true;
                }

                return View(indexPageData);
            }
            catch
            {
                ViewBag.IndexCarousel = true;
                return View();
            }
        }


        public async Task<IActionResult> ViewImages(string pcidfk)
        {
            if (string.IsNullOrEmpty(pcidfk))
            {
                return BadRequest("Photo category ID is required.");
            }

            try
            {
                var photosUnderCategory = await galleryRepo.GetImagesByCategoryIdAsync(pcidfk);

                if (photosUnderCategory.ResponseData == null || !photosUnderCategory.ResponseData.Any())
                {
                    return View(new List<Photos>()); // Optional: Use a separate view for "no images found" scenario.
                }
                ViewBag.PhotoCategoryName = photosUnderCategory.ResponseData.Select(p => p.PhotoCatName).FirstOrDefault();
                return View(photosUnderCategory.ResponseData);
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using ILogger)
                return StatusCode(500, "An error occurred while fetching the images.");
            }
        }

        public async Task<IActionResult> ViewAllCategories()
        {
            try
            {
                var catResponse = await galleryRepo.GetPhotoCategoriesAsync();
                if (catResponse.IsSuccess == true && catResponse.Status == 1)
                {
                    //now generate url of each file
                    if (catResponse.ResponseData.Count() > 0)
                    {
                        catResponse.ResponseData = galleryRepo.GeneratePhotosUrl(catResponse.ResponseData);
                    }

                }
                return View(catResponse.ResponseData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //this is default page which is being displayed initially when app loads
        //public async Task<IActionResult> Default()
        //{
        //    try
        //    {
        //        var slides = await sliderRepo.GetSlidesAsync();
        //        foreach (var slide in slides.ResponseData)
        //        {
        //            slide.SliderImage = FileUploadHelper.GenerateFileUrl(slide.SliderImage, httpContextAccessor.HttpContext!.Request);
        //        }
        //        // var slides = pageLoadRepo.GetSlidersAsync();
        //        if (slides.ResponseData.Count() <= 0)
        //        {
        //            ViewBag.ShowDefaultCarousel = true;
        //            return View();
        //        }
        //        ViewBag.ShowDefaultCarousel = false;
        //        return View(slides.ResponseData);
        //    }
        //    catch
        //    {
        //        ViewBag.ShowDefaultCarousel = true;
        //        return View();
        //    }
        //}

        //get notice list for notification , but select on top 3
        //[HttpGet]
        //public async Task<IActionResult> GetNoticeList()
        //{
        //    try
        //    {
        //        var noticeList = await noticeRepo.GetNoticeListAsync();
        //        if (noticeList.ResponseData.Count() != 0)
        //        {
        //            var request = httpContextAccessor.HttpContext!.Request;
        //            foreach (var notice in noticeList.ResponseData)
        //            {
        //                if (notice.ThumbNail != null)
        //                {
        //                    var thumbNailUrl = FileUploadHelper.GenerateFileUrl(notice.ThumbNail!, request);
        //                    if (thumbNailUrl != null)
        //                    {
        //                        notice.ThumbNail = thumbNailUrl;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"Thumbnail file not found: {notice.ThumbNail}");
        //                    }
        //                }

        //                if (notice.FilePath != null)
        //                {
        //                    var filePathUrl = FileUploadHelper.GenerateFileUrl(notice.FilePath!, request);
        //                    if (filePathUrl != null)
        //                    {
        //                        notice.FilePath = filePathUrl;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"File not found: {notice.FilePath}");
        //                    }
        //                }
        //            }
        //        }
        //        var noticesToDisplay = noticeList.ResponseData
        //                                         .OrderByDescending(n => n.NDate)
        //                                         .Take(5)
        //                                         .ToList();
        //        //return Json(new List<Notice>());
        //        return Json(noticesToDisplay);
        //        //return Json(noticeList.ResponseData);
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }

        //}

        //[HttpGet]
        //public async Task<IActionResult> GetLatestNewsList()
        //{
        //    try
        //    {
        //        var noticeList = await noticeRepo.GetNoticeListAsync();
        //        if (noticeList.ResponseData.Count() != 0)
        //        {
        //            var request = httpContextAccessor.HttpContext!.Request;
        //            foreach (var notice in noticeList.ResponseData)
        //            {
        //                if (notice.ThumbNail != null)
        //                {
        //                    var thumbNailUrl = FileUploadHelper.GenerateFileUrl(notice.ThumbNail!, request);
        //                    if (thumbNailUrl != null)
        //                    {
        //                        notice.ThumbNail = thumbNailUrl;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"Thumbnail file not found: {notice.ThumbNail}");
        //                    }
        //                }

        //                if (notice.FilePath != null)
        //                {
        //                    var filePathUrl = FileUploadHelper.GenerateFileUrl(notice.FilePath!, request);
        //                    if (filePathUrl != null)
        //                    {
        //                        notice.FilePath = filePathUrl;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"File not found: {notice.FilePath}");
        //                    }
        //                }
        //            }
        //        }
        //        return Json(noticeList.ResponseData.Where(n=>n.Ncid == "3").ToList());
        //    }
        //    catch
        //    {
        //        return BadRequest();
        //    }

        //}
        [HttpGet]
        public async Task<IActionResult> GetNoticeList()
        {
            try
            {
                var noticeList = await noticeRepo.GetNoticeListAsync();

                if (noticeList?.ResponseData != null && noticeList.ResponseData.Any())
                {
                    var request = httpContextAccessor.HttpContext!.Request;
                    foreach (var notice in noticeList.ResponseData)
                    {
                        if (!string.IsNullOrEmpty(notice.ThumbNail))
                        {
                            var thumbNailUrl = FileUploadHelper.GenerateFileUrl(notice.ThumbNail, request);
                            notice.ThumbNail = thumbNailUrl ?? notice.ThumbNail;
                        }

                        if (!string.IsNullOrEmpty(notice.FilePath))
                        {
                            var filePathUrl = FileUploadHelper.GenerateFileUrl(notice.FilePath, request);
                            notice.FilePath = filePathUrl ?? notice.FilePath;
                        }
                    }
                    var responseData = noticeList.ResponseData
                        .OrderByDescending(n => DateTime.Parse(n.NDate).Year)  // Sort by year, descending
                        .ThenByDescending(n => DateTime.Parse(n.NDate).Month)  // Sort by month, descending
                        .ThenByDescending(n => DateTime.Parse(n.NDate).Day);   // Sort by day, descending
                    return Json(responseData.Take(6).ToList());

                    //var noticesToDisplay = noticeList.ResponseData
                    //                                  .OrderByDescending(n => n.NDate)
                    //                                  .Take(5)
                    //                                  .ToList();
                    //return Json(noticesToDisplay);
                }

                // Return an empty list if no data is present
                return Json(new List<NoticeDTO>());
            }
            catch
            {
                return BadRequest(new { message = "An error occurred while fetching notices." });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetLatestNewsList()
        {
            try
            {
                var noticeList = await noticeRepo.GetNoticeListAsync();

                if (noticeList?.ResponseData != null && noticeList.ResponseData.Any())
                {
                    var request = httpContextAccessor.HttpContext!.Request;
                    foreach (var notice in noticeList.ResponseData)
                    {
                        if (!string.IsNullOrEmpty(notice.ThumbNail))
                        {
                            var thumbNailUrl = FileUploadHelper.GenerateFileUrl(notice.ThumbNail, request);
                            notice.ThumbNail = thumbNailUrl ?? notice.ThumbNail;
                        }

                        if (!string.IsNullOrEmpty(notice.FilePath))
                        {
                            var filePathUrl = FileUploadHelper.GenerateFileUrl(notice.FilePath, request);
                            notice.FilePath = filePathUrl ?? notice.FilePath;
                        }
                    }
                    var responseData = noticeList.ResponseData
                        .OrderByDescending(n => DateTime.Parse(n.NDate).Year)  // Sort by year, descending
                        .ThenByDescending(n => DateTime.Parse(n.NDate).Month)  // Sort by month, descending
                        .ThenByDescending(n => DateTime.Parse(n.NDate).Day);   // Sort by day, descending

                    //return Json(noticeList.ResponseData.Where(n => n.Ncid == "3").ToList());
                    return Json(responseData.Where(n => n.Ncid == "3").ToList());
                }

                // Return an empty list if no data is present
                return Json(new List<NoticeDTO>());
            }
            catch
            {
                return BadRequest(new { message = "An error occurred while fetching the latest news." });
            }
        }



        [Route("Home/Page={pageUrl}/{id}")] //generate view dynamically for all pages
        public async Task<IActionResult> DynamicPageContent(string pageUrl, string id)
        {
            try
            {
                //page id for aboutus page is 20005
                //string pageId = "20005";
                var page = await pageLoadRepo.GetPageByIdAsync(id);
                return View(page);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //[HttpGet]
        //[Route("/Home/GetNotice/{id}")]
        //public async Task<IActionResult> GetNotice(string id)
        //{

        //    try
        //    {
        //        var noticeResponse = await noticeRepo.GetNoticeByIdAsync(id);
        //        if(noticeResponse.IsSuccess == true && noticeResponse.Status == 1)
        //        {
        //            var noticeDto = noticeResponse.ResponseData;
        //            if(noticeDto !=null)
        //            {
        //                //generate file and image url
        //                if (noticeDto.ThumbNail != null)
        //                {
        //                    var thumbNailUrl = FileUploadHelper.GenerateFileUrl(noticeDto.ThumbNail!, httpContextAccessor.HttpContext!.Request);
        //                    if (thumbNailUrl != null)
        //                    {
        //                        noticeDto.ThumbNail = thumbNailUrl;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"Thumbnail file not found: {noticeDto.ThumbNail}");
        //                    }
        //                }

        //                if (noticeDto.FilePath != null)
        //                {
        //                    var filePathUrl = FileUploadHelper.GenerateFileUrl(noticeDto.FilePath!, httpContextAccessor.HttpContext!.Request);
        //                    if (filePathUrl != null)
        //                    {
        //                        noticeDto.FilePath = filePathUrl;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"File not found: {noticeDto.FilePath}");
        //                    }
        //                }

        //                return View(noticeDto);

        //            }
        //        }
        //        return BadRequest($"{noticeResponse.Message} \n\n {noticeResponse.Error}");
        //    }
        //    catch(Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //get all notifications
        //[HttpGet]
        //public async Task<IActionResult> ViewAllNotifications()
        //{
        //    try
        //    {
        //        var noticeList = await noticeRepo.GetNoticeListAsync();
        //        if(noticeList.ResponseData.Count() >0)
        //        {
        //            var request = httpContextAccessor.HttpContext!.Request;
        //            foreach (var notice in noticeList.ResponseData)
        //            {
        //                if (notice.ThumbNail != null)
        //                {
        //                    var thumbNailUrl = FileUploadHelper.GenerateFileUrl(notice.ThumbNail!, request);
        //                    if (thumbNailUrl != null)
        //                    {
        //                        notice.ThumbNail = thumbNailUrl;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"Thumbnail file not found: {notice.ThumbNail}");
        //                    }
        //                }

        //                if (notice.FilePath != null)
        //                {
        //                    var filePathUrl = FileUploadHelper.GenerateFileUrl(notice.FilePath!, request);
        //                    if (filePathUrl != null)
        //                    {
        //                        notice.FilePath = filePathUrl;
        //                    }
        //                    else
        //                    {
        //                        Console.WriteLine($"File not found: {notice.FilePath}");
        //                    }
        //                }
        //            }

        //            return View(noticeList.ResponseData);
        //        }
        //        else
        //        {
        //            return NotFound("No Notice Found");
        //        }

        //    }
        //    catch(Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}

        //Directors Message



        [HttpGet]
        [Route("/Home/GetNotice/{id}")]
        public async Task<IActionResult> GetNotice(string id)
        {
            try
            {
                // var noticeResponse = await sliderRepo.GetNoticeListWithFilesAsync();
                // var notice = noticeResponse.ResponseData.FirstOrDefault(n => n.Nid == id);
                // return View(notice);
                //var noticeResponse = await noticeRepo.GetNoticeByIdAsync(id);

                //if (noticeResponse.IsSuccess && noticeResponse.Status == 1)
                //{
                //    var noticeDto = noticeResponse.ResponseData;
                //    if (noticeDto != null)
                //    {
                //        // Generate file and image URLs
                //        //var request = httpContextAccessor.HttpContext?.Request;
                //        //if (request == null)
                //        //{
                //        //    return BadRequest("Unable to retrieve the HTTP request context.");
                //        //}

                //        //if (!string.IsNullOrEmpty(noticeDto.ThumbNail))
                //        //{
                //        //    var thumbNailUrl = FileUploadHelper.GenerateFileUrl(noticeDto.ThumbNail, request);
                //        //    noticeDto.ThumbNail = thumbNailUrl ?? noticeDto.ThumbNail;
                //        //}

                //        //if (!string.IsNullOrEmpty(noticeDto.FilePath))
                //        //{
                //        //    var filePathUrl = FileUploadHelper.GenerateFileUrl(noticeDto.FilePath, request);
                //        //    noticeDto.FilePath = filePathUrl ?? noticeDto.FilePath;
                //        //}

                //        return View(noticeDto);
                //    }

                //    return NotFound("Notice data is null.");
                //}

                //return BadRequest($"{noticeResponse.Message} \n\n {noticeResponse.Error}");
                var noticeResponse = await noticeRepo.GetNoticeListWithFilesAsync();
                var notice = noticeResponse.ResponseData!.FirstOrDefault(n => n.Nid == id);
                notice!.ThumbNail = null;
                return View(notice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the notice with ID {Id}", id);
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }


        [HttpGet]
        public async Task<IActionResult> ViewAllNotifications()
        {
            try
            {
                //var noticeList = await noticeRepo.GetNoticeListAsync();

                //if (noticeList.ResponseData != null && noticeList.ResponseData.Any())
                //{
                //    var request = httpContextAccessor.HttpContext?.Request;
                //    if (request == null)
                //    {
                //        return BadRequest("Unable to retrieve the HTTP request context.");
                //    }

                //    foreach (var notice in noticeList.ResponseData)
                //    {
                //        if (!string.IsNullOrEmpty(notice.ThumbNail))
                //        {
                //            var thumbNailUrl = FileUploadHelper.GenerateFileUrl(notice.ThumbNail, request);
                //            if (thumbNailUrl != null)
                //            {
                //                notice.ThumbNail = thumbNailUrl;
                //            }
                //            else
                //            {
                //                _logger.LogWarning("Thumbnail file not found: {ThumbNail}", notice.ThumbNail);
                //            }
                //        }

                //        if (!string.IsNullOrEmpty(notice.FilePath))
                //        {
                //            var filePathUrl = FileUploadHelper.GenerateFileUrl(notice.FilePath, request);
                //            if (filePathUrl != null)
                //            {
                //                notice.FilePath = filePathUrl;
                //            }
                //            else
                //            {
                //                _logger.LogWarning("File not found: {FilePath}", notice.FilePath);
                //            }
                //        }
                //    }

                //    return View(noticeList.ResponseData);
                //}
                //else
                //{
                //    return NotFound("No notices found in the database.");
                //}

                var noticeResponse = await noticeRepo.GetNoticeListWithFilesAsync();
                return View(noticeResponse.ResponseData);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving notifications.");
                return StatusCode(500, "An unexpected error occurred. Please try again later.");
            }
        }

        public async Task<IActionResult> DirectorAndChairmanMessage()
        {
            try
            {
                var dPageId = "6"; //director page id
                var cPageId = "20007"; //chairman page id
                var directorPageData = await pageLoadRepo.GetPageByIdAsync(dPageId);
                var chairmanPageData = await pageLoadRepo.GetPageByIdAsync(cPageId);
                Console.WriteLine(directorPageData!.PagePic);
                Console.WriteLine(chairmanPageData!.PagePic);

                var cdViewModel = new DirectorChairManViewModel()
                {
                    DirectorsMessage = directorPageData,
                    ChairMansMessage = chairmanPageData,

                };


                return View(cdViewModel);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("Alldownloads")]
        public async Task<IActionResult> AllDownload()
        {
           try
           {
               await LoadDownlaodAndClassTypeForDropDown();
               return View();
           }
           catch(Exception ex)
           {
               return BadRequest(ex.Message);
           }
        }

        public async Task LoadDownlaodAndClassTypeForDropDown()
        {
            var downloadTypes = await downloadRepo.GetAllDownloadsTypesForDropDownAsync();
            var classTypes = await downloadRepo.GetAllClassesForDropDownAsync();

            if (downloadTypes.IsSuccess && downloadTypes.Status == 1 && classTypes.IsSuccess && classTypes.Status == 1)
            {
                // Map response data to ClassTypes
                var classes = classTypes.ResponseData!.Select(download => new ClassTypes
                {
                    ClassId = download.ClassID,
                    NameName = download.DownloadTypeName
                }).ToList();

                // Map response data to DownloadType
                var dTypes = downloadTypes.ResponseData!.Select(download => new DownloadType
                {
                    DTypeId = download.DType,
                    DTypeName = download.DownloadTypeName
                }).ToList();

                // Generate dropdowns for ClassTypes and DownloadTypes
                ViewBag.ClassList = classes.Select(c => new SelectListItem
                {
                    Value = c.ClassId,
                    Text = c.NameName
                }).ToList();

                ViewBag.DownloadTypes = dTypes.Select(d => new SelectListItem
                {
                    Value = d.DTypeId,
                    Text = d.DTypeName
                }).ToList();


            }
            else
            {
                // Handle failure scenarios, e.g., showing a default message if data is unavailable
                ViewBag.ClassList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "No Class available" }
                };
                ViewBag.DownloadTypes = new List<SelectListItem>
                {
                    new SelectListItem { Value = "", Text = "No Download Types available" }
                };

            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDownloads(string classId, string dtypeId)
        {
            try
            {
                var downloadsResponse = await downloadRepo.GetDownloadForStudent(classId, dtypeId);
                if (downloadsResponse.ResponseData.Count() <= 0)
                {
                    return Json(new Response
                    {
                        IsSuccess = true,
                        Status = 0,
                        Message = "No data found for the selected criteria."
                    });
                }
                return Json(downloadsResponse.ResponseData);
            }
            catch (Exception ex)
            {
                return Json(new Response
                {
                    IsSuccess = false,
                    Status = -1,
                    Message = "Technical Error",
                    Error = ex.Message
                });
            }
        }

            //generate youtube links for index page
            [HttpGet]
            public async Task<IActionResult> GetYoutubeLinks()
            {
                try
                {
                    var youtubeRes = await galleryRepo.GetAllYoutubeLinksAsync("10014");
                    if(youtubeRes.IsSuccess && youtubeRes.Status == 1)
                    {
                        if(youtubeRes.ResponseData!.Count() > 0)
                        {
                            var links = youtubeRes.ResponseData.Select(y =>
                            {
                                var videoId = ExtractVideoId(y.ThumbNail); // Extract video ID using helper method
                                return new YoutubeDTO
                                {
                                    Yid = y.PID,
                                    Title = y.Title,
                                    Description = y.Description,
                                    Date = y.PDate,
                                    Url = y.ThumbNail,
                                    VideoId = videoId // Assign video ID to a new property in DTO
                                };
                            });
                            return Json(links.Take(3).ToList());
                        }
                    }
                    return Json(new List<YoutubeDTO>());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            private string ExtractVideoId(string url)
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    return null;
                }

                try
                {
                    var uri = new Uri(url);

                    if (uri.Host.Contains("youtube.com") && uri.Query.Contains("v="))
                    {
                        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                        return query.Get("v");
                    }
                    else if (uri.Host.Contains("youtu.be"))
                    {
                        return uri.AbsolutePath.TrimStart('/');
                    }
                }
                catch
                {
                    // Log error if necessary
                }

                return null;
            }

            public async Task<IActionResult> AllYoutubeVideos()
            {
                try
                {
                    var youtubeRes = await galleryRepo.GetAllYoutubeLinksAsync("10014");
                    if (youtubeRes.IsSuccess && youtubeRes.Status == 1)
                    {
                        if (youtubeRes.ResponseData!.Count() > 0)
                        {
                            var links = youtubeRes.ResponseData.Select(y =>
                            {
                                var videoId = ExtractVideoId(y.ThumbNail); // Extract video ID using helper method
                                return new YoutubeDTO
                                {
                                    Yid = y.PID,
                                    Title = y.Title,
                                    Description = y.Description,
                                    Date = y.PDate,
                                    Url = y.ThumbNail,
                                    VideoId = videoId // Assign video ID to a new property in DTO
                                };
                            });
                            return View(links);
                        }
                    }
                    return View(new List<YoutubeDTO>());
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
