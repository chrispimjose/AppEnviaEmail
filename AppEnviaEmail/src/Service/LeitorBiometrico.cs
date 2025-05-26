using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppEnviaEmail.src.DAO;
using NITGEN.SDK.NBioBSP;
using AppEnviaEmail.src.Model;

using static NITGEN.SDK.NBioBSP.NBioAPI;
using static NITGEN.SDK.NBioBSP.NBioAPI.Type;

namespace AppEnviaEmail.src.Service
{
    class LeitorBiometrico
    {
        private static uint AbrirDevice(NBioAPI api)
        {
            using (api)
            {
                uint returnFromAPI = api.OpenDevice(DEVICE_ID.AUTO);

                if (returnFromAPI != Error.NONE)
                {
                    throw new Exception("Erro ao abrir o leitor");
                }

                return returnFromAPI;
            }
        }

        private static uint FecharDevice(NBioAPI api)
        {
            using (api)
            {
                uint returnFromAPI = api.CloseDevice(DEVICE_ID.AUTO);

                if (returnFromAPI != Error.NONE)
                {
                    throw new Exception("Erro ao fechar leitor");
                }

                return returnFromAPI;
            }
        }

        internal static void Captura(string nome)
        {
            using (var api = new NBioAPI())
            {
                uint returnFromAPI = api.OpenDevice(DEVICE_ID.AUTO);
                if (returnFromAPI != Error.NONE)
                {
                    throw new Exception("Erro ao abrir o leitor");
                }
                Console.WriteLine("Leitor aberto com sucesso.");

                HFIR hFIR;
                FIR_TEXTENCODE textTemplate;

                returnFromAPI = api.Enroll(out hFIR, null);
                if (returnFromAPI != Error.NONE)
                {
                    throw new Exception("Erro ao capturar biometria");
                }

                api.GetTextFIRFromHandle(hFIR, out textTemplate, true);
                UserDAO.Create(new User(
                    nome,
                    textTemplate.TextFIR
                ));

                returnFromAPI = FecharDevice(api);
                Console.WriteLine("Leitor fechado com sucesso.");
            }
        }

        internal static void Comparar(string nome)
        {
            using (var api = new NBioAPI())
            {
                uint returnFromAPI;
                bool result;

                User storedUser = UserDAO.FindByUserName(nome);
                if (storedUser == null)
                {
                    // throw new Exception("Erro usuáro inexistente");

                    MessageBox.Show(
                        "Nome inserido não consta no banco de dados",
                        "Usuário inexistente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                FIR_TEXTENCODE storedTextFIR = new FIR_TEXTENCODE();
                storedTextFIR.TextFIR = storedUser.textTemplate;

                returnFromAPI = api.OpenDevice(DEVICE_ID.AUTO);
                if (returnFromAPI != Error.NONE)
                {
                    throw new Exception("Erro ao inicializar dispositivo");
                }



                FIR_PAYLOAD myPayload = new FIR_PAYLOAD();

                uint retornoVerificacao = api.Verify(storedTextFIR, out result, myPayload);
                if (retornoVerificacao != Error.NONE)
                {
                    MessageBox.Show("Erro na verificação!");
                    return;
                }

                if (result)
                {
                    // Verifica ou não existência de payload 
                    //if (myPayload.Data != null)
                    //{
                    //    MessageBox.Show("Payload Encontrado");
                    //}
                    //else
                    //{
                    //    MessageBox.Show("Payload não encontrado");
                    //}

                    MessageBox.Show(
                        "Digitais conferem, usuário existe no banco de dados",
                        "Digitais conferem",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Digitais não conferem",
                        "Digitais não conferem",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }

                returnFromAPI = api.CloseDevice(DEVICE_ID.AUTO);
                if (returnFromAPI != Error.NONE)
                {
                    throw new Exception("Erro ao fechar dispositivo");
                }
            }
        }
    }
}
