using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using jbp.msg;
using TechTools.Core.Hana;
using System.Data;
using jbp.msg.sap;
using jbp.core.sapDiApi;
using System.Threading;



namespace jbp.business.hana
{
    public class SolicitudTransferenciaBusiness
    {
        public static readonly object control = new object();
        //public static SapSolicitudTransferencia sapST = new SapSolicitudTransferencia();

        public static DocSapInsertadoMsg Save(StMsg me, int numIntentos = 0, SapTransferenciaStock sapTransferenciaStock=null)
        {
            /*
                se pasa como parametro xq sapTransferenciaStock puede iniciar una tx
                (se requiere que la conexión a sap sea de la misma compañía)
            */
            if (numIntentos > 3)
                throw new Exception("Se ha tratado de procesar esta ST por 3 veces y no se ha podido establecer conexión con SAP!!");

            Monitor.Enter(control);
            var ms = new DocSapInsertadoMsg();
            try
            {

                // se instancia aquí porque se utiliza esta función en transacciones multi hilo
                // con la finalidad de que no se comparta el objeto sapST
                var sapST = new SapSolicitudTransferencia(); 
                ConectarASap(sapST, sapTransferenciaStock);
                ms = sapST.AddST(me);
            }
            catch (Exception e)
            {
                if (e.Message == "You are not connected to a company" || e.Message.Contains("RPC_E_SERVERFAULT"))
                {
                    //me vuelvo a conectar y reproceso
                    numIntentos++;
                    Save(me, numIntentos);
                }
                else
                    ms.Error=e.Message;
            }
            finally
            {
                Monitor.Exit(control);
            }
            return ms;
        }
        private static void ConectarASap(SapSolicitudTransferencia sapST, BaseSapObj baseSapObj)
        {
            if (sapST == null)
                sapST = new SapSolicitudTransferencia();

            if (!sapST.IsConected())
            {
                if (!sapST.Connect(baseSapObj)) // cuando no se puede conectar es por que el obj sap se inhibe
                {
                    sapST = null;
                    sapST = new SapSolicitudTransferencia(); //se reinicia el objeto para hacer otro intento de conexión
                    if (!sapST.Connect(baseSapObj))
                    {
                        sapST = null;
                        throw new Exception("Alta concurrencia: Vuelva a intentar la sincronización en 1 minuto");
                    }
                }
            }
        }

