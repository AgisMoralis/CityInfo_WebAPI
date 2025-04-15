using CityInfo.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        private readonly Dictionary<int, string> _dataResources;
        public FilesController(FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new ArgumentNullException("No available FileExtensionContentTypeProvider");
            _dataResources = new Dictionary<int, string>()
            {
                { 1, "GitIcon.png"},
                { 2, "GithubIcon.jpg"}
            };
        }

        [HttpGet("{fileId}")]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetFile(int fileId)
        {
            if (!_dataResources.TryGetValue(fileId, out var fileData))
            {
                return NotFound();
            }

            string filePath = string.Join("/", new string[] { "Data", fileData });
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            if (!_fileExtensionContentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                // Default arbitraty binary data content type
                contentType = "application/octet-stream";
            }

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, contentType, Path.GetFileName(filePath));
        }

        [HttpPost]
        public async Task<ActionResult> CreateFile(IFormFile file)
        {
            // Validate that the input file is not larger than 20Mb
            // and the application accepts .png images
            if (file.Length == 0 || file.Length > 20 * 1024 * 1024
                || file.ContentType != "image/png")
            {
                return BadRequest("Invalid input file (wrong size or file type)");
            }

            // Create a filepath to save the uploaded file to.
            // We should avoid using the "file.FileName" as it is from the input
            // as an attacker might provide in there information with malicious data
            // NOTE: Normally we should not store the upladed files in the directory of the application,
            // NOTE: but rathen in another safe location (another disk/machine) only for hosting those files
            var fileName = $"image_{Guid.NewGuid()}.png";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", fileName);

            // Use a FileStream to save the file into the selected path
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok("The file has been uploaded successfully");
        }
    }
}
