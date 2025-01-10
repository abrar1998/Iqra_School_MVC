using SchoolProj.Models;

namespace SchoolProj.General
{
    public class FileUploadHelper
    {

        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
        private static readonly string[] AllowedFileExtensions = { ".pdf", ".doc", ".docx" }; // Add Word extensions
        private static readonly IWebHostEnvironment webHostEnvironment;
        private const int GalleryMaxFileSize = 10 * 1024 * 1024; // 10 MB
        // private const int GalleryMaxDocumentFileSize = 10 * 1024 * 1024; // 10 MB
        private const int MaxFileSize = 1 * 1024 * 1024; // 1 MB
        private const int MaxDocumentFileSize = 500 * 1024 * 1024; // 500 MB


        //public FileUploadHelper(IWebHostEnvironment webHostEnvironment)
        //{
        //    this.webHostEnvironment = webHostEnvironment;
        //}

        public static ResponseDTO<object> ValidateGalleryPhoto(IFormFile? imageFile)
        {
            // Validate image file
            if (imageFile != null)
            {
                string imageExtension = Path.GetExtension(imageFile.FileName).ToLower();
                if (imageFile.Length > GalleryMaxFileSize)
                {
                    return new ResponseDTO<object>
                    {
                        IsSuccess = false,
                        Message = "false",
                        Status = 0,
                        ResponseData = null,
                        Error = "Image size exceeds the limit of 1Mb. Should be less than 10 MB."
                    };
                }

                if (!AllowedImageExtensions.Contains(imageExtension))
                {
                    return new ResponseDTO<object>
                    {
                        IsSuccess = false,
                        Message = "false",
                        Status = 0,
                        ResponseData = null,
                        Error = "Invalid image file extension. Allowed extensions are .jpg, .jpeg, .png, and .gif."
                    };
                }
            }
            return new ResponseDTO<object> { IsSuccess = true };
        }

        public static ResponseDTO<object> ValidateFiles(IFormFile? imageFile, IFormFile? documentFile)
        {
            // Validate image file
            if (imageFile != null)
            {
                string imageExtension = Path.GetExtension(imageFile.FileName).ToLower();
                if (imageFile.Length > MaxFileSize)
                {
                    return new ResponseDTO<object>
                    {
                        IsSuccess = false,
                        Message = "false",
                        Status = 0,
                        ResponseData = null,
                        Error = "Image size exceeds the limit of 1Mb. Should be less than 400KB."
                    };
                }

                if (!AllowedImageExtensions.Contains(imageExtension))
                {
                    return new ResponseDTO<object>
                    {
                        IsSuccess = false,
                        Message = "false",
                        Status = 0,
                        ResponseData = null,
                        Error = "Invalid image file extension. Allowed extensions are .jpg, .jpeg, .png, and .gif."
                    };
                }
            }

            // Validate document file (PDF or Word)
            if (documentFile != null)
            {
                string fileExtension = Path.GetExtension(documentFile.FileName).ToLower();
                if (!AllowedFileExtensions.Contains(fileExtension))
                {
                    return new ResponseDTO<object>
                    {
                        IsSuccess = false,
                        Message = "false",
                        Status = 0,
                        ResponseData = null,
                        Error = "Invalid document file extension. Allowed extensions are .pdf, .doc, .docx."
                    };
                }

                if (documentFile.Length > MaxDocumentFileSize)
                {
                    return new ResponseDTO<object>
                    {
                        IsSuccess = false,
                        Message = "false",
                        Status = 0,
                        ResponseData = null,
                        Error = "Document file size exceeds the limit of 500MB. Should be less than 10MB."
                    };
                }
            }

            return new ResponseDTO<object> { IsSuccess = true };
        }

        //send files whether its image or pdf and this method will save them inside he wwwroot folder
        public static async Task<string> SaveFileAsync(IFormFile file, string targetFolder) //before sending targetFolder make sure you have created folder inside wwwroot folder
        {
            string filename = Guid.NewGuid().ToString() + "-" + file.FileName;
             string fileDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", targetFolder);
            //string fileDir = Path.Combine(webHostEnvironment.WebRootPath, targetFolder);
            string filePath = Path.Combine(fileDir, filename);

            // Ensure the directory exists
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(targetFolder, filename);
        }
        public static async Task<string> UploadFileAsync(IFormFile file, string targetFolder) //before sending targetFolder make sure you have created folder inside wwwroot folder
        {
            string filename = Guid.NewGuid().ToString() + "-" + file.FileName;
            string fileDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", targetFolder);
            string filePath = Path.Combine(fileDir, filename);

            // Ensure the directory exists
            if (!Directory.Exists(fileDir))
            {
                Directory.CreateDirectory(fileDir);
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine(targetFolder, filename);
        }
        // Method to handle the upload process for both image and document files
        public static async Task<(string imagePath, string documentPath)> HandleFileUploadAsync(IFormFile? imageFile, IFormFile? documentFile)
        {
            string? imagePath = null;
            string? documentPath = null;

            // Upload image file if it exists
            if (imageFile != null && imageFile.Length > 0)
            {
                imagePath = await SaveFileAsync(imageFile, "Files");
            }

            // Upload document file if it exists
            if (documentFile != null && documentFile.Length > 0)
            {
                documentPath = await SaveFileAsync(documentFile, "Files");
            }

            return (imagePath, documentPath);
        }


        //public static string GenerateImageUrl(string relativePath, HttpRequest request) 
        //{
        //    var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
        //    return new Uri(new Uri(baseUrl), relativePath).ToString(); 
        //}


        //here we will get uniqur url , so that we need to store images on front side instead of that we will sent them to api and store in wwwroot folder
        //this mehtod will return file url and will be send to the front end
        public static string GenerateFileUrl(string relativePath, HttpRequest request)
        {
            // Ensure the relative path starts from the wwwroot folder
            var relativeUrl = Path.Combine(relativePath).Replace("\\", "/");
            var baseUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";
            return new Uri(new Uri(baseUrl), relativeUrl).ToString();
        }

        //delete file from wwwroot folder
        //public static void DeleteFile(string relativePath) 
        //{
        //    // Get the wwwroot folder path dynamically
        //    string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory());

        //    // Combine wwwroot and the relative path to get the full path
        //    string fullFilePath = Path.Combine(wwwRootPath, relativePath);

        //    // Extract the folder path (e.g., "wwwroot/Files")
        //    string folderPath = Path.GetDirectoryName(fullFilePath);

        //    if (System.IO.File.Exists(fullFilePath))
        //    {
        //        // Delete the file
        //        System.IO.File.Delete(fullFilePath);
        //        Console.WriteLine("File deleted successfully.");
        //    }
        //}

        public static void DeleteFile(string relativePath)
        {
            try
            {
                // Combine to get the full path to the file
                string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                string fullFilePath = Path.Combine(wwwRootPath, relativePath);

                // Check if the file exists
                if (System.IO.File.Exists(fullFilePath))
                {
                    // Delete the file
                    System.IO.File.Delete(fullFilePath);
                    Console.WriteLine("File deleted successfully.");
                }
                else
                {
                    Console.WriteLine("File not found: " + fullFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error deleting file: " + ex.Message);
            }
        }


    }
}
