using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Configuration;

namespace mIntegracion.Clases
{

    public class Globales
    {
        private SQL sqlClass;

        public Globales(SQL _sqlClass)
        {
            this.sqlClass = _sqlClass;
        }

        /// <summary>
        /// Metodo encargado de extraer las bodegas desde Exactus       
        /// </summary>
        public DataTable getBodegas(SQL _sqlClass)
        {
            DataTable dtBodega = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select BODEGA Bodega, BODEGA + ' - ' + NOMBRE Descripcion from ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".BODEGA ");
            sentencia.Append("ORDER BY 1 ASC");

            dtBodega = _sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dtBodega;
        }


        /// <summary>
        /// Metodo encargado de extraer el consecutivo de pedidos       
        /// </summary>
        public DataTable getConsecutivo(SQL _sqlClass)
        {
            DataTable dtConsecutivo = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CONSECUTIVO , CONSECUTIVO + ' - ' + DESCRIPCION descripcion from ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".CONSECUTIVO ");
            sentencia.Append("ORDER BY 1 ASC");

            dtConsecutivo = _sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dtConsecutivo;
        }

        /// <summary>
        /// Metodo encargado de extraer el consecutivo de pedidos       
        /// </summary>
        public DataTable getConsecutivoCI(SQL _sqlClass)
        {
            DataTable dtConsecutivo = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CONSECUTIVO , CONSECUTIVO + ' - ' + DESCRIPCION descripcion from ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".CONSECUTIVO_CI ");
            sentencia.Append("ORDER BY 1 ASC");

            dtConsecutivo = _sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dtConsecutivo;
        }


        /// <summary>
        /// Metodo encargado de extraer el consecutivo de pedidos       
        /// </summary>
        public DataTable getPaqueteCI(SQL _sqlClass)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select PAQUETE_INVENTARIO , PAQUETE_INVENTARIO + ' - ' + DESCRIPCION descripcion from ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".PAQUETE_INVENTARIO ");
            sentencia.Append("ORDER BY 1 ASC");

            dt = _sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }


        /// <summary>
        /// Metodo encargado de extraer las globales       
        /// </summary>
        public DataTable getGlobales(SQL _sqlClass)
        {
            DataTable dtGlobales = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append(" CB_PAGOS ");
            sentencia.Append(",CB_PAGOS + ' - ' + cbp.DESCRIPCION CB_PAGOS_DESC ");
            sentencia.Append(",CC_PAGOS ");
            sentencia.Append(",CC_PAGOS + ' - ' + ccp.DESCRIPCION CC_PAGOS_DESC ");
            sentencia.Append(",CC_INVERSION ");
            sentencia.Append(",CC_INVERSION + ' - ' + cciv.DESCRIPCION CC_INVERSION_DESC ");
            sentencia.Append(",CC_INV_INTERES ");
            sentencia.Append(",CC_INV_INTERES + ' - ' + ccin.DESCRIPCION CC_INV_INTERES_DESC ");
            sentencia.Append(",CC_INV_COMISION ");
            sentencia.Append(",CC_INV_COMISION + ' - ' + ccc.DESCRIPCION CC_INV_COMISION_DESC ");
            sentencia.Append(",FOLDERS from ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".MIV_GLOBALES g left join ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".CONSECUTIVO ccp on g.CC_PAGOS = ccp.CONSECUTIVO left join  ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".CONSECUTIVO cbp on g.CB_PAGOS = cbp.CONSECUTIVO left join ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".CONSECUTIVO cciv on g.CC_INVERSION = cciv.CONSECUTIVO left join ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".CONSECUTIVO ccin on g.CC_INV_INTERES = ccin.CONSECUTIVO left join ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".CONSECUTIVO ccc on g.CC_INV_COMISION = ccc.CONSECUTIVO ");

            dtGlobales = _sqlClass.EjecutarConsultaDS(sentencia.ToString());
            return dtGlobales;
        }


        /// <summary>
        /// Actualizar las globales del modulo
        /// </summary>
        public bool updGlobales(string cbp, string ccp, string cci, string ccint, string ccc, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".MIV_GLOBALES ");
                sentencia.Append("SET CB_PAGOS = '");
                sentencia.Append(cbp);
                sentencia.Append("' , CC_PAGOS = '");
                sentencia.Append(ccp);
                sentencia.Append("', CC_INVERSION = '");
                sentencia.Append(cci);
                sentencia.Append("', CC_INV_INTERES = '");
                sentencia.Append(ccint);
                sentencia.Append("', CC_INV_COMISION = '");
                sentencia.Append(ccc);                             
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[updGlobales]: Se presentaron problemas actualizando las globales. ");
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updGlobales]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }


        /// <summary>
        /// Actualizar las globales del modulo
        /// </summary>
        public bool updGlobalesConsecFolders(string folder, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".MIV_GLOBALES ");
                sentencia.Append("SET FOLDERS = '");
                sentencia.Append(folder);
                sentencia.Append("' ");

                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[updGlobalesConsecFolders]: Se presentaron problemas actualizando las globales. ");
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updGlobalesConsecFolders]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }

        /// <summary>
        /// Metodo que obtiene el siguiente valor de la cadena.
        /// </summary>
        public string sgtValor(string valor)
        {
            string nValor = string.Empty;
            string txt = string.Empty;
            Int64 sgt = 0;

            //entera
            Regex entera = new Regex(@"\d+");
            Match mEntera = entera.Match(valor);
            //text
            Regex texto = new Regex(@"([A-Za-z\-]+)");
            Match mTexto = texto.Match(valor);

            txt = mTexto.Value;
            sgt = Int64.Parse(mEntera.Value) + 1;
            nValor = txt + sgt.ToString().PadLeft(mEntera.Length, '0');

            return (nValor);
        }

        /// <summary>
        /// Obtener el listado      
        /// </summary>
        public DataTable getProvinciasCB()
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select DIVISION_GEOGRAFICA1, NOMBRE from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".DIVISION_GEOGRAFICA1 ");
            sentencia.Append("order by 2 asc ");
            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }


        /// <summary>
        /// Obtener el listado de las instituciones      
        /// </summary>
        public DataTable getCantonesCB(string provincia)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select DIVISION_GEOGRAFICA2, NOMBRE from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".DIVISION_GEOGRAFICA2 ");
            sentencia.Append("where DIVISION_GEOGRAFICA1 = '");
            sentencia.Append(provincia);
            sentencia.Append("' ");
            sentencia.Append("order by 2 asc ");
            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }


        /// <summary>
        /// Obtener el listado de las instituciones      
        /// </summary>
        public DataTable getDistritosCB(string provincia, string canton)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select DIVISION_GEOGRAFICA3, NOMBRE from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".DIVISION_GEOGRAFICA3 ");
            sentencia.Append("where DIVISION_GEOGRAFICA1 = '");
            sentencia.Append(provincia);
            sentencia.Append("' and DIVISION_GEOGRAFICA2 = '");
            sentencia.Append(canton);
            sentencia.Append("' order by 2 asc ");
            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }


        /// <summary>
        /// Obtener el listado de las instituciones      
        /// </summary>
        public DataTable getBarriosCB(string provincia, string canton, string distrito)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select DIVISION_GEOGRAFICA4, NOMBRE from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".DIVISION_GEOGRAFICA4 ");
            sentencia.Append("where DIVISION_GEOGRAFICA1 = '");
            sentencia.Append(provincia);
            sentencia.Append("' and DIVISION_GEOGRAFICA2 = '");
            sentencia.Append(canton);
            sentencia.Append("' and DIVISION_GEOGRAFICA3 = '");
            sentencia.Append(distrito);
            sentencia.Append("' order by 2 asc ");
            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }

        /// <summary>
        /// Metodo encargado de extraer el consecutivo de pedidos       
        /// </summary>
        public string getConsecutivo(SqlTransaction transac, string consecutivo)
        {
            string ultValor = string.Empty;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select VALOR_CONSECUTIVO from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CONSECUTIVO_FA ");
            sentencia.Append("WHERE CODIGO_CONSECUTIVO = '");
            sentencia.Append(consecutivo);
            sentencia.Append("'");

            ultValor = sqlClass.EjecutarScalar(sentencia.ToString(), transac).ToString();

            return sgtValor(ultValor);
        }


        public string[] getConfigValues()
        {
            string[] values = new string[]
            {
                 ConfigurationManager.AppSettings["CONSEC_CLIENTE"].ToString() //0 
                ,ConfigurationManager.AppSettings["CLIENTE_CONTADO"].ToString() //1
                ,ConfigurationManager.AppSettings["CATEGORIA_CLIENTE"].ToString() //2
                ,ConfigurationManager.AppSettings["MONEDA_CLIENTE"].ToString() //3
                ,ConfigurationManager.AppSettings["TIPO_IMPUESTO"].ToString() //4
                ,ConfigurationManager.AppSettings["TIPO_TARIFA"].ToString() //5
                ,ConfigurationManager.AppSettings["PORC_TARIFA"].ToString() //6
                ,ConfigurationManager.AppSettings["TIPIFICACION_CLIENTE"].ToString() //7
                ,ConfigurationManager.AppSettings["AFECTACION_IVA"].ToString() //8
                ,ConfigurationManager.AppSettings["CONDICION_PAGO_CONTADO"].ToString() //9
                ,ConfigurationManager.AppSettings["BODEGA"].ToString() //10
                ,ConfigurationManager.AppSettings["NIVEL_PRECIO"].ToString() //11
                ,ConfigurationManager.AppSettings["MONEDA"].ToString() //12
                ,ConfigurationManager.AppSettings["PAIS"].ToString() //13
                ,ConfigurationManager.AppSettings["CODIGO_IMPUESTO"].ToString() //14                
                ,ConfigurationManager.AppSettings["ACTIVIDAD_COMERCIAL"].ToString() //15
            };
            return (values);
        }
    }
}
