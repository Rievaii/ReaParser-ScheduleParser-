using System;
using System.Data.SQLite;

namespace ScheduleParser
{
    internal class Database
    {
        SQLiteConnection sqlite_conn = new SQLiteConnection(@"Data Source=C:\Program Files\SQLiteStudio\schedulebot");
        SQLiteDataReader sqlite_datareader;
        SQLiteCommand sqlite_cmd;   

        string _UserId;
        string _UserGroup;

        public string isRegistred(string UserId)
        {
            try
            {
                sqlite_conn.Open();
                
                sqlite_cmd = sqlite_conn.CreateCommand();

                //get exact group number with user id 
                sqlite_cmd.CommandText = $"SELECT * FROM users WHERE UserId = '{UserId}';";

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    int myreader = sqlite_datareader.GetInt32(0);
                    _UserId = sqlite_datareader.GetString(1);
                    _UserGroup = sqlite_datareader.GetString(2);
                    Console.WriteLine("Запрошенный пользователь: \nid: " + myreader + "\nUserID: " + _UserId + "\nUserGroup: " + _UserGroup);
                }
                if (_UserGroup != null && _UserGroup != null)
                {
                    return _UserGroup;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                Console.WriteLine("Невозможно получить доступ к базе данных / Не найдены записи");
                return "";
            }
            finally
            {
                sqlite_conn.Close();
            }
        }
        /*
        public void AddUser(string UserId, string UserGroup)
        {
            if (isRegistred(UserId))
            {
                Console.WriteLine("Пользователь есть в базе");
                //only for testing
                Console.Read();
            }
            else if (!isRegistred(UserId))
            {
                Console.WriteLine("Такого пользователя нет в базе");
                //only for testing
                Console.Read(); 
                
                sqlite_cmd = conn.CreateCommand();

                sqlite_cmd.CommandText = "INSERT INTO users (UserId, UserGroup) VALUES (228, 4.105);";
                sqlite_cmd.ExecuteNonQuery();
            
            }
        }
        */
    }
}
