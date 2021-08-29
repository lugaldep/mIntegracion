using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace mIntegracion.Clases
{
    class Seguridad
    {
        private SQL sqlClass;

        public Seguridad(SQL _sqlClass)
        {
            this.sqlClass = _sqlClass;
        }

        public bool getAcceso(int accion, string cia)
        {
            bool ret = false;

            //es usuario 'SA'
            if (sqlClass.Usuario.ToUpper().CompareTo("SA") != 0)
            {
                //valida si es administrador
                if (!getUsuarioAdministrador())
                {
                    //valida la accion
                    if (!getAccesoAccion(accion, cia))
                    {
                        //valida la membresia
                        if (!getAccesoMembresia(accion, cia))
                            ret = false;
                        else
                            ret = true;
                    }
                    else
                        ret = true;
                }
                else
                    ret = true;
            }
            else
                ret = true;

            return (ret);
        }



        /// <summary>
        /// Obtener acceso a la opcion    
        /// </summary>
        public bool getAccesoAccion(int accion, string cia)
        {
            bool ret = false;
            int q = 0;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select COUNT(1) from ");
            sentencia.Append("ERPADMIN.PRIVILEGIO_EX  ");
            sentencia.Append("where USUARIO = '");
            sentencia.Append(sqlClass.Usuario);
            sentencia.Append("' and CONJUNTO ='");
            sentencia.Append(cia);
            sentencia.Append("' and ACCION =");
            sentencia.Append(accion);
            sentencia.Append(" ");

            q = (int)sqlClass.EjecutarScalar(sentencia.ToString());

            if (q > 0)
                ret = true;
            else
                ret = false;

            return (ret);
        }


        /// <summary>
        /// Obtener acceso a la opcion    
        /// </summary>
        public bool getAccesoAccion(int accion, string cia, string usuario)
        {
            bool ret = false;
            int q = 0;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select COUNT(1) from ");
            sentencia.Append("ERPADMIN.PRIVILEGIO_EX  ");
            sentencia.Append("where USUARIO = '");
            sentencia.Append(usuario);
            sentencia.Append("' and CONJUNTO ='");
            sentencia.Append(cia);
            sentencia.Append("' and ACCION =");
            sentencia.Append(accion);
            sentencia.Append(" ");

            q = (int)sqlClass.EjecutarScalar(sentencia.ToString());

            if (q > 0)
                ret = true;
            else
                ret = false;

            return (ret);
        }

        /// <summary>
        /// Obtener acceso a la opcion    
        /// </summary>
        public bool getUsuarioAdministrador()
        {
            //Usuario administrador constante de Exactus
            int accion = 4;
            bool ret = false;
            int q = 0;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select COUNT(1) from ");
            sentencia.Append("ERPADMIN.PRIVILEGIO_EX  ");
            sentencia.Append("where USUARIO = '");
            sentencia.Append(sqlClass.Usuario);
            sentencia.Append("' and CONJUNTO ='");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append("' and ACCION =");
            sentencia.Append(accion);
            sentencia.Append(" ");

            q = (int)sqlClass.EjecutarScalar(sentencia.ToString());

            if (q > 0)
                ret = true;
            else
                ret = false;

            return (ret);
        }


        /// <summary>
        /// Obtener acceso a la opcion    
        /// </summary>
        public bool getAccesoMembresia(int accion, string cia)
        {
            bool ret = false;
            DataTable dt;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select GRUPO from ");
            sentencia.Append("ERPADMIN.MEMBRESIA ");
            sentencia.Append("where USUARIO = '");
            sentencia.Append(sqlClass.Usuario);
            sentencia.Append("' ");

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            foreach (DataRow dr in dt.Rows)
            {
                ret = getAccesoAccion(accion, cia, dr["GRUPO"].ToString());

                if (ret)
                    break;
            }

            return (ret);
        }


        /// <summary>
        /// Metodo valida si el usuario conectado es supervisor    
        /// </summary>
        public bool esSupervisor(string usuario)
        {
            int q = 0;
            StringBuilder sentencia = new StringBuilder();
            sentencia.Append("SELECT COUNT(1) FROM ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".SUPERVISOR ");
            sentencia.Append("WHERE SUPERVISOR = '");
            sentencia.Append(usuario);
            sentencia.Append("'");
            q = int.Parse(sqlClass.EjecutarScalar(sentencia.ToString()).ToString());
            return (q > 0);
        }

        /// <summary>
        /// Obtener el listado de usuarios      
        /// </summary>
        public DataTable getUsuarios()
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select USUARIO, USUARIO + ' - ' + NOMBRE DESCRIPCION from ");
            sentencia.Append("ERPADMIN.USUARIO ");
            sentencia.Append("where TIPO = 'U' and ACTIVO = 'S' ");
            sentencia.Append("order by USUARIO asc ");
            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }

        /// <summary>
        /// Obtener el listado de usuarios      
        /// </summary>
        public DataTable getUsuariosSupervisor(SqlTransaction transac)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select u.USUARIO, u.USUARIO + ' - ' + u.NOMBRE DESCRIPCION from ");
            sentencia.Append("ERPADMIN.USUARIO u inner join ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".SUPERVISOR s on u.USUARIO = s.SUPERVISOR ");
            sentencia.Append("where u.TIPO = 'U' and u.ACTIVO = 'S' ");
            sentencia.Append("order by u.USUARIO asc ");
            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString(), transac);

            return dt;
        }
    }
}


