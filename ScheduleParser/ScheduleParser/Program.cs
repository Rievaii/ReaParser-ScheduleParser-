namespace ScheduleParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //15.27Д-ИСТ15/22б
            ReaParser reaParser = new ReaParser();
            VKBot vk = new VKBot();

            reaParser.GetSchedule("15.27Д-ИСТ15/22б",3);
            //vk.Connect();
            
        }
    }
}
