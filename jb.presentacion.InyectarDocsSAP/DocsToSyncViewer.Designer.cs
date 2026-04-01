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
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
            bsDocsToSync = new BindingSource(components);
            dgvMensajes = new DataGridView();
            fechaLogDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            msgDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tipoMsgDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            mensajesSincronizacionBindingSource = new BindingSource(components);
            groupBox1 = new GroupBox();
            tabDetalleDocToSync = new TabControl();
            tpPedido = new TabPage();
            groupBox3 = new GroupBox();
            dataGridView2 = new DataGridView();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn11 = new DataGridViewTextBoxColumn();
            price = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn9 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn10 = new DataGridViewTextBoxColumn();
            SubTotal = new DataGridViewTextBoxColumn();
            ordenLinesMsgBindingSource = new BindingSource(components);
            tpCobro = new TabPage();
            tabControl1 = new TabControl();
            tabPage2 = new TabPage();
            groupBox2 = new GroupBox();
            dataGridView4 = new DataGridView();
            bancoTxtDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            monto = new DataGridViewTextBoxColumn();
            fechaVencimientoChequeStrDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            NumCheque = new DataGridViewTextBoxColumn();
            Posfechado = new DataGridViewTextBoxColumn();
            chequesBindingSource = new BindingSource(components);
            tipoPagoMsgBindingSource = new BindingSource(components);
            dataGridView3 = new DataGridView();
            tipoPagoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            montoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            numTransferenciaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            bancoTxtDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            saldoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tabPage1 = new TabPage();
            dataGridView1 = new DataGridView();
            numDocDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tipoDocumentoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            totalDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            toPayDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            dateDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            pagadoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            porcentajePPDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            descuentoPPDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            valorPagadoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            docCarteraMsgBindingSource = new BindingSource(components);
            label16 = new Label();
            label15 = new Label();
            label14 = new Label();
            label13 = new Label();
            label12 = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            label1 = new Label();
            label3 = new Label();
            label4 = new Label();
            textBox1 = new TextBox();
            ordenMsgBindingSource = new BindingSource(components);
            textBox2 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvDocs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsDocsToSync).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvMensajes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)mensajesSincronizacionBindingSource).BeginInit();
            groupBox1.SuspendLayout();
            tabDetalleDocToSync.SuspendLayout();
            tpPedido.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ordenLinesMsgBindingSource).BeginInit();
            tpCobro.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chequesBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tipoPagoMsgBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).BeginInit();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)docCarteraMsgBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ordenMsgBindingSource).BeginInit();
            SuspendLayout();
            // 
            // dgvDocs
            // 
            dgvDocs.AllowUserToAddRows = false;
            dgvDocs.AllowUserToOrderColumns = true;
            dgvDocs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDocs.AutoGenerateColumns = false;
            dgvDocs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDocs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDocs.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn7, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn5 });
            dgvDocs.DataSource = bsDocsToSync;
            dgvDocs.Location = new Point(4, 3);
            dgvDocs.Name = "dgvDocs";
            dgvDocs.ReadOnly = true;
            dgvDocs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDocs.Size = new Size(1019, 137);
            dgvDocs.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.DataPropertyName = "FechaSincronizacionVendedor";
            dataGridViewTextBoxColumn1.HeaderText = "FechaSincronizacionVendedor";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn7
            // 
            dataGridViewTextBoxColumn7.DataPropertyName = "TipoDocumento";
            dataGridViewTextBoxColumn7.HeaderText = "TipoDocumento";
            dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            dataGridViewTextBoxColumn7.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.DataPropertyName = "Cliente";
            dataGridViewTextBoxColumn3.HeaderText = "Cliente";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            dataGridViewTextBoxColumn6.DataPropertyName = "Vendedor";
            dataGridViewTextBoxColumn6.HeaderText = "Vendedor";
            dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            dataGridViewTextBoxColumn5.DataPropertyName = "Total";
            dataGridViewTextBoxColumn5.HeaderText = "Total";
            dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // bsDocsToSync
            // 
            bsDocsToSync.DataSource = typeof(jbp.msg.sap.DocsToSyncMsg);
            // 
            // dgvMensajes
            // 
            dgvMensajes.AllowUserToAddRows = false;
            dgvMensajes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvMensajes.AutoGenerateColumns = false;
            dgvMensajes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMensajes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMensajes.Columns.AddRange(new DataGridViewColumn[] { fechaLogDataGridViewTextBoxColumn, msgDataGridViewTextBoxColumn, tipoMsgDataGridViewTextBoxColumn });
            dgvMensajes.DataSource = mensajesSincronizacionBindingSource;
            dgvMensajes.Location = new Point(3, 19);
            dgvMensajes.Name = "dgvMensajes";
            dgvMensajes.ReadOnly = true;
            dgvMensajes.Size = new Size(1013, 119);
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
            // mensajesSincronizacionBindingSource
            // 
            mensajesSincronizacionBindingSource.DataMember = "MensajesSincronizacion";
            mensajesSincronizacionBindingSource.DataSource = bsDocsToSync;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(dgvMensajes);
            groupBox1.Location = new Point(5, 146);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1019, 141);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Info Sincronización";
            // 
            // tabDetalleDocToSync
            // 
            tabDetalleDocToSync.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabDetalleDocToSync.Controls.Add(tpPedido);
            tabDetalleDocToSync.Controls.Add(tpCobro);
            tabDetalleDocToSync.Location = new Point(5, 365);
            tabDetalleDocToSync.Name = "tabDetalleDocToSync";
            tabDetalleDocToSync.SelectedIndex = 0;
            tabDetalleDocToSync.Size = new Size(1019, 192);
            tabDetalleDocToSync.TabIndex = 5;
            // 
            // tpPedido
            // 
            tpPedido.Controls.Add(groupBox3);
            tpPedido.Location = new Point(4, 24);
            tpPedido.Name = "tpPedido";
            tpPedido.Padding = new Padding(3);
            tpPedido.Size = new Size(1011, 164);
            tpPedido.TabIndex = 0;
            tpPedido.Text = "Detalle del Pedido";
            tpPedido.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(dataGridView2);
            groupBox3.Dock = DockStyle.Fill;
            groupBox3.Location = new Point(3, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(1005, 158);
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
            dataGridView2.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn11, price, dataGridViewTextBoxColumn8, dataGridViewTextBoxColumn9, dataGridViewTextBoxColumn10, SubTotal });
            dataGridView2.DataSource = ordenLinesMsgBindingSource;
            dataGridView2.Dock = DockStyle.Fill;
            dataGridView2.Location = new Point(3, 19);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.Size = new Size(999, 136);
            dataGridView2.TabIndex = 5;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.DataPropertyName = "CodArticulo";
            dataGridViewTextBoxColumn2.HeaderText = "CodArticulo";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.Width = 96;
            // 
            // dataGridViewTextBoxColumn11
            // 
            dataGridViewTextBoxColumn11.DataPropertyName = "Articulo";
            dataGridViewTextBoxColumn11.HeaderText = "Articulo";
            dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            dataGridViewTextBoxColumn11.Width = 74;
            // 
            // price
            // 
            price.DataPropertyName = "price";
            price.HeaderText = "PrecioUnitario";
            price.Name = "price";
            price.Width = 107;
            // 
            // dataGridViewTextBoxColumn8
            // 
            dataGridViewTextBoxColumn8.DataPropertyName = "CantSolicitada";
            dataGridViewTextBoxColumn8.HeaderText = "CantSolicitada";
            dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            dataGridViewTextBoxColumn8.Width = 108;
            // 
            // dataGridViewTextBoxColumn9
            // 
            dataGridViewTextBoxColumn9.DataPropertyName = "CantBonificacion";
            dataGridViewTextBoxColumn9.HeaderText = "CantBonificacion";
            dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            dataGridViewTextBoxColumn9.Width = 123;
            // 
            // dataGridViewTextBoxColumn10
            // 
            dataGridViewTextBoxColumn10.DataPropertyName = "CantBruta";
            dataGridViewTextBoxColumn10.HeaderText = "CantBruta";
            dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            dataGridViewTextBoxColumn10.ReadOnly = true;
            dataGridViewTextBoxColumn10.Width = 85;
            // 
            // SubTotal
            // 
            SubTotal.DataPropertyName = "SubTotal";
            SubTotal.HeaderText = "SubTotal";
            SubTotal.Name = "SubTotal";
            SubTotal.ReadOnly = true;
            SubTotal.Width = 78;
            // 
            // ordenLinesMsgBindingSource
            // 
            ordenLinesMsgBindingSource.DataSource = typeof(jbp.msg.sap.OrdenLinesMsg);
            // 
            // tpCobro
            // 
            tpCobro.Controls.Add(tabControl1);
            tpCobro.Location = new Point(4, 24);
            tpCobro.Name = "tpCobro";
            tpCobro.Padding = new Padding(3);
            tpCobro.Size = new Size(1011, 164);
            tpCobro.TabIndex = 1;
            tpCobro.Text = "Detalle Cobro";
            tpCobro.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(3, 3);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1005, 158);
            tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Controls.Add(dataGridView3);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(997, 130);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Tipos Pago";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(dataGridView4);
            groupBox2.Location = new Point(564, 6);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(427, 118);
            groupBox2.TabIndex = 16;
            groupBox2.TabStop = false;
            groupBox2.Text = "Detalle Cheques";
            // 
            // dataGridView4
            // 
            dataGridView4.AllowUserToAddRows = false;
            dataGridView4.AllowUserToDeleteRows = false;
            dataGridView4.AllowUserToOrderColumns = true;
            dataGridView4.AutoGenerateColumns = false;
            dataGridView4.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView4.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView4.Columns.AddRange(new DataGridViewColumn[] { bancoTxtDataGridViewTextBoxColumn1, monto, fechaVencimientoChequeStrDataGridViewTextBoxColumn, NumCheque, Posfechado });
            dataGridView4.DataSource = chequesBindingSource;
            dataGridView4.Dock = DockStyle.Fill;
            dataGridView4.Location = new Point(3, 19);
            dataGridView4.Name = "dataGridView4";
            dataGridView4.ReadOnly = true;
            dataGridView4.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView4.Size = new Size(421, 96);
            dataGridView4.TabIndex = 16;
            // 
            // bancoTxtDataGridViewTextBoxColumn1
            // 
            bancoTxtDataGridViewTextBoxColumn1.DataPropertyName = "bancoTxt";
            bancoTxtDataGridViewTextBoxColumn1.HeaderText = "bancoTxt";
            bancoTxtDataGridViewTextBoxColumn1.Name = "bancoTxtDataGridViewTextBoxColumn1";
            bancoTxtDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // monto
            // 
            monto.DataPropertyName = "monto";
            monto.HeaderText = "Monto";
            monto.Name = "monto";
            monto.ReadOnly = true;
            // 
            // fechaVencimientoChequeStrDataGridViewTextBoxColumn
            // 
            fechaVencimientoChequeStrDataGridViewTextBoxColumn.DataPropertyName = "FechaVencimientoChequeStr";
            fechaVencimientoChequeStrDataGridViewTextBoxColumn.HeaderText = "FechaVencimientoChequeStr";
            fechaVencimientoChequeStrDataGridViewTextBoxColumn.Name = "fechaVencimientoChequeStrDataGridViewTextBoxColumn";
            fechaVencimientoChequeStrDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // NumCheque
            // 
            NumCheque.DataPropertyName = "NumCheque";
            NumCheque.HeaderText = "NumCheque";
            NumCheque.Name = "NumCheque";
            NumCheque.ReadOnly = true;
            // 
            // Posfechado
            // 
            Posfechado.DataPropertyName = "Posfechado";
            Posfechado.HeaderText = "Posfechado";
            Posfechado.Name = "Posfechado";
            Posfechado.ReadOnly = true;
            // 
            // chequesBindingSource
            // 
            chequesBindingSource.DataMember = "cheques";
            chequesBindingSource.DataSource = tipoPagoMsgBindingSource;
            // 
            // tipoPagoMsgBindingSource
            // 
            tipoPagoMsgBindingSource.DataSource = typeof(jbp.msg.sap.TipoPagoMsg);
            // 
            // dataGridView3
            // 
            dataGridView3.AllowUserToAddRows = false;
            dataGridView3.AutoGenerateColumns = false;
            dataGridView3.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView3.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView3.Columns.AddRange(new DataGridViewColumn[] { tipoPagoDataGridViewTextBoxColumn, montoDataGridViewTextBoxColumn, numTransferenciaDataGridViewTextBoxColumn, bancoTxtDataGridViewTextBoxColumn, saldoDataGridViewTextBoxColumn });
            dataGridView3.DataSource = tipoPagoMsgBindingSource;
            dataGridView3.Location = new Point(3, 3);
            dataGridView3.Name = "dataGridView3";
            dataGridView3.ReadOnly = true;
            dataGridView3.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView3.Size = new Size(555, 124);
            dataGridView3.TabIndex = 15;
            // 
            // tipoPagoDataGridViewTextBoxColumn
            // 
            tipoPagoDataGridViewTextBoxColumn.DataPropertyName = "tipoPago";
            tipoPagoDataGridViewTextBoxColumn.HeaderText = "tipoPago";
            tipoPagoDataGridViewTextBoxColumn.Name = "tipoPagoDataGridViewTextBoxColumn";
            tipoPagoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // montoDataGridViewTextBoxColumn
            // 
            montoDataGridViewTextBoxColumn.DataPropertyName = "monto";
            montoDataGridViewTextBoxColumn.HeaderText = "monto";
            montoDataGridViewTextBoxColumn.Name = "montoDataGridViewTextBoxColumn";
            montoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // numTransferenciaDataGridViewTextBoxColumn
            // 
            numTransferenciaDataGridViewTextBoxColumn.DataPropertyName = "NumTransferencia";
            numTransferenciaDataGridViewTextBoxColumn.HeaderText = "NumTransferencia";
            numTransferenciaDataGridViewTextBoxColumn.Name = "numTransferenciaDataGridViewTextBoxColumn";
            numTransferenciaDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // bancoTxtDataGridViewTextBoxColumn
            // 
            bancoTxtDataGridViewTextBoxColumn.DataPropertyName = "bancoTxt";
            bancoTxtDataGridViewTextBoxColumn.HeaderText = "bancoTxt";
            bancoTxtDataGridViewTextBoxColumn.Name = "bancoTxtDataGridViewTextBoxColumn";
            bancoTxtDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // saldoDataGridViewTextBoxColumn
            // 
            saldoDataGridViewTextBoxColumn.DataPropertyName = "saldo";
            saldoDataGridViewTextBoxColumn.HeaderText = "saldo";
            saldoDataGridViewTextBoxColumn.Name = "saldoDataGridViewTextBoxColumn";
            saldoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dataGridView1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(997, 130);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Facturas";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { numDocDataGridViewTextBoxColumn, tipoDocumentoDataGridViewTextBoxColumn, totalDataGridViewTextBoxColumn, toPayDataGridViewTextBoxColumn, dateDataGridViewTextBoxColumn, pagadoDataGridViewTextBoxColumn, porcentajePPDataGridViewTextBoxColumn, descuentoPPDataGridViewTextBoxColumn, valorPagadoDataGridViewTextBoxColumn });
            dataGridView1.DataSource = docCarteraMsgBindingSource;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(3, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(991, 124);
            dataGridView1.TabIndex = 14;
            // 
            // numDocDataGridViewTextBoxColumn
            // 
            numDocDataGridViewTextBoxColumn.DataPropertyName = "numDoc";
            numDocDataGridViewTextBoxColumn.HeaderText = "numDoc";
            numDocDataGridViewTextBoxColumn.Name = "numDocDataGridViewTextBoxColumn";
            numDocDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // tipoDocumentoDataGridViewTextBoxColumn
            // 
            tipoDocumentoDataGridViewTextBoxColumn.DataPropertyName = "tipoDocumento";
            tipoDocumentoDataGridViewTextBoxColumn.HeaderText = "tipoDocumento";
            tipoDocumentoDataGridViewTextBoxColumn.Name = "tipoDocumentoDataGridViewTextBoxColumn";
            tipoDocumentoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // totalDataGridViewTextBoxColumn
            // 
            totalDataGridViewTextBoxColumn.DataPropertyName = "total";
            totalDataGridViewTextBoxColumn.HeaderText = "total";
            totalDataGridViewTextBoxColumn.Name = "totalDataGridViewTextBoxColumn";
            totalDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // toPayDataGridViewTextBoxColumn
            // 
            toPayDataGridViewTextBoxColumn.DataPropertyName = "toPay";
            toPayDataGridViewTextBoxColumn.HeaderText = "toPay";
            toPayDataGridViewTextBoxColumn.Name = "toPayDataGridViewTextBoxColumn";
            toPayDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dateDataGridViewTextBoxColumn
            // 
            dateDataGridViewTextBoxColumn.DataPropertyName = "date";
            dateDataGridViewTextBoxColumn.HeaderText = "date";
            dateDataGridViewTextBoxColumn.Name = "dateDataGridViewTextBoxColumn";
            dateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // pagadoDataGridViewTextBoxColumn
            // 
            pagadoDataGridViewTextBoxColumn.DataPropertyName = "pagado";
            pagadoDataGridViewTextBoxColumn.HeaderText = "pagado";
            pagadoDataGridViewTextBoxColumn.Name = "pagadoDataGridViewTextBoxColumn";
            pagadoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // porcentajePPDataGridViewTextBoxColumn
            // 
            porcentajePPDataGridViewTextBoxColumn.DataPropertyName = "porcentajePP";
            porcentajePPDataGridViewTextBoxColumn.HeaderText = "porcentajePP";
            porcentajePPDataGridViewTextBoxColumn.Name = "porcentajePPDataGridViewTextBoxColumn";
            porcentajePPDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // descuentoPPDataGridViewTextBoxColumn
            // 
            descuentoPPDataGridViewTextBoxColumn.DataPropertyName = "descuentoPP";
            descuentoPPDataGridViewTextBoxColumn.HeaderText = "descuentoPP";
            descuentoPPDataGridViewTextBoxColumn.Name = "descuentoPPDataGridViewTextBoxColumn";
            descuentoPPDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // valorPagadoDataGridViewTextBoxColumn
            // 
            valorPagadoDataGridViewTextBoxColumn.DataPropertyName = "valorPagado";
            valorPagadoDataGridViewTextBoxColumn.HeaderText = "valorPagado";
            valorPagadoDataGridViewTextBoxColumn.Name = "valorPagadoDataGridViewTextBoxColumn";
            valorPagadoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // docCarteraMsgBindingSource
            // 
            docCarteraMsgBindingSource.DataSource = typeof(jbp.msg.sap.DocCarteraMsg);
            // 
            // label16
            // 
            label16.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label16.AutoSize = true;
            label16.DataBindings.Add(new Binding("Text", bsDocsToSync, "FechaIngresoSap", true));
            label16.Location = new Point(461, 328);
            label16.Name = "label16";
            label16.Size = new Size(24, 15);
            label16.TabIndex = 13;
            label16.Text = "NA";
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label15.AutoSize = true;
            label15.DataBindings.Add(new Binding("Text", bsDocsToSync, "FechaSincronizacionVendedor", true));
            label15.Location = new Point(461, 309);
            label15.Name = "label15";
            label15.Size = new Size(24, 15);
            label15.TabIndex = 12;
            label15.Text = "NA";
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label14.AutoSize = true;
            label14.DataBindings.Add(new Binding("Text", bsDocsToSync, "Total", true));
            label14.Location = new Point(461, 345);
            label14.Name = "label14";
            label14.Size = new Size(24, 15);
            label14.TabIndex = 11;
            label14.Text = "NA";
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label13.AutoSize = true;
            label13.DataBindings.Add(new Binding("Text", bsDocsToSync, "Vendedor", true));
            label13.Location = new Point(78, 346);
            label13.Name = "label13";
            label13.Size = new Size(24, 15);
            label13.TabIndex = 10;
            label13.Text = "NA";
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label12.Location = new Point(334, 327);
            label12.Name = "label12";
            label12.Size = new Size(121, 15);
            label12.TabIndex = 8;
            label12.Text = "Fecha Ingreso a SAP:";
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label10.AutoSize = true;
            label10.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label10.Location = new Point(327, 309);
            label10.Name = "label10";
            label10.Size = new Size(128, 15);
            label10.TabIndex = 4;
            label10.Text = "Fecha Sync Vendedor:";
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label9.Location = new Point(418, 344);
            label9.Name = "label9";
            label9.Size = new Size(37, 15);
            label9.TabIndex = 3;
            label9.Text = "Total:";
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label8.Location = new Point(14, 325);
            label8.Name = "label8";
            label8.Size = new Size(49, 15);
            label8.TabIndex = 2;
            label8.Text = "Cliente:";
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label7.AutoSize = true;
            label7.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label7.Location = new Point(14, 345);
            label7.Name = "label7";
            label7.Size = new Size(64, 15);
            label7.TabIndex = 1;
            label7.Text = "Vendedor:";
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(601, 309);
            label1.Name = "label1";
            label1.Size = new Size(121, 15);
            label1.TabIndex = 14;
            label1.Text = "Error Sincronización:";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.AutoSize = true;
            label3.DataBindings.Add(new Binding("Text", bsDocsToSync, "IdCache", true));
            label3.Location = new Point(78, 306);
            label3.Name = "label3";
            label3.Size = new Size(24, 15);
            label3.TabIndex = 17;
            label3.Text = "NA";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label4.Location = new Point(15, 306);
            label4.Name = "label4";
            label4.Size = new Size(57, 15);
            label4.TabIndex = 16;
            label4.Text = "Id Cache:";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.DataBindings.Add(new Binding("Text", bsDocsToSync, "Error", true));
            textBox1.Location = new Point(603, 327);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(414, 34);
            textBox1.TabIndex = 18;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            textBox2.DataBindings.Add(new Binding("Text", bsDocsToSync, "Cliente", true));
            textBox2.Location = new Point(79, 322);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(242, 23);
            textBox2.TabIndex = 19;
            // 
            // DocsToSyncViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(label1);
            Controls.Add(label16);
            Controls.Add(tabDetalleDocToSync);
            Controls.Add(label15);
            Controls.Add(groupBox1);
            Controls.Add(label14);
            Controls.Add(dgvDocs);
            Controls.Add(label13);
            Controls.Add(label10);
            Controls.Add(label7);
            Controls.Add(label12);
            Controls.Add(label8);
            Controls.Add(label9);
            Name = "DocsToSyncViewer";
            Size = new Size(1026, 570);
            ((System.ComponentModel.ISupportInitialize)dgvDocs).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsDocsToSync).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMensajes).EndInit();
            ((System.ComponentModel.ISupportInitialize)mensajesSincronizacionBindingSource).EndInit();
            groupBox1.ResumeLayout(false);
            tabDetalleDocToSync.ResumeLayout(false);
            tpPedido.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)ordenLinesMsgBindingSource).EndInit();
            tpCobro.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView4).EndInit();
            ((System.ComponentModel.ISupportInitialize)chequesBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)tipoPagoMsgBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).EndInit();
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ((System.ComponentModel.ISupportInitialize)docCarteraMsgBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)ordenMsgBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dgvDocs;
        private DataGridViewTextBoxColumn fechaSincronizacionVendedorDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn clienteDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn vendedorDataGridViewTextBoxColumn;
        private DataGridView dgvMensajes;
        private GroupBox groupBox1;
        private TabControl tabDetalleDocToSync;
        private TabPage tpPedido;
        private Label label16;
        private Label label15;
        private Label label14;
        private Label label13;
        private Label label12;
        private GroupBox groupBox3;
        private DataGridView dataGridView2;
        private Label label10;
        private Label label9;
        private Label label8;
        private Label label7;
        private TabPage tpCobro;
        private DataGridViewTextBoxColumn codArticuloDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn articuloDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantSolicitadaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantBonificacionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantBrutaDataGridViewTextBoxColumn;
        private DataGridView dataGridView1;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridView dataGridView3;
        private DataGridViewTextBoxColumn totalDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn tipoDocumentoDataGridViewTextBoxColumn1;
        private DataGridView dataGridView4;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private GroupBox groupBox2;
        private Label label1;
        private Label label3;
        private Label label4;
        private TextBox textBox1;
        private BindingSource ordenMsgBindingSource;
        private BindingSource bsDocsToSync;
        private BindingSource mensajesSincronizacionBindingSource;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private BindingSource tipoPagoMsgBindingSource;
        private DataGridViewTextBoxColumn bancoTxtDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn monto;
        private DataGridViewTextBoxColumn fechaVencimientoChequeStrDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn NumCheque;
        private DataGridViewTextBoxColumn Posfechado;
        private BindingSource chequesBindingSource;
        private BindingSource ordenLinesMsgBindingSource;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private DataGridViewTextBoxColumn price;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private DataGridViewTextBoxColumn SubTotal;
        private DataGridViewTextBoxColumn tipoPagoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn montoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn numTransferenciaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn bancoTxtDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn saldoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fechaLogDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn msgDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn tipoMsgDataGridViewTextBoxColumn;
        private TextBox textBox2;
        private BindingSource docCarteraMsgBindingSource;
        private DataGridViewTextBoxColumn numDocDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn tipoDocumentoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn totalDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn toPayDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dateDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn pagadoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn porcentajePPDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn descuentoPPDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn valorPagadoDataGridViewTextBoxColumn;
    }
}
