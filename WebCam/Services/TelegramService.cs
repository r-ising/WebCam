using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RxTelegram.Bot;
using RxTelegram.Bot.Interface.BaseTypes.Requests.Attachments;
using WebCam.Options;

namespace WebCam.Services
{
    public class TelegramService
    {
        private readonly TelegramOptions _telegramOptions;

        public TelegramService(IOptions<TelegramOptions> telegramOptions) =>
            _telegramOptions = telegramOptions.Value;

        public async Task SendImage(Stream image)
        {
            var telegram = new TelegramBot(_telegramOptions.ApiKey);

            await telegram.SendPhoto(new SendPhoto
                                     {
                                         ChatId = _telegramOptions.Target,
                                         Photo = new InputFile(image, "image.jpg")
                                     });
        }
    }
}
