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
        public DocSapInsertadoMsg TranferirEntreUbicaciones(TsBodegaMsg me)
        {
            /*
             Un lote de producto que se distribuye en varias ubicaciones
             */

            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer= this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            stockTransfer.DocDate = DateTime.Now;
            stockTransfer.FromWarehouse = me.CodBodegaDesde;
            stockTransfer.ToWarehouse = me.CodBodegaHasta;
            stockTransfer.PriceList = -2; //Último precio determinado
            if (conf.Default.NroSerieTSPorDefecto > 0) {
                stockTransfer.Series = conf.Default.NroSerieTSPorDefecto; //TR_HUM
            }
            
            //solo es una linea con n ubicaciones y un solo lote
            stockTransfer.Lines.ItemCode = me.CodArticulo;
            stockTransfer.Lines.Quantity = Convert.ToDouble(me.CantidadTotal);
            stockTransfer.Lines.FromWarehouseCode = me.CodBodegaDesde;
            stockTransfer.Lines.WarehouseCode = me.CodBodegaHasta;

            //es un solo lote
            stockTransfer.Lines.BatchNumbers.BatchNumber = me.Lote;
            stockTransfer.Lines.BatchNumbers.Quantity = Convert.ToDouble(me.CantidadTotal);
            stockTransfer.Lines.BatchNumbers.Add();
            
            //ubicacion desde
            if (me.IdUbicacionDesde > 0)
            {
                stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                stockTransfer.Lines.BinAllocations.BinAbsEntry = me.IdUbicacionDesde;
                stockTransfer.Lines.BinAllocations.Quantity = Convert.ToDouble(me.CantidadTotal);
                stockTransfer.Lines.BinAllocations.Add();
            }
            //varias ubicaciones destino
            me.UbicacionesCantidadHasta.ForEach(uch => { //si no se ha mandado ubicaciones solo hace transferencia entre bodegas
                stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batToWarehouse; 
                stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0; //0 porque solo hay un lote desde
                stockTransfer.Lines.BinAllocations.BinAbsEntry = uch.IdUbicacion;
                stockTransfer.Lines.BinAllocations.Quantity = Convert.ToDouble(uch.Cantidad);
                stockTransfer.Lines.BinAllocations.Add();
            });
           
            stockTransfer.Lines.Add();
                
            var error = stockTransfer.Add();
            if (error != 0)
                ms.Error = this.Company.GetLastErrorDescription();
            else
                ms.Id = this.Company.GetNewObjectKey();
            return ms;
        }

        //proyecto Espinosa Paez
        public DocSapInsertadoMsg TransferirSinUbicaciones(TsBalanzasMsg me)
        {
            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer = this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            stockTransfer.DocDate = DateTime.Now;
            stockTransfer.FromWarehouse = me.CodBodegaDesde;
            stockTransfer.ToWarehouse = me.CodBodegaHasta;
            if (conf.Default.NroSerieTSPorDefecto > 0)
            {
                stockTransfer.Series = conf.Default.NroSerieTSPorDefecto; //TR_HUM
            }
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

        
        public DocSapInsertadoMsg AddFromSt(TsFromPickingME me)
        {
            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer = this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            stockTransfer.FromWarehouse = me.BodegaOrigen;
            stockTransfer.ToWarehouse = me.BodegaDestino;
            stockTransfer.DocDate = DateTime.Now;
            if (conf.Default.NroSerieTSPorDefecto > 0)
            {
                stockTransfer.Series = conf.Default.NroSerieTSPorDefecto; //TR_HUM
            }
            me.Componentes.ForEach(line =>
            {
                stockTransfer.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest; // Solicitud de transferencia
                stockTransfer.Lines.BaseEntry = me.Id;
                stockTransfer.Lines.BaseLine = line.LineNum;
                stockTransfer.Lines.FromWarehouseCode = line.BodegaOrigen;
                stockTransfer.Lines.WarehouseCode = line.BodegaDestino;
                stockTransfer.Lines.Quantity = line.cantidadEnviada;
                
                //lotes
                stockTransfer.Lines.BatchNumbers.BatchNumber = line.Lote;
                stockTransfer.Lines.BatchNumbers.Quantity = line.cantidadEnviada;
                stockTransfer.Lines.BatchNumbers.Add();
                

                //ubicacion desde
                if (line.IdUbicacion > 0)
                {
                    stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                    stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = 0;
                    stockTransfer.Lines.BinAllocations.BinAbsEntry = line.IdUbicacion;
                    stockTransfer.Lines.BinAllocations.Quantity = line.cantidadEnviada;
                    stockTransfer.Lines.BinAllocations.Add();
                }
                stockTransfer.Lines.Add();
            });
           
            var error = stockTransfer.Add();
            if (error != 0)
                ms.Error = this.Company.GetLastErrorDescription();
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
