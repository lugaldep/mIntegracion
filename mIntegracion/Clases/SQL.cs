using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace mIntegracion.Clases
{
    public class SQL
    {
        private SqlConnection sqlCon;
        private SqlCommand sqlCmd;
        private SqlDataAdapter sqlAdapter;
        SqlConnectionStringBuilder sqlConStr;
        private string cia;
        private string user;
        private string server;
        private string database;

        public SqlConnection SQLCon
        {
            get { return sqlCon; }
        }

        public SQL(string user, string pass, string dataBase, string server, string cia)
        {
            sqlConStr = new SqlConnectionStringBuilder();
            sqlConStr.UserID = user;
            sqlConStr.Password = pass;
            sqlConStr.InitialCatalog = dataBase;
            sqlConStr.DataSource = server;

            sqlCon = new SqlConnection(sqlConStr.ToString());
            sqlCon.Open();

            this.cia = cia;
            this.user = user;
            this.server = server;
            this.database = dataBase;
        }

        public string Compannia
        {
            get { return this.cia; }
        }

        public string Usuario
        {
            get { return this.user; }
        }

        public string Servidor
        {
            get { return this.server; }
        }

        public string BaseDatos
        {
            get { return this.database; }
        }

        /// <summary>
        /// Ejecuta la sentencia dada por parámetro
        /// </summary>
        /// <param name="sentencia">sentencia SQL</param>
        /// <returns>DataReader con los resultados de la sentencia</returns>
        public SqlDataReader EjecutarConsulta(string sentencia)
        {
            SqlDataReader dtReader = null;
            try
            {
                if (sqlCon.State != ConnectionState.Open) sqlCon.Open();
                this.sqlCmd = new SqlCommand(sentencia, this.sqlCon);
                dtReader = this.sqlCmd.ExecuteReader();
            }
            finally
            {
                this.sqlCmd.Dispose();
                this.sqlCmd = null;
            }

            return dtReader;
        }

        public SqlDataReader EjecutarConsulta(string sentencia, SqlTransaction transac)
        {
            SqlDataReader dtReader = null;
            try
            {
                if (sqlCon.State != ConnectionState.Open) sqlCon.Open();
                this.sqlCmd = new SqlCommand(sentencia, this.sqlCon, transac);
                dtReader = this.sqlCmd.ExecuteReader();
            }
            finally
            {
                this.sqlCmd.Dispose();
                this.sqlCmd = null;
            }

            return dtReader;
        }

        public object EjecutarScalar(string sentencia)
        {
            object result;
            try
            {
                if (sqlCon.State != ConnectionState.Open) sqlCon.Open();
                this.sqlCmd = new SqlCommand(sentencia, this.sqlCon);
                result = this.sqlCmd.ExecuteScalar();
            }
            finally
            {
                this.sqlCmd.Dispose();
                this.sqlCmd = null;
            }

            return result;
        }

        public object EjecutarScalar(string sentencia, SqlTransaction transac)
        {
            object result;
            try
            {
                if (sqlCon.State != ConnectionState.Open) sqlCon.Open();
                this.sqlCmd = new SqlCommand(sentencia, this.sqlCon, transac);
                result = this.sqlCmd.ExecuteScalar();
            }
            finally
            {
                this.sqlCmd.Dispose();
                this.sqlCmd = null;
            }

            return result;
        }

        /// <summary>
        /// Ejecuta la sentencia dada por parámetro
        /// </summary>
        /// <param name="sentencia">sentencia SQL</param>
        /// <returns>DataTable con los resultados de la sentencia</returns>
        public DataTable EjecutarConsultaDS(string sentencia)
        {
            DataSet dsDatos = new DataSet();
            try
            {
                sqlAdapter = new SqlDataAdapter(sentencia, this.sqlCon);                
                    sqlAdapter.Fill(dsDatos);
            }
            finally
            {
                sqlAdapter.Dispose();
                sqlAdapter = null;
            }


            return (dsDatos != null && dsDatos.Tables.Count > 0) ? dsDatos.Tables[0] : null;
        }


        public DataTable EjecutarConsultaDS(string sentencia, SqlTransaction transac)
        {
            DataSet dsDatos = new DataSet();
            try
            {
                sqlAdapter = new SqlDataAdapter(sentencia, this.sqlCon);
                sqlAdapter.SelectCommand.Transaction = transac;
                sqlAdapter.Fill(dsDatos);
            }
            finally
            {
                sqlAdapter.Dispose();
                sqlAdapter = null;
            }


            return (dsDatos != null && dsDatos.Tables.Count > 0) ? dsDatos.Tables[0] : null;
        }

        /// <summary>
        /// Ejecuta la sentencia UPDATE/INSERT/DELETE dada por parámetro
        /// </summary>
        /// <returns>Cantidad de registros afectados</returns>
        public int EjecutarUpdate(string sentencia)
        {
            int cantRowUpt = 0;

            try
            {
                if (sqlCon.State != ConnectionState.Open) sqlCon.Open();
                this.sqlCmd = new SqlCommand(sentencia, this.sqlCon);
                cantRowUpt = this.sqlCmd.ExecuteNonQuery();
            }
            finally
            {
                this.sqlCmd.Dispose();
                this.sqlCmd = null;

            }

            return cantRowUpt;

        }

        public int EjecutarUpdate(string sentencia, SqlTransaction transac)
        {
            int cantRowUpt = 0;

            try
            {
                if (sqlCon.State != ConnectionState.Open) sqlCon.Open();
                this.sqlCmd = new SqlCommand(sentencia, this.sqlCon);
                this.sqlCmd.Transaction = transac;
                cantRowUpt = this.sqlCmd.ExecuteNonQuery();
            }
            finally
            {
                this.sqlCmd.Dispose();
                this.sqlCmd = null;
            }

            return cantRowUpt;

        }
    }
}