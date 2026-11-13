using jbp.core.sapDiApi;
using jbp.msg;
using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TechTools.Core.Hana;

namespace jbp.business.hana
{
    public class EntradaMercanciaBussiness
    {
        public static readonly object control = new object();
        public static SapEntradaMercancia sapEntradaMercancia = new SapEntradaMercancia();

        public static EntradaMercanciaMsg Ingresar(EntradaMercanciaMsg me, bool porCompra)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessEM(me, porCompra);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }

        private static EntradaMercanciaMsg ProcessEM(EntradaMercanciaMsg me, bool porCompra)
        {
            try
            {
                if (me != null)
                {
                    
                    if (sapEntradaMercancia == null)
                        sapEntradaMercancia = new SapEntradaMercancia();
                    if (!sapEntradaMercancia.IsConected())
                        sapEntradaMercancia.Connect();//se conecta a sap
                    //desde la app no se controla el envío de lineas sin asignacion lote
                    me.Lineas = me.Lineas.FindAll(l => l.AsignacionesLote.Count > 0);
                    if (me.Lineas == null)
                        throw new Exception("SRV: Ninguna línea de la entrada de mergancía tiene lotes asignados");
                    EntradaMercanciaMsg.SetCodBodegaEnLineas(me.Lineas); //se asigna el código de bodega a las líneas de la entrada de mercancia
                    SetNewLotes(me); //se asigna los lotes nuevos desde la bdd
                    me.Lineas.ForEach(line => {
                        line.AsignacionesLote.ForEach(al => {
                            if (!string.IsNullOrEmpty(al.Ubicacion)) { 
                                al.IdUbicacion= BodegaBusiness.GetIdUbicacionByName(al.Ubicacion);
                            }
                        });
                    });
                    SetGastosAdicionales(ref me);
                    var ms=(porCompra)? sapEntradaMercancia.AddPorCompra(me): sapEntradaMercancia.Add(me);
                    if (string.IsNullOrEmpty(ms.Error)) { //si no hay error
                        ms.DocNumEntradaMercancia = GetDocNumById(ms.IdEM);
                        SetComentarioAndUpdateResponsableEM(me, ms.IdEM);
                        ms.Lineas.ForEach(l => {
                            l.AsignacionesLote.ForEach(al => {
                                ActualizarEstadoLoteCuarentena(al.Lote);
                            });
                            //registro la unidad de medida del articulo
                            me.Lineas.ForEach(lme => {
                                if (lme.CodArticulo == l.CodArticulo) {
                                    l.UnidadMedida = lme.UnidadMedida;
                                }

                            });
                        });
                        //InsertResumenEM(ms);
                    }
                    return ms;
                }
                return null;
            }
            catch (Exception e)
            {
                return new EntradaMercanciaMsg { Error = e.Message };
            }
        }

        

