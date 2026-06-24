using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;
using System.Data;
using jbp.msg.sap;
using System.Threading;

namespace jbp.business.hana
{
    public class OrdenFabricacionBusiness
    {
        public static List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje(string codArticuloAFabricar = null, string codInsumo = null)
        {
            var ms = new List<OrdenFabricacionLiberadaPesajeMsg>();
            var sql = @"
                select 
                 distinct
                 ""DocNum"",
                 ""CodArticulo"",
                 ""Articulo"",
                 ""Lote""
                from  
                 ""JbVw_OFsConTSaPesaje""
            ";
            var hayParametros = false;
            var parametros = new Dictionary<string, object> { };
            if (!string.IsNullOrEmpty(codArticuloAFabricar))
            {
                hayParametros = true;
                sql += string.Format(@"
                 where ""CodArticulo""=?
                ");
                parametros.Add("@0", codArticuloAFabricar);
            }
            if (!string.IsNullOrEmpty(codArticuloAFabricar) && !string.IsNullOrEmpty(codInsumo))
            {
                hayParametros = true;
                sql += string.Format(@"
                 and ""CodInsumo""=?
                ");
                parametros.Add("@1", codInsumo);
            }
            //lote del producto a fabricarse (para que se respete el orden de resarva de los lotes de los componentes)
            sql += @"
                order by 
                 ""Lote"",""DocNum"" 
            ";
            
            var bc = new BaseCore();
            if (!hayParametros)
                parametros = null;
            var dt = bc.GetDataTableByQuery(sql, parametros);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new OrdenFabricacionLiberadaPesajeMsg() { 
                    NumOrdenFabricacion=bc.GetInt(dr["DocNum"]),
                    CodigoArticulo= dr["CodArticulo"].ToString(),
                    Descripcion= dr["Articulo"].ToString(),
                    Lote= dr["Lote"].ToString()
                });
            }
            return ms;
        }
        public static OFMasComponentesMsg GetComponentesAPesarOfByDocNum(int docNum, string codInsumo = null)
        {
            var ms = new OFMasComponentesMsg();
            ms.NumOrdenFabricacion = docNum;
            var sql = string.Format(@"
            select 
             ""Id"",
             ""DocNum"",
             ""CodArticulo"",
             ""Articulo"",
             ""CodInsumo"",
             ""UnidadMedida"",
             ""Insumo"",
             ""BodegaDesde"",
             ""BodegaHasta"",
             ""Lote"",
             ""FechaVencimiento"",
             ""Observaciones"",
             ""IdST"",
             ""LoteInsumo"",
             sum(""CantidadPlanificada"") ""CantidadPlanificada"",
             sum(""Cantidad"") ""Cantidad"" --reservada del lote
            from 
             ""JbVw_OFsConTSaPesaje""
            where
             ""DocNum""=?
            " );
            var parametros = new Dictionary<string, object> {
                {"@0",docNum }
            };
            if (!string.IsNullOrEmpty(codInsumo))
            {
                sql += string.Format(@"
                 and ""CodInsumo""=?
                ");
                parametros.Add("@1",codInsumo);
            }
            
            sql = string.Format(@"
            {0}
            group by
             ""Id"",
             ""DocNum"",
             ""CodArticulo"",
             ""Articulo"",
             ""CodInsumo"",
             ""UnidadMedida"",
             ""Insumo"",
             ""BodegaDesde"",
             ""BodegaHasta"",
             ""Lote"",
             ""FechaVencimiento"",
             ""Observaciones"",
             ""IdST"",
             ""LoteInsumo""
            ", sql);

            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql, parametros);

            //var bodegasComponentes = GetBodegasComponentes(docNum);
            string codInsumoAnterior = null;
            string codInsumoActual = null;
            var componente = new ComponentesMsg();
            foreach (DataRow dr in dt.Rows)
            {
                if (ms.CodArticulo == null) // para registrar la cabecera del mensaje
                {
                    ms.IdOf = bc.GetInt(dr["Id"].ToString());
                    ms.CodArticulo = dr["CodArticulo"].ToString();
                    ms.Descripcion = dr["Articulo"].ToString();
                    var docNumOf = bc.GetInt(dr["DocNum"]);
                    // bodega origen y destino para los componentes fraccionados
                    ms.BodegaDesde = dr["BodegaDesde"].ToString(); 
                    ms.BodegaHasta = dr["BodegaHasta"].ToString();
                    ms.LotePT = dr["Lote"].ToString();
                    ms.IdST = bc.GetInt(dr["IdST"]);
                }
                codInsumoActual = dr["CodInsumo"].ToString();
                if (codInsumoActual != null && (codInsumoActual != codInsumoAnterior))
                { //incluyo componente
                    componente = new ComponentesMsg
                    {
                        CodigoArticulo = dr["CodInsumo"].ToString(),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                        Descripcion = dr["Insumo"].ToString(),
                        //LineNumST = bc.GetInt(dr["LineNumST"]),
                        CantidadRequerida = bc.GetDecimal(dr["CantidadPlanificada"], 6),
                        CantidadesPorLote = new List<CantidadLoteOFMsg>()
                    };

                }
                //Añado lotes al componente nuevo o existente
                componente.CantidadesPorLote.Add(new CantidadLoteOFMsg
                {
                    Lote = dr["LoteInsumo"].ToString(),
                    Cantidad = bc.GetDecimal(dr["Cantidad"], 6),
                    FechaVence = dr["FechaVencimiento"].ToString(),
                    AnalisisMP = dr["Observaciones"].ToString(),
                }
                );
                if (codInsumoActual != null && (codInsumoActual != codInsumoAnterior))
                    ms.Componentes.Add(componente);
                codInsumoAnterior = codInsumoActual;
            }
            return ms;
        }
        private static List<CantidadLoteOFMsg> GetCantidadesPorLote(int docNumOF, string codigoArticulo)
        {
            var ms=new List<CantidadLoteOFMsg>();
            try
            {
                var sql = string.Format(@"
                    call ""JbSP_LotesTransferidosPorArticuloOF""(?, ?)
                ");
                var bc = new BaseCore();
                var dt=bc.GetDataTableByQuery(sql, new Dictionary<string, object> {
                    {"@0" , docNumOF }, {"@1",codigoArticulo }
                });
                foreach (DataRow dr in dt.Rows) {
                    ms.Add(new CantidadLoteOFMsg
                    {
                        Lote = dr["Lote"].ToString(),
                        Cantidad = bc.GetDecimal(dr["Cantidad"],6),
                        FechaVence = dr["FechaVencimiento"].ToString(),
                        AnalisisMP = dr["Observaciones"].ToString(),
                    }); ;
                }
            }
            catch(Exception e){//en vez del lote se inyecta el error
                ms.Add(new CantidadLoteOFMsg
                {
                    Lote = string.Format("Error: {0}", e.Message)
                });
            }
            return ms;
        }

        internal static int GetIdByDocNum(int docNum)
        {
            var sql = string.Format(@"
                select
                ""Id""
                from
                 ""JbpVw_OrdenFabricacion""
                where
                 ""DocNum"" = ?
            ");
            return new BaseCore().GetIntScalarByQuery(sql, new Dictionary<string, object> { { "@0", docNum } });
        }

        internal static bool EstaLiberada(int idOF)
        {
            var estado = GetEstado(idOF);
            return estado == "Liberado";
        }

        public static string GetEstado(int idOF)
        {
            var sql = string.Format(@"
                select
                ""Estado""
                from
                 ""JbpVw_OrdenFabricacion""
                where
                 ""Id"" = ?
            ");
            return new BaseCore().GetScalarByQuery(sql, new Dictionary<string, object> { { "@0", idOF } });
        }

        public static object getOrdenFab(int DocNum)
        {
            var sql = string.Format(@"
                select
                ""Id"",
                ""DocNum"",
                ""CodArticulo"",
                ""Articulo"",
                ""Lote"",
                ""FechaFabricacion"",
                ""FechaInicio"",
                ""FechaVencimiento"",
                ""FechaFinalizacion"",
                ""FechaCierre"",
                ""FechaCreacion"",
                ""Estado""
                from
                 ""JbpVw_OrdenFabricacion""
                where
                 ""DocNum"" = ?
            ");
            var resp = new BaseCore().GetDataTableByQuery(sql, new Dictionary<string, object> {
                    {"@0" , DocNum }
                });

            if (resp != null && !string.IsNullOrEmpty(resp.ToString())){
                // toJSON
                return new System.Web.Script.Serialization.JavaScriptSerializer().DeserializeObject(resp.ToString());
            }
            return resp;
        }

        public static void crearCampania(CampaniaRequest datos)
        {
            BaseCore bc = new BaseCore();

            try{
                bc.BeginTransaction();

                // nueva campaña
                var sqlCampania = @"
                    INSERT INTO JB_CAMPANIA( NOMBRE, FECHA_DESDE, FECHA_HASTA, FINALIZADA)
                    VALUES( ?, ?, ?, ?)";
                bc.ExecuteQueryTransaction( sqlCampania, new Dictionary<string, object>{
                        {"@1", datos.NombreCampania},
                        {"@2", datos.FechaDesde},
                        {"@3", datos.FechaHasta},
                        {"@4", datos.Finalizada}
                    }
                );

                // Obtener el ID
                var sqlId = @"SELECT CURRENT_IDENTITY_VALUE() FROM DUMMY";
                int campaniaId = bc.GetIntScalarTransaction(sqlId, null);

                // Insertar detalle
                foreach (var of in datos.OrdenesFabricacion){
                    var sqlDetalle = @"
                        INSERT INTO JB_ORDENES_FAB_CAMP(ID_CAMPANIA,NRO_OF)
                        VALUES(?, ?)";
                    bc.ExecuteQueryTransaction( sqlDetalle, new Dictionary<string, object>{
                            {"@0", campaniaId},
                            {"@1", of.DocNum}
                        }
                    );
                }

                bc.Commit();
            }
            catch{
                bc.Rollback();
                throw;
            }
        }

    }
}
