using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace mIntegracion.Clases
{
    class Compras
    {
        private SQL sqlClass;

        /*          
         Estados de las Ordenes de Compra

         OCPLANEADA = 'A'
         TEXTO_OCPLANEADA = 'Planeada'
         OCTRANSITO = 'E'
         TEXTO_OCTRANSITO = 'Tránsito'
         OCBACKORDER = 'I'
         TEXTO_OCBACKORDER = 'Backorder'
         OCCANCELADA = 'O'
         TEXTO_OCCANCELADA = 'Cancelada'
         OCCERRADA = 'U'
         TEXTO_OCCERRADA = 'Cerrada'
         OCRECIBIDA = 'R'
         TEXTO_OCRECIBIDA = 'Recibida'
         OCNOAPROBADA = 'N'
         TEXTO_OCNOAPROBADA = 'No Aprobar'
         
         */

        public Compras(SQL _sqlClass)
        {
            this.sqlClass = _sqlClass;
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
        /// Metodo encargado de extraer el consecutivo de ordenes de compra       
        /// </summary>
        public string getConsecutivoOC(SqlTransaction transac, string cia)
        {
            string ultValor = string.Empty;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ult_orden_compra from ");
            sentencia.Append(cia);
            sentencia.Append(".globales_co ");            

            ultValor = sqlClass.EjecutarScalar(sentencia.ToString(), transac).ToString();

            return sgtValor(ultValor);
        }

        /// <summary>
        /// Metodo encargado de extraer las globales CO 
        /// </summary>
        public DataTable getGlobalesOC(SqlTransaction transac, string cia)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ULT_ORDEN_COMPRA, DIRECCION_EMBARQUE, DIRECCION_COBRO from ");
            sentencia.Append(cia);
            sentencia.Append(".GLOBALES_CO ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }


        /// <summary>
        /// Actualizar las globales del modulo
        /// </summary>
        public bool updConsecOC(SqlTransaction transac, string consec, string cia, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(cia);
                sentencia.Append(".GLOBALES_CO ");
                sentencia.Append("SET ult_orden_compra = '");
                sentencia.Append(consec);
                sentencia.Append("' ");

                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[updConsecOC]: Se presentaron problemas actualizando las globales. ");
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updConsecOC]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }

        /// <summary>
        /// Metodo encargado de extraer ordenes de compra en     
        /// </summary>
        public DataTable getOCDet(string cia, string bodega, string articulo, string estado)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select oc.ORDEN_COMPRA ");
            sentencia.Append(", oc.PROVEEDOR ");
            sentencia.Append(", oc.FECHA_HORA ");
            sentencia.Append(", p.NOMBRE ");
            sentencia.Append(", ocl.ARTICULO ");
            sentencia.Append(", a.DESCRIPCION ");
            sentencia.Append(", ocl.BODEGA ");
            sentencia.Append(", ocl.CANTIDAD_ORDENADA ");
            sentencia.Append(" from ");
            sentencia.Append(cia);
            sentencia.Append(".ORDEN_COMPRA oc inner join ");
            sentencia.Append(cia);
            sentencia.Append(".ORDEN_COMPRA_LINEA ocl on oc.ORDEN_COMPRA = ocl.ORDEN_COMPRA inner join ");
            sentencia.Append(cia);
            sentencia.Append(".PROVEEDOR p on oc.PROVEEDOR = p.PROVEEDOR inner join ");
            sentencia.Append(cia);
            sentencia.Append(".ARTICULO a on ocl.ARTICULO = a.ARTICULO ");
            sentencia.Append("where oc.ESTADO = '" + estado + "'");
            sentencia.Append("and ocl.BODEGA  = '" + bodega + "'");
            sentencia.Append("and ocl.ARTICULO = '" + articulo + "'");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }


        /// <summary>
        /// Metodo encargado de extraer ordenes de compra en     
        /// </summary>
        public DataTable getArticulo(SqlTransaction transac, string cia, string articulo)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select a.ARTICULO,a.UNIDAD_ALMACEN,a.IMPUESTO,i.TIPO_IMPUESTO1,i.TIPO_TARIFA1,i.TIPO_IMPUESTO2,i.TIPO_TARIFA2  ");           
            sentencia.Append(" from ");
            sentencia.Append(cia);
            sentencia.Append(".ARTICULO a left join ");
            sentencia.Append(cia);
            sentencia.Append(".IMPUESTO i on a.IMPUESTO = i.IMPUESTO ");
            sentencia.Append("where a.ARTICULO = '" + articulo + "'");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }

        /// <summary>
        /// Metodo obtiene cond pago proveedor
        /// </summary>
        public string getCondPagoProveedor(SqlTransaction transac, string cia, string prov)
        {
            String q = string.Empty;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT CONDICION_PAGO FROM ");
            sentencia.Append(cia);
            sentencia.Append(".PROVEEDOR ");
            sentencia.Append("WHERE PROVEEDOR ='");
            sentencia.Append(prov);
            sentencia.Append("'");
            q = sqlClass.EjecutarScalar(sentencia.ToString(), transac).ToString();
            return (q);
        }

        /// <summary>
        /// Metodo valida si el articulo - proveedor
        /// </summary>
        public int existeArticuloProveedor(SqlTransaction transac, string cia, string articulo, string prov)
        {
            int q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT COUNT(1) FROM ");
            sentencia.Append(cia);
            sentencia.Append(".ARTICULO_PROVEEDOR ");
            sentencia.Append("WHERE ARTICULO = '");
            sentencia.Append(articulo);
            sentencia.Append("' AND PROVEEDOR ='");
            sentencia.Append(prov);
            sentencia.Append("'");
            q = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString(),transac).ToString());
            return (q);
        }


        /// <summary>
        /// Metodo encargado de insertar el articulo - proveedor
        /// </summary>
        public bool insArticuloProveedor(SqlTransaction transac, string cia, string articulo, string proveedor, string und, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(cia);
                sentencia.Append(".ARTICULO_PROVEEDOR ");
                sentencia.Append("(ARTICULO,PROVEEDOR,LOTE_MINIMO,LOTE_ESTANDAR,PESO_MINIMO_ORDEN,MULTIPLO_COMPRA,CANT_ECONOMICA_COM,UNIDAD_MEDIDA_COMP" +
                                ",FACTOR_CONVERSION,PLAZO_REABASTECIMI,PORC_AJUSTE_COSTO,PAIS,TIPO");
                sentencia.Append(") VALUES (");
                sentencia.Append("@ARTICULO,@PROVEEDOR,@LOTE_MINIMO,@LOTE_ESTANDAR,@PESO_MINIMO_ORDEN,@MULTIPLO_COMPRA,@CANT_ECONOMICA_COM,@UNIDAD_MEDIDA_COMP" +
                                ",@FACTOR_CONVERSION,@PLAZO_REABASTECIMI,@PORC_AJUSTE_COSTO,@PAIS,@TIPO)");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros  
                cmd.Parameters.Add("@ARTICULO          ", SqlDbType.VarChar,20).Value = articulo;
                cmd.Parameters.Add("@PROVEEDOR         ", SqlDbType.VarChar,20).Value = proveedor;
                cmd.Parameters.Add("@LOTE_MINIMO       ", SqlDbType.Decimal).Value = 1;
                cmd.Parameters.Add("@LOTE_ESTANDAR     ", SqlDbType.Decimal).Value = 1;
                cmd.Parameters.Add("@PESO_MINIMO_ORDEN ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@MULTIPLO_COMPRA   ", SqlDbType.Decimal).Value = 1;
                cmd.Parameters.Add("@CANT_ECONOMICA_COM", SqlDbType.Decimal).Value = 1;
                cmd.Parameters.Add("@UNIDAD_MEDIDA_COMP", SqlDbType.VarChar,10).Value = und;
                cmd.Parameters.Add("@FACTOR_CONVERSION ", SqlDbType.Decimal).Value = 1;
                cmd.Parameters.Add("@PLAZO_REABASTECIMI", SqlDbType.SmallInt).Value = 1;
                cmd.Parameters.Add("@PORC_AJUSTE_COSTO ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@PAIS              ", SqlDbType.VarChar,4).Value = "CRI";
                cmd.Parameters.Add("@TIPO              ", SqlDbType.VarChar,    1).Value = "P";

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insArticuloProveedor]: Se presentaron problemas insertando el articulo: ");
                    errores.AppendLine(articulo + " | " + proveedor);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insArticuloProveedor]: Detalle Técnico: " + e.Message);
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
        /// Metodo encargado de insertar orden_compra
        /// </summary>
        public bool insOC(SqlTransaction transac, string cia, string oc, string prov, string bodega, string condpago, string moneda, string pais, string dirEmb, string dirCobro
            ,decimal porDesc, decimal mtnDesc, decimal totalMerc, decimal imp1, decimal imp2, string estado, decimal baseImp1, decimal baseImp2
            ,ref StringBuilder error)
        {

            bool ok = true;
            StringBuilder sQuery = new StringBuilder();
            SqlCommand cmd = null;
            string lerror = string.Empty;

            try
            {
                sQuery.AppendLine("Insert into " + cia + ".ORDEN_COMPRA (");
                sQuery.AppendLine("ORDEN_COMPRA,PROVEEDOR,BODEGA,CONDICION_PAGO,MONEDA,PAIS,MODULO_ORIGEN,FECHA,FECHA_REQUERIDA,DIRECCION_EMBARQUE,DIRECCION_COBRO");
                sQuery.AppendLine(",TIPO_DESCUENTO,PORC_DESCUENTO,MONTO_DESCUENTO,TOTAL_MERCADERIA,TOTAL_IMPUESTO1,TOTAL_IMPUESTO2,MONTO_FLETE,MONTO_SEGURO,MONTO_DOCUMENTACIO");
                sQuery.AppendLine(",MONTO_ANTICIPO,TOTAL_A_COMPRAR,PRIORIDAD,ESTADO,IMPRESA,FECHA_HORA,OBSERVACIONES,REQUIERE_CONFIRMA,CONFIRMADA,ORDEN_PROGRAMADA,RECIBIDO_DE_MAS");
                sQuery.AppendLine(",TIPO_PRORRATEO_OC,BASE_IMPUESTO1,BASE_IMPUESTO2,TOT_IMP1_ASUM_DESC,TOT_IMP1_ASUM_NODESC,TOT_IMP1_RETE_DESC,TOT_IMP1_RETE_NODESC");
                sQuery.AppendLine(") VALUES (");
                sQuery.AppendLine("@ORDEN_COMPRA,@PROVEEDOR,@BODEGA,@CONDICION_PAGO,@MONEDA,@PAIS,@MODULO_ORIGEN,@FECHA,@FECHA_REQUERIDA,@DIRECCION_EMBARQUE,@DIRECCION_COBRO");
                sQuery.AppendLine(",@TIPO_DESCUENTO,@PORC_DESCUENTO,@MONTO_DESCUENTO,@TOTAL_MERCADERIA,@TOTAL_IMPUESTO1,@TOTAL_IMPUESTO2,@MONTO_FLETE,@MONTO_SEGURO,@MONTO_DOCUMENTACIO");
                sQuery.AppendLine(",@MONTO_ANTICIPO,@TOTAL_A_COMPRAR,@PRIORIDAD,@ESTADO,@IMPRESA,@FECHA_HORA,@OBSERVACIONES,@REQUIERE_CONFIRMA,@CONFIRMADA,@ORDEN_PROGRAMADA,@RECIBIDO_DE_MAS");
                sQuery.AppendLine(",@TIPO_PRORRATEO_OC,@BASE_IMPUESTO1,@BASE_IMPUESTO2,@TOT_IMP1_ASUM_DESC,@TOT_IMP1_ASUM_NODESC,@TOT_IMP1_RETE_DESC,@TOT_IMP1_RETE_NODESC");
                sQuery.AppendLine(")");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sQuery.ToString();

                cmd.Parameters.AddWithValue("@ORDEN_COMPRA        ", oc);
                cmd.Parameters.AddWithValue("@PROVEEDOR           ", prov);
                cmd.Parameters.AddWithValue("@BODEGA              ", bodega);
                cmd.Parameters.AddWithValue("@CONDICION_PAGO      ", condpago);
                cmd.Parameters.AddWithValue("@MONEDA              ", moneda);
                cmd.Parameters.AddWithValue("@PAIS                ", pais);
                cmd.Parameters.AddWithValue("@MODULO_ORIGEN       ", "CO");
                cmd.Parameters.AddWithValue("@FECHA               ", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@FECHA_REQUERIDA     ", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@DIRECCION_EMBARQUE  ", dirEmb);
                cmd.Parameters.AddWithValue("@DIRECCION_COBRO     ", dirCobro);
                cmd.Parameters.AddWithValue("@TIPO_DESCUENTO      ", string.Empty);
                cmd.Parameters.AddWithValue("@PORC_DESCUENTO      ", porDesc);
                cmd.Parameters.AddWithValue("@MONTO_DESCUENTO     ", mtnDesc);
                cmd.Parameters.AddWithValue("@TOTAL_MERCADERIA    ", totalMerc);
                cmd.Parameters.AddWithValue("@TOTAL_IMPUESTO1     ", imp1);
                cmd.Parameters.AddWithValue("@TOTAL_IMPUESTO2     ", imp2);
                cmd.Parameters.AddWithValue("@MONTO_FLETE         ", 0);
                cmd.Parameters.AddWithValue("@MONTO_SEGURO        ", 0);
                cmd.Parameters.AddWithValue("@MONTO_DOCUMENTACIO  ", 0);
                cmd.Parameters.AddWithValue("@MONTO_ANTICIPO      ", 0);
                cmd.Parameters.AddWithValue("@TOTAL_A_COMPRAR     ", totalMerc + imp1 + imp2);
                cmd.Parameters.AddWithValue("@PRIORIDAD           ", "M");
                cmd.Parameters.AddWithValue("@ESTADO              ", estado);
                cmd.Parameters.AddWithValue("@IMPRESA             ", "N");
                cmd.Parameters.AddWithValue("@FECHA_HORA          ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@OBSERVACIONES       ", string.Empty);
                cmd.Parameters.AddWithValue("@REQUIERE_CONFIRMA   ", "S");
                cmd.Parameters.AddWithValue("@CONFIRMADA          ", "N");
                cmd.Parameters.AddWithValue("@ORDEN_PROGRAMADA    ", "P");
                cmd.Parameters.AddWithValue("@RECIBIDO_DE_MAS     ", "N");
                cmd.Parameters.AddWithValue("@TIPO_PRORRATEO_OC   ", "NI");
                cmd.Parameters.AddWithValue("@BASE_IMPUESTO1      ", baseImp1);
                cmd.Parameters.AddWithValue("@BASE_IMPUESTO2      ", baseImp2);
                cmd.Parameters.AddWithValue("@TOT_IMP1_ASUM_DESC  ", 0);
                cmd.Parameters.AddWithValue("@TOT_IMP1_ASUM_NODESC", 0);
                cmd.Parameters.AddWithValue("@TOT_IMP1_RETE_DESC  ", 0);
                cmd.Parameters.AddWithValue("@TOT_IMP1_RETE_NODESC", 0);

                if (cmd.ExecuteNonQuery() < 1)
                {
                    error.AppendLine("[insOC]: Se presentaron problemas insertando la OC: ");
                    error.AppendLine(oc);
                    ok = false;
                }

            }
            catch (Exception ex)
            {                
                error.AppendLine("Orden de compra: " + oc);
                error.AppendLine(ex.Message);
            }

            finally
            {
                cmd.Dispose();
                cmd = null;
            }

            return ok;
        }


        /// <summary>
        /// Metodo encargado de insertar orden_compra
        /// </summary>
        public bool insOCLinea(SqlTransaction transac, string cia, string oc, int linea, string articulo, string artDesc, string bodega, decimal cant
            , decimal precio, decimal imp1, decimal imp2, string estado, string tipoImp1, string tipoTar1
            , ref StringBuilder error)
        {

            bool ok = true;
            StringBuilder sQuery = new StringBuilder();
            SqlCommand cmd = null;
            string lerror = string.Empty;

            try
            {
                sQuery.AppendLine("Insert into " + cia + ".ORDEN_COMPRA_LINEA (");
                sQuery.AppendLine("  ORDEN_COMPRA, ORDEN_COMPRA_LINEA, ARTICULO, BODEGA, LINEA_USUARIO, DESCRIPCION, CANTIDAD_ORDENADA ");
                sQuery.AppendLine(", CANTIDAD_EMBARCADA, CANTIDAD_RECIBIDA, CANTIDAD_RECHAZADA, PRECIO_UNITARIO, IMPUESTO1, IMPUESTO2  ");
                sQuery.AppendLine(", TIPO_DESCUENTO, PORC_DESCUENTO, MONTO_DESCUENTO, FECHA, ESTADO, FACTOR_CONVERSION, FECHA_REQUERIDA");
                sQuery.AppendLine(", DIAS_PARA_ENTREGA, CANTIDAD_ACEPTADA, IMP2_POR_CANTIDAD, IMP1_AFECTA_COSTO, IMP1_ASUMIDO_DESC     ");
                sQuery.AppendLine(", IMP1_ASUMIDO_NODESC, IMP1_RETENIDO_DESC, IMP1_RETENIDO_NODESC, PRECIO_ART_PROV, TIPO_IMPUESTO1    ");
                sQuery.AppendLine(", TIPO_TARIFA1, TIPO_IMPUESTO2, TIPO_TARIFA2, PORC_EXONERACION, MONTO_EXONERACION, ES_CANASTA_BASICA");
                sQuery.AppendLine(", SUBTOTAL_BIENES, SUBTOTAL_SERVICIOS                                                               ");                
                sQuery.AppendLine(") VALUES (");
                sQuery.AppendLine(" @ORDEN_COMPRA,@ORDEN_COMPRA_LINEA,@ARTICULO,@BODEGA,@LINEA_USUARIO,@DESCRIPCION,@CANTIDAD_ORDENADA  ");
                sQuery.AppendLine(",@CANTIDAD_EMBARCADA,@CANTIDAD_RECIBIDA,@CANTIDAD_RECHAZADA,@PRECIO_UNITARIO,@IMPUESTO1,@IMPUESTO2  ");
                sQuery.AppendLine(",@TIPO_DESCUENTO,@PORC_DESCUENTO,@MONTO_DESCUENTO,@FECHA,@ESTADO,@FACTOR_CONVERSION,@FECHA_REQUERIDA");
                sQuery.AppendLine(",@DIAS_PARA_ENTREGA,@CANTIDAD_ACEPTADA,@IMP2_POR_CANTIDAD,@IMP1_AFECTA_COSTO,@IMP1_ASUMIDO_DESC     ");
                sQuery.AppendLine(",@IMP1_ASUMIDO_NODESC,@IMP1_RETENIDO_DESC,@IMP1_RETENIDO_NODESC,@PRECIO_ART_PROV,@TIPO_IMPUESTO1    ");
                sQuery.AppendLine(",@TIPO_TARIFA1,@TIPO_IMPUESTO2,@TIPO_TARIFA2,@PORC_EXONERACION,@MONTO_EXONERACION,@ES_CANASTA_BASICA");
                sQuery.AppendLine(",@SUBTOTAL_BIENES,@SUBTOTAL_SERVICIOS                                                               "); 
                sQuery.AppendLine(")");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sQuery.ToString();

                cmd.Parameters.AddWithValue("@ORDEN_COMPRA        ", oc);
                cmd.Parameters.AddWithValue("@ORDEN_COMPRA_LINEA  ", linea);
                cmd.Parameters.AddWithValue("@ARTICULO            ", articulo);
                cmd.Parameters.AddWithValue("@BODEGA              ", bodega);
                cmd.Parameters.AddWithValue("@LINEA_USUARIO       ", linea);
                cmd.Parameters.AddWithValue("@DESCRIPCION         ", artDesc);
                cmd.Parameters.AddWithValue("@CANTIDAD_ORDENADA   ", cant);
                cmd.Parameters.AddWithValue("@CANTIDAD_EMBARCADA  ", 0);
                cmd.Parameters.AddWithValue("@CANTIDAD_RECIBIDA   ", 0);
                cmd.Parameters.AddWithValue("@CANTIDAD_RECHAZADA  ", 0);
                cmd.Parameters.AddWithValue("@PRECIO_UNITARIO     ", precio);
                cmd.Parameters.AddWithValue("@IMPUESTO1           ", imp1);
                cmd.Parameters.AddWithValue("@IMPUESTO2           ", imp2);
                cmd.Parameters.AddWithValue("@TIPO_DESCUENTO      ", "P");
                cmd.Parameters.AddWithValue("@PORC_DESCUENTO      ", 0);
                cmd.Parameters.AddWithValue("@MONTO_DESCUENTO     ", 0);
                cmd.Parameters.AddWithValue("@FECHA               ", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@ESTADO              ", estado);
                cmd.Parameters.AddWithValue("@FACTOR_CONVERSION   ", 1);
                cmd.Parameters.AddWithValue("@FECHA_REQUERIDA     ", DateTime.Now.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@DIAS_PARA_ENTREGA   ", 0);
                cmd.Parameters.AddWithValue("@CANTIDAD_ACEPTADA   ", 0);
                cmd.Parameters.AddWithValue("@IMP2_POR_CANTIDAD   ", "N");
                cmd.Parameters.AddWithValue("@IMP1_AFECTA_COSTO   ", "N");
                cmd.Parameters.AddWithValue("@IMP1_ASUMIDO_DESC   ", 0);
                cmd.Parameters.AddWithValue("@IMP1_ASUMIDO_NODESC ", 0);
                cmd.Parameters.AddWithValue("@IMP1_RETENIDO_DESC  ", 0);
                cmd.Parameters.AddWithValue("@IMP1_RETENIDO_NODESC", 0);
                cmd.Parameters.AddWithValue("@PRECIO_ART_PROV     ", precio);
                cmd.Parameters.AddWithValue("@TIPO_IMPUESTO1      ", tipoImp1);
                cmd.Parameters.AddWithValue("@TIPO_TARIFA1        ", tipoTar1);
                cmd.Parameters.AddWithValue("@TIPO_IMPUESTO2      ", DBNull.Value);
                cmd.Parameters.AddWithValue("@TIPO_TARIFA2        ", DBNull.Value);
                cmd.Parameters.AddWithValue("@PORC_EXONERACION    ", 0);
                cmd.Parameters.AddWithValue("@MONTO_EXONERACION   ", 0);
                cmd.Parameters.AddWithValue("@ES_CANASTA_BASICA   ", "N");
                cmd.Parameters.AddWithValue("@SUBTOTAL_BIENES     ", 0);
                cmd.Parameters.AddWithValue("@SUBTOTAL_SERVICIOS  ", 0);

                if (cmd.ExecuteNonQuery() < 1)
                {
                    error.AppendLine("[insOCLinea]: Se presentaron problemas insertando la OC línea: ");
                    error.AppendLine(oc);
                    ok = false;
                }
            }
            catch (Exception ex)
            {
                error.AppendLine("Orden de compra: " + oc);
                error.AppendLine(ex.Message);
            }

            finally
            {
                cmd.Dispose();
                cmd = null;
            }

            return ok;
        }
























    }
}
