using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AppEnviaEmail.src.Model;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.Crmf;
using static AppEnviaEmail.src.Service.ConnectionFactory;

namespace AppEnviaEmail.src.DAO
{
    internal class UserDAO
    {
        public static void Create(User user)
        {
            using (var connection = GetConnection())
            {
                if (FindByUserName(user.Name) != null)
                {
                    throw new InvalidOperationException("Nome de usuário já existente no banco");
                }

                string sql = "INSERT INTO usuarios (nome, template) " +
                    "VALUES (@nome, @template)";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@nome", user.Name);
                command.Parameters.AddWithValue("@template", user.textTemplate);

                command.ExecuteNonQuery();
            }
        }

        private static bool VerificarSeNomeExiste(string userName)
        {
            bool existe = false;

            using (var connection = GetConnection())
            {
                try
                {
                    string query = "SELECT COUNT(nome) FROM usuarios WHERE nome = @nome;";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@nome", userName);

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        existe = count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }

            return existe;
        }

        public static User FindByUserName(string userName)
        {
            using (var connection = GetConnection())
            {
                try
                {
                    string sql = "SELECT nome, template FROM usuarios WHERE nome = @userName;";

                    using (var command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@userName", userName);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var name = reader["nome"].ToString();
                                var textTemplate = reader["template"].ToString();

                                return new User(name, textTemplate);
                            }
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao buscar usuário: {ex.Message}");
                    return null;
                }
            }
        }

        private static void VerificarSeDigitalExiste(User user, MySqlConnection connection)
        {
            string checkSql = "SELECT COUNT(*) FROM usuarios WHERE digital_code = @digital_code";
            MySqlCommand checkCommand = new MySqlCommand(checkSql, connection);
            checkCommand.Parameters.AddWithValue("@digital_code", user.textTemplate);

            long count = (long)checkCommand.ExecuteScalar();

            if (count > 0)
            {
                throw new InvalidOperationException("Essa digital já está cadastrada no sistema.");
            }
        }
    }
}
