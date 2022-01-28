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
    public class RespWsMsg {
        public int codigo { get; set; }
        public string mensaje { get; set; }
    }
    public class SavedMs:MensajeSalidaMsg {

        public bool Saved { get; set; }
    }
    public class SocioNegocioMsg
    {
        public string Ruc { get; set; }
        public string Nombre { get; set; }
        public string ConocidoComo { get; set; }
        public string Telefonos { get; set; }
        public string Celular { get; set; }
        public string Email { get; set; }
        public string Vendedor { get; set; }
        public string Ciudad { get; set; }
        public string Provincia { get; set; }
        public string Direccion { get; set; }
        public string TipoContacto { get; set; }
        public string Genero { get; set; }
        public string TipoIdentificacion { get; set; }
        public string EstadoCivil { get; set; }
        public string Error { get; set; }
        public string TipoSocioNegocio { get; set; }

        public SocioNegocioMsg() { }

    }
    public class ParticipanteCampoMsg
    {
        public string nroDocumento { get; set; }
        public List<ListaCamposMsg> listaCamposActualizar { get; set; }
        public ParticipanteCampoMsg()
        {
            this.listaCamposActualizar = new List<ListaCamposMsg>();
        }
    }
    public class ListaCamposMsg
    {
        public string valor { get; set; }

        public string nombreCampo { get; set; }
    }
    public class ParticipantesPuntosMsg
    {
        public bool Activo { get; set; }
        public string apellidos { get; set; }
        public string celular { get; set; }
        public string clave { get; set; }
        public object Comentario { get; set; }
        private bool _Elite;
        public bool Elite
        {
            get { return _Elite; }
            set
            {
                this._Elite = value;
                this.tipoCatalogo = (value ? 1 : 2);
            }
        }
        public string email { get; set; }
        public int estado { get; set; }
        private DateTime _FechaNacimiento;
        public string nombreComercial;
        public List<DocumentoParticipanteMsg> documentos;

        public DateTime FechaNacimiento
        {
            get { return this._FechaNacimiento; }
            set
            {
                this._FechaNacimiento = value;
                this.fechaNacimiento = value.ToString("dd/MM/yyyy");
            }
        }
        public string fechaNacimiento { get; set; }
        public int idCatalogo { get; set; }
        public int metaAnual { get; set; }
        public string nombres { get; set; }
        public string nroDocumento { get; set; }
        public string NroDocumentoAnterior { get; set; } //cuado se quiera actualizar el ruc
        public string RucPrincipal { get; set; }
        public string telefono { get; set; }
        public int tipoCatalogo { get; set; }
        public int tipoDocumento { get; set; }
        public int tipoGenero { get; set; }
        public string vendedor { get; set; }
        public string Error { get; set; }
        public string vendedorStr { get; set; }
        public string correoVendedor { get; set; }
    }
    public class SocioNegocioItemMsg
    {
        public string Ruc { get; set; }
        public string Nombre { get; set; }
    }
    public class ClientMsg
    {
        public object ruc;
        public string birthDate;
        public string priceListId;
        public string telefono;

        public string id { get; set; }
        public string name { get; set; }
        public string comercialName { get; set; }
        public List<ClientDirectionMsg> directions { get; set; }
        public List<ClientContactMsg> contacts { get; set; }
        public List<ClientVendorOnservationMsg> observations { get; set; }
        public string celular { get; set; }
    }
    public class ClientDirectionMsg
    {
        public string city;
        public string direction;
    }
    public class ClientContactMsg
    {
        public string name;
        public string direction;
        public string phone;
        public string cellular;
        public string email;
    }
    public class ClientVendorOnservationMsg
    {
        public string observation { get; set; }
        public DateTime date { get; set; }
    }
    
    
}
