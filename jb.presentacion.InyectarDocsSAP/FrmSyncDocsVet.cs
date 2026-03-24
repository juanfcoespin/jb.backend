using jbp.business.hana;
using jbp.msg.sap;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
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
        #region test functions
        public static string DesencriptarCadena()
        {
            var Cadena = "fIR6A9KwtrBjhtxmAUsWfxPhAUOSq2VDzYgdGOgbfQhz2SXWW3eGLbQ6IAG99AervNGciLc2kPOzKsHbF441XKUw5G9uXJxKnTLH/WfVaupRInPGzUvSle264FMWbB2W";
            byte[] buffer = Convert.FromBase64String(Cadena);
            byte[] numArray = new byte[buffer.Length];
            string empty = string.Empty;
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            using (MemoryStream memoryStream = new MemoryStream(buffer))
            {
                var clave = Encoding.ASCII.GetBytes("SistemaZero102zz");
                var IV = Encoding.ASCII.GetBytes("Cadena.RequErida");
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, rijndaelManaged.CreateDecryptor(clave, IV), CryptoStreamMode.Read))
                {
                    using (StreamReader streamReader = new StreamReader((Stream)cryptoStream, true))
                        return streamReader.ReadToEnd();
                }
            }
        }
        public static string EncriptarCadena(string Cadena)
        {
            var clave = Encoding.ASCII.GetBytes("SistemaZero102zz");
            var IV = Encoding.ASCII.GetBytes("Cadena.RequErida");
            byte[] bytes = Encoding.ASCII.GetBytes(Cadena);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            byte[] array;
            using (MemoryStream memoryStream = new MemoryStream(bytes.Length))
            {
                using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, rijndaelManaged.CreateEncryptor(clave, IV), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(bytes, 0, bytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cryptoStream.Close();
                }
                array = memoryStream.ToArray();
            }
            return "(encriptado)" + Convert.ToBase64String(array);
        }
        #endregion
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
            if(this.numConsultas==1000)
                this.numConsultas=0;
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
                    MessageBox.Show(err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                });
            };
            syncBusiness.onDocsToSync += (docs) =>
            {
                if (docs.Count == 0)
                    return;
                //muestra los documentos a sincronizar
                RunOnUI(() =>
                {
                    _docsToSync = new BindingList<DocsToSyncMsg>(docs);
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
            if (item != null)
            {
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

            this.bsFiltroHistorico.EndEdit();
            var syncBusiness = new SincronizationBusiness();
            syncBusiness.onError += (err) =>
            {
                MessageBox.Show(err);
                cmdConsultarHistorico.Enabled = true;
                return;
            };
            var tipoDoc = eTipoDocToSync.NoDefinido;
            if (!FiltroConsultaValido())
                return;

            cmdConsultarHistorico.Enabled = false;
            switch (this._filtroConsultaHistorico.TipoDocumento)
            {
                case "Pedido":
                    tipoDoc = eTipoDocToSync.Pedido;
                    break;
                case "Cobro":
                    tipoDoc = eTipoDocToSync.Cobro;
                    break;
                default:
                    MessageBox.Show("No implementado");
                    cmdConsultarHistorico.Enabled = true;
                    return;
            }

            var docs = syncBusiness.ConsultarHistoricoDocsSincronizados(this._filtroConsultaHistorico, tipoDoc);
            if (docs != null && docs.Count > 0)
                ctrResultado.SetData(new BindingList<DocsToSyncMsg>(docs));
            else
                MessageBox.Show("No se han encontrador resultados con el filtro aplicado!!");
            cmdConsultarHistorico.Enabled = true;
        }

        private bool FiltroConsultaValido()
        {
            if (string.IsNullOrEmpty(_filtroConsultaHistorico.Vendedor))
            {
                MessageBox.Show("Debe indicar el vendedor a consultar!!");
                return false;
            }
            if (string.IsNullOrEmpty(_filtroConsultaHistorico.Cliente))
            {
                MessageBox.Show("Debe indicar el cliente a consultar!!");
                return false;
            }
            if (string.IsNullOrEmpty(_filtroConsultaHistorico.TipoDocumento))
            {
                MessageBox.Show("Debe seleccionar el tipo de documento a consultar!!");
                return false;
            }
            return true;
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
