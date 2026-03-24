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
            tabDetalleDocToSync = new TabControl();
            tpPedido = new TabPage();
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
            bsCurrentDocToSync = new BindingSource(components);
            tpCobro = new TabPage();
            tabControl1 = new TabControl();
            tabPage2 = new TabPage();
            groupBox2 = new GroupBox();
            dataGridView4 = new DataGridView();
            dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
            monto = new DataGridViewTextBoxColumn();
            FechaVencimientoChequeStr = new DataGridViewTextBoxColumn();
            NumCheque = new DataGridViewTextBoxColumn();
            Posfechado = new DataGridViewTextBoxColumn();
            chequeMsgBindingSource = new BindingSource(components);
            dataGridView3 = new DataGridView();
            tipoPagoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            montoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            numTransferenciaDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            bancoTxtDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            saldoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tipoPagoMsgBindingSource = new BindingSource(components);
            tabPage1 = new TabPage();
            dataGridView1 = new DataGridView();
            dateDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            numDocDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            totalDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            toPayDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            pagadoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            toPayMasProntoPagoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            porcentajePPDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            descuentoPPDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            tipoDocumentoDataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            valorPagadoDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            docCarteraMsgBindingSource = new BindingSource(components);
            label16 = new Label();
            label15 = new Label();
            label14 = new Label();
            label13 = new Label();
            label11 = new Label();
            label12 = new Label();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            ordenMsgBindingSource = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)dgvDocs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsDocsToSyncMsg).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvMensajes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsMensajesSincronizacion).BeginInit();
            groupBox1.SuspendLayout();
            tabDetalleDocToSync.SuspendLayout();
            tpPedido.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)linesBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bsCurrentDocToSync).BeginInit();
            tpCobro.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chequeMsgBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)tipoPagoMsgBindingSource).BeginInit();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)docCarteraMsgBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ordenMsgBindingSource).BeginInit();
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
            dgvDocs.Size = new Size(1075, 137);
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
            dgvMensajes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvMensajes.AutoGenerateColumns = false;
            dgvMensajes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMensajes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMensajes.Columns.AddRange(new DataGridViewColumn[] { fechaLogDataGridViewTextBoxColumn, msgDataGridViewTextBoxColumn, tipoMsgDataGridViewTextBoxColumn });
            dgvMensajes.DataSource = bsMensajesSincronizacion;
            dgvMensajes.Location = new Point(3, 19);
            dgvMensajes.Name = "dgvMensajes";
            dgvMensajes.ReadOnly = true;
            dgvMensajes.Size = new Size(1069, 138);
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
            groupBox1.Location = new Point(5, 146);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1075, 160);
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
            tabDetalleDocToSync.Size = new Size(1075, 192);
            tabDetalleDocToSync.TabIndex = 5;
            // 
            // tpPedido
            // 
            tpPedido.Controls.Add(groupBox3);
            tpPedido.Location = new Point(4, 24);
            tpPedido.Name = "tpPedido";
            tpPedido.Padding = new Padding(3);
            tpPedido.Size = new Size(975, 164);
            tpPedido.TabIndex = 0;
            tpPedido.Text = "Detalle del Pedido";
            tpPedido.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(dataGridView2);
            groupBox3.Location = new Point(6, 6);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(991, 152);
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
            dataGridView2.Size = new Size(985, 130);
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
            linesBindingSource.DataSource = bsCurrentDocToSync;
            // 
            // bsCurrentDocToSync
            // 
            bsCurrentDocToSync.DataSource = typeof(jbp.msg.sap.OrdenMsg);
            // 
            // tpCobro
            // 
            tpCobro.Controls.Add(tabControl1);
            tpCobro.Location = new Point(4, 24);
            tpCobro.Name = "tpCobro";
            tpCobro.Padding = new Padding(3);
            tpCobro.Size = new Size(1067, 164);
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
            tabControl1.Size = new Size(1061, 158);
            tabControl1.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(groupBox2);
            tabPage2.Controls.Add(dataGridView3);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1053, 130);
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
            groupBox2.Size = new Size(483, 118);
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
            dataGridView4.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn4, monto, FechaVencimientoChequeStr, NumCheque, Posfechado });
            dataGridView4.DataSource = chequeMsgBindingSource;
            dataGridView4.Dock = DockStyle.Fill;
            dataGridView4.Location = new Point(3, 19);
            dataGridView4.Name = "dataGridView4";
            dataGridView4.ReadOnly = true;
            dataGridView4.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView4.Size = new Size(477, 96);
            dataGridView4.TabIndex = 16;
            // 
            // dataGridViewTextBoxColumn4
            // 
            dataGridViewTextBoxColumn4.DataPropertyName = "bancoTxt";
            dataGridViewTextBoxColumn4.HeaderText = "Banco";
            dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // monto
            // 
            monto.DataPropertyName = "monto";
            monto.HeaderText = "Monto";
            monto.Name = "monto";
            monto.ReadOnly = true;
            // 
            // FechaVencimientoChequeStr
            // 
            FechaVencimientoChequeStr.DataPropertyName = "FechaVencimientoChequeStr";
            FechaVencimientoChequeStr.HeaderText = "FechaVen";
            FechaVencimientoChequeStr.Name = "FechaVencimientoChequeStr";
            FechaVencimientoChequeStr.ReadOnly = true;
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
            // chequeMsgBindingSource
            // 
            chequeMsgBindingSource.DataSource = typeof(jbp.msg.sap.ChequeMsg);
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
            tipoPagoDataGridViewTextBoxColumn.HeaderText = "Tipo";
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
            bancoTxtDataGridViewTextBoxColumn.HeaderText = "Banco";
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
            // tipoPagoMsgBindingSource
            // 
            tipoPagoMsgBindingSource.DataSource = typeof(jbp.msg.sap.TipoPagoMsg);
            tipoPagoMsgBindingSource.CurrentChanged += tipoPagoMsgBindingSource_CurrentChanged;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dataGridView1);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(961, 130);
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
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { dateDataGridViewTextBoxColumn, numDocDataGridViewTextBoxColumn, totalDataGridViewTextBoxColumn1, toPayDataGridViewTextBoxColumn, pagadoDataGridViewTextBoxColumn, toPayMasProntoPagoDataGridViewTextBoxColumn, porcentajePPDataGridViewTextBoxColumn, descuentoPPDataGridViewTextBoxColumn, tipoDocumentoDataGridViewTextBoxColumn1, valorPagadoDataGridViewTextBoxColumn });
            dataGridView1.DataSource = docCarteraMsgBindingSource;
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(3, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.Size = new Size(955, 124);
            dataGridView1.TabIndex = 14;
            // 
            // dateDataGridViewTextBoxColumn
            // 
            dateDataGridViewTextBoxColumn.DataPropertyName = "date";
            dateDataGridViewTextBoxColumn.HeaderText = "Fecha";
            dateDataGridViewTextBoxColumn.Name = "dateDataGridViewTextBoxColumn";
            dateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // numDocDataGridViewTextBoxColumn
            // 
            numDocDataGridViewTextBoxColumn.DataPropertyName = "numDoc";
            numDocDataGridViewTextBoxColumn.HeaderText = "Num Doc";
            numDocDataGridViewTextBoxColumn.Name = "numDocDataGridViewTextBoxColumn";
            numDocDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // totalDataGridViewTextBoxColumn1
            // 
            totalDataGridViewTextBoxColumn1.DataPropertyName = "total";
            totalDataGridViewTextBoxColumn1.HeaderText = "total";
            totalDataGridViewTextBoxColumn1.Name = "totalDataGridViewTextBoxColumn1";
            totalDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // toPayDataGridViewTextBoxColumn
            // 
            toPayDataGridViewTextBoxColumn.DataPropertyName = "toPay";
            toPayDataGridViewTextBoxColumn.HeaderText = "toPay";
            toPayDataGridViewTextBoxColumn.Name = "toPayDataGridViewTextBoxColumn";
            toPayDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // pagadoDataGridViewTextBoxColumn
            // 
            pagadoDataGridViewTextBoxColumn.DataPropertyName = "pagado";
            pagadoDataGridViewTextBoxColumn.HeaderText = "pagado";
            pagadoDataGridViewTextBoxColumn.Name = "pagadoDataGridViewTextBoxColumn";
            pagadoDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // toPayMasProntoPagoDataGridViewTextBoxColumn
            // 
            toPayMasProntoPagoDataGridViewTextBoxColumn.DataPropertyName = "toPayMasProntoPago";
            toPayMasProntoPagoDataGridViewTextBoxColumn.HeaderText = "toPayMasProntoPago";
            toPayMasProntoPagoDataGridViewTextBoxColumn.Name = "toPayMasProntoPagoDataGridViewTextBoxColumn";
            toPayMasProntoPagoDataGridViewTextBoxColumn.ReadOnly = true;
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
            // tipoDocumentoDataGridViewTextBoxColumn1
            // 
            tipoDocumentoDataGridViewTextBoxColumn1.DataPropertyName = "tipoDocumento";
            tipoDocumentoDataGridViewTextBoxColumn1.HeaderText = "tipoDocumento";
            tipoDocumentoDataGridViewTextBoxColumn1.Name = "tipoDocumentoDataGridViewTextBoxColumn1";
            tipoDocumentoDataGridViewTextBoxColumn1.ReadOnly = true;
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
            label16.DataBindings.Add(new Binding("Text", bsCurrentDocToSync, "FechaIngresoSap", true));
            label16.Location = new Point(651, 326);
            label16.Name = "label16";
            label16.Size = new Size(171, 15);
            label16.TabIndex = 13;
            label16.Text = "<En espera de procesamiento>";
            // 
            // label15
            // 
            label15.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label15.AutoSize = true;
            label15.DataBindings.Add(new Binding("Text", bsCurrentDocToSync, "FechaSincronizacionVendedor", true));
            label15.Location = new Point(651, 308);
            label15.Name = "label15";
            label15.Size = new Size(24, 15);
            label15.TabIndex = 12;
            label15.Text = "NA";
            // 
            // label14
            // 
            label14.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label14.AutoSize = true;
            label14.DataBindings.Add(new Binding("Text", bsCurrentDocToSync, "Total", true));
            label14.Location = new Point(78, 347);
            label14.Name = "label14";
            label14.Size = new Size(24, 15);
            label14.TabIndex = 11;
            label14.Text = "NA";
            // 
            // label13
            // 
            label13.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label13.AutoSize = true;
            label13.DataBindings.Add(new Binding("Text", bsCurrentDocToSync, "Vendedor", true));
            label13.Location = new Point(78, 326);
            label13.Name = "label13";
            label13.Size = new Size(24, 15);
            label13.TabIndex = 10;
            label13.Text = "NA";
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label11.AutoSize = true;
            label11.DataBindings.Add(new Binding("Text", bsCurrentDocToSync, "Cliente", true));
            label11.Location = new Point(78, 308);
            label11.Name = "label11";
            label11.Size = new Size(24, 15);
            label11.TabIndex = 9;
            label11.Text = "NA";
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label12.AutoSize = true;
            label12.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label12.Location = new Point(529, 326);
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
            label10.Location = new Point(471, 308);
            label10.Name = "label10";
            label10.Size = new Size(182, 15);
            label10.TabIndex = 4;
            label10.Text = "Fecha Sincronizacion Vendedor:";
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label9.Location = new Point(15, 347);
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
            label8.Location = new Point(14, 308);
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
            label7.Location = new Point(14, 326);
            label7.Name = "label7";
            label7.Size = new Size(64, 15);
            label7.TabIndex = 1;
            label7.Text = "Vendedor:";
            // 
            // ordenMsgBindingSource
            // 
            ordenMsgBindingSource.DataSource = typeof(jbp.msg.sap.OrdenMsg);
            // 
            // DocsToSyncViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label16);
            Controls.Add(tabDetalleDocToSync);
            Controls.Add(label15);
            Controls.Add(groupBox1);
            Controls.Add(label14);
            Controls.Add(dgvDocs);
            Controls.Add(label13);
            Controls.Add(label10);
            Controls.Add(label11);
            Controls.Add(label7);
            Controls.Add(label12);
            Controls.Add(label8);
            Controls.Add(label9);
            Name = "DocsToSyncViewer";
            Size = new Size(1082, 570);
            ((System.ComponentModel.ISupportInitialize)dgvDocs).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsDocsToSyncMsg).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvMensajes).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsMensajesSincronizacion).EndInit();
            groupBox1.ResumeLayout(false);
            tabDetalleDocToSync.ResumeLayout(false);
            tpPedido.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)linesBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)bsCurrentDocToSync).EndInit();
            tpCobro.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView4).EndInit();
            ((System.ComponentModel.ISupportInitialize)chequeMsgBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView3).EndInit();
            ((System.ComponentModel.ISupportInitialize)tipoPagoMsgBindingSource).EndInit();
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
        private TabControl tabDetalleDocToSync;
        private TabPage tpPedido;
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
        private TabPage tpCobro;
        private BindingSource bsCurrentDocToSync;
        private DataGridViewTextBoxColumn codArticuloDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn articuloDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantSolicitadaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantBonificacionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantBrutaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn price;
        private DataGridViewTextBoxColumn SubTotal;
        private BindingSource linesBindingSource;
        private DataGridView dataGridView1;
        private BindingSource ordenMsgBindingSource;
        private BindingSource docCarteraMsgBindingSource;
        private DataGridViewTextBoxColumn folioNumDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn datosAdicionalesDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn codClienteDataGridViewTextBoxColumn;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private DataGridView dataGridView3;
        private BindingSource tipoPagoMsgBindingSource;
        private DataGridViewTextBoxColumn dateDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn numDocDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn totalDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn toPayDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn pagadoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn toPayMasProntoPagoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn porcentajePPDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn descuentoPPDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn tipoDocumentoDataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn valorPagadoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn codigoCuentaJBDataGridViewTextBoxColumn;
        private DataGridView dataGridView4;
        private BindingSource chequeMsgBindingSource;
        private DataGridViewTextBoxColumn tipoPagoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn montoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn numTransferenciaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn bancoTxtDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn saldoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private DataGridViewTextBoxColumn monto;
        private DataGridViewTextBoxColumn FechaVencimientoChequeStr;
        private DataGridViewTextBoxColumn NumCheque;
        private DataGridViewTextBoxColumn Posfechado;
        private GroupBox groupBox2;
    }
}
