using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TelegramBot;

class BotService
{
    public static System.Threading.Tasks.Task DoSomething()
    {
        var botClient = new TelegramBotClient("8290677204:AAEsMZ1HssGgIH17OmXifSuX-Dabo9TXZ5M");

        botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync));

        Console.WriteLine("Bot is running...");
        Console.ReadLine();
        return Task.CompletedTask;
    }

    private static async System.Threading.Tasks.Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, System.Threading.CancellationToken cancellationToken)
    {
        if (update.Message is { Text: { } messageText })
        {
            string userInput = update.Message.Text;

            // اجرای دستورات مختلف
            if (userInput.StartsWith("/start"))
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Welcome! Bot is ready to serve you.");
            }
            else if (userInput.StartsWith("/hello"))
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Hello!");
            }
            else if (userInput.StartsWith("/echo"))
            {
                string echoMessage = userInput.Substring("/echo".Length).Trim();
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, echoMessage);
            }
            // افزودن دستورات دیگر به دلخواه
            // ...
            else
            {
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Command not recognized.");
            }
            // var chatId = update.Message.Chat.Id;
            // Console.WriteLine($"Received a message from {chatId}: {messageText}");
            //
            // // تنظیم متنی که می‌خواهید به عنوان خروجی ارسال شود
            // string outputText = "Hello, I received your message: " + messageText;
            //
            // // ارسال پاسخ به کاربر
            // await botClient.SendTextMessageAsync(chatId, outputText);
        }
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, System.Threading.CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}