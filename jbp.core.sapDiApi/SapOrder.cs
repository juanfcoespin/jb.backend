using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace jbp.core.sapDiApi
{
    public class SapOrder:BaseSapObj
    {
        SapOrder()
        {
            this.obj = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
        }
        public void AddOrder()
        {

        }
    }
}
