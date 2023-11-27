using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mIntegracion.Clases
{
    public class recordIDP
    {
        public string idmovimiento { get; set; }
        public string record { get; set; }
    }


    public class ReasonP
    {
        public string idmovimiento { get; set; }
        public string error { get; set; }
    }


    public class responseHandlerPresupuestos
    {
        public string Status { get; set; }
        public IList<recordIDP> recordID { get; set; }
        public IList<ReasonP> Reason { get; set; }
    }
}
