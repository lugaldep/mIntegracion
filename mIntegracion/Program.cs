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
            string user, pass, dataBase, server, cia, row, error = string.Empty;
            string ejec = "0";
            string idDoc = string.Empty;
            string docGen = string.Empty;
            string tipoDoc = string.Empty;
            bool okFlag = true;
#if DEBUG
            args = new string[6];

            //args[0] = "sa";
            //args[1] = "spTIF$7295";
            //args[2] = "SOFTLAND";
            //args[3] = "172.16.10.39";
            //args[4] = "FARYVET";
            //args[5] = "2";


            //args[0] = "sa";
            //args[1] = "sql@2019";
            //args[2] = "NOSARA";
            //args[3] = @"LUGALDE-SPEC\SQL2019";
            //args[4] = "MANTNOSRA";
            //args[5] = "0";


            args[0] = "sa";
            args[1] = "lL@Pe!rBe*";
            args[2] = "PRUEBAS";
            args[3] = @"172.30.1.65\DESARROLLO,14334";
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
                row = string.Empty;
                ejec = args[5];

                //se realiza una conexión de prueba
                if (okFlag)
                {
                    try
                    {
                        if (ConfigurationManager.AppSettings["Debug"].ToString().CompareTo("True") == 0)
                            MessageBox.Show("Información de conexión\n Srv: " + server + "\n BD: " + dataBase + "\n Usr: " + user + "\n Pwd: " + pass + "\n Cia: " + cia + "\n Row: " + row, "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);

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
                        //else if (ejec.CompareTo("1") == 0) //Visor estandar sin CxC
                        //{
                        //    Application.Run(new frmVisor(sqlClass, 1));
                        //}
                        //else if (ejec.CompareTo("2") == 0) //Visor de generacion CxC
                        //{
                        //    Application.Run(new frmGenDocs(sqlClass));
                        //}
                        //else if (ejec.CompareTo("3") == 0) //Desatendido sin CxC los documentos
                        //{
                        //    Globales glb = new Globales(sqlClass);
                        //    Facturacion fac = new Facturacion(sqlClass);
                        //    //CuentasCobrar cc = new CuentasCobrar(sqlClass);
                        //    System.Data.DataTable dtG = glb.getGlobales(sqlClass);
                        //    Proceso prc = new Proceso(sqlClass);
                        //    StringBuilder Errores = new StringBuilder();

                        //    //genera pedidos
                        //    okFlag = fac.generaPedido(dtG, ref idDoc, ref docGen, ref Errores);
                        //    if (!okFlag)
                        //        prc.insBitacoraINT("P", idDoc, "E", docGen, Errores.ToString(), ref Errores);

                        //    //genera devoluciones
                        //    if (okFlag)
                        //    {
                        //        okFlag = fac.generaDevolucion(dtG, ref idDoc, ref docGen, ref Errores);
                        //        if (!okFlag)
                        //            prc.insBitacoraINT("D", idDoc, "E", docGen, Errores.ToString(), ref Errores);
                        //    }
                        //    ////genera documentosCC
                        //    //if (okFlag)
                        //    //{
                        //    //    okFlag = cc.generaDocumentoCC(dtG, ref tipoDoc, ref idDoc, ref docGen, ref Errores);
                        //    //    if (!okFlag)
                        //    //        prc.insBitacoraINT(tipoDoc, idDoc, "E", docGen, Errores.ToString(), ref Errores);
                        //    //}                            
                        //}
                        //else if (ejec.CompareTo("4") == 0) //Desatendido todos los documentos
                        //{
                        //    Globales glb = new Globales(sqlClass);
                        //    Facturacion fac = new Facturacion(sqlClass);
                        //    CuentasCobrar cc = new CuentasCobrar(sqlClass);
                        //    System.Data.DataTable dtG = glb.getGlobales(sqlClass);
                        //    Proceso prc = new Proceso(sqlClass);
                        //    StringBuilder Errores = new StringBuilder();
                        //    /* 0 = Consecutivo del ERP
                        //     * 1 = ID de Pocket Link */
                        //    string modoConsec = ConfigurationManager.AppSettings["modoConsec"].ToString();

                        //    //genera pedidos
                        //    okFlag = fac.generaPedido(dtG, ref idDoc, ref docGen, ref Errores);
                        //    if (!okFlag)
                        //        prc.insBitacoraINT("P", idDoc, "E", docGen, Errores.ToString(), ref Errores);

                        //    //genera devoluciones
                        //    if (okFlag)
                        //    {
                        //        okFlag = fac.generaDevolucion(dtG, ref idDoc, ref docGen, ref Errores);
                        //        if (!okFlag)
                        //            prc.insBitacoraINT("D", idDoc, "E", docGen, Errores.ToString(), ref Errores);
                        //    }
                        //    //genera documentosCC
                        //    if (okFlag)
                        //    {
                        //        okFlag = cc.generaDocumentoCC(dtG, modoConsec, ref tipoDoc, ref idDoc, ref docGen, ref Errores);
                        //        if (!okFlag)
                        //            prc.insBitacoraINT(tipoDoc, idDoc, "E", docGen, Errores.ToString(), ref Errores);
                        //    }
                        //}

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

            //if (!okFlag)
            //{
            //    if (ejec.CompareTo("0") == 0)                
            //        MessageBox.Show(errores.ToString(), "Módulo de Integración", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}

        }

    }
}
