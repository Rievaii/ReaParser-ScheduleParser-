using Microsoft.Data.Sqlite;
using System;

namespace ScheduleParser
{
    internal class Database
    { 
        public bool CheckUser(string UserId)
        {
            using (var connection = new SqliteConnection("Data Source= schedulebot.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand($"SELECT * FROM users WHERE UserId = '{UserId}'", connection);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows) // data gained
                    {
                        while (reader.Read())   // read data
                        {
                            var id = reader.GetValue(0);
                            var UId = reader.GetValue(1);
                            var UserGroup = reader.GetValue(2);
                            Console.WriteLine($"Номер: {id} \t Айди:{UId} \t Группа: {UserGroup}");

                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            
        }

        public void AddUser(string UserId, string UserGroup)
        {
            using (var connection = new SqliteConnection("Data Source= schedulebot.db"))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = $"INSERT INTO users (UserId, UserGroup) VALUES ({UserId}, {UserGroup})";
                int number = command.ExecuteNonQuery();

                Console.WriteLine($"В таблицу Users добавлено объектов: {number}");
            }
        }
    }
}
