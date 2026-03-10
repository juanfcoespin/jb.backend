using System.Diagnostics;
using System.ComponentModel;
using jbp.business.hana;
using jbp.msg.sap;

namespace jb.presentacion.InyectarDocsSAP
{
    public partial class frmSyncAppVET : Form
    {
        BindingList<DocsToSyncMsg> logs = new BindingList<DocsToSyncMsg>();
        BindingList<DocsToSyncMsg> docsToSync = new BindingList<DocsToSyncMsg>();
        public frmSyncAppVET()
        {
            InitializeComponent();
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

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

        private void BloquearDesbloquearInicioServicio(bool iniciar)
        {
            this.cmdIniciar.Enabled = !iniciar;
            this.cmdDetener.Enabled = iniciar;
        }

        private void setTimer(bool start)
        {
            if (start)
            {
                this.timer1.Start();
                this.timer1.Interval = conf.Default.IntervaloPoolingMinutos * 60 * 1000;
            }
            else
                this.timer1.Stop();
            this.timer1.Enabled = start;

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


        private bool procesando = false;
        private int numConsultas = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.numConsultas++;
            this.lblIntentos.Text = "num: " + numConsultas.ToString();
            if (!this.procesando)
            {
                this.procesando = true;
                procesarDocumentos();
                this.procesando = false;
            }
        }

        private void procesarDocumentos()
        {
            //log(string.Format("{0} pedidos de venta por sincronizar", cantOrdenes));
            var syncBusiness = new SincronizationBusiness();
            syncBusiness.onError += (err) =>
            {
                // cuando se da un error se para el timer y mustra el msg
                iniciar(false);
                MessageBox.Show(err);
            };
            syncBusiness.onDocsToSync += (docs) =>
            {
                var cantOrdenes = docs.Count;
                if (cantOrdenes > 0)
                    MostrarDocsToSync(docs);

            };
            syncBusiness.onChangeSyncStatus += (docStatus) =>
            {
                foreach (var doc in this.docsToSync)
                {
                    //solo actualiza el status de sincronización del documento
                    if (doc.IdCache == docStatus.IdCache)
                    {
                        doc.Status = docStatus.Status;
                        doc.TipoMsg = docStatus.TipoMsg;
                        log(doc);
                    }
                }
            };
            syncBusiness.SincronizarPedidoYCobros();
        }


        private void MostrarDocsToSync(List<DocsToSyncMsg> docs)
        {
            docsToSync.Clear();
            docs.ForEach(doc =>
            {
                docsToSync.Add(doc);
            });
        }

        private void log(DocsToSyncMsg doc)
        {
            //solo se inserta en log si el mensaje no existe
            if (this.logs.Any(l => l.IdCache == doc.IdCache && l.Status == doc.Status))
                return;
            doc.FechaLog = DateTime.Now;
            this.logs.Add(doc);
        }

        FiltroHistoricoMsg _filtroConsultaHistorico;
        private void frmSyncAppVET_Load(object sender, EventArgs e)
        {
            inicializarBindings();
        }

        private void inicializarBindings()
        {
            this.dgLog.DataSource = this.logs;
            this.dgDocsToSync.DataSource = this.docsToSync;
            this._filtroConsultaHistorico = new FiltroHistoricoMsg
            {
                Desde = DateTime.Today,
                Hasta = DateTime.Today,
            };
            this.bsFiltroHistorico.DataSource = this._filtroConsultaHistorico;
        }


        private void cmdConsultarHistorico_Click(object sender, EventArgs e)
        {
            ConsultarHistorico();
        }

        private void ConsultarHistorico()
        {
            var syncBusiness = new SincronizationBusiness();

            this.bsFiltroHistorico.EndEdit();
            syncBusiness.ConsultarHistorico(this._filtroConsultaHistorico);
        }
    }
}
