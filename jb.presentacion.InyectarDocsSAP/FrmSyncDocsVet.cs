using jbp.business.hana;
using jbp.msg.sap;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using static jbp.business.hana.SincronizationBusiness;

namespace jb.presentacion.InyectarDocsSAP
{
    public partial class frmSyncAppVET : Form
    {
        private List<DocsToSyncMsg> _logs = new List<DocsToSyncMsg>();
        private List<DocsToSyncMsg> _docsToSync = new List<DocsToSyncMsg>();
        public frmSyncAppVET()
        {
            InitializeComponent();
        }

        private void inicializarBindings()
        {
            this._filtroConsultaHistorico = new FiltroHistoricoMsg
            {
                Desde = DateTime.Today,
                Hasta = DateTime.Today,
            };
            this.bsFiltroHistorico.DataSource = this._filtroConsultaHistorico;
        }
        #region Timer Region
        private void cmdIniciar_Click(object sender, EventArgs e)
        {

            this.WindowState = FormWindowState.Minimized;
            mostrarMsgOnTrySystem("Se ha iniciado el proceso de sincronización");
            iniciar(true);
        }
        private void cmdDetener_Click(object sender, EventArgs e)
        {
            iniciar(false);
        }
        private void iniciar(bool iniciar)
        {
            BloquearDesbloquearInicioServicio(iniciar);
            setTimer(iniciar);
        }
        private void setTimer(bool start)
        {
            if (start)
            {
                if (!this.timer1.Enabled)
                    this.timer1.Enabled = true;
                this.timer1.Interval = conf.Default.IntervaloPoolingSegundos * 1000;
                this.timer1.Start();
            }
            else
                this.timer1.Stop();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.numConsultas++;
            this.lblIntentos.Text = "Num: " + numConsultas.ToString();
            if (!this.procesando)
            {
                backgroundWorkerSyncDocs.RunWorkerAsync();
            }
        }
        #endregion
        #region Interfaz region
        private void cmdConsultarHistorico_Click(object sender, EventArgs e)
        {
            ConsultarHistorico();
        }
        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }
        private void BloquearDesbloquearInicioServicio(bool iniciar)
        {
            this.cmdIniciar.Enabled = !iniciar;
            this.cmdDetener.Enabled = iniciar;
        }

        private void mostrarMsgOnTrySystem(string msg)
        {
            notifyIcon1.BalloonTipTitle = "Notificación Sync - AppVET";
            notifyIcon1.BalloonTipText = msg;
            notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;

            notifyIcon1.ShowBalloonTip(3000);
        }

        private void cerrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.notifyIcon1.Visible = false;
            Application.Exit();
        }

        private void mostrarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
        }
        #endregion

        void RunOnUI(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }

        private bool procesando = false;
        private int numConsultas = 0;
        
        private void procesarDocumentos()
        {
            var syncBusiness = new SincronizationBusiness();
            syncBusiness.onError += (err) =>
            {
                // cuando se da un error se para el timer y mustra el msg
                iniciar(false);
                MessageBox.Show(err);
            };
            syncBusiness.onDocsToSync += (docs) =>
            {
                if (docs.Count == 0)
                    return;

                RunOnUI(() =>
                {
                    //muestra los documentos a sincronizar
                    _docsToSync=docs;
                    ctrDocsToSync.SetData(_docsToSync);
                });
            };
            syncBusiness.onChangeSyncStatus += (docOnSync) =>
            {
                //actualiza el estado de la sincronización
                RunOnUI(() =>
                {
                    var doc = _docsToSync.FirstOrDefault(d => d.IdCache == docOnSync.IdCache);
                    if (doc != null)
                    {
                        doc.MensajesSincronizacion = docOnSync.MensajesSincronizacion;
                        ctrDocsToSync.RefreshMensgesOnSync();
                    }

                });
            };
            //Cuando se sincroniza mando al log
            syncBusiness.onDocSyncOK += (docSincronizado) =>
            {
                _docsToSync.RemoveAll(d => d.IdCache==docSincronizado.IdCache);
                _logs.Add(docSincronizado);
                ctrlDocsLogs.SetData(_logs);
            };
            syncBusiness.SincronizarPedidoYCobros();
        }

        

        FiltroHistoricoMsg _filtroConsultaHistorico;
        private void frmSyncAppVET_Load(object sender, EventArgs e)
        {
            inicializarBindings();
        }

        private void ConsultarHistorico()
        {
            cmdConsultarHistorico.Enabled = false;
            ordenMsgBindingSource.Clear();
            this.bsFiltroHistorico.EndEdit();
            var syncBusiness = new SincronizationBusiness();
            syncBusiness.onError += (err) => { MessageBox.Show(err); };
            if (this._filtroConsultaHistorico.TipoDocumento == "Pedido")
            {
                var pedidos = syncBusiness.ConsultarHistoricoPedidos(this._filtroConsultaHistorico);
                ordenMsgBindingSource.DataSource = pedidos;
                if (pedidos != null && pedidos.Count > 0)
                    setCurrentPedido(pedidos[0]);
            }
            else
            {
                MessageBox.Show("No implementado");
            }
            cmdConsultarHistorico.Enabled = true;
        }

        private void setCurrentPedido(OrdenMsg me)
        {
            bsCurrentPedido.DataSource = me;
            bsCurrentPedido.EndEdit();
        }

        private void dgResultadoBusqueda_SelectionChanged(object sender, EventArgs e)
        {
            if (dgResultadoBusqueda.CurrentRow != null && dgResultadoBusqueda.CurrentRow.DataBoundItem != null)
                this.setCurrentPedido((OrdenMsg)dgResultadoBusqueda.CurrentRow.DataBoundItem);
        }

        private void backgroundWorkerSyncDocs_DoWork(object sender, DoWorkEventArgs e)
        {
            this.procesando = true;
            procesarDocumentos();
        }

        private void backgroundWorkerSyncDocs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.procesando = false;
        }
    }
}
