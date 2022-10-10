namespace ScheduleParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ReaParser reaParser = new ReaParser();
            reaParser.RunParse("15.27Д-ИСТ15/22б");
        }
    }
}
