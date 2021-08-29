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
    class Inventarios
    {
        private SQL sqlClass;

        public Inventarios(SQL _sqlClass)
        {
            this.sqlClass = _sqlClass;
        }

        private string ajusteConfig(string tipo)
        {
            switch (tipo)
            {
                case "Compra":
                    return ("~OO~");
                case "Traspaso":
                    return ("~TT~");
                default:
                    return ("~TT~");
            }
        }


        private string ajusteTipo(string tipo)
        {
            switch (tipo)
            {
                case "Compra":
                    return ("O");
                case "Traspaso":
                    return ("T");
                default:
                    return ("T");
            }
        }        

        /// <summary>
        /// Metodo que obtiene el siguiente valor de la cadena.
        /// </summary>
        public string sgtValor(string valor, int longitud)
        {
            string nValor = string.Empty;
            string txt = string.Empty;
            int sgt = 0;

            //entera
            Regex entera = new Regex(@"\d+");
            Match mEntera = entera.Match(valor);

            //text
            Regex texto = new Regex(@"([A-Za-z\-]+)");
            Match mTexto = texto.Match(valor);

            txt = mTexto.Value;
            sgt = int.Parse(mEntera.Value) + 1;
            nValor = txt + sgt.ToString().PadLeft(mEntera.Length, '0');

            return (nValor);
        }


        /// <summary>
        /// Metodo encargado de extraer la informacion del articulo       
        /// </summary>
        public DataTable getArticulo(string articulo, bool moneda)
        {
            DataTable dtCliente = null;
            StringBuilder sentencia = new StringBuilder();

            if (moneda) //local
            {
                sentencia.Append("select convert(varchar, cast(a.COSTO_PROM_LOC as money), 1) COSTO, a.UNIDAD_VENTA UND, convert(varchar, cast(ap.PRECIO as money), 1) PRECIO from ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".ARTICULO a left join ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".ARTICULO_PRECIO_LOCAL ap on a.ARTICULO = ap.ARTICULO ");
                sentencia.Append("WHERE a.ARTICULO ='");
                sentencia.Append(articulo);
                sentencia.Append("'");
            }
            else //dolar
            {
                sentencia.Append("select convert(varchar, cast(a.COSTO_PROM_DOL as money), 1) COSTO, a.UNIDAD_VENTA UND, convert(varchar, cast(ap.PRECIO as money), 1) PRECIO from ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".ARTICULO a left join ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".ARTICULO_PRECIO_DOLAR ap on a.ARTICULO = ap.ARTICULO ");
                sentencia.Append("WHERE a.ARTICULO ='");
                sentencia.Append(articulo);
                sentencia.Append("'");
            }

            dtCliente = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dtCliente;
        }


        /// <summary>
        /// Obtener el listado de los clasificaciones de articulos (agrupacion 1)      
        /// </summary>
        public DataTable getClasificacion1(SQL _sqlClass)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CLASIFICACION, CLASIFICACION + ' - ' + DESCRIPCION DESCRIPCION from ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".CLASIFICACION ");
            sentencia.Append("where AGRUPACION = 1  ");
            sentencia.Append("order by CLASIFICACION asc ");
            dt = _sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }

        /// <summary>
        /// Obtener el listado de los clasificaciones de articulos (agrupacion 2)      
        /// </summary>
        public DataTable getClasificacion2(SQL _sqlClass)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CLASIFICACION, CLASIFICACION + ' - ' + DESCRIPCION DESCRIPCION from ");
            sentencia.Append(_sqlClass.Compannia);
            sentencia.Append(".CLASIFICACION ");
            sentencia.Append("where AGRUPACION = 2  ");
            sentencia.Append("order by CLASIFICACION asc ");
            dt = _sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }

        /// <summary>
        /// Obtener el listado de los clasificaciones de articulos (agrupacion 1)      
        /// </summary>
        public DataTable getArticuloCtrCta(SqlTransaction transac, string articulo)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CTR_VENTAS_LOC, CTA_VENTAS_LOC from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".ARTICULO a inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".ARTICULO_CUENTA ac on a.ARTICULO_CUENTA = ac.ARTICULO_CUENTA ");
            sentencia.Append("where a.ARTICULO = '");
            sentencia.Append(articulo);
            sentencia.Append("'");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }

        /// <summary>
        /// Metodo obtener informacion del consecutivo
        /// </summary>
        public string getConsecutivoCI(string consecutivo)
        {
            string consec = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select SIGUIENTE_CONSEC from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CONSECUTIVO_CI ");
            sentencia.Append("WHERE CONSECUTIVO = '");
            sentencia.Append(consecutivo);
            sentencia.Append("' ");

            consec = sqlClass.EjecutarScalar(sentencia.ToString()).ToString();

            return consec;
        }

        /// <summary>
        /// Metodo valida si el articulo existe     
        /// </summary>
        public int existeArticulo(string articulo)
        {
            int q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT COUNT(1) FROM ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".ARTICULO ");
            sentencia.Append("WHERE ARTICULO = '");
            sentencia.Append(articulo);
            sentencia.Append("'");
            q = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString()).ToString());
            return (q);

        }


        /// <summary>
        /// Metodo valida si el articulo existe     
        /// </summary>
        public int existeArticuloBodega(string articulo, string bodega)
        {
            int q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT COUNT(1) FROM ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".EXISTENCIA_BODEGA ");
            sentencia.Append("WHERE ARTICULO = '");
            sentencia.Append(articulo);
            sentencia.Append("' AND BODEGA ='");
            sentencia.Append(bodega);
            sentencia.Append("'");
            q = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString()).ToString());
            return (q);
        }


        /// <summary>
        /// Metodo valida si el articulo existe     
        /// </summary>
        public int existeArticuloLote(string articulo, string lote)
        {
            int q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT COUNT(1) FROM ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".LOTE ");
            sentencia.Append("WHERE ARTICULO = '");
            sentencia.Append(articulo);
            sentencia.Append("' AND LOTE ='");
            sentencia.Append(lote);
            sentencia.Append("'");
            q = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString()).ToString());
            return (q);
        }

        /// <summary>
        /// Metodo valida si el articulo existe     
        /// </summary>
        public decimal existeArticuloBodegaExistencia(string articulo, string bodega)
        {
            decimal q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT ISNULL(CANT_DISPONIBLE,0) FROM ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".EXISTENCIA_BODEGA ");
            sentencia.Append("WHERE ARTICULO = '");
            sentencia.Append(articulo);
            sentencia.Append("' AND BODEGA ='");
            sentencia.Append(bodega);
            sentencia.Append("' ");

            object res = sqlClass.EjecutarScalar(sentencia.ToString());

            if (res != null)
                q = decimal.Parse(res.ToString());

            return (q);
        }

        /// <summary>
        /// Obtener la transaccion siguiente      
        /// </summary>
        public int getSgtAudiTransInv()
        {
            int dt = 0;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append(" select isnull(max(AUDIT_TRANS_INV),1) from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".AUDIT_TRANS_INV ");

            dt = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString()).ToString());

            return dt;
        }


        /// <summary>
        /// Obtener la transaccion siguiente      
        /// </summary>
        public int getAudiTransInv(string aplicacion)
        {
            int dt = 0;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append(" select AUDIT_TRANS_INV from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".AUDIT_TRANS_INV ");
            sentencia.Append(" where APLICACION = '");
            sentencia.Append(aplicacion);
            sentencia.Append("'");

            dt = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString()).ToString());

            return dt;
        }


        /// <summary>
        /// Obtener el articulo siguiente      
        /// </summary>
        public string getSgtArticulo()
        {
            string dt = string.Empty;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append(" select top 1 ARTICULO from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".ARTICULO ");
            sentencia.Append("ORDER BY CREATEDATE DESC ");

            dt = sgtValor(sqlClass.EjecutarScalar(sentencia.ToString()).ToString(), 0);

            return dt;
        }

        /// <summary>
        /// Metodo valida si el articulo existe     
        /// </summary>
        public decimal getTipoCambio(string tipoCambio)
        {
            decimal q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT top 1 MONTO FROM ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".TIPO_CAMBIO_HIST ");
            sentencia.Append("WHERE TIPO_CAMBIO = '");
            sentencia.Append(tipoCambio);
            sentencia.Append("' order by FECHA desc ");
            q = decimal.Parse(sqlClass.EjecutarScalar(sentencia.ToString()).ToString());
            return (q);

        }

        /// <summary>
        /// Metodo encargado de insertar el articulo
        /// </summary>
        public bool insArticulo(string empeno, string articulo, string desc, string tipo, decimal peso, decimal costo, decimal precio, string clasif1, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();
            decimal tc = getTipoCambio("TVEN");

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".ARTICULO ");
                sentencia.Append("(ARTICULO,DESCRIPCION,TIPO,ORIGEN_CORP,PESO_NETO,PESO_BRUTO,VOLUMEN,BULTOS,ARTICULO_CUENTA,IMPUESTO,FACTOR_EMPAQUE" +
                                " ,FACTOR_VENTA,EXISTENCIA_MINIMA,EXISTENCIA_MAXIMA,PUNTO_DE_REORDEN,COSTO_FISCAL,COSTO_COMPARATIVO,COSTO_PROM_LOC " +
                                " ,COSTO_PROM_DOL,COSTO_STD_LOC,COSTO_STD_DOL,COSTO_ULT_LOC,COSTO_ULT_DOL,PRECIO_BASE_LOCAL,PRECIO_BASE_DOLAR      " +
                                " ,ULTIMA_SALIDA,ULTIMO_MOVIMIENTO,ULTIMO_INGRESO,ULTIMO_INVENTARIO,CLASE_ABC,FRECUENCIA_CONTEO,ACTIVO,USA_LOTES   " +
                                " ,OBLIGA_CUARENTENA,MIN_VIDA_COMPRA,MIN_VIDA_CONSUMO,MIN_VIDA_VENTA,VIDA_UTIL_PROM,DIAS_CUARENTENA,ORDEN_MINIMA   " +
                                " ,PLAZO_REABAST,LOTE_MULTIPLO,UTILIZADO_MANUFACT,USA_NUMEROS_SERIE,UNIDAD_ALMACEN,UNIDAD_EMPAQUE,UNIDAD_VENTA     " +
                                " ,PERECEDERO,CLASIFICACION_1,NOTAS,USUARIO_CREACION,FCH_HORA_CREACION,MODALIDAD_INV_FIS,USA_REGLAS_LOCALES");
                sentencia.Append(") VALUES (");
                sentencia.Append("  @ARTICULO,@DESCRIPCION,@TIPO,@ORIGEN_CORP,@PESO_NETO,@PESO_BRUTO,@VOLUMEN,@BULTOS,@ARTICULO_CUENTA,@IMPUESTO,@FACTOR_EMPAQUE" +
                                " ,@FACTOR_VENTA,@EXISTENCIA_MINIMA,@EXISTENCIA_MAXIMA,@PUNTO_DE_REORDEN,@COSTO_FISCAL,@COSTO_COMPARATIVO,@COSTO_PROM_LOC " +
                                " ,@COSTO_PROM_DOL,@COSTO_STD_LOC,@COSTO_STD_DOL,@COSTO_ULT_LOC,@COSTO_ULT_DOL,@PRECIO_BASE_LOCAL,@PRECIO_BASE_DOLAR      " +
                                " ,@ULTIMA_SALIDA,@ULTIMO_MOVIMIENTO,@ULTIMO_INGRESO,@ULTIMO_INVENTARIO,@CLASE_ABC,@FRECUENCIA_CONTEO,@ACTIVO,@USA_LOTES   " +
                                " ,@OBLIGA_CUARENTENA,@MIN_VIDA_COMPRA,@MIN_VIDA_CONSUMO,@MIN_VIDA_VENTA,@VIDA_UTIL_PROM,@DIAS_CUARENTENA,@ORDEN_MINIMA   " +
                                " ,@PLAZO_REABAST,@LOTE_MULTIPLO,@UTILIZADO_MANUFACT,@USA_NUMEROS_SERIE,@UNIDAD_ALMACEN,@UNIDAD_EMPAQUE,@UNIDAD_VENTA     " +
                                " ,@PERECEDERO,@CLASIFICACION_1,@NOTAS,@USUARIO_CREACION,@FCH_HORA_CREACION,@MODALIDAD_INV_FIS,@USA_REGLAS_LOCALES)");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros  
                cmd.Parameters.Add("@ARTICULO          ", SqlDbType.VarChar, 20).Value = articulo;
                cmd.Parameters.Add("@DESCRIPCION       ", SqlDbType.VarChar, 254).Value = desc.ToUpper(); ;
                cmd.Parameters.Add("@TIPO              ", SqlDbType.VarChar, 1).Value = tipo;
                cmd.Parameters.Add("@ORIGEN_CORP       ", SqlDbType.VarChar, 1).Value = "T";
                cmd.Parameters.Add("@PESO_NETO         ", SqlDbType.Decimal).Value = peso;
                cmd.Parameters.Add("@PESO_BRUTO        ", SqlDbType.Decimal).Value = peso;
                cmd.Parameters.Add("@VOLUMEN           ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@BULTOS            ", SqlDbType.SmallInt).Value = 1;
                cmd.Parameters.Add("@ARTICULO_CUENTA   ", SqlDbType.VarChar, 4).Value = ConfigurationManager.AppSettings["ARTICULO_CUENTA"].ToString();
                cmd.Parameters.Add("@IMPUESTO          ", SqlDbType.VarChar, 4).Value = ConfigurationManager.AppSettings["IMPUESTO"].ToString();
                cmd.Parameters.Add("@FACTOR_EMPAQUE    ", SqlDbType.Decimal).Value = 1;
                cmd.Parameters.Add("@FACTOR_VENTA      ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@EXISTENCIA_MINIMA ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@EXISTENCIA_MAXIMA ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@PUNTO_DE_REORDEN  ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@COSTO_FISCAL      ", SqlDbType.VarChar, 1).Value = ConfigurationManager.AppSettings["COSTO_FISCAL"].ToString();
                cmd.Parameters.Add("@COSTO_COMPARATIVO ", SqlDbType.VarChar, 1).Value = ConfigurationManager.AppSettings["COSTO_COMPARATIVO"].ToString();
                cmd.Parameters.Add("@COSTO_PROM_LOC    ", SqlDbType.Decimal).Value = costo;
                cmd.Parameters.Add("@COSTO_PROM_DOL    ", SqlDbType.Decimal).Value = costo / tc;
                cmd.Parameters.Add("@COSTO_STD_LOC     ", SqlDbType.Decimal).Value = costo;
                cmd.Parameters.Add("@COSTO_STD_DOL     ", SqlDbType.Decimal).Value = costo / tc;
                cmd.Parameters.Add("@COSTO_ULT_LOC     ", SqlDbType.Decimal).Value = costo;
                cmd.Parameters.Add("@COSTO_ULT_DOL     ", SqlDbType.Decimal).Value = costo / tc;
                cmd.Parameters.Add("@PRECIO_BASE_LOCAL ", SqlDbType.Decimal).Value = precio;
                cmd.Parameters.Add("@PRECIO_BASE_DOLAR ", SqlDbType.Decimal).Value = precio / tc;
                cmd.Parameters.Add("@ULTIMA_SALIDA     ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@ULTIMO_MOVIMIENTO ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@ULTIMO_INGRESO    ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@ULTIMO_INVENTARIO ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@CLASE_ABC         ", SqlDbType.VarChar, 1).Value = "A";
                cmd.Parameters.Add("@FRECUENCIA_CONTEO ", SqlDbType.SmallInt).Value = 0;
                cmd.Parameters.Add("@ACTIVO            ", SqlDbType.VarChar, 1).Value = "S";
                cmd.Parameters.Add("@USA_LOTES         ", SqlDbType.VarChar, 1).Value = "N";
                cmd.Parameters.Add("@OBLIGA_CUARENTENA ", SqlDbType.VarChar, 1).Value = "N";
                cmd.Parameters.Add("@MIN_VIDA_COMPRA   ", SqlDbType.SmallInt).Value = 0;
                cmd.Parameters.Add("@MIN_VIDA_CONSUMO  ", SqlDbType.SmallInt).Value = 0;
                cmd.Parameters.Add("@MIN_VIDA_VENTA    ", SqlDbType.SmallInt).Value = 0;
                cmd.Parameters.Add("@VIDA_UTIL_PROM    ", SqlDbType.SmallInt).Value = 0;
                cmd.Parameters.Add("@DIAS_CUARENTENA   ", SqlDbType.SmallInt).Value = 0;
                cmd.Parameters.Add("@ORDEN_MINIMA      ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@PLAZO_REABAST     ", SqlDbType.SmallInt).Value = 0;
                cmd.Parameters.Add("@LOTE_MULTIPLO     ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@UTILIZADO_MANUFACT", SqlDbType.VarChar, 1).Value = "N";
                cmd.Parameters.Add("@USA_NUMEROS_SERIE ", SqlDbType.VarChar, 1).Value = "N";
                cmd.Parameters.Add("@MODALIDAD_INV_FIS ", SqlDbType.VarChar, 1).Value = "T";
                cmd.Parameters.Add("@USA_REGLAS_LOCALES ", SqlDbType.VarChar, 1).Value = "S";
                cmd.Parameters.Add("@UNIDAD_ALMACEN    ", SqlDbType.VarChar, 6).Value = "UND";
                cmd.Parameters.Add("@UNIDAD_EMPAQUE    ", SqlDbType.VarChar, 6).Value = "UND";
                cmd.Parameters.Add("@UNIDAD_VENTA      ", SqlDbType.VarChar, 6).Value = "UND";
                cmd.Parameters.Add("@PERECEDERO        ", SqlDbType.VarChar, 1).Value = "N";
                cmd.Parameters.Add("@CLASIFICACION_1   ", SqlDbType.VarChar, 12).Value = clasif1;
                cmd.Parameters.Add("@NOTAS   ", SqlDbType.VarChar, 12).Value = "Empeño #: " + empeno;
                cmd.Parameters.Add("@USUARIO_CREACION   ", SqlDbType.VarChar, 12).Value = sqlClass.Usuario;
                cmd.Parameters.Add("@FCH_HORA_CREACION   ", SqlDbType.VarChar, 12).Value = DateTime.Now.ToString("yyyy-MM-dd");

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insArticulo]: Se presentaron problemas insertando el articulo: ");
                    errores.AppendLine(empeno + " - " + articulo);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insArticulo]: Detalle Tecnico: " + e.Message);
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
        /// Metodo encargado de insertar el articulo
        /// </summary>
        public bool insLote(string articulo, string lote, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".LOTE ");
                sentencia.Append("(lote, articulo,lote_del_proveedor,fecha_entrada,fecha_vencimiento,fecha_cuarentena,cantidad_ingresada,estado,tipo_ingreso,ultimo_ingreso");
                sentencia.Append(") VALUES (");
                sentencia.Append("@lote,@articulo,@lote_del_proveedor,@fecha_entrada,@fecha_vencimiento,@fecha_cuarentena,@cantidad_ingresada,@estado,@tipo_ingreso,@ultimo_ingreso)");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros  
                cmd.Parameters.Add("@lote         ", SqlDbType.VarChar, 15).Value = lote;
                cmd.Parameters.Add("@articulo           ", SqlDbType.VarChar, 25).Value = articulo;
                cmd.Parameters.Add("@lote_del_proveedor", SqlDbType.VarChar, 15).Value = lote;
                cmd.Parameters.Add("@fecha_entrada", SqlDbType.DateTime).Value = new DateTime(1980, 01, 01);
                cmd.Parameters.Add("@fecha_vencimiento ", SqlDbType.DateTime).Value = new DateTime(1980, 01, 01);
                cmd.Parameters.Add("@fecha_cuarentena  ", SqlDbType.DateTime).Value = new DateTime(1980, 01, 01);
                cmd.Parameters.Add("@cantidad_ingresada   ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@estado ", SqlDbType.VarChar, 1).Value = "V";
                cmd.Parameters.Add("@tipo_ingreso     ", SqlDbType.VarChar, 1).Value = "P";
                cmd.Parameters.Add("@ultimo_ingreso    ", SqlDbType.Int).Value = 0;

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insLote]: Se presentaron problemas insertando el lote: ");
                    errores.AppendLine(articulo);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insLote]: Detalle Tecnico: " + e.Message);
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
        /// Metodo encargado de insertar el articulo
        /// </summary>
        public bool insArticuloBodega(string articulo, string bodega, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".EXISTENCIA_BODEGA ");
                sentencia.Append("(ARTICULO,BODEGA,EXISTENCIA_MINIMA,EXISTENCIA_MAXIMA,PUNTO_DE_REORDEN,CANT_DISPONIBLE  " +
                                ",CANT_RESERVADA,CANT_NO_APROBADA,CANT_VENCIDA,CANT_TRANSITO,CANT_PRODUCCION,CANT_PEDIDA" +
                                ",CANT_REMITIDA,CONGELADO,BLOQUEA_TRANS                                                 ");
                sentencia.Append(") VALUES (");
                sentencia.Append("@ARTICULO,@BODEGA,@EXISTENCIA_MINIMA,@EXISTENCIA_MAXIMA,@PUNTO_DE_REORDEN,@CANT_DISPONIBLE  " +
                                ",@CANT_RESERVADA,@CANT_NO_APROBADA,@CANT_VENCIDA,@CANT_TRANSITO,@CANT_PRODUCCION,@CANT_PEDIDA" +
                                ",@CANT_REMITIDA,@CONGELADO,@BLOQUEA_TRANS                                                 )");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros  
                cmd.Parameters.Add("@ARTICULO         ", SqlDbType.VarChar, 20).Value = articulo;
                cmd.Parameters.Add("@BODEGA           ", SqlDbType.VarChar, 4).Value = bodega;
                cmd.Parameters.Add("@EXISTENCIA_MINIMA", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@EXISTENCIA_MAXIMA", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@PUNTO_DE_REORDEN ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@CANT_DISPONIBLE  ", SqlDbType.Decimal).Value = 1;
                cmd.Parameters.Add("@CANT_RESERVADA   ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@CANT_NO_APROBADA ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@CANT_VENCIDA     ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@CANT_TRANSITO    ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@CANT_PRODUCCION  ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@CANT_PEDIDA      ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@CANT_REMITIDA    ", SqlDbType.Decimal).Value = 0;
                cmd.Parameters.Add("@CONGELADO        ", SqlDbType.VarChar, 1).Value = "N";
                cmd.Parameters.Add("@BLOQUEA_TRANS    ", SqlDbType.VarChar, 1).Value = "N";

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insArticuloBodega]: Se presentaron problemas insertando el articulo: ");
                    errores.AppendLine(articulo);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insArticuloBodega]: Detalle Tecnico: " + e.Message);
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
        /// Actualizar la informacion del articulo
        /// </summary>
        public bool updArticulo(string articulo, string desc, string tipo, decimal peso, decimal costo, decimal precio, string clasif1, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".ARTICULO ");
                sentencia.Append("set DESCRIPCION = '" + desc.ToUpper() + "' ");
                sentencia.Append(",TIPO = '" + tipo + "' ");
                sentencia.Append(", PESO_NETO    = " + peso.ToString());
                sentencia.Append(", PESO_BRUTO    = " + peso.ToString());
                sentencia.Append(", COSTO_PROM_LOC    = " + costo.ToString());
                sentencia.Append(", COSTO_PROM_DOL    = " + costo.ToString());
                sentencia.Append(", COSTO_STD_LOC     = " + costo.ToString());
                sentencia.Append(", COSTO_STD_DOL     = " + costo.ToString());
                sentencia.Append(", COSTO_ULT_LOC     = " + costo.ToString());
                sentencia.Append(", COSTO_ULT_DOL     = " + costo.ToString());
                sentencia.Append(", PRECIO_BASE_LOCAL = " + precio.ToString());
                sentencia.Append(", PRECIO_BASE_DOLAR = " + precio.ToString());
                sentencia.Append(",CLASIFICACION_1 = '" + clasif1 + "' ");
                sentencia.Append(" WHERE ARTICULO = '");
                sentencia.Append(articulo);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[updArticulo]: Se presentaron problemas actualizando el articulo: ");
                    errores.Append(articulo);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updArticulo]: Detalle Tecnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }


        /// <summary>
        /// Actualizar la informacion del articulo
        /// </summary>
        public bool updArticuloBodega(string articulo, string bodega, string bodegaN, int cant, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".EXISTENCIA_BODEGA ");
                sentencia.Append("set BODEGA = '" + bodegaN + "' ");
                sentencia.Append(", CANT_DISPONIBLE = CANT_DISPONIBLE + " + cant.ToString());
                sentencia.Append(" WHERE ARTICULO = '");
                sentencia.Append(articulo);
                sentencia.Append("' AND BODEGA ='");
                sentencia.Append(bodega);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[updArticulo]: Se presentaron problemas actualizando el articulo: ");
                    errores.Append(articulo);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updArticulo]: Detalle Tecnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }

        /// <summary>
        /// Actualizar el consecutivo
        /// </summary>
        public bool updConsecutivoCI(string consecutivo, string ultimo_valor, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".CONSECUTIVO_CI ");
                sentencia.Append("SET SIGUIENTE_CONSEC = '");
                sentencia.Append(ultimo_valor);
                sentencia.Append("' WHERE CONSECUTIVO = '");
                sentencia.Append(consecutivo);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[updConsecutivoCI]: Se presentaron problemas actualizando el consecutivo: ");
                    errores.Append(ultimo_valor);
                    errores.AppendLine(" Para el consecutivo:");
                    errores.Append(consecutivo);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updConsecutivoCI]: Detalle Tecnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }

        /// <summary>
        /// Metodo encargado de insertar el audit trans inv
        /// </summary>
        public bool insAuditTransInv(ref int audit, string consec, string aplicacion, string referencia, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".AUDIT_TRANS_INV ");
                sentencia.Append("(CONSECUTIVO,USUARIO,FECHA_HORA,MODULO_ORIGEN,APLICACION,REFERENCIA");
                sentencia.Append(") VALUES (");
                sentencia.Append("@CONSECUTIVO,@USUARIO,@FECHA_HORA,@MODULO_ORIGEN,@APLICACION,@REFERENCIA)");
                sentencia.Append(" SET @ID = SCOPE_IDENTITY(); ");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros
                cmd.Parameters.Add("@CONSECUTIVO", SqlDbType.VarChar, 10).Value = consec;
                cmd.Parameters.Add("@USUARIO", SqlDbType.VarChar, 10).Value = sqlClass.Usuario;
                cmd.Parameters.Add("@FECHA_HORA", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@MODULO_ORIGEN", SqlDbType.VarChar, 4).Value = "CI";
                cmd.Parameters.Add("@APLICACION", SqlDbType.VarChar, 249).Value = aplicacion;
                cmd.Parameters.Add("@REFERENCIA", SqlDbType.VarChar, 200).Value = referencia;
                cmd.Parameters.Add("@ID", SqlDbType.Int).Direction = ParameterDirection.Output;

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insAuditTransInv]: Se presentaron problemas insertando el audit_trans_inv: ");
                    errores.AppendLine(consec);
                    lbOK = false;
                }
                audit = int.Parse(cmd.Parameters["@ID"].Value.ToString());
            }
            catch (Exception e)
            {
                errores.AppendLine("[insAuditTransInv]: Detalle Tecnico: " + e.Message);
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
        /// Metodo encargado de insertar el trans inv
        /// </summary>
        public bool insTransaccionInv(int audit, int linea, string ajuste, string articulo, string bodega, string naturaleza, decimal cant, decimal costo, decimal precio, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();
            decimal tc = getTipoCambio("TVEN");

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".TRANSACCION_INV ");
                sentencia.Append("(AUDIT_TRANS_INV,CONSECUTIVO,FECHA_HORA_TRANSAC,AJUSTE_CONFIG,ARTICULO,BODEGA,TIPO" +
                                ",SUBTIPO,SUBSUBTIPO,NATURALEZA,CANTIDAD,COSTO_TOT_FISC_LOC,COSTO_TOT_FISC_DOL" +
                                ",COSTO_TOT_COMP_LOC,COSTO_TOT_COMP_DOL,PRECIO_TOTAL_LOCAL,PRECIO_TOTAL_DOLAR" +
                                ",CONTABILIZADA,FECHA");
                sentencia.Append(") VALUES (");
                sentencia.Append("@AUDIT_TRANS_INV,@CONSECUTIVO,@FECHA_HORA_TRANSAC,@AJUSTE_CONFIG,@ARTICULO,@BODEGA,@TIPO" +
                                ",@SUBTIPO,@SUBSUBTIPO,@NATURALEZA,@CANTIDAD,@COSTO_TOT_FISC_LOC,@COSTO_TOT_FISC_DOL" +
                                ",@COSTO_TOT_COMP_LOC,@COSTO_TOT_COMP_DOL,@PRECIO_TOTAL_LOCAL,@PRECIO_TOTAL_DOLAR" +
                                ",@CONTABILIZADA,@FECHA)");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros
                cmd.Parameters.Add("@AUDIT_TRANS_INV", SqlDbType.Int).Value = audit;
                cmd.Parameters.Add("@CONSECUTIVO", SqlDbType.Int).Value = linea;
                cmd.Parameters.Add("@FECHA_HORA_TRANSAC", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@AJUSTE_CONFIG", SqlDbType.VarChar, 4).Value = ajusteConfig(ajuste);
                cmd.Parameters.Add("@ARTICULO", SqlDbType.VarChar, 20).Value = articulo;
                cmd.Parameters.Add("@BODEGA", SqlDbType.VarChar, 4).Value = bodega;
                cmd.Parameters.Add("@TIPO", SqlDbType.VarChar, 1).Value = ajusteTipo(ajuste);
                cmd.Parameters.Add("@SUBTIPO           ", SqlDbType.VarChar, 1).Value = "D";
                cmd.Parameters.Add("@SUBSUBTIPO        ", SqlDbType.VarChar, 1).Value = " ";
                cmd.Parameters.Add("@NATURALEZA        ", SqlDbType.VarChar, 1).Value = naturaleza;
                cmd.Parameters.Add("@CANTIDAD          ", SqlDbType.Decimal).Value = cant;
                cmd.Parameters.Add("@COSTO_TOT_FISC_LOC", SqlDbType.Decimal).Value = costo;
                cmd.Parameters.Add("@COSTO_TOT_FISC_DOL", SqlDbType.Decimal).Value = costo / tc;
                cmd.Parameters.Add("@COSTO_TOT_COMP_LOC", SqlDbType.Decimal).Value = costo;
                cmd.Parameters.Add("@COSTO_TOT_COMP_DOL", SqlDbType.Decimal).Value = costo / tc;
                cmd.Parameters.Add("@PRECIO_TOTAL_LOCAL", SqlDbType.Decimal).Value = precio;
                cmd.Parameters.Add("@PRECIO_TOTAL_DOLAR", SqlDbType.Decimal).Value = precio / tc;
                cmd.Parameters.Add("@CONTABILIZADA     ", SqlDbType.VarChar, 1).Value = "N";
                cmd.Parameters.Add("@FECHA             ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd");

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insTransaccionInv]: Se presentaron problemas insertando el transaccion_inv: ");
                    errores.AppendLine(audit.ToString());
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insTransaccionInv]: Detalle Tecnico: " + e.Message);
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
        /// Metodo encargado de insertar el documento_inv
        /// </summary>
        public bool insDocumentoInv(string paquete, string consecutivo, string documento, string referencia, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".DOCUMENTO_INV ");
                sentencia.Append("(PAQUETE_INVENTARIO,DOCUMENTO_INV,CONSECUTIVO,REFERENCIA,FECHA_HOR_CREACION,FECHA_DOCUMENTO,SELECCIONADO,USUARIO,MENSAJE_SISTEMA");
                sentencia.Append(") VALUES (");
                sentencia.Append("@PAQUETE_INVENTARIO,@DOCUMENTO_INV,@CONSECUTIVO,@REFERENCIA,@FECHA_HOR_CREACION,@FECHA_DOCUMENTO,@SELECCIONADO,@USUARIO,@MENSAJE_SISTEMA)");
                

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros

                cmd.Parameters.Add("@PAQUETE_INVENTARIO", SqlDbType.VarChar, 4).Value = paquete;
                cmd.Parameters.Add("@DOCUMENTO_INV     ", SqlDbType.VarChar, 50).Value = documento;
                cmd.Parameters.Add("@CONSECUTIVO       ", SqlDbType.VarChar, 10).Value = consecutivo;
                cmd.Parameters.Add("@REFERENCIA        ", SqlDbType.VarChar, 200).Value = referencia;
                cmd.Parameters.Add("@FECHA_HOR_CREACION", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cmd.Parameters.Add("@FECHA_DOCUMENTO   ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@SELECCIONADO      ", SqlDbType.VarChar, 1).Value = "N";
                cmd.Parameters.Add("@USUARIO           ", SqlDbType.VarChar, 10).Value = sqlClass.Usuario;
                cmd.Parameters.Add("@MENSAJE_SISTEMA   ", SqlDbType.VarChar, 20).Value = string.Empty;

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insDocumentoInv]: Se presentaron problemas insertando el documento_inv: ");
                    errores.AppendLine(documento);
                    lbOK = false;
                }
                
            }
            catch (Exception e)
            {
                errores.AppendLine("[insDocumentoInv]: Detalle Técnico: " + e.Message);
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
        /// Metodo encargado de insertar el linea_documento_inv
        /// </summary>
        public bool insLineaDocumentoInv(string paquete, string documento, int linea, string ajuste, string articulo, string bodegaO, string tipo, string subsubtipo, decimal cantidad, decimal costo_local, decimal costo_dolar, decimal precio_local, decimal precio_dolar, string bodegaD, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".LINEA_DOC_INV ");
                sentencia.Append("(PAQUETE_INVENTARIO,DOCUMENTO_INV,LINEA_DOC_INV,AJUSTE_CONFIG,ARTICULO,BODEGA,TIPO,SUBTIPO,SUBSUBTIPO,CANTIDAD,COSTO_TOTAL_LOCAL,COSTO_TOTAL_DOLAR,PRECIO_TOTAL_LOCAL,PRECIO_TOTAL_DOLAR,BODEGA_DESTINO,CENTRO_COSTO,CUENTA_CONTABLE");
                sentencia.Append(") VALUES (");
                sentencia.Append("@PAQUETE_INVENTARIO,@DOCUMENTO_INV,@LINEA_DOC_INV,@AJUSTE_CONFIG,@ARTICULO,@BODEGA,@TIPO,@SUBTIPO,@SUBSUBTIPO,@CANTIDAD,@COSTO_TOTAL_LOCAL,@COSTO_TOTAL_DOLAR,@PRECIO_TOTAL_LOCAL,@PRECIO_TOTAL_DOLAR,@BODEGA_DESTINO,@CENTRO_COSTO,@CUENTA_CONTABLE)");


                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros
                cmd.Parameters.Add("@PAQUETE_INVENTARIO", SqlDbType.VarChar, 4).Value = paquete;
                cmd.Parameters.Add("@DOCUMENTO_INV     ", SqlDbType.VarChar, 50).Value = documento;
                cmd.Parameters.Add("@LINEA_DOC_INV     ", SqlDbType.Int).Value = linea;
                cmd.Parameters.Add("@AJUSTE_CONFIG     ", SqlDbType.VarChar, 4).Value = ajuste;
                cmd.Parameters.Add("@ARTICULO          ", SqlDbType.VarChar, 20).Value = articulo;
                cmd.Parameters.Add("@BODEGA            ", SqlDbType.VarChar, 4).Value = bodegaO;
                cmd.Parameters.Add("@TIPO              ", SqlDbType.VarChar, 1).Value = tipo;
                cmd.Parameters.Add("@SUBTIPO           ", SqlDbType.VarChar, 1).Value = "D";
                cmd.Parameters.Add("@SUBSUBTIPO        ", SqlDbType.VarChar, 1).Value = subsubtipo;
                cmd.Parameters.Add("@CANTIDAD          ", SqlDbType.Decimal).Value = cantidad;
                cmd.Parameters.Add("@COSTO_TOTAL_LOCAL ", SqlDbType.Decimal).Value = costo_local;
                cmd.Parameters.Add("@COSTO_TOTAL_DOLAR ", SqlDbType.Decimal).Value = costo_dolar;
                cmd.Parameters.Add("@PRECIO_TOTAL_LOCAL", SqlDbType.Decimal).Value = precio_local;
                cmd.Parameters.Add("@PRECIO_TOTAL_DOLAR", SqlDbType.Decimal).Value = precio_dolar;
                if(bodegaD.CompareTo(string.Empty)==0)
                    cmd.Parameters.Add("@BODEGA_DESTINO    ", SqlDbType.VarChar, 4).Value = DBNull.Value;
                else
                    cmd.Parameters.Add("@BODEGA_DESTINO    ", SqlDbType.VarChar, 4).Value = bodegaD;

                cmd.Parameters.Add("@CENTRO_COSTO      ", SqlDbType.VarChar, 25).Value = DBNull.Value;
                cmd.Parameters.Add("@CUENTA_CONTABLE   ", SqlDbType.VarChar, 25).Value = DBNull.Value;

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insLineaDocumentoInv]: Se presentaron problemas insertando el linea_documento_inv: ");
                    errores.AppendLine(documento);
                    lbOK = false;
                }

            }
            catch (Exception e)
            {
                errores.AppendLine("[insLineaDocumentoInv]: Detalle Técnico: " + e.Message);
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
        /// Eliminar documento inv
        /// </summary>
        public bool delDocumentoInvDet(string docinv, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("DELETE FROM ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".LINEA_DOC_INV ");
                sentencia.Append(" WHERE DOCUMENTO_INV = '");
                sentencia.Append(docinv);
                sentencia.Append("'");


                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[delDocumentoInvDet]: Se presentaron problemas eliminando el documento inv det: ");
                    errores.Append(docinv);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[delDocumentoInvDet]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }

        /// <summary>
        /// Eliminar documento inv
        /// </summary>
        public bool delDocumentoInv(string docinv, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("DELETE FROM ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".DOCUMENTO_INV ");
                sentencia.Append(" WHERE DOCUMENTO_INV = '");
                sentencia.Append(docinv);
                sentencia.Append("'");


                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[delDocumentoInv]: Se presentaron problemas eliminando el documento inventario: ");
                    errores.Append(docinv);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[delDocumentoInv]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }


        /// <summary>
        /// Metodo encargado de procesar la creacion de documento inv
        /// </summary>
        public bool procesoCrearDocInv(string paquete, string consecutivo, string referencia, ref string rvalor, ref StringBuilder errores)
        {
            bool lbOk = true;

            try
            {
                //obtener informacion consecutivo
                string sgtConsec = getConsecutivoCI(consecutivo);

                //insertar el documento inv
                lbOk = insDocumentoInv(paquete, consecutivo, sgtConsec, referencia, ref errores);
                if (lbOk)
                {
                    string valor = sgtValor(sgtConsec, 0);

                    //actualizar el consecutivo en la tbl CONSECUTIVO
                    lbOk = updConsecutivoCI(consecutivo, valor, ref errores);

                    if (lbOk)
                    {
                        rvalor = sgtConsec;                       
                    }
                }
            }
            catch (Exception ex)
            {
                errores.AppendLine("[procesoCrearDocInv]: Detalle Técnico: " + ex.Message);
                lbOk = false;
            }
            return (lbOk);
        }


        /// <summary>
        /// Obtener listado de articulos      
        /// </summary>
        public DataTable getArticulo(SqlTransaction transac, string articulo, string np, string moneda)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append("  a.ARTICULO  ");
            sentencia.Append(", a.DESCRIPCION  ");
            sentencia.Append(", a.CODIGO_BARRAS_VENT  ");
            sentencia.Append(", a.COSTO_PROM_LOC  ");
            //sentencia.Append(", ap.PRECIO / (1 + (i.IMPUESTO1/100)) PRECIO ");
            //sentencia.Append(", ap.PRECIO  PRECIO_IVI ");
            sentencia.Append(", ap.PRECIO  PRECIO ");
            sentencia.Append(", ap.PRECIO + (ap.PRECIO * (i.IMPUESTO1/100))  PRECIO_IVI ");
            sentencia.Append(", eb.CANT_DISPONIBLE  ");
            sentencia.Append(", eb.BODEGA  ");
            sentencia.Append(", ISNULL(c.U_DESCUENTO, 0) U_DESCUENTO ");            
            sentencia.Append(", a.TIPO ");
            sentencia.Append(",i.IMPUESTO1, i.TIPO_IMPUESTO1, i.TIPO_TARIFA1, a.CANASTA_BASICA, a.ES_OTRO_CARGO, ac.CTR_VENTAS_LOC, ac.CTA_VENTAS_LOC ");
            sentencia.Append(" from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".ARTICULO a inner join  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".EXISTENCIA_BODEGA eb on a.ARTICULO = eb.ARTICULO left join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".ARTICULO_PRECIO ap on a.ARTICULO = ap.ARTICULO inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".VERSION_NIVEL v on ap.NIVEL_PRECIO = v.NIVEL_PRECIO and ap.MONEDA = v.MONEDA and ap.VERSION = v.VERSION inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".IMPUESTO i on a.IMPUESTO = i.IMPUESTO left join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".CLASIFICACION c on a.CLASIFICACION_3 = c.CLASIFICACION and c.AGRUPACION = " + ConfigurationManager.AppSettings["No_CLASIF_DESCUESTO"].ToString() + " inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".ARTICULO_CUENTA ac on a.ARTICULO_CUENTA = ac.ARTICULO_CUENTA ");
            sentencia.Append("where v.NIVEL_PRECIO = '" + np + "'");
            sentencia.Append(" and v.MONEDA = '" + moneda + "'");
            sentencia.Append("and v.ESTADO = 'A' and GETDATE() between v.FECHA_INICIO and v.FECHA_CORTE ");           
            sentencia.Append(" and a.ARTICULO = '");
            sentencia.Append(articulo);
            sentencia.Append("' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }

        /// <summary>
        /// Obtener el nombre de las clasificaciones de articulos  
        /// </summary>
        public DataTable getNombreClasificaciones()
        {
            DataTable dt;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CONCAT(NOMBRE_CLASIF_1,':') NOMBRE_CLASIF_1 ");
            sentencia.Append(",CONCAT(NOMBRE_CLASIF_2,':') NOMBRE_CLASIF_2 ");
            sentencia.Append(",CONCAT(NOMBRE_CLASIF_3,':') NOMBRE_CLASIF_3 ");
            sentencia.Append(",CONCAT(NOMBRE_CLASIF_4,':') NOMBRE_CLASIF_4 ");
            sentencia.Append(",CONCAT(NOMBRE_CLASIF_5,':') NOMBRE_CLASIF_5 ");
            sentencia.Append(",CONCAT(NOMBRE_CLASIF_6,':') NOMBRE_CLASIF_6 from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".GLOBALES_CI ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }



    }
}
