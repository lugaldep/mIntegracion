using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Windows.Forms;
using RestSharp;
using Newtonsoft.Json;


namespace mIntegracion.Clases
{
    class Proceso
    {
        private SQL sqlClass;
        private const string ciaPrin = "PRINCIPAL";
        public Proceso(SQL _sqlClass)
        {
            this.sqlClass = _sqlClass;
        }


        /* INDICADOR DE SINCRONIZACION INTERNO PARA PAGOS
         * IND_SINCRO	
            C	CREADO
            P	PAGADO
            E	ERROR
        */


        /* INDICADOR DE PROCESO EN ANDAMIO
         * IND_PROCESO	
            P	PENDIENTE
            S	SINCRONIZADO
            E	ERROR
        */


        public string EstadoCodigo(string estado)
        {
            switch (estado)
            {
                case "Sincronizado":
                    return ("S");
                case "Error":
                    return ("E");
                case "Pendiente":
                    return ("P");

                default:
                    return ("P");
            }
        }
        /*
            Compañías
            Proveedores
            Ordenes de compra
            Solicitudes de pago
            Pagos
        */

        public string TipoCodigoBIT(string tipo)
        {
            switch (tipo)
            {
                case "Compañías":
                    return ("C");
                case "Proveedores":
                    return ("P");
                case "Ordenes de compra":
                    return ("OC");
                case "Solicitudes de pago":
                    return ("SP");
                case "Pagos":
                    return ("PA");
                default:
                    return ("C");
            }
        }

        /// <summary>
        /// Metodo encargado de extraer cias      
        /// </summary>
        public DataTable getIntGlobales()
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select BODEGA, PAIS, DEPARTAMENTO, SERVICE, CONSUMER_KEY, CONSUMER_SECRET ");           
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_GLOBALES ");            

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }



