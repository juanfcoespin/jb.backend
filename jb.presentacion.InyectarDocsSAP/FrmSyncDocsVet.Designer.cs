namespace jb.presentacion.InyectarDocsSAP
{
    partial class frmSyncAppVET
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSyncAppVET));
            tabControl1 = new TabControl();
            tabPage4 = new TabPage();
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
            price = new DataGridViewTextBoxColumn();
            SubTotal = new DataGridViewTextBoxColumn();
            linesBindingSource = new BindingSource(components);
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            tabPage6 = new TabPage();
            groupBox2 = new GroupBox();
            dgResultadoBusqueda = new DataGridView();
            clienteDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            vendedorDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            totalDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            fechaSincronizacionVendedorDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            fechaIngresoSapDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            ordenMsgBindingSource = new BindingSource(components);
            groupBox1 = new GroupBox();
            groupBox4 = new GroupBox();
            dateTimePicker2 = new DateTimePicker();
            bsFiltroHistorico = new BindingSource(components);
            dateTimePicker1 = new DateTimePicker();
            label4 = new Label();
            label5 = new Label();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            comboBox1 = new ComboBox();
            label6 = new Label();
            cmdConsultarHistorico = new Button();
            label3 = new Label();
            label2 = new Label();
            tabPage1 = new TabPage();
            ctrDocsToSync = new DocsToSyncViewer();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            cmdIniciar = new Button();
            cmdDetener = new Button();
            label1 = new Label();
            lblProcesando = new Label();
            panel1 = new Panel();
            lblError = new Label();
            lblOk = new Label();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            mostrarToolStripMenuItem = new ToolStripMenuItem();
            cerrarToolStripMenuItem = new ToolStripMenuItem();
            timer1 = new System.Windows.Forms.Timer(components);
            lblIntentos = new Label();
            backgroundWorkerSyncDocs = new System.ComponentModel.BackgroundWorker();
            ctrlDocsLogs = new DocsToSyncViewer();
            tabControl1.SuspendLayout();
            tabPage4.SuspendLayout();
            tabControl2.SuspendLayout();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bsCurrentPedido).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)linesBindingSource).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgResultadoBusqueda).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ordenMsgBindingSource).BeginInit();
            groupBox1.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bsFiltroHistorico).BeginInit();
            tabPage1.SuspendLayout();
            tabPage3.SuspendLayout();
            panel1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(12, 51);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1040, 638);
            tabControl1.TabIndex = 0;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(tabControl2);
            tabPage4.Controls.Add(groupBox2);
            tabPage4.Controls.Add(groupBox1);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(1032, 610);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Consulta Histórica";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            tabControl2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl2.Controls.Add(tabPage5);
            tabControl2.Controls.Add(tabPage6);
            tabControl2.Location = new Point(13, 226);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(1006, 369);
            tabControl2.TabIndex = 4;
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
            tabPage5.Size = new Size(998, 341);
            tabPage5.TabIndex = 0;
            tabPage5.Text = "Detalle del Pedido";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.DataBindings.Add(new Binding("Text", bsCurrentPedido, "FechaIngresoSap", true));
            label16.Location = new Point(663, 30);
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
            label15.Location = new Point(663, 12);
            label15.Name = "label15";
            label15.Size = new Size(24, 15);
            label15.TabIndex = 12;
            label15.Text = "NA";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.DataBindings.Add(new Binding("Text", bsCurrentPedido, "Total", true));
            label14.Location = new Point(90, 51);
            label14.Name = "label14";
            label14.Size = new Size(24, 15);
            label14.TabIndex = 11;
            label14.Text = "NA";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.DataBindings.Add(new Binding("Text", bsCurrentPedido, "Vendedor", true));
            label13.Location = new Point(90, 30);
            label13.Name = "label13";
            label13.Size = new Size(24, 15);
            label13.TabIndex = 10;
            label13.Text = "NA";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.DataBindings.Add(new Binding("Text", bsCurrentPedido, "Cliente", true));
            label11.Location = new Point(90, 12);
            label11.Name = "label11";
            label11.Size = new Size(24, 15);
            label11.TabIndex = 9;
            label11.Text = "NA";
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(541, 30);
            label12.Name = "label12";
            label12.Size = new Size(116, 15);
            label12.TabIndex = 8;
            label12.Text = "Fecha Ingreso a SAP:";
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.Controls.Add(dataGridView2);
            groupBox3.Location = new Point(8, 77);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(968, 258);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "Líneas del Pedido";
            // 
            // dataGridView2
            // 
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Columns.AddRange(new DataGridViewColumn[] { codArticuloDataGridViewTextBoxColumn, articuloDataGridViewTextBoxColumn, cantSolicitadaDataGridViewTextBoxColumn, cantBonificacionDataGridViewTextBoxColumn, price, SubTotal });
            dataGridView2.DataSource = linesBindingSource;
            dataGridView2.Location = new Point(9, 22);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.Size = new Size(948, 222);
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
            label10.Location = new Point(483, 12);
            label10.Name = "label10";
            label10.Size = new Size(174, 15);
            label10.TabIndex = 4;
            label10.Text = "Fecha Sincronizacion Vendedor:";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(27, 51);
            label9.Name = "label9";
            label9.Size = new Size(36, 15);
            label9.TabIndex = 3;
            label9.Text = "Total:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(26, 12);
            label8.Name = "label8";
            label8.Size = new Size(47, 15);
            label8.TabIndex = 2;
            label8.Text = "Cliente:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(26, 30);
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
            tabPage6.Size = new Size(998, 341);
            tabPage6.TabIndex = 1;
            tabPage6.Text = "Detalle Cobro";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(dgResultadoBusqueda);
            groupBox2.Location = new Point(337, 7);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(682, 213);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Resultado";
            // 
            // dgResultadoBusqueda
            // 
            dgResultadoBusqueda.AllowUserToAddRows = false;
            dgResultadoBusqueda.AllowUserToDeleteRows = false;
            dgResultadoBusqueda.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgResultadoBusqueda.AutoGenerateColumns = false;
            dgResultadoBusqueda.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgResultadoBusqueda.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgResultadoBusqueda.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgResultadoBusqueda.Columns.AddRange(new DataGridViewColumn[] { clienteDataGridViewTextBoxColumn, vendedorDataGridViewTextBoxColumn, totalDataGridViewTextBoxColumn, fechaSincronizacionVendedorDataGridViewTextBoxColumn, fechaIngresoSapDataGridViewTextBoxColumn });
            dgResultadoBusqueda.DataSource = ordenMsgBindingSource;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgResultadoBusqueda.DefaultCellStyle = dataGridViewCellStyle1;
            dgResultadoBusqueda.Location = new Point(6, 19);
            dgResultadoBusqueda.Name = "dgResultadoBusqueda";
            dgResultadoBusqueda.ReadOnly = true;
            dgResultadoBusqueda.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgResultadoBusqueda.Size = new Size(670, 188);
            dgResultadoBusqueda.TabIndex = 0;
            dgResultadoBusqueda.SelectionChanged += dgResultadoBusqueda_SelectionChanged;
            // 
            // clienteDataGridViewTextBoxColumn
            // 
            clienteDataGridViewTextBoxColumn.DataPropertyName = "Cliente";
            clienteDataGridViewTextBoxColumn.HeaderText = "Cliente";
            clienteDataGridViewTextBoxColumn.Name = "clienteDataGridViewTextBoxColumn";
            clienteDataGridViewTextBoxColumn.ReadOnly = true;
            clienteDataGridViewTextBoxColumn.Width = 69;
            // 
            // vendedorDataGridViewTextBoxColumn
            // 
            vendedorDataGridViewTextBoxColumn.DataPropertyName = "Vendedor";
            vendedorDataGridViewTextBoxColumn.HeaderText = "Vendedor";
            vendedorDataGridViewTextBoxColumn.Name = "vendedorDataGridViewTextBoxColumn";
            vendedorDataGridViewTextBoxColumn.ReadOnly = true;
            vendedorDataGridViewTextBoxColumn.Width = 82;
            // 
            // totalDataGridViewTextBoxColumn
            // 
            totalDataGridViewTextBoxColumn.DataPropertyName = "Total";
            totalDataGridViewTextBoxColumn.HeaderText = "Total";
            totalDataGridViewTextBoxColumn.Name = "totalDataGridViewTextBoxColumn";
            totalDataGridViewTextBoxColumn.ReadOnly = true;
            totalDataGridViewTextBoxColumn.Width = 58;
            // 
            // fechaSincronizacionVendedorDataGridViewTextBoxColumn
            // 
            fechaSincronizacionVendedorDataGridViewTextBoxColumn.DataPropertyName = "FechaSincronizacionVendedor";
            fechaSincronizacionVendedorDataGridViewTextBoxColumn.HeaderText = "FecSyncVendedor";
            fechaSincronizacionVendedorDataGridViewTextBoxColumn.Name = "fechaSincronizacionVendedorDataGridViewTextBoxColumn";
            fechaSincronizacionVendedorDataGridViewTextBoxColumn.ReadOnly = true;
            fechaSincronizacionVendedorDataGridViewTextBoxColumn.Width = 125;
            // 
            // fechaIngresoSapDataGridViewTextBoxColumn
            // 
            fechaIngresoSapDataGridViewTextBoxColumn.DataPropertyName = "FechaIngresoSap";
            fechaIngresoSapDataGridViewTextBoxColumn.HeaderText = "FechaIngresoSap";
            fechaIngresoSapDataGridViewTextBoxColumn.Name = "fechaIngresoSapDataGridViewTextBoxColumn";
            fechaIngresoSapDataGridViewTextBoxColumn.ReadOnly = true;
            fechaIngresoSapDataGridViewTextBoxColumn.Width = 121;
            // 
            // ordenMsgBindingSource
            // 
            ordenMsgBindingSource.DataSource = typeof(jbp.msg.sap.OrdenMsg);
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(groupBox4);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(cmdConsultarHistorico);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(13, 7);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(318, 213);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Filtrar Búsqueda Por:";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(dateTimePicker2);
            groupBox4.Controls.Add(dateTimePicker1);
            groupBox4.Controls.Add(label4);
            groupBox4.Controls.Add(label5);
            groupBox4.Location = new Point(6, 102);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(306, 75);
            groupBox4.TabIndex = 7;
            groupBox4.TabStop = false;
            groupBox4.Text = "Fecha Sync Vendedor";
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dateTimePicker2.DataBindings.Add(new Binding("Value", bsFiltroHistorico, "Hasta", true));
            dateTimePicker2.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Location = new Point(56, 44);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(242, 23);
            dateTimePicker2.TabIndex = 10;
            // 
            // bsFiltroHistorico
            // 
            bsFiltroHistorico.DataSource = typeof(jbp.msg.sap.FiltroHistoricoMsg);
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dateTimePicker1.DataBindings.Add(new Binding("Value", bsFiltroHistorico, "Desde", true));
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Location = new Point(56, 20);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(242, 23);
            dateTimePicker1.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 26);
            label4.Name = "label4";
            label4.Size = new Size(42, 15);
            label4.TabIndex = 2;
            label4.Text = "Desde:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(8, 45);
            label5.Name = "label5";
            label5.Size = new Size(40, 15);
            label5.TabIndex = 3;
            label5.Text = "Hasta:";
            // 
            // textBox2
            // 
            textBox2.DataBindings.Add(new Binding("Text", bsFiltroHistorico, "Cliente", true));
            textBox2.Location = new Point(70, 72);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(242, 23);
            textBox2.TabIndex = 8;
            // 
            // textBox1
            // 
            textBox1.DataBindings.Add(new Binding("Text", bsFiltroHistorico, "Vendedor", true));
            textBox1.Location = new Point(70, 45);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(242, 23);
            textBox1.TabIndex = 7;
            // 
            // comboBox1
            // 
            comboBox1.DataBindings.Add(new Binding("Text", bsFiltroHistorico, "TipoDocumento", true));
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Pedido", "Cobro" });
            comboBox1.Location = new Point(70, 16);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(242, 23);
            comboBox1.TabIndex = 6;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 19);
            label6.Name = "label6";
            label6.Size = new Size(58, 15);
            label6.TabIndex = 5;
            label6.Text = "Tipo Doc:";
            // 
            // cmdConsultarHistorico
            // 
            cmdConsultarHistorico.Anchor = AnchorStyles.Bottom;
            cmdConsultarHistorico.Location = new Point(119, 182);
            cmdConsultarHistorico.Name = "cmdConsultarHistorico";
            cmdConsultarHistorico.Size = new Size(78, 23);
            cmdConsultarHistorico.TabIndex = 4;
            cmdConsultarHistorico.Text = "Consultar";
            cmdConsultarHistorico.UseVisualStyleBackColor = true;
            cmdConsultarHistorico.Click += cmdConsultarHistorico_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 75);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 1;
            label3.Text = "Cliente:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(7, 45);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 0;
            label2.Text = "Vendedor:";
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(ctrDocsToSync);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1032, 610);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Docs en Sincronización";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // ctrDocsToSync
            // 
            ctrDocsToSync.Dock = DockStyle.Fill;
            ctrDocsToSync.Location = new Point(3, 3);
            ctrDocsToSync.Name = "ctrDocsToSync";
            ctrDocsToSync.Size = new Size(1026, 604);
            ctrDocsToSync.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1032, 610);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Documentos con Error";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(ctrlDocsLogs);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1032, 610);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Log";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // cmdIniciar
            // 
            cmdIniciar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdIniciar.Location = new Point(893, 695);
            cmdIniciar.Name = "cmdIniciar";
            cmdIniciar.Size = new Size(75, 23);
            cmdIniciar.TabIndex = 1;
            cmdIniciar.Text = "Iniciar";
            cmdIniciar.UseVisualStyleBackColor = true;
            cmdIniciar.Click += cmdIniciar_Click;
            // 
            // cmdDetener
            // 
            cmdDetener.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdDetener.Enabled = false;
            cmdDetener.Location = new Point(980, 695);
            cmdDetener.Name = "cmdDetener";
            cmdDetener.Size = new Size(75, 23);
            cmdDetener.TabIndex = 2;
            cmdDetener.Text = "Detener";
            cmdDetener.UseVisualStyleBackColor = true;
            cmdDetener.Click += cmdDetener_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.Location = new Point(16, 12);
            label1.Name = "label1";
            label1.Size = new Size(311, 21);
            label1.TabIndex = 3;
            label1.Text = "Sincronización de Documentos AppVET";
            // 
            // lblProcesando
            // 
            lblProcesando.AutoSize = true;
            lblProcesando.Location = new Point(4, 11);
            lblProcesando.Name = "lblProcesando";
            lblProcesando.Size = new Size(75, 15);
            lblProcesando.TabIndex = 4;
            lblProcesando.Text = "Procesando: ";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            panel1.Controls.Add(lblError);
            panel1.Controls.Add(lblOk);
            panel1.Controls.Add(lblProcesando);
            panel1.Location = new Point(818, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(230, 36);
            panel1.TabIndex = 5;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(132, 11);
            lblError.Name = "lblError";
            lblError.Size = new Size(38, 15);
            lblError.TabIndex = 6;
            lblError.Text = "Error: ";
            // 
            // lblOk
            // 
            lblOk.AutoSize = true;
            lblOk.BackColor = SystemColors.Control;
            lblOk.ForeColor = Color.Green;
            lblOk.Location = new Point(73, 11);
            lblOk.Name = "lblOk";
            lblOk.Size = new Size(29, 15);
            lblOk.TabIndex = 5;
            lblOk.Text = "OK: ";
            // 
            // notifyIcon1
            // 
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            notifyIcon1.Icon = (Icon)resources.GetObject("notifyIcon1.Icon");
            notifyIcon1.Text = "Sincronización AppVET";
            notifyIcon1.Visible = true;
            notifyIcon1.DoubleClick += notifyIcon1_DoubleClick;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { mostrarToolStripMenuItem, cerrarToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(116, 48);
            // 
            // mostrarToolStripMenuItem
            // 
            mostrarToolStripMenuItem.Name = "mostrarToolStripMenuItem";
            mostrarToolStripMenuItem.Size = new Size(115, 22);
            mostrarToolStripMenuItem.Text = "Mostrar";
            mostrarToolStripMenuItem.Click += mostrarToolStripMenuItem_Click;
            // 
            // cerrarToolStripMenuItem
            // 
            cerrarToolStripMenuItem.Name = "cerrarToolStripMenuItem";
            cerrarToolStripMenuItem.Size = new Size(115, 22);
            cerrarToolStripMenuItem.Text = "Cerrar";
            cerrarToolStripMenuItem.Click += cerrarToolStripMenuItem_Click;
            // 
            // timer1
            // 
            timer1.Interval = 1000;
            timer1.Tick += timer1_Tick;
            // 
            // lblIntentos
            // 
            lblIntentos.AutoSize = true;
            lblIntentos.Location = new Point(358, 12);
            lblIntentos.Name = "lblIntentos";
            lblIntentos.Size = new Size(37, 15);
            lblIntentos.TabIndex = 6;
            lblIntentos.Text = "Num:";
            // 
            // backgroundWorkerSyncDocs
            // 
            backgroundWorkerSyncDocs.DoWork += backgroundWorkerSyncDocs_DoWork;
            backgroundWorkerSyncDocs.RunWorkerCompleted += backgroundWorkerSyncDocs_RunWorkerCompleted;
            // 
            // ctrlDocsLogs
            // 
            ctrlDocsLogs.Dock = DockStyle.Fill;
            ctrlDocsLogs.Location = new Point(3, 3);
            ctrlDocsLogs.Name = "ctrlDocsLogs";
            ctrlDocsLogs.Size = new Size(1026, 604);
            ctrlDocsLogs.TabIndex = 0;
            // 
            // frmSyncAppVET
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1067, 739);
            ControlBox = false;
            Controls.Add(lblIntentos);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(cmdDetener);
            Controls.Add(cmdIniciar);
            Controls.Add(tabControl1);
            Name = "frmSyncAppVET";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Sincronización AppVET";
            Load += frmSyncAppVET_Load;
            tabControl1.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bsCurrentPedido).EndInit();
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            ((System.ComponentModel.ISupportInitialize)linesBindingSource).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgResultadoBusqueda).EndInit();
            ((System.ComponentModel.ISupportInitialize)ordenMsgBindingSource).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bsFiltroHistorico).EndInit();
            tabPage1.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private Button cmdIniciar;
        private Button cmdDetener;
        private Label label1;
        private Label lblProcesando;
        private Panel panel1;
        private Label lblOk;
        private Label lblError;
        private NotifyIcon notifyIcon1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem mostrarToolStripMenuItem;
        private ToolStripMenuItem cerrarToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private Label lblIntentos;
        private TabPage tabPage4;
        private GroupBox groupBox1;
        private Label label4;
        private Label label3;
        private Label label2;
        private Button cmdConsultarHistorico;
        private Label label5;
        private ComboBox comboBox1;
        private Label label6;
        private BindingSource bsFiltroHistorico;
        private DateTimePicker dateTimePicker1;
        private TextBox textBox2;
        private TextBox textBox1;
        private GroupBox groupBox2;
        private DataGridView dgResultadoBusqueda;
        private TabControl tabControl2;
        private TabPage tabPage5;
        private TabPage tabPage6;
        private Label label8;
        private Label label7;
        private Label label10;
        private Label label9;
        private Label label12;
        private GroupBox groupBox3;
        private DataGridView dataGridView2;
        private DateTimePicker dateTimePicker2;
        private BindingSource ordenMsgBindingSource;
        private GroupBox groupBox4;
        private Label label16;
        private Label label15;
        private Label label14;
        private Label label13;
        private Label label11;
        private BindingSource bsCurrentPedido;
        private DataGridViewTextBoxColumn codArticuloDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn articuloDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantSolicitadaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn cantBonificacionDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn price;
        private DataGridViewTextBoxColumn SubTotal;
        private BindingSource linesBindingSource;
        private DataGridViewTextBoxColumn clienteDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn vendedorDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn totalDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fechaSincronizacionVendedorDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn fechaIngresoSapDataGridViewTextBoxColumn;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSyncDocs;
        private DataGridViewTextBoxColumn fechaLogDataGridViewTextBoxColumn;
        private DocsToSyncViewer ctrDocsToSync;
        private DocsToSyncViewer ctrlDocsLogs;
    }
}
