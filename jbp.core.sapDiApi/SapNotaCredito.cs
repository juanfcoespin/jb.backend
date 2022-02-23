using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using TechTools.Utils;

namespace jbp.core.sapDiApi
{
    public class SapNotaCredito:BaseSapObj
    {
        public SapNotaCredito()
        {
            //this.Connect();
        }
        public string AddNcProntoPago(NotaCreditoPPMsg me)
        {
            var ms = "ok"; 
            this.obj = this.Company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDrafts);
            this.obj.DocObjectCode = SAPbobsCOM.BoObjectTypes.oCreditNotes;
            this.obj.Series = 87; //NC_PP
            this.obj.DocDueDate = DateTime.Now;
            this.obj.CardCode = me.CodCliente;
            this.obj.Comments = me.Comentario;
            
            this.obj.UserFields.Fields.Item("U_NUM_FAC_REL").Value = me.FolioNumFacturaRelacionada.ToString();
            // para que no se envie al SRI
            this.obj.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "N";
            switch (me.TipoDescPP) {
                case eNcPPType.Veterinario:
                    this.obj.Lines.ItemCode = "DESC.PP.VET";
                    break;
            }
            if(!string.IsNullOrEmpty(this.obj.Lines.ItemCode))
            {
                this.obj.Lines.Quantity = 1;
                this.obj.Lines.UserFields.Fields.Item("U_IXX_PRECIO_UNI").Value = me.TotalNC;
                this.obj.Lines.UserFields.Fields.Item("U_VALOR_TOTAL").Value = me.TotalNC;
                this.obj.Lines.Add();
            }
            this.obj.DocTotal = me.TotalNC;
            var error = this.obj.Add();
            if (error != 0)
            {
                ms = "Error: " + this.Company.GetLastErrorDescription();
            }
            return ms;
        }

       

        ~SapNotaCredito()
        {
            //cuando se destruye el objeto
            this.Disconnect();
        }
    }
}