        /// <summary>
        /// Actualizar 
        /// </summary>
        public bool updIntConjunto(SqlTransaction transac, string conjunto, string indProc, string mensaje, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".INT_CONJUNTO ");
                sentencia.Append("SET IND_PROCESO = '" + indProc + "'");
                sentencia.Append(", MENSAJE = '" + mensaje + "'");
                sentencia.Append(" WHERE CONJUNTO = '");
                sentencia.Append(conjunto);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[updIntConjunto]: Se presentaron problemas actualizando el conjunto en la tabla intermedia: ");
                    errores.Append(conjunto);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updIntConjunto]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }



        /// <summary>
        /// Metodo encargado de extraer cias      
        /// </summary>
        public DataTable getCompaniasSync(SqlTransaction transac, string urlMultimedia)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append("NOMBRE nombre ");
            sentencia.Append(",TELEFONO telefono ");
            sentencia.Append(",NIT nit ");
            sentencia.Append(",CONJUNTO codigo ");
            sentencia.Append(",isnull('"+urlMultimedia+"'+LOGO, '" + urlMultimedia + "Logo_PRINCIPAL.BMP') imagnombre  ");
            //sentencia.Append(",LOGO imagnombre  ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_CONJUNTO ");
            sentencia.Append("where IND_PROCESO = 'P' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }


        /// <summary>
        /// Metodo encargado de extraer proveedores      
        /// </summary>
        public DataTable getProveedoresSync(SqlTransaction transac)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append(" i.NOMBRE nombreproveedor ");
            sentencia.Append(",i.CONTRIBUYENTE cedulajuridica ");
            sentencia.Append(",case i.ACTIVO when 'S' then 1 else 0 end activo ");
            sentencia.Append(",cast(p.DIRECCION as varchar(50)) direccion ");
            sentencia.Append(",i.CONTACTO contacto ");
            sentencia.Append(",i.TELEFONO1 telefono ");
            sentencia.Append(",i.E_MAIL correo ");
            sentencia.Append(",i.PROVEEDOR codigoproveedor ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_PROVEEDOR i inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".PROVEEDOR p on i.PROVEEDOR = p.PROVEEDOR ");
            sentencia.Append("where i.IND_PROCESO = 'P' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }



        /// <summary>
        /// Metodo encargado de extraer cias      
        /// </summary>
        public DataTable getDocsCPSync(SqlTransaction transac)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append("CONJUNTO, PROVEEDOR, DOCUMENTO, TIPO ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_DOCS_CP ");
            sentencia.Append("where IND_PROCESO = 'P' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }



        /// <summary>
        /// Metodo encargado de extraer cias      
        /// </summary>
        public DataTable getDocCPSync(SqlTransaction transac, string cia, string prov, string doc, string tipo, string urlDocs)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            //sentencia.Append(" icp.DOCUMENTO numerosolicitud ");
            sentencia.Append("'" + cia + "'+'|'+icp.DOCUMENTO+'|'+icp.PROVEEDOR+'|'+icp.TIPO idsolicitud ");
            sentencia.Append(",'' numerosolicitud ");
            sentencia.Append(",icp.ORDEN_SERVICIO ordenservicio ");
            sentencia.Append(",icp.DOCUMENTO numerodocumento ");
            sentencia.Append(",icp.TIPO tipodocumento ");
            sentencia.Append(",ISNULL(icp.DOCUMENTO_APLIC, icp.DOCUMENTO) facturaplica ");
            sentencia.Append(",icp.FECHA fechafactura ");
            sentencia.Append(",icp.FECHA fecharige ");
            sentencia.Append(",icp.APLICACION descripciondocumento ");
            sentencia.Append(",icp.SUBTOTAL erpmontobruto ");
            sentencia.Append(",icp.IMPUESTO1 erpmontoiva ");
            sentencia.Append(",icp.MONTO erpmontoneto ");
            sentencia.Append(",icp.SUBTOTAL * (ioc.PORC_ADELANTO/100) erpmontonadelanto ");
            sentencia.Append(",icp.SUBTOTAL * (ioc.PORC_RETENCION/100) erpmontonretencion ");
            sentencia.Append(",icp.TIPO_CAMBIO_DOLAR erptipocambio ");
            sentencia.Append(",'" + sqlClass.Usuario + "' usuariosoftland ");
            sentencia.Append(",icp.CP_RowPointer erprowpoint ");
            sentencia.Append(",icp.ESTADO erpestadosolicitudpago ");
            sentencia.Append(",icp.CONJUNTO erpcompania ");
            sentencia.Append(",icp.SUBTIPO_DESC subtipodocumento ");
            sentencia.Append(",'' facturadigitalcontent ");
            sentencia.Append(", '" +  urlDocs + cia + "/PDF/'" + " + icp.CONJUNTO+'_'+icp.TIPO+'_'+icp.PROVEEDOR+'_'+icp.DOCUMENTO+'_'+ FORMAT(icp.FECHA_DOCUMENTO, 'yyyy-MM-dd')+'.pdf' facturadigital ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_DOCS_CP icp left join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_ORDEN_COMPRA ioc on icp.ORDEN_COMPRA = ioc.ORDEN_COMPRA " +
                                                    "and icp.ORDEN_SERVICIO = ioc.ORDEN_SERVICIO " +
                                                    "and icp.CONJUNTO = ioc.CONJUNTO ");
            sentencia.Append("where icp.IND_PROCESO = 'P' ");
            sentencia.Append("and icp.CONJUNTO = '" + cia +"' ");
            sentencia.Append("and icp.PROVEEDOR = '" + prov +"' ");
            sentencia.Append("and icp.DOCUMENTO = '" + doc +"' ");
            sentencia.Append("and icp.TIPO = '" + tipo +"' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }

        /// <summary>
        /// Metodo encargado de extraer cias      
        /// </summary>
        public DataTable getDocCPLineaSync(SqlTransaction transac, string cia, string prov, string doc, string tipo)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");            
            sentencia.Append(" SUBTOTAL lineerpsubtotal ");
            sentencia.Append(",IVA lineerpmontoiva ");
            sentencia.Append(",BRUTO lineerpmontobruto ");
            sentencia.Append(",ORDEN_SERVICIO_LINEA recorddetalleos ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_DOCS_CP_LINEAS ");
            sentencia.Append("where  CONJUNTO = '" + cia + "'");
            sentencia.Append("and PROVEEDOR = '" + prov + "'");
            sentencia.Append("and DOCUMENTO = '" + doc + "'");
            sentencia.Append("and TIPO = '" + tipo + "'");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }



        /// <summary>
        /// Metodo encargado de extraer pagos      
        /// </summary>
        public DataTable getPagosCPSync(SqlTransaction transac)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append("p.CONJUNTO+'|'+p.PROVEEDOR+'|'+p.DOCUMENTO_CRE+'|'+p.TIPO_CRE+'|'+p.DOCUMENTO_DEB+'|'+p.TIPO_DEB idpago ");
            sentencia.Append(",cp.ORDEN_SERVICIO ordenservicio ");
            sentencia.Append(",p.DOCUMENTO_CRE factura ");
            sentencia.Append(",cp.FECHA_DOCUMENTO fechafactura ");
            sentencia.Append(",p.MONEDA moneda ");
            sentencia.Append(",p.MONTO_LOCAL monto ");
            sentencia.Append(",p.MONTO_DOLAR montodolares ");
            sentencia.Append(",p.DOCUMENTO_DEB documentodepago ");
            sentencia.Append(",p.FECHA_DOCUMENTO fechapago ");
            sentencia.Append(",p.MONTO_LOCAL montopagocrc ");
            sentencia.Append(",p.TIPO_CAMBIO_DOLAR tipocambio ");
            sentencia.Append(",'A' estado ");
            sentencia.Append(",p.TIPO_DEB tipopago ");
            sentencia.Append(",p.TIPO_CRE tipodocumentopagar ");
            sentencia.Append(",p.ROWPOINTER rowpointer ");
            sentencia.Append(",p.FECHA_VENCE fechaentregapago ");
            sentencia.Append(",cp.SOLICITUD_PAGO solicitudpago ");
            sentencia.Append(",p.MONTO_DOLAR montopagousd ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_PAGOS_CP p inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_DOCS_CP cp on p.CONJUNTO = cp.CONJUNTO and p.DOCUMENTO_CRE = cp.DOCUMENTO and p.TIPO_CRE = cp.TIPO and p.PROVEEDOR = cp.PROVEEDOR ");
            sentencia.Append("where p.IND_PROCESO = 'P' ");
            sentencia.Append("AND p.TIPO_DEB <> 'N/C' "); //restriccion para que no sync las NC y se pasan estas como solicitudes de Pagos

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }



        /// <summary>
        /// Metodo encargado de extraer pagos      
        /// </summary>
        public DataTable getPagosCPSyncGEN()
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append("p.CONJUNTO+'|'+p.PROVEEDOR+'|'+p.DOCUMENTO_CRE+'|'+p.TIPO_CRE+'|'+p.DOCUMENTO_DEB+'|'+p.TIPO_DEB idpago ");
            sentencia.Append(",cp.ORDEN_SERVICIO ordenservicio ");
            sentencia.Append(",p.DOCUMENTO_CRE factura ");
            sentencia.Append(",cp.FECHA_DOCUMENTO fechafactura ");
            sentencia.Append(",p.MONEDA moneda ");
            sentencia.Append(",p.MONTO_LOCAL monto ");
            sentencia.Append(",p.MONTO_DOLAR montodolares ");
            sentencia.Append(",p.DOCUMENTO_DEB documentodepago ");
            sentencia.Append(",p.FECHA_DOCUMENTO fechapago ");
            sentencia.Append(",p.MONTO_LOCAL montopagocrc ");
            sentencia.Append(",p.TIPO_CAMBIO_DOLAR tipocambio ");
            sentencia.Append(",'A' estado ");
            sentencia.Append(",p.TIPO_DEB tipopago ");
            sentencia.Append(",p.TIPO_CRE tipodocumentopagar ");
            sentencia.Append(",p.ROWPOINTER rowpointer ");
            sentencia.Append(",p.FECHA_VENCE fechaentregapago ");
            sentencia.Append(",cp.SOLICITUD_PAGO solicitudpago ");
            sentencia.Append(",p.MONTO_DOLAR montopagousd ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_PAGOS_CP p inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_DOCS_CP cp on p.CONJUNTO = cp.CONJUNTO and p.DOCUMENTO_CRE = cp.DOCUMENTO and p.TIPO_CRE = cp.TIPO and p.PROVEEDOR = cp.PROVEEDOR ");
            sentencia.Append("where p.TIPO_DEB <> 'N/C' "); //restriccion para que no sync las NC y se pasan estas como solicitudes de Pagos
            sentencia.Append("and p.DOCUMENTO_CRE in (");
            sentencia.Append("'00100001010000007720'");
            sentencia.Append(", '00100001010000007720'");
            sentencia.Append(", '00500001010004389169'");
            sentencia.Append(", '00500001010004389169'");
            sentencia.Append(", '00100001010000002940'");
            sentencia.Append(", '00100001010000002941')");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }



        /// <summary>
        /// Actualizar 
        /// </summary>
        public bool updIntProveedor(SqlTransaction transac, string prov, string indProc, string mensaje, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".INT_PROVEEDOR ");
                sentencia.Append("SET IND_PROCESO = '" + indProc + "'");
                sentencia.Append(", MENSAJE = '" + mensaje + "'");
                sentencia.Append(" WHERE PROVEEDOR = '");
                sentencia.Append(prov);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[updIntProveedor]: Se presentaron problemas actualizando el proveedor en la tabla intermedia: ");
                    errores.Append(prov);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updIntProveedor]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }


        /// <summary>
        /// Actualizar 
        /// </summary>
        public bool updIntDocCP(SqlTransaction transac, string cia, string doc, string prov, string tipo, string indProc, string mensaje, string solPago, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".INT_DOCS_CP ");
                sentencia.Append("SET IND_PROCESO = '" + indProc + "'");
                sentencia.Append(", MENSAJE = '" + mensaje + "'");
                sentencia.Append(", SOLICITUD_PAGO = '" + solPago + "'");
                sentencia.Append(" WHERE CONJUNTO = '" + cia + "' ");
                sentencia.Append("AND DOCUMENTO = '" + doc + "' ");
                sentencia.Append("AND PROVEEDOR = '" + prov + "' ");
                sentencia.Append("AND TIPO = '" + tipo + "' ");

                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[updIntDocCP]: Se presentaron problemas actualizando el doc en la tabla intermedia: ");
                    errores.Append(prov);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updIntDocCP]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }

        /// <summary>
        /// Actualizar 
        /// </summary>
        public bool updIntDocCP(SqlTransaction transac, string cia, string prov, string doc, string tipo, string indSync, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".INT_DOCS_CP ");
                sentencia.Append("SET IND_SINCRO = '" + indSync + "'");
                sentencia.Append(" WHERE CONJUNTO = '" + cia + "' ");
                sentencia.Append("AND DOCUMENTO = '" + doc + "' ");
                sentencia.Append("AND PROVEEDOR = '" + prov + "' ");
                sentencia.Append("AND TIPO = '" + tipo + "' ");

                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[updIntDocCP]: Se presentaron problemas actualizando el doc en la tabla intermedia: ");
                    errores.Append(prov);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updIntDocCP]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }


        /// <summary>
        /// Actualizar 
        /// </summary>
        public bool updIntPagosCP(SqlTransaction transac, string cia, string prov, string docCre, string tipoCre, string docDeb, string tipoDeb, string indProc, string mensaje, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".INT_PAGOS_CP ");
                sentencia.Append("SET IND_PROCESO = '" + indProc + "'");
                sentencia.Append(", MENSAJE = '" + mensaje + "'");                
                sentencia.Append(" WHERE CONJUNTO = '" + cia + "' ");
                sentencia.Append("AND PROVEEDOR = '" + prov + "' ");
                sentencia.Append("AND DOCUMENTO_CRE = '" + docCre + "' ");                
                sentencia.Append("AND TIPO_CRE = '" + tipoCre + "' ");
                sentencia.Append("AND DOCUMENTO_DEB = '" + docDeb + "' ");
                sentencia.Append("AND TIPO_DEB = '" + tipoDeb + "' ");


                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[updIntPagosCP]: Se presentaron problemas actualizando el doc en la tabla intermedia: ");
                    errores.Append(prov);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updIntPagosCP]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }


        /// <summary>
        /// Obtener listado de cotizaciones      
        /// </summary>
        public DataTable getBitacora(string estado, string tipo, DateTime fcInicio, DateTime fcFin)
        {
            string tipoCod = TipoCodigoBIT(tipo);
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            //cia
            if(tipoCod.CompareTo("C")==0)
                sentencia.Append("CONJUNTO Compañía,NOMBRE Nombre, ");
            else if (tipoCod.CompareTo("P") == 0)
                //prov
                sentencia.Append("PROVEEDOR Proveedor, NOMBRE Nombre, ");
            else if (tipoCod.CompareTo("OC") == 0)
                //oc
                sentencia.Append("CONJUNTO Compañía, ORDEN_COMPRA Orden_Compra, ORDEN_SERVICIO Orden_Servicio, PROVEEDOR Proveedor, ");
            else if (tipoCod.CompareTo("SP") == 0)
                //solicitud pagos
                sentencia.Append("CONJUNTO Compañía, DOCUMENTO Documento, TIPO Tipo, PROVEEDOR Proveedor, SOLICITUD_PAGO Solicitud_Pago, ");
            else if (tipoCod.CompareTo("PA") == 0)
                //pagos
                sentencia.Append("CONJUNTO Compañía, PROVEEDOR Proveedor, DOCUMENTO_CRE Credito, TIPO_CRE Tipo_Cre, DOCUMENTO_DEB Debito, TIPO_DEB Tipo_Deb, ");

            sentencia.Append("FCH_PROCESO Fecha, ");
            sentencia.Append("case IND_PROCESO when 'P' then 'Pendiente' ");
            sentencia.Append("when 'E' then 'Error' when 'S' then 'Sincronizado' ");
            sentencia.Append("end Estado, ");
            sentencia.Append("MENSAJE Mensaje ");            
            sentencia.Append("from ");

            sentencia.Append(sqlClass.Compannia);
           
            if (tipoCod.CompareTo("C") == 0)
                //cia
                sentencia.Append(".INT_CONJUNTO ");
            else if (tipoCod.CompareTo("P") == 0)
                //prov
                sentencia.Append(".INT_PROVEEDOR ");
            else if (tipoCod.CompareTo("OC") == 0)
                //oc
                sentencia.Append(".INT_ORDEN_COMPRA ");
            else if (tipoCod.CompareTo("SP") == 0)
                //solicitud pagos
                sentencia.Append(".INT_DOCS_CP ");          
            else if (tipoCod.CompareTo("PA") == 0)
                //pagos   
                sentencia.Append(".INT_PAGOS_CP ");

            sentencia.Append("where FCH_PROCESO is not null");

            if (estado.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and IND_PROCESO = '");
                sentencia.Append(EstadoCodigo(estado));
                sentencia.Append("' ");
            }

            //if (tipo.CompareTo(string.Empty) != 0)
            //{
            //    sentencia.Append(" and TIPO = '");
            //    sentencia.Append(tipoCod);
            //    sentencia.Append("' ");
            //}

            if ((fcInicio.ToString("yyyy").CompareTo("0001") != 0) && (fcFin.ToString("yyyy").CompareTo("0001") != 0))
            {
                sentencia.Append(" and FCH_PROCESO between '");
                sentencia.Append(fcInicio.ToString("yyyy-MM-dd"));
                sentencia.Append("' and '");
                sentencia.Append(fcFin.ToString("yyyy-MM-dd"));
                sentencia.Append("' ");
            }

            sentencia.Append(" order by FCH_PROCESO desc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }








        #region SolicitudPagos

        /*SOLICITUD DE PAGOS*/

        /// <summary>
        /// --Obtener las cias en el andamio      
        /// </summary>
        public DataTable getIntCompanias(SqlTransaction transac)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select distinct CONJUNTO ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_ORDEN_COMPRA ");
            sentencia.Append("where ORDEN_COMPRA is not null ");

            //sentencia.Append("and CONJUNTO = 'ROCALLA' ");


            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }


        /// <summary>
        /// --Obtener las ordenes de compra vigentes  
        /// </summary>
        public DataTable getOCActivas(SqlTransaction transac, string cia, string filtroEstadoOC)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ioc.ORDEN_COMPRA, ioc.ORDEN_SERVICIO ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_ORDEN_COMPRA ioc inner join ");
            sentencia.Append(cia);
            sentencia.Append(".ORDEN_COMPRA oc on ioc.ORDEN_COMPRA = oc.ORDEN_COMPRA ");
            sentencia.Append("where oc.ESTADO in " + filtroEstadoOC);
            sentencia.Append(" and ioc.CONJUNTO = '" + cia + "'");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }



        /// <summary>
        /// Metodo encargado de insertar cia | bod
        /// </summary>
        public bool insIntDocsCP(SqlTransaction transac, string cia, string oc, string ocServicio, DataRow doc, string indSincro, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".INT_DOCS_CP ");
                sentencia.Append("(CONJUNTO,PROVEEDOR,DOCUMENTO,TIPO,FECHA_DOCUMENTO,FECHA,APLICACION,MONTO,SALDO,SALDO_LOCAL,MONTO_DOLAR,SALDO_DOLAR,TIPO_CAMBIO_MONEDA");
                sentencia.Append(",TIPO_CAMBIO_DOLAR,APROBADO,SELECCIONADO,CONGELADO,MONTO_PROV,SALDO_PROV,TIPO_CAMBIO_PROV,SUBTOTAL,DESCUENTO,IMPUESTO1,IMPUESTO2,FECHA_ULT_MOD");
                sentencia.Append(",USUARIO_ULT_MOD,CONDICION_PAGO,MONEDA,SUBTIPO,FECHA_VENCE,FECHA_ANUL,AUD_USUARIO_ANUL,AUD_FECHA_ANUL,USUARIO_APROBACION,FECHA_APROBACION");
                sentencia.Append(",ANULADO,FECHA_PROYECTADA,DOCUMENTO_FISCAL,ESTADO,NOTAS,IND_PROCESO,FCH_PROCESO,MONTO_LOCAL,ORDEN_COMPRA,EMBARQUE,IND_SINCRO,SUBTIPO_DESC,ORDEN_SERVICIO");
                sentencia.Append(",CP_ROWPOINTER");
                sentencia.Append(") VALUES (");
                sentencia.Append("@CONJUNTO,@PROVEEDOR,@DOCUMENTO,@TIPO,@FECHA_DOCUMENTO,@FECHA,@APLICACION,@MONTO,@SALDO,@SALDO_LOCAL,@MONTO_DOLAR,@SALDO_DOLAR,@TIPO_CAMBIO_MONEDA");
                sentencia.Append(",@TIPO_CAMBIO_DOLAR,@APROBADO,@SELECCIONADO,@CONGELADO,@MONTO_PROV,@SALDO_PROV,@TIPO_CAMBIO_PROV,@SUBTOTAL,@DESCUENTO,@IMPUESTO1,@IMPUESTO2,@FECHA_ULT_MOD");
                sentencia.Append(",@USUARIO_ULT_MOD,@CONDICION_PAGO,@MONEDA,@SUBTIPO,@FECHA_VENCE,@FECHA_ANUL,@AUD_USUARIO_ANUL,@AUD_FECHA_ANUL,@USUARIO_APROBACION,@FECHA_APROBACION");
                sentencia.Append(",@ANULADO,@FECHA_PROYECTADA,@DOCUMENTO_FISCAL,@ESTADO,@NOTAS,@IND_PROCESO,@FCH_PROCESO,@MONTO_LOCAL,@ORDEN_COMPRA,@EMBARQUE,@IND_SINCRO,@SUBTIPO_DESC,@ORDEN_SERVICIO");
                sentencia.Append(",@CP_ROWPOINTER)");
                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros  
                cmd.Parameters.Add("@CONJUNTO          ", SqlDbType.VarChar, 10).Value = cia;
                cmd.Parameters.Add("@PROVEEDOR         ", SqlDbType.VarChar, 20).Value = doc["PROVEEDOR"];
                cmd.Parameters.Add("@DOCUMENTO         ", SqlDbType.VarChar, 50).Value = doc["DOCUMENTO"];
                cmd.Parameters.Add("@TIPO              ", SqlDbType.VarChar, 3).Value = doc["TIPO"];
                cmd.Parameters.Add("@FECHA_DOCUMENTO   ", SqlDbType.DateTime).Value = doc["FECHA_DOCUMENTO"];
                cmd.Parameters.Add("@FECHA             ", SqlDbType.DateTime).Value = doc["FECHA"];
                cmd.Parameters.Add("@APLICACION        ", SqlDbType.VarChar, 249).Value = doc["APLICACION"];
                cmd.Parameters.Add("@MONTO             ", SqlDbType.Decimal).Value = doc["MONTO"];
                cmd.Parameters.Add("@SALDO             ", SqlDbType.Decimal).Value = doc["SALDO"];
                cmd.Parameters.Add("@SALDO_LOCAL       ", SqlDbType.Decimal).Value = doc["SALDO_LOCAL"];
                cmd.Parameters.Add("@MONTO_DOLAR       ", SqlDbType.Decimal).Value = doc["MONTO_DOLAR"];
                cmd.Parameters.Add("@SALDO_DOLAR       ", SqlDbType.Decimal).Value = doc["SALDO_DOLAR"];
                cmd.Parameters.Add("@TIPO_CAMBIO_MONEDA", SqlDbType.Decimal).Value = doc["TIPO_CAMBIO_MONEDA"];
                cmd.Parameters.Add("@TIPO_CAMBIO_DOLAR ", SqlDbType.Decimal).Value = doc["TIPO_CAMBIO_DOLAR"];
                cmd.Parameters.Add("@APROBADO          ", SqlDbType.VarChar, 1).Value = doc["APROBADO"];
                cmd.Parameters.Add("@SELECCIONADO      ", SqlDbType.VarChar, 1).Value = doc["SELECCIONADO"];
                cmd.Parameters.Add("@CONGELADO         ", SqlDbType.VarChar, 1).Value = doc["CONGELADO"];
                cmd.Parameters.Add("@MONTO_PROV        ", SqlDbType.Decimal).Value = doc["MONTO_PROV"];
                cmd.Parameters.Add("@SALDO_PROV        ", SqlDbType.Decimal).Value = doc["SALDO_PROV"];
                cmd.Parameters.Add("@TIPO_CAMBIO_PROV  ", SqlDbType.Decimal).Value = doc["TIPO_CAMBIO_PROV"];
                cmd.Parameters.Add("@SUBTOTAL          ", SqlDbType.Decimal).Value = doc["SUBTOTAL"];
                cmd.Parameters.Add("@DESCUENTO         ", SqlDbType.Decimal).Value = doc["DESCUENTO"];
                cmd.Parameters.Add("@IMPUESTO1         ", SqlDbType.Decimal).Value = doc["IMPUESTO1"];
                cmd.Parameters.Add("@IMPUESTO2         ", SqlDbType.Decimal).Value = doc["IMPUESTO2"];
                cmd.Parameters.Add("@FECHA_ULT_MOD     ", SqlDbType.DateTime).Value = doc["FECHA_ULT_MOD"];
                cmd.Parameters.Add("@USUARIO_ULT_MOD   ", SqlDbType.VarChar, 25).Value = doc["USUARIO_ULT_MOD"];
                cmd.Parameters.Add("@CONDICION_PAGO    ", SqlDbType.VarChar, 4).Value = doc["CONDICION_PAGO"];
                cmd.Parameters.Add("@MONEDA            ", SqlDbType.VarChar, 4).Value = doc["MONEDA"];
                cmd.Parameters.Add("@SUBTIPO           ", SqlDbType.VarChar, 50).Value = doc["SUBTIPO"];
                cmd.Parameters.Add("SUBTIPO_DESC       ", SqlDbType.VarChar, 25).Value = doc["SUBTIPO_DESC"];
                cmd.Parameters.Add("@FECHA_VENCE       ", SqlDbType.DateTime, 50).Value = doc["FECHA_VENCE"];
                cmd.Parameters.Add("@FECHA_ANUL        ", SqlDbType.DateTime, 50).Value = doc["FECHA_ANUL"];
                cmd.Parameters.Add("@AUD_USUARIO_ANUL  ", SqlDbType.VarChar, 25).Value = doc["AUD_USUARIO_ANUL"];
                cmd.Parameters.Add("@AUD_FECHA_ANUL    ", SqlDbType.DateTime, 50).Value = doc["AUD_FECHA_ANUL"];
                cmd.Parameters.Add("@USUARIO_APROBACION", SqlDbType.VarChar, 25).Value = doc["USUARIO_APROBACION"];
                cmd.Parameters.Add("@FECHA_APROBACION  ", SqlDbType.DateTime).Value = doc["FECHA_APROBACION"];
                cmd.Parameters.Add("@ANULADO           ", SqlDbType.VarChar, 1).Value = doc["ANULADO"]; 
                cmd.Parameters.Add("@FECHA_PROYECTADA  ", SqlDbType.DateTime).Value = doc["FECHA_PROYECTADA"];
                cmd.Parameters.Add("@DOCUMENTO_FISCAL  ", SqlDbType.VarChar, 50).Value = doc["DOCUMENTO_FISCAL"];
                cmd.Parameters.Add("@ESTADO            ", SqlDbType.VarChar, 1).Value = doc["ESTADO"];
                cmd.Parameters.Add("@NOTAS             ", SqlDbType.VarChar, 2500).Value = doc["NOTAS"];
                cmd.Parameters.Add("@IND_PROCESO       ", SqlDbType.VarChar, 1).Value = "P" ;
                cmd.Parameters.Add("@FCH_PROCESO       ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd");                
                cmd.Parameters.Add("@MONTO_LOCAL       ", SqlDbType.Decimal).Value = doc["MONTO_LOCAL"];               
                cmd.Parameters.Add("@ORDEN_COMPRA      ", SqlDbType.VarChar, 10).Value = oc;
                cmd.Parameters.Add("@ORDEN_SERVICIO      ", SqlDbType.BigInt).Value = ocServicio;
                cmd.Parameters.Add("@EMBARQUE          ", SqlDbType.VarChar, 10).Value = doc["EMBARQUE"];
                cmd.Parameters.Add("@IND_SINCRO        ", SqlDbType.VarChar, 1).Value = indSincro ;
                cmd.Parameters.Add("@CP_ROWPOINTER     ", SqlDbType.VarChar, 100).Value = Convert.ToString(doc["RowPointer"]);

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insIntDocsCP]: Se presentaron problemas insertando INT_DOCS_CP: ");
                    errores.AppendLine(oc);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insIntDocsCP]: Detalle Técnico: " + e.Message);
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
        public bool insIntDocsCP_Aplic(SqlTransaction transac, string cia, string oc, string ocServicio, DataRow doc, string indSincro, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".INT_DOCS_CP ");
                sentencia.Append("(CONJUNTO,PROVEEDOR,DOCUMENTO,TIPO,FECHA_DOCUMENTO,FECHA,APLICACION,MONTO,SALDO,SALDO_LOCAL,MONTO_DOLAR,SALDO_DOLAR,TIPO_CAMBIO_MONEDA");
                sentencia.Append(",TIPO_CAMBIO_DOLAR,APROBADO,SELECCIONADO,CONGELADO,MONTO_PROV,SALDO_PROV,TIPO_CAMBIO_PROV,SUBTOTAL,DESCUENTO,IMPUESTO1,IMPUESTO2,FECHA_ULT_MOD");
                sentencia.Append(",USUARIO_ULT_MOD,CONDICION_PAGO,MONEDA,SUBTIPO,FECHA_VENCE,FECHA_ANUL,AUD_USUARIO_ANUL,AUD_FECHA_ANUL,USUARIO_APROBACION,FECHA_APROBACION");
                sentencia.Append(",ANULADO,FECHA_PROYECTADA,DOCUMENTO_FISCAL,ESTADO,NOTAS,IND_PROCESO,FCH_PROCESO,MONTO_LOCAL,ORDEN_COMPRA,EMBARQUE,IND_SINCRO,SUBTIPO_DESC,ORDEN_SERVICIO");
                sentencia.Append(",CP_ROWPOINTER,DOCUMENTO_APLIC,TIPO_APLIC");
                sentencia.Append(") VALUES (");
                sentencia.Append("@CONJUNTO,@PROVEEDOR,@DOCUMENTO,@TIPO,@FECHA_DOCUMENTO,@FECHA,@APLICACION,@MONTO,@SALDO,@SALDO_LOCAL,@MONTO_DOLAR,@SALDO_DOLAR,@TIPO_CAMBIO_MONEDA");
                sentencia.Append(",@TIPO_CAMBIO_DOLAR,@APROBADO,@SELECCIONADO,@CONGELADO,@MONTO_PROV,@SALDO_PROV,@TIPO_CAMBIO_PROV,@SUBTOTAL,@DESCUENTO,@IMPUESTO1,@IMPUESTO2,@FECHA_ULT_MOD");
                sentencia.Append(",@USUARIO_ULT_MOD,@CONDICION_PAGO,@MONEDA,@SUBTIPO,@FECHA_VENCE,@FECHA_ANUL,@AUD_USUARIO_ANUL,@AUD_FECHA_ANUL,@USUARIO_APROBACION,@FECHA_APROBACION");
                sentencia.Append(",@ANULADO,@FECHA_PROYECTADA,@DOCUMENTO_FISCAL,@ESTADO,@NOTAS,@IND_PROCESO,@FCH_PROCESO,@MONTO_LOCAL,@ORDEN_COMPRA,@EMBARQUE,@IND_SINCRO,@SUBTIPO_DESC,@ORDEN_SERVICIO");
                sentencia.Append(",@CP_ROWPOINTER,@DOCUMENTO_APLIC,@TIPO_APLIC)");
                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros  
                cmd.Parameters.Add("@CONJUNTO          ", SqlDbType.VarChar, 10).Value = cia;
                cmd.Parameters.Add("@PROVEEDOR         ", SqlDbType.VarChar, 20).Value = doc["PROVEEDOR"];
                cmd.Parameters.Add("@DOCUMENTO         ", SqlDbType.VarChar, 50).Value = doc["DOCUMENTO"];
                cmd.Parameters.Add("@TIPO              ", SqlDbType.VarChar, 3).Value = doc["TIPO"];
                cmd.Parameters.Add("@FECHA_DOCUMENTO   ", SqlDbType.DateTime).Value = doc["FECHA_DOCUMENTO"];
                cmd.Parameters.Add("@FECHA             ", SqlDbType.DateTime).Value = doc["FECHA"];
                cmd.Parameters.Add("@APLICACION        ", SqlDbType.VarChar, 249).Value = doc["APLICACION"];
                cmd.Parameters.Add("@MONTO             ", SqlDbType.Decimal).Value = doc["MONTO"];
                cmd.Parameters.Add("@SALDO             ", SqlDbType.Decimal).Value = doc["SALDO"];
                cmd.Parameters.Add("@SALDO_LOCAL       ", SqlDbType.Decimal).Value = doc["SALDO_LOCAL"];
                cmd.Parameters.Add("@MONTO_DOLAR       ", SqlDbType.Decimal).Value = doc["MONTO_DOLAR"];
                cmd.Parameters.Add("@SALDO_DOLAR       ", SqlDbType.Decimal).Value = doc["SALDO_DOLAR"];
                cmd.Parameters.Add("@TIPO_CAMBIO_MONEDA", SqlDbType.Decimal).Value = doc["TIPO_CAMBIO_MONEDA"];
                cmd.Parameters.Add("@TIPO_CAMBIO_DOLAR ", SqlDbType.Decimal).Value = doc["TIPO_CAMBIO_DOLAR"];
                cmd.Parameters.Add("@APROBADO          ", SqlDbType.VarChar, 1).Value = doc["APROBADO"];
                cmd.Parameters.Add("@SELECCIONADO      ", SqlDbType.VarChar, 1).Value = doc["SELECCIONADO"];
                cmd.Parameters.Add("@CONGELADO         ", SqlDbType.VarChar, 1).Value = doc["CONGELADO"];
                cmd.Parameters.Add("@MONTO_PROV        ", SqlDbType.Decimal).Value = doc["MONTO_PROV"];
                cmd.Parameters.Add("@SALDO_PROV        ", SqlDbType.Decimal).Value = doc["SALDO_PROV"];
                cmd.Parameters.Add("@TIPO_CAMBIO_PROV  ", SqlDbType.Decimal).Value = doc["TIPO_CAMBIO_PROV"];
                cmd.Parameters.Add("@SUBTOTAL          ", SqlDbType.Decimal).Value = doc["SUBTOTAL"];
                cmd.Parameters.Add("@DESCUENTO         ", SqlDbType.Decimal).Value = doc["DESCUENTO"];
                cmd.Parameters.Add("@IMPUESTO1         ", SqlDbType.Decimal).Value = doc["IMPUESTO1"];
                cmd.Parameters.Add("@IMPUESTO2         ", SqlDbType.Decimal).Value = doc["IMPUESTO2"];
                cmd.Parameters.Add("@FECHA_ULT_MOD     ", SqlDbType.DateTime).Value = doc["FECHA_ULT_MOD"];
                cmd.Parameters.Add("@USUARIO_ULT_MOD   ", SqlDbType.VarChar, 25).Value = doc["USUARIO_ULT_MOD"];
                cmd.Parameters.Add("@CONDICION_PAGO    ", SqlDbType.VarChar, 4).Value = doc["CONDICION_PAGO"];
                cmd.Parameters.Add("@MONEDA            ", SqlDbType.VarChar, 4).Value = doc["MONEDA"];
                cmd.Parameters.Add("@SUBTIPO           ", SqlDbType.VarChar, 50).Value = doc["SUBTIPO"];
                cmd.Parameters.Add("SUBTIPO_DESC       ", SqlDbType.VarChar, 25).Value = doc["SUBTIPO_DESC"];
                cmd.Parameters.Add("@FECHA_VENCE       ", SqlDbType.DateTime, 50).Value = doc["FECHA_VENCE"];
                cmd.Parameters.Add("@FECHA_ANUL        ", SqlDbType.DateTime, 50).Value = doc["FECHA_ANUL"];
                cmd.Parameters.Add("@AUD_USUARIO_ANUL  ", SqlDbType.VarChar, 25).Value = doc["AUD_USUARIO_ANUL"];
                cmd.Parameters.Add("@AUD_FECHA_ANUL    ", SqlDbType.DateTime, 50).Value = doc["AUD_FECHA_ANUL"];
                cmd.Parameters.Add("@USUARIO_APROBACION", SqlDbType.VarChar, 25).Value = doc["USUARIO_APROBACION"];
                cmd.Parameters.Add("@FECHA_APROBACION  ", SqlDbType.DateTime).Value = doc["FECHA_APROBACION"];
                cmd.Parameters.Add("@ANULADO           ", SqlDbType.VarChar, 1).Value = doc["ANULADO"];
                cmd.Parameters.Add("@FECHA_PROYECTADA  ", SqlDbType.DateTime).Value = doc["FECHA_PROYECTADA"];
                cmd.Parameters.Add("@DOCUMENTO_FISCAL  ", SqlDbType.VarChar, 50).Value = doc["DOCUMENTO_FISCAL"];
                cmd.Parameters.Add("@ESTADO            ", SqlDbType.VarChar, 1).Value = doc["ESTADO"];
                cmd.Parameters.Add("@NOTAS             ", SqlDbType.VarChar, 2500).Value = doc["NOTAS"];
                cmd.Parameters.Add("@IND_PROCESO       ", SqlDbType.VarChar, 1).Value = "P";
                cmd.Parameters.Add("@FCH_PROCESO       ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@MONTO_LOCAL       ", SqlDbType.Decimal).Value = doc["MONTO_LOCAL"];
                cmd.Parameters.Add("@ORDEN_COMPRA      ", SqlDbType.VarChar, 10).Value = oc;
                cmd.Parameters.Add("@ORDEN_SERVICIO      ", SqlDbType.BigInt).Value = ocServicio;
                cmd.Parameters.Add("@EMBARQUE          ", SqlDbType.VarChar, 10).Value = doc["EMBARQUE"];
                cmd.Parameters.Add("@IND_SINCRO        ", SqlDbType.VarChar, 1).Value = indSincro;
                cmd.Parameters.Add("@CP_ROWPOINTER     ", SqlDbType.VarChar, 100).Value = Convert.ToString(doc["RowPointer"]);
                cmd.Parameters.Add("@DOCUMENTO_APLIC ", SqlDbType.VarChar, 50).Value = doc["DOCUMENTO_APLIC"];
                cmd.Parameters.Add("@TIPO_APLIC     ", SqlDbType.VarChar, 3).Value = doc["TIPO_APLIC"];

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insIntDocsCP]: Se presentaron problemas insertando INT_DOCS_CP: ");
                    errores.AppendLine(oc);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insIntDocsCP]: Detalle Técnico: " + e.Message);
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
        public bool insIntDocsCPLineas(SqlTransaction transac, string cia, string oc, string ocServicio, DataRow doc, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".INT_DOCS_CP_LINEAS ");
                sentencia.Append("(CONJUNTO,PROVEEDOR,DOCUMENTO,TIPO,ORDEN_SERVICIO,ORDEN_COMPRA,ORDEN_COMPRA_LINEA,ORDEN_SERVICIO_LINEA,SUBTOTAL,IVA,BRUTO");
                sentencia.Append(") VALUES (");
                sentencia.Append("@CONJUNTO,@PROVEEDOR,@DOCUMENTO,@TIPO,@ORDEN_SERVICIO,@ORDEN_COMPRA,@ORDEN_COMPRA_LINEA,@ORDEN_SERVICIO_LINEA,@SUBTOTAL,@IVA,@BRUTO)");
                
                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros  
                cmd.Parameters.Add("@CONJUNTO          ", SqlDbType.VarChar, 10).Value = cia;
                cmd.Parameters.Add("@PROVEEDOR         ", SqlDbType.VarChar, 20).Value = doc["PROVEEDOR"];
                cmd.Parameters.Add("@DOCUMENTO         ", SqlDbType.VarChar, 50).Value = doc["DOCUMENTO"];
                cmd.Parameters.Add("@TIPO              ", SqlDbType.VarChar, 3).Value = doc["TIPO"]; 
                cmd.Parameters.Add("@ORDEN_COMPRA      ", SqlDbType.VarChar, 10).Value = oc;
                cmd.Parameters.Add("@ORDEN_COMPRA_LINEA  ", SqlDbType.SmallInt).Value = doc["ORDEN_COMPRA_LINEA"];
                cmd.Parameters.Add("@ORDEN_SERVICIO      ", SqlDbType.BigInt).Value = ocServicio;
                cmd.Parameters.Add("@ORDEN_SERVICIO_LINEA", SqlDbType.BigInt).Value = doc["ORDEN_SERVICIO_LINEA"]; ;
                cmd.Parameters.Add("@SUBTOTAL            ", SqlDbType.Decimal).Value = doc["SUBTOTAL_L"];
                cmd.Parameters.Add("@IVA                 ", SqlDbType.Decimal).Value = doc["IMPUESTO1_L"];
                cmd.Parameters.Add("@BRUTO               ", SqlDbType.Decimal).Value = doc["TOTAL_L"];

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insIntDocsCPLineas]: Se presentaron problemas insertando INT_DOCS_CP_LINEAS: ");
                    errores.AppendLine(oc);
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insIntDocsCPLineas]: Detalle Técnico: " + e.Message);
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
        /// --Obtener las ordenes de compra vigentes  
        /// </summary>
        public DataTable getAnticipoDocs(SqlTransaction transac, string cia, string oc, string tipo, string subtipo)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select  cp.PROVEEDOR ");
            sentencia.Append(",cp.DOCUMENTO  ");
            sentencia.Append(",cp.TIPO  ");
            sentencia.Append(",cp.FECHA_DOCUMENTO  ");
            sentencia.Append(",cp.FECHA  ");
            sentencia.Append(",cp.APLICACION ");
            sentencia.Append(",cp.MONTO  ");
            sentencia.Append(",cp.SALDO  ");
            sentencia.Append(",cp.SALDO_LOCAL  ");
            sentencia.Append(",cp.MONTO_DOLAR  ");
            sentencia.Append(",cp.SALDO_DOLAR  ");
            sentencia.Append(",cp.TIPO_CAMBIO_MONEDA  ");
            sentencia.Append(",cp.TIPO_CAMBIO_DOLAR  ");
            sentencia.Append(",cp.APROBADO  ");
            sentencia.Append(",cp.SELECCIONADO  ");
            sentencia.Append(",cp.CONGELADO  ");
            sentencia.Append(",cp.MONTO_PROV  ");
            sentencia.Append(",cp.SALDO_PROV  ");
            sentencia.Append(",cp.TIPO_CAMBIO_PROV  ");
            sentencia.Append(",cp.SUBTOTAL  ");
            sentencia.Append(",cp.DESCUENTO  ");
            sentencia.Append(",cp.IMPUESTO1  ");
            sentencia.Append(",cp.IMPUESTO2  ");
            sentencia.Append(",cp.FECHA_ULT_MOD  ");
            sentencia.Append(",cp.USUARIO_ULT_MOD  ");
            sentencia.Append(",cp.CONDICION_PAGO  ");
            sentencia.Append(",cp.MONEDA  ");
            sentencia.Append(",cp.SUBTIPO  ");
            sentencia.Append(",cp.FECHA_VENCE  ");
            sentencia.Append(",cp.FECHA_ANUL  ");
            sentencia.Append(",cp.AUD_USUARIO_ANUL  ");
            sentencia.Append(",cp.AUD_FECHA_ANUL  ");
            sentencia.Append(",cp.USUARIO_APROBACION  ");
            sentencia.Append(",cp.FECHA_APROBACION  ");
            sentencia.Append(",cp.ANULADO  ");
            sentencia.Append(",cp.FECHA_PROYECTADA  ");
            sentencia.Append(",cp.DOCUMENTO_FISCAL  ");
            sentencia.Append(",cp.ESTADO  ");
            sentencia.Append(",cp.NOTAS  ");
            sentencia.Append(",cp.MONTO_LOCAL  ");
            sentencia.Append(",'' EMBARQUE  ");
            sentencia.Append(",sdcp.DESCRIPCION SUBTIPO_DESC ");
            sentencia.Append(",cp.RowPointer  ");
            sentencia.Append(" from  ");
            sentencia.Append(cia + ".DOCUMENTOS_CP cp  ");            
            sentencia.Append("inner join " + cia + ".SUBTIPO_DOC_CP sdcp on cp.SUBTIPO = sdcp.SUBTIPO ");
            sentencia.Append("where cp.TIPO = '" + tipo + "' ");
            sentencia.Append("and cp.SUBTIPO in " + subtipo + " ");
            sentencia.Append("and cp.U_ORDEN_COMPRA = '" + oc + "' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }




        /// <summary>
        /// --Obtener las ordenes de compra vigentes  
        /// </summary>
        public DataTable getNotasCredito(SqlTransaction transac, string cia, string tipo)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select  pcp.PROVEEDOR ");
            sentencia.Append(",pcp.DOCUMENTO_DEB DOCUMENTO  ");
            sentencia.Append(",pcp.TIPO_DEB TIPO  ");
            sentencia.Append(",pcp.DOCUMENTO_CRE DOCUMENTO_APLIC  ");
            sentencia.Append(",pcp.TIPO_CRE TIPO_APLIC  ");
            sentencia.Append(",cp.FECHA_DOCUMENTO  ");
            sentencia.Append(",cp.FECHA  ");
            sentencia.Append(",cp.APLICACION ");
            sentencia.Append(",cp.MONTO  ");
            sentencia.Append(",cp.SALDO  ");
            sentencia.Append(",cp.SALDO_LOCAL  ");
            sentencia.Append(",cp.MONTO_DOLAR  ");
            sentencia.Append(",cp.SALDO_DOLAR  ");
            sentencia.Append(",cp.TIPO_CAMBIO_MONEDA  ");
            sentencia.Append(",cp.TIPO_CAMBIO_DOLAR  ");
            sentencia.Append(",cp.APROBADO  ");
            sentencia.Append(",cp.SELECCIONADO  ");
            sentencia.Append(",cp.CONGELADO  ");
            sentencia.Append(",cp.MONTO_PROV  ");
            sentencia.Append(",cp.SALDO_PROV  ");
            sentencia.Append(",cp.TIPO_CAMBIO_PROV  ");
            sentencia.Append(",cp.SUBTOTAL  ");
            sentencia.Append(",cp.DESCUENTO  ");
            sentencia.Append(",cp.IMPUESTO1  ");
            sentencia.Append(",cp.IMPUESTO2  ");
            sentencia.Append(",cp.FECHA_ULT_MOD  ");
            sentencia.Append(",cp.USUARIO_ULT_MOD  ");
            sentencia.Append(",cp.CONDICION_PAGO  ");
            sentencia.Append(",cp.MONEDA  ");
            sentencia.Append(",cp.SUBTIPO  ");
            sentencia.Append(",cp.FECHA_VENCE  ");
            sentencia.Append(",cp.FECHA_ANUL  ");
            sentencia.Append(",cp.AUD_USUARIO_ANUL  ");
            sentencia.Append(",cp.AUD_FECHA_ANUL  ");
            sentencia.Append(",cp.USUARIO_APROBACION  ");
            sentencia.Append(",cp.FECHA_APROBACION  ");
            sentencia.Append(",cp.ANULADO  ");
            sentencia.Append(",cp.FECHA_PROYECTADA  ");
            sentencia.Append(",cp.DOCUMENTO_FISCAL  ");
            sentencia.Append(",cp.ESTADO  ");
            sentencia.Append(",cp.NOTAS  ");
            sentencia.Append(",cp.MONTO_LOCAL  ");
            sentencia.Append(",icp.ORDEN_COMPRA, icp.ORDEN_SERVICIO  ");
            sentencia.Append(",icp.EMBARQUE  ");
            sentencia.Append(",sdcp.DESCRIPCION SUBTIPO_DESC ");
            sentencia.Append(",icpl.ORDEN_COMPRA_LINEA ");
            sentencia.Append(",icpl.ORDEN_SERVICIO_LINEA ");
            sentencia.Append(",cp.RowPointer  ");
            sentencia.Append(",icpl.SUBTOTAL SUBTOTAL_L ");
            sentencia.Append(",icpl.IVA IMPUESTO1_L ");
            sentencia.Append(",icpl.BRUTO TOTAL_L   ");
            sentencia.Append(" from  ");
            sentencia.Append(sqlClass.Compannia + ".INT_PAGOS_CP pcp ");
            sentencia.Append("inner join " + sqlClass.Compannia  + ".INT_DOCS_CP icp on pcp.PROVEEDOR = icp.PROVEEDOR ");
            sentencia.Append("								and pcp.DOCUMENTO_CRE = icp.DOCUMENTO ");
            sentencia.Append("								and pcp.TIPO_CRE = icp.TIPO	");
            sentencia.Append("inner join " + sqlClass.Compannia + ".INT_DOCS_CP_LINEAS icpl on icp.PROVEEDOR = icpl.PROVEEDOR ");
            sentencia.Append("								and icp.DOCUMENTO = icpl.DOCUMENTO");
            sentencia.Append("								and icp.TIPO = icpl.TIPO ");
            sentencia.Append("inner join " + cia + ".DOCUMENTOS_CP cp on pcp.PROVEEDOR = cp.PROVEEDOR ");
            sentencia.Append("								and pcp.DOCUMENTO_DEB = cp.DOCUMENTO ");
            sentencia.Append("								and pcp.TIPO_DEB = cp.TIPO	");
            sentencia.Append("inner join " + cia + ".SUBTIPO_DOC_CP sdcp on cp.SUBTIPO = sdcp.SUBTIPO ");
            sentencia.Append("WHERE pcp.TIPO_DEB = '" + tipo + "' ");
            //sentencia.Append("and icp.IND_SINCRO = 'C'"); //PAGOS SIN SINCRONIZAR
            sentencia.Append("and pcp.IND_PROCESO = 'P' "); //PAGOS SIN SINCRONIZAR

            sentencia.Append(" order by pcp.PROVEEDOR, pcp.DOCUMENTO_DEB, pcp.TIPO_DEB asc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }


        /*METODO SIN AGRUPACION*/

        ///// <summary>
        ///// --Obtener las ordenes de compra vigentes  
        ///// </summary>
        //public DataTable getSolPagosDocs(SqlTransaction transac, string cia, string oc)
        //{
        //    DataTable dt = null;
        //    StringBuilder sentencia = new StringBuilder();

        //    sentencia.Append("select ");
        //    sentencia.Append(" el.PROVEEDOR ");
        //    sentencia.Append(",el.DOCUMENTO ");
        //    sentencia.Append(",cp.TIPO ");
        //    sentencia.Append(",cp.FECHA_DOCUMENTO ");
        //    sentencia.Append(",cp.FECHA ");
        //    sentencia.Append(",cp.APLICACION ");
        //    sentencia.Append(",cp.MONTO ");
        //    sentencia.Append(",cp.SALDO ");
        //    sentencia.Append(",cp.SALDO_LOCAL ");
        //    sentencia.Append(",cp.MONTO_DOLAR ");
        //    sentencia.Append(",cp.SALDO_DOLAR ");
        //    sentencia.Append(",cp.TIPO_CAMBIO_MONEDA ");
        //    sentencia.Append(",cp.TIPO_CAMBIO_DOLAR ");
        //    sentencia.Append(",cp.APROBADO ");
        //    sentencia.Append(",cp.SELECCIONADO ");
        //    sentencia.Append(",cp.CONGELADO ");
        //    sentencia.Append(",cp.MONTO_PROV ");
        //    sentencia.Append(",cp.SALDO_PROV ");
        //    sentencia.Append(",cp.TIPO_CAMBIO_PROV ");
        //    sentencia.Append(",cp.SUBTOTAL ");
        //    sentencia.Append(",cp.DESCUENTO ");
        //    sentencia.Append(",cp.IMPUESTO1 ");
        //    sentencia.Append(",cp.IMPUESTO2 ");
        //    sentencia.Append(",cp.FECHA_ULT_MOD ");
        //    sentencia.Append(",cp.USUARIO_ULT_MOD ");
        //    sentencia.Append(",cp.CONDICION_PAGO ");
        //    sentencia.Append(",cp.MONEDA ");
        //    sentencia.Append(",cp.SUBTIPO ");
        //    sentencia.Append(",cp.FECHA_VENCE ");
        //    sentencia.Append(",cp.FECHA_ANUL ");
        //    sentencia.Append(",cp.AUD_USUARIO_ANUL ");
        //    sentencia.Append(",cp.AUD_FECHA_ANUL ");
        //    sentencia.Append(",cp.USUARIO_APROBACION ");
        //    sentencia.Append(",cp.FECHA_APROBACION ");
        //    sentencia.Append(",cp.ANULADO ");
        //    sentencia.Append(",cp.FECHA_PROYECTADA ");
        //    sentencia.Append(",cp.DOCUMENTO_FISCAL ");
        //    sentencia.Append(",cp.ESTADO ");
        //    sentencia.Append(",cp.NOTAS ");
        //    //sentencia.Append("--IND_PROCESO ");
        //    //sentencia.Append("--FCH_PROCESO ");
        //    //sentencia.Append("--MENSAJE ");
        //    sentencia.Append(",cp.MONTO_LOCAL ");            
        //    sentencia.Append(",oc.ORDEN_COMPRA ");
        //    sentencia.Append(",el.EMBARQUE ");
        //    sentencia.Append(",sdcp.DESCRIPCION SUBTIPO_DESC, ocl.ORDEN_COMPRA_LINEA, ocl.U_RECORDID_QB ORDEN_SERVICIO_LINEA,cp.RowPointer ");
        //    sentencia.Append(",el.CANTIDAD_RECIBIDA ,el.SUBTOTAL SUBTOTAL_L, el.DESCUENTO DESCUENTO_L, el.IMPUESTO1 IMPUESTO1_L, el.IMPUESTO2 IMPUESTO2_L, (el.SUBTOTAL + el.IMPUESTO1 + el.IMPUESTO2)-el.DESCUENTO TOTAL_L ");
        //    sentencia.Append(" from  ");
        //    sentencia.Append(cia + ".ORDEN_COMPRA oc ");
        //    sentencia.Append("left join " + cia + ".ORDEN_COMPRA_LINEA ocl on oc.ORDEN_COMPRA = ocl.ORDEN_COMPRA ");
        //    sentencia.Append("left join " + cia + ".EMBARQUE_LINEA el on oc.ORDEN_COMPRA = el.ORDEN_COMPRA ");
        //    sentencia.Append("						and ocl.ORDEN_COMPRA_LINEA = el.ORDEN_COMPRA_LINEA ");
        //    //sentencia.Append("left join " + sqlClass.Compannia + ".INT_OC_LINEA iocl on ocl.ORDEN_COMPRA = iocl.ORDEN_COMPRA ");
        //    //sentencia.Append("						and ocl.ORDEN_COMPRA_LINEA = iocl.ORDEN_COMPRA_LINEA ");
        //    //sentencia.Append("						and iocl.CONJUNTO = '" + cia + "' ");
        //    sentencia.Append("left join " + cia + ".DOCUMENTOS_CP cp on el.DOCUMENTO = cp.DOCUMENTO ");
        //    sentencia.Append("						and el.PROVEEDOR = cp.PROVEEDOR ");
        //    //sentencia.Append("						--and el.TIPO_DOCUMENTO = cp.TIPO ");
        //    sentencia.Append("left join " + cia + ".EMBARQUE_DOC_CP ecp on el.EMBARQUE = ecp.EMBARQUE ");
        //    sentencia.Append("						and el.DOCUMENTO = ecp.DOCUMENTO ");
        //    sentencia.Append("						and el.PROVEEDOR = ecp.PROVEEDOR ");
        //    sentencia.Append("inner join " + cia + ".SUBTIPO_DOC_CP sdcp on cp.SUBTIPO = sdcp.SUBTIPO ");            
        //    sentencia.Append("where oc.ORDEN_COMPRA = '" + oc +"' ");

        //    dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

        //    return dt;
        //}



        /// <summary>
        /// --Obtener las ordenes de compra vigentes  
        /// </summary>
        public DataTable getSolPagosDocs(SqlTransaction transac, string cia, string oc)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append(" el.PROVEEDOR  ");
            sentencia.Append(",el.DOCUMENTO  ");
            sentencia.Append(",cp.TIPO  ");
            sentencia.Append(",max(cp.FECHA_DOCUMENTO) FECHA_DOCUMENTO ");
            sentencia.Append(",max(cp.FECHA)  FECHA ");
            sentencia.Append(",max(cp.APLICACION) APLICACION  ");
            sentencia.Append(",max(cp.MONTO) 				MONTO  ");
            sentencia.Append(",max(cp.SALDO) 				SALDO  ");
            sentencia.Append(",max(cp.SALDO_LOCAL) 		SALDO_LOCAL  ");
            sentencia.Append(",max(cp.MONTO_DOLAR) 		MONTO_DOLAR  ");
            sentencia.Append(",max(cp.SALDO_DOLAR) 		SALDO_DOLAR  ");
            sentencia.Append(",max(cp.TIPO_CAMBIO_MONEDA)	TIPO_CAMBIO_MONEDA ");
            sentencia.Append(",max(cp.TIPO_CAMBIO_DOLAR) 	TIPO_CAMBIO_DOLAR  ");
            sentencia.Append(",max(cp.APROBADO) 			APROBADO  ");
            sentencia.Append(",max(cp.SELECCIONADO) 		SELECCIONADO  ");
            sentencia.Append(",max(cp.CONGELADO) 			CONGELADO  ");
            sentencia.Append(",max(cp.MONTO_PROV)			MONTO_PROV  ");
            sentencia.Append(",max(cp.SALDO_PROV)			SALDO_PROV  ");
            sentencia.Append(",max(cp.TIPO_CAMBIO_PROV) 	TIPO_CAMBIO_PROV  ");
            sentencia.Append(",max(cp.SUBTOTAL) 			SUBTOTAL  ");
            sentencia.Append(",max(cp.DESCUENTO) 			DESCUENTO  ");
            sentencia.Append(",max(cp.IMPUESTO1) 		   IMPUESTO1  ");
            sentencia.Append(",max(cp.IMPUESTO2) 		   IMPUESTO2  ");
            sentencia.Append(",max(cp.FECHA_ULT_MOD) 	   FECHA_ULT_MOD  ");
            sentencia.Append(",max(cp.USUARIO_ULT_MOD)   USUARIO_ULT_MOD  ");
            sentencia.Append(",max(cp.CONDICION_PAGO)    CONDICION_PAGO  ");
            sentencia.Append(",max(cp.MONEDA)			   MONEDA  ");
            sentencia.Append(",max(cp.SUBTIPO) 		   SUBTIPO  ");
            sentencia.Append(",max(cp.FECHA_VENCE) 	   FECHA_VENCE  ");
            sentencia.Append(",max(cp.FECHA_ANUL)		   FECHA_ANUL  ");
            sentencia.Append(",max(cp.AUD_USUARIO_ANUL)  AUD_USUARIO_ANUL  ");
            sentencia.Append(",max(cp.AUD_FECHA_ANUL)	   AUD_FECHA_ANUL  ");
            sentencia.Append(",max(cp.USUARIO_APROBACION) USUARIO_APROBACION ");
            sentencia.Append(",max(cp.FECHA_APROBACION)  FECHA_APROBACION  ");
            sentencia.Append(",max(cp.ANULADO) 		   ANULADO  ");
            sentencia.Append(",max(cp.FECHA_PROYECTADA)  FECHA_PROYECTADA  ");
            sentencia.Append(",max(cp.DOCUMENTO_FISCAL) DOCUMENTO_FISCAL ");
            sentencia.Append(",max(cp.ESTADO) 	ESTADO  ");
            sentencia.Append(",max(cast(cp.NOTAS as varchar(8000))) NOTAS             ");
            sentencia.Append(",max(cp.MONTO_LOCAL) MONTO_LOCAL             ");
            sentencia.Append(",oc.ORDEN_COMPRA  ");
            sentencia.Append(",el.EMBARQUE  ");
            sentencia.Append(",max(sdcp.DESCRIPCION) SUBTIPO_DESC ");
            sentencia.Append(",max(ocl.ORDEN_COMPRA_LINEA) ORDEN_COMPRA_LINEA ");
            sentencia.Append(",ocl.U_RECORDID_QB ORDEN_SERVICIO_LINEA ");
            sentencia.Append(",max(cp.RowPointer) RowPointer ");
            sentencia.Append(",sum(el.CANTIDAD_RECIBIDA) CANTIDAD_RECIBIDA ");
            sentencia.Append(",sum(el.SUBTOTAL) SUBTOTAL_L ");
            sentencia.Append(",sum(el.DESCUENTO) DESCUENTO_L ");
            sentencia.Append(",sum(el.IMPUESTO1) IMPUESTO1_L ");
            sentencia.Append(",sum(el.IMPUESTO2) IMPUESTO2_L ");
            sentencia.Append(",sum((el.SUBTOTAL + el.IMPUESTO1 + el.IMPUESTO2)-el.DESCUENTO) TOTAL_L  ");
            sentencia.Append(" from  ");
            sentencia.Append(cia + ".ORDEN_COMPRA oc ");
            sentencia.Append("left join " + cia + ".ORDEN_COMPRA_LINEA ocl on oc.ORDEN_COMPRA = ocl.ORDEN_COMPRA ");
            sentencia.Append("left join " + cia + ".EMBARQUE_LINEA el on oc.ORDEN_COMPRA = el.ORDEN_COMPRA ");
            sentencia.Append("						and ocl.ORDEN_COMPRA_LINEA = el.ORDEN_COMPRA_LINEA ");
            //sentencia.Append("left join " + sqlClass.Compannia + ".INT_OC_LINEA iocl on ocl.ORDEN_COMPRA = iocl.ORDEN_COMPRA ");
            //sentencia.Append("						and ocl.ORDEN_COMPRA_LINEA = iocl.ORDEN_COMPRA_LINEA ");
            //sentencia.Append("						and iocl.CONJUNTO = '" + cia + "' ");
            sentencia.Append("left join " + cia + ".DOCUMENTOS_CP cp on el.DOCUMENTO = cp.DOCUMENTO ");
            sentencia.Append("						and el.PROVEEDOR = cp.PROVEEDOR ");
            //sentencia.Append("						--and el.TIPO_DOCUMENTO = cp.TIPO ");
            sentencia.Append("left join " + cia + ".EMBARQUE_DOC_CP ecp on el.EMBARQUE = ecp.EMBARQUE ");
            sentencia.Append("						and el.DOCUMENTO = ecp.DOCUMENTO ");
            sentencia.Append("						and el.PROVEEDOR = ecp.PROVEEDOR ");
            sentencia.Append("inner join " + cia + ".SUBTIPO_DOC_CP sdcp on cp.SUBTIPO = sdcp.SUBTIPO ");
            sentencia.Append("where oc.ORDEN_COMPRA = '" + oc + "' ");
            sentencia.Append("group by  el.PROVEEDOR ");
            sentencia.Append(",el.DOCUMENTO  ");
            sentencia.Append(",cp.TIPO  ");
            sentencia.Append(",oc.ORDEN_COMPRA  ");
            sentencia.Append(",el.EMBARQUE  ");
            sentencia.Append(",ocl.U_RECORDID_QB ");
            sentencia.Append(" order by el.PROVEEDOR ,el.DOCUMENTO  ,cp.TIPO asc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }

        /// <summary>
        /// Metodo valida si existe     
        /// </summary>
        public int existIntDoc(SqlTransaction transac, string cia, string proveedor, string documento, string tipo)
        {
            int q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT COUNT(1) FROM ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_DOCS_CP ");
            sentencia.Append("WHERE CONJUNTO = '");
            sentencia.Append(cia);
            sentencia.Append("' AND PROVEEDOR = '");
            sentencia.Append(proveedor);
            sentencia.Append("' AND DOCUMENTO = '");
            sentencia.Append(documento);
            sentencia.Append("' AND TIPO = '");
            sentencia.Append(tipo);
            sentencia.Append("' ");
            q = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString(), transac).ToString());
            return (q);
        }



        /// <summary>
        /// Metodo que extrae las solicitudes de pago y las inserta en el andamio
        /// </summary>
        public bool extractAnticipos(ref StringBuilder error)
        {
            SqlTransaction transaction = null;
            bool ok = true;
            string anticipoTipo, anticipoSubtipo;
            DataTable dtCia, dtOC, dtDocs = null;

            try
            {
                anticipoTipo = ConfigurationManager.AppSettings["AnticipoTipo"].ToString();
                anticipoSubtipo = ConfigurationManager.AppSettings["AnticipoSubtipo"].ToString();

                //inicio transaction
                transaction = sqlClass.SQLCon.BeginTransaction();

                //obtener cias en el andamio
                dtCia = getIntCompanias(transaction);
                string cia = string.Empty;
                foreach (DataRow drCia in dtCia.Rows)
                {
                    cia = drCia["CONJUNTO"].ToString();

                    //Obtener las ordenes de compra vigentes
                    dtOC = getOCActivas(transaction, cia, "('E', 'I', 'R' )");

                    foreach (DataRow drOC in dtOC.Rows)
                    {
                        //Obtener lo docs de los anticipos 
                        dtDocs = getAnticipoDocs(transaction, cia, drOC["ORDEN_COMPRA"].ToString(), anticipoTipo, anticipoSubtipo);
                        
                        if (dtDocs.Rows.Count > 0)
                        {
                            foreach (DataRow drDoc in dtDocs.Rows)
                            {
                                //Validar si existen
                                if (existIntDoc(transaction, cia
                                , drDoc["PROVEEDOR"].ToString()
                                , drDoc["DOCUMENTO"].ToString()
                                , drDoc["TIPO"].ToString()) <= 0)
                                {
                                    //inserto en el andamio
                                    ok = insIntDocsCP(transaction, cia, drOC["ORDEN_COMPRA"].ToString(), drOC["ORDEN_SERVICIO"].ToString(), drDoc, "C", ref error);
                                   
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                error.AppendLine("[extractSolicitudPagos]: Se presentaron problemas extrayendo solicitudes de pago:");
                error.AppendLine(ex.Message);
                ok = false;
            }

            finally
            {
                if (sqlClass != null)
                {
                    if (ok)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }

            return ok;
        }



        /// <summary>
        /// Metodo que extrae las solicitudes de pago y las inserta en el andamio
        /// </summary>
        public bool extractNotasCredito(ref StringBuilder error)
        {
            SqlTransaction transaction = null;
            bool ok = true;
            string NC = "N/C";
            string prov = string.Empty, docu = string.Empty, tipo = string.Empty;
            DataTable dtCia, dtDocs = null;

            try
            {   
                //inicio transaction
                transaction = sqlClass.SQLCon.BeginTransaction();

                //obtener los documentos 
                dtCia = getIntCompanias(transaction);
                string cia = string.Empty;

                foreach (DataRow drCia in dtCia.Rows)
                {
                    cia = drCia["CONJUNTO"].ToString();

                    //Obtener lo docs de los anticipos 
                    dtDocs = getNotasCredito(transaction, cia, NC);
                    
                    if (dtDocs.Rows.Count > 0)
                    {
                        foreach (DataRow drDoc in dtDocs.Rows)
                        {
                            //Validar si existen
                            if (existIntDoc(transaction, cia
                            , drDoc["PROVEEDOR"].ToString()
                            , drDoc["DOCUMENTO"].ToString()
                            , drDoc["TIPO"].ToString()) <= 0)
                            {
                                //inserto en el andamio
                                ok = insIntDocsCP_Aplic(transaction, cia, drDoc["ORDEN_COMPRA"].ToString(), drDoc["ORDEN_SERVICIO"].ToString(), drDoc, "P", ref error);

                                //inserto lineas en andamio
                                if (ok)
                                {
                                    ok = insIntDocsCPLineas(transaction, cia, drDoc["ORDEN_COMPRA"].ToString(), drDoc["ORDEN_SERVICIO"].ToString(), drDoc, ref error);
                                }
                                //indicar tmp para procesar las multiples lineas 
                                if (ok)
                                {
                                    prov = drDoc["PROVEEDOR"].ToString();
                                    docu = drDoc["DOCUMENTO"].ToString();
                                    tipo = drDoc["TIPO"].ToString();
                                }

                                if (ok)
                                {

                                    //actualizar el pago como sincronizado para la N/C

                                    //actualizo pago en andamio
                                    ok = updIntPagosCP(transaction
                                        , cia
                                        , drDoc["PROVEEDOR"].ToString()
                                        , drDoc["DOCUMENTO_APLIC"].ToString()
                                        , drDoc["TIPO_APLIC"].ToString()
                                        , drDoc["DOCUMENTO"].ToString()
                                        , drDoc["TIPO"].ToString()
                                        , "S"
                                        , string.Empty
                                        , ref error);
                                }

                                if (ok)
                                {
                                    //actualizo el IntDocCP en indicador pendiente de pago
                                    ok = updIntDocCP(transaction
                                     , cia
                                     , drDoc["PROVEEDOR"].ToString()
                                     , drDoc["DOCUMENTO_APLIC"].ToString()
                                     , drDoc["TIPO_APLIC"].ToString()
                                     , "P" //pagado
                                     , ref error);
                                }


                                if (!ok)
                                    break;

                            }
                            else if ((prov.CompareTo(drDoc["PROVEEDOR"].ToString()) == 0)
                                  && (docu.CompareTo(drDoc["DOCUMENTO"].ToString()) == 0)
                                  && (tipo.CompareTo(drDoc["TIPO"].ToString()) == 0)) //valida si hay mas lineas que asoc en la misma factura
                            {
                                ok = insIntDocsCPLineas(transaction, cia, drDoc["ORDEN_COMPRA"].ToString(), drDoc["ORDEN_SERVICIO"].ToString(), drDoc, ref error);
                            }
                        }                        
                    }

                    if (!ok)
                        break;

                }
            }
            catch (Exception ex)
            {
                error.AppendLine("[extractNotasCredito]: Se presentaron problemas extrayendo solicitudes de pago:");
                error.AppendLine(ex.Message);
                ok = false;
            }

            finally
            {
                if (sqlClass != null)
                {
                    if (ok)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }

            return ok;
        }





        /// <summary>
        /// Metodo que extrae las solicitudes de pago y las inserta en el andamio
        /// </summary>
        public bool extractSolicitudPagos(ref StringBuilder error)
        {
            SqlTransaction transaction = null;
            bool ok = true;
            string prov = string.Empty, docu = string.Empty, tipo = string.Empty;
            DataTable dtCia, dtOC, dtDocs = null;

            try
            {
                //inicio transaction
                transaction = sqlClass.SQLCon.BeginTransaction();

                //obtener cias en el andamio
                dtCia = getIntCompanias(transaction);
                string cia = string.Empty;
                foreach (DataRow drCia in dtCia.Rows)
                {
                    cia = drCia["CONJUNTO"].ToString();

                    //Obtener las ordenes de compra vigentes
                    dtOC = getOCActivas(transaction, cia, "('I', 'R' )");

                    foreach (DataRow drOC in dtOC.Rows)
                    {
                        //Obtener lo docs de la OC
                        dtDocs = getSolPagosDocs(transaction, cia, drOC["ORDEN_COMPRA"].ToString());

                        if (dtDocs.Rows.Count > 0)
                        {
                            foreach (DataRow drDoc in dtDocs.Rows)
                            {
                                //Validar si existen
                                if (existIntDoc(transaction, cia
                                , drDoc["PROVEEDOR"].ToString()
                                , drDoc["DOCUMENTO"].ToString()
                                , drDoc["TIPO"].ToString()) <= 0)
                                {
                                    //inserto en el andamio
                                    ok = insIntDocsCP(transaction, cia, drOC["ORDEN_COMPRA"].ToString(), drOC["ORDEN_SERVICIO"].ToString(), drDoc, "C", ref error);

                                    //inserto lineas en andamio
                                    if (ok)
                                    {
                                        ok = insIntDocsCPLineas(transaction, cia, drOC["ORDEN_COMPRA"].ToString(), drOC["ORDEN_SERVICIO"].ToString(), drDoc, ref error);
                                    }
                                    //indicar tmp para procesar las multiples lineas 
                                    if (ok)
                                    {                                        
                                        prov = drDoc["PROVEEDOR"].ToString();
                                        docu = drDoc["DOCUMENTO"].ToString();
                                        tipo = drDoc["TIPO"].ToString();
                                    }
                                }
                                else if ( (prov.CompareTo(drDoc["PROVEEDOR"].ToString())==0) 
                                          &&(docu.CompareTo(drDoc["DOCUMENTO"].ToString())==0)
                                          &&(tipo.CompareTo(drDoc["TIPO"].ToString())==0) ) //valida si hay mas lineas que asoc en la misma factura
                                {
                                        ok = insIntDocsCPLineas(transaction, cia, drOC["ORDEN_COMPRA"].ToString(), drOC["ORDEN_SERVICIO"].ToString(), drDoc, ref error);

                                }

                                if (!ok)
                                    break;

                            }
                        }

                        if (!ok)
                            break;
                    }
                }

            }
            catch (Exception ex)
            {
                error.AppendLine("[extractSolicitudPagos]: Se presentaron problemas extrayendo solicitudes de pago:");
                error.AppendLine(ex.Message);
                ok = false;
            }

            finally
            {
                if (sqlClass != null)
                {
                    if (ok)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }

            return ok;
        }







        #endregion



        #region Pagos

        /// <summary>
        /// --Obtener los facturas pendientes de pagar del andamio      
        /// </summary>
        public DataTable getIntDocsCP(SqlTransaction transac)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CONJUNTO, PROVEEDOR, DOCUMENTO, TIPO, SOLICITUD_PAGO, ORDEN_SERVICIO ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_DOCS_CP ");
            //sentencia.Append("where IND_SINCRO = 'C' and APROBADO = 'S' and CONGELADO = 'N' ");
            //sentencia.Append("where IND_SINCRO = 'C' and APROBADO = 'S' ");
            sentencia.Append("where IND_SINCRO = 'C' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }


        /// <summary>
        /// --Obtener la informacion del pago
        /// </summary>
        public DataTable getPagosCP(SqlTransaction transac, string cia, string prov, string doc, string tipo, string tipoPago)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append(" aux.PROVEEDOR ");
            sentencia.Append(",aux.CREDITO DOCUMENTO_CRE ");
            sentencia.Append(",aux.TIPO_CREDITO TIPO_CRE ");
            sentencia.Append(",aux.DEBITO DOCUMENTO_DEB ");
            sentencia.Append(",aux.TIPO_DEBITO TIPO_DEB ");
            sentencia.Append(",cp.FECHA ");
            sentencia.Append(",cp.FECHA_DOCUMENTO ");
            sentencia.Append(",cp.FECHA_VENCE ");
            sentencia.Append(",aux.MONEDA_DEBITO MONEDA ");
            sentencia.Append(",aux.MONTO_LOCAL ");
            sentencia.Append(",aux.MONTO_DOLAR ");
            sentencia.Append(",cp.TIPO_CAMBIO_MONEDA ");
            sentencia.Append(",cp.TIPO_CAMBIO_DOLAR ");
            sentencia.Append(",cp.ROWPOINTER ");
            sentencia.Append("from  ");
            sentencia.Append(cia);
            sentencia.Append(".AUXILIAR_CP aux inner join ");
            sentencia.Append(cia);
            sentencia.Append(".DOCUMENTOS_CP cp on aux.DEBITO = cp.DOCUMENTO and aux.TIPO_DEBITO = cp.TIPO and aux.PROVEEDOR = cp.PROVEEDOR ");
            sentencia.Append("where aux.CREDITO = '" + doc + "' ");
            sentencia.Append("and aux.PROVEEDOR = '" + prov + "' ");
            sentencia.Append("and aux.TIPO_CREDITO = '" + tipo + "' ");
            sentencia.Append("and aux.TIPO_DEBITO in " + tipoPago);

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }


        /// <summary>
        /// Metodo valida si existe     
        /// </summary>
        public int existIntPago(SqlTransaction transac, string cia, string proveedor, string documentoCre, string tipoCre, string documentoDeb, string tipoDeb)
        {
            int q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT COUNT(1) FROM ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_PAGOS_CP ");
            sentencia.Append("WHERE CONJUNTO = '");
            sentencia.Append(cia);
            sentencia.Append("' AND PROVEEDOR = '");
            sentencia.Append(proveedor);
            sentencia.Append("' AND DOCUMENTO_CRE = '");
            sentencia.Append(documentoCre);
            sentencia.Append("' AND TIPO_CRE = '");
            sentencia.Append(tipoCre);            
            sentencia.Append("' AND DOCUMENTO_DEB = '");
            sentencia.Append(documentoDeb);
            sentencia.Append("' AND TIPO_DEB = '");
            sentencia.Append(tipoDeb);
            sentencia.Append("' ");
            q = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString(), transac).ToString());
            return (q);
        }

        /// <summary>
        /// --Obtener los facturas pendientes de pagar del andamio      
        /// </summary>
        public DataTable getIntPagosCP(SqlTransaction transac)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select CONJUNTO, PROVEEDOR, DOCUMENTO_CRE, TIPO_CRE, DOCUMENTO_DEB, TIPO_DEB ");
            sentencia.Append("from  ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".INT_PAGOS_CP ");
            sentencia.Append("where IND_PROCESO = 'P' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }


        /// <summary>
        /// Metodo encargado de insertar cia | bod
        /// </summary>
        public bool insIntPagosCP(SqlTransaction transac, string cia, DataRow doc, DataRow pago, ref StringBuilder errores)
        {

            bool lbOK = true;
            SqlCommand cmd = null;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("INSERT INTO ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".INT_PAGOS_CP ");
                sentencia.Append("(CONJUNTO,PROVEEDOR,DOCUMENTO_CRE,TIPO_CRE,DOCUMENTO_DEB,TIPO_DEB,FECHA,FECHA_DOCUMENTO,FECHA_VENCE,MONEDA,MONTO_LOCAL");
                sentencia.Append(",MONTO_DOLAR,TIPO_CAMBIO_MONEDA,TIPO_CAMBIO_DOLAR,ROWPOINTER,IND_PROCESO,FCH_PROCESO,MENSAJE");
                sentencia.Append(") VALUES (");
                sentencia.Append("@CONJUNTO,@PROVEEDOR,@DOCUMENTO_CRE,@TIPO_CRE,@DOCUMENTO_DEB,@TIPO_DEB,@FECHA,@FECHA_DOCUMENTO,@FECHA_VENCE,@MONEDA,@MONTO_LOCAL");
                sentencia.Append(",@MONTO_DOLAR,@TIPO_CAMBIO_MONEDA,@TIPO_CAMBIO_DOLAR,@ROWPOINTER,@IND_PROCESO,@FCH_PROCESO,@MENSAJE");
                sentencia.Append(")");

                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sentencia.ToString();

                //seteo los parametros  
                cmd.Parameters.Add("@CONJUNTO          ", SqlDbType.VarChar, 10).Value = cia;
                cmd.Parameters.Add("@PROVEEDOR         ", SqlDbType.VarChar, 20).Value = doc["PROVEEDOR"];
                cmd.Parameters.Add("@DOCUMENTO_CRE     ", SqlDbType.VarChar, 50).Value = doc["DOCUMENTO"]; 
                cmd.Parameters.Add("@TIPO_CRE          ", SqlDbType.VarChar, 3).Value = doc["TIPO"];
                cmd.Parameters.Add("@DOCUMENTO_DEB     ", SqlDbType.VarChar, 50).Value = pago["DOCUMENTO_DEB"];
                cmd.Parameters.Add("@TIPO_DEB          ", SqlDbType.VarChar, 3).Value = pago["TIPO_DEB"];
                cmd.Parameters.Add("@FECHA             ", SqlDbType.DateTime).Value = pago["FECHA"];
                cmd.Parameters.Add("@FECHA_DOCUMENTO   ", SqlDbType.DateTime).Value = pago["FECHA_DOCUMENTO"];
                cmd.Parameters.Add("@FECHA_VENCE       ", SqlDbType.DateTime).Value = pago["FECHA_VENCE"];
                cmd.Parameters.Add("@MONEDA            ", SqlDbType.VarChar, 4).Value = pago["MONEDA"];
                cmd.Parameters.Add("@MONTO_LOCAL       ", SqlDbType.Decimal).Value = pago["MONTO_LOCAL"];
                cmd.Parameters.Add("@MONTO_DOLAR       ", SqlDbType.Decimal).Value = pago["MONTO_DOLAR"];
                cmd.Parameters.Add("@TIPO_CAMBIO_MONEDA", SqlDbType.Decimal).Value = pago["TIPO_CAMBIO_MONEDA"];
                cmd.Parameters.Add("@TIPO_CAMBIO_DOLAR ", SqlDbType.Decimal).Value = pago["TIPO_CAMBIO_DOLAR"];
                cmd.Parameters.Add("@ROWPOINTER        ", SqlDbType.VarChar, 100).Value = Convert.ToString(pago["RowPointer"]);
                cmd.Parameters.Add("@IND_PROCESO       ", SqlDbType.VarChar, 1).Value = "P";
                cmd.Parameters.Add("@FCH_PROCESO       ", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd");
                cmd.Parameters.Add("@MENSAJE           ", SqlDbType.VarChar, 500).Value = string.Empty;

                if (cmd.ExecuteNonQuery() < 1)
                {
                    errores.AppendLine("[insIntPagosCP]: Se presentaron problemas insertando INT_PAGOS_CP: ");
                    errores.AppendLine(doc["PROVEEDOR"].ToString() +" | Credito: "+ doc["DOCUMENTO"].ToString() + " | Debito: " + pago["DOCUMENTO_DEB"].ToString());
                    lbOK = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[insIntPagosCP]: Detalle Técnico: " + e.Message);
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
        /// Metodo que extrae las solicitudes de pago y las inserta en el andamio
        /// </summary>
        public bool extractPagos(ref StringBuilder error)
        {
            SqlTransaction transaction = null;
            bool ok = true;
            DataTable dtDocs, dtPagos = null;
            string cia = string.Empty;
            try
            {

                //inicio transaction
                transaction = sqlClass.SQLCon.BeginTransaction();

                //obtener documentos pendientes de sincronizar
                dtDocs = getIntDocsCP(transaction);
                
                foreach (DataRow drDocs in dtDocs.Rows)
                {
                    cia = drDocs["CONJUNTO"].ToString();

                    //Obtener la informacion del pago
                    dtPagos = getPagosCP(transaction, cia
                                , drDocs["PROVEEDOR"].ToString()
                                , drDocs["DOCUMENTO"].ToString()
                                , drDocs["TIPO"].ToString()
                                , ConfigurationManager.AppSettings["TipoPagoCP"].ToString());

                    foreach (DataRow drPago in dtPagos.Rows)
                    {                        
                        //Validar si existen el pago
                        if (existIntPago(transaction, cia
                        , drDocs["PROVEEDOR"].ToString()
                        , drDocs["DOCUMENTO"].ToString()
                        , drDocs["TIPO"].ToString()
                        , drPago["DOCUMENTO_DEB"].ToString()
                        , drPago["TIPO_DEB"].ToString()
                        ) <= 0)
                        {
                            //inserto en el andamio
                            ok = insIntPagosCP(transaction, cia, drDocs, drPago, ref error);
                        }                           
                    }
                }

            }
            catch (Exception ex)
            {
                error.AppendLine("[extractPagos]: Se presentaron problemas extrayendo pagos:");
                error.AppendLine(ex.Message);
                ok = false;
            }

            finally
            {
                if (sqlClass != null)
                {
                    if (ok)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }

            return ok;
        }



        #endregion



        #region Sync











        /// <summary>
        /// Metodo que sincroniza los proveedores
        /// </summary>
        public bool syncProveedores(string url, string ck, string cs, ref StringBuilder error)
        {
            SqlTransaction transaction = null;
            bool ok = true;
            DataTable dtProv = null;
            StringBuilder json = new StringBuilder();
            jsonHandler j = new jsonHandler();

            int p = 0;

            try
            {
                //inicio transaction
                transaction = sqlClass.SQLCon.BeginTransaction();

                //obtener companias a sync
                dtProv = getProveedoresSync(transaction);

                while (p < dtProv.Rows.Count)
                {
                    //obtener encabezado
                    json.Append(j.getHeader(ck, cs, "proveedor"));

                    //obtener json de la cia
                    json.Append(j.DataRowToJson(dtProv, p));

                    //obtener footer
                    json.Append(j.getFooter());

                    //sync
                    var client = new RestClient(url);
                    var request = new RestRequest(Method.POST);
                    request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    //revisar respuesta
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        responseHandler r = JsonConvert.DeserializeObject<responseHandler>(response.Content);
                        if (r.Status.CompareTo("OK") == 0)
                        {
                            updIntProveedor(transaction, dtProv.Rows[p]["codigoproveedor"].ToString(), "S", string.Empty, ref error);
                        }
                        else
                        {
                            updIntProveedor(transaction, dtProv.Rows[p]["codigoproveedor"].ToString(), "E", r.Reason, ref error);
                        }
                    }
                    else
                    {
                        updIntProveedor(transaction, dtProv.Rows[p]["codigoproveedor"].ToString(), "E", response.Content, ref error);
                    }

                    //sgt cia - limpiar variables
                    p++;
                    json.Clear();
                }
            }
            catch (Exception ex)
            {
                error.AppendLine("[syncProveedores]: Se presentaron problemas sincronizando proveedores:");
                error.AppendLine(ex.Message);
                ok = false;
            }

            finally
            {
                if (sqlClass != null)
                {
                    if (ok)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }

            return ok;
        }




        /// <summary>
        /// Metodo que sincroniza las companias
        /// </summary>
        public bool syncCompanias(string url, string ck, string cs, ref StringBuilder error)
        {
            SqlTransaction transaction = null;
            bool ok = true;
            DataTable dtCias = null;
            StringBuilder json = new StringBuilder();
            jsonHandler j = new jsonHandler();
            int p = 0;

            try
            {
                //inicio transaction
                transaction = sqlClass.SQLCon.BeginTransaction();

                //obtener companias a sync
                dtCias = getCompaniasSync(transaction, ConfigurationManager.AppSettings["urlMultimedia"].ToString());

                while (p < dtCias.Rows.Count)
                {
                    //obtener encabezado
                    json.Append(j.getHeader(ck, cs, "compania"));

                    //obtener json de la cia
                    json.Append(j.DataRowToJson(dtCias, p));

                    //obtener footer
                    json.Append(j.getFooter());

                    //sync
                    var client = new RestClient(url);
                    var request = new RestRequest(Method.POST);
                    request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    //revisar respuesta
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        responseHandler r = JsonConvert.DeserializeObject<responseHandler>(response.Content);
                        if (r.Status.CompareTo("OK") == 0)
                        {
                            updIntConjunto(transaction, dtCias.Rows[p]["codigo"].ToString(), "S", string.Empty, ref error);
                        }
                        else
                        {
                            updIntConjunto(transaction, dtCias.Rows[p]["codigo"].ToString(), "E", r.Reason, ref error);
                        }
                    }
                    else
                    {
                        updIntConjunto(transaction, dtCias.Rows[p]["codigo"].ToString(), "E", response.Content, ref error);
                    }

                    //sgt cia - limpiar variables
                    p++;
                    json.Clear();
                }
            }
            catch (Exception ex)
            {
                error.AppendLine("[syncCompanias]: Se presentaron problemas sincronizando cias:");
                error.AppendLine(ex.Message);
                ok = false;
            }

            finally
            {
                if (sqlClass != null)
                {
                    if (ok)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }

            return ok;
        }



        /// <summary>
        /// Metodo que sincroniza las solicitudes de pago
        /// </summary>
        public bool syncSolicitudesPago(string url, string ck, string cs, ref StringBuilder error)
        {
            SqlTransaction transaction = null;
            bool ok = true;
            DataTable dtDocs = null;
            StringBuilder json = new StringBuilder();
            jsonHandler j = new jsonHandler();           

            try
            {
                //inicio transaction
                transaction = sqlClass.SQLCon.BeginTransaction();              
               
                //obtener encabezado
                json.Append(j.getHeader(ck, cs, "solicitudpago"));

                //abrir arreglo de docs
                json.Append(j.getOpenArray());

                dtDocs = getDocsCPSync(transaction);
                foreach (DataRow doc in dtDocs.Rows)
                {
                    //obtener doc
                    json.Append(j.DataRowToJson(getDocCPSync(transaction
                                                                , doc["CONJUNTO"].ToString()
                                                                , doc["PROVEEDOR"].ToString()
                                                                , doc["DOCUMENTO"].ToString()
                                                                , doc["TIPO"].ToString()
                                                                , ConfigurationManager.AppSettings["urlDocs"].ToString()),0,1));
                    //add lines
                    json.Append(j.getLines());

                    json.Append(j.DataTableToJson(getDocCPLineaSync(transaction
                                                                , doc["CONJUNTO"].ToString()
                                                                , doc["PROVEEDOR"].ToString()
                                                                , doc["DOCUMENTO"].ToString()
                                                                , doc["TIPO"].ToString() )));

                    json.Append(j.getCloseLines());
                }

                //remover , del ultimo item
                json.Remove(json.Length-1, 1);

                //cerrar array
                json.Append(j.getCloseArray());

                //obtener footer
                json.Append(j.getFooter());

                //existen documentos para enviar
                if (dtDocs.Rows.Count > 0)
                {
                    //sync
                    var client = new RestClient(url);
                    var request = new RestRequest(Method.POST);
                    request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    //revisar respuesta
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //parsear la respuesta
                        responseHandlerSolPagos r = JsonConvert.DeserializeObject<responseHandlerSolPagos>(response.Content);

                        //revisar la respuesta existosa
                        if (r.recordID.Count > 0)
                        {
                            int q = 0;
                            while (q < r.recordID.Count)
                            {
                                string[] doc = r.recordID[q].idsolicitud.Split('|');

                                ok = updIntDocCP(transaction
                                    , doc[0].ToString()
                                    , doc[1].ToString()
                                    , doc[2].ToString()
                                    , doc[3].ToString()
                                    , "S"
                                    , string.Empty
                                    , r.recordID[q].record
                                    , ref error);

                                if (!ok)
                                    break;
                                
                                q++;
                            }
                        }

                        //revisar la respuesta error
                        if (r.Reason.Count > 0)
                        {
                            int q = 0;
                            while (q < r.Reason.Count)
                            {
                                string[] doc = r.Reason[q].idsolicitud.Split('|');

                                ok = updIntDocCP(transaction
                                    , doc[0].ToString()
                                    , doc[1].ToString()
                                    , doc[2].ToString()
                                    , doc[3].ToString()
                                    , "E"
                                    , r.Reason[q].error
                                    , string.Empty
                                    , ref error);

                                if (!ok)
                                    break;

                                q++;

                            }
                        }
                        
                    }
                    else
                    {
                        //No se pudo hacer el request, no hay catch y se intenta de nuevo.
                    }
                }

            }
            catch (Exception ex)
            {
                error.AppendLine("[syncSolicitudesPago]: Se presentaron problemas sincronizando:");
                error.AppendLine(ex.Message);
                ok = false;
            }

            finally
            {
                if (sqlClass != null)
                {
                    if (ok)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }

            return ok;
        }



        /// <summary>
        /// Metodo que sincroniza los pagos
        /// </summary>
        public bool syncPagos(string url, string ck, string cs, ref StringBuilder error)
        {
            SqlTransaction transaction = null;
            bool ok = true;
            DataTable dtDocs = null;
            StringBuilder json = new StringBuilder();
            jsonHandler j = new jsonHandler();            

            try
            {
                //inicio transaction
                transaction = sqlClass.SQLCon.BeginTransaction();

                //obtener encabezado
                json.Append(j.getHeader(ck, cs, "pago"));

                //abrir arreglo de docs
                //json.Append(j.getOpenArray());

                //select de los pagos a sincronizar
                dtDocs = getPagosCPSync(transaction);

                //select pagos actualizar indicadores
                //dtUpd = getIntPagosCP(transaction);

                //genero json list
                json.Append(j.DataTableToJson(dtDocs));

                //cerrar array
                //json.Append(j.getCloseArray());

                //obtener footer
                json.Append(j.getFooter());

                if (dtDocs.Rows.Count > 0)
                {
                    //sync
                    var client = new RestClient(url);
                    var request = new RestRequest(Method.POST);
                    request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    //revisar respuesta
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        responseHandlerPagos r = JsonConvert.DeserializeObject<responseHandlerPagos>(response.Content);

                        //revisar respuesta exitosa
                        if (r.recordID.Count > 0)
                        {
                            int q = 0;
                            while (q < r.recordID.Count)
                            {
                                string[] doc = r.recordID[q].idpago.Split('|');

                                //actualizo pago en andamio
                                ok = updIntPagosCP(transaction
                                    , doc[0].ToString()
                                    , doc[1].ToString()
                                    , doc[2].ToString()
                                    , doc[3].ToString()
                                    , doc[4].ToString()
                                    , doc[5].ToString()
                                    , "S"
                                    , r.recordID[q].record
                                    , ref error);

                                if (ok)
                                {
                                    //actualizo el IntDocCP en indicador pendiente de pago
                                    ok = updIntDocCP(transaction
                                     , doc[0].ToString()
                                     , doc[1].ToString()
                                     , doc[2].ToString()
                                     , doc[3].ToString()
                                     , "P" //pagado
                                     , ref error);
                                }

                                if (!ok)
                                    break;

                                q++;
                            }
                        }

                        //revisar la respuesta con error
                        if (r.Reason.Count > 0)
                        {
                            int q = 0;
                            while (q < r.Reason.Count)
                            {
                                string[] doc = r.Reason[q].idpago.Split('|');

                                //actualizo pago en andamio
                                ok = updIntPagosCP(transaction
                                    , doc[0].ToString()
                                    , doc[1].ToString()
                                    , doc[2].ToString()
                                    , doc[3].ToString()
                                    , doc[4].ToString()
                                    , doc[5].ToString()
                                    , "E"
                                    , r.Reason[q].error
                                    , ref error);

                                if (ok)
                                {
                                    //actualizo el IntDocCP en indicador pendiente de pago
                                    ok = updIntDocCP(transaction
                                     , doc[0].ToString()
                                     , doc[1].ToString()
                                     , doc[2].ToString()
                                     , doc[3].ToString()
                                     , "P" //pagado
                                     , ref error);
                                }

                                if (!ok)
                                    break;

                                q++;

                            }                            
                        }
                    }
                    else
                    {
                        //No se pudo hacer el request, no hay catch y se intenta de nuevo en la siguiente corrida
                    }
                }

            }
            catch (Exception ex)
            {
                error.AppendLine("[syncPagos]: Se presentaron problemas sincronizando:");
                error.AppendLine(ex.Message);
                ok = false;
            }

            finally
            {
                if (sqlClass != null)
                {
                    if (ok)
                        transaction.Commit();
                    else
                        transaction.Rollback();
                }
            }

            return ok;
        }




        #endregion
























































































































    }
}
