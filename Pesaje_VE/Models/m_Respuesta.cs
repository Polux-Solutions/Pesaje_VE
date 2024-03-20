using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pesaje_VE.models
{
    public class mRespuesta
    {
        public string ID { get; set; }
        public string Estado { get; set; }
        public string Mensaje_Error { get; set; }
        public int Peso { get; set; }
    }
}
