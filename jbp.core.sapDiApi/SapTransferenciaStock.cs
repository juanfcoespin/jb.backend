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
                var cantLinea = 0.0;
                stockTransfer.Lines.ItemCode = line.CodArticulo;
                stockTransfer.Lines.FromWarehouseCode = me.CodBodegaDesde;
                stockTransfer.Lines.WarehouseCode = me.CodBodegaHasta;
                line.Lotes.ForEach(lote => {
                    cantLinea += lote.Cantidad;
                    stockTransfer.Lines.BatchNumbers.BatchNumber = lote.Lote;
                    stockTransfer.Lines.BatchNumbers.Quantity = lote.Cantidad;
                    stockTransfer.Lines.BatchNumbers.Add();
                });
                stockTransfer.Lines.Quantity = cantLinea;
                stockTransfer.Lines.Add();

            });
            var error = stockTransfer.Add();
            if (error != 0)
                ms.Error = "Error: " + this.Company.GetLastErrorDescription();
            else
                ms.Id = this.Company.GetNewObjectKey();
            return ms;
        }

        /*public DocSapInsertadoMsg AddFromSt2(TsFromPickingME me)
        {
            var ms = new DocSapInsertadoMsg();
            StockTransfer oTransferReq = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryTransferRequest);
            if (oTransferReq.GetByKey(me.Id)) //Load a transfer request
            {
                //initialize a stock transfer
                StockTransfer oStTransfer = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);
                oStTransfer.DocDate = DateTime.Now;

                //Read lines from Transfer Request
                for (int i = 0; i < oTransferReq.Lines.Count; i++)
                {
                    oTransferReq.Lines.SetCurrentLine(i);
                    if (oStTransfer.Lines.BaseEntry != 0)
                        oStTransfer.Lines.Add();
                    //Set the reference for the transfer request, sap will copy the other infos automatically.
                    oStTransfer.Lines.BaseEntry = oTransferReq.DocEntry;
                    oStTransfer.Lines.BaseLine = oTransferReq.Lines.LineNum;
                    oStTransfer.Lines.BaseType = InvBaseDocTypeEnum.InventoryTransferRequest;

                    //lotes
                    oStTransfer.Lines.BatchNumbers.BatchNumber = oTransferReq.Lines.BatchNumbers.BatchNumber;
                    oStTransfer.Lines.BatchNumbers.Quantity = oTransferReq.Lines.BatchNumbers.Quantity;
                    oStTransfer.Lines.BatchNumbers.Add();
                }
                var error = oStTransfer.Add();
                if (error != 0)
                    ms.Error = "Error: " + this.Company.GetLastErrorDescription();
                else
                    ms.Id = this.Company.GetNewObjectKey();
            }
            return ms;
        }*/
        public DocSapInsertadoMsg AddFromSt(TsFromPickingME me)
        {
            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer = this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            stockTransfer.FromWarehouse = me.BodegaOrigen;
            stockTransfer.ToWarehouse = me.BodegaDestino;
            stockTransfer.DocDate = DateTime.Now; 
            me.Componentes.ForEach(line =>
            {
                stockTransfer.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest; // Solicitud de transferencia
                stockTransfer.Lines.BaseEntry = me.Id;
                stockTransfer.Lines.BaseLine = line.LineNum;
                stockTransfer.Lines.FromWarehouseCode = line.BodegaOrigen;
                stockTransfer.Lines.WarehouseCode = line.BodegaDestino;
                stockTransfer.Lines.Quantity = line.Cantidad;
                
                //lotes
                stockTransfer.Lines.BatchNumbers.BatchNumber = line.Lote;
                stockTransfer.Lines.BatchNumbers.Quantity = line.Cantidad;
                stockTransfer.Lines.BatchNumbers.Add();
                

                //ubicacion desde
                if (line.IdUbicacion > 0)
                {
                    stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                    stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                    stockTransfer.Lines.BinAllocations.BinAbsEntry = line.IdUbicacion;
                    stockTransfer.Lines.BinAllocations.Quantity = line.Cantidad;
                    stockTransfer.Lines.BinAllocations.Add();
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
