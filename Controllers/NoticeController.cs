    using Microsoft.AspNetCore.Mvc;
using SchoolProj.DAL.NoticeRepository;
using SchoolProj.General;
using SchoolProj.Models.DTO;
using SchoolProj.Models;
using SchoolProj.Models.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Globalization;

namespace SchoolProj.Controllers
{
    public class NoticeController : Controller
    {
        private readonly INoticeRepo noticeRepo;
        private readonly IHttpContextAccessor httpContextAccessor;

        public NoticeController(INoticeRepo noticeRepo, IHttpContextAccessor httpContextAccessor)
        {
            this.noticeRepo = noticeRepo;
            this.httpContextAccessor = httpContextAccessor;
        }

        
        [HttpGet]
        public IActionResult AddNotice()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNotice(NoticeViewModel noticeDTO)
        {
            if (ModelState.IsValid)
            {
                string? filePath = null;
                string? thumbnailPath = null;


                try
                {

                    // Parse the incoming date, if valid use it, otherwise use current date
                    var Date = DateTime.TryParse(noticeDTO.NDate, out var parsedDate) ? parsedDate : DateTime.Now;
                    int isFile = 0;
                    
                    if (noticeDTO.FilePath != null)
                    {
                        isFile = 1;
                    }


                    if (noticeDTO.ThumbNail != null || noticeDTO.FilePath != null)
                    {
                        // Validate the files (image and document) before proceeding
                        var fileValidationResult = FileUploadHelper.ValidateFiles(noticeDTO.ThumbNail, noticeDTO.FilePath);

                        if (!fileValidationResult.IsSuccess)
                        {
                            // If validation fails, return the response with the error message
                            var response = new Response
                            {
                                IsSuccess = true,
                                Message = "Files Not Valid, Please Add Valid Files, File Size should be Less than or equal to 500 Mb and Thumbnail size should be less or equal to 1Mb",
                                Status = 0,
                                ResponseData = null,
                                Error = fileValidationResult.Error // Send the error from file validation
                            };
                            //return Ok(response);
                            return Json(response);
                        }

                        // Handle file uploads (both image and document files)
                        var uploadResult = await FileUploadHelper.HandleFileUploadAsync(noticeDTO.ThumbNail, noticeDTO.FilePath);
                        filePath = uploadResult.documentPath;
                        thumbnailPath = uploadResult.imagePath;

                    }

                    // If files are uploaded successfully, create the Notice object
                    var notice = new Notice
                    {
                        Ncid = noticeDTO.Ncid,
                        NDate = Date,
                        Title = noticeDTO.Title,
                        Description = noticeDTO.Description,
                        FilePath = filePath,
                        IsFile = isFile,
                        UserName = noticeDTO.UserName,
                        ThumbNail = thumbnailPath,  // This will be null if no image was uploaded,
                        ActionType = 1

                    };
                    if (noticeDTO.ThumbNail == null)
                    {
                        notice.ThumbNail = "Images/Default_noticeicon-sm.jpg";
                    }
                 

                    // Save the notice to the database (assuming you have a repository method for this)
                    // Call CreateNoticeAsync and await the result
                    var result = await noticeRepo.CreateNoticeAsync(notice);

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
                        if(noticeDTO.FilePath !=null || noticeDTO.ThumbNail !=null)
                        {
                            //so when post action method failed , then it is zaroori to remove the uploaded file from wwwroot folder
                            FileUploadHelper.DeleteFile(filePath!);
                            FileUploadHelper.DeleteFile(thumbnailPath!);
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
                    if (noticeDTO.FilePath != null || noticeDTO.ThumbNail != null)
                    {
                        //so when post action method failed , then it is zaroori to remove the uploaded file from wwwroot folder
                        FileUploadHelper.DeleteFile(filePath!);
                        FileUploadHelper.DeleteFile(thumbnailPath!);
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

        
       

        //get list of all notices
        public async Task<IActionResult> GetNoticeList()
        {
            try
            {
                var response = await noticeRepo.GetNoticeListAsync();
                if(response.IsSuccess == true && response.Status == 1)
                {
                    //var responseData = response.ResponseData.OrderByDescending(n => n.NDate);
                    var responseData = response.ResponseData!
                                       .OrderByDescending(n => DateTime.Parse(n.NDate).Year)  // Sort by year, descending
                                       .ThenByDescending(n => DateTime.Parse(n.NDate).Month)  // Sort by month, descending
                                       .ThenByDescending(n => DateTime.Parse(n.NDate).Day);   // Sort by day, descending


                    return View(responseData);
                }
                return View(new List<NoticeDTO>());
                
            }
            catch(Exception exp) 
            {
                return BadRequest(exp.Message);
            }
        }

        //get notice by id
        public async Task<IActionResult> GetNotice(string id)
        {
            try
            {
                var response = await noticeRepo.GetNoticeByIdAsync(id);
                if(response.IsSuccess == true && response.Status ==1)
                {
                    var responseData = response.ResponseData;
                    if(responseData.ThumbNail !=null || responseData.FilePath !=null)
                    {
                       
                        //generate file url
                        responseData.FilePath = FileUploadHelper.GenerateFileUrl(responseData.FilePath!, httpContextAccessor.HttpContext!.Request);
                        //generate image url
                        responseData.ThumbNail = FileUploadHelper.GenerateFileUrl(responseData.ThumbNail!, httpContextAccessor.HttpContext!.Request);
                    }
                    return View(responseData);
                }
                else
                {
                    var notice = new NoticeDTO();
                    return View(notice);
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //get all notices for admin
        
        //delete notice
        public async Task<IActionResult> DeleteNotice(string id)
        {
            try
            {
                var filePatTodelete = "";
                var thumbNailTodelete = "";
                //first get notice current data so that we can delete there files if they exist in Files folder
                //get all notice
                var noticeResponse = await noticeRepo.GetNoticeListAsync();
                var noticeListDto = noticeResponse.ResponseData;
                var notice = noticeListDto!.Find(n => n.Nid == id);
                if (notice == null)
                {
                    var response = new Response
                    {
                        IsSuccess = true,
                        Message = "Notice Details not found",
                        Status = 0,
                        Error = null,
                        ResponseData = null,

                    };
                    return Ok(response);
                }
                else
                {
                    
                    if (notice.ThumbNail != null || notice.FilePath != null)
                    {
                        filePatTodelete = notice.FilePath;
                        if (notice.ThumbNail != "Images/Default_noticeicon-sm.jpg")
                        {           
                            thumbNailTodelete = notice.ThumbNail;
                        }
                        
                    }
                }

                var res = await noticeRepo.DeleteNoticeByIdAsync(notice,id);
                if(res.IsSuccess == true && res.Status ==1)
                {
                    //now delete file paths
                    if (notice.ThumbNail != null || notice.FilePath != null)
                    {
                        FileUploadHelper.DeleteFile(filePatTodelete!);
                        FileUploadHelper.DeleteFile(thumbNailTodelete!);
                    }
                   
                    var response = new Response
                    {
                        IsSuccess = true,
                        Message = res.Message,
                        Status = 1,
                        ResponseData = res.ResponseData,
                        Error = null 
                    };

                    //return Ok(errorResponse); // use only in api's
                    return Json(response);
                }
                else
                {
                    var response = new Response
                    {
                        IsSuccess = true,
                        Message = res.Message,
                        Status = 0,
                        ResponseData = res.ResponseData,
                        Error = null
                    };
                }
                return Ok();
            }
            catch(Exception ex)
            {
                var response = new Response
                {
                    IsSuccess = false,
                    Message = "Technical Error",
                    Status = -1,
                    ResponseData = null,
                    Error = ex.Message // Concatenate all error messages
                };

                //return Ok(errorResponse); // use only in api's
                return Json(response);
            }
        }



    }
}
