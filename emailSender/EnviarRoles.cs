using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using jbp.business.hana;
using TechTools.Net;
using static System.Net.WebRequestMethods;
using System.Net.Mail;
using System.Net;

namespace emailSender
{
    public partial class EnviarRoles : Form
    {
        private List<string> roles = new List<string>();
        private UiMsg ui = new UiMsg();
        public EnviarRoles()
        {
            InitializeComponent();
            
        }

        private void btnExaminar_Click(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            pbRoles.Value = 0;
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                txtRolesFolder.Text = fbd.SelectedPath;
                var filesName = Directory.GetFiles(txtRolesFolder.Text);
                this.roles = getRoles(filesName);
                lblEncontrados.Text = this.roles.Count.ToString();
                pbRoles.Maximum = this.roles.Count;
                if (this.roles.Count == 0)
                    MessageBox.Show("La carpeta seleccionada no contiene roles!!");
                else 
                    btnEnviarRoles.Enabled = true;
            }
                
        }

        private void btnEnviarRoles_Click(object sender, EventArgs e)
        {
            //enviarCorreoPrueba();
            btnEnviarRoles.Enabled = false;
            this.ui.ListaCorreosNoEnviados.Clear();
            this.ui = new UiMsg();
            this.uiMsgBindingSource.DataSource = this.ui;
            backgroundWorker1.RunWorkerAsync();
            button1.Enabled = true;

        }
        public void enviarCorreoPrueba() {
            var mail = "juanfco.espin@gmail.com";
            //var mail = "jespin@jbp.com.ec";
            string error = null;
            var msg = "hola";
            MailUtils.SendAndGetError(ref error, mail, conf.Default.asunto, msg);

        }

        private static List<string> getRoles(string[] filesName)
        {
            var rolFiles = new List<string>();
            foreach (var fileName in filesName)
            {
                //Ej. RolFile: 1803281631_Juan Francisco Espin.pdf
                if (fileName.Contains("_") && fileName.Contains(".pdf"))
                    rolFiles.Add(fileName);
            }
            return rolFiles;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var processed = 0;
            this.roles.ForEach(rolFileWithPath =>
            {
                if (backgroundWorker1.CancellationPending)
                {
                    
                    e.Cancel = true;
                    return; //cancela el procesamiento
                }
                //Ej. RolFile: 1803281631_Juan Francisco Espin.pdf
                var rolFile = GetRoleFileName(rolFileWithPath);
                var matrix = rolFile.Split(new char[] { '_', '.' });
                if (matrix.Length >= 2)
                {
                    var cedula = matrix[0];
                    var mail = EmpleadoBusiness.GetMailByCedula(cedula);
                    //var mail = "juan.espin@yahoo.com";
                    var msg = conf.Default.cuerpoMensaje;
                    //para que lea html ya que en el archivo app.config no se puede registrar html 
                    msg = msg.Replace("{", "<");
                    msg = msg.Replace("}", ">");
                    var ms = new EmailResp();
                    ms.Usuario = matrix[1];
                    ms.Mail = mail;
                    var error = "";
                    var files=new List<string>();
                    files.Add(rolFileWithPath);
                    //var enviarCorreo = !(mail.ToLower().Contains("@jbp.com.ec") || mail.ToLower().Contains("@jamesbrownpharma.com"));
                    //if (enviarCorreo) {
                        var enviado = MailUtils.SendAndGetError(ref error, mail, conf.Default.asunto, msg, files);
                        if (enviado)
                            this.ui.CorreosEnviados++;
                        else
                        {
                            ms.Error = error;
                            ms.FilePath = rolFileWithPath;
                            this.ui.ListaCorreosNoEnviados.Add(ms);
                            this.ui.CorreosNoEnviados++;
                        }
                    //}
                    processed++;
                    backgroundWorker1.ReportProgress(processed);
                }
            });
        }

        private string GetRoleFileName(string rolFileWithPath)
        {
            //Ej. RolFile: 1803281631_Juan Francisco Espin.pdf
            var matrix = rolFileWithPath.Split(new char[] { '\\' });
            return matrix[matrix.Length-1];
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbRoles.Value=e.ProgressPercentage;
            lblProcesados.Text = string.Format("{0} de {1}",
                pbRoles.Value, pbRoles.Maximum);
            ;
            mostrarResultadoEnvio();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnEnviarRoles.Enabled = true;
            MessageBox.Show("Se finalizó el procesamiento", "Mensaje del Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void mostrarResultadoEnvio()
        {
            uiMsgBindingSource.ResetBindings(false);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Rows.Count>0 && e.ColumnIndex>0 && e.ColumnIndex == 2) //link file
            {
                var path= dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                System.Diagnostics.Process.Start(path);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            MessageBox.Show("Se está finalizando el procesamiento!!");
            backgroundWorker1.CancelAsync();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /*var bb = new BaseBusiness();
            bb.EnviarPorCorreo("jespin@jbp.com.ec", "test correo", "msg correo");*/
            testCorreo();
        }
        private void testCorreo() {
            try
            {
                var from = "rr_hh@jbp.com.ec";
                var to = "jespin@jbp.com.ec";
                //var contraseñaApp = "dQn8Q~t.16l1ukNrcgAE4vC66hVd5jKdpaYI0a1a"; // Tu contraseña de aplicación
                var contraseñaApp = "486750"; // Tu contraseña de aplicación

                MailMessage mail = new MailMessage(from, to);
                mail.Subject = "Prueba de conexión SMTP";
                mail.Body = "Este es un mensaje de prueba enviado desde C# con Office 365.";

                SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential(from, contraseñaApp);
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Timeout = 20000;

                smtp.Send(mail);
                MessageBox.Show(to + " - Correo enviado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al enviar el correo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
    }
}
