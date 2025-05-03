using System.Text.RegularExpressions;
using NITGEN.SDK.NBioBSP;
using Org.BouncyCastle.Ocsp;
using static NITGEN.SDK.NBioBSP.NBioAPI.Type;

namespace AppEnviaEmail
{
    public partial class Form1 : Form
    {
        uint ret;
        string strFIRHex;
        string strFIRText;
        NBioAPI.Type.FIR biFIR1; // objeto que armazena a digital em bin�rio
        string strFIRText15; // vari�vel para armazenar a string de 15 caracteres

        public Form1()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
           // SmtpServerEnvio.ProcessarEmail();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            // Lista os dipositivos dispon�veis
            NBioAPI m_NBioAPI;
            m_NBioAPI = new NBioAPI();

            int i;
            uint nNumDevice;
            short[] nDeviceID;
            ret = m_NBioAPI.EnumerateDevice(out nNumDevice, out nDeviceID);
            if (ret == NBioAPI.Error.NONE)
            {
                comboDevice.Items.Add("Auto_Detect");
                for (i = 0; i < nNumDevice; i++)
                {
                    switch (nDeviceID[i])
                    {
                        case NBioAPI.Type.DEVICE_NAME.FDU01:
                            comboDevice.Items.Add("FDU01");
                            break;
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
             * M�todo para captura da imagem e armazenar no banco de dados             * 
             */

            // mensagem para posicionar o dedo no scanner
            MessageBox.Show("Posicione o dedo no scanner.", "Captura", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Cria o objeto de biometria
            NBioAPI m_NBioAPI;
            m_NBioAPI = new NBioAPI();
            // Inicia o scanner plugado e obt�m o ID do dispositivo
            m_NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);

            //Handle to Full Image Record - vari�vel que obtem a imagem da digital
            NBioAPI.Type.HFIR hNewFIR;
      
            // Captura �nica com callback  
            ret = m_NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out hNewFIR, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);

            // Verifica a captura
            if (ret == NBioAPI.Error.NONE)
            {
                // Dispositivo inicializado com sucesso ... 
                // Utiliza o FIR gravado em bin�rio
                NBioAPI.Type.FIR biFIR;
                m_NBioAPI.GetFIRFromHandle(hNewFIR, out biFIR);
                biFIR1 = biFIR; // Armazena a digital em bin�rio para compara��o posterior
                // Utiliza o FIR gravado em string  
                NBioAPI.Type.FIR_TEXTENCODE textFIR;
                m_NBioAPI.GetTextFIRFromHandle(hNewFIR, out textFIR, true);
                // Grava o FIR no banco de dados  

                // Converte biFIR para hexadecimal  
                strFIRHex = BitConverter.ToString(biFIR.Data).Replace("-", ""); // Converte para hex sem h�fens  
                                                                                // Obt�m a representa��o textual do textFIR  
                strFIRText = textFIR.TextFIR; // J� est� em formato texto (Base64 ou similar)  
                // Exibe o resultado  
                label1.Text = "Digital capturada.";
                MessageBox.Show($"Digital Hexadecimal: {strFIRHex}\nDigital Texto: {strFIRText}",
                                "Captura", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
                // Dispositivo inicializado com insucesso ...  ...  
                label1.Text = "Dedo n�o detectado.";

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

        private void button3_Click(object sender, EventArgs e)
        {
            /*
             * M�todo para captura da imagem e verificar se coincide 
             * com a digital armazenada no banco de dados 
             */

            // mensagem para posicionar o dedo no scanner
            MessageBox.Show("Posicione o dedo no scanner.", "Verifica��o", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Cria o objeto de biometria
            NBioAPI m_NBioAPI;
            m_NBioAPI = new NBioAPI();
            // Inicia o scanner plugado e obt�m o ID do dispositivo
            m_NBioAPI.OpenDevice(NBioAPI.Type.DEVICE_ID.AUTO);

            //Handle to Full Image Record - vari�vel que obtem a imagem da digital
            NBioAPI.Type.HFIR hNewFIR2;

            // Captura �nica com callback  
            ret = m_NBioAPI.Capture(NBioAPI.Type.FIR_PURPOSE.VERIFY, out hNewFIR2, NBioAPI.Type.TIMEOUT.DEFAULT, null, null);

            // Verifica a captura
            if (ret == NBioAPI.Error.NONE)
            {
                // Dispositivo inicializado com sucesso ... 
                // Utiliza o FIR gravado em bin�rio
                NBioAPI.Type.FIR biFIR;
                m_NBioAPI.GetFIRFromHandle(hNewFIR2, out biFIR);
                // Utiliza o FIR gravado em string  
                NBioAPI.Type.FIR_TEXTENCODE textFIR;
                m_NBioAPI.GetTextFIRFromHandle(hNewFIR2, out textFIR, true);
                // Grava o FIR no banco de dados  

                // Converte biFIR para hexadecimal  
                strFIRHex = BitConverter.ToString(biFIR.Data).Replace("-", ""); // Converte para hex sem h�fens  
                                                                                // Obt�m a representa��o textual do textFIR  
                strFIRText = textFIR.TextFIR; // J� est� em formato texto (Base64 ou similar)  
                // Exibe o resultado  
                label1.Text = "Digital capturada.";
                MessageBox.Show($"Digital Hexadecimal: {strFIRHex}\nDigital Texto: {strFIRText}",
                                "Captura", MessageBoxButtons.OK, MessageBoxIcon.Information);


                // Verifica se a string tem pelo menos 15 caracteres
                if (strFIRText.Length >= 15)
                {
                    // Gera um �ndice inicial aleat�rio
                    Random random = new Random();
                    int startIndex = random.Next(0, strFIRText.Length - 14); // Garante que h� 15 caracteres dispon�veis

                    // Extrai uma substring de 15 caracteres
                    strFIRText15 = strFIRText.Substring(startIndex, 15);

                    // Exibe ou usa a parte selecionada
                    // Exemplo: MessageBox.Show($"Parte selecionada: {selectedPart}", "Resultado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // Caso a string seja muito curta
                    MessageBox.Show("A string � muito curta para extrair 15 caracteres.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }     

            }
            else
                // Dispositivo inicializado com insucesso ...  ...  
                label1.Text = "Dedo n�o detectado.";


            // Verifica a digital capturada com a digital armazenada no banco de dados
            bool result;
            NBioAPI.Type.FIR_PAYLOAD myPayload = new NBioAPI.Type.FIR_PAYLOAD();
            ret = m_NBioAPI.VerifyMatch(hNewFIR2, biFIR1, out result, myPayload);

            if (ret == NBioAPI.Error.NONE)
            {
                // Verifica��o realizada com sucesso ... 
                if (result)
                {
                    // Digital verificada com sucesso ...  
                    MessageBox.Show("Digital verificada com sucesso.", "Verifica��o", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Envia o e-mail
                    SmtpServerEnvio.ProcessarEmail(strFIRText15); // Chama o m�todo para enviar o e-mail

                }
                else
                {
                    // Digital n�o verificada ...  
                    MessageBox.Show("Digital n�o verificada.", "Verifica��o", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // Dispositivo inicializado com insucesso ...  
                label1.Text = "Dedo n�o detectado.";
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
            }           

        }
    }
}
