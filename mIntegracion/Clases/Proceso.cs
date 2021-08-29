using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Windows.Forms;

namespace mIntegracion.Clases
{
    class Proceso
    {
        private SQL sqlClass;

        public Proceso(SQL _sqlClass)
        {
            this.sqlClass = _sqlClass;
        }


        /// <summary>
        /// Metodo encargado de extraer cias      
        /// </summary>
        public DataTable getCias()
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CONJUNTO, NOMBRE from ");
            sentencia.Append("ERPADMIN");
            sentencia.Append(".CONJUNTO ");
            sentencia.Append("WHERE CONJUNTO <> 'ERPADMIN' order by 1 asc ");
            
            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }


        /// <summary>
        /// Metodo encargado de extraer bodegas    
        /// </summary>
        public DataTable getBodega(string cia)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select BODEGA , NOMBRE from ");
            sentencia.Append(cia);
            sentencia.Append(".BODEGA ");
            sentencia.Append("WHERE TIPO = 'V' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }

        /// <summary>
        /// Obtener el número máximo de documento   
        /// </summary>
        public string getSgtOC()
        {
            Globales glb = new Globales(sqlClass);
            string q = string.Empty;

            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ISNULL(MAX(ORDEN_COMPRA),'OC00000000000000') SGT from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CCO_ORDEN_COMPRA ");

            q = sqlClass.EjecutarScalar(sentencia.ToString()).ToString();

            return (glb.sgtValor(q));
        }

        public string OCEstadoTxt(string estado)
        {
            switch (estado)
            {
                case "Creada":
                    return ("C");
                case "Generada":
                    return ("G");
                case "Cerrada":
                    return ("E");
                default:
                    return ("C");
            }
        }

        public string OCEstadoId(string estado)
        {
            switch (estado)
            {
                case "C":
                    return ("Creada");
                case "G":
                    return ("Generada");
                case "E":
                    return ("Cerrada");
                default:
                    return ("Creada");
            }
        }

        /// <summary>
        /// Obtener listado       
        /// </summary>
        public DataTable getOCList(string ocInicio, string ocFin, string estado, DateTime fcInicio, DateTime fcFin)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ORDEN_COMPRA Orden_Compra ");
            sentencia.Append(", FECHA_CREACION Fecha ");
            sentencia.Append(", case ESTADO when 'C' then 'Creada' ");
            sentencia.Append("			when 'G' then 'Generada' ");
            sentencia.Append("			when 'E' then 'Cerrada' end Estado ");
            sentencia.Append(", convert(varchar, cast(TOTAL as money), 1) Total  ");           
            sentencia.Append("from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CCO_ORDEN_COMPRA ");
            sentencia.Append("where ORDEN_COMPRA is not null");

            if (ocInicio.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and ORDEN_COMPRA >= '");
                sentencia.Append(ocInicio);
                sentencia.Append("' ");
            }

            if (ocFin.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and ORDEN_COMPRA <= '");
                sentencia.Append(ocFin);
                sentencia.Append("' ");
            }

            if (estado.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and ESTADO in ");
                sentencia.Append(estado);
                sentencia.Append(" ");
            }

            if ((fcInicio.ToString("yyyy").CompareTo("0001") != 0) && (fcFin.ToString("yyyy").CompareTo("0001") != 0))
            {
                sentencia.Append(" and FECHA_CREACION between '");
                sentencia.Append(fcInicio.ToString("yyyy-MM-dd"));
                sentencia.Append("' and '");
                sentencia.Append(fcFin.ToString("yyyy-MM-dd"));
                sentencia.Append("' ");
            }

            sentencia.Append(" order by FECHA_CREACION desc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }

        /// <summary>
        /// Actualizar documento
        /// </summary>
        public bool updOCEstado(string oc, DateTime fecha, string usuario, string estado, string campo_fecha, string campo_usuario, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA ");
                sentencia.Append("SET " + campo_usuario + " = '");
                sentencia.Append(usuario);
                sentencia.Append("', " + campo_fecha + " = '");
                sentencia.Append(fecha.ToString("yyyy-MM-dd"));
                sentencia.Append("', ESTADO = '");
                sentencia.Append(estado);
                sentencia.Append("' WHERE ORDEN_COMPRA = '");
                sentencia.Append(oc);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[updOCEstado]: Se presentaron problemas actualizando: ");
                    errores.Append(oc);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updOCEstado]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }


        /// <summary>
        /// Actualizar documento
        /// </summary>
        public bool updOCEstado(SqlTransaction transac, string oc, DateTime fecha, string usuario, string estado, string campo_fecha, string campo_usuario, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA ");
                sentencia.Append("SET " + campo_usuario + " = '");
                sentencia.Append(usuario);
                sentencia.Append("', " + campo_fecha + " = '");
                sentencia.Append(fecha.ToString("yyyy-MM-dd"));
                sentencia.Append("', ESTADO = '");
                sentencia.Append(estado);
                sentencia.Append("' WHERE ORDEN_COMPRA = '");
                sentencia.Append(oc);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[updOCEstado]: Se presentaron problemas actualizando: ");
                    errores.Append(oc);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updOCEstado]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }

