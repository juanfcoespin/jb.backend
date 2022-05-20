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
        public string AddPorCompra(EntradaMercanciaMsg me)
        {
            var entradaMercancia= this.Company.GetBusinessObject(BoObjectTypes.oPurchaseDeliveryNotes);
            var ms = "ok";
            entradaMercancia.DocDate = DateTime.Now;
            entradaMercancia.CardCode = me.CodProveedor;
            me.Lineas.ForEach(line =>
            {
                entradaMercancia.Lines.ItemCode = line.CodArticulo;
                //entradaMercancia.Lines.Quantity = line.Cantidad;
                entradaMercancia.Lines.WarehouseCode = me.CodBodega;
                double cantidadLinea = 0;
                line.AsignacionesLote.ForEach(asignacionLote =>
                {
                    cantidadLinea += asignacionLote.Cantidad;
                    entradaMercancia.Lines.BatchNumbers.BatchNumber = asignacionLote.Lote;
                    entradaMercancia.Lines.BatchNumbers.ManufacturingDate = Convert.ToDateTime(asignacionLote.FechaFabricacion);
                    entradaMercancia.Lines.BatchNumbers.ExpiryDate = Convert.ToDateTime(asignacionLote.FechaVencimiento);
                    entradaMercancia.Lines.BatchNumbers.Quantity = asignacionLote.Cantidad;
                    entradaMercancia.Lines.BatchNumbers.Add();
                });
                entradaMercancia.Lines.Quantity = cantidadLinea;
                entradaMercancia.Lines.Add();
                
            });
            var error = entradaMercancia.Add();
            if (error != 0)
            {
                ms = "Error: " + this.Company.GetLastErrorDescription();
            }
            return ms+ this.Company.GetNewObjectKey();
        }
        
        ~SapEntradaMercancia()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
