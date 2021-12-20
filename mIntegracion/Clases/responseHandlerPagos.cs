using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mIntegracion.Clases
{    
    public class recordID
    {
        public string idpago { get; set; }
        public string record { get; set; }
    }


    public class Reason
    {
        public string idpago { get; set; }
        public string error { get; set; }
    }


    public class responseHandlerPagos
    {
        public string Status { get; set; }
        public IList<recordID> recordID { get; set; }
        public IList<Reason> Reason { get; set; }
    }
    
}
