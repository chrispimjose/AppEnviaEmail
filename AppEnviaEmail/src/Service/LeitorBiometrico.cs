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

            PersistirNoDatabase(nome, textFIR.TextFIR);

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

        private static void PersistirNoDatabase(string nome, string strFIRText)
        {
            var user = new User(nome, strFIRText);
            UserDAO.PersistUser(user);
        }
    }
}
