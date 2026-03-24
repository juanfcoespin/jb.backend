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
            lblError = new Label();
            ctrDocsToSync = new DocsToSyncViewer();
            lblOk = new Label();
            tabPage3 = new TabPage();
            ctrlDocsLogs = new DocsToSyncViewer();
            tabPage2 = new TabPage();
            ctrlDocsError = new DocsToSyncViewer();
            tabPage4 = new TabPage();
            ctrResultado = new DocsToSyncViewer();
            groupBox1 = new GroupBox();
            groupBox4 = new GroupBox();
            dateTimePicker2 = new DateTimePicker();
            bsFiltroHistorico = new BindingSource(components);
            dateTimePicker1 = new DateTimePicker();
            label4 = new Label();
            label5 = new Label();
            textBox2 = new TextBox();
            comboBox1 = new ComboBox();
            label6 = new Label();
            textBox1 = new TextBox();
            cmdConsultarHistorico = new Button();
            label3 = new Label();
            label2 = new Label();
            linesBindingSource = new BindingSource(components);
            cmdIniciar = new Button();
            cmdDetener = new Button();
            label1 = new Label();
            notifyIcon1 = new NotifyIcon(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            mostrarToolStripMenuItem = new ToolStripMenuItem();
            cerrarToolStripMenuItem = new ToolStripMenuItem();
            timer1 = new System.Windows.Forms.Timer(components);
            lblIntentos = new Label();
            backgroundWorkerSyncDocs = new System.ComponentModel.BackgroundWorker();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage3.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage4.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)bsFiltroHistorico).BeginInit();
            ((System.ComponentModel.ISupportInitialize)linesBindingSource).BeginInit();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Location = new Point(12, 51);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1131, 641);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(lblError);
            tabPage1.Controls.Add(ctrDocsToSync);
            tabPage1.Controls.Add(lblOk);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1123, 613);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Docs en Sincronización";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblError
            // 
            lblError.AutoSize = true;
            lblError.ForeColor = Color.Red;
            lblError.Location = new Point(90, 4);
            lblError.Name = "lblError";
            lblError.Size = new Size(38, 15);
            lblError.TabIndex = 6;
            lblError.Text = "Error: ";
            // 
            // ctrDocsToSync
            // 
            ctrDocsToSync.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ctrDocsToSync.Location = new Point(6, 22);
            ctrDocsToSync.Name = "ctrDocsToSync";
            ctrDocsToSync.Size = new Size(1111, 585);
            ctrDocsToSync.TabIndex = 0;
            // 
            // lblOk
            // 
            lblOk.AutoSize = true;
            lblOk.BackColor = Color.Transparent;
            lblOk.ForeColor = Color.Green;
            lblOk.Location = new Point(15, 4);
            lblOk.Name = "lblOk";
            lblOk.Size = new Size(29, 15);
            lblOk.TabIndex = 5;
            lblOk.Text = "OK: ";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(ctrlDocsLogs);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(976, 613);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Log Documentos Sincronizados";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // ctrlDocsLogs
            // 
            ctrlDocsLogs.Dock = DockStyle.Fill;
            ctrlDocsLogs.Location = new Point(3, 3);
            ctrlDocsLogs.Name = "ctrlDocsLogs";
            ctrlDocsLogs.Size = new Size(970, 607);
            ctrlDocsLogs.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(ctrlDocsError);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(976, 613);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Documentos con Error";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // ctrlDocsError
            // 
            ctrlDocsError.Dock = DockStyle.Fill;
            ctrlDocsError.Location = new Point(3, 3);
            ctrlDocsError.Name = "ctrlDocsError";
            ctrlDocsError.Size = new Size(970, 607);
            ctrlDocsError.TabIndex = 1;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(ctrResultado);
            tabPage4.Controls.Add(groupBox1);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Size = new Size(976, 613);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "Consulta Histórica Documentos Sincronizados";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // ctrResultado
            // 
            ctrResultado.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ctrResultado.Location = new Point(13, 62);
            ctrResultado.Name = "ctrResultado";
            ctrResultado.Size = new Size(942, 548);
            ctrResultado.TabIndex = 2;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(groupBox4);
            groupBox1.Controls.Add(textBox2);
            groupBox1.Controls.Add(comboBox1);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(cmdConsultarHistorico);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Location = new Point(13, 0);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(940, 55);
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
            groupBox4.Location = new Point(537, 10);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(300, 41);
            groupBox4.TabIndex = 7;
            groupBox4.TabStop = false;
            groupBox4.Text = "Fecha Sincronización del Vendedor";
            // 
            // dateTimePicker2
            // 
            dateTimePicker2.DataBindings.Add(new Binding("Value", bsFiltroHistorico, "Hasta", true));
            dateTimePicker2.Format = DateTimePickerFormat.Short;
            dateTimePicker2.Location = new Point(198, 16);
            dateTimePicker2.Name = "dateTimePicker2";
            dateTimePicker2.Size = new Size(97, 23);
            dateTimePicker2.TabIndex = 10;
            // 
            // bsFiltroHistorico
            // 
            bsFiltroHistorico.DataSource = typeof(jbp.msg.sap.FiltroHistoricoMsg);
            // 
            // dateTimePicker1
            // 
            dateTimePicker1.DataBindings.Add(new Binding("Value", bsFiltroHistorico, "Desde", true));
            dateTimePicker1.Format = DateTimePickerFormat.Short;
            dateTimePicker1.Location = new Point(56, 17);
            dateTimePicker1.Name = "dateTimePicker1";
            dateTimePicker1.Size = new Size(84, 23);
            dateTimePicker1.TabIndex = 9;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 20);
            label4.Name = "label4";
            label4.Size = new Size(42, 15);
            label4.TabIndex = 2;
            label4.Text = "Desde:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(152, 20);
            label5.Name = "label5";
            label5.Size = new Size(40, 15);
            label5.TabIndex = 3;
            label5.Text = "Hasta:";
            // 
            // textBox2
            // 
            textBox2.DataBindings.Add(new Binding("Text", bsFiltroHistorico, "Cliente", true));
            textBox2.Location = new Point(386, 24);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(140, 23);
            textBox2.TabIndex = 8;
            // 
            // comboBox1
            // 
            comboBox1.DataBindings.Add(new Binding("Text", bsFiltroHistorico, "TipoDocumento", true));
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Pedido", "Cobro" });
            comboBox1.Location = new Point(71, 24);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(77, 23);
            comboBox1.TabIndex = 6;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label6.AutoSize = true;
            label6.Location = new Point(7, 27);
            label6.Name = "label6";
            label6.Size = new Size(58, 15);
            label6.TabIndex = 5;
            label6.Text = "Tipo Doc:";
            // 
            // textBox1
            // 
            textBox1.DataBindings.Add(new Binding("Text", bsFiltroHistorico, "Vendedor", true));
            textBox1.Location = new Point(216, 22);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(110, 23);
            textBox1.TabIndex = 7;
            // 
            // cmdConsultarHistorico
            // 
            cmdConsultarHistorico.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cmdConsultarHistorico.Location = new Point(850, 19);
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
            label3.Location = new Point(341, 30);
            label3.Name = "label3";
            label3.Size = new Size(47, 15);
            label3.TabIndex = 1;
            label3.Text = "Cliente:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(154, 27);
            label2.Name = "label2";
            label2.Size = new Size(60, 15);
            label2.TabIndex = 0;
            label2.Text = "Vendedor:";
            // 
            // linesBindingSource
            // 
            linesBindingSource.DataMember = "Lines";
            // 
            // cmdIniciar
            // 
            cmdIniciar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdIniciar.Location = new Point(984, 698);
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
            cmdDetener.Location = new Point(1071, 698);
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
            lblIntentos.Size = new Size(97, 15);
            lblIntentos.TabIndex = 6;
            lblIntentos.Text = "Num Busquedas:";
            // 
            // backgroundWorkerSyncDocs
            // 
            backgroundWorkerSyncDocs.DoWork += backgroundWorkerSyncDocs_DoWork;
            backgroundWorkerSyncDocs.RunWorkerCompleted += backgroundWorkerSyncDocs_RunWorkerCompleted;
            // 
            // frmSyncAppVET
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1158, 729);
            ControlBox = false;
            Controls.Add(lblIntentos);
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
            tabPage1.PerformLayout();
            tabPage3.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)bsFiltroHistorico).EndInit();
            ((System.ComponentModel.ISupportInitialize)linesBindingSource).EndInit();
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
        private DateTimePicker dateTimePicker2;
        private GroupBox groupBox4;
        private BindingSource linesBindingSource;
        private System.ComponentModel.BackgroundWorker backgroundWorkerSyncDocs;
        private DataGridViewTextBoxColumn fechaLogDataGridViewTextBoxColumn;
        private DocsToSyncViewer ctrDocsToSync;
        private DocsToSyncViewer ctrlDocsLogs;
        private DocsToSyncViewer ctrlDocsError;
        private DocsToSyncViewer ctrResultado;
    }
}
