using NITGEN.SDK.NBioBSP;
using MySql.Data.MySqlClient;
using AppEnviaEmail.src.DAO;
using AppEnviaEmail.src.Model;
using Org.BouncyCastle.Ocsp;

using static NITGEN.SDK.NBioBSP.NBioAPI.Type;
using static NITGEN.SDK.NBioBSP.NBioAPI.Error;
using static System.Windows.Forms.MessageBox;
using static System.Windows.Forms.MessageBoxButtons;
using static System.Windows.Forms.MessageBoxIcon;
using static System.Windows.Forms.MessageBoxOptions;
using static AppEnviaEmail.src.DAO.UserDAO;


namespace AppEnviaEmail.src.Service
{
    class LeitorBiometrico
    {
        public static void CadastrarDigitais(string nome)
        {
            NBioAPI api = new NBioAPI();
            api.OpenDevice(DEVICE_ID.AUTO);

            try
            {

                HFIR identificadorFIR;
                uint apiResult = api.Enroll(out identificadorFIR, null);

                if (apiResult != NONE)
                {
                    Show("Falha ao capturar a digital.", "Falha", OK, Warning);
                }
                else
                {
                    FIR binaryFingerImageRecord;
                    api.GetFIRFromHandle(identificadorFIR, out binaryFingerImageRecord);

                    byte[] digitalBinaria = binaryFingerImageRecord.Data;

                    PersistUser(new User(
                        nome, digitalBinaria
                        ));

                    Show("Digital cadastrada com sucesso!", "Cadastrado", OK, Information);
                }
            }
            catch (Exception ex)
            {
                Show(ex.Message);
            }
            finally
            {
                api.CloseDevice(DEVICE_ID.AUTO);
            }
        }
        public static void VerificarBiometria(string nome)
        {
            NBioAPI api = new NBioAPI();
            api.OpenDevice(DEVICE_ID.AUTO);

            try
            {
                byte[] templateArmazenadoDB = BuscarDigitalPorNome(nome);
                FIR firDoTemplateArmazenadoDB = new FIR();

                if (templateArmazenadoDB.Length == 0)
                {
                    Show("Sem resultados no banco de dados");
                }
                else
                {
                    firDoTemplateArmazenadoDB.Data = templateArmazenadoDB; // Converte binário para FIR
                }


                uint returnFromAPI;
                bool matchResult;


                FIR_PAYLOAD myPayload = new FIR_PAYLOAD();
                // Verifica o FIR em formato binário 
                returnFromAPI = api.Verify(firDoTemplateArmazenadoDB, out matchResult, myPayload);
                if (returnFromAPI == NONE)
                {
                    // Verificado com sucesso 
                    // Payload checado 
                    if (myPayload.Data == null)
                    {
                        //var textPayload.Text = myPayload.Data;
                        Show("Biometria encontrada com sucesso!", "Encontrada", OK, Information);
                    }
                }
                else Show("Biometria não localizada ou inexistente.", "Não localizada", OK, Warning);
            }
            catch (Exception ex)
            {
                Show(ex.Message);
            }
            finally
            {
                api.CloseDevice(DEVICE_ID.AUTO);
            }
        }
    }
}

