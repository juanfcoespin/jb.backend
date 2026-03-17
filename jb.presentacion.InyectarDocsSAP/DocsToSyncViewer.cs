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
            if (bsDocs.Count == 0)
            {
                bsMensajes.DataSource = null;
                bsCurrentPedido.DataSource = null;
                return;
            }

            if (bsDocs.Current is DocsToSyncMsg doc)
            {
                bsMensajes.DataSource = doc.MensajesSincronizacion;
                if (doc.GetType() == typeof(OrdenMsg))
                    bsCurrentPedido.DataSource = doc;
            }
        }

        public void SetData(BindingList<DocsToSyncMsg> docs) {
            bsDocs.DataSource = null;
            bsDocs.DataSource = docs;
        }
        public void RefreshMensgesOnSync() {
            bsMensajes.ResetBindings(false);
        }
    }
}
