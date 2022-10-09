namespace ScheduleParser
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ReaParser reaParser = new ReaParser();
            reaParser.RunParse(@"https://rasp.rea.ru/?q=15.27%D0%B4-%D0%B8%D1%81%D1%8215%2F22%D0%B1");
        }
    }
}
