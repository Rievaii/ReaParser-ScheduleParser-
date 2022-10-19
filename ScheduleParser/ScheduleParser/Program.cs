using System;

namespace ScheduleParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //15.27Д-ИСТ15/22б
            ReaParser reaParser = new ReaParser();
            VKBot vk = new VKBot();

            vk.Connect();

            foreach(var element in reaParser.GetWeekSchedule("15.27Д-ИСТ15/22б"))
            {
                Console.WriteLine(element); 
            }
        }
    }
}
