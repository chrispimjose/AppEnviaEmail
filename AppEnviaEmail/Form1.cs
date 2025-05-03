using System.Text.RegularExpressions;
using NITGEN.SDK.NBioBSP;
using Org.BouncyCastle.Ocsp;
using static NITGEN.SDK.NBioBSP.NBioAPI.Type;

namespace AppEnviaEmail
{
    public partial class Form1 : Form
    {


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

            devices = m_Biometric.Iniciador(); // Inicializa o scanner

            // Adiciona os dispositivos encontrado disponíveis no comboBox
            comboDevice.Items.Add("Auto_Detect");
            foreach (var deviceID in devices)
            {
                switch (deviceID)
                {
                    case "FDU01":
                        comboDevice.Items.Add("FDU01");
                        break;
                }
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*
             * Captura a imagem do dedo e armazena no banco de dados             
             */

            // Instacia o objeto de biometria
            Biometric m_Biometric = new Biometric();
            m_Biometric.Captura(); // Captura a digital
        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*
            * Compara a imagem do dedo e armazenada no banco de dados   
            * com a imagem digitada e valida o usuário
            */

            // Instacia o objeto de biometria
            Biometric m_Biometric = new Biometric();
            bool validacao = m_Biometric.Comparar(); // Compara a digital

            // Lógica para validação a partir daqui

        }
    }
}
