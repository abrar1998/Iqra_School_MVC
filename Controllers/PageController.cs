using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Plugins;
using SchoolProj.DAL.NoticeRepository;
using SchoolProj.DAL.PageRepository;
using SchoolProj.General;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;

namespace SchoolProj.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageRepo pageRepo;
        private readonly IHttpContextAccessor httpContextAccessor;

        public PageController(IPageRepo pageRepo, IHttpContextAccessor httpContextAccessor)
        {
            this.pageRepo = pageRepo;
            this.httpContextAccessor = httpContextAccessor;
        }

        [Route("AddPage")]
        [HttpGet]
        public IActionResult AddPage()
        {
            return View();
        }

        //Home Page Director message page id=20011
        //Home page About message page id = 20012

        [Route("AddPage")]
        [HttpPost]
        public async Task<IActionResult> AddPage(PageViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                string? filePath = null;
                //string? thumbnailPath = null
                try
                {
                    // Parse the incoming date, if valid use it, otherwise use current date
                    if (pvm.PagePic !=null)
                    {
                        // Validate the files (image and document) before proceeding
                        var fileValidationResult = FileUploadHelper.ValidateFiles(pvm.PagePic, null);
                        if (!fileValidationResult.IsSuccess)
                        {
                            // If validation fails, return the response with the error message
                            var response = new Response
                            {
                                IsSuccess = true,
                                Message = "Photo Not Valid, Please Add Valid Photo, File Size should be Less than or equal to 1 Mb.",
                                Status = 0,
                                ResponseData = null,
                                Error = fileValidationResult.Error // Send the error from file validation
                            };
                            //return Ok(response);
                            return Json(response);
                        }

                        // Handle file uploads (both image and document files)
                        var uploadResult = await FileUploadHelper.HandleFileUploadAsync(pvm.PagePic, null); //send image, and in documnet send null
                        filePath = uploadResult.imagePath;
                    }

                    // If files are uploaded successfully, create the Notice object
                    var page = new Page
                    {
                       PageName = pvm.PageName,
                       PageTitle = pvm.PageTitle,
                       PageUrl = pvm.PageUrl,
                       PagePic = filePath,
                       PageData = pvm.PageData,
                       PageHeading = pvm.PageHeading,
                       PageCat = "1"

                    };

                    // Save the notice to the database (assuming you have a repository method for this)
                    // Call CreateNoticeAsync and await the result
                    var result = await pageRepo.CreatePageAsync(page);

                    if (result.IsSuccess == true && result.Status == 1)
                    {
                        // Success case
                        var response = new Response
                        {
                            IsSuccess = true,
                            Message = result.Message,
                            Status = 1,
                            ResponseData = result, // Return the full notice with the generated NID
                            Error = null
                        };
                        return Json(response);
                    }
                    else
                    {
                        //if we fail to save data in our api, but file will be already uploaded in wwwroot folder
                        if (pvm.PagePic !=null)
                        {
                            //so when post action method failed , then it is zaroori to remove the uploaded file from wwwroot folder
                            FileUploadHelper.DeleteFile(filePath!);
                        }
                        // Error case: notice creation failed
                        var response = new Response
                        {
                            IsSuccess = false, // Make sure IsSuccess is false in case of failure
                            Message = result.Message,
                            Status = 0,
                            ResponseData = null,
                            Error = result.Error
                        };
                        return Json(response);
                    }


                }
                catch (Exception exp)
                {

                    if (pvm.PagePic != null)
                    {
                        //so when post action method failed , then it is zaroori to remove the uploaded file from wwwroot folder
                        FileUploadHelper.DeleteFile(filePath!);
                    }
                    var response = new Response
                    {
                        IsSuccess = false,
                        Message = "Technical Error",
                        Status = -1,
                        Error = exp.Message,
                        ResponseData = null,

                    };
                    //return Ok(response);
                    return Json(response);

                }

            }
            else
            {
                // Capture the model state errors if validation fails
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,  // Property Name
                        e => e.Value.Errors.Select(x => x.ErrorMessage).ToArray()  // Error Messages
                    );

                var response = new Response
                {
                    IsSuccess = true,
                    Message = "Model State Failed To Valid, Please check you are sending all fields",
                    Status = 0,
                    ResponseData = null,
                    Error = string.Join(", ", errors.SelectMany(e => e.Value))  // Concatenate all error messages
                };

                //return Ok(errorResponse); // use only in api's
                return Json(response);

            }
        }


        [HttpGet]
        public async Task<IActionResult> GetWebPageList()
        {
            try
            {
                var response = await pageRepo.GetAllPagesAsync();
                if (response.IsSuccess == true && response.Status == 1)
                {
                    var responseData = response.ResponseData;

                    return View(responseData);
                }
                return View();

            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditWebPage(string id)
        {
            try
            {
                //Home Page Director message page id=20011
                //Home page About message page id = 20012
                if(id == "20011")
                {
                    //we will redirect them to another eidt page
                    return RedirectToAction("EditHomePageData", new {id = id});
                }
                //remove old session
                HttpContext.Session.Remove("PagePhotoPath");
                var page = await pageRepo.GetPageAsync(id);
                //now first get PageEditViewModel because we are going to send different model on edit page
                var pEdit = pageRepo.GetPageEditViewModel(page.ResponseData!.FirstOrDefault()!);
                if (page.IsSuccess && page.Status == 1)
                {
                    var singlePge = page.ResponseData!.FirstOrDefault();
                    if (singlePge.PagePic != null)
                    {
                        HttpContext.Session.SetString("PagePhotoPath", singlePge.PagePic!);
                    }
                }
                if (pEdit.ExistingPagePic !=null)
                {
                    //generate url
                    pEdit.ExistingPagePic = FileUploadHelper.GenerateFileUrl(pEdit.ExistingPagePic, httpContextAccessor.HttpContext!.Request);
                }
                return View(pEdit);
            }
            catch(Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditHomePageData(string id)
        {
            try
            {
                //remove old session
                HttpContext.Session.Remove("PagePhotoPath");
                var page = await pageRepo.GetPageAsync(id);
                //now first get PageEditViewModel because we are going to send different model on edit page
                var pEdit = pageRepo.GetPageEditViewModel(page.ResponseData!.FirstOrDefault()!);
                if(page.IsSuccess && page.Status == 1)
                {
                    var singlePge = page.ResponseData!.FirstOrDefault();
                    if(singlePge.PagePic !=null)
                    {
                        HttpContext.Session.SetString("PagePhotoPath", singlePge.PagePic!);
                    }
                }
                if (pEdit.ExistingPagePic != null)
                {
                    //generate url
                    pEdit.ExistingPagePic = FileUploadHelper.GenerateFileUrl(pEdit.ExistingPagePic, httpContextAccessor.HttpContext!.Request);
                }
                return View(pEdit);
            }
            catch(Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditWebPage(PageEditModel pem)
        {
            if (ModelState.IsValid)
            {
                string? NewfilePath ="";
                string? ExistingFilePath = HttpContext.Session.GetString("PagePhotoPath");
                try
                {
                    // Parse the incoming date, if valid use it, otherwise use current date
                    if (pem.NewPagePic != null)
                    {
                        // Validate the files (image and document) before proceeding
                        var fileValidationResult = FileUploadHelper.ValidateFiles(pem.NewPagePic, null);

                        if (!fileValidationResult.IsSuccess)
                        {
                            // If validation fails, return the response with the error message
                            var response = new Response
                            {
                                IsSuccess = true,
                                Message = "Photo Not Valid, Please Add Valid Photo, File Size should be Less than or equal to 1 Mb.",
                                Status = 0,
                                ResponseData = null,
                                Error = fileValidationResult.Error // Send the error from file validation
                            };
                            //return Ok(response);
                            return Json(response);
                        }

                        // Handle file uploads (both image and document files)
                        var uploadResult = await FileUploadHelper.HandleFileUploadAsync(pem.NewPagePic, null); //send image, and in documnet send null
                        NewfilePath = uploadResult.imagePath;
                        //now delte old file path
                        FileUploadHelper.DeleteFile(ExistingFilePath!);
                    }

                    // Retrieve the existing photo path from the session
                    var existingPhotoPath = HttpContext.Session.GetString("PagePhotoPath");
                    // Determine the path to use for the page picture
                    var pagePicPath = string.IsNullOrEmpty(NewfilePath) ? existingPhotoPath : NewfilePath;
                    // If files are uploaded successfully, create the Notice object
                    var page = new Page
                    {
                        ID = pem.Id,
                        PageName = pem.PageName,
                        PageTitle = pem.PageTitle,
                        PageUrl = pem.PageUrl,
                        PagePic = pagePicPath,
                        PageData = pem.PageData,
                        PageHeading = pem.PageHeading,
                        PageCat = "1",
                        Download = null,
                        ThumbNail=null,

                    };

                    // Save the notice to the database (assuming you have a repository method for this)
                    // Call CreateNoticeAsync and await the result
                    var result = await pageRepo.UpdatePageAsync(page);

                    if (result.IsSuccess == true && result.Status == 1)
                    {
                        HttpContext.Session.Remove("PagePhotoPath");
                        // Success case
                        var response = new Response
                        {
                            IsSuccess = true,
                            Message = "Web Page Updated Successfully.",
                            Status = 1,
                            ResponseData = result, // Return the full notice with the generated NID
                            Error = null
                        };
                        return Json(response);
                    }
                    else
                    {
                        //if we fail to save data in our api, but file will be already uploaded in wwwroot folder
                        if (pem.NewPagePic != null)
                        {
                            //so when post action method failed , then it is zaroori to remove the uploaded file from wwwroot folder
                            FileUploadHelper.DeleteFile(NewfilePath!);
                        }
                        // Error case: notice creation failed
                        var response = new Response
                        {
                            IsSuccess = false, // Make sure IsSuccess is false in case of failure
                            Message = result.Message,
                            Status = 0,
                            ResponseData = null,
                            Error = result.Error
                        };
                        return Json(response);
                    }


                }
                catch (Exception exp)
                {

                    if (pem.NewPagePic != null)
                    {
                        //so when post action method failed , then it is zaroori to remove the uploaded file from wwwroot folder
                        FileUploadHelper.DeleteFile(NewfilePath!);
                    }
                    var response = new Response
                    {
                        IsSuccess = false,
                        Message = "Technical Error",
                        Status = -1,
                        Error = exp.Message,
                        ResponseData = null,

                    };
                    //return Ok(response);
                    return Json(response);

                }

            }
            else
            {
                // Capture the model state errors if validation fails
                var errors = ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,  // Property Name
                        e => e.Value.Errors.Select(x => x.ErrorMessage).ToArray()  // Error Messages
                    );

                var response = new Response
                {
                    IsSuccess = true,
                    Message = string.Join(", ", errors.SelectMany(e => e.Value)),
                    Status = 0,
                    ResponseData = null,
                    Error = null  // Concatenate all error messages
                };

                //return Ok(errorResponse); // use only in api's
                return Json(response);

            }
        }

    }
}
