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

        }

        public void SetData(List<DocsToSyncMsg> docs) {
            bsDocs.DataSource = docs;
            if (docs != null && docs.Count > 0)
            {
                bsMensajes.DataSource = docs[0].MensajesSincronizacion;
            }
        }
        public void RefreshMensgesOnSync() {
            bsMensajes.ResetBindings(false);
        }

        private void dgvDocs_SelectionChanged(object sender, EventArgs e)
        {
            if (bsDocs.Current is DocsToSyncMsg doc)
            {
                bsMensajes.DataSource = doc.MensajesSincronizacion;
            }
        }
    }
}
