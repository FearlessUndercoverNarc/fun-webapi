using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Models.Configs;

namespace Services.External
{
    public static class TelegramAPI
    {
        private static TelegramConfig _telegramConfig;
        private static string _env;

        public static void Configure(TelegramConfig telegramConfig, string env)
        {
            _telegramConfig = telegramConfig;
            _env = env + "\n";
        }

        public static async Task Send(string message)
        {
            var webClient = new WebClient();
            try
            {
                var parameters = new NameValueCollection
                {
                    ["chat_id"] = $"-{_telegramConfig.ChatId}",
                    ["parse_mode"] = "Markdown",
                    ["text"] = _env.Length + message.Length <= 4096 ? _env + message : _env + message[..^_env.Length]
                };

                await webClient.UploadValuesTaskAsync($"https://api.telegram.org/bot{_telegramConfig.Token}/sendMessage", "POST", parameters);
            }
            catch (WebException ex)
            {
                var httpWebResponse = ex.Response as HttpWebResponse;
                var response = await new StreamReader(httpWebResponse!.GetResponseStream()).ReadToEndAsync();
                Console.WriteLine($"Failed to send error to Telegram : {response}");
            }
        }
    }
}