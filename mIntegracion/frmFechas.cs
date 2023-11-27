using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mIntegracion
{
    public partial class frmFechas : Form
    {
        public DateTime fi;
        public DateTime ff;
        public frmFechas()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnContinuar_Click(object sender, EventArgs e)
        {
            fi = dtCreacionInicio.Value;
            ff = dtCreacionFin.Value;
            this.Close();

        }
    }
}
