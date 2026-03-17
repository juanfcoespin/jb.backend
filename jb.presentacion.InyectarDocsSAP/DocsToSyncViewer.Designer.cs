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
            tabControl2 = new TabControl();
            tabPage5 = new TabPage();
            label16 = new Label();
            bsCurrentPedido = new BindingSource(components);
            label15 = new Label();
            label14 = new Label();
            label13 = new Label();
            label11 = new Label();
            label12 = new Label();
            groupBox3 = new GroupBox();
            dataGridView2 = new DataGridView();
            codArticuloDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            articuloDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            cantSolicitadaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            cantBonificacionDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            cantBrutaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            price = new DataGridViewTextBoxColumn();
            SubTotal = new DataGridViewTextBoxColumn();
            linesBindingSource = new BindingSource(components);
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            tabPage6 = new TabPage();
            ((System.ComponentModel.ISupportInitialize)dgvDocs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsDocsToSyncMsg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvMensajes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsMensajesSincronizacion).BeginInit();
            groupBox1.SuspendLayout();
            tabControl2.SuspendLayout();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bsCurrentPedido).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)linesBindingSource).BeginInit();
            SuspendLayout();
            // 
            // dgvDocs
            // 
            dgvDocs.AllowUserToAddRows = false;
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
            dgvDocs.Size = new Size(1011, 83);
            dgvDocs.TabIndex = 0;
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
            dgvMensajes.AllowUserToAddRows = false;
            dgvMensajes.AutoGenerateColumns = false;
            dgvMensajes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMensajes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMensajes.Columns.AddRange(new DataGridViewColumn[] { fechaLogDataGridViewTextBoxColumn, msgDataGridViewTextBoxColumn, tipoMsgDataGridViewTextBoxColumn });
            dgvMensajes.DataSource = bsMensajesSincronizacion;
            dgvMensajes.Dock = DockStyle.Fill;
            dgvMensajes.Location = new Point(3, 19);
            dgvMensajes.Name = "dgvMensajes";
            dgvMensajes.ReadOnly = true;
            dgvMensajes.Size = new Size(1005, 93);
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
            groupBox1.Location = new Point(5, 92);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1011, 115);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Info Sincronización";
            // 
            // tabControl2
            // 
            tabControl2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl2.Controls.Add(tabPage5);
            tabControl2.Controls.Add(tabPage6);
            tabControl2.Location = new Point(5, 213);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(1006, 279);
            tabControl2.TabIndex = 5;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(label16);
            tabPage5.Controls.Add(label15);
            tabPage5.Controls.Add(label14);
            tabPage5.Controls.Add(label13);
            tabPage5.Controls.Add(label11);
            tabPage5.Controls.Add(label12);
            tabPage5.Controls.Add(groupBox3);
            tabPage5.Controls.Add(label10);
            tabPage5.Controls.Add(label9);
            tabPage5.Controls.Add(label8);
            tabPage5.Controls.Add(label7);
            tabPage5.Location = new Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3);
            tabPage5.Size = new Size(998, 251);
            tabPage5.TabIndex = 0;
            tabPage5.Text = "Detalle del Pedido";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.DataBindings.Add(new Binding("Text", bsCurrentPedido, "FechaIngresoSap", true));
            label16.Location = new Point(666, 33);
            label16.Name = "label16";
            label16.Size = new Size(24, 15);
            label16.TabIndex = 13;
            label16.Text = "NA";
            // 
            // bsCurrentPedido
            // 
            bsCurrentPedido.DataSource = typeof(jbp.msg.sap.OrdenMsg);
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.DataBindings.Add(new Binding("Text", bsCurrentPedido, "FechaSincronizacionVendedor", true));
            label15.Location = new Point(666, 15);
            label15.Name = "label15";
            label15.Size = new Size(24, 15);
            label15.TabIndex = 12;
            label15.Text = "NA";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.DataBindings.Add(new Binding("Text", bsCurrentPedido, "Total", true));
            label14.Location = new Point(93, 54);
            label14.Name = "label14";
            label14.Size = new Size(24, 15);
            label14.TabIndex = 11;
            label14.Text = "NA";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.DataBindings.Add(new Binding("Text", bsCurrentPedido, "Vendedor", true));
            label13.Location = new Point(93, 33);
            label13.Name = "label13";
            label13.Size = new Size(24, 15);
            label13.TabIndex = 10;
            label13.Text = "NA";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.DataBindings.Add(new Binding("Text", bsCurrentPedido, "Cliente", true));
            label11.Location = new Point(93, 15);
            label11.Name = "label11";
            label11.Size = new Size(24, 15);
            label11.TabIndex = 9;
            label11.Text = "NA";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(544, 33);
            label12.Name = "label12";
            label12.Size = new Size(116, 15);
            label12.TabIndex = 8;
            label12.Text = "Fecha Ingreso a SAP:";
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(dataGridView2);
            groupBox3.Location = new Point(19, 72);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(954, 173);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "Líneas del Pedido";
            // 
            // dataGridView2
            // 
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Columns.AddRange(new DataGridViewColumn[] { codArticuloDataGridViewTextBoxColumn, articuloDataGridViewTextBoxColumn, cantSolicitadaDataGridViewTextBoxColumn, cantBonificacionDataGridViewTextBoxColumn, cantBrutaDataGridViewTextBoxColumn, price, SubTotal });
            dataGridView2.DataSource = linesBindingSource;
            dataGridView2.Dock = DockStyle.Fill;
            dataGridView2.Location = new Point(3, 19);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.Size = new Size(948, 151);
            dataGridView2.TabIndex = 5;
            // 
            // codArticuloDataGridViewTextBoxColumn
            // 
            codArticuloDataGridViewTextBoxColumn.DataPropertyName = "CodArticulo";
            codArticuloDataGridViewTextBoxColumn.HeaderText = "CodArticulo";
            codArticuloDataGridViewTextBoxColumn.Name = "codArticuloDataGridViewTextBoxColumn";
            codArticuloDataGridViewTextBoxColumn.Width = 96;
            // 
            // articuloDataGridViewTextBoxColumn
            // 
            articuloDataGridViewTextBoxColumn.DataPropertyName = "Articulo";
            articuloDataGridViewTextBoxColumn.HeaderText = "Articulo";
            articuloDataGridViewTextBoxColumn.Name = "articuloDataGridViewTextBoxColumn";
            articuloDataGridViewTextBoxColumn.Width = 74;
            // 
            // cantSolicitadaDataGridViewTextBoxColumn
            // 
            cantSolicitadaDataGridViewTextBoxColumn.DataPropertyName = "CantSolicitada";
            cantSolicitadaDataGridViewTextBoxColumn.HeaderText = "CantSolicitada";
            cantSolicitadaDataGridViewTextBoxColumn.Name = "cantSolicitadaDataGridViewTextBoxColumn";
            cantSolicitadaDataGridViewTextBoxColumn.Width = 108;
            // 
            // cantBonificacionDataGridViewTextBoxColumn
            // 
            cantBonificacionDataGridViewTextBoxColumn.DataPropertyName = "CantBonificacion";
            cantBonificacionDataGridViewTextBoxColumn.HeaderText = "CantBonificacion";
            cantBonificacionDataGridViewTextBoxColumn.Name = "cantBonificacionDataGridViewTextBoxColumn";
            cantBonificacionDataGridViewTextBoxColumn.Width = 123;
            // 
            // cantBrutaDataGridViewTextBoxColumn
            // 
            cantBrutaDataGridViewTextBoxColumn.DataPropertyName = "CantBruta";
            cantBrutaDataGridViewTextBoxColumn.HeaderText = "CantBruta";
            cantBrutaDataGridViewTextBoxColumn.Name = "cantBrutaDataGridViewTextBoxColumn";
            cantBrutaDataGridViewTextBoxColumn.ReadOnly = true;
            cantBrutaDataGridViewTextBoxColumn.Width = 85;
            // 
            // price
            // 
            price.DataPropertyName = "price";
            price.HeaderText = "PrecioUnitario";
            price.Name = "price";
            price.Width = 107;
            // 
            // SubTotal
            // 
            SubTotal.DataPropertyName = "SubTotal";
            SubTotal.HeaderText = "SubTotal";
            SubTotal.Name = "SubTotal";
            SubTotal.ReadOnly = true;
            SubTotal.Width = 78;
            // 
            // linesBindingSource
            // 
            linesBindingSource.DataMember = "Lines";
            linesBindingSource.DataSource = bsCurrentPedido;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(486, 15);
            label10.Name = "label10";
            label10.Size = new Size(174, 15);
            label10.TabIndex = 4;
            label10.Text = "Fecha Sincronizacion Vendedor:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(30, 54);
            label9.Name = "label9";
            label9.Size = new Size(36, 15);
            label9.TabIndex = 3;
            label9.Text = "Total:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(29, 15);
            label8.Name = "label8";
            label8.Size = new Size(47, 15);
            label8.TabIndex = 2;
            label8.Text = "Cliente:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(29, 33);
            label7.Name = "label7";
            label7.Size = new Size(60, 15);
            label7.TabIndex = 1;
            label7.Text = "Vendedor:";
            // 
            // tabPage6
            // 
            tabPage6.Location = new Point(4, 24);
            tabPage6.Name = "tabPage6";
            tabPage6.Padding = new Padding(3);
            tabPage6.Size = new Size(998, 251);
            tabPage6.TabIndex = 1;
            tabPage6.Text = "Detalle Cobro";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // DocsToSyncViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tabControl2);
            Controls.Add(groupBox1);
            Controls.Add(dgvDocs);
            Name = "DocsToSyncViewer";
            Size = new Size(1018, 505);
            ((System.ComponentModel.ISupportInitialize)dgvDocs).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsDocsToSyncMsg).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMensajes).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsMensajesSincronizacion).EndInit();
            groupBox1.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bsCurrentPedido).EndInit();
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)linesBindingSource).EndInit();
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
        private TabControl tabControl2;
        private TabPage tabPage5;
        private Label label16;
        private Label label15;
        private Label label14;
        private Label label13;
        private Label label11;
        private Label label12;
        private GroupBox groupBox3;
        private DataGridView dataGridView2;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private TabPage tabPage6;
        private BindingSource bsCurrentPedido;
        private DataGridViewTextBoxColumn codArticuloDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn articuloDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantSolicitadaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantBonificacionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantBrutaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn price;
        private DataGridViewTextBoxColumn SubTotal;
        private BindingSource linesBindingSource;
    }
}
