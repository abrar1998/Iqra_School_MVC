using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolProj.DAL.GalleryRepository;
using SchoolProj.DAL.SliderRepository;
using SchoolProj.General;
using SchoolProj.Models.Domain;
using SchoolProj.Models.DTO;

namespace SchoolProj.Controllers
{
    public class GalleryController : Controller
    {
        private readonly IGalleryRepo galleryRepo;
        private readonly IHttpContextAccessor httpContextAccessor;

        public GalleryController(IGalleryRepo galleryRepo, IHttpContextAccessor httpContextAccessor)
        {
            this.galleryRepo = galleryRepo;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public async Task<IActionResult> ManageCategory()
        {
            try
            {
                var catResponse = await galleryRepo.GetPhotoCategoriesAsync();
                if(catResponse.IsSuccess == true && catResponse.Status == 1)
                {
                    //now generate url of each file
                    if(catResponse.ResponseData.Count() > 0)
                    {
                        catResponse.ResponseData = galleryRepo.GeneratePhotosUrl(catResponse.ResponseData);
                    }
                    return View(catResponse.ResponseData!.OrderByDescending(c=>c.PCIDFK));
                }
                else if(catResponse.IsSuccess == true && catResponse.Status == 0)
                {
                    return View(catResponse.ResponseData);
                }
                else if(catResponse.IsSuccess == false && catResponse.Status == -1)
                {
                    return BadRequest(catResponse.Error);
                }
                return View(new List<Photos>());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoCategory(CategoryViewModel cvm)
        {

            if (ModelState.IsValid)
            {
                string? filePath = null;
                try
                {
                    if (cvm.Thumbnail != null)
                    {
                        // Validate the files (image and document) before proceeding
                        var fileValidationResult = FileUploadHelper.ValidateGalleryPhoto(cvm.Thumbnail);

                        if (!fileValidationResult.IsSuccess)
                        {
                            // If validation fails, return the response with the error message
                            var response = new Response
                            {
                                IsSuccess = true,
                                Message = fileValidationResult.Error,
                                Status = 0,
                                ResponseData = null,
                                Error = fileValidationResult.Error // Send the error from file validation
                            };
                            //return Ok(response);
                            return Json(response);
                        }

                        // Handle file uploads (both image and document files)
                        filePath = await FileUploadHelper.UploadFileAsync(cvm.Thumbnail, "Gallery"); //Gallery here is the folder we have created in wwwroot folder where we will store all Photos, Make sure you have created

                    }

                    var category = new Photos
                    {
                        ThumbNail = filePath,
                        PhotoCatName = cvm.CategoryName,
                        CatDate = cvm.CategoryDate,
                    };


                    // Save the category to the database (assuming you have a repository method for this)

                    var result = await galleryRepo.CreatePhotoCategoryAsync(category);

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
                        if (cvm.Thumbnail != null)
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
                    if (cvm.Thumbnail != null)
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


        public async Task<IActionResult> DeletePhotoCategory(string id)
        {
            try
            {
                string fileToDelte = null;
                //first get current slide where we will get its image path becasue we have to delete that image from wwwroot folder also, if we dont do this , deleted slides images will be also stored in our app which will make our app heavy
                // var currentSlide = sliderRepo.GetSlideByIdAsync(id);
                var categories = await galleryRepo.GetPhotoCategoriesAsync();
                var currentCategory = categories.ResponseData!.FirstOrDefault(s => s.PCIDFK == id);
                //now get current Slides path
                fileToDelte = currentCategory!.ThumbNail!;

                var res = await galleryRepo.DeletePhotoCategoryAsync(currentCategory);
                if (res.IsSuccess == true && res.Status == 1)
                {
                    //once our operation is completed , slide data is deleted from databse we will delete slide image from our wwwroot folers Files folder
                    FileUploadHelper.DeleteFile(fileToDelte);
                    var response = new Response
                    {
                        IsSuccess = true,
                        Message = "Photo Category Deleted Successfully!",
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
                        Message = "Failed to delete category",
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

        [HttpGet]
        public async Task<IActionResult> GetPhotoCategory(string id)
        {
            var categoryRes = await galleryRepo.GetPhotoCategoriesAsync();

            if (categoryRes.ResponseData.Count() <= 0)
            {
                return Json(new { isSuccess = false, message = "Category not found." });
            }

            var category = categoryRes.ResponseData.FirstOrDefault(c => c.PCIDFK == id);
            //now generate url
            category.ThumbNail = FileUploadHelper.GenerateFileUrl(category.ThumbNail, httpContextAccessor.HttpContext!.Request);

            return Json(new
            {
                isSuccess = true,
                status = 1,
                data = new
                {
                    id = category.PCIDFK,
                    name = category.PhotoCatName,
                    
                    thumbnailUrl = Url.Content(category.ThumbNail) // Adjust this as per your logic
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> EditPhotoCategory(CategoryViewEditModel model)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return Json(new { isSuccess = false, message = "Invalid data." });
                }

                // Fetch categories from the repository
                var categoryRes = await galleryRepo.GetPhotoCategoriesAsync();
                if (categoryRes.ResponseData == null || !categoryRes.ResponseData.Any())
                {
                    return Json(new { isSuccess = false, message = "Category not found." });
                }

                // Get the category to be edited
                var category = categoryRes.ResponseData.FirstOrDefault(c => c.PCIDFK == model.CategoryId);
                if (category == null)
                {
                    return Json(new { isSuccess = false, message = "Category not found." });
                }

                // Save the old thumbnail path
                string oldThumbnail = category.ThumbNail ?? string.Empty;
                string newThumbnail = oldThumbnail;

                // If a new thumbnail is provided, process it
                if (model.Thumbnail != null)
                {
                    // Validate the uploaded file
                    var fileValidationResult = FileUploadHelper.ValidateGalleryPhoto(model.Thumbnail);
                    if (!fileValidationResult.IsSuccess)
                    {
                        return Json(new
                        {
                            isSuccess = false,
                            message = "Image Not Valid. Please provide a valid image. Image size should be ≤ 10MB.",
                            error = fileValidationResult.Error
                        });
                    }

                    // Upload the new thumbnail
                    newThumbnail = await FileUploadHelper.UploadFileAsync(model.Thumbnail, "Gallery");
                }

                // Update the category details
                category.ThumbNail = newThumbnail;
                category.PhotoCatName = model.CategoryName;
                category.UpdatedOn = model.CategoryDate;
                category.PCIDFK = model.CategoryId;

                // Save the updated category
                var updateResult = await galleryRepo.UpdatePhotoCategoryAsync(category);
                if (updateResult.IsSuccess && updateResult.Status == 1)
                {
                    // Delete the old thumbnail if a new one was uploaded
                    if (!string.IsNullOrEmpty(oldThumbnail) && oldThumbnail != newThumbnail)
                    {
                        FileUploadHelper.DeleteFile(oldThumbnail);
                    }

                    return Json(new { isSuccess = true, status = 1, message = "Category updated successfully." });
                }

                return Json(new { isSuccess = false, status = 0, message = updateResult.Message });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, status = -1, message = ex.Message });
            }
        }


        //images section

        [HttpGet]
        public async Task<IActionResult> ManageImages()
        {
            try
            {
                var catRes = await galleryRepo.GetPhotoCategoriesAsync();
                if (catRes.ResponseData!.Count() <= 0)
                {
                    ViewBag.Categories = new List<SelectListItem>
                    {
                        new SelectListItem { Value = "", Text = "No Category available" }
                    };
                }
                else
                {
                    ViewBag.Categories = catRes.ResponseData!.Where(c => c.PCIDFK != "10014").Select(d => new SelectListItem
                    {
                        Value = d.PCIDFK.ToString(),
                        Text = d.PhotoCatName
                    }).ToList();
                }
                return View();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }


        public async Task<IActionResult> AddPhoto(PhotoViewModel pvm)
        {
            if (ModelState.IsValid)
            {
                string? filePath = null;
                try
                {
                    if (pvm.Image != null)
                    {
                        // Validate the files (image and document) before proceeding
                        var fileValidationResult = FileUploadHelper.ValidateGalleryPhoto(pvm.Image);

                        if (!fileValidationResult.IsSuccess)
                        {
                            // If validation fails, return the response with the error message
                            var response = new Response
                            {
                                IsSuccess = true,
                                Message = "Image Not Valid, Please Add Valid image,  Image size should be less or equal to 10Mb",
                                Status = 0,
                                ResponseData = null,
                                Error = fileValidationResult.Error // Send the error from file validation
                            };
                            //return Ok(response);
                            return Json(response);
                        }

                        // Handle file uploads (both image and document files)
                        filePath = await FileUploadHelper.UploadFileAsync(pvm.Image, "Gallery"); //Gallery here is the folder we have created in wwwroot folder where we will store all Photos, Make sure you have created

                    }
                    var photo = new Photos
                    {
                        PhotoPath = filePath!,
                        Title = pvm.Title ?? "",
                        Description = pvm.Description ?? "",
                        PDate = pvm.PhotoDate,
                        PCIDFK = pvm.PCIDFK,
                    };

                    // Save the category to the database (assuming you have a repository method for this)

                    var result = await galleryRepo.CreatePhotoAsync(photo);

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
                        if (pvm.Image != null)
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
                    if (pvm.Image != null)
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


        public async Task<IActionResult> GetImagesByAlbum(string albumId)
        {
            try
            {
                // Validate the input albumId
                if (string.IsNullOrEmpty(albumId))
                {
                    return Json(new { success = false, message = "Album ID is required." });
                }

                // Fetch images by albumId
                var imgRes = await galleryRepo.GetImagesByCategoryIdAsync(albumId);

                // Check if the result is successful and contains images
                if (imgRes.IsSuccess && imgRes.Status == 1)
                {
                    // Prepare image data for the client
                    var images = imgRes.ResponseData!.Select(img => new
                    {
                        Id = img.PID,                  // Assuming `PID` is the ID of the image
                        ImageUrl = img.PhotoPath,      // Assuming `PhotoPath` contains the image URL
                        UploadDate = img.PDate // Format date
                    }).ToList();

                    // Return JSON response with images
                    return Json(new { success = true, status = 1, images });
                }

                // Handle scenarios where the response is successful but no images are found
                if (imgRes.IsSuccess && imgRes.Status == 0)
                {
                    return Json(new { success = true, status = 0, message = imgRes.Message ?? "No images found." });
                }
                if (!imgRes.IsSuccess && imgRes.Status == 0)
                {
                    return Json(new { success = true, status = 0, message = imgRes.Message ?? "No images found." });
                }

                // General fallback for unsuccessful responses
                return Json(new { success = false, message = imgRes.Message ?? "An error occurred while fetching images." });
            }
            catch (Exception ex)
            {
                // Log the exception (you can add logging logic here)
                // Return an error response
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        public async Task<IActionResult> DeletePhoto(string id)
        {
            try
            {
                string fileToDelte = null;
                //first get current slide where we will get its image path becasue we have to delete that image from wwwroot folder also, if we dont do this , deleted slides images will be also stored in our app which will make our app heavy
                // var currentSlide = sliderRepo.GetSlideByIdAsync(id);
                var photo = await galleryRepo.GetPhotoIdAsync(id);
                var photoTodelete = photo.ResponseData.FirstOrDefault(p=>p.PID == id);
                if(photo.IsSuccess && photo.Status == 1)
                {
                    //now get current Slides path
                    fileToDelte = photoTodelete!.PhotoPath;
                    var res = await galleryRepo.DeletePhotoAsync(photoTodelete);
                    if (res.IsSuccess == true && res.Status == 1)
                    {
                        //once our operation is completed , slide data is deleted from databse we will delete slide image from our wwwroot folers Files folder
                        FileUploadHelper.DeleteFile(fileToDelte);
                        var responsedto= new Response
                        {
                            IsSuccess = true,
                            Message = "Photo Deleted Successfully!",
                            Status = 1,
                            ResponseData = res.ResponseData,
                            Error = null
                        };

                        //return Ok(errorResponse); // use only in api's
                        return Json(responsedto);
                    }
                    else
                    {
                        var responsedto = new Response
                        {
                            IsSuccess = true,
                            Message = "Failed to delete photo",
                            Status = 0,
                            ResponseData = res.ResponseData,
                            Error = null
                        };
                        return Json(responsedto);
                    }
                }
                var response = new Response
                {
                    IsSuccess = true,
                    Message = "not found",
                    Status = 0,
                    ResponseData = null,
                    Error = null
                };
                return Json(response);
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

        //youtube section

        public async Task<IActionResult> ManageYoutube()
        {
            try
            {
                var youtubeRes = await galleryRepo.GetAllYoutubeLinksAsync("10014");

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

        //Add youtube video link
        [HttpPost]
        public async Task<IActionResult> AddYoutubeLink(YoutubeDTO yDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var photo = new Photos
                    {
                        // this id will be same for every youtube link you add because we have
                        // created youtube category Youtube under which we will add all youtube
                        // links in the photos tabe and later we ill fetch all links under this id
                        PCIDFK = "10014", 
                        ThumbNail = yDto.Url,
                        Title = yDto.Title,
                        Description = yDto.Description,
                        PDate = DateTime.Now.ToString("dd-MM-yyyy"),
                        PhotoPath = "This Is Youtube Video Link"
                    };

                    // Save the category to the database (assuming you have a repository method for this)

                    var result = await galleryRepo.CreatePhotoAsync(photo);

                    if (result.IsSuccess == true && result.Status == 1)
                    {
                        // Success case
                        var response = new Response
                        {
                            IsSuccess = true,
                            Message = "Youtube Link Added",
                            Status = 1,
                            ResponseData = result, // Return the full notice with the generated NID
                            Error = null
                        };
                        return Json(response);
                    }
                    else if(!result.IsSuccess && result.Status == -1)
                    {
                        var response = new Response
                        {
                            IsSuccess = false, // Make sure IsSuccess is false in case of failure
                            Message = result.Message,
                            Status = -1,
                            ResponseData = null,
                            Error = result.Error
                        };
                        return Json(response);
                    }
                    else
                    {
                        var response = new Response
                        {
                            IsSuccess = true, // Make sure IsSuccess is false in case of failure
                            Message = result.Message,
                            Status = -0,
                            ResponseData = null,
                            Error = null
                        };
                        return Json(response);
                    }
                }
                catch (Exception exp)
                {
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

        public async Task<IActionResult> DeleteYoutubeVideoLink(string id)
        {
            try
            {
                var photo = await galleryRepo.GetPhotoIdAsync(id);
                var photoTodelete = photo.ResponseData.FirstOrDefault(p => p.PID == id);
                if (photo.IsSuccess && photo.Status == 1)
                {
                    var res = await galleryRepo.DeletePhotoAsync(photoTodelete!);
                    if (res.IsSuccess == true && res.Status == 1)
                    {
                        var responsedto = new Response
                        {
                            IsSuccess = true,
                            Message = "Video Link Deleted Successfully!",
                            Status = 1,
                            ResponseData = res.ResponseData,
                            Error = null
                        };

                        //return Ok(errorResponse); // use only in api's
                        return Json(responsedto);
                    }
                    else
                    {
                        var responsedto = new Response
                        {
                            IsSuccess = true,
                            Message = "Failed to delete video link",
                            Status = 0,
                            ResponseData = res.ResponseData,
                            Error = null
                        };
                        return Json(responsedto);
                    }
                }
                var response = new Response
                {
                    IsSuccess = true,
                    Message = "not found",
                    Status = 0,
                    ResponseData = null,
                    Error = null
                };
                return Json(response);
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
