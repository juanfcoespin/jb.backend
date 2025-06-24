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
    public class LineaMsg {
        public string bd { get; set; }
        public string bh { get; set; }
    }
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
             Se transfiere las cantidades de un solo lote
                de n bodegas y ubicaciones origen a n bodegas y ubicaciones destino
             */
            this.sendNotififacationMessage("Iniciando DIAPI transferencia entre ubicaciones");
            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer= this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            this.sendNotififacationMessage("Estableciendo parámetros generales");
            stockTransfer.DocDate = DateTime.Now;
            stockTransfer.PriceList = -2; //Último precio determinado
            stockTransfer.Comments = me.Responsable;
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
                this.sendNotififacationMessage("Parametrizando líneas de transferencia");
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
                    if (!ubicacionesDesde.Contains(m.IdUbicacionDesde) && m.IdUbicacionDesde > 0)
                        ubicacionesDesde.Add(m.IdUbicacionDesde);
                });
                movimientosPorLinea.ForEach(m => {
                    if (!ubicacionesHasta.Contains(m.IdUbicacionHasta) && m.IdUbicacionHasta>0)
                        ubicacionesHasta.Add(m.IdUbicacionHasta);
                });
                //----------- retistro las ubicaciones por linea ----------------

                //registro el lote por linea
                this.sendNotififacationMessage("Parametrizando lotes");
                stockTransfer.Lines.BatchNumbers.BatchNumber = me.Lote;
                stockTransfer.Lines.BatchNumbers.Quantity = cantLinea;
                stockTransfer.Lines.BatchNumbers.Add();

                this.sendNotififacationMessage("Parametrizando ubicaciones");
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
                //ubicaciones hasta unica
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
            this.sendNotififacationMessage("Insertando transaccion en la DIAPI de SAP...");
            var error = stockTransfer.Add();
            if (error != 0)
                ms.Error = this.Company.GetLastErrorDescription();
            else
                ms.Id = this.Company.GetNewObjectKey();
            return ms;
        }
        public DocSapInsertadoMsg AddFromSt(TsFromPickingME me)
         {
            //para garantizar que solo un hilo acceda a la vez
            var ms = new DocSapInsertadoMsg();
            StockTransfer stockTransfer = this.Company.GetBusinessObject(BoObjectTypes.oStockTransfer);
            stockTransfer.FromWarehouse = me.BodegaOrigen;
            stockTransfer.ToWarehouse = me.BodegaDestino;
            stockTransfer.DocDate = DateTime.Now;
            stockTransfer.Comments = me.Responsable;
            if (conf.Default.NroSerieTSPorDefecto > 0)
            {
                stockTransfer.Series = conf.Default.NroSerieTSPorDefecto; //TR_HUM
            }
            me.Componentes.ForEach(line =>
            {
                if (line.CantidadEnviada > 0) {
                    if (me.Id > 0) //si la Ts tiene como documento base una solicitud de transferencia
                    {
                        stockTransfer.Lines.BaseType = SAPbobsCOM.InvBaseDocTypeEnum.InventoryTransferRequest; // Solicitud de transferencia
                        stockTransfer.Lines.BaseEntry = me.Id;
                        stockTransfer.Lines.BaseLine = line.LineNum;
                    }
                    else {
                        stockTransfer.Lines.ItemCode = line.CodArticulo;
                    }
                    stockTransfer.Lines.FromWarehouseCode = line.BodegaOrigen;
                    stockTransfer.Lines.WarehouseCode = line.BodegaDestino;
                    stockTransfer.Lines.Quantity = Math.Round(line.CantidadEnviada,4);
                    var i=0;
                    double cantidadEnLotes = 0;
                    line.Lotes.ForEach(lote =>
                    {//lotes
                        stockTransfer.Lines.BatchNumbers.BatchNumber = lote.Lote;
                        stockTransfer.Lines.BatchNumbers.Quantity = Math.Round(lote.CantidadEnviada,4);
                        double cantidadEnUbicaciones = 0;
                        lote.Ubicaciones.ForEach(ubicacion => {//ubicacion desde
                            if (ubicacion.IdUbicacion > 0)
                            {
                                stockTransfer.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.batFromWarehouse;
                                stockTransfer.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i;
                                stockTransfer.Lines.BinAllocations.BinAbsEntry = ubicacion.IdUbicacion;
                                stockTransfer.Lines.BinAllocations.Quantity = Math.Round(ubicacion.Cantidad,4);
                                stockTransfer.Lines.BinAllocations.Add();
                                cantidadEnUbicaciones += ubicacion.Cantidad;
                            }
                        });
                        if (cantidadEnUbicaciones > 0) {
                            cantidadEnUbicaciones=Math.Round(cantidadEnUbicaciones, 4);
                            stockTransfer.Lines.BatchNumbers.Quantity = cantidadEnUbicaciones;
                        }
                        stockTransfer.Lines.BatchNumbers.Add();
                        i++;
                        cantidadEnLotes += cantidadEnUbicaciones;
                    });
                    if (cantidadEnLotes > 0) {
                        stockTransfer.Lines.Quantity = Math.Round(cantidadEnLotes, 4);
                    }
                    stockTransfer.Lines.Add();
                }
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
