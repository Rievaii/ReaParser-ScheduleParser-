using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace ScheduleParser
{
    internal class Database
    {
        public bool CheckUser(string UserId)
        {
            using (var connection = new SqliteConnection("Data Source=usersdata.db"))
            {
                connection.Open();
            }
            //check if user exists
            return true;
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
