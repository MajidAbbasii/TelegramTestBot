using TelegramBot;

Task.Run(() =>
{
    BotService.DoSomething();
});
while (true)
{
    Thread.Sleep(new TimeSpan(1,1,1));
}