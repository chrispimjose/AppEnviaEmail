
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AppEnviaEmail
{
    internal class SmtpServerEnvio
    {
        // E-mail que será usado para enviar (precisa estar habilitado para SMTP com senha de app)
        private const string emailEnvio = "josepadilha@gmail.com";

        // E-mail que receberá a mensagem
        private const string emailDestino = "erivelton.lima @animaeducacao.com.br";

        //erivelton.lima @animaeducacao.com.br
        // jose.chrispim@animaeducacao.com.br

        // Senha de aplicativo gerada no Gmail (não é a senha normal)
        private const string Appsenha = "riqv fwtx svca nohc";

        /// <summary>
        /// Configura o cliente SMTP para envio via servidor do Gmail.
        /// </summary>
        /// <returns>Instância de SmtpClient pronta para enviar</returns>
        private static SmtpClient ConfigurarSmtp()
        {
            var smtp = new SmtpClient()
            {
                Host = "smtp.gmail.com",            // Endereço do servidor SMTP do Google
                Port = 587,                         // Porta padrão para TLS/STARTTLS
                EnableSsl = true,                   // Ativa criptografia SSL/TLS
                DeliveryMethod = SmtpDeliveryMethod.Network, // Envio pela rede
                UseDefaultCredentials = false,      // Não usa as credenciais do Windows
                Credentials = new NetworkCredential(emailEnvio, Appsenha) // Login e senha (senha de app)
            };

            return smtp;
        }

        /// <summary>
        /// Cria a mensagem que será enviada por e-mail.
        /// </summary>
        /// <returns>Objeto MailMessage com assunto, corpo, remetente e destinatário</returns>
        private static MailMessage ConfigurarMensagem(string assinatura)
        {
            var msg = new MailMessage
            {
                From = new MailAddress(emailEnvio, "Serviço de Odontologia"), // Remetente com nome personalizado
                SubjectEncoding = Encoding.UTF8,                              // Codificação do assunto   
                Subject = "Entrega de caixa para esterilização",              // Assunto do e-mail
                IsBodyHtml = true,                                            // Habilita HTML no corpo
                BodyEncoding = Encoding.UTF8,                                 // Codificação do corpo
                Body = "Prezado Erivelton" + "<br><br>" + "Esse email é a confirmação da entrega da caixa do procedimento: Ortodôntico, entregue na data: 02/05/2025, às 20:46 horas,  pelo Serviço de Odontologia da UNP." + "<br><br>" + "Você validou a entrega através de captura biométrica à 20:45 horas conforme registrado no sistema." + "<br><br>" + "Assinatura digital: " + assinatura+ "<br><br>"                                               // Corpo da mensagem                     
            };

            msg.To.Add(new MailAddress(emailDestino)); // Adiciona destinatário

            return msg;
        }

        /// <summary>
        /// Executa o envio do e-mail, capturando e exibindo mensagens de sucesso ou erro.
        /// </summary>
        public static void ProcessarEmail(string assinatura)
        {
            try
            {
                var msg = ConfigurarMensagem(assinatura); // Prepara a mensagem
                var smtp = ConfigurarSmtp();    // Prepara o cliente SMTP

                smtp.Send(msg);                 // Envia o e-mail
                smtp.Dispose();                 // Libera recursos

                MessageBox.Show("Email enviado com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                // Exibe qualquer erro ocorrido no envio
                MessageBox.Show("Erro ao enviar o email: " + e.Message + "\n" + e.StackTrace, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
