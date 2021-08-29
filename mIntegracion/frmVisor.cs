using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using mIntegracion.Clases;


namespace mIntegracion
{
    public partial class frmVisor : Form
    {
        const string modulo = "mIntegracion";
        StringBuilder Errores = new StringBuilder();

        private SQL sqlClass;
        //private AuthLic autLic;
        private Proceso prc;
        private int mode = -1;

        public frmVisor(SQL _sqlClass, int modo)
        {
            this.sqlClass = _sqlClass;
            string version = ConfigurationManager.AppSettings["version"].ToString();
            InitializeComponent();
            lbMensaje.Text = "Usuario conectado: " + sqlClass.Usuario + " | Compañia: " + sqlClass.Compannia + " | Versión: " + version;
            //insertLicencia();
            prc = new Proceso(sqlClass);
            mode = modo;

        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {

        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
