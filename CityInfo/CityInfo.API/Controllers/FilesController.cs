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
    }
}
