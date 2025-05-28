using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.Core.Hana;
using System.Data;
using jbp.msg.sap;

namespace jbp.business.hana
{
    public class OrdenFabricacionBusiness
    {
        public static List<OrdenFabricacionLiberadaPesajeMsg> GetOfLiberadasPesaje(string codPT=null, string codInsumo=null)
        {
            var ms = new List<OrdenFabricacionLiberadaPesajeMsg>();
            var sql = @"
                select 
                 ""DocNum"",
                 ""CodArticulo"",
                 ""Articulo""
                from  
                 ""JbpVw_OrdenFabricacion""
                where ""DocNum"" in(
                 select	distinct DOC_NUM_OF from JB_LOTES_PESAJE
                )
            ";
            if (!string.IsNullOrEmpty(codPT) && !string.IsNullOrEmpty(codInsumo))
            {
                sql = string.Format(@"
                {0}
                 and ""CodArticulo""='{1}'
                 and ""CodInsumo""='{2}'
                ", sql, codPT, codInsumo);
            }
            
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new OrdenFabricacionLiberadaPesajeMsg() { 
                    NumOrdenFabricacion=bc.GetInt(dr["DocNum"]),
                    CodigoArticulo= dr["CodArticulo"].ToString(),
                    Descripcion= dr["Articulo"].ToString()
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
                 ""CantidadPlanificada"",
                 ""Lote"",
                 ""BodegaDesde"",
                 ""BodegaHasta"",
                 ""LoteInsumo"",
                 ""FechaVencimiento"",
                 ""Observaciones"",
                 ""Cantidad"",
                 ""IdST"",
                 ""LineNumST""
                from 
                 ""JbVw_OFsConTSaPesaje""
                where
                 ""DocNum""={0}                  

            ", docNum);
            if (!string.IsNullOrEmpty(codInsumo))
            {
                sql += string.Format(@"
                 and ""CodInsumo""='{0}'
                ", codInsumo);
            }
            else
                sql += string.Format(@"
                order by 
                   ""CodInsumo"" 
            ");

            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);

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
                        LineNumST = bc.GetInt(dr["LineNumST"]),
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
        public static OFMasComponentesMsg GetComponentesAPesarOfByDocNumBK(int docNum, string codInsumo = null)
        {
            var ms = new OFMasComponentesMsg();
            ms.NumOrdenFabricacion = docNum;
            var sql = string.Format(@"
                 select 
                  t1.""Id"",
                  t1.""DocNum"",
                  t1.""CodArticulo"",
                  t1.""Articulo"",
                  t2.""CodInsumo"",
                  t2.""UnidadMedida"",
                  t2.""Insumo"",
                  t0.""CantidadPlanificada"",
                  t0.""CantidadPesada"",
                  t1.""Articulo"",
                  t1.""Lote"",
                  t1.""BodegaHasta"",
                  t3.""Lote"" ""LoteInsumo"",
                  t3.""FechaVencimiento"",
                  t3.""Observaciones"",
                  t3.""Cantidad""
                 from 
                  ""JbpVw_OrdenFabricacionLinea"" t0 inner join
                  ""JbpVw_OrdenFabricacion"" t1 on t1.""Id""=t0.""IdOrdenFabricacion"" inner join
                  ""JbpVw_Insumos"" t2 on t2.""CodInsumo""=t0.""CodInsumo"" inner join
                  ""JbVw_OFsConTSaPesaje"" t3 on t3.""DocNumOrdenFabricacion""=t1.""DocNum"" and t3.""CodArticulo""=t0.""CodInsumo""
                 where
                  t1.""DocNum""={0}
                  and t2.""TipoInsumo""='Artículo'
                  and ( --solo componentes sujetos a pesarse ver si se incluyen litros
                     lower(t2.""UnidadMedida"") like '%kg%'
                     or lower(t2.""UnidadMedida"") like '%g%'
                     or lower(t2.""UnidadMedida"") like '%mg%'
                  )
                  

            ", docNum);
            if (!string.IsNullOrEmpty(codInsumo))
            {
                sql += string.Format(@"
                 and t2.""CodInsumo""='{0}'
                ", codInsumo);
            }
            else
                sql += string.Format(@"
                order by 
                   t2.""CodInsumo"" 
            ");

            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);

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
                    ms.BodegaDesde = "PSJ1";
                    ms.BodegaHasta = dr["BodegaHasta"].ToString();
                    ms.LotePT = dr["Lote"].ToString();
                }
                codInsumoActual = dr["CodInsumo"].ToString();
                if (codInsumoActual != null && (codInsumoActual != codInsumoAnterior))
                { //incluyo componente
                    componente = new ComponentesMsg
                    {
                        CodigoArticulo = dr["CodInsumo"].ToString(),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                        Descripcion = dr["Insumo"].ToString(),
                        CantidadRequerida = bc.GetDecimal(dr["CantidadPlanificada"], 6),
                        CantidadPesada = bc.GetDecimal(dr["CantidadPesada"], 6),
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
                    call ""JbSP_LotesTransferidosPorArticuloOF""('{0}', '{1}')
                ", docNumOF, codigoArticulo);
                var bc = new BaseCore();
                var dt=bc.GetDataTableByQuery(sql);
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
                 ""DocNum"" = {0}
            ", docNum);
            return new BaseCore().GetIntScalarByQuery(sql);
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
                 ""Id"" = {0}
            ",idOF);
            return new BaseCore().GetScalarByQuery(sql);
        }
        
    }
}
