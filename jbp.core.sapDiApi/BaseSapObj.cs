﻿using System;
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

        public BaseSapObj()
        {
            this.Company= new SAPbobsCOM.Company();
            this.Company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            this.Company.Server = conf.Default.server; 
            this.Company.language= SAPbobsCOM.BoSuppLangs.ln_Spanish;
            this.Company.UseTrusted = false;
            this.Company.DbUserName = conf.Default.dbUser;
            this.Company.DbPassword = conf.Default.dbPwd;
            
        }
        public void StartTransaction() { 
            this.Company.StartTransaction();
        }
        public void RollBackTransaction() {
            this.Company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
        }
        public void CommitTransaction()
        {
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
        public bool Connect(BaseSapObj me=null)
        {
            try
            {
                this.onNotififacationMessage?.Invoke("Conectando a SAP Business One... " + conf.Default.server + " - " + conf.Default.dbName);
                if (me!=null && me.Company!=null)
                    this.Company=me.Company;
                if (this.Company.Connected)
                    return true;
                this.Company.Server = conf.Default.server;
                this.Company.CompanyDB = conf.Default.dbName;
                this.Company.DbUserName = conf.Default.dbUser;
                this.Company.DbPassword = conf.Default.dbPwd;
                this.Company.UserName = conf.Default.sapUser;
                this.Company.Password = conf.Default.sapPwd;
                this.Company.Connect();
                return this.Company.Connected;
            }
            catch
            {
                return false;
            }
        }
        public void Disconnect() {
            if (this.Company!=null && this.Company.Connected)
                this.Company.Disconnect();
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
