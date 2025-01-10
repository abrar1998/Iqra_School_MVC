using Microsoft.AspNetCore.Mvc;
using SchoolProj.DAL.NoticeRepository;
using SchoolProj.DAL.SliderRepository;
using SchoolProj.General;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;
using System.Runtime.ExceptionServices;
using System.Threading.RateLimiting;

namespace SchoolProj.Controllers
{
    public class SliderController : Controller
    {
        private readonly ISliderRepo sliderRepo;
        private readonly IHttpContextAccessor httpContextAccessor;

        public SliderController(ISliderRepo sliderRepo, IHttpContextAccessor httpContextAccessor)
        {
            this.sliderRepo = sliderRepo;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult AddSlider()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddSlider(SliderViewModel svm)
        {
            if (ModelState.IsValid)
            {
                string? filePath = null;
                try
                {
                    if (svm.SliderImage != null)
                    {
                        // Validate the files (image and document) before proceeding
                        var fileValidationResult = FileUploadHelper.ValidateFiles(svm.SliderImage, null);

                        if (!fileValidationResult.IsSuccess)
                        {
                            // If validation fails, return the response with the error message
                            var response = new Response
                            {
                                IsSuccess = true,
                                Message = "Image Not Valid, Please Add Valid image,  Image size should be less or equal to 1Mb",
                                Status = 0,
                                ResponseData = null,
                                Error = fileValidationResult.Error // Send the error from file validation
                            };
                            //return Ok(response);
                            return Json(response);
                        }

                        // Handle file uploads (both image and document files)
                        var uploadResult = await FileUploadHelper.HandleFileUploadAsync(svm.SliderImage, null);
                        filePath = uploadResult.imagePath;

                    }
                    var slide = new Slider
                    {
                        SLiderTitle = svm.SLiderTitle,
                        SliderDesc = svm.SliderDesc,
                        SliderImage = filePath,
                    };


                    // Save the slider to the database (assuming you have a repository method for this)
                    // Call CreateSlideeAsync and await the result
                    var result = await sliderRepo.CreateSliderAsync(slide);

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
                        if (svm.SliderImage !=null)
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
                    if (svm.SliderImage !=null)
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
        public async Task<IActionResult> ManageSlides()
        {
            try
            {
                var response = await sliderRepo.GetSlidesAsync();
                if (response.IsSuccess == true && response.Status == 1)
                {
                    var responseData = response.ResponseData;
                    //now generate image url for all slides
                    foreach(var slide in responseData!)
                    {
                        slide.SliderImage = FileUploadHelper.GenerateFileUrl(slide.SliderImage!, httpContextAccessor.HttpContext!.Request);
                    }

                    return View(responseData);
                }
                return View(new List<Slider>());

            }
            catch (Exception exp)
            {
                return BadRequest(exp.Message);
            }
        }

        public async Task<IActionResult> DeleteSlide(string id)
        {
            try
            {
                string fileToDelte = null;
                //first get current slide where we will get its image path becasue we have to delete that image from wwwroot folder also, if we dont do this , deleted slides images will be also stored in our app which will make our app heavy
                // var currentSlide = sliderRepo.GetSlideByIdAsync(id);
                var slides = await sliderRepo.GetSlidesAsync();
                var currentSlide = slides.ResponseData!.FirstOrDefault(s => s.SliderId == id);
                //now get current Slides path
                fileToDelte = currentSlide!.SliderImage!;

                var res = await sliderRepo.DeleteSlideByIdAsync(id);
                if (res.IsSuccess == true && res.Status == 1)
                {
                    //once our operation is completed , slide data is deleted from databse we will delete slide image from our wwwroot folers Files folder
                    FileUploadHelper.DeleteFile(fileToDelte);
                    var response = new Response
                    {
                        IsSuccess = true,
                        Message = "Slided Deleted Successfully!",
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
                        Message = "Failed to delete slide",
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

    }
}
