using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NITGEN.SDK.NBioBSP;
using static NITGEN.SDK.NBioBSP.NBioAPI.Type;

namespace AppEnviaEmail.src.Model
{
    internal class User
    {
        private string nome;
        private FIR biFIR;

        public string Name { get; set; }
        public FIR biFIR { get; set; }

        public User(string nome, FIR binaryFingerPrintRecord)
        {
            this.nome = nome;
            this.biFIR = binaryFingerPrintRecord;
        }
    }
}
