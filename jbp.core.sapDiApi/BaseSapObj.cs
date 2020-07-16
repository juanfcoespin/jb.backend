using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.core.sapDiApi
{
    public class BaseSapObj
    {
        public SAPbobsCOM.Documents obj;
        public SAPbobsCOM.Company Company;

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
        private bool Connect()
        {
            try
            {
                this.Company.CompanyDB = conf.Default.dbName;
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
        private void Disconnect() {
            if (this.Company.Connected)
                this.Company.Disconnect();
        }
    }
}
