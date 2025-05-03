using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace AppEnviaEmail
{
    public class SmtpServer
    {
        private readonly TcpListener _listener;
        private bool _isRunning;
        private readonly string _senderEmail;
        private readonly string _senderPassword;

        public SmtpServer(string ipAddress = "127.0.0.1", int port = 587, string senderEmail = null, string senderPassword = null)
        {
            _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            _senderEmail = senderEmail;
            _senderPassword = senderPassword;
        }

        public async Task StartAsync()
        {
            _isRunning = true;
            try
            {
                _listener.Start();
                Console.WriteLine($"SMTP Server started on {_listener.LocalEndpoint}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to start SMTP Server: {ex.Message}");
                return;
            }

            while (_isRunning)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine("New client connected");
                    _ = Task.Run(() => HandleClientAsync(client)); // Processa cliente em nova thread
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error accepting client: {ex.Message}");
                }
            }
        }

        public void Stop()
        {
            _isRunning = false;
            _listener.Stop();
            Console.WriteLine("SMTP Server stopped.");
        }

        private async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            using (var stream = client.GetStream())
            {
                try
                {
                    // Envia saudação inicial
                    await SendResponseAsync(stream, "220 localhost SMTP Server ready");

                    string from = null, to = null, data = null;
                    bool inData = false;
                    string currentCommand = "";

                    byte[] buffer = new byte[4096];
                    while (client.Connected)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                        {
                            Console.WriteLine("Client disconnected unexpectedly");
                            break;
                        }

                        string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        var lines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);

                        foreach (var line in lines)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            if (inData)
                            {
                                if (line == ".")
                                {
                                    inData = false;
                                    await SendResponseAsync(stream, "250 Message accepted for delivery");
                                    Console.WriteLine($"Message accepted for delivery from {from} to {to}");
                                    // Envia o e-mail real
                                    await SendEmailAsync(from, to, data);
                                    data = null;
                                }
                                else
                                {
                                    data += line + "\r\n";
                                }
                            }
                            else
                            {
                                currentCommand = line.ToUpper();
                                Console.WriteLine($"Received command: {line}");
                                if (currentCommand.StartsWith("HELO") || currentCommand.StartsWith("EHLO"))
                                {
                                    await SendResponseAsync(stream, "250 localhost");
                                }
                                else if (currentCommand.StartsWith("MAIL FROM:"))
                                {
                                    from = ExtractEmail(currentCommand);
                                    await SendResponseAsync(stream, "250 OK");
                                }
                                else if (currentCommand.StartsWith("RCPT TO:"))
                                {
                                    to = ExtractEmail(currentCommand);
                                    await SendResponseAsync(stream, "250 OK");
                                }
                                else if (currentCommand == "DATA")
                                {
                                    inData = true;
                                    data = "";
                                    await SendResponseAsync(stream, "354 Start mail input; end with <CRLF>.<CRLF>");
                                }
                                else if (currentCommand == "QUIT")
                                {
                                    await SendResponseAsync(stream, "221 Bye");
                                    Console.WriteLine("Client session ended");
                                    return;
                                }
                                else
                                {
                                    await SendResponseAsync(stream, "502 Command not implemented");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Client error: {ex.Message}");
                }
            }
        }

        private async Task SendResponseAsync(NetworkStream stream, string response)
        {
            byte[] data = Encoding.ASCII.GetBytes(response + "\r\n");
            await stream.WriteAsync(data, 0, data.Length);
            Console.WriteLine($"Sent response: {response}");
        }

        private string ExtractEmail(string command)
        {
            int start = command.IndexOf(':') + 1;
            string email = command.Substring(start).Trim('<', '>', ' ');
            return email;
        }

        private async Task SendEmailAsync(string from, string to, string body)
        {
            try
            {
                // Cria mensagem de e-mail
                var mailMessage = new MailMessage(from, to)
                {
                    Subject = "Test Email from Local SMTP Server",
                    Body = body,
                    IsBodyHtml = false
                };

                // Configura cliente SMTP para o servidor de destino
                string domain = to.Substring(to.IndexOf('@') + 1);
                Console.WriteLine($"Resolving MX record for domain: {domain}");
                string mxServer = await GetMxRecordAsync(domain);

                if (string.IsNullOrEmpty(mxServer))
                {
                    Console.WriteLine($"Failed to resolve MX record for {domain}");
                    return;
                }

                Console.WriteLine($"Connecting to MX server: {mxServer} on port 587");
                using (var smtpClient = new SmtpClient(mxServer, 587))
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = true; // Necessário para a maioria dos servidores MX na porta 587
                    if (!string.IsNullOrEmpty(_senderEmail) && !string.IsNullOrEmpty(_senderPassword))
                    {
                        smtpClient.Credentials = new NetworkCredential(_senderEmail, _senderPassword);
                        Console.WriteLine($"Using credentials for authentication: {_senderEmail}");
                    }
                    else
                    {
                        Console.WriteLine("No credentials provided for MX server authentication");
                    }
                    await smtpClient.SendMailAsync(mailMessage);
                    Console.WriteLine($"Email sent successfully to {to} via {mxServer}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        private async Task<string> GetMxRecordAsync(string domain)
        {
            try
            {
                // Usa Dns.GetHostEntryAsync como fallback para resolver o domínio
                var hostEntry = await Dns.GetHostEntryAsync(domain);
                if (hostEntry.AddressList.Length > 0)
                {
                    Console.WriteLine($"Resolved domain {domain} to {hostEntry.HostName}");
                    return hostEntry.HostName;
                }
                Console.WriteLine($"No address found for domain {domain}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to resolve domain {domain}: {ex.Message}");
                return null;
            }
        }
    }
}