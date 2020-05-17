using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using WebCam.Options;
using Xabe.FFmpeg;

namespace WebCam.Services
{
    public class WebCamService
    {
        private readonly WebCamOptions _webCamOptions;

        public WebCamService(IOptions<WebCamOptions> webCamOptions)
        {
            _webCamOptions = webCamOptions.Value;
        }

        public async Task<byte[]> GetCurrentImage()
        {
            // add jpg suffix because of ffmpeg
            var tempFile = Path.GetTempFileName() + ".jpg";
            var ffmpeg =
                await FFmpeg.Conversions.FromSnippet.Snapshot(_webCamOptions.Stream, tempFile,
                                                              TimeSpan.Zero);
            ffmpeg.AddParameter("-rtsp_transport tcp", ParameterPosition.PreInput);
            ffmpeg.SetOverwriteOutput(true);
            await ffmpeg.Start();

            try
            {
                return await File.ReadAllBytesAsync(tempFile);
            }
            finally
            {
                File.Delete(tempFile);
            }
        }
    }
}
