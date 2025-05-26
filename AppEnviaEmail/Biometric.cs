using System.Text.RegularExpressions;
using AppEnviaEmail.src.DAO;
using AppEnviaEmail.src.Model;
using NITGEN.SDK.NBioBSP;

namespace AppEnviaEmail
{
    public class Biometric
    {
        uint ret;
        string strFIRHex;
        string strFIRText;
        private static NBioAPI.Type.FIR biFIR1; // objeto que armazena a digital em binário
        string strFIRText16; // variável para armazenar a string de 15 caracteres

        public List<string> Iniciador()
        {
            // Lista os dispositivos disponíveis
            NBioAPI m_NBioAPI;
            m_NBioAPI = new NBioAPI();

            List<string> devices = new List<string>();

            int i;
            uint nNumDevice;
            short[] nDeviceID;
            uint ret = m_NBioAPI.EnumerateDevice(out nNumDevice, out nDeviceID); // Use a local variable for 'ret'
            if (ret == NBioAPI.Error.NONE)
            {
                devices.Add("Auto_Detect");
                for (i = 0; i < nNumDevice; i++)
                {
                    switch (nDeviceID[i])
                    {
                        case NBioAPI.Type.DEVICE_NAME.FDU01:
                            devices.Add("FDU01");
                            break;
                    }
                }
            }

            // Ensure a return value in all code paths
            return devices;
        }

        public void Captura(string nome)
        {
            /*
            * Método para captura da imagem e armazenar no banco de dados             
            */

            // mensagem para posicionar o dedo no scanner
            MessageBox.Show("Posicione o dedo no scanner.", "Captura", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Cria o objeto de biometria
            NBioAPI m_NBioAPI;
            m_NBioAPI = new NBioAPI();
            // Inicia o scanner plugado e obtém o ID do dispositivo
            m_NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);

            //Handle to Full Image Record - variável que obtem a imagem da digital
            NBioAPI.Type.HFIR hNewFIR;

            // Captura única com callback  
            ret = m_NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out hNewFIR, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);

