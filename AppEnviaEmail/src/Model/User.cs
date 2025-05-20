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
        public string Name { get; set; }
        public FIR_TEXTENCODE TextFIR { get; set; }

        public User(string nome, FIR_TEXTENCODE textFingerPrintRecord)
        {
            this.Name = nome;
            this.TextFIR = textFingerPrintRecord;
        }
    }
}
