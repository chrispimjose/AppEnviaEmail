using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppEnviaEmail.src.Model
{
    internal class User
    {
        public string Name { get; set; }
        public long DigitalCode { get; set; }

        public User(string name, long digitalCode)
        {
            this.Name = name;
            this.DigitalCode = digitalCode;
        }
    }
}
