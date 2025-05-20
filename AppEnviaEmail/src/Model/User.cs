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
        public byte[] DigitalBinaria { get; set; }

        public User(string nome, byte[] digitalBinaria)
        {
            this.Name = nome;
            this.DigitalBinaria = digitalBinaria;
        }
    }
}
