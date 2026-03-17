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
        private BindingList<DocsToSyncMsg> _logs = new BindingList<DocsToSyncMsg>();
        private BindingList<DocsToSyncMsg> _docsConError = new BindingList<DocsToSyncMsg>();
        private BindingList<DocsToSyncMsg> _docsToSync = new BindingList<DocsToSyncMsg>();
        FiltroHistoricoMsg _filtroConsultaHistorico;
        private bool procesando = false;
        private int numConsultas = 0;
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
            //se asignan los bindings en los controles de visualización
            
            ctrlDocsError.SetData(_docsConError);
            ctrlDocsLogs.SetData(_logs);
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
            if (!this.procesando && !backgroundWorkerSyncDocs.IsBusy)
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
        
        private void procesarDocumentos()
        {
            var syncBusiness = new SincronizationBusiness();
            //errores no controlados en la capa de negocio
            syncBusiness.onError += (err) =>
            {
                // cuando se da un error se para el timer y mustra el msg
                RunOnUI(() =>
                {
                    iniciar(false);
                    MessageBox.Show(err);
                });
            };
            syncBusiness.onDocsToSync += (docs) =>
            {
                if (docs.Count == 0)
                    return;
                //muestra los documentos a sincronizar
                RunOnUI(() =>
                {
                    _docsToSync= new BindingList<DocsToSyncMsg>(docs);
                    ctrDocsToSync.SetData(_docsToSync);
                    ctrDocsToSync.RefreshMensgesOnSync();
                });
            };
            //actualizaciones de sincronización o error
            syncBusiness.onChangeSyncStatus += (docOnSync) =>
            {
                //actualiza el estado de la sincronización
                RunOnUI(() =>
                {
                    if (_docsToSync == null || _docsToSync.Count == 0)
                        return;
                    var doc = _docsToSync.FirstOrDefault(d => d.IdCache == docOnSync.IdCache);
                    if (doc != null)
                    {
                        doc.MensajesSincronizacion = docOnSync.MensajesSincronizacion;
                        ctrDocsToSync.RefreshMensgesOnSync();
                        if (doc.TieneError)
                        {
                            UpdateGridControl(doc, ctrlDocsError, _docsConError);
                            lblError.Text = "Error: " + _docsConError.Count.ToString();
                        }
                    }
                });
            };
            //Cuando se sincroniza mando al log
            syncBusiness.onDocSyncOK += (docSincronizado) =>
            {
                RunOnUI(() =>
                {
                    UpdateGridControl(docSincronizado, ctrlDocsLogs, _logs);
                    lblOk.Text = "OK: " + _logs.Count.ToString();
                });
            };
            syncBusiness.SincronizarPedidoYCobros();
        }


        private void UpdateGridControl(DocsToSyncMsg docSincronizado, DocsToSyncViewer ctrl, BindingList<DocsToSyncMsg> list)
        {
            var docEnList = list.FirstOrDefault(d => d.IdCache == docSincronizado.IdCache);
            // no hay opción que este documento exista previemante en la grilla de destino por eso solo se lo añade
            if (docEnList == null)
            {
                list.Add(docSincronizado);
                ctrl.RefreshMensgesOnSync();
            }
            // remuevo el item del control por sincronizar (o dio error o se sincronizó)
            var item = _docsToSync.FirstOrDefault(d => d.IdCache == docSincronizado.IdCache);
            if (item != null) {
                _docsToSync.Remove(item);
                ctrDocsToSync.RefreshMensgesOnSync();
            }
        }
        private void frmSyncAppVET_Load(object sender, EventArgs e)
        {
            inicializarBindings();
        }

        private void ConsultarHistorico()
        {
            cmdConsultarHistorico.Enabled = false;
            this.bsFiltroHistorico.EndEdit();
            var syncBusiness = new SincronizationBusiness();
            syncBusiness.onError += (err) => { MessageBox.Show(err); };
            if (this._filtroConsultaHistorico.TipoDocumento == "Pedido")
            {
                var pedidos = syncBusiness.ConsultarHistoricoPedidos(this._filtroConsultaHistorico);
                if (pedidos != null && pedidos.Count > 0) {
                    ctrResultado.SetData(new BindingList<DocsToSyncMsg>(pedidos));
                }
                    
            }
            else
            {
                MessageBox.Show("No implementado");
            }
            cmdConsultarHistorico.Enabled = true;
        }

        

        

        private void backgroundWorkerSyncDocs_DoWork(object sender, DoWorkEventArgs e)
        {
            this.procesando = true;
            procesarDocumentos();
            this.procesando = false;
        }

        private void backgroundWorkerSyncDocs_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.procesando = false;
        }
    }
}
