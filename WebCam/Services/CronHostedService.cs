using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebCam.Services
{
    public class CronHostedService : BackgroundService
    {
        private readonly ILogger<CronHostedService> _logger;
        private readonly WebCamService _webCamService;
        private readonly TelegramService _telegramService;
        private DateTime? _lastRun;

        public CronHostedService(
            ILogger<CronHostedService> logger,
            WebCamService webCamService,
            TelegramService telegramService)
        {
            _logger = logger;
            _webCamService = webCamService;
            _telegramService = telegramService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(CronHostedService)} is running.");
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await DoWork();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing");
                }

                await Task.Delay(10000, stoppingToken);
            }
        }

        private async Task DoWork()
        {
            // work only at 9 am
            if (DateTime.Now.Hour != 9)
            {
                _logger.Log(LogLevel.Trace, "Wrong time");
                return;
            }

            // work already done on this day?
            if (_lastRun != null &&
                _lastRun.Value.Day == DateTime.Now.Day)
            {
                _logger.Log(LogLevel.Trace, "Nothing to do");
                return;
            }

            _logger.Log(LogLevel.Information, "Capture image and send via telegram");
            var image = await _webCamService.GetCurrentImage();
            var stream = new MemoryStream(image);
            await _telegramService.SendImage(stream);
            _lastRun = DateTime.Now;
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(CronHostedService)} is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}
