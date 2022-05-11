using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using SAPbobsCOM;

namespace jbp.core.sapDiApi
{
    public class SapEntradaMercancia:BaseSapObj
    {
        public SapEntradaMercancia()
        {
            //this.Connect();
        }
        public string Add(EntradaMercanciaMsg me)
        {
            var entradaMercancia= this.Company.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
            var ms = "ok";
            entradaMercancia.DocDate = DateTime.Now;
            entradaMercancia.CardCode = me.CodProveedor;
            me.Lineas.ForEach(line =>
            {
                entradaMercancia.Lines.ItemCode = line.CodArticulo;
                entradaMercancia.Lines.Quantity = line.Cantidad;
                entradaMercancia.Lines.WarehouseCode = me.CodBodega;

                //line.Lotes.ForEach(loteAsignado =>
                //{
                //    stockTransfer.Lines.BatchNumbers.BatchNumber = loteAsignado.Lote;
                //    stockTransfer.Lines.BatchNumbers.Quantity = loteAsignado.Cantidad;
                //    stockTransfer.Lines.BatchNumbers.Add();
                //});
                entradaMercancia.Lines.Add();
                
            });
            var error = entradaMercancia.Add();
            if (error != 0)
            {
                ms = "Error: " + this.Company.GetLastErrorDescription();
            }
            return ms;
        }
        
        ~SapEntradaMercancia()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
