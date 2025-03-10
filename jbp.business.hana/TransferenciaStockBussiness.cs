using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.core.sapDiApi;
using System.Threading;
using TechTools.Core.Hana;
using System.Data;
using jbp.msg.sap;
using jbp.msg;

namespace jbp.business.hana
{
    public class TransferenciaStockBussiness
    {
        public static readonly object control = new object();
        public static SapTransferenciaStock sapTransferenciaStock = new SapTransferenciaStock();

        #region Desde solicitud de transferencia
        public static DocSapInsertadoMsg SaveFromST(TsFromPickingME me)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessTSFromST(me);
                if (string.IsNullOrEmpty(ms.Error))
                {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                    UpdateResponsableTS(me.Responsable, ms.Id);
                }
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        public static List<LotesCuarMS> GetLotesCuarentena() { 
            var ms = new List<LotesCuarMS>();
            var sql = @"
                select 
                 t0.""CodArticulo"",
                 t1.""Lote"",
                 t1.""Estado"",
                 t0.""CodBodega"",
                 round(t0.""Cantidad"",4) ""Cantidad""
                from
                 ""JbpVw_CantidadesPorLote"" t0 inner join
                 ""JbpVw_Lotes"" t1 on t1.""Id"" = t0.""IdLote"" and t0.""CodArticulo"" = t1.""CodArticulo""
                where
                  t0.""CodBodega"" like 'CUAR%'
                  and t0.""Cantidad"" != 0
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new LotesCuarMS {
                    CodArticulo = dr["CodArticulo"].ToString(),
                    Lote = dr["Lote"].ToString(),
                    Estado = dr["Estado"].ToString(),
                    CodBodega = dr["CodBodega"].ToString(),
                    Cantidad = dr["Cantidad"].ToString()
                });
            }
            return ms;
        }
        //sistema balanzas espinoza paez
        public static DocSapInsertadoMsg TransferFromBalanzas(TsBalanzasMsg me)
        {
            var ms = new DocSapInsertadoMsg();
            try
            {
                if (me.DocNumOF > 0)
                {
                    ms = TS_ConLotes(me);
                    if (string.IsNullOrEmpty(ms.Error))
                    {
                        UpdateResponsableTS("Sistema Balanzas Espinoza Paez", ms.Id);
                        CheckAllInsumosPesados(me.DocNumOF); //verifica si todos los componentes fueron pesados
                    }
                }
                else
                    throw new Exception("No se ha pasado como parámetro en número de orden de fabricación 'DocNumOF'");
                
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
                
                
            }
            if (!string.IsNullOrEmpty(ms.Error) && ms.Error.Contains("(-5002)"))
                ms.Error += " La materia prima a fraccionarse no ha sido movida en SAP a la bodega de Pesaje!!";
            return ms;
        }

        private static void CheckAllInsumosPesados(int docNumOF)
        {
            var sql = string.Format(@"
                select 
                 count(*)
                from 
                 ""JbpVw_OrdenFabricacionLinea"" t0 inner join
                 ""JbpVw_OrdenFabricacion"" t1 on t1.""Id""=t0.""IdOrdenFabricacion"" inner join
                 ""JbpVw_Insumos"" t2 on t2.""CodInsumo""=t0.""CodInsumo"" 
                where
                 t2.""TipoInsumo""='Artículo'
                 and t2.""CodInsumo"" not in(
                  '11000238' --Agua purificada
                 )
                 and t0.""CantidadPesada"" < t0.""CantidadPlanificada""
                 and t1.""DocNum""={0}
            ", docNumOF);
            var insumosPorPesar = new BaseCore().GetIntScalarByQuery(sql);
            if (insumosPorPesar == 0) { // se fraccionaron todos los insumos
                SetOFPesada(docNumOF);
            }
        }
        private static void SetOFPesada(int docNumOF)
        {
            var sql = string.Format(@"
                update OWOR
                set ""U_JbFraccionadoPesaje""='SI'
                where ""DocNum""={0}
            ", docNumOF);
            new BaseCore().Execute(sql);
        }

        public static DocSapInsertadoMsg TS_ConLotes(TsBalanzasMsg me)
        {

            Monitor.Enter(control);
            var ms = new DocSapInsertadoMsg();
            try
            {
                if (me != null)
                {
                    if (sapTransferenciaStock == null)
                        sapTransferenciaStock = new SapTransferenciaStock();
                    if (!sapTransferenciaStock.IsConected())
                        sapTransferenciaStock.Connect();//se conecta a sap
                    SetCantidadPesadaByTS(me);
                    ms = sapTransferenciaStock.TransferirSinUbicaciones(me);
                }
                if (string.IsNullOrEmpty(ms.Error))
                {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                }else
                    SetCantidadPesadaByTS(me, true);

            }
            catch (Exception e)
            {
                ms.Error = e.Message;

            }
            Monitor.Exit(control);
            return ms;
        }

        private static void SetCantidadPesadaByTS(TsBalanzasMsg me, bool rollback=false)
        {
            var idOf = OrdenFabricacionBusiness.GetIdByDocNum(me.DocNumOF);
            me.Lineas.ForEach(line =>
            {
                double cantidadPesada = 0;
                line.Lotes.ForEach(lote =>
                {
                    cantidadPesada += lote.Cantidad;
                });
                var cp = new CantPesadaComponenteOF
                {
                    CantPesada = cantidadPesada,
                    IdOf = idOf,
                    CodArticulo = line.CodArticulo
                };
                BodegaBusiness.SetCantPesadaComponenteOF(cp, rollback);
            });
        }

        private static DocSapInsertadoMsg ProcessTSFromST(TsFromPickingME me, int numIntentos=0) { 
            if (numIntentos > 3)
                throw new Exception("Se ha tratado de procesar esta transferencia por 3 veces y no se ha podido establecer conexión con SAP!!");
        
            /*
             
             */
            var ms= new DocSapInsertadoMsg();
            try
            {
                ConectarASap();
                var esStDePesaje = false;
                me.Componentes.ForEach(c => {
                    /*
                     Cuando se transfiere a pesaje se pasa el bulto completo no solo la cantidad reservada por la OF
                        - Puede ser que del mismo bulto ya estén reservadas cantidades para otras OFs en otras STs
                        - Se transtiere la totalidad requerida menos las reservas de las otras ST
                        - Luego se llama a esta misma función para transferir el saldo de las otras STs
                    
                    Para que se mantengan las reservas de lotes se generan nuevas STs de PSJ->PROD
                     */
                    if (c.cantidadEnviada != c.CantidadReservada) {
                        if (c.BodegaDestino.ToUpper().Contains("PSJ"))
                        {
                            esStDePesaje = true;
                            c.cantidadesReservadasPorLote = GetCantidadesReservadasPorLote(c.CodArticulo, c.Lote);
                            if (c.cantidadesReservadasPorLote.Count > 0)
                            {
                                c.cantidadesReservadasPorLote.ForEach(crl => {
                                    if (crl.DocNumST != me.NumST)
                                    {
                                        c.cantidadEnviada -= crl.Cantidad; //xq solo se puede mover esta cantidad desde la ST original
                                        var otraST = GetOtraST(crl, me, c);
                                        ProcessTSFromST(otraST);
                                    }
                                });
                            }
                            
                        }
                    }
                    c.IdUbicacion = BodegaBusiness.GetIdUbicacionByName(c.ubicacionSeleccionada);
                });
                ms= sapTransferenciaStock.AddFromSt(me);
                /*
                 Hasta que se haga el fraccionamiento necesitamos volver a reservar los lotes creando otras ST para reemplazar 
                 las que eran de MAT->PROD por PSJ->PROD
                 */
                if (esStDePesaje)
                    ReservarLotesComponentes(me);
            }
            catch (Exception e)
            {
                if (e.Message == "You are not connected to a company" || e.Message.Contains("RPC_E_SERVERFAULT"))
                {
                    //me vuelvo a conectar y reproceso
                    sapTransferenciaStock = null;
                    numIntentos++;
                    ProcessTSFromST(me, numIntentos);
                }else
                    ms.Error=e.Message;
            }
            return ms;
        }

        private static void ReservarLotesComponentes(TsFromPickingME me)
        {
            throw new NotImplementedException();
        }

        private static TsFromPickingME GetOtraST(CantidadesReservadasPorLoteMsg crl, TsFromPickingME stOriginal, ComponenteMsg componenteOriginal)
        {
            var ms = new TsFromPickingME();
            var datosComponente = GetDatosComponenteFromDocNumST(crl.DocNumST, componenteOriginal.CodArticulo);
            ms.Id=datosComponente.Id;
            ms.NumST = crl.DocNumST;
            ms.Responsable = stOriginal.Responsable;
            ms.BodegaOrigen=stOriginal.BodegaOrigen;
            ms.BodegaDestino= componenteOriginal.BodegaDestino;
            ms.Componentes.Add(new ComponenteMsg { 
                CodArticulo = componenteOriginal.CodArticulo,
                Cantidad = crl.Cantidad,
                cantidadEnviada = crl.Cantidad,
                CantidadReservada = crl.Cantidad,
                BodegaOrigen = componenteOriginal.BodegaOrigen,
                BodegaDestino = componenteOriginal.BodegaDestino,
                ubicacionSeleccionada = componenteOriginal.ubicacionSeleccionada,
                Lote = componenteOriginal.loteSeleccionado,
                LineNum = datosComponente.LineNum,
            });
            return ms;
            
        }

        private static DatosComponenteMsg GetDatosComponenteFromDocNumST(int docNum, string codArticulo)
        {
            var ms= new DatosComponenteMsg();
            var sql = string.Format(@"
                select
                 t2.""IdSolicitudTraslado"",
                 t2.""LineNum""
                from
                 ""JbpVw_SolicitudTraslado"" t1 inner join
                 ""JbpVw_SolicitudTrasladoLinea"" t2 on t2.""IdSolicitudTraslado""=t1.""Id""
                where
                 t1.""DocNum""={0}
                 and t2.""CodArticulo""='{1}'
            ", docNum, codArticulo);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Id = bc.GetInt(dr["IdSolicitudTraslado"]);
                ms.LineNum = bc.GetInt(dr["LineNum"]);
            }
            return ms;
        }

        private static List<CantidadesReservadasPorLoteMsg> GetCantidadesReservadasPorLote(string codArticulo, string lote)
        {
            var ms= new List<CantidadesReservadasPorLoteMsg>();
            var sql = string.Format(@"
                select 
                  t1.""DocNum"",
                  t1.""DocNumOrdenFabricacion"",  
                  t3.""Cantidad""
                 from 
                  ""JbpVw_SolicitudTraslado"" t1 inner join
                  ""JbpVw_SolicitudTrasladoLinea"" t2 on t2.""IdSolicitudTraslado""=t1.""Id"" inner join
                  ""JbpVw_OperacionesLote"" t3 on t3.""IdDocBase""=t1.""Id"" 
   	                and t3.""IdTipoDocumento""=t1.""TipoObjeto""
 	                and t3.""CodArticulo""=t2.""CodArticulo""
 	                and t3.""DireccionTexto""='Asignada'
                 where
                  t3.""Lote""='{0}'
                  and t3.""CodArticulo""='{1}'
                  and t2.""LineStatus""='O'
            ",lote, codArticulo);
            var bc = new BaseCore();
            var dt= bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new CantidadesReservadasPorLoteMsg
                { 
                    DocNumST= bc.GetInt(dr["DocNum"]),
                    DocnNumOF= bc.GetInt(dr["DocNumOrdenFabricacion"]),
                    Cantidad = bc.GetDouble(dr["Cantidad"])
                });
            }
            return ms;
        }

        private static void ConectarASap()
        {
            if (sapTransferenciaStock == null)
                sapTransferenciaStock = new SapTransferenciaStock();

            if (!sapTransferenciaStock.IsConected())
            {
                if (!sapTransferenciaStock.Connect()) // cuando no se puede conectar es por que el obj sap se inhibe
                {
                    sapTransferenciaStock = null;
                    sapTransferenciaStock = new SapTransferenciaStock(); //se reinicia el objeto para hacer otro intento de conexión
                    if (!sapTransferenciaStock.Connect())
                    {
                        sapTransferenciaStock = null;
                        throw new Exception("Alta concurrencia: Vuelva a intentar la sincronización en 1 minuto");
                    }
                }
            }
        }


        private static int GetIdSTFromDocNumOF(int docNum)
        {
            var sql = string.Format(@"
                select ""Id"" from ""vw_STConDocNumOF""
                where ""DocNumOF"" = '{0}'
            ");
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt.Rows.Count > 1)
                throw new Exception("La Orden de fabriación " + docNum + " tiene mas de una Solicitud de transferencia activa!!");
            if (dt.Rows.Count == 1)
                return bc.GetInt(dt.Rows[0]["Id"]);
            else
                return 0;
        }
        #endregion

        // app bodega
        public static DocSapInsertadoMsg TransferToUbicaciones(TsBodegaMsg me)
        {
            Monitor.Enter(control);
            try
            {
                me.Lote = QuitarCodArticuloDelLote(me.Lote);
                string error = null;
                me.movimientos.ForEach(movimiento => {
                    if (error == null) {
                        if (movimiento.UbicacionDesde == movimiento.UbicacionHasta)
                        {
                            error = "Error: La ubicación destino no puede ser igual a la de origen (" + movimiento.UbicacionDesde + ")";
                        }
                        if (error == null)
                        {
                            if (!string.IsNullOrEmpty(movimiento.UbicacionDesde))
                            { //si se envia la ubicación destino explicitamente
                                movimiento.IdUbicacionDesde = BodegaBusiness.GetIdUbicacionByName(movimiento.UbicacionDesde);
                            }
                            if (!string.IsNullOrEmpty(movimiento.UbicacionHasta))
                            { //si se envia la ubicación destino explicitamente
                                movimiento.IdUbicacionHasta = BodegaBusiness.GetIdUbicacionByName(movimiento.UbicacionHasta);
                            }
                        }
                    }
                });
                if (error != null) {
                    return new DocSapInsertadoMsg
                    {
                        Error = error
                    };
                }
                var estadoLote = getEstadoLote(me);
                /*
                 solo se permiten hacer transferencias de stock en lotes liberados
                 Por esto para que desde bodega puedan mover los artículos temporalmente 
                 se cambia el estado del lote a liberado, luego se vuelve a poner el estado
                 anterior del lote
                */
                if (estadoLote != Convert.ToInt32(eEstadoLote.Liberado).ToString())
                    CambiarEstadoLote(eEstadoLote.Liberado, me);
                var ms = ProcessTS(me);
                if (estadoLote != Convert.ToInt32(eEstadoLote.Liberado).ToString())
                    CambiarEstadoLote(estadoLote, me);//vuelte al estado anterior
                if (string.IsNullOrEmpty(ms.Error))
                {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                    UpdateResponsableTS(me.Responsable, ms.Id);
                }
                return ms;
            }
            catch (Exception ex) {
                return new DocSapInsertadoMsg
                {
                    Error = ex.Message
                };
            }
            finally
            {
                Monitor.Exit(control);
            }
        }

        private static string QuitarCodArticuloDelLote(string lote)
        {
            //JB-230317151244&codArticulo=10500001
            if (!string.IsNullOrEmpty(lote) && lote.Contains("&")) { 
                var matrix = lote.Split('&');
                if (matrix.Count() == 2) { 
                    return matrix[0];
                }
            }
            return lote;
        }

        private static string getEstadoLote(TsBodegaMsg me)
        {
            var sql = string.Format(@"
             select ""Status""
             from OBTN
             where 
              ""DistNumber""='{0}'
              and ""ItemCode""='{1}'
            ", me.Lote, me.CodArticulo);
            var ms=new BaseCore().GetScalarByQuery(sql);
            return ms;
        }
        private static void CambiarEstadoLote(eEstadoLote estado, TsBodegaMsg me) {
            CambiarEstadoLote(Convert.ToInt32(estado).ToString(), me);
        }
        private static void CambiarEstadoLote(string estado, TsBodegaMsg me)
        {
            var sql = string.Format(@"
             update OBTN
              set ""Status""={0}
             where 
              ""DistNumber""='{1}'
              and ""ItemCode""='{2}'
            ", estado, me.Lote, me.CodArticulo);
            new BaseCore().Execute(sql);
        }
        private static void UpdateResponsableTS(string responsable, string id)
        {
            var sql = string.Format(@"
                update OWTR
                set ""Comments""='** Responsable Ingreso: {0} **   ' ||  ""Comments""
                where ""DocEntry"" = {1}
            ", responsable, id);
            new BaseCore().Execute(sql);
        }

        private static int GetDocNumBYId(string id)
        {
            var sql = string.Format(@"
            select
             ""DocNum""
            from
             ""JbpVw_TransferenciaStock""
            where
             ""Id"" = {0}
            ", id);
            return new BaseCore().GetIntScalarByQuery(sql);
        }

        /*private static void SetLotesFEFO(TsBodegaLineaMsg line, string CodBodegaDesde)
        {
            
            var cantlotes = GetLotesByCodArt_CodBodega(line.CodArticulo, CodBodegaDesde);
            double asignado = 0;
            double porAsignar = line.Cantidad;
            cantlotes.ForEach(cl =>
            {
                if (porAsignar > 0) {
                    if (cl.Disponible <= porAsignar){
                        asignado = cl.Disponible;
                        porAsignar -= cl.Disponible;
                    }
                    else{
                        asignado = porAsignar;
                        porAsignar = 0;
                    }
                    line.Lotes.Add(new AsignacionLoteMsg{
                        Lote = cl.Lote,
                        Cantidad = asignado
                    });
                }
                    
            });
            //si no hay suficiente cantidad en lotes para transferir se lanza una excepcion
            if (porAsignar > 0) {
                var err = string.Format("No existe cantidad en lotes suficiente para transferir {0} unidades del artículo {1}", line.Cantidad, line.CodArticulo);
                if (cantlotes.Count > 0) {
                    err += "Información de lotes encontrados: ";
                    cantlotes.ForEach(cl => {
                        err += string.Format("Lote: {0}, Cantidad: {1}, Bodega: {2} ",cl.Lote, cl.Disponible, cl.CodBodega);
                    });
                }
                throw new Exception(err);
            }
            
        }*/

        private static List<CantidadLoteMsg> GetLotesByCodArt_CodBodega(string codArticulo, string codBodegaDesde)
        {
            var ms = new List<CantidadLoteMsg>();
            var sql = string.Format(@"
                select
                 ""Lote"",
                 ""Disponible"",
                 ""CodAlmacen""
                from ""JbpVw_CantDisponibleArticuloPorLotes""
                where
                 ""CodArticulo"" = '{0}'
                 and ""CodAlmacen"" = '{1}'
                 and  ""Disponible"" > 0
                 and ""FechaVencimiento"">=current_Date --lotes que no estén vencidos
                order by
                 ""FechaVencimiento"" -- Desde lo que esta mas próximo a vencerse
            ", codArticulo, codBodegaDesde);
            var bc = new BaseCore();
            var dt=bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new CantidadLoteMsg
                {
                    Lote = dr["Lote"].ToString(),
                    Disponible = bc.GetDouble(dr["Disponible"]),
                    CodBodega =dr["CodAlmacen"].ToString()
                });
            }
            return ms;
        }

        private static DocSapInsertadoMsg ProcessTS(TsBodegaMsg me)
        {
            try
            {
                if (me != null)
                {
                    if (sapTransferenciaStock == null)
                        sapTransferenciaStock = new SapTransferenciaStock();
                    if (!sapTransferenciaStock.IsConected())
                        sapTransferenciaStock.Connect();//se conecta a sap
                    return sapTransferenciaStock.TranferirEntreUbicaciones(me);
                }
                return null;
            }
            catch (Exception e)
            {
                return new DocSapInsertadoMsg { 
                    Error = e.Message
                };
            }
        }
    }
}