        /// <summary>
        /// Eliminar orden de compra
        /// </summary>
        public bool delOC(string oc, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("DELETE FROM ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA ");
                sentencia.Append(" WHERE ORDEN_COMPRA = '");
                sentencia.Append(oc);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[delOC]: Se presentaron problemas eliminando: ");
                    errores.Append(oc);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[delOC]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }



        ///// <summary>
        ///// Obtener el detalle de la sugerencia      
        ///// </summary>
        //public DataTable getSugerencia(SqlTransaction transac, string cia, string bodega, string artIni, string artFin, string prov
        //    , string cla1, string cla2, string cla3, string cla4, string cla5, string cla6)
        //{
        //    DataTable dt = null;
        //    StringBuilder sentencia = new StringBuilder();

        //    sentencia.Append("select a.ARTICULO");
        //    sentencia.Append(", a.DESCRIPCION ARTICULO_DESC");
        //    sentencia.Append(", a.COSTO_PROM_LOC");
        //    sentencia.Append(", a.COSTO_PROM_DOL");
        //    sentencia.Append(", a.CLASIFICACION_1");
        //    sentencia.Append(", a.CLASIFICACION_2");
        //    sentencia.Append(", a.CLASIFICACION_3");
        //    sentencia.Append(", a.CLASIFICACION_4");
        //    sentencia.Append(", a.CLASIFICACION_5");
        //    sentencia.Append(", a.CLASIFICACION_6");
        //    sentencia.Append(", a.IMPUESTO");
        //    sentencia.Append(", i.IMPUESTO1");
        //    sentencia.Append(", i.IMPUESTO2");
        //    sentencia.Append(", eb.EXISTENCIA_MINIMA");
        //    sentencia.Append(", eb.PUNTO_DE_REORDEN");
        //    sentencia.Append(", eb.EXISTENCIA_MAXIMA");
        //    sentencia.Append(", eb.CANT_DISPONIBLE");
        //    sentencia.Append(", eb.CANT_TRANSITO");

        //    sentencia.Append(",ISNULL((select top 1 el.PRECIO_UNITARIO from " + cia + ".EMBARQUE e inner join " + cia + ".EMBARQUE_LINEA el on e.EMBARQUE = el.EMBARQUE and a.ARTICULO = el.ARTICULO order by e.FECHA_EMBARQUE desc),a.COSTO_PROM_LOC) PRECIO_UNITARIO");
        //    sentencia.Append(",ISNULL((select top 1 el.MONTO_DESC_UNITARIO from " + cia + ".EMBARQUE e inner join " + cia + ".EMBARQUE_LINEA el on e.EMBARQUE = el.EMBARQUE and a.ARTICULO = el.ARTICULO order by e.FECHA_EMBARQUE desc),0) MONTO_DESC_UNITARIO");
        //    sentencia.Append(",ISNULL((select top 1 el.PROVEEDOR from " + cia + ".EMBARQUE e inner join " + cia + ".EMBARQUE_LINEA el on e.EMBARQUE = el.EMBARQUE and a.ARTICULO = el.ARTICULO order by e.FECHA_EMBARQUE desc), (select top 1 ap.PROVEEDOR from " + cia + ".ARTICULO_PROVEEDOR ap where ap.ARTICULO = a.ARTICULO order by CreateDate desc) ) PROVEEDOR");            
        //    sentencia.Append(",ISNULL((select top 1 el.MONEDA_OC from " + cia + ".EMBARQUE e inner join " + cia + ".EMBARQUE_LINEA el on e.EMBARQUE = el.EMBARQUE and a.ARTICULO = el.ARTICULO order by e.FECHA_EMBARQUE desc),'CRC') MONEDA_OC");
        //    sentencia.Append(",ISNULL((select top 1 el.TC_PRECIO_DOC_DOLAR from " + cia + ".EMBARQUE e inner join " + cia + ".EMBARQUE_LINEA el on e.EMBARQUE = el.EMBARQUE and a.ARTICULO = el.ARTICULO order by e.FECHA_EMBARQUE desc),0) TC_PRECIO_DOC_DOLAR");
        //    sentencia.Append(",ISNULL((select top 1 el.TC_PRECIO_DOC_LOCAL from " + cia + ".EMBARQUE e inner join " + cia + ".EMBARQUE_LINEA el on e.EMBARQUE = el.EMBARQUE and a.ARTICULO = el.ARTICULO order by e.FECHA_EMBARQUE desc),0) TC_PRECIO_DOC_LOCAL");
                        
