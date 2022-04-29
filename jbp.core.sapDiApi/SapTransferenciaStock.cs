using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;

namespace jbp.core.sapDiApi
{
    public class SapTransferenciaStock:BaseSapObj
    {
        public SapTransferenciaStock()
        {
            //this.Connect();
        }
        public string Add(TsBodegaMsg me)
        {
            this.obj = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer);
            var ms = "ok";
            this.obj.DocDate = DateTime.Now;
            //this.obj.FromWarehouse = me.CodBodegaDesde;
            //this.obj.ToWarehouse = me.CodBodegaHasta;

            me.Lineas.ForEach(line =>
            {
                this.obj.Lines.ItemCode = line.CodArticulo;
                this.obj.Lines.Quantity = line.Cantidad;
                //this.obj.Lines.FromWarehouseCode = me.CodBodegaDesde;
                this.obj.Lines.WarehouseCode = me.CodBodegaHasta;
                
                line.Lotes.ForEach(loteAsignado =>
                {
                    this.obj.Lines.BatchNumbers.BatchNumber = loteAsignado.Lote;
                    this.obj.Lines.BatchNumbers.Quantity = loteAsignado.Cantidad;
                });
                this.obj.Lines.Add();
                
            });
            var error = this.obj.Add();
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
        ~SapTransferenciaStock()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
