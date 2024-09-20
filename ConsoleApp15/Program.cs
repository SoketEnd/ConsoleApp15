using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
namespace ConsoleApp15
{
    class Program
    {
        private static string token { get; set; } = "7432274321:AAGxGF2d76TFTioF5aPxwogtwg7w5hJbXoc";

        static void Main(string[] args)
        {
            var botClient = new TelegramBotClient(token);
            var messageHandler = new MessageHandler();
            var bot = new BOT(botClient);

            // Регистрация бота как наблюдателя
            messageHandler.RegisterObserver(bot);

            using var cts = new CancellationTokenSource();

            // Запуск обработки обновлений
            botClient.StartReceiving(
                async (client, update, token) =>
                {
                    await messageHandler.HandleUpdateAsync(update);
                },
                BOT.HandleErrorAsync, // Здесь будет вызван метод обработки ошибок
                new ReceiverOptions(),
                cancellationToken: cts.Token
            );

            Console.WriteLine("Бот запущен.");
            Console.ReadLine();

            // Остановка бота
            cts.Cancel();
        }
    }
}
