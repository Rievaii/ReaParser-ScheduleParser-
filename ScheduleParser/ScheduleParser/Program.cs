using System;

namespace ScheduleParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            Database database = new Database();
            database.CheckUser("123");
            //VKBot vk = new VKBot();

            //vk.Connect();

        }
    }
}
