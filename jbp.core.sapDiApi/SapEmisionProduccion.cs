using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;

namespace jbp.core.sapDiApi
{
    public class SapEmisionProduccion:BaseSapObj
    {
        public SapEmisionProduccion()
        {
            //this.Connect();
        }
        public string Add(SalidaBodegaMsg me)
        {
            this.obj = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit); //salida de inventario
            var ms = "ok";
            this.obj.DocDueDate = DateTime.Now;
            
            me.Lineas.ForEach(line =>
            {
                this.obj.Lines.BaseType = 202; // Orden de fabricación
                this.obj.Lines.BaseEntry = me.IdOF;
                this.obj.Lines.ItemCode = line.CodArticulo;
                //this.obj.Lines.UserFields.Fields.Item("U_IXX_PRECIO_UNI").Value = line.price;
                this.obj.Lines.Quantity = line.Cantidad;
                this.obj.Lines.Add();
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
        ~SapEmisionProduccion()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
