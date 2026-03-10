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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSyncAppVET));
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            dgDocsToSync = new DataGridView();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            dgLog = new DataGridView();
            tabPage4 = new TabPage();
            tabControl2 = new TabControl();
            tabPage5 = new TabPage();
            label12 = new Label();
            groupBox3 = new GroupBox();
            dataGridView2 = new DataGridView();
            label10 = new Label();
            label9 = new Label();
            label8 = new Label();
            label7 = new Label();
            tabPage6 = new TabPage();
            groupBox2 = new GroupBox();
            dataGridView1 = new DataGridView();
            groupBox1 = new GroupBox();
            dateTimePicker2 = new DateTimePicker();
            bsFiltroHistorico = new BindingSource(components);
            dateTimePicker1 = new DateTimePicker();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            comboBox1 = new ComboBox();
            label6 = new Label();
            cmdConsultarHistorico = new Button();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
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
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgDocsToSync).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgLog).BeginInit();
            tabPage4.SuspendLayout();
            tabControl2.SuspendLayout();
            tabPage5.SuspendLayout();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView2).BeginInit();
            groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bsFiltroHistorico).BeginInit();
            panel1.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Location = new Point(40, 71);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1012, 550);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(dgDocsToSync);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1004, 522);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Docs en Sincronización";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgDocsToSync
            // 
            dgDocsToSync.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgDocsToSync.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgDocsToSync.Location = new Point(6, 6);
            dgDocsToSync.Name = "dgDocsToSync";
            dgDocsToSync.Size = new Size(964, 502);
            dgDocsToSync.TabIndex = 1;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1004, 522);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Documentos con Error";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dgLog);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1004, 522);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Log";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgLog
            // 
            dgLog.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgLog.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgLog.Location = new Point(6, 6);
            dgLog.Name = "dgLog";
            dgLog.Size = new Size(992, 510);
            dgLog.TabIndex = 0;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(tabControl2);
            tabPage4.Controls.Add(groupBox2);
            tabPage4.Controls.Add(groupBox1);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(1004, 522);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Consulta Histórica";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            tabControl2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl2.Controls.Add(tabPage5);
            tabControl2.Controls.Add(tabPage6);
            tabControl2.Location = new Point(13, 190);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(968, 317);
            tabControl2.TabIndex = 4;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(label12);
            tabPage5.Controls.Add(groupBox3);
            tabPage5.Controls.Add(label10);
            tabPage5.Controls.Add(label9);
            tabPage5.Controls.Add(label8);
            tabPage5.Controls.Add(label7);
            tabPage5.Location = new Point(4, 24);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3);
            tabPage5.Size = new Size(960, 289);
            tabPage5.TabIndex = 0;
            tabPage5.Text = "Detalle del Pedido";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(483, 30);
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
            groupBox3.Size = new Size(930, 206);
            groupBox3.TabIndex = 6;
            groupBox3.TabStop = false;
            groupBox3.Text = "Líneas del Pedido";
            // 
            // dataGridView2
            // 
            dataGridView2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView2.Location = new Point(9, 22);
            dataGridView2.Name = "dataGridView2";
            dataGridView2.Size = new Size(910, 170);
            dataGridView2.TabIndex = 5;
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
            tabPage6.Size = new Size(960, 289);
            tabPage6.TabIndex = 1;
            tabPage6.Text = "Detalle Cobro";
            tabPage6.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(dataGridView1);
            groupBox2.Location = new Point(478, 22);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(513, 162);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Resultado";
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(6, 19);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(501, 137);
            dataGridView1.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(dateTimePicker2);
            groupBox1.Controls.Add(dateTimePicker1);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(cmdConsultarHistorico);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(13, 22);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(453, 162);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Filtrar Búsqueda Por:";
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dateTimePicker2.DataBindings.Add(new Binding("Value", bsFiltroHistorico, "Hasta", true));
            dateTimePicker2.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Location = new Point(323, 87);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(112, 23);
            dateTimePicker2.TabIndex = 10;
            // 
            // bsFiltroHistorico
            // 
            bsFiltroHistorico.DataSource = typeof(jbp.msg.sap.FiltroHistoricoMsg);
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            dateTimePicker1.DataBindings.Add(new Binding("Value", bsFiltroHistorico, "Desde", true));
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Location = new Point(323, 63);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(112, 23);
            dateTimePicker1.TabIndex = 9;
            // 
            // textBox2
            // 
            textBox2.DataBindings.Add(new Binding("Text", bsFiltroHistorico, "Cliente", true));
            textBox2.Location = new Point(81, 85);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(183, 23);
            textBox2.TabIndex = 8;
            // 
            // textBox1
            // 
            textBox1.DataBindings.Add(new Binding("Text", bsFiltroHistorico, "Vendedor", true));
            textBox1.Location = new Point(81, 57);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(183, 23);
            textBox1.TabIndex = 7;
            // 
            // comboBox1
            // 
            comboBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            comboBox1.DataBindings.Add(new Binding("Text", bsFiltroHistorico, "TipoDocumento", true));
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Pedido", "Cobro" });
            comboBox1.Location = new Point(112, 16);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(323, 23);
            comboBox1.TabIndex = 6;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(6, 19);
            label6.Name = "label6";
            label6.Size = new Size(100, 15);
            label6.TabIndex = 5;
            label6.Text = "Tipo Documento:";
            // 
            // cmdConsultarHistorico
            // 
            cmdConsultarHistorico.Anchor = AnchorStyles.Bottom;
            cmdConsultarHistorico.Location = new Point(190, 123);
            cmdConsultarHistorico.Name = "cmdConsultarHistorico";
            cmdConsultarHistorico.Size = new Size(89, 23);
            cmdConsultarHistorico.TabIndex = 4;
            cmdConsultarHistorico.Text = "Consultar";
            cmdConsultarHistorico.UseVisualStyleBackColor = true;
            cmdConsultarHistorico.Click += cmdConsultarHistorico_Click;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label5.AutoSize = true;
            label5.Location = new Point(275, 88);
            label5.Name = "label5";
            label5.Size = new Size(40, 15);
            label5.TabIndex = 3;
            label5.Text = "Hasta:";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(275, 69);
            label4.Name = "label4";
            label4.Size = new Size(42, 15);
            label4.TabIndex = 2;
            label4.Text = "Desde:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 93);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 1;
            label3.Text = "Cliente:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 64);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 0;
            label2.Text = "Vendedor:";
            // 
            // cmdIniciar
            // 
            cmdIniciar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdIniciar.Location = new Point(893, 627);
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
            cmdDetener.Location = new Point(980, 627);
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
            label1.Location = new Point(25, 32);
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
            panel1.Location = new Point(946, 12);
            panel1.Name = "panel1";
            panel1.Size = new Size(109, 70);
            panel1.TabIndex = 5;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(4, 41);
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
            lblOk.Location = new Point(4, 26);
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
            lblIntentos.Location = new Point(410, 33);
            lblIntentos.Name = "lblIntentos";
            lblIntentos.Size = new Size(37, 15);
            lblIntentos.TabIndex = 6;
            lblIntentos.Text = "Num:";
            // 
            // frmSyncAppVET
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1067, 671);
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
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgDocsToSync).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgLog).EndInit();
            tabPage4.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView2).EndInit();
            groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bsFiltroHistorico).EndInit();
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
        private DataGridView dgLog;
        private Label lblIntentos;
        private DataGridView dgDocsToSync;
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
        private DataGridView dataGridView1;
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
    }
}
