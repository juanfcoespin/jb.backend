using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using SAPbobsCOM;

namespace jbp.core.sapDiApi
{
    public class SapTransferenciaStock:BaseSapObj
    {
        public SapTransferenciaStock()
        {
            //this.Connect();
        }
        public DocSapInsertadoMsg Add(TsBodegaMsg me)
        {
            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer= this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            stockTransfer.DocDate = DateTime.Now;
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
                    loteAsignado.UbicacionesCantidadDesde.ForEach(uc => { //si no se ha mandado ubicaciones solo hace transferencia entre bodegas
                        stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                        stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0; //0 porque por defecto coge el lote que se agrego antes
                        stockTransfer.Lines.BinAllocations.BinAbsEntry = uc.IdUbicacion;
                        stockTransfer.Lines.BinAllocations.Quantity = uc.Cantidad;
                        stockTransfer.Lines.BinAllocations.Add();
                    });
                    // se asigna la ubicación de destino si fue enviada en el mensaje
                    if (me.IdUbicacionHasta > 0) {
                        stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batToWarehouse;
                        stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                        stockTransfer.Lines.BinAllocations.BinAbsEntry = me.IdUbicacionHasta;
                        stockTransfer.Lines.BinAllocations.Quantity = loteAsignado.Cantidad;
                        stockTransfer.Lines.BinAllocations.Add();
                    }
                });
                stockTransfer.Lines.Add();
                
            });
            var error = stockTransfer.Add();
            if (error != 0)
                ms.Error = "Error: " + this.Company.GetLastErrorDescription();
            else
                ms.Id = this.Company.GetNewObjectKey();
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
        ~SapTransferenciaStock()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
