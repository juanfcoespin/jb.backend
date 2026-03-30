using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.PerformanceData;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace jb.presentacion.InyectarDocsSAP
{
    public partial class DocsToSyncViewer : UserControl
    {
        private BindingSource bsDocs = new BindingSource();
        private BindingSource bsMensajes = new BindingSource();

        public DocsToSyncViewer()
        {
            InitializeComponent();
            dgvDocs.DataSource = bsDocs;
            dgvMensajes.DataSource = bsMensajes;
            bsDocs.CurrentChanged += BsDocs_CurrentChanged;

        }

        private void BsDocs_CurrentChanged(object? sender, EventArgs e)
        {
            ordenLinesMsgBindingSource.DataSource = null;
            docCarteraMsgBindingSource.DataSource = null;
            tipoPagoMsgBindingSource.DataSource = null;
            chequesBindingSource.DataSource = null;
            if (bsDocs.Count == 0)
            {
                bsMensajes.DataSource = null;
                return;
            }

            if (bsDocs.Current is DocsToSyncMsg doc)
            {
                bsDocsToSync.DataSource = doc;
                bsMensajes.DataSource = doc.MensajesSincronizacion;
                if (doc.GetType() == typeof(OrdenMsg))
                {
                    tabDetalleDocToSync.SelectedTab = tpPedido;
                    ordenLinesMsgBindingSource.DataSource = ((OrdenMsg)doc).Lines;
                }
                if (doc.GetType() == typeof(PagosMsg))
                {
                    tabDetalleDocToSync.SelectedTab = tpCobro;
                    docCarteraMsgBindingSource.DataSource = ((PagosMsg)doc).facturasAPagar;
                    tipoPagoMsgBindingSource.DataSource = ((PagosMsg)doc).tiposPagoToSave;
                }


            }
        }

        public void SetData(BindingList<DocsToSyncMsg> docs)
        {
            bsDocs.DataSource = null;
            bsDocs.DataSource = docs;
        }
        public void RefreshMensgesOnSync()
        {
            bsMensajes.ResetBindings(false);
        }

        private void tipoPagoMsgBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            
            if (tipoPagoMsgBindingSource.Current is TipoPagoMsg tipoPago)
            {
                chequesBindingSource.DataSource = tipoPago.cheques;
            }
        }
    }
}
