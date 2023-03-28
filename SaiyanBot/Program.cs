namespace SaiyanBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bot = new SaiyanBot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}