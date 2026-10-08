using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SkillStack.Infrastructure.Services
{
    public class TelegramNotifierService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _cfg;
        private readonly ILogger<TelegramNotifierService> _log;

        public TelegramNotifierService(HttpClient http, IConfiguration cfg, ILogger<TelegramNotifierService> log)
        {
            _http = http;
            _cfg = cfg;
            _log = log;
        }

        public async Task SendAsync(string text)
        {
            try
            {
                var token = _cfg["Telegram:Token"];
                var chatId = _cfg["Telegram:ChatId"];               

                await _http.PostAsJsonAsync(
                    $"https://api.telegram.org/bot{token}/sendMessage",
                    new { chat_id = chatId, text });
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Falha ao notificar Telegram");
            }
        }
    }
}