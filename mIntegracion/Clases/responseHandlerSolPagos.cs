using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mIntegracion.Clases
{
    public class recordIDS
    {
        public string idsolicitud { get; set; }
        public string record { get; set; }
    }


    public class ReasonS
    {
        public string idsolicitud { get; set; }
        public string error { get; set; }
    }


    public class responseHandlerSolPagos
    {
        public string Status { get; set; }
        public IList<recordIDS> recordID { get; set; }
        public IList<ReasonS> Reason { get; set; }
    }
}
