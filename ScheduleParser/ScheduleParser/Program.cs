using System;

namespace ScheduleParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            Database database = new Database();
            Console.WriteLine(database.isRegistred("1235"));
            
            VKBot vk = new VKBot();

            //vk.Connect();

        }
    }
}
