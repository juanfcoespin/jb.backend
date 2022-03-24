using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;

namespace jbp.msg.sap
{
    public class MedicamentoConLotesMsg
    {
        public string codArticulo { get; set; }
        public string articulo { get; set; }
        public List<LoteMsg> lotes { get; set; }
    }
    public class ReaccionesMsg
    {
        public int idRangoEdad { get; set; }
        public int idQuienPadecioReaccion { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string sexo { get; set; }
        
        public int pesoKg { get; set; }
        public int alturaCm { get; set; }
        public bool padeceOtraEnfermedad { get; set; }
        public string notificador { get; set; }
        public string notificadorMail { get; set; }
        public string notificadorTelefono { get; set; }
        public string otraEnfermedad { get; set; }
        public List<ReaccionItem> reacciones { get; set; }
        public string fechaRegistro { get; set; }
        public List<MedicamentoItem> medicamentos { get; set; }
        public List<InfoReaccion> informacionesReaccion { get; set; }
        public int id { get; set; }
        public string rangoEdad { get; set; }
        public string quienPadecioEnfermedad { get; set; }
        public List<string> reaccionesStr { get; set; }
    }
    public class InfoReaccion
    {
        public int idReaccion { get; set; }
        public int idEstadoPersonaAfectada { get; set; }
        public string estadoPersonaAfectada { get; set; }
        public string fechaInicio { get; set; }
        public string fechaFin { get; set; }
        public bool siguioTratamiento { get; set; }
        public object sintomas { get; set; }
        public object tratamiento { get; set; }
        public int id { get; set; }
    }
    public class MedicamentoItem
    {
        public int idReaccion { get; set; }
        public int codViaAdministracion { get; set; }
        public int idQuePasoConMedicamento { get; set; }
        public string quePasoConMedicamento { get; set; }
        public string codMedicamento { get; set; }
        public string lote { get; set; }
        public string fechaVencimiento { get; set; }
        public string cantidadFrecuencia { get; set; }
        public string fechaUtilizacion { get; set; }
        public string cuandoDejoUsar { get; set; }
        public bool haVueltoReaccion { get; set; }
        public string paraQueUtilizo { get; set; }
        public string posologia { get; set; }
        public string viaAdministracion { get; set; }
        public string medicamento { get; set; }
        public int id { get; set; }
    }
    public class ReaccionItem : ItemCatalogo
    {
        public bool selected{ get; set; }
    }
    public class CatalogosReacciones
    {
        public  string error { get; set; }

        public List<ItemCatalogo> quienPadecioReaccion { get; set; }
        public List<ItemCatalogo> viaAdministracion { get; set; }
        public List<ItemCatalogo> quePasoConMedicamento { get; set; }
        public List<ItemCatalogo> rangoEdad { get; set; }
        public List<MedicamentoConLotesMsg> medicamentosConLotes { get; set; }
        public List<ItemCatalogo> reacciones { get; set; }
        public List<ItemCatalogo> estadoPersonaAfectada { get; set; }
    }
}
