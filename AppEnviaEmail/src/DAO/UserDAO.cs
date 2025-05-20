using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppEnviaEmail.src.Model;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using static AppEnviaEmail.src.Service.ConnectionFactory;
using static NITGEN.SDK.NBioBSP.NBioAPI.Type;

namespace AppEnviaEmail.src.DAO
{
    internal class UserDAO
    {
        public static void PersistUser(User user)
        {
            using (var connection = GetConnection())
            {
                VerificarSeDigitalExisteAoCadastrar(user, connection);

                string sql = "INSERT INTO usuarios (nome, digital_binaria) " +
                    "VALUES (@nome, @digital_binaria)";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nome", user.Name);
                command.Parameters.AddWithValue("@digital_binaria", user.DigitalBinaria);

                command.ExecuteNonQuery();
            }
        }

        private static void VerificarSeDigitalExisteAoCadastrar(User user, MySqlConnection connection)
        {
            string checkSql = "SELECT COUNT(*) FROM usuarios WHERE digital_binaria = @digital_binaria";
            MySqlCommand checkCommand = new MySqlCommand(checkSql, connection);
            checkCommand.Parameters.AddWithValue("@digital_binaria", user.DigitalBinaria);

            long count = (long)checkCommand.ExecuteScalar();

            if (count > 0)
            {
                throw new InvalidOperationException("Essa digital já está cadastrada no sistema.");
            }
        }

        public static void BuscarDigital(User user)
        {
            using (var connection = GetConnection())
            {
                //string checkSql = "SELECT COUNT(*) FROM usuarios WHERE digital_code = @digital_code";
                string sql = "select nome, HEX(digital_code) as digital from usuarios where nome = '" + user.Name + "';";
                MySqlCommand command = new MySqlCommand(sql, connection);

                command.Parameters.AddWithValue("@nome", user.Name);



                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(" >>>>> Digital:"+ reader["digital"]);
                    }
                }

            }

        }
    }
}
