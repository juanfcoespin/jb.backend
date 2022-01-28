namespace emailSender
{
    partial class EnviarRoles
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtRolesFolder = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExaminar = new System.Windows.Forms.Button();
            this.btnEnviarRoles = new System.Windows.Forms.Button();
            this.pbRoles = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblEncontrados = new System.Windows.Forms.Label();
            this.lblProcesados = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.listaCorreosNoEnviadosBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label8 = new System.Windows.Forms.Label();
            this.FilePath = new System.Windows.Forms.DataGridViewLinkColumn();
            this.Error = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.usuarioDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mailDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uiMsgBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.listaCorreosNoEnviadosBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiMsgBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // txtRolesFolder
            // 
            this.txtRolesFolder.Location = new System.Drawing.Point(18, 71);
            this.txtRolesFolder.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtRolesFolder.Name = "txtRolesFolder";
            this.txtRolesFolder.ReadOnly = true;
            this.txtRolesFolder.Size = new System.Drawing.Size(741, 26);
            this.txtRolesFolder.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 46);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Carpeta Roles:";
            // 
            // btnExaminar
            // 
            this.btnExaminar.Location = new System.Drawing.Point(770, 71);
            this.btnExaminar.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnExaminar.Name = "btnExaminar";
            this.btnExaminar.Size = new System.Drawing.Size(112, 35);
            this.btnExaminar.TabIndex = 4;
            this.btnExaminar.Text = "Examinar...";
            this.btnExaminar.UseVisualStyleBackColor = true;
            this.btnExaminar.Click += new System.EventHandler(this.btnExaminar_Click);
            // 
            // btnEnviarRoles
            // 
            this.btnEnviarRoles.Location = new System.Drawing.Point(18, 177);
            this.btnEnviarRoles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnEnviarRoles.Name = "btnEnviarRoles";
            this.btnEnviarRoles.Size = new System.Drawing.Size(165, 32);
            this.btnEnviarRoles.TabIndex = 5;
            this.btnEnviarRoles.Text = "Enviar Roles";
            this.btnEnviarRoles.UseVisualStyleBackColor = true;
            this.btnEnviarRoles.Click += new System.EventHandler(this.btnEnviarRoles_Click);
            // 
            // pbRoles
            // 
            this.pbRoles.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbRoles.Location = new System.Drawing.Point(18, 261);
            this.pbRoles.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.pbRoles.Name = "pbRoles";
            this.pbRoles.Size = new System.Drawing.Size(875, 32);
            this.pbRoles.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 132);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Roles Encontrados:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 235);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(142, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Roles Procesados:";
            // 
            // lblEncontrados
            // 
            this.lblEncontrados.AutoSize = true;
            this.lblEncontrados.Location = new System.Drawing.Point(177, 132);
            this.lblEncontrados.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblEncontrados.Name = "lblEncontrados";
            this.lblEncontrados.Size = new System.Drawing.Size(18, 20);
            this.lblEncontrados.TabIndex = 10;
            this.lblEncontrados.Text = "0";
            // 
            // lblProcesados
            // 
            this.lblProcesados.AutoSize = true;
            this.lblProcesados.Location = new System.Drawing.Point(166, 235);
            this.lblProcesados.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblProcesados.Name = "lblProcesados";
            this.lblProcesados.Size = new System.Drawing.Size(0, 20);
            this.lblProcesados.TabIndex = 11;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Maroon;
            this.label4.Location = new System.Drawing.Point(18, 318);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(160, 20);
            this.label4.TabIndex = 13;
            this.label4.Text = "Correos no Enviados:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Green;
            this.label5.Location = new System.Drawing.Point(359, 318);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 20);
            this.label5.TabIndex = 14;
            this.label5.Text = "Correos Enviados:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uiMsgBindingSource, "CorreosNoEnviados", true));
            this.label6.ForeColor = System.Drawing.Color.Maroon;
            this.label6.Location = new System.Drawing.Point(191, 318);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(18, 20);
            this.label6.TabIndex = 15;
            this.label6.Text = "0";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.uiMsgBindingSource, "CorreosEnviados", true));
            this.label7.ForeColor = System.Drawing.Color.Green;
            this.label7.Location = new System.Drawing.Point(527, 318);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 20);
            this.label7.TabIndex = 16;
            this.label7.Text = "0";
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.usuarioDataGridViewTextBoxColumn,
            this.mailDataGridViewTextBoxColumn,
            this.FilePath,
            this.Error});
            this.dataGridView1.DataSource = this.listaCorreosNoEnviadosBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(22, 376);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(871, 168);
            this.dataGridView1.TabIndex = 17;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // listaCorreosNoEnviadosBindingSource
            // 
            this.listaCorreosNoEnviadosBindingSource.DataMember = "ListaCorreosNoEnviados";
            this.listaCorreosNoEnviadosBindingSource.DataSource = this.uiMsgBindingSource;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(23, 353);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(211, 20);
            this.label8.TabIndex = 18;
            this.label8.Text = "Detalle correos no Enviados:";
            // 
            // FilePath
            // 
            this.FilePath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.FilePath.DataPropertyName = "FilePath";
            this.FilePath.HeaderText = "Ubicación Rol";
            this.FilePath.MinimumWidth = 8;
            this.FilePath.Name = "FilePath";
            this.FilePath.ReadOnly = true;
            this.FilePath.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.FilePath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // Error
            // 
            this.Error.DataPropertyName = "Error";
            this.Error.HeaderText = "Error";
            this.Error.MinimumWidth = 8;
            this.Error.Name = "Error";
            this.Error.ReadOnly = true;
            this.Error.Width = 150;
            // 
            // usuarioDataGridViewTextBoxColumn
            // 
            this.usuarioDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.usuarioDataGridViewTextBoxColumn.DataPropertyName = "Usuario";
            this.usuarioDataGridViewTextBoxColumn.HeaderText = "Usuario";
            this.usuarioDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.usuarioDataGridViewTextBoxColumn.Name = "usuarioDataGridViewTextBoxColumn";
            this.usuarioDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // mailDataGridViewTextBoxColumn
            // 
            this.mailDataGridViewTextBoxColumn.DataPropertyName = "Mail";
            this.mailDataGridViewTextBoxColumn.HeaderText = "Mail";
            this.mailDataGridViewTextBoxColumn.MinimumWidth = 8;
            this.mailDataGridViewTextBoxColumn.Name = "mailDataGridViewTextBoxColumn";
            this.mailDataGridViewTextBoxColumn.ReadOnly = true;
            this.mailDataGridViewTextBoxColumn.Width = 150;
            // 
            // uiMsgBindingSource
            // 
            this.uiMsgBindingSource.DataSource = typeof(emailSender.UiMsg);
            // 
            // EnviarRoles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 571);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblProcesados);
            this.Controls.Add(this.lblEncontrados);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pbRoles);
            this.Controls.Add(this.btnEnviarRoles);
            this.Controls.Add(this.btnExaminar);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtRolesFolder);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "EnviarRoles";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "EnviarRoles";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.listaCorreosNoEnviadosBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uiMsgBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtRolesFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExaminar;
        private System.Windows.Forms.Button btnEnviarRoles;
        private System.Windows.Forms.ProgressBar pbRoles;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblEncontrados;
        private System.Windows.Forms.Label lblProcesados;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.BindingSource uiMsgBindingSource;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource listaCorreosNoEnviadosBindingSource;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridViewTextBoxColumn usuarioDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mailDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewLinkColumn FilePath;
        private System.Windows.Forms.DataGridViewTextBoxColumn Error;
    }
}