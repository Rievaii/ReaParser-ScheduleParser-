using System;
using System.Threading.Tasks;

namespace ScheduleParser
{
    internal class Program
    {
        private async static Task Main(string[] args)
        {

            Database database = new Database();
            
            VKBot vk = new VKBot();

            await vk.VkConnectAsync();

        }
    }
}
