using NITGEN.SDK.NBioBSP;
using AppEnviaEmail.src.Model;
using AppEnviaEmail.src.DAO;

using static NITGEN.SDK.NBioBSP.NBioAPI.Type;
using static NITGEN.SDK.NBioBSP.NBioAPI.Error;
using static System.Windows.Forms.MessageBox;
using static System.Windows.Forms.MessageBoxButtons;
using static System.Windows.Forms.MessageBoxIcon;

namespace AppEnviaEmail.src.Service
{
    class LeitorDigital
    {
        private static FIR? InMemoryBinaryFIR;
        private static string digitalFIRHex;
        private static object digitalFIRText;

        public static FIR? CapturaDigitalSemPersistir(string nome)
        {
            uint ret = 0;

            MessageBox.Show("Posicione o dedo no scanner.", "Captura", MessageBoxButtons.OK, MessageBoxIcon.Information);

            var apiNBIO = new NBioAPI();
            HFIR digitalImage; // Imagem da digital (Handle to Full)

            try
            {
                // O que é ret (?????)
                // Inicia o scanner plugado e obtém o ID do dispositivo
                apiNBIO.OpenDevice(DEVICE_ID.AUTO);

                // Captura única com callback  
                apiNBIO.Capture(FIR_PURPOSE.VERIFY, out digitalImage, TIMEOUT.DEFAULT, null, null);

                if (ret != NONE) // Caso dedo não seja posicionado
                {
                    MessageBox.Show("Dedo não detectado.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return null;
                }

                FIR currentBinaryFIR;
                apiNBIO.GetFIRFromHandle(digitalImage, out currentBinaryFIR);
                InMemoryBinaryFIR = currentBinaryFIR;

                FIR_TEXTENCODE digitalText;
                apiNBIO.GetTextFIRFromHandle(digitalImage, out digitalText, true);

                digitalFIRHex = BitConverter.ToString(currentBinaryFIR.Data).Replace("-", "");
                digitalFIRText = digitalText.TextFIR;

                Show("Digital capturada com sucesso", "Sucesso", OK, Information);

                //UserDAO.BuscarDigital(new User(nome, currentBinaryFIR));

                //bool isDigitalExists = UserDAO.BuscarDigital(
                //    new User(nome, currentBinaryFIR)
                //    );

                /*
                if (!isDigitalExists)
                {
                    Show("Digital não existe no banco de dados!", "Aviso", OK, Warning);
                }
                else
                {
                    Show("Digital existe no banco de dados!", "Existente", OK, Information);

                }
                */

            }
            catch (Exception e)
            {
                Console.WriteLine($"Ocorreu um erro {e.Message}");
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                ret = apiNBIO.CloseDevice(DEVICE_ID.AUTO);
            }

            if (ret != NONE)
            {
                Show("Falha ao fechar dispositivo.", "Erro", OK, Error);
            }

            Show("Scanner encerrado.", "Encerramento", OK, Information);

            return InMemoryBinaryFIR;
        }
    }
}
