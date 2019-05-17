using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg
{
    public class HabilitadoCanjearPuntosMS
    {
        /// <summary>
        /// 1 proceso Exitoso
        /// -500 error interno en el procesamiento
        /// </summary>
        public int CodResp { get; set; }
        /// <summary>
        /// True: si está habilitado para canjear puntos
        /// False: Si no está habilitado para canjear puntos
        /// </summary>
        public bool Resp { get; set; }
    }
    public class SavedMs:MensajeSalidaMsg {

        public bool Saved { get; set; }
    }
    public class SocioNegocioMsg
    {
        public string Nombre { get; set; }
        public string NombreComercial { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Ciudad { get; set; }
        public SocioNegocioMsg() { }

    }
    public class ParticipantesPuntosMsg
    {
        public bool Activo { get; set; }
        public string Apellidos { get; set; }
        public string Celular { get; set; }
        public string Clave { get; set; }
        public int CupoAnual { get; set; }
        public bool Elite { get; set; }
        public string Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int IdCatalogo { get; set; }
        
        public int IdGenero { get; set; }
        public int IdTipoDocumento { get; set; }
        public string Nombres { get; set; }
        public string NroDocumento { get; set; }
        public string NroDocumentoAnterior { get; set; } //cuado se quiera actualizar el ruc
        public string RucPrincipal { get; set; }
        public string Telefono { get; set; }
        public string Vendedor { get; set; }
        public object Comentario { get; set; }
    }
    public class SocioNegocioItemMsg
    {
        public string Ruc { get; set; }
        public string Nombre { get; set; }
    }
}
