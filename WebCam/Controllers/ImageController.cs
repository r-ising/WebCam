using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebCam.Services;

namespace WebCam.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ILogger<ImageController> _logger;

        private readonly WebCamService _webCamService;

        public ImageController(
            ILogger<ImageController> logger,
            WebCamService webCamService)
        {
            _logger = logger;
            _webCamService = webCamService;
        }

        [HttpGet]
        public async Task<FileResult> Get()
        {
            _logger.Log(LogLevel.Information, "Get Image via Controller");
            var image = await _webCamService.GetCurrentImage();
            var stream = new MemoryStream(image);
            var cd = new System.Net.Mime.ContentDisposition
                     {
                         FileName = "image.jpg",
                         Inline = true
                     };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            return File(stream, "image/jpg");
        }
    }
}
