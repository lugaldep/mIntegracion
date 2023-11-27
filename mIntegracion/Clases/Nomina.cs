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
    class Nomina
    {
        private SQL sqlClass;

        public Nomina(SQL _sqlClass)
        {
            this.sqlClass = _sqlClass;
        }

        /// <summary>
        /// Obtener el estado de la nomina
        /// </summary>
        public DataTable getNominaEstado(string nomina)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select n.NOMINA, n.ESTADO,   ");
            sentencia.Append("case n.ESTADO                ");
            sentencia.Append("when 'U' then 'NUEVA'        ");
            sentencia.Append("when 'I' then 'INICIALIZADA' ");
            sentencia.Append("when 'C' then 'CALCULADA'    ");
            sentencia.Append("when 'M' then 'MODIFICADA'   ");
            sentencia.Append("when 'P' then 'APROBADA'     ");
            sentencia.Append("when 'A' then 'APLICADA'     ");
            sentencia.Append("when 'N' then 'ANTICIPADA'   ");
            sentencia.Append("end ESTADO_DESC from         ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".NOMINA n ");
            sentencia.Append("WHERE NOMINA = '");
            sentencia.Append(nomina);
            sentencia.Append("'");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());
            return dt;
        }

        /// <summary>
        /// Obtener el estado de la nomina
        /// </summary>
        public DataTable getNominaDetalle(string nomina, string numero)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select n.NOMINA, n.ESTADO, nh.FECHA_INICIO, nh.FECHA_FIN, ");
            sentencia.Append("case n.ESTADO                ");
            sentencia.Append("when 'U' then 'NUEVA'        ");
            sentencia.Append("when 'I' then 'INICIALIZADA' ");
            sentencia.Append("when 'C' then 'CALCULADA'    ");
            sentencia.Append("when 'M' then 'MODIFICADA'   ");
            sentencia.Append("when 'P' then 'APROBADA'     ");
            sentencia.Append("when 'A' then 'APLICADA'     ");
            sentencia.Append("when 'N' then 'ANTICIPADA'   ");
            sentencia.Append("end ESTADO_DESC from         ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".NOMINA n inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".NOMINA_HISTORICO nh on n.NOMINA = nh.NOMINA ");            
            sentencia.Append("WHERE n.NOMINA = '");
            sentencia.Append(nomina);
            sentencia.Append("' and nh.NUMERO_NOMINA = ");
            sentencia.Append(numero);
            

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());
            return dt;
        }


        /// <summary>
        /// Obtener la ULTIMA nomina
        /// </summary>
        public DataTable getUltimaNomina(SqlTransaction transac, string cia, string nomina)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select TOP 1 n.NOMINA, n.ESTADO, nh.FECHA_INICIO, nh.FECHA_FIN, nh.NUMERO_NOMINA, nh.ASIENTO,  ");
            sentencia.Append("case n.ESTADO                ");
            sentencia.Append("when 'U' then 'NUEVA'        ");
            sentencia.Append("when 'I' then 'INICIALIZADA' ");
            sentencia.Append("when 'C' then 'CALCULADA'    ");
            sentencia.Append("when 'M' then 'MODIFICADA'   ");
            sentencia.Append("when 'P' then 'APROBADA'     ");
            sentencia.Append("when 'A' then 'APLICADA'     ");
            sentencia.Append("when 'N' then 'ANTICIPADA'   ");
            sentencia.Append("end ESTADO_DESC from         ");
            sentencia.Append(cia);
            sentencia.Append(".NOMINA n inner join ");
            sentencia.Append(cia);
            sentencia.Append(".NOMINA_HISTORICO nh on n.NOMINA = nh.NOMINA ");
            sentencia.Append("WHERE n.NOMINA = '");
            sentencia.Append(nomina);
            sentencia.Append("' ");
            sentencia.Append(" ORDER BY nh.NUMERO_NOMINA desc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);
            return dt;
        }


        /// <summary>
        /// Obtener la ULTIMA nomina por rango de fechas
        /// </summary>
        public DataTable getUltimaNomina(SqlTransaction transac, string cia, string nomina, DateTime fInicio, DateTime fFin)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select TOP 1 n.NOMINA, n.ESTADO, nh.FECHA_INICIO, nh.FECHA_FIN, nh.NUMERO_NOMINA, nh.ASIENTO,  ");
            sentencia.Append("case n.ESTADO                ");
            sentencia.Append("when 'U' then 'NUEVA'        ");
            sentencia.Append("when 'I' then 'INICIALIZADA' ");
            sentencia.Append("when 'C' then 'CALCULADA'    ");
            sentencia.Append("when 'M' then 'MODIFICADA'   ");
            sentencia.Append("when 'P' then 'APROBADA'     ");
            sentencia.Append("when 'A' then 'APLICADA'     ");
            sentencia.Append("when 'N' then 'ANTICIPADA'   ");
            sentencia.Append("end ESTADO_DESC from         ");
            sentencia.Append(cia);
            sentencia.Append(".NOMINA n inner join ");
            sentencia.Append(cia);
            sentencia.Append(".NOMINA_HISTORICO nh on n.NOMINA = nh.NOMINA ");
            sentencia.Append("WHERE n.NOMINA = '");
            sentencia.Append(nomina);
            sentencia.Append("' ");

            sentencia.Append(" and nh.FECHA_INICIO >= '" + fInicio.ToString("yyyy-MM-dd") + "'");
            sentencia.Append(" and nh.FECHA_FIN <= '" + fFin.ToString("yyyy-MM-dd") + "'");

            sentencia.Append(" ORDER BY nh.NUMERO_NOMINA desc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);
            return dt;
        }


        /// <summary>
        /// Obtener la ULTIMA nomina por rango de fechas
        /// </summary>
        public string getUltimaNominaAsiento(SqlTransaction transac, string cia, string nomina, DateTime fInicio, DateTime fFin)
        {
            
            StringBuilder sentencia = new StringBuilder();
            string ultValor = string.Empty;
            DataTable dt;

            sentencia.Append("select TOP 1  nh.ASIENTO from ");
            sentencia.Append(cia);
            sentencia.Append(".NOMINA n inner join ");
            sentencia.Append(cia);
            sentencia.Append(".NOMINA_HISTORICO nh on n.NOMINA = nh.NOMINA ");
            sentencia.Append("WHERE n.NOMINA = '");
            sentencia.Append(nomina);
            sentencia.Append("' ");
            sentencia.Append(" and nh.FECHA_INICIO >= '" + fInicio.ToString("yyyy-MM-dd") + "'");
            sentencia.Append(" and nh.FECHA_FIN <= '" + fFin.ToString("yyyy-MM-dd") + "'");
            sentencia.Append(" ORDER BY nh.NUMERO_NOMINA desc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            if (dt.Rows.Count > 0)
            {
                ultValor = dt.Rows[0]["ASIENTO"].ToString();
            }
            else
                ultValor = string.Empty;

                        
            return ultValor;
        }

        // <summary>
        /// Actualizar documento
        /// </summary>
        public bool updEmpleadoConcNomi(string nomina, int numero, string empleado, string concepto, decimal monto, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(sqlClass.Compannia);
                sentencia.Append(".EMPLEADO_CONC_NOMI ");
                sentencia.Append("SET FECHA_MODIFICACION    ='" + DateTime.Now.ToString("yyyy-MM-dd") + "'");                
                sentencia.Append(", USUARIO ='" + sqlClass.Usuario + "'");
                sentencia.Append(", CANTIDAD = 0 ");
                sentencia.Append(", MONTO =  MONTO +" + monto.ToString() + "");
                sentencia.Append(", TOTAL = TOTAL + " + monto.ToString() + "");
                sentencia.Append(" WHERE NOMINA = '");
                sentencia.Append(nomina);
                sentencia.Append("' AND NUMERO_NOMINA = ");
                sentencia.Append(numero);
                sentencia.Append(" AND EMPLEADO = '");
                sentencia.Append(empleado);
                sentencia.Append("' AND CONCEPTO = '");
                sentencia.Append(concepto);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString()) < 1)
                {
                    errores.AppendLine("[updEmpleadoConcNomi]: Se presentaron problemas actualizando EMPLEADO_CONC_NOMI: ");
                    errores.Append(empleado);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updEmpleadoConcNomi]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }


        // <summary>
        /// Actualizar documento
        /// </summary>
        public bool updEmpleadoConcNomi(SqlTransaction transac, string cia, string nomina, int numero, string empleado, string concepto, decimal monto, decimal cant, ref StringBuilder errores)
        {
            bool lbOk = true;
            StringBuilder sentencia = new StringBuilder();

            try
            {
                sentencia.Append("UPDATE ");
                sentencia.Append(cia);
                sentencia.Append(".EMPLEADO_CONC_NOMI ");
                sentencia.Append("SET FECHA_MODIFICACION    ='" + DateTime.Now.ToString("yyyy-MM-dd") + "'");
                sentencia.Append(", USUARIO ='" + sqlClass.Usuario + "'");
                sentencia.Append(", CANTIDAD = CANTIDAD + " + cant.ToString() );
                sentencia.Append(", MONTO =  MONTO +" + monto.ToString() + "");
                sentencia.Append(", TOTAL = TOTAL + " + monto.ToString() + "");
                sentencia.Append(" WHERE NOMINA = '");
                sentencia.Append(nomina);
                sentencia.Append("' AND NUMERO_NOMINA = ");
                sentencia.Append(numero);
                sentencia.Append(" AND EMPLEADO = '");
                sentencia.Append(empleado);
                sentencia.Append("' AND CONCEPTO = '");
                sentencia.Append(concepto);
                sentencia.Append("'");

                if (sqlClass.EjecutarUpdate(sentencia.ToString(), transac) < 1)
                {
                    errores.AppendLine("[updEmpleadoConcNomi]: Se presentaron problemas actualizando EMPLEADO_CONC_NOMI: ");
                    errores.Append(empleado);
                    lbOk = false;
                }
            }
            catch (Exception e)
            {
                errores.AppendLine("[updEmpleadoConcNomi]: Detalle Técnico: " + e.Message);
                lbOk = false;
            }
            return lbOk;
        }


        /// <summary>
        /// Inserta en la tabla de empleado conc nomi
        /// </summary>
        public bool insEmpleadoConcNomi(SqlTransaction transac, string cia, string empleado, string concepto, string nomina, int numero, string cc, string formaApli, decimal cant, decimal monto, decimal total
            , ref StringBuilder error)
        {

            bool ok = true;
            StringBuilder sQuery = new StringBuilder();
            SqlCommand cmd = null;
            string lerror = string.Empty;

            sQuery.AppendLine("Insert into " + cia + ".EMPLEADO_CONC_NOMI (");
            sQuery.AppendLine("EMPLEADO,CONCEPTO,NOMINA,NUMERO_NOMINA,CENTRO_COSTO,FORMA_APLICACION,CANTIDAD,MONTO,TOTAL ");
            sQuery.AppendLine(") values (");
            sQuery.AppendLine("@EMPLEADO,@CONCEPTO,@NOMINA,@NUMERO_NOMINA,@CENTRO_COSTO,@FORMA_APLICACION,@CANTIDAD,@MONTO,@TOTAL");
            sQuery.AppendLine(" )");

            try
            {
                //seteo el command
                cmd = sqlClass.SQLCon.CreateCommand();
                cmd.Transaction = transac;
                cmd.CommandText = sQuery.ToString();
                
                cmd.Parameters.AddWithValue("EMPLEADO        ", empleado);
                cmd.Parameters.AddWithValue("CONCEPTO        ", concepto);
                cmd.Parameters.AddWithValue("NOMINA          ", nomina);
                cmd.Parameters.AddWithValue("NUMERO_NOMINA   ", numero);
                cmd.Parameters.AddWithValue("CENTRO_COSTO    ", cc);
                cmd.Parameters.AddWithValue("FORMA_APLICACION", formaApli);
                cmd.Parameters.AddWithValue("CANTIDAD        ", cant);
                cmd.Parameters.AddWithValue("MONTO           ", monto);
                cmd.Parameters.AddWithValue("TOTAL           ", total);


                if (cmd.ExecuteNonQuery() < 1)
                {
                    error.AppendLine("[insEmpleadoConcNomi]: Se presentaron problemas insertando el empleado conc nomi: ");
                    error.AppendLine(empleado + " | " + concepto);
                    ok = false;
                }
            }
            catch (Exception ex)
            {
                error.AppendLine("[insEmpleadoConcNomi]: Se presentaron problemas insertando el empleado conc nomi: ");
                error.AppendLine(empleado + " | " + concepto);
                error.AppendLine(ex.Message);
                ok = false;
            }

            finally
            {
                cmd.Dispose();
                cmd = null;
            }
            return ok;

        }


        /// <summary>
        /// Obtener el max del empleado conc nomi
        /// </summary>
        public int getMaxEmpleadoConcNomi(SqlTransaction transac, string cia)
        {
            int q = 0;

            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select isnull(MAX(CONSECUTIVO)+1, 1) AS CONSECUTIVO from ");
            sentencia.Append(cia);
            sentencia.Append(".EMPLEADO_CONC_NOMI ");

            q = Convert.ToInt32(sqlClass.EjecutarScalar(sentencia.ToString(), transac));

            return (q);
        }



        /// <summary>
        /// Obtener el empleado
        /// </summary>
        public string getEmpleado(string vendedor)
        {           
            string q = string.Empty;

            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select EMPLEADO from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".VENDEDOR ");
            sentencia.Append("WHERE VENDEDOR = '");
            sentencia.Append(vendedor);
            sentencia.Append("' ");
            
            q = sqlClass.EjecutarScalar(sentencia.ToString()).ToString();

            return (q);
        }


        /// <summary>
        /// Metodo valida existe     
        /// </summary>
        public int existeConceptoNomina(SqlTransaction transac, string cia, string empleado, string nomina, int numero, string concepto)
        {
            int q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT COUNT(1) FROM ");
            sentencia.Append(cia);
            sentencia.Append(".EMPLEADO_CONC_NOMI ");
            sentencia.Append("WHERE EMPLEADO = '" + empleado + "'");
            sentencia.Append(" and CONCEPTO = '" + concepto + "'");
            sentencia.Append(" and NOMINA = '" + concepto + "'");
            sentencia.Append(" and NUMERO_NOMINA = " + numero + "");
            
            q = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString(), transac).ToString());
            
            return (q);

        }

        /// <summary>
        /// Metodo encargado de obtener los empleados
        /// </summary>
        public DataTable getEmpleados(string cia, string nomina, string numero, string id)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            //sentencia.Append("select e.EMPLEADO, e.NOMBRE, e.E_MAIL, e.ACTIVO, e.IDENTIFICACION  from ");
            //sentencia.Append(cia);
            //sentencia.Append(".EMPLEADO e inner join ");
            //sentencia.Append(cia);
            //sentencia.Append(".EMPLEADO_NOMI_NETO enn on e.EMPLEADO = enn.EMPLEADO ");
            //sentencia.Append("and enn.NUMERO_NOMINA = " + numero + " ");
            //sentencia.Append("and e.NOMINA = enn.NOMINA ");
            //sentencia.Append("WHERE e.NOMINA = '");
            //sentencia.Append(nomina);
            //sentencia.Append("' and e.ACTIVO = 'S' order by e.EMPLEADO asc ");


            sentencia.Append("select e.EMPLEADO, e.NOMBRE, e.E_MAIL, e.ACTIVO, e.IDENTIFICACION  from ");
            sentencia.Append(cia);
            sentencia.Append(".EMPLEADO e inner join ");
            sentencia.Append(cia);
            sentencia.Append(".EMPLEADO_NOMI_NETO enn on e.EMPLEADO = enn.EMPLEADO ");
            sentencia.Append("and enn.NUMERO_NOMINA = " + numero + " ");
            sentencia.Append("WHERE enn.NOMINA = '");
            sentencia.Append(nomina);
            sentencia.Append("' and e.ACTIVO = 'S' ");

            if (id.CompareTo(string.Empty)!=0)
            {
                sentencia.Append(" and e.IDENTIFICACION  = '" + id + "'");
            }

            sentencia.Append(" order by e.EMPLEADO asc ");


            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }



        /// <summary>
        /// Metodo encargado de obtener los empleados
        /// </summary>
        public DataTable getEmpleados(SqlTransaction transac, string cia, DateTime fUtlEjecucion, string id)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select EMPLEADO, ACTIVO, IDENTIFICACION, U_IDENTIFICACION_ANT IDENTIFICACION_ANT, NOMBRE, FECHA_INGRESO, FECHA_SALIDA, E_MAIL, PASAPORTE  from ");
            sentencia.Append(cia);
            sentencia.Append(".EMPLEADO ");
            sentencia.Append("WHERE ACTIVO = 'S' ");
            sentencia.Append("and  CONVERT(DATE, FECHA_INGRESO) >= '" + fUtlEjecucion.ToString("yyyy-MM-dd") + "'");

            if (id.CompareTo(string.Empty) != 0)
            {
                sentencia.Append(" and IDENTIFICACION  = '" + id + "'");
            }

            sentencia.Append(" order by EMPLEADO asc ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }


        /// <summary>
        /// Metodo encargado de obtener los empleados para sincronizar
        /// </summary>
        public DataTable getEmpleadosData(SqlTransaction transac, string cia, string id, string id_ant, int dias)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select e.EMPLEADO");
            sentencia.Append(", e.IDENTIFICACION");
            sentencia.Append(", e.U_IDENTIFICACION_ANT");
            sentencia.Append(", e.NOMBRE");
            sentencia.Append(", ecn.CONCEPTO");
            sentencia.Append(", c.DESCRIPCION");
            sentencia.Append(", ecn.NOMINA");
            sentencia.Append(", ecn.NUMERO_NOMINA");
            sentencia.Append(", ecn.CENTRO_COSTO");
            sentencia.Append(", ecn.CANTIDAD");
            sentencia.Append(", ecn.MONTO");
            sentencia.Append(", ecn.TOTAL");
            sentencia.Append(", n.FECHA_INICIO");
            sentencia.Append(", n.FECHA_FIN");
            sentencia.Append(", DATEDIFF(DAY, n.FECHA_INICIO, GETDATE()) DAYS");
            sentencia.Append(" from ");
            sentencia.Append(cia + ".EMPLEADO e inner join ");
            sentencia.Append(cia + ".EMPLEADO_CONC_NOMI ecn on e.EMPLEADO = ecn.EMPLEADO inner join  ");
            sentencia.Append(cia + ".NOMINA_HISTORICO n ON n.NOMINA = ecn.NOMINA and n.NUMERO_NOMINA = ecn.NUMERO_NOMINA inner join  ");
            sentencia.Append(cia + ".CONCEPTO c on c.CONCEPTO = ecn.CONCEPTO ");
            sentencia.Append("where e.ACTIVO = 'N'   ");
            sentencia.Append("and c.U_CONC_TRASLADO = 'Si' ");
            sentencia.Append("and (e.IDENTIFICACION = '" + id + "' or e.IDENTIFICACION = '" + id_ant + "') ");
            sentencia.Append("and DATEDIFF(DAY, n.FECHA_INICIO, GETDATE()) <= " + dias.ToString());

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }

    }
}
