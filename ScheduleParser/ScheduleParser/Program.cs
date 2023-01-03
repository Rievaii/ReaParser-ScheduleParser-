using System;

namespace ScheduleParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            Database database = new Database();
            
            VKBot vk = new VKBot();

            vk.Connect();

        }
    }
}
