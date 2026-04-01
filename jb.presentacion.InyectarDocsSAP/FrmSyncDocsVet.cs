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
        private BindingList<DocsToSyncMsg> _docsToSync = new BindingList<DocsToSyncMsg>();
        private BindingList<DocsToSyncMsg> _logs = new BindingList<DocsToSyncMsg>();
        private BindingList<DocsToSyncMsg> _docsConError = new BindingList<DocsToSyncMsg>();
        private BindingList<DocsToSyncMsg> _docsResultado = new BindingList<DocsToSyncMsg>();

        FiltroHistoricoMsg _filtroConsultaHistorico;
        private bool procesando = false;
        private readonly object _lock = new object();

        private SincronizationBusiness _syncBusiness = new SincronizationBusiness();

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
            ctrDocsToSync.SetData(_docsToSync);
            ctrlDocsLogs.SetData(_logs);
            ctrlDocsError.SetData(_docsConError);
            ctrResultado.SetData(_docsResultado);


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
            lblFechaInicio.Text = DateTime.Now.ToString();
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
        private async void timer1_Tick(object sender, EventArgs e)
        {

            lblFechaUltimaConsulta.Text = DateTime.Now.ToString();
            if (procesando)
                return;
            procesando = true;
            try
            {
                await Task.Run(() => procesarDocumentos());
            }
            catch (Exception ex)
            {
                showError(ex);
            }
            finally
            {
                procesando = false;
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
            try
            {
                if (this.IsDisposed || !this.IsHandleCreated)
                    return;

                if (this.InvokeRequired)
                    this.Invoke(action);
                else
                    action();
            }
            catch (Exception ex)
            {
                File.AppendAllText("fatal.log",
                    $"RunOnUI ERROR {DateTime.Now}\n{ex}\n\n");
            }
        }
        private void showError(Exception e)
        {
            showError(e.Message + e.StackTrace);
        }
        private void showError(string errorStr)
        {
            if (this.InvokeRequired)
            {
                try
                {
                    this.Invoke(() => MessageBox.Show(errorStr, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
                }
                catch
                {
                    // fallback si el form ya murió
                    File.AppendAllText("fatal.log", DateTime.Now.ToString() + errorStr + Environment.NewLine);
                }
            }
            else
            {
                MessageBox.Show(errorStr, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool _ejecutandoBusiness = false;
        private void procesarDocumentos()
        {
            if (_ejecutandoBusiness)
                return;
            try
            {
                _ejecutandoBusiness = true;
                _syncBusiness.SincronizarPedidoYCobros();
            }
            finally
            {
                _ejecutandoBusiness = false;
            }
        }
        private void frmSyncAppVET_Load(object sender, EventArgs e)
        {
            inicializarEventosBussiness();
            inicializarBindings();
        }
        private void inicializarEventosBussiness()
        {
            _syncBusiness.onError += (err) =>
            {
                // cuando se da un error se para el timer y mustra el msg
                RunOnUI(() =>
                {
                    iniciar(false);
                    showError(err);
                });
            };

            //errores no controlados en la capa de negocio
            _syncBusiness.onDocsToSync += (docs) =>
            {
                if (docs.Count == 0)
                    return;
                //muestra los documentos a sincronizar
                RunOnUI(() =>
                {
                    mapDataToGrid(docs, ref _docsToSync);
                });
            };
            //actualizaciones de sincronización o error
            _syncBusiness.onChangeSyncStatus += (docOnSync) =>
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
                            _docsConError.Add(doc);
                            lblError.Text = "Error: " + _docsConError.Count.ToString();
                        }
                    }
                });
            };
            //Cuando se sincroniza mando al log
            _syncBusiness.onDocSyncOK += (docSincronizado) =>
            {
                RunOnUI(() =>
                {
                    lock (_lock) {
                        _logs.Add(docSincronizado);
                        if (_docsToSync.Count > 0) { 
                            var index=_docsToSync.IndexOf(docSincronizado);
                            _docsToSync.RemoveAt(index);
                        }
                        lblOk.Text = "Docs Sincronizados: " + _logs.Count.ToString();
                    }
                });
            };
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
                case "Todos":
                    tipoDoc = eTipoDocToSync.NoDefinido;
                    break;
                default:
                    MessageBox.Show("No implementado");
                    cmdConsultarHistorico.Enabled = true;
                    return;
            }

            var docs = syncBusiness.ConsultarHistoricoDocsSincronizados(this._filtroConsultaHistorico, tipoDoc);
            mapDataToGrid(docs, ref _docsResultado);
            if (_docsResultado.Count == 0)
                MessageBox.Show("No se han encontrador resultados con el filtro aplicado!!");
            cmdConsultarHistorico.Enabled = true;
        }

        private void mapDataToGrid(List<DocsToSyncMsg>? docsLocalVariable, ref BindingList<DocsToSyncMsg> docsGlobalVariable)
        {
            docsGlobalVariable.Clear();
            if (docsLocalVariable != null && docsLocalVariable.Count > 0) {
                foreach (var doc in docsLocalVariable)
                    docsGlobalVariable.Add(doc);
            }
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



        private void button1_Click(object sender, EventArgs e)
        {
            ConsultarDocsConError();
        }

        private void ConsultarDocsConError()
        {
            
            var docsConError = new SincronizationBusiness().ConsultarDocsConError();
            mapDataToGrid(docsConError, ref _docsConError);
            
            if (docsConError.Count == 0)
                MessageBox.Show("No existen documentos sin sincronizar :)");
            lblError.Text = "Error: " + _docsConError.Count.ToString();
        }
    }
}
