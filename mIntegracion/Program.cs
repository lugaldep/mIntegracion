using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text;
using System.Configuration;
using mIntegracion.Clases;

namespace mIntegracion
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            SQL sqlClass = null;
            StringBuilder errores = new StringBuilder();
            string user, pass, dataBase, server, cia = string.Empty;
            string ejec = "0";         
            bool okFlag = true;
#if DEBUG
            args = new string[6];

            //args[0] = "sa";
            //args[1] = "sql@2019";
            //args[2] = "NOSARA";
            //args[3] = @"LUGALDE-SPEC\SQL2019";
            //args[4] = "MANTNOSRA";
            //args[5] = "0";


            //args[0] = "sa";
            //args[1] = "lL@Pe!rBe*";
            //args[2] = "PRUEBAS";
            //args[3] = @"172.30.1.65\DESARROLLO,14334";
            //args[4] = "PRINCIPAL";
            //args[5] = "0";



            args[0] = "erpadmin";
            args[1] = "PlazaBratsi1";
            args[2] = "PORTAFOLIO";
            args[3] = @"172.30.1.65";
            args[4] = "PRINCIPAL";
            args[5] = "0";

#endif
            //$U$P$B$S$C

            if (args.Length > 0 && args.Length == 6)
            {
                user = args[0];
                pass = args[1];
                dataBase = args[2];
                server = args[3];
                cia = args[4];                
                ejec = args[5];

                //se realiza una conexión de prueba
                if (okFlag)
                {
                    try
                    {
                        if (ConfigurationManager.AppSettings["Debug"].ToString().CompareTo("True") == 0)
                            MessageBox.Show("Información de conexión\n Srv: " + server + "\n BD: " + dataBase + "\n Usr: " + user + "\n Pwd: " + pass + "\n Cia: " + cia, "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

                        sqlClass = new SQL(user, pass, dataBase, server, cia);
                    }
                    catch (SqlException sqlEx)
                    {
                        errores.Append("No fue posible realizar una conexión válida con la base de datos. Detalle técnico: ");
                        errores.Append(sqlEx.Message);
                        okFlag = false;
                    }
                }

                if (okFlag)
                {
                    try
                    {
                        string licMsg = string.Empty;
                        string value = ConfigurationManager.AppSettings["reflectionCode"].ToString();
                        string amt = ConfigurationManager.AppSettings["reflectionUser"].ToString();
                        tkEncrypterClass.License lic = new tkEncrypterClass.License();

                        //AuthLic autLic = new AuthLic(sqlClass);

                        //if (!lic.isValid(value, ref licMsg))
                        //{
                        //    MessageBox.Show("La licencia del desarrollo ha caducado. Por favor póngase en contacto con el administrador del sistema, para que este contacte al proveedor. " + licMsg, "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        //    Application.Exit();
                        //}
                        //else
                        //{        

                        //Visor estandar todos los documentos
                        if (ejec.CompareTo("0") == 0)
                        {
                            Application.Run(new frmVisor(sqlClass, 0));
                        }
                        else if (ejec.CompareTo("1") == 0) //Corriendo desde tarea programada
                        {
                            Proceso prc = new Proceso(sqlClass);
                            StringBuilder Errores = new StringBuilder();
                            System.Data.DataTable dt = prc.getIntGlobales();
                            string ck = dt.Rows[0]["CONSUMER_KEY"].ToString();
                            string cs = dt.Rows[0]["CONSUMER_SECRET"].ToString();
                            string url = dt.Rows[0]["SERVICE"].ToString();

                            //proceso extraccion
                            prc.extractAnticipos(ref Errores);
                            prc.extractSolicitudPagos(ref Errores);
                            prc.extractPagos(ref Errores);
                            prc.extractNotasCredito(ref Errores);
                            //cias
                            prc.syncCompanias(url, ck, cs, ref Errores);
                            //prov
                            prc.syncProveedores(url, ck, cs, ref Errores);
                            //solpagos
                            prc.syncSolicitudesPago(url, ck, cs, ref Errores);
                            //pagos
                            prc.syncPagos(url, ck, cs, ref Errores);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("El desarrollo no ha detectado la licencia del producto. Por favor póngase en contacto con el administrador del sistema, para que este contacte al proveedor. " + ex.Message.ToString(), "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                errores.Append("Los parámetros de inicio no fueron definidos o alguno de ellos no fue localizado");
                okFlag = false;
            }

            if (!okFlag)
            {
                if (ejec.CompareTo("0") == 0)
                    MessageBox.Show(errores.ToString(), "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

    }
}
