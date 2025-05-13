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
                VerificarSeDigitalExiste(user, connection);

                string sql = "INSERT INTO usuarios (nome, digital_code) " +
                    "VALUES (@nome, @digital_code)";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nome", user.Name);
                command.Parameters.AddWithValue("@digital_code", user.biFIR);

                command.ExecuteNonQuery();
            }
        }

        private static void VerificarSeDigitalExiste(User user, MySqlConnection connection)
        {
            string checkSql = "SELECT COUNT(*) FROM usuarios WHERE digital_code = @digital_code";
            MySqlCommand checkCommand = new MySqlCommand(checkSql, connection);
            checkCommand.Parameters.AddWithValue("@digital_code", user.biFIR);

            long count = (long)checkCommand.ExecuteScalar();

            if (count > 0)
            {
                throw new InvalidOperationException("Essa digital já está cadastrada no sistema.");
            }
        }
    }
}
