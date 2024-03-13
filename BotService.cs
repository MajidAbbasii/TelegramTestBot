using Telegram.Bot;

namespace TelegramBot;

class BotService
{
    public static System.Threading.Tasks.Task DoSomething()
    {
        var botClient = new TelegramBotClient("7190677204:AAEsMZ1HssGgIH17OmXifSuX-Dabo9TXZ5M");

        botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync));

        Console.WriteLine("Bot is running...");
        Console.ReadLine();
        return Task.CompletedTask;
    }

    private static async System.Threading.Tasks.Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, System.Threading.CancellationToken cancellationToken)
    {
        if (update.Message is { Text: { } messageText })
        {
            var chatId = update.Message.Chat.Id;
            Console.WriteLine($"Received a message from {chatId}: {messageText}");

            // تنظیم متنی که می‌خواهید به عنوان خروجی ارسال شود
            string outputText = "Hello, I received your message: " + messageText;

            // ارسال پاسخ به کاربر
            await botClient.SendTextMessageAsync(chatId, outputText);
        }
    }

    private static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, System.Threading.CancellationToken cancellationToken)
    {
        Console.WriteLine(exception.Message);
        return Task.CompletedTask;
    }
}