using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AppEnviaEmail.src.Service
{
    internal class ConnectionFactory
    {
        private static string connectionProperties =
               "Server=localhost;" +
               "Port=3306;" +
               "Database=database_odontologia;" +
               "User=root;" +
               "Password=";

        public static MySqlConnection GetConnection()
        {
            try
            {
                var connection = new MySqlConnection(connectionProperties);
                connection.Open();
                return connection;
            }
            catch (Exception e)
            {
                throw new Exception("Erro ao conectar ao banco de dados", e);
            }
        }
    }
}
