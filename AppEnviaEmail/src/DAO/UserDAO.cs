using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppEnviaEmail.src.Model;
using MySql.Data.MySqlClient;
using static AppEnviaEmail.src.Service.ConnectionFactory;

namespace AppEnviaEmail.src.DAO
{
    internal class UserDAO
    {
        public static void Create(User user)
        {
            using (var connection = GetConnection())
            {
                string sql = "INSERT INTO usuarios (nome, digital_code) " +
                    "VALUES (@nome, @digital_code)";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nome", user.Name);
                command.Parameters.AddWithValue("@digital_code", user.DigitalCode);

                command.ExecuteNonQuery();
            }
        }
    }
}
