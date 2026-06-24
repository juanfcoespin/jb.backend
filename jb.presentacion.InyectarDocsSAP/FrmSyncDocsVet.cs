using jbp.business.hana;
using jbp.msg.sap;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using static jbp.business.hana.SincronizationBusiness;
using TechTools.Utils;

namespace jb.presentacion.InyectarDocsSAP
{
    public partial class frmSyncAppVET : Form
    {
        private BindingList<DocsToSyncMsg> _docsToSync = new BindingList<DocsToSyncMsg>();
        private BindingList<DocsToSyncMsg> _docsSincronizados = new BindingList<DocsToSyncMsg>();
        private BindingList<DocsToSyncMsg> _docsConError = new BindingList<DocsToSyncMsg>();
        private BindingList<DocsToSyncMsg> _docsResultado = new BindingList<DocsToSyncMsg>();

        FiltroHistoricoMsg _filtroConsultaHistorico;
        private bool procesando = false;
        private readonly object _lockStatus = new object();
        private readonly object _lockOK = new object();
        private readonly object _lockError = new object();
        private readonly object _lockOnDocsToSync = new object();

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
            ctrlDocsLogs.SetData(_docsSincronizados);
            ctrlDocsError.SetData(_docsConError);
            ctrResultado.SetData(_docsResultado);


        }
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
            Logger.Info("TIMER TICK");
            lblFechaUltimaConsulta.Text = DateTime.Now.ToString();
            if (procesando)
                return;
            procesando = true;
            try
            {
                Logger.Info("INICIO PROCESO");
                await Task.Run(() => procesarDocumentos());
                Logger.Info("FIN PROCESO");
            }
            catch (Exception ex)
            {
                Logger.Error("ERROR EN TIMER");
                Logger.Error(ex);
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
                Logger.Error("RunOnUI ERROR");
                Logger.Error(ex);
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
            {
                Logger.Info("SKIP - business ya ejecutando");
                return;
            }
            try
            {
                _ejecutandoBusiness = true;
                Logger.Info("LLAMANDO SincronizarPedidoYCobros()");
                _syncBusiness.SincronizarPedidoYCobros();
                Logger.Info("FIN SincronizarPedidoYCobros()");
            }
            catch (Exception ex)
            {
                Logger.Error("ERROR EN procesarDocumentos");
                Logger.Error(ex);
                throw;
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

            cmdIniciar.PerformClick();
        }

        //// evento para atrapar el motivo de cierre inesperado y evitar cierre
        private void frmSyncAppVET_FormClosing(object sender, FormClosingEventArgs e)
        {
        //    // Loguear para diagnóstico
        //    var motivo = e.CloseReason.ToString();
        //    Logger.Info($"FORM CLOSING - Motivo: {motivo}, Timer activo: {timer1?.Enabled}, Procesando: {procesando}");

        //    // Si no es cierre intencional por el usuario, prevenir y minimizar
        //    if (e.CloseReason != CloseReason.UserClosing && e.CloseReason != CloseReason.ApplicationExitCall)
        //    {
        //        Logger.Info("Intento de cierre no intencional - previniendo cierre");
        //        e.Cancel = true;
        //        this.WindowState = FormWindowState.Minimized;
        //        return;
        //    }
        }

        private void inicializarEventosBussiness()
        {
            _syncBusiness.onError += (err) =>
            {
                Logger.Error("onError: " + err);
                // cuando se da un error se para el timer y mustra el msg
                //errores no controlados en la capa de negocio
                RunOnUI(() =>
                {
                    lock (_lockError)
                    {
                        //iniciar(false);
                        showError(err);
                    }
                });
            };
            
            _syncBusiness.onDocsToSync += (docs) =>
            {
                Logger.Info($"onDocsToSync: {docs.Count} docs");

                if (docs.Count == 0)
                    return;
                //muestra los documentos a sincronizar
                // no se hace el lock porque esto se controla en la capa de negocio
                RunOnUI(() =>
                {
                    lock (_lockOnDocsToSync) {
                        mapDataToGrid(docs, _docsToSync);
                    }
                    
                });
            };
            //actualizaciones de sincronización o error
            _syncBusiness.onChangeSyncStatus += (docOnSync) =>
            {
                Logger.Info($"onChangeSyncStatus: {docOnSync.IdCache}, tipo {docOnSync.TipoDocumento}");
                //actualiza el estado de la sincronización
                RunOnUI(() =>
                {
                    lock (_lockStatus)
                    {
                        try
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
                                    Logger.Error($"DOC ERROR: {doc.IdCache}, error: {docOnSync.MensajesSincronizacion}");
                                    _docsConError.Add(doc);
                                    lblError.Text = "Docs con Error: " + _docsConError.Count.ToString();
                                }
                            }
                        }
                        catch (Exception e) {
                            Logger.Error("onChangeSyncStatus");
                            Logger.Error(e);
                        }
                        
                    }
                });
            };
            //Cuando se sincroniza mando al log
            _syncBusiness.onDocSyncOK += (docSincronizado) =>
            {
                Logger.Info($"onDocSyncOK: {docSincronizado.IdCache}, tipo {docSincronizado.TipoDocumento}");
                RunOnUI(() =>
                {
                    lock (_lockOK) {
                        _docsSincronizados.Add(docSincronizado);
                        if (_docsToSync.Count > 0) { 
                            var index=_docsToSync.IndexOf(docSincronizado);
                            if (index >= 0)
                                _docsToSync.RemoveAt(index);
                        }
                        lblOk.Text = "Docs Sincronizados: " + _docsSincronizados.Count.ToString();
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
            mapDataToGrid(docs,  _docsResultado);
            if (_docsResultado.Count == 0)
                MessageBox.Show("No se han encontrador resultados con el filtro aplicado!!");
            cmdConsultarHistorico.Enabled = true;
        }

        private readonly object _lockGrid = new object();
        private void mapDataToGrid(List<DocsToSyncMsg>? docsLocalVariable, BindingList<DocsToSyncMsg> docsGlobalVariable)
        {
            try
            {
                lock (_lockGrid)
                {
                    Logger.Info("mapDataToGrid START");

                    //suspende eventos hacia la UI
                    docsGlobalVariable.RaiseListChangedEvents = false;

                    docsGlobalVariable.Clear();

                    if (docsLocalVariable != null)
                    {
                        foreach (var doc in docsLocalVariable)
                            docsGlobalVariable.Add(doc);
                    }

                    // reactiva eventos
                    docsGlobalVariable.RaiseListChangedEvents = true;

                    // fuerza refresh del binding
                    docsGlobalVariable.ResetBindings();

                    Logger.Info($"mapDataToGrid OK - {docsGlobalVariable.Count} items");
                }
            }
            catch (Exception e)
            {
                Logger.Error("ERROR en mapDataToGrid");
                Logger.Error(e);
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
            mapDataToGrid(docsConError, _docsConError);
            
            if (docsConError.Count == 0)
                MessageBox.Show("No existen documentos sin sincronizar :)");
            lblError.Text = "Docs con Error: " + _docsConError.Count.ToString();
        }
    }
}
