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
            StockTransfer stockTransfer= this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            var ms = "ok";
            stockTransfer.DocDate = DateTime.Now;
            if(me.Serie>0)
                stockTransfer.Series = me.Serie;
            stockTransfer.FromWarehouse = me.CodBodegaDesde;
            stockTransfer.ToWarehouse = me.CodBodegaHasta;

            me.Lineas.ForEach(line =>
            {
                stockTransfer.Lines.ItemCode = line.CodArticulo;
                stockTransfer.Lines.Quantity = line.Cantidad;
                stockTransfer.Lines.FromWarehouseCode = me.CodBodegaDesde;
                stockTransfer.Lines.WarehouseCode = me.CodBodegaHasta;
                
                line.Lotes.ForEach(loteAsignado =>
                {
                    stockTransfer.Lines.BatchNumbers.BatchNumber = loteAsignado.Lote;
                    stockTransfer.Lines.BatchNumbers.Quantity = loteAsignado.Cantidad;
                    stockTransfer.Lines.BatchNumbers.Add();
                });
                stockTransfer.Lines.Add();
                
            });
            var error = stockTransfer.Add();
            if (error != 0)
            {
                ms = "Error: " + this.Company.GetLastErrorDescription();
            }
            return ms;
        }
        public string AddFromSt(SalidaBodegaMsg me)
        {
            this.obj = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer); 
            var ms = "ok";
            this.obj.DocDueDate = DateTime.Now; 
            
            
            me.Lineas.ForEach(line =>
            {
                if (me.DocBaseType == EDocBase.SolicitudTransferencia) {
                    this.obj.Lines.BaseType = (int)SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest; // Solicitud de transferencia
                    this.obj.Lines.BaseEntry = me.IdDocBase;
                    this.obj.Lines.BaseLine = line.LineNum;
                    this.obj.Lines.Quantity = line.Cantidad;
                    //lotes
                    this.obj.Lines.BatchNumbers.BatchNumber = line.Lote;
                    this.obj.Lines.BatchNumbers.Quantity = line.Cantidad;
                    this.obj.Lines.Add();
                }
            });
            var error = this.obj.Add();
            if (error != 0)
            {
                ms= "Error: "+this.Company.GetLastErrorDescription();
            }
            return ms;
        }
        public OrdenMsg GetById(int id)
        {
            this.obj.GetByKey(id);
            var ms = new OrdenMsg();
            ms.Id = this.obj.DocEntry;
            ms.CodCliente = this.obj.CardCode;
            ms.Cliente = this.obj.CardName;
            return ms;
        }
        ~SapEntradaMercancia()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
