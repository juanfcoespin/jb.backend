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
        void Connect();
        void Disconect();
        void Execute(string sql);
    }
}
