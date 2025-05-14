using Asp.Versioning;
using CityInfo.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace CityInfo.API.Controllers
{
    [ApiController]
    //[Authorize]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion(1)]
    [ApiVersion(2)]
    public class FilesController : ControllerBase
    {
        // Private members
        private readonly ILogger<FilesController> _logger;
        private readonly FileExtensionContentTypeProvider _fileExtensionContentTypeProvider;
        private readonly Dictionary<int, string> _dataResources;

        public FilesController(ILogger<FilesController> logger, FileExtensionContentTypeProvider fileExtensionContentTypeProvider)
        {
            _logger = logger;
            _fileExtensionContentTypeProvider = fileExtensionContentTypeProvider ?? throw new ArgumentNullException("No available FileExtensionContentTypeProvider");
            _dataResources = new Dictionary<int, string>()
            {
                { 1, "GitIcon.png"},
                { 2, "GithubIcon.jpg"}
            };
        }

        /// <summary>
        /// Get a specific file by an Id
        /// </summary>
        /// <param name="fileId">The Id of the file to be returned</param>
        /// <returns>A file (image)</returns>
        /// <response code="200">The file (image) with the requested Id</response>
        [HttpGet("{fileId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetFile(int fileId)
        {
            try
            {
                if (!_dataResources.TryGetValue(fileId, out var fileData))
                {
                    _logger.LogError($"The file with id {fileId} was not found in the resources, when trying to access the files.");
                    return NotFound();
                }

                string filePath = string.Join("/", new string[] { "Data", fileData });
                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogError($"The local path of file with id {fileId} was not found, when trying to access the files.");
                    return NotFound();
                }

                if (!_fileExtensionContentTypeProvider.TryGetContentType(filePath, out var contentType))
                {
                    _logger.LogInformation($"The content type of the file with id {fileId} could not be identified by the {nameof(FileExtensionContentTypeProvider)}, when trying to access the files.");
                    _logger.LogInformation($"The default content type \"application/octet-stream\" will be used.");
                    // Default arbitraty binary data content type
                    contentType = "application/octet-stream";
                }

                var bytes = System.IO.File.ReadAllBytes(filePath);
                return File(bytes, contentType, Path.GetFileName(filePath));
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to access the file with id {fileId}.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }

        /// <summary>
        /// Uploads a new file into the server
        /// </summary>
        /// <param name="file">The new file to be uploaded</param>
        /// <returns>A confirmation 200 Status message that the upload was successful</returns>
        /// <response code="200">A confirmation message that the new file was successfully uploaded</response>
        [HttpPost]
        [ApiVersion(0.1, Deprecated = true)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> CreateFile(IFormFile file)
        {
            try
            {
                // Validate that the input file is not larger than 20Mb
                // and the application accepts .png images
                if (file.Length == 0 || file.Length > 20 * 1024 * 1024
                    || file.ContentType != "image/png")
                {
                    _logger.LogError($"The input file is invalid (wrong size or file type), when trying to upload a new file.");
                    return BadRequest("Invalid input file (wrong size or file type)");
                }

                // Create a filepath to save the uploaded file to.
                // We should avoid using the "file.FileName" as it is from the input
                // as an attacker might provide in there information with malicious data
                // NOTE: Normally we should not store the upladed files in the directory of the application,
                // NOTE: but rathen in another safe location (another disk/machine) only for hosting those files
                var fileName = $"image_{Guid.NewGuid()}.png";
                var filePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "Data", fileName);

                // Use a FileStream to save the file into the selected path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return Ok("The file has been uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Unexpected exception occurred when trying to upload a new file.", ex);
                return StatusCode(500, $"A problem occurred while handling your request.");
            }
        }
    }
}
