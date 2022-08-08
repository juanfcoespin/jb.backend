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
            }
            me.IdEM=this.Company.GetNewObjectKey();
            return me;
        }

        public EntradaMercanciaMsg AddPorCompra(EntradaMercanciaMsg me)
        {
            var entradaMercancia = this.Company.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
            entradaMercancia.DocDate = DateTime.Now;
            entradaMercancia.CardCode = me.CodProveedor;
            me.Lineas.ForEach(line =>
            {
                if (line.AsignacionesLote != null && line.AsignacionesLote.Count > 0) {
                    entradaMercancia.Lines.BaseType = (int)BoObjectTypes.oPurchaseOrders;
                    entradaMercancia.Lines.BaseEntry = me.IdOrdenCompra;
                    entradaMercancia.Lines.BaseLine = line.LineNum;
                    entradaMercancia.Lines.WarehouseCode = line.CodBodega;
                    double cantidadLinea = 0;
                    line.AsignacionesLote.ForEach(asignacionLote =>
                    {
                        cantidadLinea += asignacionLote.Cantidad;
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
                    });
                    entradaMercancia.Lines.Quantity = cantidadLinea;
                    entradaMercancia.Lines.Add();
                }
            });
            var error = entradaMercancia.Add();
            if (error != 0)
            {
                me.Error = "Error: " + this.Company.GetLastErrorDescription();
            }
            me.IdEM= this.Company.GetNewObjectKey();
            return me;
        }

        ~SapEntradaMercancia()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
