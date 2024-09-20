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
using System.Runtime.InteropServices.JavaScript;

namespace ConsoleApp15
{
    class BOT : IObserver
    {
        private TelegramBotClient _client;
        private string _currentFunction;
        private IBotCommand _currentCommand;

        public BOT(TelegramBotClient client)
        {
            _client = client;
            _currentFunction = string.Empty;
        }

        // Реализация метода обновления в рамках паттерна «Наблюдатель»
        public async Task Update(Update update)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                string message = update.Message.Text;
                Console.WriteLine($"Получено сообщение: {message}");

                if (message == "/start")
                {
                    var KeyBordButton = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton[] { "Количество символов в предложении", "Сумма чисел" }
                    })
                    {
                        ResizeKeyboard = true
                    };
                    await _client.SendTextMessageAsync(update.Message.Chat.Id,
                        "Выберите команду",
                        replyMarkup: KeyBordButton);
                }
                else if (_currentFunction == string.Empty)
                {
                    if (message == "Количество символов в предложении")
                    {
                        _currentFunction = "Количество символов в предложении";
                        _currentCommand = new GetLengthText();
                        await _client.SendTextMessageAsync(update.Message.Chat.Id, "Отправьте символы для вычисления длины.");
                    }
                    else if (message == "Сумма чисел")
                    {
                        _currentFunction = "Сумма чисел";
                        _currentCommand = new GetSumBot();
                        await _client.SendTextMessageAsync(update.Message.Chat.Id, "Отправьте числа через пробел для вычисления суммы.");
                    }
                    else
                    {
                        await _client.SendTextMessageAsync(update.Message.Chat.Id, "Неизвестная команда. Используйте /start, Количество символов в предложении или Сумма чисел.");
                    }
                }
                else
                {
                    string res = _currentCommand.Execute(update.Message.Text);
                    await _client.SendTextMessageAsync(update.Message.Chat.Id, res, replyToMessageId: update.Message.MessageId);
                    _currentFunction = string.Empty;
                }
            }
        }

        // Метод для обработки ошибок
        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var errorMessage = exception switch
            {
                ApiRequestException apiEx => $"Telegram API Error:\n[{apiEx.ErrorCode}]\n{apiEx.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine($"Ошибка: {errorMessage}");
            return Task.CompletedTask;
        }
    }

    class MessageHandler : ISubject
    {
        private readonly List<IObserver> _observers;

        public MessageHandler()
        {
            _observers = new List<IObserver>();
        }

        public void RegisterObserver(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(IObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyObservers(Update update)
        {
            foreach (var observer in _observers)
            {
                _ = observer.Update(update); // Игнорируем результат, так как это Task
            }
        }

        // Метод обработки сообщений (будет вызываться при обновлениях)
        public async Task HandleUpdateAsync(Update update)
        {
            NotifyObservers(update);
        }
    }

    class GetLengthText : IBotCommand
    {
        public string Execute(string input)
        {
            string strL = input.Replace(" ", string.Empty);
            int len = strL.Length;
            return $"В вашем сообщении {len} символов";
        }
    }

    // Класс для подсчета суммы чисел
    class GetSumBot : IBotCommand
    {
        public string Execute(string input)
        {
            try
            {
                int sum = input.Split(' ').Select(int.Parse).Sum();
                return $"Сумма ваших чисел равна {sum}";
            }
            catch (FormatException)
            {
                return $"Ошибка: Нужно ввести числа, формат строк не поддерживается";
            }
            catch (Exception)
            {
                return $"Неизвестная ошибка";
            }
        }
    }
}
