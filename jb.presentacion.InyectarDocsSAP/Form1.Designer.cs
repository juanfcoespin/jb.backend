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
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            dataGridView1 = new DataGridView();
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
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
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
            tabControl1.Location = new Point(40, 71);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(812, 361);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(804, 333);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Procesando:";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(804, 333);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Documentos con Error";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dataGridView1);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(804, 333);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Log";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(6, 6);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(792, 321);
            dataGridView1.TabIndex = 0;
            // 
            // cmdIniciar
            // 
            cmdIniciar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cmdIniciar.Location = new Point(675, 438);
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
            cmdDetener.Location = new Point(762, 438);
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
            panel1.Location = new Point(728, 12);
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
            ClientSize = new Size(849, 482);
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
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
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
        private DataGridView dataGridView1;
        private Label lblIntentos;
    }
}
