using NITGEN.SDK.NBioBSP;
using static NITGEN.SDK.NBioBSP.NBioAPI.Type;
using static NITGEN.SDK.NBioBSP.NBioAPI.Error;
using static System.Windows.Forms.MessageBox;
using static System.Windows.Forms.MessageBoxButtons;
using static System.Windows.Forms.MessageBoxIcon;
using static System.Windows.Forms.MessageBoxOptions;
using MySql.Data.MySqlClient;
using AppEnviaEmail.src.DAO;
using AppEnviaEmail.src.Model;
using Org.BouncyCastle.Ocsp;


namespace AppEnviaEmail.src.Service
{
    class LeitorBiometrico
    {
        public static void Captura(String nome)
        {
            var nBioAPI = new NBioAPI();
            uint returnFromDeviceHandler = AbrirDispositivo(nBioAPI);

            HFIR hFIR = new HFIR();
            returnFromDeviceHandler = nBioAPI.Capture(FIR_PURPOSE.VERIFY, out hFIR,
TIMEOUT.DEFAULT, null, null);

            if (returnFromDeviceHandler != NONE)
            {
                throw new Exception("Erro ao capturar digital");
            }

            FIR biFIR;
            nBioAPI.GetFIRFromHandle(hFIR, out biFIR);
            returnFromDeviceHandler = FecharDispositivo(nBioAPI);

            byte[] biFIRData = biFIR.Data;

            FIR_TEXTENCODE textFIR;
            nBioAPI.GetTextFIRFromHandle(hFIR, out textFIR, true);

            PersistirNoDatabase(nome, textFIR);

            Show(
                $"biFIR Header: {biFIR.Header.ToString}" +
                $"\n\nbiFIR Data (Hexaedecimal): {BitConverter.ToString(biFIRData)}" +
                //$"arrayDeByteBiFIR: {hexArrayBytesBiFIR}"
                //$"\nDigital em HFIR: {hFIR.hFIR}" +
                $"\n \nDigital textual: {textFIR.TextFIR}"
                //$"\n Digital hexadecimal: {hexadecimalFIR}"
                ,
                "Digital capturada com sucesso!",
                OK,
                Information);
        }

        public static void Captura_Enroll()
        {
            var nBioAPI = new NBioAPI();
            uint returnFromDeviceHandler = AbrirDispositivo(nBioAPI);

            HFIR hFIR = new HFIR();
            returnFromDeviceHandler = nBioAPI.Enroll(out hFIR, null);

            if (returnFromDeviceHandler != NONE)
            {
                throw new Exception("Erro ao capturar digital");
            }

            FIR biFIR;
            nBioAPI.GetFIRFromHandle(hFIR, out biFIR);
            returnFromDeviceHandler = FecharDispositivo(nBioAPI);

            byte[] biFIRData = biFIR.Data;


            Show(
                $"biFIR Header: {biFIR.Header.ToString}" +
                $"biFIR Data: {BitConverter.ToString(biFIRData)}"
                //$"arrayDeByteBiFIR: {hexArrayBytesBiFIR}"
                //$"\nDigital em HFIR: {hFIR.hFIR}" +
                //$"\nDigital textual: {textFIR.TextFIR}" +
                //$"\n Digital hexadecimal: {hexadecimalFIR}"
                ,
                "Digital capturada com sucesso!",
                OK,
                Information);
        }

        private static uint FecharDispositivo(NBioAPI nBioAPI)
        {
            uint returnFromDeviceHandler = nBioAPI.CloseDevice(DEVICE_ID.AUTO);
            if (returnFromDeviceHandler != NONE)
            {
                throw new Exception("Falha ao fechar dispositivo");
            }

            return returnFromDeviceHandler;
        }

        private static uint AbrirDispositivo(NBioAPI nBioAPI)
        {
            var returnFromDeviceHandler = nBioAPI.OpenDevice(DEVICE_ID.AUTO);
            if (returnFromDeviceHandler != NONE)
            {
                throw new Exception("Falha ao abrir dispositivo");
            }

            return returnFromDeviceHandler;
        }

        private static void PersistirNoDatabase(string nome, FIR_TEXTENCODE TextFIR)
        {
            var user = new User(nome, TextFIR);
            UserDAO.PersistUser(user);
        }


        public static void CapturaFromAI()
        {
            // 1. Capturar a digital
            NBioAPI m_NBioAPI = new NBioAPI();
            HFIR hNewFIR;
            uint returnFromDevice = m_NBioAPI.Enroll(out hNewFIR, null);

            if (returnFromDevice == NONE)
            {
                // 2. Converter para binário (BLOB)
                FIR biFIR;
                m_NBioAPI.GetFIRFromHandle(hNewFIR, out biFIR);
                byte[] digitalBinaria = biFIR.Data; // Dados binários para MySQL (BLOB)

                // 3. Converter para texto codificado (TEXT)
                FIR_TEXTENCODE textFIR;
                m_NBioAPI.GetTextFIRFromHandle(hNewFIR, out textFIR, true);
                string digitalTexto = textFIR.TextFIR; // String codificada para MySQL (TEXT)

                // 4. Inserir no banco de dados
                SalvarDigitalNoMySQL("João Silva", digitalBinaria, digitalTexto);
            }
            else
            {
                Show("Falha ao capturar a digital.");
            }
        }

        public static void SalvarDigitalNoMySQL(string nome, byte[] digitalBinaria, string digitalTexto)
        {
            string connectionString = "Server=localhost;" +
               "Port=3306;" +
               "Database=database_odontologia;" +
               "User=root;" +
               "Password=";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"
            INSERT INTO usuarios (nome, digital_binaria, digital_texto)
            VALUES (@nome, @digitalBinaria, @digitalTexto)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@nome", nome);
                    cmd.Parameters.AddWithValue("@digitalBinaria", digitalBinaria); // BLOB
                    cmd.Parameters.AddWithValue("@digitalTexto", digitalTexto);     // TEXT

                    cmd.ExecuteNonQuery();
                }
            }

            Show("Digital cadastrada com sucesso!");
        }

        public bool VerificarDigital(string nome)
        {
            // 1. Capturar nova digital
            HFIR hCapturedFIR;
            m_NBioAPI.Capture(out hCapturedFIR);

            // 2. Buscar template no MySQL
            byte[] templateArmazenado = BuscarDigitalDoMySQL(nome);

            if (templateArmazenado != null)
            {
                // 3. Converter para FIR
                NBioAPI.Type.FIR firArmazenado = new NBioAPI.Type.FIR();
                firArmazenado.FIRData = templateArmazenado;

                // 4. Verificar
                bool result;
                m_NBioAPI.Verify(firArmazenado, out result, null);

                return result; // True se coincidir
            }

            return false;
        }
    }
}
