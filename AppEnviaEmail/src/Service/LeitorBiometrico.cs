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
                uint returnFromAPI;
                byte[] templateArmazenadoDB = BuscarDigitalPorNome(nome);
                bool verifyResult;

                if (templateArmazenadoDB.Length == 0)
                {
                    Show("Sem resultados no banco de dados");
                }

                FIR firDoTemplateArmazenadoDB = new FIR();
                firDoTemplateArmazenadoDB.Data = templateArmazenadoDB;

                // 4. Capturar uma nova digital para comparação
                HFIR hCapturedFIR;
                returnFromAPI = api.Capture(out hCapturedFIR);
                if (returnFromAPI == NONE)
                {
                    // 5. Converter o FIR capturado para o mesmo formato
                    FIR capturedFIR;
                    api.GetFIRFromHandle(hCapturedFIR, out capturedFIR);

                    // 6. Comparar as digitais
                    bool matchResult;
                    FIR_PAYLOAD payload = new FIR_PAYLOAD();
                    returnFromAPI = api.Verify(firDoTemplateArmazenadoDB, out matchResult, payload);

                    if (returnFromAPI == NONE && matchResult)
                    {
                        Show("Digital corresponde", "Verificado com sucesso");
                        Show("Biometria encontrada com sucesso!", "Encontrada", OK, Information);
                    }
                    else
                    {
                        Show("Biometria não localizada ou inexistente.", "Não localizada", OK, Warning);
                        Console.WriteLine("Digital NÃO corresponde!");
                    }
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
    }
}

