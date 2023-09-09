using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using SAPbobsCOM;

namespace jbp.core.sapDiApi
{
    public class LineaMsg2 {
        public string bd { get; set; }
        public string bh { get; set; }
    }
    public class SapTransferenciaStock2:BaseSapObj
    {
        public SapTransferenciaStock2()
        {
            //this.Connect();
        }

        //Transferencia entre ubicaciones
        public DocSapInsertadoMsg TranferirEntreUbicaciones(TsBodegaMsg me)
        {
            /*
             Se transfiere las cantidades de un solo lote
                de n bodegas y ubicaciones origen a n bodegas y ubicaciones destino
             */

            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer= this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            stockTransfer.DocDate = DateTime.Now;
            stockTransfer.PriceList = -2; //Último precio determinado
            if (conf.Default.NroSerieTSPorDefecto > 0)
            {
                stockTransfer.Series = conf.Default.NroSerieTSPorDefecto; //TR_HUM
            }
            //identifico las lineas a agregarse
            var lineas = new List<LineaMsg>();
            me.movimientos.ForEach(m => {
                var linea = new LineaMsg (){ bd = m.CodBodegaDesde, bh = m.CodBodegaHasta };
                var regEncontrados = lineas.FindAll(l => l.bd == m.CodBodegaDesde && l.bh == m.CodBodegaHasta);
                if(regEncontrados.Count()==0)
                    lineas.Add(linea);  
            });
            //barrido de movimientos por bodega
            var i = 0;
            var cantLinea = 0.0;
            int numLotes = 0;

            lineas.ForEach(l => {
                if (i == 0) //solo en la primera linea asigno las bodegas de la TS
                {
                    stockTransfer.FromWarehouse = l.bd;
                    stockTransfer.ToWarehouse = l.bh;
                }
                var movimientosPorLinea = me.movimientos.FindAll(m => m.CodBodegaDesde==l.bd && m.CodBodegaHasta==l.bh);
                movimientosPorLinea.ForEach(m => cantLinea+=m.Cantidad);
                //añado una linea
                stockTransfer.Lines.ItemCode = me.CodArticulo;
                stockTransfer.Lines.Quantity = cantLinea;
                stockTransfer.Lines.FromWarehouseCode = l.bd;
                stockTransfer.Lines.WarehouseCode = l.bh;

                

                var ubicacionesDesde = new List<int>();
                var ubicacionesHasta = new List<int>();
                movimientosPorLinea.ForEach(m => { 
                    if(!ubicacionesDesde.Contains(m.IdUbicacionDesde) && m.IdUbicacionDesde > 0)
                        ubicacionesDesde.Add(m.IdUbicacionDesde);
                });
                movimientosPorLinea.ForEach(m => {
                    if (!ubicacionesHasta.Contains(m.IdUbicacionHasta) && m.IdUbicacionHasta>0)
                        ubicacionesHasta.Add(m.IdUbicacionHasta);
                });
                //----------- retistro las ubicaciones por linea ----------------
                
                //registro el lote por linea
                stockTransfer.Lines.BatchNumbers.BatchNumber = me.Lote;
                stockTransfer.Lines.BatchNumbers.Quantity = cantLinea;
                stockTransfer.Lines.BatchNumbers.Add();

                //ubicaciones desde
                ubicacionesDesde.ForEach(idUbicacion => {
                    var cantUbicacion = 0.0;
                    movimientosPorLinea.ForEach(m => {
                        if (m.IdUbicacionDesde == idUbicacion)
                            cantUbicacion += m.Cantidad;
                    });
                    stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                    stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                    stockTransfer.Lines.BinAllocations.BinAbsEntry = idUbicacion;
                    stockTransfer.Lines.BinAllocations.Quantity = cantUbicacion;
                    stockTransfer.Lines.BinAllocations.Add();
                 });
                //ubicaciones hasta
                ubicacionesHasta.ForEach(idUbicacion => {
                    var cantUbicacion = 0.0;
                    movimientosPorLinea.ForEach(m => {
                        if (m.IdUbicacionHasta == idUbicacion)
                            cantUbicacion += m.Cantidad;
                    });
                    stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batToWarehouse;
                    stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i; //por cada bodega
                    stockTransfer.Lines.BinAllocations.BinAbsEntry = idUbicacion;
                    stockTransfer.Lines.BinAllocations.Quantity = cantUbicacion;
                    stockTransfer.Lines.BinAllocations.Add();
                });

                stockTransfer.Lines.Add();
                i++;
            });
            
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
        ~SapTransferenciaStock2()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