        //    sentencia.Append(" from ");
        //    sentencia.Append(cia);
        //    sentencia.Append(".ARTICULO a inner join  ");
        //    sentencia.Append(cia);
        //    sentencia.Append(".IMPUESTO i on a.IMPUESTO = i.IMPUESTO inner join ");
        //    sentencia.Append(cia);
        //    sentencia.Append(".EXISTENCIA_BODEGA eb on a.ARTICULO = eb.ARTICULO ");            
        //    sentencia.Append("where eb.BODEGA = '" + bodega + "'");

        //    if (artIni.CompareTo(string.Empty) != 0)
        //    {
        //        sentencia.Append(" and a.ARTICULO >= '");
        //        sentencia.Append(artIni);
        //        sentencia.Append("' ");
        //    }
        //    if (artFin.CompareTo(string.Empty) != 0)
        //    {
        //        sentencia.Append(" and a.ARTICULO <= '");
        //        sentencia.Append(artFin);
        //        sentencia.Append("' ");
        //    }
        //    //if (prov.CompareTo(string.Empty) != 0)
        //    //{
        //    //    sentencia.Append(" and ap.PROVEEDOR = '");
        //    //    sentencia.Append(prov);
        //    //    sentencia.Append("' ");
        //    //}
        //    if (cla1.CompareTo(string.Empty) != 0)
        //    {
        //        sentencia.Append(" and a.CLASIFICACION_1 = '");
        //        sentencia.Append(cla1);
        //        sentencia.Append("' ");
        //    }
        //    if (cla2.CompareTo(string.Empty) != 0)
        //    {
        //        sentencia.Append(" and a.CLASIFICACION_2 = '");
        //        sentencia.Append(cla2);
        //        sentencia.Append("' ");
        //    }
        //    if (cla3.CompareTo(string.Empty) != 0)
        //    {
        //        sentencia.Append(" and a.CLASIFICACION_3 = '");
        //        sentencia.Append(cla3);
        //        sentencia.Append("' ");
        //    }
        //    if (cla4.CompareTo(string.Empty) != 0)
        //    {
        //        sentencia.Append(" and a.CLASIFICACION_4 = '");
        //        sentencia.Append(cla4);
        //        sentencia.Append("' ");
        //    }
        //    if (cla5.CompareTo(string.Empty) != 0)
        //    {
        //        sentencia.Append(" and a.CLASIFICACION_5 = '");
        //        sentencia.Append(cla5);
        //        sentencia.Append("' ");
        //    }
        //    if (cla6.CompareTo(string.Empty) != 0)
        //    {
        //        sentencia.Append(" and a.CLASIFICACION_6 = '");
        //        sentencia.Append(cla6);
        //        sentencia.Append("' ");
        //    }          

        //    sentencia.Append(" order by a.ARTICULO asc ");

        //    dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

        //    if (prov.CompareTo(string.Empty) != 0)
        //    {
        //        string filter = "PROVEEDOR = '" + prov + "'";
        //        DataTable dtP = dt.Select(filter).CopyToDataTable();
        //        dt = dtP;
        //    }


        //    return dt;
        //}


