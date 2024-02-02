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
        public EntradaMercanciaMsg Add(EntradaMercanciaMsg me)
        {
            var entradaMercancia= this.Company.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
            entradaMercancia.DocDate = DateTime.Now;
            entradaMercancia.CardCode = me.CodProveedor;
            me.Lineas.ForEach(line =>
            {
                entradaMercancia.Lines.ItemCode = line.CodArticulo;
                entradaMercancia.Lines.WarehouseCode = line.CodBodega;
                double cantidadLinea = 0;
                line.AsignacionesLote.ForEach(asignacionLote =>
                {
                    if (line.AsignacionesLote != null && line.AsignacionesLote.Count > 0)
                    {
                        cantidadLinea += asignacionLote.Cantidad;
                        entradaMercancia.Lines.BatchNumbers.BatchNumber = asignacionLote.Lote;
                        entradaMercancia.Lines.BatchNumbers.ManufacturingDate = Convert.ToDateTime(asignacionLote.FechaFabricacion);
                        entradaMercancia.Lines.BatchNumbers.ExpiryDate = Convert.ToDateTime(asignacionLote.FechaVencimiento);
                        entradaMercancia.Lines.BatchNumbers.Quantity = asignacionLote.Cantidad;
                        entradaMercancia.Lines.BatchNumbers.Add();
                    }
                });
                entradaMercancia.Lines.Quantity = cantidadLinea;
                entradaMercancia.Lines.Add();
                
            });
            var error = entradaMercancia.Add();
            if (error != 0)
            {
                me.Error = "Error: " + this.Company.GetLastErrorDescription();
            }else
                me.IdEM=this.Company.GetNewObjectKey();
            return me;
        }
        public void RegistrarGastosAdicionales(dynamic entradaMercancia, List<GastosAdicionalesMsg> me) {
            if (me != null && me.Count > 0) {
                var i = 0;
                me.ForEach(ga => {
                    entradaMercancia.Expenses.SetCurrentLine(i);
                    entradaMercancia.Expenses.BaseDocEntry=ga.DocEntry;
                    entradaMercancia.Expenses.BaseDocLine = ga.LineNum;
                    entradaMercancia.Expenses.BaseDocType = ga.ObjType;
                    entradaMercancia.Expenses.Add();
                    i++;
                });
            }
        }
        public EntradaMercanciaMsg AddPorCompra(EntradaMercanciaMsg me)
        {
            var entradaMercancia = this.Company.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
            entradaMercancia.DocDate = DateTime.Now;
            entradaMercancia.CardCode = me.CodProveedor;
            me.Lineas.ForEach(line =>
            {
                if (line.AsignacionesLote != null && line.AsignacionesLote.Count > 0) {
                    var baseType= this.GetSAPBaseType(me.tipo);
                    if (baseType == null)
                        throw new Exception("No se ha podido definir el documento base para EM!!");
                    entradaMercancia.Lines.BaseType = baseType;
                    entradaMercancia.Lines.BaseEntry = me.IdDocOrigen;
                    entradaMercancia.Lines.BaseLine = line.LineNum;
                    if(!string.IsNullOrEmpty(line.CodBodega))
                        entradaMercancia.Lines.WarehouseCode = line.CodBodega;
                    double cantidadLinea = 0;
                    line.AsignacionesLote.ForEach(asignacionLote =>
                    {
                        cantidadLinea += asignacionLote.Cantidad;
                    });
                    entradaMercancia.Lines.Quantity = cantidadLinea;
                    var i = 0;
                    line.AsignacionesLote.ForEach(asignacionLote =>
                    {
                        entradaMercancia.Lines.BatchNumbers.BatchNumber = asignacionLote.Lote;
                        entradaMercancia.Lines.BatchNumbers.ManufacturingDate = Convert.ToDateTime(asignacionLote.FechaFabricacion);
                        entradaMercancia.Lines.BatchNumbers.ExpiryDate = Convert.ToDateTime(asignacionLote.FechaVencimiento);
                        entradaMercancia.Lines.BatchNumbers.Quantity = asignacionLote.Cantidad;
                        entradaMercancia.Lines.BatchNumbers.ManufacturerSerialNumber =
                            asignacionLote.Fabricante.Length > 31 ? asignacionLote.Fabricante.Substring(0, 31) : asignacionLote.Fabricante; //da error si es mas grande
                        entradaMercancia.Lines.BatchNumbers.InternalSerialNumber = asignacionLote.LoteFabricante;
                        entradaMercancia.Lines.BatchNumbers.UserFields.Fields.Item("U_FecRet").Value = Convert.ToDateTime(asignacionLote.FechaRetest);
                        entradaMercancia.Lines.BatchNumbers.UserFields.Fields.Item("U_IXX_CANT_BULTOS").Value = asignacionLote.Bultos ;
                        entradaMercancia.Lines.BatchNumbers.Add();

                        if (!string.IsNullOrEmpty(asignacionLote.Ubicacion)) {
                            //se asigna ubicacion destino
                            //entradaMercancia.Lines.BinAllocations.BinActionType = SAPbobsCOM.BinActionTypeEnum.;
                            entradaMercancia.Lines.BinAllocations.SerialAndBatchNumbersBaseLine = i; //0 porque por defecto coge el lote que se agrego antes
                            entradaMercancia.Lines.BinAllocations.BinAbsEntry = asignacionLote.IdUbicacion;
                            entradaMercancia.Lines.BinAllocations.Quantity = asignacionLote.Cantidad;
                            entradaMercancia.Lines.BinAllocations.Add();
                        }
                        i++;
                    });
                    
                    entradaMercancia.Lines.Add();
                }
            });
            
            RegistrarGastosAdicionales(entradaMercancia, me.GastosAdicionales);
            
            var error = entradaMercancia.Add();
            if (error != 0)
            {
                me.Error = "Error: " + this.Company.GetLastErrorDescription();
            }else
                me.IdEM= this.Company.GetNewObjectKey();
            return me;
        }

        private dynamic GetSAPBaseType(string tipo)
        {
            if(tipo == "Pedido de Compra")
                return (int)BoObjectTypes.oPurchaseOrders;
            if (tipo == "Factura de Reserva")
                return (int)BoObjectTypes.oPurchaseInvoices;
            return null;
        }

        ~SapEntradaMercancia()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
