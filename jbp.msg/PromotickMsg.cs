﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Utils;

namespace jbp.msg
{
    public class ParametroAceleradoresMsg
    {
        public string CodigosProductos { get; set; }
        public int Año { get; set; }
        public string Meses { get; set; }
    }
    public class AceleradorMsg
    {
        public string NroDocumento { get; set; }
        public int puntos { get; set; }
    }
    public enum eTipoDocumentoPtk { 
        FacturaDeVenta, 
        NotaDeCredito,
        AjusteFacturaVentas,
        AjusteNotaCredito,
        NoDefinido
    }


    /// <summary>
    /// Este mensaje no tiene semántica, ya que la metadata
    /// del servicio web de promotick está definido así
    /// </summary>
    public class DocumentoPromotickMsg : MensajeSalidaMsg
    {
        public int id { get; set; }
        /// <summary>
        /// fecha del documento
        /// </summary>
        public string fechaFactura { get; set; }

        /// <summary>
        /// Para tener trazabilidad cuando se cambia la fechas por problemas de envio
        /// Ej: si estamos en marzo y por algún problema no se envio algun documento de febrero
        /// </summary>
        public string fechaDocumentoOriginal{ get; set; }
        /// <summary>
        /// numero del documento
        /// </summary>
        public string numFactura { get; set; }
        /// <summary>
        /// se requiere este campo por la estructura del WS
        /// </summary>
        public string descripcion {
            get
            {
                return EnumTipoDocumento.ToString();
            }
        }
        /// <summary>
        /// se refiere al ruc
        /// </summary>
        public string numDocumento { get; set; }
        public int montoFactura { get; set; }
        public int puntos { get; set; }
        public int numIntentosTx { get; set; }
        public eTipoDocumentoPtk EnumTipoDocumento { get; set; }
        public string tipoDocumento{
            get{
                return EnumTipoDocumento.ToString();
            }
        }
        public string RespuestaWS { get; set; }
    }
    public class DocumentosPtkMsg
    {
        public List<DocumentoPromotickMsg> facturas { get; set; }
    }
    public class RespPtkWSFacturasMsg: RespPtkMsg
    {
        public string numFactura { get; set; }
    }
    public class RespPtkMsg {
        public int codigo { get; set; }
        public string mensaje { get; set; }
    }
    public class RespPtkAcelerador:RespPtkMsg {
        public string NroDocumento { get; set; }
    }
    public class RespuestasPtkWsFacturasMsg
    {
        public List<RespPtkWSFacturasMsg> respuesta { get; set; }
    }
    public enum eTipoParticipante { 
        TipoA,
        TipoB,
        NoDefinido
    }
    public class ParticipantesMsg {
        public string Ruc { get; set; }
        public List<string> RucsSecundarios { get; set; } //de los rucs a los que se suma los montos de las facturas
        public eTipoParticipante TipoParticipante { get; set; }
        public string RucsSecundariosSeparadosPorComas { 
            get {
                return StringUtils.GetSqlInStringFromList(this.RucsSecundarios);
            } 
        }
    }
}