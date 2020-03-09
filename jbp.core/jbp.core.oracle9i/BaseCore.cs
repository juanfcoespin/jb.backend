using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TechTools.Exceptions;
using System.Data.OracleClient;
using System.Data;

namespace jbp.core
{
    public class BaseCore
    {
        public OracleConnection DbConnection;

        public void Connect()
        {
            //var tipoCpu = CPU.GetCpuType();
            DbConnection = new OracleConnection();
            DbConnection.ConnectionString = Variables.Default.bddStringConnection;
            DbConnection.Open();
        }

        public void Disconect()
        {
            DbConnection.Close();
            DbConnection.Dispose();
        }
        /// <summary>
        /// Para sentencias sql tipo select
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        
        public DataTable GetDataTableByQuery(string sql)
        {
            try
            {
                Connect();
                var dt = new DataTable();
                var da = new OracleDataAdapter(sql, DbConnection);
                da.Fill(dt);
                da.Dispose();
                Disconect();
                return dt;
            }
            catch (Exception ex)
            {
                Disconect();
                ex = ExceptionManager.GetDeepErrorMessage(ex, ExceptionManager.eCapa.Core);
                throw new Exception(string.Format("{0}\r\nSql:\r\n{1}", ex.Message, sql));
            }
        }
        public int GetIntScalarByQuery(string sql)
        {
            try
            {
                var ms = GetScalarByQuery(sql);
                return ms != null ? Convert.ToInt32(ms) : 0;
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Core);
                throw new Exception( string.Format("{0}\r\nSql:\r\n{1}", e.Message,sql));
            }
            
        }
        public string GetScalarByQuery(string sql)
        {
            try
            {
                Connect();
                var dt = new DataTable();
                var da = new OracleDataAdapter(sql, DbConnection);
                da.Fill(dt);
                da.Dispose();
                Disconect();
                return (dt.Rows.Count == 0 ? null : dt.Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                Disconect();
                ex = ExceptionManager.GetDeepErrorMessage(ex, ExceptionManager.eCapa.Core);
                throw new Exception(string.Format("{0}\r\nSql:\r\n{1}", ex.Message, sql));
            }
        }
        /// <summary>
        /// Para sentencias sql como insert, update, delete
        /// </summary>
        /// <param name="sql"></param>
        
        public void Execute(string sql)
        {
            try
            {
                sql = ReeplaceToAscci(sql);
                Connect();
                var command = new OracleCommand(sql, this.DbConnection);
                command.ExecuteReader();
                command.Dispose();
                Disconect();
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Core);
                throw new Exception(string.Format("{0}\r\nSql:\r\n{1}", e.Message, sql));
            }

        }

        /// <summary>
        /// reemplaza los caracteres que se espesifiquen en el archivo de configuración
        /// por sus respectivos ascci
        /// </summary>
        /// <param name="sql">Ej; la Ñ por chr(209)</param>
        /// <returns></returns>
        private string ReeplaceToAscci(string sql)
        {
            //Ñ,209;Á,193;É,201;Í,205;Ó,211;Ú,218;ñ,241;á,225;é,233;í,237;ó,243;ú,250
            var matriz = Variables.Default.acciReplace.Split(new char[] { ';' });
            if (matriz != null) {
                foreach (string vector in matriz) {
                    var token = vector.Split(new char[] { ','});
                    if (sql.Contains(token[0])) {
                        // sql = sql.Replace("Ñ", "'||chr(209)||'");
                        sql = sql.Replace(token[0], string.Format("'||chr({0})||'", token[1]));
                    }
                }
            }
            return sql;
        }

        public DateTime GetDateTime(object me) {
            if (DBNull.Value.Equals(me)) {
                return DateTime.MinValue;
            }
            return Convert.ToDateTime(me);
        }
        public bool GetBoolean(object me)
        {
            if (DBNull.Value.Equals(me))
            {
                return false;
            }
            return Convert.ToBoolean(me);
        }
        public Int32 GetInt(object me)
        {
            if (DBNull.Value.Equals(me))
            {
                return 0;
            }
            return Convert.ToInt32(me);
        }
        public decimal GetDecimal(object me)
        {
            if (DBNull.Value.Equals(me))
            {
                return 0;
            }
            var ms= Convert.ToDecimal(me);
            return decimal.Round(ms, 2);
        }
        /// <summary>
        /// Ej: token: "juan francisco espin"
        /// campos: {"ruc", "nombre", "conocidoComo"}
        /// putAndFirst: true
        /// 
        /// Resultado:
        ///and
        ///(
        /// (lower(nombre) like '%espin%' and lower(nombre) like '%juan%' and lower(nombre) like '%francisco%') or
        /// (lower(ruc) like '%espin%' and lower(ruc) like '%juan%' and lower(ruc) like '%francisco%') or
        /// (lower(CONOCIDOCOMO) like '%espin%' and lower(CONOCIDOCOMO) like '%juan%' and lower(CONOCIDOCOMO) like '%francisco%')
        ///)
        /// </summary>
        /// <param name="putAndFist">pone and al inicio si existe una condición where previa</param>
        /// <param name="campos">listado de campos en el query a evaluar</param>
        /// <param name="token">el valor registrado por el usuario para la búsqueda</param>
        /// <returns></returns>
        public string GetSearchCondition(bool putAndFist, List<string> campos, string token) {
            token = token.ToLower();
            var ms = putAndFist?" and (": string.Empty;
            var matrixToken = token.Split(new char[] {' '});
            var i = 0;
            var j = 0;
            campos.ForEach(campo=> {
                if (i > 0)
                    ms += " or ";
                ms += "(";
                foreach (var miniToken in matrixToken)
                {
                    if (!string.IsNullOrEmpty(miniToken)) {
                        if (j > 0)
                            ms += " and ";
                        ms += string.Format(" lower({0}) like '%{1}%' ",campo,miniToken);
                        j++;
                    }
                    
                }
                ms += ")";
                i++;
                j = 0;
            });
            ms += ")";
            
            return ms;
        }
    }
}
