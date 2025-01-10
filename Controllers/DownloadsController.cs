using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolProj.DAL.DownloadRepository;
using SchoolProj.DAL.NoticeRepository;
using SchoolProj.DAL.PageRepository;
using SchoolProj.General;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;
using System.Globalization;

namespace SchoolProj.Controllers
{
    public class DownloadsController : Controller
    {
        private readonly IDownloadRepo downloadRepo;
        private readonly IHttpContextAccessor httpContextAccessor;

        public DownloadsController(IDownloadRepo downloadRepo, IHttpContextAccessor httpContextAccessor)
        {
            this.downloadRepo = downloadRepo;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> AddDownload()
        {
            await LoadDownlaodAndClassTypeForDropDown();
            return View();
        }

        //add download
        [HttpPost]
        public async Task<IActionResult> AddDownload(DownloadViewModel dvm)
        { 

            if (ModelState.IsValid)
            {
                await LoadDownlaodAndClassTypeForDropDown();
                string? filePath = null;

                try
                {
                    
                    // Parse and format the date
                    //string formattedDate;
                    // Force date format for production
                    string formattedDate = dvm.DDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                    // Parse the incoming date, if valid use it, otherwise use current datee
                    if (dvm.FilePath != null)
                    {
                        // Validate the files (image and document) before proceeding
                        var fileValidationResult = FileUploadHelper.ValidateFiles(null, dvm.FilePath); //first parameter is null because it it accepts image, second parameter accepts file

                        if (!fileValidationResult.IsSuccess)
                        {
                            // If validation fails, return the response with the error message
                            var response = new Response
                            {
                                IsSuccess = true,
                                Message = fileValidationResult.Error,
                                Status = 0,
                                ResponseData = null,
                                Error = null// Send the error from file validation
                            };
                            //return Ok(response);
                            return Json(response);
                        }
                        // Handle file uploads (both image and document files)
                        var uploadResult = await FileUploadHelper.HandleFileUploadAsync(null, dvm.FilePath); //send image, and in documnet send null
                        filePath = uploadResult.documentPath; // here we need document path not image path

                    }

                    // If files are uploaded successfully, create the Notice object

                    var download = new Downloads
                    {
                        Title = dvm.Title,
                        Description = dvm.Description,
                        FilePath = filePath,
                        DType = dvm.DType,
                        ClassID = dvm.ClassID,
                        //DDate = ParseDate(dvm.DDate.ToString("dd-MM-yyyy")), // Convert DateTime to string first
                        DDate = formattedDate,
                        ActionType = 1,
                        DCounter = "1"
                    };

                    //save downloads , send to api
                    var result = await downloadRepo.CreateDownloadsAsync(download);

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
                    else if(result.IsSuccess && result.Status == 0)
                    {
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
                        if (dvm.FilePath != null)
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

                    if (dvm.FilePath != null)
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
                await LoadDownlaodAndClassTypeForDropDown();
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


        // Parsing and formatting the date
        //public static string ParseDate(string dateString)
        //{
        //    DateTime parsedDate = DateTime.ParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //    return parsedDate.ToString("yyyy-MM-dd"); // Returns the date as a string
        //}

        //public static string ParseDate(string dateString)
        //{
        //    try
        //    {
        //        DateTime parsedDate = DateTime.ParseExact(dateString,
        //                                                  "dd-MM-yyyy HH:mm:ss",
        //                                                  CultureInfo.InvariantCulture);
        //        return parsedDate.ToString("yyyy-MM-dd");
        //    }
        //    catch (FormatException)
        //    {
        //        throw new FormatException($"Invalid date format: {dateString}");
        //    }
        //}

        public static string ParseDate(string dateString)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(
                    dateString,
                    "dd-MM-yyyy",
                    CultureInfo.InvariantCulture
                );
                return parsedDate.ToString("yyyy-MM-dd"); // Format for database storage
            }
            catch (FormatException)
            {
                throw new FormatException($"Invalid date format: {dateString}. Expected format: 'dd-MM-yyyy'.");
            }
        }





        [HttpGet]
        public async Task<IActionResult> DownLoadList()
        {
            try
            {
                var downloadsResponse = await downloadRepo.GetAllDownloadsAsync();
                if(downloadsResponse.IsSuccess && downloadsResponse.Status == 1)
                {
                    var downloads = downloadsResponse.ResponseData;
                    //generate file url
                    foreach (var download in downloads!)
                    {
                        download.FilePath = FileUploadHelper.GenerateFileUrl(download.FilePath!, httpContextAccessor.HttpContext!.Request);
                    }
                    return View(downloads);
                    
                }
                else
                {
                    return View(new List<Downloads>());
                }
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> DeleteDownload(string dId)
        {
            try
            {
                var filePatTodelete = "";
                //first get notice current data so that we can delete there files if they exist in Files folder
                //get all notice
                var downloadResponse = await downloadRepo.GetDownloadAsync(dId);
                var download = downloadResponse.ResponseData.First();
                if (download == null)
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

                    if (download.FilePath != null)
                    {
                        filePatTodelete = download.FilePath;  
                    }
                }
                download.ActionType = 2;
                var res = await downloadRepo.DeleteDownloadAsync(download);
                if (res.IsSuccess == true && res.Status == 1)
                {
                    //now delete file paths
                    if (download.FilePath != null)
                    {
                        FileUploadHelper.DeleteFile(filePatTodelete!);
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
            catch (Exception ex)
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







    }
}
