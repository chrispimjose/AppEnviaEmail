using System.Text.RegularExpressions;
using AppEnviaEmail.src.Service;
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
        FIR biFIR1; // objeto que armazena a digital em binário
        string strFIRText15; // variável para armazenar a string de 15 caracteres
        private TextBox txtNome; 
        private Label lblNome;


        public Form1()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            ConfigurarLabelNome();
        }

        private void ConfigurarLabelNome()
        {
            lblNome = new Label();
            lblNome.Text = "Nome:";
            lblNome.Location = new Point(20, 20);
            lblNome.Size = new Size(100, 20);
            this.Controls.Add(lblNome);

            txtNome = new TextBox();
            txtNome.Location = new Point(120, 20);
            txtNome.Size = new Size(200, 20);
            this.Controls.Add(txtNome);
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


            //Biometric biometric = new Biometric(); // Classe de Padilha


            string nome = txtNome.Text.Trim();
            LeitorBiometrico.Captura(nome);

        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            /*
             * Captura e compara a biometria
             */

            //Biometric biometric = new Biometric(); // Classe de Padilha

            string nome = txtNome.Text.Trim();
            LeitorBiometrico.Comparar(nome);
        }
    }
}
