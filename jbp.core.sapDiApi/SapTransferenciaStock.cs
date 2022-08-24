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

        //Transferencia entre ubicaciones
        public DocSapInsertadoMsg TranferUbicaciones(TsBodegaMsg me)
        {
            /*
             Un lote de producto que se distribuye en varias ubicaciones
             */

            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer= this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            stockTransfer.DocDate = DateTime.Now;
            stockTransfer.FromWarehouse = me.CodBodegaDesde;
            stockTransfer.ToWarehouse = me.CodBodegaHasta;
            //solo es una linea con n ubicaciones y un solo lote
            stockTransfer.Lines.ItemCode = me.CodArticulo;
            stockTransfer.Lines.Quantity = me.CantidadTotal;
            stockTransfer.Lines.FromWarehouseCode = me.CodBodegaDesde;
            stockTransfer.Lines.WarehouseCode = me.CodBodegaHasta;

            //es un solo lote
            stockTransfer.Lines.BatchNumbers.BatchNumber = me.Lote;
            stockTransfer.Lines.BatchNumbers.Quantity = me.CantidadTotal;
            stockTransfer.Lines.BatchNumbers.Add();
            
            //ubicacion desde
            if (me.IdUbicacionDesde > 0)
            {
                stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                stockTransfer.Lines.BinAllocations.BinAbsEntry = me.IdUbicacionDesde;
                stockTransfer.Lines.BinAllocations.Quantity = me.CantidadTotal;
                stockTransfer.Lines.BinAllocations.Add();
            }
            //varias ubicaciones destino
            me.UbicacionesCantidadHasta.ForEach(uch => { //si no se ha mandado ubicaciones solo hace transferencia entre bodegas
                stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batToWarehouse; 
                stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0; //0 porque por defecto coge el lote que se agrego antes
                stockTransfer.Lines.BinAllocations.BinAbsEntry = uch.IdUbicacion;
                stockTransfer.Lines.BinAllocations.Quantity = uch.Cantidad;
                stockTransfer.Lines.BinAllocations.Add();
            });
           
            stockTransfer.Lines.Add();
                
            var error = stockTransfer.Add();
            if (error != 0)
                ms.Error = "Error: " + this.Company.GetLastErrorDescription();
            else
                ms.Id = this.Company.GetNewObjectKey();
            return ms;
        }

        //proyecto Espinosa Paez
        public DocSapInsertadoMsg AddFromBalazas(TsBalanzasMsg me)
        {
            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer = this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            stockTransfer.DocDate = DateTime.Now;
            stockTransfer.FromWarehouse = me.CodBodegaDesde;
            stockTransfer.ToWarehouse = me.CodBodegaHasta;
            me.Lineas.ForEach(line =>
            {
                stockTransfer.Lines.ItemCode = line.CodArticulo;
                stockTransfer.Lines.Quantity = line.Cantidad;
                stockTransfer.Lines.FromWarehouseCode = me.CodBodegaDesde;
                stockTransfer.Lines.WarehouseCode = me.CodBodegaHasta;
                if (!string.IsNullOrEmpty(line.Lote)) {
                    stockTransfer.Lines.BatchNumbers.BatchNumber = line.Lote;
                    stockTransfer.Lines.BatchNumbers.Quantity = line.Cantidad;
                    stockTransfer.Lines.BatchNumbers.Add();
                }
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
