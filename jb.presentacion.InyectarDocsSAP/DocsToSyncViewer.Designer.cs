namespace jb.presentacion.InyectarDocsSAP
{
    partial class DocsToSyncViewer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            dgvDocs = new DataGridView();
            fechaSincronizacionVendedorDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tipoDocumentoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            clienteDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            vendedorDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            totalDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            FechaIngresoSap = new DataGridViewTextBoxColumn();
            bsDocsToSyncMsg = new BindingSource(components);
            dgvMensajes = new DataGridView();
            fechaLogDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            msgDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tipoMsgDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            bsMensajesSincronizacion = new BindingSource(components);
            groupBox1 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)dgvDocs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsDocsToSyncMsg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvMensajes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsMensajesSincronizacion).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // dgvDocs
            // 
            dgvDocs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDocs.AutoGenerateColumns = false;
            dgvDocs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDocs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDocs.Columns.AddRange(new DataGridViewColumn[] { fechaSincronizacionVendedorDataGridViewTextBoxColumn, tipoDocumentoDataGridViewTextBoxColumn, clienteDataGridViewTextBoxColumn, vendedorDataGridViewTextBoxColumn, totalDataGridViewTextBoxColumn, FechaIngresoSap });
            dgvDocs.DataSource = bsDocsToSyncMsg;
            dgvDocs.Location = new Point(4, 3);
            dgvDocs.Name = "dgvDocs";
            dgvDocs.ReadOnly = true;
            dgvDocs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDocs.Size = new Size(1011, 403);
            dgvDocs.TabIndex = 0;
            dgvDocs.SelectionChanged += dgvDocs_SelectionChanged;
            // 
            // fechaSincronizacionVendedorDataGridViewTextBoxColumn
            // 
            fechaSincronizacionVendedorDataGridViewTextBoxColumn.DataPropertyName = "FechaSincronizacionVendedor";
            fechaSincronizacionVendedorDataGridViewTextBoxColumn.HeaderText = "FechaSincronizacionVendedor";
            fechaSincronizacionVendedorDataGridViewTextBoxColumn.Name = "fechaSincronizacionVendedorDataGridViewTextBoxColumn";
            fechaSincronizacionVendedorDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tipoDocumentoDataGridViewTextBoxColumn
            // 
            tipoDocumentoDataGridViewTextBoxColumn.DataPropertyName = "TipoDocumento";
            tipoDocumentoDataGridViewTextBoxColumn.HeaderText = "TipoDocumento";
            tipoDocumentoDataGridViewTextBoxColumn.Name = "tipoDocumentoDataGridViewTextBoxColumn";
            tipoDocumentoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // clienteDataGridViewTextBoxColumn
            // 
            clienteDataGridViewTextBoxColumn.DataPropertyName = "Cliente";
            clienteDataGridViewTextBoxColumn.HeaderText = "Cliente";
            clienteDataGridViewTextBoxColumn.Name = "clienteDataGridViewTextBoxColumn";
            clienteDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // vendedorDataGridViewTextBoxColumn
            // 
            vendedorDataGridViewTextBoxColumn.DataPropertyName = "Vendedor";
            vendedorDataGridViewTextBoxColumn.HeaderText = "Vendedor";
            vendedorDataGridViewTextBoxColumn.Name = "vendedorDataGridViewTextBoxColumn";
            vendedorDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // totalDataGridViewTextBoxColumn
            // 
            totalDataGridViewTextBoxColumn.DataPropertyName = "Total";
            totalDataGridViewTextBoxColumn.HeaderText = "Total";
            totalDataGridViewTextBoxColumn.Name = "totalDataGridViewTextBoxColumn";
            totalDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // FechaIngresoSap
            // 
            FechaIngresoSap.DataPropertyName = "FechaIngresoSap";
            FechaIngresoSap.HeaderText = "FechaIngresoSap";
            FechaIngresoSap.Name = "FechaIngresoSap";
            FechaIngresoSap.ReadOnly = true;
            // 
            // bsDocsToSyncMsg
            // 
            bsDocsToSyncMsg.DataSource = typeof(jbp.msg.sap.DocsToSyncMsg);
            // 
            // dgvMensajes
            // 
            dgvMensajes.AutoGenerateColumns = false;
            dgvMensajes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMensajes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMensajes.Columns.AddRange(new DataGridViewColumn[] { fechaLogDataGridViewTextBoxColumn, msgDataGridViewTextBoxColumn, tipoMsgDataGridViewTextBoxColumn });
            dgvMensajes.DataSource = bsMensajesSincronizacion;
            dgvMensajes.Dock = DockStyle.Fill;
            dgvMensajes.Location = new Point(3, 19);
            dgvMensajes.Name = "dgvMensajes";
            dgvMensajes.ReadOnly = true;
            dgvMensajes.Size = new Size(1005, 190);
            dgvMensajes.TabIndex = 1;
            // 
            // fechaLogDataGridViewTextBoxColumn
            // 
            fechaLogDataGridViewTextBoxColumn.DataPropertyName = "FechaLog";
            fechaLogDataGridViewTextBoxColumn.HeaderText = "FechaLog";
            fechaLogDataGridViewTextBoxColumn.Name = "fechaLogDataGridViewTextBoxColumn";
            fechaLogDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // msgDataGridViewTextBoxColumn
            // 
            msgDataGridViewTextBoxColumn.DataPropertyName = "Msg";
            msgDataGridViewTextBoxColumn.HeaderText = "Msg";
            msgDataGridViewTextBoxColumn.Name = "msgDataGridViewTextBoxColumn";
            msgDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tipoMsgDataGridViewTextBoxColumn
            // 
            tipoMsgDataGridViewTextBoxColumn.DataPropertyName = "TipoMsg";
            tipoMsgDataGridViewTextBoxColumn.HeaderText = "TipoMsg";
            tipoMsgDataGridViewTextBoxColumn.Name = "tipoMsgDataGridViewTextBoxColumn";
            tipoMsgDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bsMensajesSincronizacion
            // 
            bsMensajesSincronizacion.DataMember = "MensajesSincronizacion";
            bsMensajesSincronizacion.DataSource = bsDocsToSyncMsg;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(dgvMensajes);
            groupBox1.Location = new Point(7, 427);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1011, 212);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Info Sincronización";
            // 
            // DocsToSyncViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(groupBox1);
            Controls.Add(dgvDocs);
            Name = "DocsToSyncViewer";
            Size = new Size(1018, 642);
            ((System.ComponentModel.ISupportInitialize)dgvDocs).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsDocsToSyncMsg).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMensajes).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsMensajesSincronizacion).EndInit();
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dgvDocs;
        private DataGridViewTextBoxColumn fechaSincronizacionVendedorDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn tipoDocumentoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn clienteDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn vendedorDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn totalDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn FechaIngresoSap;
        private BindingSource bsDocsToSyncMsg;
        private DataGridView dgvMensajes;
        private DataGridViewTextBoxColumn fechaLogDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn msgDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn tipoMsgDataGridViewTextBoxColumn;
        private BindingSource bsMensajesSincronizacion;
        private GroupBox groupBox1;
    }
}
