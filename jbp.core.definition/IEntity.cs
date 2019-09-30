using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.core.contract
{
    public interface IEntity<T>
    {
        
        List<T> GetListByQuery(string sql);
        /// <summary>
        /// Conecta al motor de base de datos
        /// </summary>
        void Connect();
        /// <summary>
        /// DesConecta del motor de base de datos
        /// </summary>
        void Disconect();
        /// <summary>
        /// Sentencias del timpo DML (crud)
        /// </summary>
        /// <param name="sql">Query a la base</param>
        void Execute(string sql);
    }
}
