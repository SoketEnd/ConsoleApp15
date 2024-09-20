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
    interface ISubject
    {
        void RegisterObserver(IObserver observer);
        void RemoveObserver(IObserver observer);
        void NotifyObservers(Update update);
    }

    // Интерфейс наблюдателя
    interface IObserver
    {
        Task Update(Update update);
    }
    interface IBotCommand
    {
        string Execute(string input);
    }

}