        /// <summary>
        /// Obtener el detalle de la sugerencia      
        /// </summary>
        public DataTable getSugerencia(SqlTransaction transac, string cia, string bodega, string artIni, string artFin, string prov
            , string cla1, string cla2, string cla3, string cla4, string cla5, string cla6)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select s.ARTICULO     ");
            sentencia.Append(",s.ARTICULO_DESC      ");
            sentencia.Append(",s.COSTO_PROM_LOC     ");
            sentencia.Append(",s.COSTO_PROM_DOL     ");
            sentencia.Append(",s.CLASIFICACION_1    ");
            sentencia.Append(",s.CLASIFICACION_2    ");
            sentencia.Append(",s.CLASIFICACION_3    ");
            sentencia.Append(",s.CLASIFICACION_4    ");
            sentencia.Append(",s.CLASIFICACION_5    ");
            sentencia.Append(",s.CLASIFICACION_6    ");
            sentencia.Append(",s.IMPUESTO           ");
            sentencia.Append(",s.IMPUESTO1          ");
            sentencia.Append(",s.IMPUESTO2          ");
            sentencia.Append(",s.BODEGA             ");
            sentencia.Append(",s.EXISTENCIA_MINIMA  ");
            sentencia.Append(",s.PUNTO_DE_REORDEN   ");
            sentencia.Append(",s.EXISTENCIA_MAXIMA  ");
            sentencia.Append(",s.CANT_DISPONIBLE    ");
            sentencia.Append(",s.CANT_TRANSITO      ");
            sentencia.Append(",s.PRECIO_UNITARIO    ");            
            sentencia.Append(",s.PROVEEDOR          ");
            sentencia.Append(",s.MONEDA_OC          ");            
            sentencia.Append(",s.TC_PRECIO_DOC_LOCAL");
            sentencia.Append(",p.NOMBRE PROVEEDOR_DESC");
            sentencia.Append(" from ");
            sentencia.Append(cia);
            sentencia.Append(".CCO_OC_SUGERENCIA s left join ");
            sentencia.Append(cia);
            sentencia.Append(".PROVEEDOR p on s.PROVEEDOR = p.PROVEEDOR ");

            sentencia.Append("where BODEGA = '" + bodega + "'");

            if (artIni.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and ARTICULO >= '");
                sentencia.Append(artIni);
                sentencia.Append("' ");
            }
            if (artFin.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and ARTICULO <= '");
                sentencia.Append(artFin);
                sentencia.Append("' ");
            }
            if (prov.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and s.PROVEEDOR = '");
                sentencia.Append(prov);
                sentencia.Append("' ");
            }
            if (cla1.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and CLASIFICACION_1 = '");
                sentencia.Append(cla1);
                sentencia.Append("' ");
            }
            if (cla2.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and CLASIFICACION_2 = '");
                sentencia.Append(cla2);
                sentencia.Append("' ");
            }
            if (cla3.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and CLASIFICACION_3 = '");
                sentencia.Append(cla3);
                sentencia.Append("' ");
            }
            if (cla4.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and CLASIFICACION_4 = '");
                sentencia.Append(cla4);
                sentencia.Append("' ");
            }
            if (cla5.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and CLASIFICACION_5 = '");
                sentencia.Append(cla5);
                sentencia.Append("' ");
            }
            if (cla6.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and CLASIFICACION_6 = '");
                sentencia.Append(cla6);
                sentencia.Append("' ");
            }

            sentencia.Append(" order by ARTICULO asc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            //if (prov.CompareTo(string.Empty) != 0)
            //{
            //    string filter = "PROVEEDOR = '" + prov + "'";
            //    DataTable dtP = dt.Select(filter).CopyToDataTable();
            //    dt = dtP;
            //}

            return dt;
        }

