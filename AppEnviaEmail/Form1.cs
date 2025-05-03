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
        NBioAPI.Type.FIR biFIR1; // objeto que armazena a digital em binário
        string strFIRText15; // variável para armazenar a string de 15 caracteres


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
            // Instacia o objeto de biometria
            Biometric m_Biometric = new Biometric();
            List<string> devices = new List<string>();


                comboDevice.Items.Add("Auto_Detect");
                {
                    {
                            comboDevice.Items.Add("FDU01");
                            break;
                    }
                }


            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
             */

            }
            else
                // Dispositivo inicializado com insucesso ...  ...  
                label1.Text = "Dedo não detectado.";

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
             */



        }
    }
}
