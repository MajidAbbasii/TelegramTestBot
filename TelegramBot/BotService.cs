using System.Globalization;
using System.Text;
using System.Text.Json;
using FlyApp.Classes;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace TelegramBot;

static class BotService
{
    public static System.Threading.Tasks.Task DoSomething()
    {
        var botClient = new TelegramBotClient("8240489877:AAGX2EtJrj40bBJuo0SDarcwJJQNvVhXMTk");

        botClient.StartReceiving(new DefaultUpdateHandler(HandleUpdateAsync, HandleErrorAsync));

        Console.WriteLine("Bot is running...");
        Console.ReadLine();
        return Task.CompletedTask;
    }

    public static async Task<string> FetchFlightData()
    {
        try
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(Guid.NewGuid().ToString("N"));
            var response = await httpClient.PostAsync("https://api.flytoday.ir/api/V1/Flight/SearchAnytime",
                new StringContent(
                    "{\"pricingSourceType\":0,\"adultCount\":1,\"childCount\":0,\"infantCount\":0,\"travelPreference\":{\"cabinType\":\"Y\",\"maxStopsQuantity\":\"All\",\"airTripType\":\"OneWay\"},\"originDestinationInformations\":[{\"destinationLocationCode\":\"KIH\",\"destinationType\":\"1\",\"originLocationCode\":\"THR\",\"originType\":\"1\"}],\"isJalali\":true}",
                    Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                StringBuilder emailBody = new StringBuilder();
                var pricedItineraryList = JsonSerializer.Deserialize<ResponseFlightService>(content).pricedItineraries;
                foreach (var pricedItinerary in pricedItineraryList)
                {
                    var price = pricedItinerary.airItineraryPricingInfo.itinTotalFare.totalFare;
                    var date = pricedItinerary.originDestinationOptions[0].flightSegments[0].departureDateTime;
                    var quantity = pricedItinerary.originDestinationOptions[0].flightSegments[0].seatsRemaining;

                    emailBody.AppendLine($"تاریخ : {date.ToPersianDateTime()}");
                    emailBody.AppendLine($"قیمت : {price:N0}");
                    emailBody.AppendLine($"تعداد : {quantity}");
                }

                return emailBody.ToString();
            }
            else
            {
                return "فاقد دیتا";
            }
        }
        catch
        {
            return "فاقد دیتا";
        }
    }

    private static async System.Threading.Tasks.Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, System.Threading.CancellationToken cancellationToken)
    {
        if (update.Message is { Text: { } messageText })
        {
            string userInput = update.Message.Text;

            // اجرای دستورات مختلف
            if (userInput.StartsWith("/start"))
            {
                var data = await FetchFlightData();
                await botClient.SendTextMessageAsync(update.Message.Chat.Id, data);
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
    public static string ToPersianDateTime(this DateTime datetime)
    {
        PersianCalendar persianCalendar = new PersianCalendar();
        return
            $"{persianCalendar.GetYear(datetime)}/{persianCalendar.GetMonth(datetime)}/{persianCalendar.GetDayOfMonth(datetime)}-{persianCalendar.GetHour(datetime).ToString().PadLeft(2, '0')}:{persianCalendar.GetMinute(datetime).ToString().PadLeft(2, '0')}";
    }
}