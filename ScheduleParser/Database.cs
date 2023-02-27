using System;
using Microsoft.Data.Sqlite;
using System.Threading.Tasks;

namespace ScheduleParser
{
    internal class Database
    {
        

        public string GetUserGroup(string UserId)
        {
            try
            {
                using (var connection = new SqliteConnection(@"Data Source = /home/app-admin/Bots/ITC-Bot/states.db"))
                {
                    connection.Open();

                    var command = connection.CreateCommand();
                    command.CommandText = $"SELECT * FROM lks WHERE vk_id = '{UserId}';";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(3);

                        }
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return "";
            }
        }


        public void AddUser(string UserId, string UserGroup)
        {
            try
            {
                if (GetUserGroup(UserId) == "")
                {
                    using (var connection = new SqliteConnection(@"Data Source=/home/app-admin/Bots/ITC-Bot/states.db"))
                    {
                        connection.Open();

                        var command = connection.CreateCommand();
                        command.CommandText = $"INSERT INTO lks (vk_id, study_group) VALUES ({UserId}, '{UserGroup}');";
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var connection = new SqliteConnection(@"Data Source = /home/app-admin/Bots/ITC-Bot/states.db"))
                    {
                        connection.Open();

                        var command = connection.CreateCommand();
                        command.CommandText = $"UPDATE lks SET study_group = '{UserGroup}' WHERE vk_id = {UserId};";
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}