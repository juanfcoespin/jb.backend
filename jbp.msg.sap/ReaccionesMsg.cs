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
        public string fechaRegistro { get; set; }
        public string nombres { get; set; }
        public string apellidos { get; set; }
        public string sexo { get; set; }
        public string rangoEdad { get; set; }
        public int pesoKg { get; set; }
        public int alturaCm { get; set; }
        public string quienPadecioReaccion { get; set; }
        public string padeceOtraEnfermedad { get; set; }
    }
    public class CatalogosReacciones
    {
        public  string error { get; set; }

        public List<ItemCatalogo> quienPadecioReaccion { get; set; }
        public List<ItemCatalogo> viaAdministracion { get; set; }
        public List<ItemCatalogo> quePasoConMedicamento { get; set; }
        public List<ItemCatalogo> rangoEdad { get; set; }
        public List<MedicamentoConLotesMsg> medicamentosConLotes { get; set; }
    }
}
