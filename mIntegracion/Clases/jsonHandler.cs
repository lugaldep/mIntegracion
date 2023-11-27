using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mIntegracion.Clases
{
    class jsonHandler
    {
        public jsonHandler()
        {            
        }


        /// <summary>
        /// Metodo convert un DT to Json  
        /// </summary>
        public string DataTableToJson(DataTable table)
        {
            string jsonString = string.Empty;
            jsonString = JsonConvert.SerializeObject(table);
            return jsonString;
        }

        /// <summary>
        /// Metodo convert un dr to Json  
        /// </summary>
        public string DataRowToJson(DataTable dt, int row)
        {
            string json = new JObject(
                       dt.Columns.Cast<DataColumn>()
                       .Select(c => new JProperty(c.ColumnName, JToken.FromObject(dt.Rows[row][c])))
                       ).ToString(Formatting.None);
            return json;
        }

        /// <summary>
        /// Metodo convert un dr to Json  
        /// </summary>
        public string DataRowToJson(DataTable dt, int row, int cantRemove)
        {
            string json = new JObject(
                       dt.Columns.Cast<DataColumn>()
                       .Select(c => new JProperty(c.ColumnName, JToken.FromObject(dt.Rows[row][c])))
                       ).ToString(Formatting.None);
            return json.Substring(0,json.Length-cantRemove);
        }


        /// <summary>
        /// Metodo obtener el encabezado del json 
        /// </summary>
        public string getHeader(string ck, string cs, string type)
        {
            //string json = "{\"conexion\": {\"consumer_key\": \"cd8bcsgff8wccbjv9ue9c46dakc\",\"consumer_secret\": \"bz9ry3_cv58_0_b72tsyzdwe2gj7dfvr8bdctx6q4f\",\"type\":\"compania\",\"compania\": 	{		\"nombre\": \"Bahia San Felipe, S.A.\",	\"telefono\": \"22880101\",		\"nit\": \"3101371510\",		\"codigo\": \"BSF\",		\"imagnombre\": \"Logo_BSF.BMP\"	}}}";
            StringBuilder sent = new StringBuilder();

            sent.Append("{\"conexion\": {");
            sent.Append("\"consumer_key\": \""+ ck + "\", ");
            sent.Append("\"consumer_secret\": \""+ cs +"\",");
            sent.Append("\"type\":\""+ type +"\",\""+ type +"\": ");
            return (sent.ToString());
           
        }


        /// <summary>
        /// Metodo obtener el encabezado del json 
        /// </summary>
        public string getHeader(string ck, string cs, string type, string typeDoc)
        {
            //string json = "{\"conexion\": {\"consumer_key\": \"cd8bcsgff8wccbjv9ue9c46dakc\",\"consumer_secret\": \"bz9ry3_cv58_0_b72tsyzdwe2gj7dfvr8bdctx6q4f\",\"type\":\"compania\",\"compania\": 	{		\"nombre\": \"Bahia San Felipe, S.A.\",	\"telefono\": \"22880101\",		\"nit\": \"3101371510\",		\"codigo\": \"BSF\",		\"imagnombre\": \"Logo_BSF.BMP\"	}}}";
            StringBuilder sent = new StringBuilder();

            sent.Append("{\"conexion\": {");
            sent.Append("\"consumer_key\": \"" + ck + "\", ");
            sent.Append("\"consumer_secret\": \"" + cs + "\",");
            sent.Append("\"type\":\"" + type + "\",\"" + typeDoc + "\": ");
            return (sent.ToString());

        }


        /// <summary>
        /// Metodo obtener el encabezado del json 
        /// </summary>
        public string getFooter()
        {            
            return ("}}");
        }

        /// <summary>
        /// Metodo obtener el encabezado del json 
        /// </summary>
        public string getOpenArray()
        {
            return ("[");
        }

        /// <summary>
        /// Metodo obtener el encabezado del json 
        /// </summary>
        public string getCloseArray()
        {
            return ("]");
        }

        /// <summary>
        /// Metodo obtener el encabezado del json 
        /// </summary>
        public string getLines()
        {
            return (", \"lines\":  ");

        }


        /// <summary>
        /// Metodo obtener el encabezado del json 
        /// </summary>
        public string getCloseLines()
        {
            return (" } ,");

        }

    }
}
