using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using SAPbobsCOM;
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
            if (me.DatosAdicionalesFactura != null) {
                this.obj.UserFields.Fields.Item("U_NUM_AUT_FR").Value = me.DatosAdicionalesFactura.NumAutorizacion;
                this.obj.UserFields.Fields.Item("U_SER_EST_FR").Value = me.DatosAdicionalesFactura.PtoEstablecimiento;
                this.obj.UserFields.Fields.Item("U_SER_PEFR").Value = me.DatosAdicionalesFactura.PtoEmision;
                this.obj.UserFields.Fields.Item("U_fecha_emi_doc_rel").Value = me.DatosAdicionalesFactura.Fecha;
                this.obj.UserFields.Fields.Item("U_DOC_DECLARABLE").Value = "S";
                this.obj.UserFields.Fields.Item("U_TIP_DOC_APLIC").Value = "18";
                this.obj.UserFields.Fields.Item("U_tipo_comprob").Value = "04";
            }
            this.obj.UserFields.Fields.Item("U_MOT_NC").Value = "06"; // Descuento Pronto Pago (de la tabla de usuario)
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
                this.obj.Lines.UnitPrice = me.TotalNC;
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
