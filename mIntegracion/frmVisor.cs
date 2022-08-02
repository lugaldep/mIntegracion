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
using System.Data.SqlClient;
using RestSharp;




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
        private string ck, cs, url;

        public frmVisor(SQL _sqlClass, int modo)
        {
            this.sqlClass = _sqlClass;
            string version = ConfigurationManager.AppSettings["version"].ToString();
            InitializeComponent();
            lbMensaje.Text = "Usuario conectado: " + sqlClass.Usuario + " | Compañia: " + sqlClass.Compannia + " | Versión: " + version;
            //insertLicencia();
            prc = new Proceso(sqlClass);
            mode = modo;
            cargarGlobales();


        }

        /// <summary>
        /// método que carga las globales 
        /// </summary>
        private void cargarGlobales()
        {
            try
            {
                DataTable dt = prc.getIntGlobales();
                ck = dt.Rows[0]["CONSUMER_KEY"].ToString();
                cs = dt.Rows[0]["CONSUMER_SECRET"].ToString();
                url = dt.Rows[0]["SERVICE"].ToString();

            }
            catch (Exception ex)
            {
                Errores.AppendLine("Problemas obteniendo las globales. Detalle Técnico: ");
                Errores.AppendLine(ex.Message);
            }
        }

        /// <summary>
        /// método que obtiene los calculos 
        /// </summary>
        private void getBitacora()
        {
            DateTime fcInicio = new DateTime();
            DateTime fcFin = new DateTime();

            try
            {
                System.Data.DataTable dt = null;
                string estado = (cbEstado.Text != null ? cbEstado.Text : "");
                string tipo = (cbTipo.Text != null ? cbTipo.Text : "");

                if (tipo.CompareTo(string.Empty) != 0)
                {


                    //Fecha de Creacion
                    if (dtCreacionInicio.Text.CompareTo(" ") != 0)
                        fcInicio = dtCreacionInicio.Value;
                    else
                        fcInicio = DateTime.MinValue;

                    if (dtCreacionFin.Text.CompareTo(" ") != 0)
                        fcFin = dtCreacionFin.Value;
                    else
                        fcFin = DateTime.MinValue;

                    dt = prc.getBitacora(estado, tipo, fcInicio, fcFin);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        dg.DataSource = dt;
                        dg.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                        dg.AutoResizeColumns();
                    }
                    else
                        dg.DataSource = null;
                }
                else
                {
                    MessageBox.Show("Debe seleccionar el tipo de documento para el filtrado de la bitacora. Seleccione alguno de los valores e intente de nuevo." , "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception sqlEx)
            {
                Errores.AppendLine("Problemas obteniendo la bitacora. Detalle Técnico: ");
                Errores.AppendLine(sqlEx.Message);
            }
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            getBitacora();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void correrExtraccion()
        {
            bool lbOk = true;
            bool gOk = true;

            try
            {
                lbOk = prc.extractAnticipos(ref Errores);
                if (lbOk)
                {
                    MessageBox.Show("El proceso de extracción de anticipos finalizó correctamente. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    gOk = false;


                lbOk = prc.extractSolicitudPagos(ref Errores);
                if (lbOk)
                {
                    MessageBox.Show("El proceso de extracción de solicitudes de pago finalizó correctamente. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    gOk = false;


                lbOk = prc.extractPagos(ref Errores);
                if (lbOk)
                {
                    MessageBox.Show("El proceso de extracción de pagos finalizó correctamente. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    gOk = false;


                lbOk = prc.extractNotasCredito(ref Errores);
                if (lbOk)
                {
                    MessageBox.Show("El proceso de extracción de notas de crédito finalizó correctamente. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    gOk = false;


                if (gOk)
                {
                    MessageBox.Show("El proceso de extracción finalizó correctamente. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (!gOk)
                    MessageBox.Show("Ocurrieron errores: " + Errores.ToString(), "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ocurrieron errores: " + Ex.Message.ToString(), "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        private void correrSync()
        {
            bool lbOk = true;
            bool gOk = true; 

            try
            {
                lbOk = prc.syncCompanias(url, ck, cs, ref Errores);
                if (lbOk)
                {
                    MessageBox.Show("El proceso de sincronización de compañías finalizó correctamente. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    gOk = false;

                lbOk = prc.syncProveedores(url, ck, cs, ref Errores);
                if (lbOk)
                {
                    MessageBox.Show("El proceso de sincronización de proveedores finalizó correctamente. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    gOk = false;


                lbOk = prc.syncSolicitudesPago(url, ck, cs, ref Errores);
                if (lbOk)
                {
                    MessageBox.Show("El proceso de sincronización de solicitudes de pago finalizó correctamente. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    gOk = false;

                lbOk = prc.syncPagos(url, ck, cs, ref Errores);
                if (lbOk)
                {
                    MessageBox.Show("El proceso de sincronización de pagos finalizó correctamente. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                    gOk = false;




                if (gOk)
                {
                    MessageBox.Show("El proceso de sincronización finalizó correctamente. " , "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (!gOk)
                    MessageBox.Show("Ocurrieron errores: " + Errores.ToString(), "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ocurrieron errores: " + Ex.Message.ToString(), "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }


        private void btnProceso_Click(object sender, EventArgs e)
        {
            correrExtraccion();

            correrSync();
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            getBitacora();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                app.Visible = true;
                app.Application.Interactive = false;
                Microsoft.Office.Interop.Excel.Workbook wb = app.Workbooks.Add(1);
                Microsoft.Office.Interop.Excel.Worksheet ws = (Microsoft.Office.Interop.Excel.Worksheet)wb.Worksheets[1];
                // changing the name of active sheet
                ws.Name = "Bitacora";

                ws.Rows.HorizontalAlignment = HorizontalAlignment.Center;

                // storing header part in Excel
                for (int i = 1; i < dg.Columns.Count + 1; i++)
                {
                    ws.Cells[1, i] = dg.Columns[i - 1].HeaderText;
                }

                // storing Each row and column value to excel sheet
                for (int i = 0; i < dg.Rows.Count; i++)
                {
                    for (int j = 0; j < dg.Columns.Count; j++)
                    {
                        ws.Cells[i + 2, j + 1] = dg.Rows[i].Cells[j].Value.ToString();
                    }
                }

                // sizing the columns
                ws.Cells.EntireColumn.AutoFit();
                app.Application.Interactive = true;

                //Notificacion de exportacion
                MessageBox.Show("Archivo Excel generado correctamente!. ", "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception Ex)
            {
                MessageBox.Show("Ocurrieron errores exportando la información a Excel. \n " + Ex.Message.ToString(), "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void frmVisor_Load(object sender, EventArgs e)
        {
            dtCreacionInicio.CustomFormat = " ";
            dtCreacionFin.CustomFormat = " ";
            cbTipo.SelectedIndex = 0;

            if(ConfigurationManager.AppSettings["Debug"].ToString().CompareTo("True")==0)
            {
                txtJsons.Visible = true;
                btnGeneraJsons.Visible = true;
            }


        }

        private void btnGeneraJsons_Click(object sender, EventArgs e)
        {   
            DataTable dtDocs = null;
            StringBuilder json = new StringBuilder();
            jsonHandler j = new jsonHandler();

            try
            {
                //obtener encabezado
                json.Append(j.getHeader(ck, cs, "pago"));

                //abrir arreglo de docs
                //json.Append(j.getOpenArray());

                //select de los pagos a sincronizar
                dtDocs = prc.getPagosCPSyncGEN();
                
                //genero json list
                json.Append(j.DataTableToJson(dtDocs));                

                //obtener footer
                json.Append(j.getFooter());


                txtJsons.Text = json.ToString();
                
            }
            catch (Exception ex)
            {   
                txtJsons.Text = ex.Message.ToString();
            }
        }


        private void dtCreacionInicio_ValueChanged(object sender, EventArgs e)
        {
            dtCreacionInicio.CustomFormat = "dd/MM/yyyy";
        }

        private void dtCreacionFin_ValueChanged(object sender, EventArgs e)
        {
            dtCreacionFin.CustomFormat = "dd/MM/yyyy";
        }
    }
}



//jsonHandler j = new jsonHandler();

//DataTable dt = prc.getCompaniasSync();

//string json = new JObject(
//            dt.Columns.Cast<DataColumn>()
//            .Select(c => new JProperty(c.ColumnName, JToken.FromObject(dt.Rows[1][c])))
//            ).ToString(Formatting.None);

//rtb.AppendText(json);


/*


string json = "{\"conexion\": {\"consumer_key\": \"cd8bcsgff8wccbjv9ue9c46dakc\",\"consumer_secret\": \"bz9ry3_cv58_0_b72tsyzdwe2gj7dfvr8bdctx6q4f\",\"type\":\"compania\",\"compania\": 	{		\"nombre\": \"Bahia San Felipe, S.A.\",	\"telefono\": \"22880101\",		\"nit\": \"3101371510\",		\"codigo\": \"BSF\",		\"imagnombre\": \"Logo_BSF.BMP\"	}}}";

var client = new RestClient("https://smarttools.smartstrategyapps.com/pinmsa/ControlDeProyectos2021/IntegracionControlProyecto.php");

//var request = new RestRequest(json, DataFormat.Json);
//var response = client.Execute(request);

var request = new RestRequest(Method.POST);
request.AddParameter("application/json", json, ParameterType.RequestBody);
IRestResponse response = client.Execute(request);

response.StatusCode.ToString();


*/