        /// <summary>
        /// Metodo encargado del extraer el detalle de cia | bod  
        /// </summary>
        public DataTable getCiasBodegas(string oc)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CONJUNTO, BODEGA from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CCO_CIAS_DET ");
            sentencia.Append("WHERE ORDEN_COMPRA = '");
            sentencia.Append(oc);
            sentencia.Append("' ORDER BY CONJUNTO ASC");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }

        /// <summary>
        /// Metodo encargado del extraer el detalle de oc a generar  
        /// </summary>
        public DataTable getOCGenerar(SqlTransaction transac, string oc)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ORDEN_COMPRA, CONJUNTO, BODEGA, PROVEEDOR, SUM(IMPUESTO) IMPUESTO, SUM(SUBTOTAL) SUBTOTAL, SUM(TOTAL) TOTAL from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CCO_ORDEN_COMPRA_DET ");
            sentencia.Append("WHERE ORDEN_COMPRA = '");
            sentencia.Append(oc);
            sentencia.Append("' and CANT_SUGERIDA > 0 and PROVEEDOR <> '' ");
            sentencia.Append(" group by ORDEN_COMPRA, CONJUNTO, BODEGA, PROVEEDOR ORDER BY ORDEN_COMPRA, CONJUNTO, BODEGA, PROVEEDOR asc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }


        /// <summary>
        /// Metodo encargado de insertar cia | bod
        /// </summary>
        public bool insOCCiasBods(SqlTransaction transac, string oc, string cia, string bod, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_CIAS_DET ");
                sentencia.Append("(CONJUNTO,BODEGA,ORDEN_COMPRA");
                sentencia.Append(") VALUES (");
                sentencia.Append("@CONJUNTO,@BODEGA,@ORDEN_COMPRA)");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros                
                cmd.Parameters.Add("@CONJUNTO    ", SqlDbType.VarChar, 10).Value = cia;
                cmd.Parameters.Add("@BODEGA      ", SqlDbType.VarChar, 4).Value = bod;
                cmd.Parameters.Add("@ORDEN_COMPRA", SqlDbType.VarChar, 50).Value = oc;

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insOCCiasBods]: Se presentaron problemas insertando OC: ");
                    errores.AppendLine(oc);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insOCCiasBods]: Detalle Técnico: " + e.Message);
                lbOK = false;
            }
            finally
            {
                cmd.Dispose();
                cmd = null;
            }
            return (lbOK);
        }


        /// <summary>
        /// Metodo encargado de insertar cia | bod
        /// </summary>
        public bool insOC(SqlTransaction transac, string oc, string estado, string moneda, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA ");
                sentencia.Append("(ORDEN_COMPRA,ESTADO,FECHA_CREACION,MONEDA,TOTAL");
                sentencia.Append(") VALUES (");
                sentencia.Append("@ORDEN_COMPRA,@ESTADO,@FECHA_CREACION,@MONEDA,@TOTAL)");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros                
                cmd.Parameters.Add("@ORDEN_COMPRA  ", SqlDbType.VarChar, 50).Value = oc;
                cmd.Parameters.Add("@ESTADO        ", SqlDbType.VarChar, 1).Value = estado;
                cmd.Parameters.Add("@FECHA_CREACION", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); ;
                cmd.Parameters.Add("@MONEDA        ", SqlDbType.VarChar, 1).Value = moneda;
                cmd.Parameters.Add("@TOTAL", SqlDbType.Decimal).Value = 0;
                

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insOC]: Se presentaron problemas insertando OC: ");
                    errores.AppendLine(oc);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insOC]: Detalle Técnico: " + e.Message);
                lbOK = false;
            }
            finally
            {
                cmd.Dispose();
                cmd = null;
            }
            return (lbOK);
        }



        /// <summary>
        /// Metodo encargado de insertar cia | bod
        /// </summary>
        public bool insOCDet(SqlTransaction transac, string oc, string prov, string art, string cia, string bod, 
            decimal min, decimal reorden, decimal max, decimal disp, decimal tran, decimal sug, decimal precio, decimal imp, decimal subtotal, decimal total, decimal impPorc,
            string artDesc, string provDesc,
            ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA_DET ");
                sentencia.Append("(ORDEN_COMPRA,PROVEEDOR,ARTICULO,CONJUNTO,BODEGA,EXISTENCIA_MINIMA,PUNTO_REORDEN,EXISTENCIA_MAXIMA,EXISTENCIA_ACTUAL,TRANSITO,CANT_SUGERIDA,PRECIO,IMPUESTO,SUBTOTAL,TOTAL,IMPUESTO_PORC,PROVEEDOR_DESC,ARTICULO_DESC");
                sentencia.Append(") VALUES (");
                sentencia.Append("@ORDEN_COMPRA,@PROVEEDOR,@ARTICULO,@CONJUNTO,@BODEGA,@EXISTENCIA_MINIMA,@PUNTO_REORDEN,@EXISTENCIA_MAXIMA,@EXISTENCIA_ACTUAL,@TRANSITO,@CANT_SUGERIDA,@PRECIO,@IMPUESTO,@SUBTOTAL,@TOTAL,@IMPUESTO_PORC,@PROVEEDOR_DESC,@ARTICULO_DESC)");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros  
                cmd.Parameters.Add("@ORDEN_COMPRA", SqlDbType.VarChar, 50).Value = oc;
                cmd.Parameters.Add("@PROVEEDOR   ", SqlDbType.VarChar, 20).Value = prov;
                cmd.Parameters.Add("@ARTICULO    ", SqlDbType.VarChar, 20).Value = art;
                cmd.Parameters.Add("@CONJUNTO    ", SqlDbType.VarChar, 10).Value = cia;
                cmd.Parameters.Add("@BODEGA      ", SqlDbType.VarChar, 4).Value = bod;
                cmd.Parameters.Add("@EXISTENCIA_MINIMA", SqlDbType.Decimal).Value = min;
                cmd.Parameters.Add("@PUNTO_REORDEN    ", SqlDbType.Decimal).Value = reorden;
                cmd.Parameters.Add("@EXISTENCIA_MAXIMA", SqlDbType.Decimal).Value = max;
                cmd.Parameters.Add("@EXISTENCIA_ACTUAL", SqlDbType.Decimal).Value = disp;
                cmd.Parameters.Add("@TRANSITO         ", SqlDbType.Decimal).Value = tran;
                cmd.Parameters.Add("@CANT_SUGERIDA    ", SqlDbType.Decimal).Value = sug;
                cmd.Parameters.Add("@PRECIO           ", SqlDbType.Decimal).Value = precio;
                cmd.Parameters.Add("@IMPUESTO         ", SqlDbType.Decimal).Value = imp;
                cmd.Parameters.Add("@SUBTOTAL         ", SqlDbType.Decimal).Value = subtotal;
                cmd.Parameters.Add("@TOTAL            ", SqlDbType.Decimal).Value = total;
                cmd.Parameters.Add("@IMPUESTO_PORC    ", SqlDbType.Decimal).Value = impPorc;
                cmd.Parameters.Add("@PROVEEDOR_DESC   ", SqlDbType.VarChar, 150).Value = provDesc;
                cmd.Parameters.Add("@ARTICULO_DESC   ", SqlDbType.VarChar, 254).Value = artDesc;
                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insOCDet]: Se presentaron problemas insertando OC: ");
                    errores.AppendLine(oc);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insOCDet]: Detalle Técnico: " + e.Message);
                lbOK = false;
            }
            finally
            {
                cmd.Dispose();
                cmd = null;
            }
            return (lbOK);
        }

        /// <summary>
        /// Actualizar documento totales
        /// </summary>
        public bool updOCTotales(SqlTransaction transac, string oc, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA  ");
                sentencia.Append("SET  ");               
                sentencia.Append("TOTAL = d.TOTAL ");           
                sentencia.Append("FROM  ");
                sentencia.Append("    (select MAX(ORDEN_COMPRA) ORDEN_COMPRA, SUM(TOTAL) TOTAL ");
                sentencia.Append("	from " + sqlClass.Compannia + ".CCO_ORDEN_COMPRA_DET where ORDEN_COMPRA = '" + oc + "') AS d  ");
                sentencia.Append("WHERE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA.ORDEN_COMPRA = d.ORDEN_COMPRA AND ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA.ORDEN_COMPRA = '");
                sentencia.Append(oc);
                sentencia.Append("' ");

                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[updOCTotales]: Se presentaron problemas actualizando el calculo: ");
                    errores.Append(oc);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updOCTotales]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }

        /// <summary>
        /// Obtener oc       
        /// </summary>
        public DataTable getOC(string oc, string cia, string bod, string prov)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select oc.ORDEN_COMPRA ");
            sentencia.Append(",oc.ESTADO ");
            sentencia.Append(",case oc.ESTADO when 'C' then 'Creada'  ");
            sentencia.Append("		when 'G' then 'Generada'  ");
            sentencia.Append("		when 'E' then 'Cerrada' end ESTADO_DESC  ");
            sentencia.Append(",oc.FECHA_CREACION ");
            sentencia.Append(",oc.FECHA_GENERACION ");
            sentencia.Append(",oc.FECHA_CIERRE ");
            sentencia.Append(",oc.MONEDA ");
            sentencia.Append(",oc.TOTAL ");
            sentencia.Append(",oc.USR_GENERACION ");
            sentencia.Append(",oc.USR_CIERRE ");
           
            sentencia.Append(",ocd.PROVEEDOR ");
            sentencia.Append(",ocd.ARTICULO ");
            sentencia.Append(",ocd.CONJUNTO ");
            sentencia.Append(",ocd.BODEGA ");
            sentencia.Append(",ocd.EXISTENCIA_MINIMA ");
            sentencia.Append(",ocd.PUNTO_REORDEN ");
            sentencia.Append(",ocd.EXISTENCIA_MAXIMA ");
            sentencia.Append(",ocd.EXISTENCIA_ACTUAL ");
            sentencia.Append(",ocd.TRANSITO ");
            sentencia.Append(",ocd.CANT_SUGERIDA ");
            sentencia.Append(",ocd.PRECIO ");
            sentencia.Append(",ocd.IMPUESTO ");
            sentencia.Append(",ocd.SUBTOTAL ");
            sentencia.Append(",ocd.TOTAL TOTAL_LINEA ");
            sentencia.Append(",ocd.IMPUESTO_PORC ");
            sentencia.Append(",ocd.PROVEEDOR_DESC ");
            sentencia.Append(",ocd.ARTICULO_DESC ");
            sentencia.Append("from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CCO_ORDEN_COMPRA oc inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CCO_ORDEN_COMPRA_DET ocd on oc.ORDEN_COMPRA = ocd.ORDEN_COMPRA ");
            sentencia.Append(" where oc.ORDEN_COMPRA = '");
            sentencia.Append(oc);
            sentencia.Append("' ");

            if (cia.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and ocd.CONJUNTO = '");
                sentencia.Append(cia);
                sentencia.Append("' ");
            }
            if (bod.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and ocd.BODEGA = '");
                sentencia.Append(bod);
                sentencia.Append("' ");
            }
            if (prov.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and ocd.PROVEEDOR = '");
                sentencia.Append(prov);
                sentencia.Append("' ");
            }

            sentencia.Append(" order by ocd.ARTICULO,ocd.CONJUNTO,ocd.BODEGA asc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }

        /// <summary>
        /// Obtener oc       
        /// </summary>
        public DataTable getOCLineas(SqlTransaction transac, string cia, string oc, string bod, string prov)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select oc.ORDEN_COMPRA ");
            sentencia.Append(",oc.ESTADO ");
            sentencia.Append(",case oc.ESTADO when 'C' then 'Creada'  ");
            sentencia.Append("		when 'G' then 'Generada'  ");
            sentencia.Append("		when 'E' then 'Cerrada' end ESTADO_DESC  ");
            sentencia.Append(",oc.FECHA_CREACION ");
            sentencia.Append(",oc.FECHA_GENERACION ");
            sentencia.Append(",oc.FECHA_CIERRE ");
            sentencia.Append(",oc.MONEDA ");
            sentencia.Append(",oc.TOTAL ");
            sentencia.Append(",oc.USR_GENERACION ");
            sentencia.Append(",oc.USR_CIERRE ");

            sentencia.Append(",ocd.PROVEEDOR ");
            sentencia.Append(",ocd.ARTICULO ");
            sentencia.Append(",ocd.CONJUNTO ");
            sentencia.Append(",ocd.BODEGA ");
            sentencia.Append(",ocd.EXISTENCIA_MINIMA ");
            sentencia.Append(",ocd.PUNTO_REORDEN ");
            sentencia.Append(",ocd.EXISTENCIA_MAXIMA ");
            sentencia.Append(",ocd.EXISTENCIA_ACTUAL ");
            sentencia.Append(",ocd.TRANSITO ");
            sentencia.Append(",ocd.CANT_SUGERIDA ");
            sentencia.Append(",ocd.PRECIO ");
            sentencia.Append(",ocd.IMPUESTO ");
            sentencia.Append(",ocd.SUBTOTAL ");
            sentencia.Append(",ocd.TOTAL TOTAL_LINEA ");
            sentencia.Append(",ocd.IMPUESTO_PORC ");
            sentencia.Append(",ocd.PROVEEDOR_DESC ");
            sentencia.Append(",ocd.ARTICULO_DESC ");
            sentencia.Append("from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CCO_ORDEN_COMPRA oc inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CCO_ORDEN_COMPRA_DET ocd on oc.ORDEN_COMPRA = ocd.ORDEN_COMPRA ");
            sentencia.Append(" where oc.ORDEN_COMPRA = '");
            sentencia.Append(oc);
            sentencia.Append("' and ocd.CANT_SUGERIDA > 0 and PROVEEDOR <> '' ");
            
            sentencia.Append(" and ocd.CONJUNTO = '");
            sentencia.Append(cia);
            sentencia.Append("' ");
            
            sentencia.Append(" and ocd.BODEGA = '");
            sentencia.Append(bod);
            sentencia.Append("' ");
           
            sentencia.Append(" and ocd.PROVEEDOR = '");
            sentencia.Append(prov);
            sentencia.Append("' ");            

            sentencia.Append(" order by ocd.ARTICULO,ocd.CONJUNTO,ocd.BODEGA asc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }

        /// <summary>
        /// Metodo valida si existe     
        /// </summary>
        public int existOC(SqlTransaction transac, string oc)
        {
            int q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT COUNT(1) FROM ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CCO_ORDEN_COMPRA ");
            sentencia.Append("WHERE ORDEN_COMPRA = '");
            sentencia.Append(oc);
            sentencia.Append("'");

            q = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString(), transac).ToString());
            return (q);
        }

        /// <summary>
        /// Eliminar fiadores
        /// </summary>
        public bool delOCDet(SqlTransaction transac, string oc, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("DELETE FROM ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA_DET ");
                sentencia.Append("WHERE ORDEN_COMPRA = '");
                sentencia.Append(oc);
                sentencia.Append("' ");

                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[delOCDet]: Se presentaron problemas eliminando el detalle: ");
                    errores.Append(oc);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[delOCDet]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }

        /// <summary>
        /// Metodo obtiene el nombre del proveedor 
        /// </summary>
        public string getProveedorNombre(SqlTransaction transac, string cia, string prov)
        {
            DataTable dt;
            string nombre = string.Empty;
            StringBuilder sentencia = new StringBuilder();

            if(prov.CompareTo(string.Empty)!=0)
            {
                sentencia.Append("SELECT NOMBRE FROM ");
                sentencia.Append(cia);
                sentencia.Append(".PROVEEDOR ");
                sentencia.Append("WHERE PROVEEDOR = '");
                sentencia.Append(prov);
                sentencia.Append("'");

                dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

                if (dt != null && dt.Rows.Count > 0)
                {
                    nombre = dt.Rows[0]["NOMBRE"].ToString();
                }
            }

            return (nombre);
        }

        /// <summary>
        /// Metodo encargado de insertar trazabilidad entre oc -> oc ERP
        /// </summary>
        public bool insOCTraz(SqlTransaction transac, string cia, string bodega, string oc, string ocGen, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CCO_ORDEN_COMPRA_TRAZ ");
                sentencia.Append("(OC_GENERADA,USUARIO,FECHA_HORA,CONJUNTO,BODEGA,ORDEN_COMPRA");
                sentencia.Append(") VALUES (");
                sentencia.Append("@OC_GENERADA,@USUARIO,@FECHA_HORA,@CONJUNTO,@BODEGA,@ORDEN_COMPRA)");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros
                cmd.Parameters.Add("@OC_GENERADA ", SqlDbType.VarChar, 10).Value = ocGen;
                cmd.Parameters.Add("@USUARIO     ", SqlDbType.VarChar, 10).Value = sqlClass.Usuario;
                cmd.Parameters.Add("@FECHA_HORA  ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); 
                cmd.Parameters.Add("@CONJUNTO    ", SqlDbType.VarChar, 10).Value = cia;
                cmd.Parameters.Add("@BODEGA      ", SqlDbType.VarChar, 4).Value = bodega;
                cmd.Parameters.Add("@ORDEN_COMPRA", SqlDbType.VarChar, 50).Value = oc;

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insOCTraz]: Se presentaron problemas insertando OC: ");
                    errores.AppendLine(oc);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insOCTraz]: Detalle Técnico: " + e.Message);
                lbOK = false;
            }
            finally
            {
                cmd.Dispose();
                cmd = null;
            }
            return (lbOK);
        }





















































































    }
}
