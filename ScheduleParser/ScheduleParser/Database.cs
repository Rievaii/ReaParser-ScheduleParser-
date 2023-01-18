using System;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace ScheduleParser
{
    internal class Database
    {
        private SQLiteConnection sqlite_conn = new SQLiteConnection(@"Data Source=C:\Users\Admin\Documents\GitHub\ReaParser-ScheduleParser-\ScheduleParser\ScheduleParser\schedulebot");
        private SQLiteDataReader sqlite_datareader;
        private SQLiteCommand sqlite_cmd;
        private string _UserId;
        private string _UserGroup;

        public string GetUserGroup(string UserId)
        {
            
            try
            {

                sqlite_conn.Open();

                sqlite_cmd = sqlite_conn.CreateCommand();

                sqlite_cmd.CommandText = $"SELECT * FROM users WHERE vkid = '{UserId}';";

                sqlite_datareader = sqlite_cmd.ExecuteReader();

                while (sqlite_datareader.Read())
                {
                    int myreader = sqlite_datareader.GetInt32(0);
                    _UserId = sqlite_datareader.GetString(1);
                    _UserGroup = sqlite_datareader.GetString(2);

                    Console.WriteLine("Текущий пользователь: \nid: " + myreader + "\n vkid: " + _UserId + "\n groupid: " + _UserGroup);

                }

                sqlite_conn.Close();

                if (_UserGroup != null && _UserId != null)
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

        }
        
        public void AddUser(string UserId, string UserGroup)
        {
            if (UserId != null && UserGroup != null)
            {
                try
                {
                    if (GetUserGroup(UserId) == "")
                    {
                        sqlite_conn.Open();

                        sqlite_cmd = sqlite_conn.CreateCommand();

                        sqlite_cmd.CommandText = $"INSERT INTO users (vkid, groupid) VALUES ({UserId}, '{UserGroup}');";

                        sqlite_cmd.ExecuteNonQuery();

                        Console.WriteLine("Пользователь успешно добавлен в базу данных " + UserId + " " + UserGroup);
                    }
                    else if (GetUserGroup(UserId) != "")
                    {
                        sqlite_conn.Open();

                        sqlite_cmd = sqlite_conn.CreateCommand();

                        sqlite_cmd.CommandText = $"UPDATE users SET groupid = '{UserGroup}' WHERE vkid = {UserId};";

                        sqlite_cmd.ExecuteNonQuery();

                        Console.WriteLine("Группа пользователя " + UserId + " обновлена на " + UserGroup);
                    }

                    sqlite_conn.Close();

                }
                catch
                {

                }
            }
        }
    }
}