            // Verifica a captura
            if (ret == NBioAPI.Error.NONE)
            {
                // Dispositivo inicializado com sucesso ... 
                // Utiliza o FIR gravado em binário
                NBioAPI.Type.FIR biFIR;
                m_NBioAPI.GetFIRFromHandle(hNewFIR, out biFIR);
                biFIR1 = biFIR; // Armazena a digital em binário para comparação posterior
                                // Utiliza o FIR gravado em string  
                NBioAPI.Type.FIR_TEXTENCODE textFIR;
                m_NBioAPI.GetTextFIRFromHandle(hNewFIR, out textFIR, true);

                PersistirNoDatabase(nome);

                // Converte biFIR para hexadecimal  
                strFIRHex = BitConverter.ToString(biFIR.Data).Replace("-", ""); // Converte para hex sem hífens  
                                                                                // Obtém a representação textual do textFIR  
                strFIRText = textFIR.TextFIR; // Já está em formato texto (Base64 ou similar)  
                                              // Exibe o resultado  
                MessageBox.Show($"Digital Hexadecimal: {strFIRHex}\nDigital Texto: {strFIRText}",
                                "Captura", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                // Dispositivo inicializado com insucesso ...  ...  
                MessageBox.Show("Dedo não detectado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);

            ret = m_NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
            if (ret == NBioAPI.Error.NONE)
            {
                // Dispositivo encerrado com sucesso ...  
                MessageBox.Show("Scanner encerrado.", "Encerramento", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Falha ao fechar dispositivo ...  
                MessageBox.Show("Falha ao fechar dispositivo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool Comparar()
        {
            /*
             * Método para captura da imagem e verificar se coincide 
             * com a digital armazenada no banco de dados 
             */

            // mensagem para posicionar o dedo no scanner
            MessageBox.Show("Posicione o dedo no scanner.", "Verificação", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Cria o objeto de biometria
            NBioAPI m_NBioAPI;
            m_NBioAPI = new NBioAPI();
            // Inicia o scanner plugado e obtém o ID do dispositivo
            m_NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);
            // Inicia o parámetro para e-mail
            string formatado = "";

            //Handle to Full Image Record - variável que obtem a imagem da digital
            NBioAPI.Type.HFIR hNewFIR2;

            // Captura única com callback  
            ret = m_NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out hNewFIR2, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);

            // Verifica a captura
            if (ret == NBioAPI.Error.NONE)
            {
                // Dispositivo inicializado com sucesso ... 
                // Utiliza o FIR gravado em binário
                NBioAPI.Type.FIR biFIR;
                m_NBioAPI.GetFIRFromHandle(hNewFIR2, out biFIR);
                // Utiliza o FIR gravado em string  
                NBioAPI.Type.FIR_TEXTENCODE textFIR;
                m_NBioAPI.GetTextFIRFromHandle(hNewFIR2, out textFIR, true);
                // Grava o FIR no banco de dados  

                // Converte biFIR para hexadecimal  
                strFIRHex = BitConverter.ToString(biFIR.Data).Replace("-", ""); // Converte para hex sem hífens  
                                                                                // Obtém a representação textual do textFIR  
                strFIRText = textFIR.TextFIR; // Já está em formato texto (Base64 ou similar)  
                                              // Exibe o resultado  
                MessageBox.Show($"Digital Hexadecimal: {strFIRHex}\nDigital Texto: {strFIRText}",
                                "Captura", MessageBoxButtons.OK, MessageBoxIcon.Information);




                // Verifica se a string tem pelo menos 16 caracteres
                if (strFIRText.Length >= 16)
                {
                    // Gera um índice inicial aleatório
                    Random random = new Random();
                    int startIndex = random.Next(0, strFIRText.Length - 15); // Garante que há 16 caracteres disponíveis

                    // Extrai uma substring de 16 caracteres
                    strFIRText16 = strFIRText.Substring(startIndex, 16);

                    formatado = Regex.Replace(strFIRText16, ".{4}", "$0."); // Insere ponto após cada grupo de 4 usando REGEX
                    formatado = formatado.TrimEnd('.'); // Remove o ponto final extra e gera a assinatura digital

                    MessageBox.Show("Mostra a assinatura digital: " + formatado, "Informa", MessageBoxButtons.OK, MessageBoxIcon.Error);


                }
                else
                {
                    // Caso a string seja muito curta
                    MessageBox.Show("A string é muito curta para extrair 15 caracteres.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                // Dispositivo inicializado com insucesso ...  ...  
                MessageBox.Show("Dedo não detectado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false; // Retorna falso para indicar que a comparação não foi realizada
            }

            // Verifica a digital capturada com a digital armazenada no banco de dados
            bool result;
            NBioAPI.Type.FIR_PAYLOAD myPayload = new NBioAPI.Type.FIR_PAYLOAD();
            ret = m_NBioAPI.VerifyMatch(hNewFIR2, biFIR1, out result, myPayload);

            if (ret == NBioAPI.Error.NONE)
            {
                // Verificação realizada com sucesso ... 
                if (result)
                {
                    // Digital verificada com sucesso ...  
                    MessageBox.Show("Digital verificada com sucesso.\nEnviando e-mail de confirmação", "Confirmação", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Envia o e-mail
                    SmtpServerEnvio.ProcessarEmail(formatado); // Chama o método para enviar o e-mail

                }
                else
                {
                    // Digital não verificada ...  
                    MessageBox.Show("Digital não confirmada.", "Verificação", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false; // Retorna falso para indicar que a comparação não foi realizada
                }
            }
            else
            {
                // Dispositivo inicializado com insucesso ...  
                MessageBox.Show("Dedo não detectado.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false; // Retorna falso para indicar que a comparação não foi realizada
            }

            // Fecha o dispositivo
            ret = m_NBioAPI.CloseDevice(NBioAPI.Type.DEVICE_ID.AUTO);
            if (ret == NBioAPI.Error.NONE)
            {
                // Dispositivo encerrado com sucesso ...  
                MessageBox.Show("Scanner encerrado.", "Encerramento", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Falha ao fechar dispositivo ...  
                MessageBox.Show("Falha ao fechar dispositivo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Retorna falso para indicar que a comparação não foi realizada
            }

            return true; // Retorna verdadeiro para indicar que a comparação foi realizada

        }
        private static void PersistirNoDatabase(string nome)
        {
            //var user = new User(nome, biFIR1);
            //UserDAO.Create(user);
        }
    }
}