        private static void SetGastosAdicionales(ref EntradaMercanciaMsg me)
        {
            if (me.tipo == "Factura de Reserva") {
                var sql = string.Format(@"
                   select 
                    ""ObjType"",
                    ""DocEntry"",
                    ""LineNum""
                     from PCH3 where ""DocEntry"" = ?
                ");
                var dt = new BaseCore().GetDataTableByQuery(sql, new Dictionary<string, object> {
                    {"@0", me.IdDocOrigen }
                });
                foreach (DataRow dr in dt.Rows) {
                    me.GastosAdicionales.Add(new GastosAdicionalesMsg { 
                        ObjType = dr["ObjType"].ToString(),
                        DocEntry = dr["DocEntry"].ToString(),
                        LineNum = dr["LineNum"].ToString()
                    });
                }
            }
        }

        private static void ActualizarEstadoLoteCuarentena(string loteJB)
        {
            /*
             En el objeto lote de la DIapi de sap no se puede gestionar el estado del lote
             por esto se lo hacer mediante una actualizacion a la base de datos

                Status 1 -> Acceso Denegado
             */
            if (!LoteEnCuarentena(loteJB))
            {
                var sql = string.Format(@"update OBTN set ""Status""=1 where ""DistNumber""=?");
                new BaseCore().Execute(sql, new Dictionary<string, object> {
                    {"@0",loteJB }
                });
            }
            
        }

        private static bool LoteEnCuarentena(string loteJB)
        {
            try
            {
                var sql = string.Format(@"
                 select 
                    ""Estado""
                 from
                    ""JbpVw_Lotes""
                 where
                    ""Lote""=?
                ");
                var estado = new BaseCore().GetScalarByQuery(sql, new Dictionary<string, object> { { "@0", loteJB } });
                return estado == "Acceso Denegado";
            }
            catch
            {
                return false;
            }
        }

        //private static void InsertResumenEM(EntradaMercanciaMsg ms)
        //{

        //}

        private static void SetComentarioAndUpdateResponsableEM(EntradaMercanciaMsg me, string idEM)
        {
            var comentarioBdd = GetComentarioEM(idEM);
            me.Comentario = String.Format("(Ingresado por: {0}) {1} {2}",
                me.responsable, comentarioBdd, me.Comentario    
            );
            // se trunca el comentario a la longitud máxima en la bdd
            if( me.Comentario.Length > 253){
                me.Comentario = me.Comentario.Substring(0, 253);
            }
            var sql = string.Format(@"
                update OPDN
                set ""Comments""=?
                where ""DocEntry"" = ?
            ");
            new BaseCore().Execute(sql, new Dictionary<string, object> {
                {"@0", me.Comentario },
                {"@1",idEM }
            });
        }

        private static string GetComentarioEM(string idEM)
        {
            var sql = string.Format(@"
                select ""Comments"" from OPDN where ""DocEntry"" = ?
            ");
            return new BaseCore().GetScalarByQuery(sql, new Dictionary<string, object> { { "@0", idEM } });
        }

        public static int GetDocNumById(string idEntradaMercancia) {
            var sql = string.Format(@"
            select
             ""DocNum""
            from
             ""JbpVw_EntradaMercancia""
            where
             ""Id"" = ?
            ");
            return new BaseCore().GetIntScalarByQuery(sql, new Dictionary<string, object> { { "@0", idEntradaMercancia } });
        }
        private static void SetNewLotes(EntradaMercanciaMsg me)
        {
            //Ej.: JB-220530142909
            // Se genera en la base de datos para garantizar que no se duplique el lote
            // (con la fecha y hora del servidor de base de datos)
            me.Lineas.ForEach(line => {
                line.AsignacionesLote.ForEach(al => {
                    if (string.IsNullOrEmpty(al.Lote))
                    {
                        var sql = "call SBO_SP_LOTES_OP('EP','');"; // se pasa como parámetro EP, para indicar a la base que es EM por compra
                        al.Lote = new BaseCore().GetScalarByQuery(sql,null);
                        System.Threading.Thread.Sleep(1000); //para que se genere un nuevo lote
                    }
                });
            });
            
        }

        public static string EntradaMercanciaAjustePesaje(){
            var bc = new BaseCore();
            string sql = "SELECT ORDEN_FAB FROM JB_PENDIENTE_AJUSTE WHERE AJUSTADO = FALSE";
            DataTable pendienteAjuste = bc.GetDataTableByQuery(sql, null);

            if (pendienteAjuste == null || pendienteAjuste.Rows.Count == 0)
                return "No hay órdenes pendientes de ajuste.";

            int totalProcesadas = 0, docNumEntrada = 0; 

            foreach (DataRow dr in pendienteAjuste.Rows){
                int docNumOF = bc.GetInt(dr["ORDEN_FAB"]);
                if (docNumOF <= 0)
                    continue;

                try{
                    DataTable tranferenciasDatos = TransferenciaStockBussiness.getTranferenciasDataTableByOF(docNumOF);
                    DocSapInsertadoMsg resp = realizarIngresosAjustePesaje(tranferenciasDatos, docNumOF);
                    docNumEntrada = resp.DocNum;

                    if (resp != null){
                        string sqlUpdate = "UPDATE JB_PENDIENTE_AJUSTE SET AJUSTADO = TRUE, UPDATED_AT = CURRENT_TIMESTAMP, DOCNUM_ENTRADA = ? WHERE ORDEN_FAB = ?";
                        bc.Execute(sqlUpdate, new Dictionary<string, object> { { "@0", resp.DocNum } , { "@1", docNumOF } });
                        totalProcesadas++;
                    }
                }
                catch (Exception ex){
                    LogErrorCorreo(docNumOF, "Error procesando ajuste OF " + docNumOF + ": " + ex.Message);
                }
            }

            return $"Proceso completado. Se procesaron {totalProcesadas} órdenes de fabricación. \nEntrada mercadería DocNum: {docNumEntrada}";
        }

        // realiza un unico ingreso de mercancia por ajuste agrupando todos los componentes pesados de la OF
        private static DocSapInsertadoMsg realizarIngresosAjustePesaje(DataTable tranferenciasDatos, int docNumOF){
            if (tranferenciasDatos == null || tranferenciasDatos.Rows.Count == 0)
                return null;

            var bc = new BaseCore();
            var ajusteMsg = new EntradaAjustePesajeMsg();

            foreach (DataRow dr in tranferenciasDatos.Rows){
                try{
                    var itemCode = dr["ItemCode"].ToString();
                    var lote = dr["Lote"].ToString();
                    var quantity = bc.GetDouble(dr["U_jb_dif_tolerancia"]);

                    if (!string.IsNullOrEmpty(itemCode) && quantity > 0){
                        var linea = ajusteMsg.DocumentLines.FirstOrDefault(l => l.ItemCode == itemCode);
                        if (linea == null){
                            linea = new EntradaAjusteLineaMsg{
                                ItemCode = itemCode
                            };
                            ajusteMsg.DocumentLines.Add(linea);
                        }

                        if (!string.IsNullOrEmpty(lote)) {
                            linea.BatchNumbers.Add(new EntradaAjusteLoteMsg{
                                BatchNumber = lote,
                                Quantity = quantity
                            });
                        }
                    }
                }
                catch (Exception exRow){
                    LogErrorCorreo(docNumOF, "Excepción al armar línea de ajuste pesaje: " + exRow.Message);
                    throw;
                }
            }

            if (ajusteMsg.DocumentLines.Count == 0)
                return null;

            DocSapInsertadoMsg respAjuste = IngresoPorAjustePesaje(ajusteMsg, docNumOF);
            if (respAjuste != null && !string.IsNullOrEmpty(respAjuste.Error)){
                LogErrorCorreo(docNumOF, "Error al ingresar ajuste pesaje OF " + docNumOF + ": " + respAjuste.Error);
                return null;
            }

            return respAjuste;
        }

        public static DocSapInsertadoMsg IngresoPorAjustePesaje(EntradaAjustePesajeMsg me, int docNumOF){
            //se necesita ejecutar una peticion por vez y q esta termine para procesar para avanzar
            Monitor.Enter(control);
            try{
                if (me == null || me.DocumentLines == null || me.DocumentLines.Count == 0)
                    return new DocSapInsertadoMsg { Error = "No existen líneas para realizar el ajuste de inventario." };

                if (sapEntradaMercancia == null)
                    sapEntradaMercancia = new SapEntradaMercancia();
                if (!sapEntradaMercancia.IsConected())
                    sapEntradaMercancia.Connect();

                var ms = sapEntradaMercancia.AddAjusteInventario(me, docNumOF);
                if (string.IsNullOrEmpty(ms.Error) && !string.IsNullOrEmpty(ms.Id))
                    ms.DocNum = GetDocNumAjusteById(ms.Id);

                return ms;
            }
            catch (Exception ex){
                return new DocSapInsertadoMsg { Error = ex.Message };
            }
            finally{
                Monitor.Exit(control);
            }
        }

        private static void LogErrorCorreo(int docNumOF, string err, string appName = "Ajuste Pesaje"){
            try{
                UserBusiness.Log(new LogMsg
                {
                    AppName = appName,
                    Obs = err,
                    UserName = ""
                });
            }
            catch { }
        }


        public static int GetDocNumAjusteById(string idEntradaAjuste)
        {
            var sql = @"select ""DocNum"" from OIGN where ""DocEntry"" = ?";
            return new BaseCore().GetIntScalarByQuery(sql, new Dictionary<string, object> { { "@0", idEntradaAjuste } });
        }
    }
}
