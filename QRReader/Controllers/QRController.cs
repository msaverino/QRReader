using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.IO;
using System.Net;

namespace QRReader.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QRController : ControllerBase
    {

        private readonly ILogger<QRController> _logger;

        public QRController(ILogger<QRController> logger)
        {
            _logger = logger;
        }
        private readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".tiff", ".bmp" };

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("QR Reader API");
        }

        // Allow the user to upload JPEG, PNG, GIF, TIFF, BMP, and PDF files
        [HttpPost]
        // Max of 4 MB
        [RequestSizeLimit(4000000)]
        // QR/Upload
        [Route("Upload")]
        public IActionResult Post(IFormFile file)
        {

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (!AllowedExtensions.Contains(fileExtension))
            {
                _logger.Log(LogLevel.Information, $"The user tried to upload a file with an invalid extension. Their IP Address is {HttpContext.Connection.RemoteIpAddress}");
                // Get all the supported extensions in a string making a sentence.
                string supportedFiles = string.Join(", ", AllowedExtensions);
                // add the word "and" before the last extension
                supportedFiles = supportedFiles.Insert(supportedFiles.LastIndexOf(",") + 1, " and");
                return BadRequest(new string[] { $"Invalid file type. The supported file types are {supportedFiles}" });

            }
            // Convert the file to a byte array
            byte[] bytes = new byte[file.Length];
            file.OpenReadStream().Read(bytes, 0, (int)file.Length);
            // Process the QR code
            return Ok(QRProcessor.Process(bytes));

        }

        // Createa  new route for Binary
        [HttpPost]
        // Max of 4 MB
        [RequestSizeLimit(4000000)]
        // QR/Binary
        [Route("Binary")]
        public IActionResult Post()
        {
            using (var stream = new MemoryStream())
            {
                Request.Body.CopyToAsync(stream).Wait();

                if (stream.Length == 0)
                {
                    return BadRequest(new string[] { "No QR Code found" });
                }
                return Ok(QRProcessor.Process(stream.ToArray()));
            }
        }
    }
}