        public static string GetLineaByCodBodegaMat(string codBodegaMat) {
            switch (codBodegaMat) {
                case "MAT1":
                    return "HUM";
                case "MAT2":
                    return "VET";
            }
            return null;
        }
        public static object GetST_OF_Liberadas(FiltroPickingProdME filtro) 
        {
            /*
             Son las St correspondientes a las OF liberadas que se requiere para hacer el picking 
             de  producción desde el app de gestión de bodegas 
            */
            try
            {
                var ms = new List<ST_OF_LiberadasMsg>();
                if (filtro == null || string.IsNullOrEmpty(filtro.CodBodegaMat))
                    throw new Exception("No se ha enviado como parámetro el código de bodega de materia prima (CodBodegaMat)");
                filtro.linea = GetLineaByCodBodegaMat(filtro?.CodBodegaMat?.ToString());


                if (string.IsNullOrEmpty(filtro.linea))
                    filtro.linea = "HUM";

                var sql = string.Format(@"
                   select 
                     t0.""Id"",   
                     t0.""DocNum"" ""NumST"",
                     t1.""DocNum"" ""NumOF"",
                     to_char(t1.""FechaInicio"", 'yyyy-mm-dd') ""FechaInicioOf"",
                     t1.""Articulo"",
                     t1.""CantidadPlanificada"",
                     t1.""UnidadMedida"",
                     t0.""BodegaOrigen"",
                     t0.""BodegaDestino""
                    from
                     ""JbpVw_SolicitudTraslado"" t0 inner join
                     ""JbpVw_OrdenFabricacion"" t1 on cast(t1.""DocNum"" as nvarchar(50)) = t0.""DocNumOrdenFabricacion""
                    where
                     t0.""DocStatus"" = 'O'--Abierto
                     and upper(""Serie"") like '%{0}%' 
                     and t1.""Estado"" = 'Liberado'
                     and upper(t0.""BodegaOrigen"") = '{1}' -- porque se enviará de MAT->PSJ (picking) luego de PSJ->PROD (área pesaje)
                     and t1.""FraccionadoPesaje"" != 'SI'  
                ", filtro.linea, filtro.CodBodegaMat);
                if (!string.IsNullOrEmpty(filtro.docNumOF))
                    sql += string.Format(@" and t1.""DocNum""={0}", filtro.docNumOF);
                if (!string.IsNullOrEmpty(filtro.articulo))
                    sql += string.Format(@" and upper(t1.""Articulo"") like '%{0}%'", filtro.articulo.ToUpper());
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        var componentes = GetComponetesById(bc.GetInt(dr["Id"]));
                        if (componentes.Count > 0)
                        {
                            ms.Add(new ST_OF_LiberadasMsg
                            {
                                Id = bc.GetInt(dr["Id"]),
                                NumST = bc.GetInt(dr["NumST"]),
                                NumOF = bc.GetInt(dr["NumOF"]),
                                FechaInicioOf = dr["FechaInicioOf"].ToString(),
                                Articulo = dr["Articulo"].ToString(),
                                CantidadPlanificada = bc.GetDecimal(dr["CantidadPlanificada"]),
                                UnidadMedida = dr["UnidadMedida"].ToString(),
                                BodegaOrigen = dr["BodegaOrigen"].ToString(),
                                BodegaDestino = dr["BodegaDestino"].ToString(),
                                Componentes = componentes
                            });
                        }
                    }
                }
                return new { sts=ms };
            }
            catch (Exception e) {
                return new
                {
                    error = e.Message
                };
            }
        }
        public static List<ST_ComponentesMsg> GetComponetesById(int id)
        {
            var ms = new List<ST_ComponentesMsg>();
            var sql = string.Format(@"
               select 
                t0.""CodArticulo"",
                t0.""Articulo"",
                t0.""CantidadAbierta"" ""Cantidad"", --puede ser que ya se hicieron transferencias parciales de esta ST
                t1.""UnidadMedida"",
                t0.""BodegaDestino"",
                t0.""LineNum""
               from
                ""JbpVw_SolicitudTrasladoLinea"" t0 inner join
                ""JbpVw_Articulos"" t1 on t1.""CodArticulo"" = t0.""CodArticulo"" left outer join
                JB_LOTES_PESAJE t2 on t2.ID_ST=t0.""IdSolicitudTraslado"" 
  	            and t2.COD_ARTICULO=t0.""CodArticulo""
  	            and t2.CANTIDAD>t0.""CantidadAbierta""
               where
                t0.""IdSolicitudTraslado"" = {0}
                and t0.""LineStatus""='O' --abierto
                and t2.LOTE is null --solo las lineas que no tienen lotes transferidos a PSJ
            ", id);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new ST_ComponentesMsg
                    {
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        Cantidad = bc.GetDecimal(dr["Cantidad"],4),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                        BodegaDestino = dr["BodegaDestino"].ToString(),
                        LineNum = bc.GetInt(dr["LineNum"]),
                    });
                }
            }
            return ms;
        }
        public static List<ST_ComponentesMsg> GetComponetesConLotesById(int id)
        {
            var ms = new List<ST_ComponentesMsg>();
            var sql = string.Format(@"
                 select 
                    t0.""CodArticulo"",
                    t0.""Articulo"",
                    t2.""Lote"",
                    t0.""CantidadAbierta"" ""Cantidad"",
                    t2.""Cantidad"" ""CantidadReservada"",
                    t1.""UnidadMedida"",
                    t3.""Id"" ""IdLote"",
                    t0.""BodegaOrigen"",
                    t0.""BodegaDestino"",
                    t0.""LineNum""
                from
                    ""JbpVw_SolicitudTrasladoLinea"" t0 inner join
                    ""JbpVw_Articulos"" t1 on t1.""CodArticulo"" = t0.""CodArticulo"" inner join
                    ""JbpVw_OperacionesLote"" t2 on
                        t2.""CodArticulo"" = t1.""CodArticulo"" and
                        t2.""IdDocBase"" = t0.""IdSolicitudTraslado"" left outer join
                    ""JbpVw_Lotes"" t3 on t3.""Lote""=t2.""Lote"" and t3.""CodArticulo""=t2.""CodArticulo"" left outer join
                    JB_LOTES_PESAJE t6 on t6.ID_ST=t0.""IdSolicitudTraslado"" and t6.LOTE=t3.""Lote"" and t6.COD_ARTICULO=t0.""CodArticulo""
                where
                    t0.""IdSolicitudTraslado"" = {0}
                    and t0.""LineStatus""='O' --abierto
                    and t2.""DireccionTexto"" = 'Asignada'
                    and t2.""Cantidad"">0
                    and t6.LOTE IS NULL --lotes que no han sido transferidos a pesaje

            ", id);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var componente = new ST_ComponentesMsg
                    {
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        Lote = dr["Lote"].ToString(),
                        Cantidad = bc.GetDecimal(dr["Cantidad"],4),
                        CantidadReservada = bc.GetDecimal(dr["CantidadReservada"], 4),
                        UnidadMedida = dr["UnidadMedida"].ToString(),
                        BodegaOrigen = dr["BodegaOrigen"].ToString(),
                        BodegaDestino = dr["BodegaDestino"].ToString(),
                        LineNum = bc.GetInt(dr["LineNum"]),
                    };
                    componente.Ubicaciones = GetUbicacionesPorLote(bc.GetInt(dr["IdLote"]), componente.BodegaOrigen);
                    if(componente.Ubicaciones!=null && componente.Ubicaciones.Count>0 && componente.Ubicaciones[0].Cantidad>0)
                        ms.Add(componente);
                }
            }
            return ms;
        }

        private static List<UbicacionLoteMsg> GetUbicacionesPorLote(int idLote, string bodegaOrigen)
        {
            var ms = new List<UbicacionLoteMsg>();
            var sql = string.Format(@"
               select 
                 t5.""Ubicacion"",
                 t4.""Cantidad""
                from
                 ""JbpVw_Lotes"" t3  left outer join
                 ""JbpVw_UbicacionPorLote"" t4 on t4.""IdLote""=t3.""Id"" left outer join
                 ""JbpVw_Ubicaciones"" t5 on t5.""Id""=t4.""IdUbicacion""
                where
                 t3.""Id"" = {0}
                 and t5.""Ubicacion"" like '{1}%'
            ", idLote, bodegaOrigen);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new UbicacionLoteMsg{
                        Ubicacion = dr["Ubicacion"].ToString(),
                        Cantidad = bc.GetDecimal(dr["Cantidad"],4),
                    });
                }
            }
            return ms;
        }

        public static List<ST_ComponentesDetalleMsg> GetDetalleComponetesById(int id)
        {
            var ms = new List<ST_ComponentesDetalleMsg>();
            var sql = string.Format(@"
               select 
                 t0.""CodArticulo"",
                 t0.""Articulo"",
                 t0.""Cantidad"" ""CantidadTotal"",
                 t1.""Lote"",
                 t1.""Cantidad"" ""CantidadLote"",
                 t1.""Bodega"",
                 t4.""Ubicacion""
                from
                 ""JbpVw_SolicitudTrasladoLinea"" t0 left outer join
                 ""JbpVw_OperacionesLote"" t1 on t1.""IdDocBase"" = t0.""IdSolicitudTraslado""

                     and t1.""CodArticulo"" = t0.""CodArticulo""

                     and t1.""NumLineaDocBase"" = t0.""LineNum"" left outer join
                 ""JbpVw_Lotes"" t2 on t2.""Lote"" = t1.""Lote"" and t2.""CodArticulo"" = t1.""CodArticulo"" left outer join
                 ""JbpVw_UbicacionPorLote"" t3 on t3.""IdLote"" = t2.""Id"" and t3.""CodBodega"" = t1.""Bodega"" left outer join
                 ""JbpVw_Ubicaciones"" t4 on t4.""Id"" = t3.""IdUbicacion""
                where
                 t0.""IdSolicitudTraslado"" = {0}
                 and t1.""DireccionTexto"" = 'Asignada'
            ", id);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new ST_ComponentesDetalleMsg
                    {
                        CodArticulo = dr["CodArticulo"].ToString(),
                        Articulo = dr["Articulo"].ToString(),
                        Cantidad = bc.GetDecimal(dr["CantidadTotal"]),
                        Lote = dr["Lote"].ToString(),
                        CantidadLote = bc.GetDecimal(dr["CantidadLote"]),
                        Bodega = dr["Bodega"].ToString(),
                        Ubicacion = dr["Ubicacion"].ToString(),
                    });
                }
            }
            return ms;
        }
    }
}
