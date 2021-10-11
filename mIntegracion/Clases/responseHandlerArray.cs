using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mIntegracion
{
    public class responseHandlerArray
    {
        public string Status { get; set; }
        public IList<string> recordID { get; set; }
        public IList<string> Reason { get; set; }
    }
}
