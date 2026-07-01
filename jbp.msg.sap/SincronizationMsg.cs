using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public enum eTipoDocToSync
    {
        NoDefinido,
        Pedido,
        Cobro
    }
    public enum eTipoMsg
    {
        Error,
        Info
    }
    
    public abstract class DocsToSyncMsg
    {
        public string IdCache { get; set; }
        public string FechaSincronizacionVendedor { get; set; }
        public string CodCliente { get; set; }
        public string Cliente { get; set; }
        public abstract double Total { get; }
        public string Vendedor { get; set; }
        public abstract string TipoDocumento { get; }
        public string FechaIngresoSap { get; set; }
        public bool TieneError { 
            get {
                if(this.MensajesSincronizacion==null || this.MensajesSincronizacion.Count==0)
                    return false;
                return this.MensajesSincronizacion.Exists(msg => msg.TipoMsg == eTipoMsg.Error);
            } 
        }
        public List<MsgSincronizacion> MensajesSincronizacion { get; set; }
        public  string JsonObj { get; set; }
        public string Error { get; set; }

        public DocsToSyncMsg() {
            this.MensajesSincronizacion = new List<MsgSincronizacion>();
        }
    }
    public class MsgSincronizacion {
        public string Key { get; set; }
        public string FechaLog { get; set; }
        public string Msg { get; set; }
        public eTipoMsg TipoMsg { get; set; }
        
    }
    public class FiltroHistoricoMsg
    {
        public string Vendedor { get; set; }
        public string Cliente { get; set; }
        public string TipoDocumento { get; set; }
        public DateTime Desde { get; set; }
        public DateTime Hasta { get; set; }
    }
}