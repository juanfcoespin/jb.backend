using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using jbp.msg.sap;
using SAPbobsCOM;

namespace jbp.core.sapDiApi
{
    public class SapSolicitudTransferencia : BaseSapObj
    {
        public SapSolicitudTransferencia()
        {
            //this.Connect();
        }

        public DocSapInsertadoMsg AddST(StMsg me)
        {
            var ms = new DocSapInsertadoMsg();
            Monitor.Enter(this);
            StockTransfer stockTransfer = this.Company.GetBusinessObject(BoObjectTypes.oInventoryTransferRequest);
            //stockTransfer.DocObjectCode = SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest;
            stockTransfer.FromWarehouse = me.BodegaOrigen;
            stockTransfer.ToWarehouse = me.BodegaDestino;
            stockTransfer.DocDate = DateTime.Now;
            stockTransfer.JournalMemo = me.DocNumOF;
            stockTransfer.Comments = me.Comentarios;

            if (conf.Default.NroSerieTSPorDefecto > 0)
            {
                stockTransfer.Series = 110; //ST_HUM
            }
            me.Lines.ForEach(line =>
            {
                if (line.Cantidad > 0) {
                    stockTransfer.Lines.ItemCode = line.CodArticulo;
                    stockTransfer.Lines.FromWarehouseCode = line.BodegaOrigen;
                    stockTransfer.Lines.WarehouseCode = line.BodegaDestino;
                    stockTransfer.Lines.Quantity = line.Cantidad;

                    //lotes
                    line.Lotes.ForEach(lote =>
                    {
                        stockTransfer.Lines.BatchNumbers.BatchNumber = lote.Lote;
                        stockTransfer.Lines.BatchNumbers.Quantity = lote.Cantidad;
                        stockTransfer.Lines.BatchNumbers.Add();
                    });
                    stockTransfer.Lines.Add();
                }
            });
            var error = stockTransfer.Add();
            Monitor.Exit(this); 
            if (error != 0)
                ms.Error = this.Company.GetLastErrorDescription();
            else
                ms.Id = this.Company.GetNewObjectKey();
            return ms;
        }
        
        ~SapSolicitudTransferencia()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
