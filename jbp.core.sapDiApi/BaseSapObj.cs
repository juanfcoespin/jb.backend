using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace jbp.core.sapDiApi
{
    public class BaseSapObj
    {
        public SAPbobsCOM.Documents obj;
        public SAPbobsCOM.Company Company;
        public delegate void dDisconected(bool connected);
        public event dDisconected onDisconected;
        public delegate void dNotififacationMessage(string msg);
        public event dNotififacationMessage onNotififacationMessage;
        public string LastError { get; set; }
        public int LastErrorCode { get; set; }

        public string GetLastErrorDescription()
        {
            if (!string.IsNullOrEmpty(this.LastError))
                return this.LastError;

            try
            {
                if (this.Company != null)
                {
                    string desc = this.Company.GetLastErrorDescription();
                    if (!string.IsNullOrWhiteSpace(desc))
                        return desc;
                }
            }
            catch
            {
            }
            return "";
        }

        public BaseSapObj()
        {
            this.Company = new SAPbobsCOM.Company();
            this.Company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            this.Company.Server = conf.Default.server;
            this.Company.language = SAPbobsCOM.BoSuppLangs.ln_Spanish;
            this.Company.UseTrusted = false;
            this.Company.DbUserName = conf.Default.dbUser;
            this.Company.DbPassword = conf.Default.dbPwd;

        }
        public void StartTransaction()
        {
            this.Company.StartTransaction();
        }
        public void RollBackTransaction()
        {
            if (this.Company.InTransaction)
                this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
        }
        public void CommitTransaction()
        {
            if (this.Company.InTransaction)
                this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
        }
        public bool IsConected()
        {
            return this.Company.Connected;
        }
        public void sendNotififacationMessage(string msg)
        {
            this.onNotififacationMessage?.Invoke(msg);
        }
        public bool Connect(BaseSapObj me = null)
        {
            try
            {
                this.LastError = "";
                this.LastErrorCode = 0;
                this.onNotififacationMessage?.Invoke("Conectando a SAP Business One... " + conf.Default.server + " - " + conf.Default.dbName);
                if (me != null && me.Company != null)
                    this.Company = me.Company;
                if (this.Company.Connected)
                {
                    this.onNotififacationMessage?.Invoke("Se ha establecido anteriormente la conexión a SAP");
                    return true;
                }

                this.Company.Server = conf.Default.server;
                this.Company.CompanyDB = conf.Default.dbName;
                this.Company.DbUserName = conf.Default.dbUser;
                this.Company.DbPassword = conf.Default.dbPwd;
                this.Company.UserName = conf.Default.sapUser;
                this.Company.Password = conf.Default.sapPwd;
                int retCode = this.Company.Connect();
                var resp = this.Company.Connected;
                if (!resp || retCode != 0)
                {
                    int errCode = 0;
                    string errMsg = "";
                    this.Company.GetLastError(out errCode, out errMsg);
                    this.LastErrorCode = errCode != 0 ? errCode : retCode;
                    this.LastError = !string.IsNullOrWhiteSpace(errMsg)
                        ? errMsg
                        : this.Company.GetLastErrorDescription();

                    if (string.IsNullOrWhiteSpace(this.LastError))
                    {
                        this.LastError = $"Error al conectar con SAP (Código: {this.LastErrorCode})";
                    }
                    else
                    {
                        this.LastError = $"[Código: {this.LastErrorCode}] {this.LastError}";
                    }
                    this.onNotififacationMessage?.Invoke("Fallo conexión SAP: " + this.LastError);
                }
                else
                {
                    this.onNotififacationMessage?.Invoke("Respuesta de conexión a SAP: " + resp);
                }
                return resp;
            }
            catch (Exception ex)
            {
                this.LastError = $"Excepción al conectar a SAP: {ex.Message}";
                this.onNotififacationMessage?.Invoke(this.LastError);
                return false;
            }
        }
        public void Disconnect()
        {
            if (this.Company != null && this.Company.Connected)
            {
                this.onNotififacationMessage?.Invoke("Desconectando de SAP: ");
                this.Company.Disconnect();
                var resp = this.Company.Connected;
                this.onNotififacationMessage?.Invoke("Conectado a SAP: " + resp);
            }

            //CheckIfIsDisconectedAsync();
        }

        private void CheckIfIsDisconectedAsync()
        {
            //wait until diApi disconect to DB in other thread for 2 minutes
            var worker = new BackgroundWorker();
            var connected = true;
            worker.DoWork += delegate {
                int timeElapsedInSeconds = 0;
                int waitTimeInSeconds = 120;
                var waitTimeInMiliseconds = 1000;
                while (timeElapsedInSeconds <= waitTimeInSeconds)
                {
                    if (!this.Company.Connected)
                    {
                        connected = false;
                        timeElapsedInSeconds = waitTimeInSeconds;
                    }
                    else
                    {
                        connected = true;

                        timeElapsedInSeconds += waitTimeInMiliseconds;
                        Thread.Sleep(waitTimeInMiliseconds);
                    }
                }
            };
            worker.RunWorkerCompleted += (s, e) => onDisconected?.Invoke(connected);
            worker.RunWorkerAsync();
        }
    }
}
