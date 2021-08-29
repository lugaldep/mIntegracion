using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Collections;

namespace mIntegracion.Clases
{
    class F1
    {
        private SQL sqlClass;
        //variables de contexto
        private string tbl;
        private string id;
        private string desc;
        private string cond;
        private string camp1;
        private string camp2;
        private string compania;        
        public F1(SQL _sqlClass, string tabla, string codigo, string descripcion, string condAdic)
        {
            this.sqlClass = _sqlClass;
            this.tbl = tabla;
            this.id = codigo.ToUpper();
            this.desc = descripcion.ToUpper();
            this.cond = condAdic.ToUpper();

        }

        public F1(SQL _sqlClass, string cia, string tabla, string codigo, string descripcion, string condAdic)
        {
            this.sqlClass = _sqlClass;
            this.tbl = tabla;
            this.id = codigo.ToUpper();
            this.desc = descripcion.ToUpper();
            this.cond = condAdic.ToUpper();
            this.compania = cia;
        }

        public F1(SQL _sqlClass, string tabla, string codigo, string descripcion, string campo1, string campo2, string condAdic)
        {
            this.sqlClass = _sqlClass;
            this.tbl = tabla;
            this.id = codigo.ToUpper();
            this.desc = descripcion.ToUpper();
            this.cond = condAdic.ToUpper();
            this.camp1 = campo1;
            this.camp2 = campo2;
        }

        /// <summary>
        /// Metodo encargado de extraer los clientes desde Exactus       
        /// </summary>
        public DataTable getDatos(string value, bool modo)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append(this.id);
            sentencia.Append(",");
            sentencia.Append(this.desc);
            sentencia.Append(" from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".");
            sentencia.Append(this.tbl);

            //consultado desde el codigo
            if (modo)
            {
                sentencia.Append(" where ");
                sentencia.Append(this.id);
                sentencia.Append(" like '");
                sentencia.Append(value.ToUpper());
                sentencia.Append("'");

                if (this.cond.CompareTo(string.Empty) != 0)
                {
                    sentencia.Append(this.cond);
                }

                sentencia.Append(" order by ");
                sentencia.Append(this.id);
                sentencia.Append(" asc ");
            }
            //consultado desde la descripcion
            else
            {
                sentencia.Append(" where ");
                sentencia.Append(this.desc);
                sentencia.Append(" like '");
                sentencia.Append(value.ToUpper());
                sentencia.Append("'");

                if (this.cond.CompareTo(string.Empty) != 0)
                {
                    sentencia.Append(this.cond);
                }

                sentencia.Append(" order by ");
                sentencia.Append(this.desc);
                sentencia.Append(" asc ");
            }

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }


        /// <summary>
        /// Metodo encargado de extraer     
        /// </summary>
        public DataTable getDatosCia(string value, bool modo)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append(this.id);
            sentencia.Append(",");
            sentencia.Append(this.desc);
            sentencia.Append(" from ");
            sentencia.Append(this.compania);
            sentencia.Append(".");
            sentencia.Append(this.tbl);

            //consultado desde el codigo
            if (modo)
            {
                sentencia.Append(" where ");
                sentencia.Append(this.id);
                sentencia.Append(" like '");
                sentencia.Append(value.ToUpper());
                sentencia.Append("'");

                if (this.cond.CompareTo(string.Empty) != 0)
                {
                    sentencia.Append(this.cond);
                }

                sentencia.Append(" order by ");
                sentencia.Append(this.id);
                sentencia.Append(" asc ");
            }
            //consultado desde la descripcion
            else
            {
                sentencia.Append(" where ");
                sentencia.Append(this.desc);
                sentencia.Append(" like '");
                sentencia.Append(value.ToUpper());
                sentencia.Append("'");

                if (this.cond.CompareTo(string.Empty) != 0)
                {
                    sentencia.Append(this.cond);
                }

                sentencia.Append(" order by ");
                sentencia.Append(this.desc);
                sentencia.Append(" asc ");
            }

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }


        /// <summary>
        /// Metodo encargado de extraer       
        /// </summary>
        public DataTable getDatosAdic(string value, bool modo)
        {
            DataTable dt = null;
            StringBuilder sentencia = new StringBuilder();

            sentencia.Append("select ");
            sentencia.Append(this.id);
            sentencia.Append(",");
            sentencia.Append(this.desc);
            sentencia.Append(",");
            sentencia.Append(this.camp1);
            sentencia.Append(",");
            sentencia.Append(this.camp2);
            sentencia.Append(" from ");
            sentencia.Append(sqlClass.Compannia);
            sentencia.Append(".");
            sentencia.Append(this.tbl);

            //consultado desde el codigo
            if (modo)
            {
                sentencia.Append(" where ");
                sentencia.Append(this.id);
                sentencia.Append(" like '");
                sentencia.Append(value.ToUpper());
                sentencia.Append("'");

                if (this.cond.CompareTo(string.Empty) != 0)
                {
                    sentencia.Append(this.cond);
                }

                sentencia.Append(" order by ");
                sentencia.Append(this.id);
                sentencia.Append(" asc ");
            }
            //consultado desde la descripcion
            else
            {
                sentencia.Append(" where ");
                sentencia.Append(this.desc);
                sentencia.Append(" like '");
                sentencia.Append(value.ToUpper());
                sentencia.Append("'");

                if (this.cond.CompareTo(string.Empty) != 0)
                {
                    sentencia.Append(this.cond);
                }

                sentencia.Append(" order by ");
                sentencia.Append(this.desc);
                sentencia.Append(" asc ");
            }

            dt = sqlClass.EjecutarConsultaDS(sentencia.ToString());

            return dt;
        }
    }
}