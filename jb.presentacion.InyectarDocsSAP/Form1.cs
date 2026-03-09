using System.Diagnostics;
using System.ComponentModel;
using jb.presentacion.InyectarDocsSAP.msg;
using jbp.business.hana;

namespace jb.presentacion.InyectarDocsSAP
{
    public partial class frmSyncAppVET : Form
    {
        BindingList<LogMsg> logs = new BindingList<LogMsg>();
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
            this.cmdIniciar.Enabled = false;
            this.cmdDetener.Enabled = true;
            this.WindowState = FormWindowState.Minimized;
            mostrarMsgOnTrySystem("Se ha iniciado el proceso de sincronización");
            this.timer1.Interval = conf.Default.IntervaloPoolingMinutos * 60 * 1000;
            this.timer1.Enabled = true;
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

        private void cmdDetener_Click(object sender, EventArgs e)
        {
            this.cmdIniciar.Enabled = true;
            this.cmdDetener.Enabled = false;
            this.timer1.Enabled = false;
        }
        private bool procesando = false;
        private int numConsultas = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.numConsultas++;
            this.lblIntentos.Text="num: "+numConsultas.ToString();
            if (!this.procesando) {
                this.procesando = true;
                procesarDocumentos();
                this.procesando=false;
            }
        }

        private void procesarDocumentos()
        {
            log("Consultando pedidos por sincronizar...");
            var ordenBussines = new jbp.business.hana.OrderBusiness();
            ordenBussines.onError += (string err) => {
                this.log(err, "error");
            };
            var pedidos = ordenBussines.GetOrderToSync();
            var cantOrdenes = pedidos.Count;
            if ( cantOrdenes > 0) {
                log(string.Format("{0} pedidos de venta por sincronizar", cantOrdenes));
                var syncBusiness = new SincronizationBusiness();
                syncBusiness.onError += (err) => { log(err, "error");};
                syncBusiness.onNotifyMsg += (msg) => { log(msg); };
                syncBusiness.SincronizarPedidoYCobros(pedidos);
            }
            else
                log("No se han encontrado pedidos para sincronizar.");
        }

        private void log(string msg, string tipo="info")
        {
            //solo se inserta en log si el mensaje no existe
            if (this.logs.Any(l => l.Msg == msg))
                return;
            this.logs.Add(new LogMsg
            {
                Fecha = DateTime.Now,
                Msg = msg,
                Tipo=tipo
            });
        }

        private void frmSyncAppVET_Load(object sender, EventArgs e)
        {
            this.dataGridView1.DataSource = this.logs;
        }
    }
}
