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

            devices = m_Biometric.Iniciador();


            foreach (string item in devices)
            {
                comboDevice.Items.Add(item);
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
             * Captura a biometria
             */

            Biometric biometric = new Biometric();
            biometric.Captura();

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            /*
             * Captura e compara a biometria
             */

            Biometric biometric = new Biometric();
            biometric.Comparar();
        }
    }
}
