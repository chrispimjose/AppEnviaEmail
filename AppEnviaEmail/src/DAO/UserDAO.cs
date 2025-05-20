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

        public static byte[] BuscarDigitalPorNome(string nome)
        {
            using (var connection = GetConnection())
            {
                string query = "SELECT digital_binaria FROM usuarios WHERE nome = @nome";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@nome", nome);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (byte[])reader["digital_binaria"];
                        }
                    }
                }

                return Array.Empty<byte>();
            }

        }
    }
}
