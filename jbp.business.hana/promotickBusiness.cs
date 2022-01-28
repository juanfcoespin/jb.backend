using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;
using jbp.msg;
using System.Data;

namespace jbp.business.hana
{
    public class promotickBusiness
    {
        public List<AceleradoresMsg> GetAceleradoresToSend()
        {
            var ms = new List<AceleradoresMsg>();
            /*
             en este store procedure configurar los meses y los articulos participantes
             */
            var sql = string.Format(@"
                call ""JbpSp_Aceleradores""()
            ");
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if(dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var acelerador = new AceleradoresMsg();
                    acelerador.nroDocumento = dr["nroDocumento"].ToString();
                    acelerador.puntos = bc.GetInt(dr["puntos"]);
                    ms.Add(acelerador);
                }
            }
            return ms;
        }
    }
}
