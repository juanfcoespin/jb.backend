using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;

namespace jbp.core.sapDiApi
{
    public class SapOrder:BaseSapObj
    {
        public SapOrder()
        {
            //this.Connect();
        }
        public string Add(OrdenMsg me)
        {
            this.obj = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders);
            var ms = "ok";
            this.obj.DocDueDate = DateTime.Now;
            this.obj.CardCode = me.CodCliente;
            this.obj.Comments = me.Comentario;
            this.obj.NumAtCard = me.Vendedor;
            me.Lines.ForEach(line =>
            {
                this.obj.Lines.ItemCode = line.CodArticulo;
                var type = this.obj.Lines.Price.GetType();
                if(!string.IsNullOrEmpty(line.CodBodega))
                    this.obj.Lines.WarehouseCode = line.CodBodega; // caso contrario coge la bodega por defecto del dato maestro del artículo
                this.obj.Lines.UserFields.Fields.Item("U_IXX_PRECIO_UNI").Value = line.price;
                this.obj.Lines.UserFields.Fields.Item("U_IXX_CANT_SOL").Value = line.CantSolicitada;
                this.obj.Lines.UserFields.Fields.Item("U_IXX_CANT_BON").Value = line.CantBonificacion;
                this.obj.Lines.Quantity = line.CantBruta;
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
        ~SapOrder()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
