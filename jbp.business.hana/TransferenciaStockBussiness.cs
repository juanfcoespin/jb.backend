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
using System.Security.Policy;
using System.Runtime.CompilerServices;

namespace jbp.business.hana
{
    public class TransferenciaStockBussiness:BaseBusiness
    {
        public static readonly object control = new object();
        public static SapTransferenciaStock sapTransferenciaStock = new SapTransferenciaStock();

        
        #region Desde solicitud de transferencia
        public static DocSapInsertadoMsg SaveFromST(TsFromPickingME me)
        {
            Monitor.Enter(control);
            try
            {
                //TechTools.Utils.ObjectUtils.DeepClone
                var ms = new DocSapInsertadoMsg();
                var componentes = me.Componentes.FindAll(p => p.CantidadEnviada > 0).ToList();
                if (componentes == null || componentes.Count == 0)
                {
                    ms.Error = "No se han enviado componentes para la transferencia!!";
                    return ms;
                }
                me.Componentes.Clear();
                //por cada componente se ejecuta una transaccion
                foreach (var c in componentes)
                {
                    me.Componentes.Add(c);
                    ms = ProcessTSFromST(me, 0, false);
                    if (!string.IsNullOrEmpty(ms.Error))
                    {//si da error
                        return ms;
                    }
                    me.Componentes.Clear();
                }
                ms.DocNum = GetDocNumBYId(ms.Id); //retorna el ultimo componente
                UpdateResponsableTS(me.Responsable, ms.Id);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        public static DocSapInsertadoMsg SaveFromST_bk(TsFromPickingME me)
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
                 and lower(t2.""UnidadMedida"") in ('kg', 'g', 'l')
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
                    SetIdSTEnComponentes(me);
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

        private static void SetIdSTEnComponentes(TsBalanzasMsg me)
        {
            if (me == null || me.DocNumOF==0)
                return;
            var sql = string.Format(@"
                select 
                 ""CodInsumo"",
                 ""IdST""
                from 
                 ""JbVw_OFsConTSaPesaje""
                where
                 ""DocNum""={0}
            ", me.DocNumOF);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            me.Lineas.ForEach(linea => {
                linea.IdSt = GetIdStFromCodArticulo(linea.CodArticulo, dt);
            });
        }

        private static int GetIdStFromCodArticulo(string codArticulo, DataTable dt)
        {
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["CodInsumo"].ToString() == codArticulo)
                    return DBNull.Value.Equals(dr["IdST"])?0: Convert.ToInt32(dr["IdST"]);
                
            }
            return 0;
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

        public static DocSapInsertadoMsg TransferFromPesajeToMat(TsFromPesajeToMatMsg me)
        {
            Monitor.Enter(control);
            try
            {
                var newMe = MapFromPesajeToMat(me);
                return ProcessTSFromST(newMe);
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        private static TsFromPickingME MapFromPesajeToMat(TsFromPesajeToMatMsg me)
        {
            var ms = new TsFromPickingME();
            ms.ClientId = me.ClientId;
            ms.Responsable = me.Responsable;
            ms.Componentes = new List<ComponenteMsg>();
            var componente = new ComponenteMsg
            {
                BodegaOrigen = me.detalleLote.CodBodega,
                BodegaDestino = me.codBodegaDestino,
                CantidadEnviada = me.detalleLote.Cantidad,
                CodArticulo = me.detalleLote.CodArticulo

            };
            componente.Lotes = new List<LoteComponenteMsg>();
            componente.Lotes.Add(new LoteComponenteMsg { 
                CantidadEnviada=componente.CantidadEnviada,
                CantidadReservada = componente.CantidadEnviada,
                Lote = me.detalleLote.Lote,
            });
            ms.Componentes.Add(componente);
            ms.BodegaOrigen = componente.BodegaOrigen;
            ms.BodegaDestino = componente.BodegaDestino;

            return ms;
        }
        private static DocSapInsertadoMsg ProcessTSFromST(TsFromPickingME me, int numIntentos = 0, bool esHijo = false)
        {
            /*
             esHijo = true -> se llama a esta función de manera recursiva cuando se encuentran otras reservas
                              del mismo componente
             */
            var msg = "";
            if (numIntentos > 3)
            {
                msg = "Se ha superado el número de intentos para conectar a SAP";
                SendMessageToClient(me.ClientId, msg, eMessageType.Error);
                throw new Exception(msg);
            }
            var ms = new DocSapInsertadoMsg();
            try
            {
                ConectarASap(me.ClientId);
                if (!esHijo)
                {
                    sapTransferenciaStock.StartTransaction();
                }
                var esStDePesaje = false;
                //por cada lote se envía un registro de componente
                me.Componentes.ForEach(c => {
                    if (!esHijo)
                    {
                        msg = string.Format("Iniciando Transacción componente {0} de la OF {1}", c.CodArticulo, me.NumOF);
                        SendMessageToClient(me.ClientId, msg, eMessageType.Success);
                    }
                    /*
                     Cuando se transfiere a pesaje se pasa el bulto completo no solo la cantidad reservada por la OF
                        - Puede ser que del mismo bulto ya estén reservadas cantidades para otras OFs en otras STs
                        - Se transfiere la totalidad requerida menos las reservas de las otras ST
                        - Luego se llama a esta misma función para transferir el saldo de las otras STs

                    Para que se mantengan las reservas de lotes se generan nuevas STs de PSJ->PROD
                     */
                    if (c.BodegaDestino.ToUpper().Contains("PSJ"))
                        esStDePesaje = true;
                    c.Lotes.ForEach(lote => {
                        if (!esHijo)
                        {
                            var msg2 = string.Format("Procesando lote {0}, cant enviada: {1}; cant. Reservada: {2}", lote.Lote, lote.CantidadEnviada, lote.CantidadReservada);
                            SendMessageToClient(me.ClientId, msg2, eMessageType.Warning);
                        }
                        lote.Ubicaciones.ForEach(u => {
                            u.IdUbicacion = BodegaBusiness.GetIdUbicacionByName(u.Ubicacion);
                        });
                        if (c.CantidadEnviada > c.CantidadRequerida)
                        {
                            procesarReservasOtrasOF(me, c, lote);
                        }
                    });
                });
                me.Comentario = "ApiPesaje (Responsable: " + me.Responsable + " )";
                if(!string.IsNullOrEmpty(me.NumOF))
                    msg = string.Format("Creando TS para OF:{0} de {1}->{2}", me.NumOF, me.BodegaOrigen, me.BodegaDestino);
                else
                    msg = string.Format("Creando TS de {0}->{1}", me.BodegaOrigen, me.BodegaDestino);
                SendMessageToClient(me.ClientId, msg);
                ms = sapTransferenciaStock.AddFromSt(me);
                if (ms != null && !string.IsNullOrEmpty(ms.Error))
                {
                    throw new Exception(ms.Error);
                }
                else
                {
                    msg = string.Format("Se creo la TS con ID: {0}", ms.Id);
                    SendMessageToClient(me.ClientId, msg);
                }
                /*
                 Hasta que se haga el fraccionamiento necesitamos volver a reservar los lotes creando otras ST para reemplazar 
                 las que eran de MAT->PROD por PSJ->PROD
                 */
                if (esStDePesaje || esHijo)
                {
                    //sapTransferenciaStock se pasa como transferencia para la transaccion y para no hacer dos conexiones a la bdd
                    ReservarLotesComponentes(me, sapTransferenciaStock);
                }
                if (!esHijo)
                { //solo la transaccion inicial hace el commit
                    SendMessageToClient(me.ClientId, "Haciendo Commit (Finalizando transacción toma aprox 60seg por componente x favor espere)...", eMessageType.Warning);
                    sapTransferenciaStock.CommitTransaction();
                    SendMessageToClient(me.ClientId, "(Commit) Transacción Finalizada!", eMessageType.Success);
                }
            }
            catch (Exception e)
            {
                numIntentos = ManejarError(me, numIntentos, esHijo, ms, e);
            }
            return ms;
        }

        private static DocSapInsertadoMsg ProcessTSFromST_bk(TsFromPickingME me, int numIntentos = 0, bool esHijo = false)
        {
            /*
             esHijo = true -> se llama a esta función desde otra función de manera recursiva cuando se encuentran otras reservas
                              del mismo componente
             */
            var msg = "";
            if (numIntentos > 3)
            {
                msg = "Se ha superado el número de intentos para conectar a SAP";
                SendMessageToClient(me.ClientId, msg, eMessageType.Error);
                throw new Exception(msg);
            }
            var ms = new DocSapInsertadoMsg();
            try
            {
                ConectarASap(me.ClientId);
                if (!esHijo)
                {
                    SendMessageToClient(me.ClientId, "Iniciando Transacción", eMessageType.Success);
                    sapTransferenciaStock.StartTransaction();
                }

                var esStDePesaje = false;
                //por cada lote se envía un registro de componente
                me.Componentes.ForEach(c => {
                    if (!esHijo)
                    {
                        msg = string.Format("Procesando componente {0} de la OF {1}", c.CodArticulo, me.NumOF);
                        SendMessageToClient(me.ClientId, msg, eMessageType.Warning);
                    }
                    /*
                     Cuando se transfiere a pesaje se pasa el bulto completo no solo la cantidad reservada por la OF
                        - Puede ser que del mismo bulto ya estén reservadas cantidades para otras OFs en otras STs
                        - Se transfiere la totalidad requerida menos las reservas de las otras ST
                        - Luego se llama a esta misma función para transferir el saldo de las otras STs

                    Para que se mantengan las reservas de lotes se generan nuevas STs de PSJ->PROD
                     */
                    if (c.BodegaDestino.ToUpper().Contains("PSJ"))
                        esStDePesaje = true;
                    c.Lotes.ForEach(lote => {
                        if (!esHijo)
                        {
                            var msg2 = string.Format("Procesando lote {0}, cant enviada: {1}, cant. Reservada: {2}", lote.Lote, lote.CantidadEnviada, lote.CantidadReservada);
                            SendMessageToClient(me.ClientId, msg2, eMessageType.Warning);
                        }
                        lote.Ubicaciones.ForEach(u => {
                            u.IdUbicacion = BodegaBusiness.GetIdUbicacionByName(u.Ubicacion);
                        });
                        if (c.CantidadEnviada > c.CantidadRequerida)
                        {
                            procesarReservasOtrasOF(me, c, lote);
                        }
                    });
                });
                me.Comentario = "ApiPesaje (Responsable: " + me.Responsable + " )";
                msg = string.Format("Creando TS para OF:{0} de {1}->{2}", me.NumOF, me.BodegaOrigen, me.BodegaDestino);
                SendMessageToClient(me.ClientId, msg);
                ms = sapTransferenciaStock.AddFromSt(me);
                if (ms != null && !string.IsNullOrEmpty(ms.Error))
                {
                    throw new Exception(ms.Error);
                }
                else
                {
                    msg = string.Format("Se creo la TS con ID: {0}", ms.Id);
                    SendMessageToClient(me.ClientId, msg);
                }
                /*
                 Hasta que se haga el fraccionamiento necesitamos volver a reservar los lotes creando otras ST para reemplazar 
                 las que eran de MAT->PROD por PSJ->PROD
                 */
                if (esStDePesaje || esHijo)
                {
                    //sapTransferenciaStock se pasa como transferencia para la transaccion y para no hacer dos conexiones a la bdd
                    ReservarLotesComponentes(me, sapTransferenciaStock);
                }
                if (!esHijo)
                { //solo la transaccion inicial hace el commit
                    SendMessageToClient(me.ClientId, "Haciendo Commit (Finalizando transacción toma aprox 40seg por componente x favor espere)...", eMessageType.Warning);
                    sapTransferenciaStock.CommitTransaction();
                    SendMessageToClient(me.ClientId, "(Commit) Transacción Finalizada!", eMessageType.Success);
                }
            }
            catch (Exception e)
            {
                numIntentos = ManejarError(me, numIntentos, esHijo, ms, e);
            }
            return ms;
        }

        private static int ManejarError(TsFromPickingME me, int numIntentos, bool esHijo, DocSapInsertadoMsg ms, Exception e)
        {
            if (e.Message == "You are not connected to a company" || e.Message.Contains("RPC_E_SERVERFAULT") || e.Message.Contains("ODBC"))
            {
                //me vuelvo a conectar y reproceso
                sapTransferenciaStock = null;
                numIntentos++;
                SendMessageToClient(me.ClientId, "Reintentando conexión a SAP (" + numIntentos + " intento)", eMessageType.Warning);
                ProcessTSFromST(me, numIntentos);
            }
            else
            {
                if (!esHijo)
                {
                    ms.Error = e.Message;
                    SendMessageToClient(me.ClientId, "Se hizo un reverso de la trasacción por Error: " + ms.Error, eMessageType.Error);
                    try
                    {
                        sapTransferenciaStock.RollBackTransaction();
                    }
                    catch { }// da error al hacer un rollback
                }
            }

            return numIntentos;
        }

        private static void procesarReservasOtrasOF(TsFromPickingME me, ComponenteMsg c, LoteComponenteMsg lote)
        {
            SendMessageToClient(me.ClientId, "Identificando reservas de otras órdenes de Fabricación para el lote: " + lote.Lote);
            var cantidadesReservadasPorLote = GetCantidadesReservadasPorLote(me, c.CodArticulo, lote.Lote);
            if (cantidadesReservadasPorLote.Count > 0){
                
                cantidadesReservadasPorLote.ForEach(crl =>
                {
                    //Si es una OF diferente de la original
                    if (crl.DocNumOF != me.NumOF && crl.Cantidad <= Math.Round(c.CantidadEnviada-c.CantidadRequerida,4))
                    {
                        var msg = string.Format("Reserva Identificada: OF:{0}, ST:{1}, Cant:{2}", crl.DocNumOF, crl.DocNumST, crl.Cantidad);
                        SendMessageToClient(me.ClientId, msg, eMessageType.Warning);

                        var ubicacionesOtraST = QuitarCantidadOtraReserva(lote, crl.Cantidad);
                        
                        var otraST = GetOtraST(crl, c, lote, me, ubicacionesOtraST);
                        //msg = string.Format("Creando TS para OF:{0} de {1}->{2}", otraST.NumOF, otraST.BodegaOrigen, otraST.BodegaDestino);
                        //SendMessageToClient(me.ClientId, msg);
                        var ms = ProcessTSFromST(otraST, 0, true);
                        if (string.IsNullOrEmpty(ms.Error))
                        {
                            //se disminuye de la cantidad enviada la reserva
                            var cantidadEnviada = Math.Round(c.CantidadEnviada-crl.Cantidad,4);
                            c.CantidadEnviada = cantidadEnviada;
                            lote.CantidadEnviada = cantidadEnviada;
                        }
                        else
                        {
                            throw new Exception(ms.Error);
                        }
                    }
                });
            }
        }

        private static List<UbicacionCantidadMsg> QuitarCantidadOtraReserva(LoteComponenteMsg lote, double cantOtraReserva)
        {
            /*
             Este algoritmo hace una distribución de las cantidades en las ubicaciones del la OF original
             y las asigna a la OF de la otra Reserva
             */
            var ubicacionesOtraReserva = new List<UbicacionCantidadMsg>();
            var ubicacionesPorQuitarOfOriginal = new List<UbicacionCantidadMsg>();
            double cantPorAsignar = cantOtraReserva;
            //ordeno las ubicaciones de mayor a menor cantidad
            lote.Ubicaciones = lote.Ubicaciones.OrderByDescending(u => u.Cantidad).ToList();
            
            lote.Ubicaciones.ForEach(u =>{
                if (cantPorAsignar > 0) { //si la 1era ubicacion tiene mas de la cant
                    if (u.Cantidad >= cantPorAsignar)
                    {
                        //con esta ubicación se manda a psj la otra reserva
                        u.Cantidad -= cantPorAsignar;
                        ubicacionesOtraReserva.Add(new UbicacionCantidadMsg
                        {
                            IdUbicacion=u.IdUbicacion,
                            Ubicacion = u.Ubicacion,
                            Cantidad = cantPorAsignar
                        });
                        if(Math.Round(u.Cantidad,4)==0) //si se asigno toda la cantidad de la ubicación
                            ubicacionesPorQuitarOfOriginal.Add(u);
                        cantPorAsignar = 0;
                    }
                    else{
                        cantPorAsignar-=u.Cantidad; 
                        ubicacionesPorQuitarOfOriginal.Add(u);
                        ubicacionesOtraReserva.Add(u);
                    }
                }
            });
            if (ubicacionesPorQuitarOfOriginal.Count > 0) {
                ubicacionesPorQuitarOfOriginal.ForEach(uxq =>
                {
                    lote.Ubicaciones.RemoveAll(u=>u.Ubicacion==uxq.Ubicacion);
                });
            }
            return ubicacionesOtraReserva;
        }

        private static void ReservarLotesComponentes(TsFromPickingME me, SapTransferenciaStock sapTransferenciaStock)
        {
            StMsg stOFOriginal = GetCabeceraST(me);
            //Hago un barrido para las reservas de la OF original
            me.Componentes.ForEach(c =>
            {
                var linea = GetLineaST(c, stOFOriginal);
                c.Lotes.ForEach(lote =>
                {
                    linea.Lotes.Add(new LoteStMsg
                    {
                        Lote = lote.Lote,
                        Cantidad = lote.CantidadReservada
                    });
            
                    linea.Cantidad += lote.CantidadReservada;
                    var msg = string.Format("Reservando {4} lote {0} de la OF {1} de la bodega {2} a {3}",
                        lote.Lote, stOFOriginal.DocNumOF, stOFOriginal.BodegaOrigen, stOFOriginal.BodegaDestino, lote.CantidadReservada);
                    SendMessageToClient(me.ClientId, msg);
                });
                stOFOriginal.Lines.Add(linea);
            });
            
            var ms=SolicitudTransferenciaBusiness.Save(stOFOriginal,0, sapTransferenciaStock);
            if (!string.IsNullOrEmpty(ms.Error))
            {
                SendMessageToClient(me.ClientId, ms.Error, eMessageType.Error);
                throw new Exception(ms.Error);
            }else
                SendMessageToClient(me.ClientId, "Reserva completada Correctamente con Id:"+ ms.Id);

        }
        private static StMsg GetCabeceraST(TsFromPickingME me)
        {
            var stOFOriginal = new StMsg();
            stOFOriginal.BodegaOrigen = me.BodegaDestino; //PSJ
            stOFOriginal.BodegaDestino = me.BodegaProd; //PROD
            stOFOriginal.DocNumOF = me.NumOF;
            stOFOriginal.Comentarios = "Realizado desde la API - Picking Bodega (Responsable: "+me.Responsable+")";
            return stOFOriginal;
        }
        private static LineStMsg GetLineaST(ComponenteMsg c, StMsg stOFOriginal)
        {
            return new LineStMsg()
            {
                CodArticulo = c.CodArticulo,
                BodegaOrigen = stOFOriginal.BodegaOrigen,
                BodegaDestino = stOFOriginal.BodegaDestino,
                Cantidad = 0
            };
        }

        private static TsFromPickingME GetOtraST(CantidadesReservadasPorLoteMsg crl, ComponenteMsg componenteOriginal, LoteComponenteMsg loteMe, TsFromPickingME me, List<UbicacionCantidadMsg> ubicacionesOtraST)
        {
            var ms = new TsFromPickingME();
            ms.ClientId = me.ClientId;
            ms.Id=crl.IdSolicitudTraslado;
            ms.NumOF = crl.DocNumOF;
            ms.NumST = crl.DocNumST;
            ms.Responsable = me.Responsable;
            ms.BodegaOrigen= me.BodegaOrigen;
            ms.BodegaDestino= me.BodegaDestino;
            ms.BodegaProd = crl.BodegaDestino;
            var componente = new ComponenteMsg
            {
                CodArticulo = componenteOriginal.CodArticulo,
                CantidadEnviada = crl.Cantidad,
                CantidadRequerida = crl.Cantidad,
                BodegaOrigen = ms.BodegaOrigen,
                BodegaDestino = ms.BodegaDestino,
                LineNum = crl.LineNum,
            };
            var lote = new LoteComponenteMsg { 
                Lote=loteMe.Lote,
                CantidadEnviada=crl.Cantidad,
                CantidadReservada = crl.Cantidad,
                Ubicaciones=ubicacionesOtraST
            };
            
            componente.Lotes.Add(lote);
            ms.Componentes.Add(componente);
            return ms;
        }

        private static List<CantidadesReservadasPorLoteMsg> GetCantidadesReservadasPorLote(TsFromPickingME me, string codArticulo, string lote)
        {
            var ms= new List<CantidadesReservadasPorLoteMsg>();
            var sql = string.Format(@"
                select 
                  distinct
                  t1.""DocNum"",
                  t1.""DocNumOrdenFabricacion"",  
                  t3.""Cantidad"",
                  t2.""BodegaDestino"",
                  t2.""IdSolicitudTraslado"",
                  t2.""LineNum""
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
                  and t2.""BodegaOrigen""= '{2}' --otras reservas hechas por planificacion
            ", lote, codArticulo, me.BodegaOrigen);
            var bc = new BaseCore();
            var dt= bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new CantidadesReservadasPorLoteMsg
                { 
                    DocNumST= bc.GetInt(dr["DocNum"]),
                    DocNumOF= dr["DocNumOrdenFabricacion"].ToString(),
                    Cantidad = bc.GetDouble(dr["Cantidad"]),
                    Lote = lote,
                    BodegaDestino = dr["BodegaDestino"].ToString(),
                    IdSolicitudTraslado = bc.GetInt(dr["IdSolicitudTraslado"]),
                    LineNum = bc.GetInt(dr["LineNum"]),
                });
            }
            if (ms.Count > 0){
                var msg = string.Format("Se ha encontrado {0} STs de {1} a {2} en las que se han hecho reservas de este lote!", ms.Count, me.BodegaOrigen, me.BodegaDestino);
                SendMessageToClient(me.ClientId, msg, eMessageType.Warning);
            }
            return ms;
        }

        private static void ConectarASap(string ClientId)
        {
            //SendMessageToClient(ClientId, "Verificando conexión a Sap");
            if (sapTransferenciaStock == null)
                sapTransferenciaStock = new SapTransferenciaStock();

            if (!sapTransferenciaStock.IsConected())
            {
                SendMessageToClient(ClientId, "Conectando a Sap...");
                if (!sapTransferenciaStock.Connect()) // cuando no se puede conectar es por que el obj sap se inhibe
                {
                    SendMessageToClient(ClientId, "No se pudo conectar, reintentando conexión");
                    sapTransferenciaStock = null;
                    sapTransferenciaStock = new SapTransferenciaStock(); //se reinicia el objeto para hacer otro intento de conexión
                    if (!sapTransferenciaStock.Connect())
                    {
                        sapTransferenciaStock = null;
                        var error = "Alta concurrencia: Vuelva a intentar la sincronización en 1 minuto";
                        SendMessageToClient(ClientId, error, eMessageType.Warning);
                        throw new Exception(error);
                    }
                }
                SendMessageToClient(ClientId, "Conectando a Sap correctamente");
            }
            //else
            //    SendMessageToClient(ClientId, "Ya se encuentra conectado a sap");
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
                    PonerLoteTemporalmenteComoLiberado(me, estadoLote);
                   
                var ms = ProcessTS(me);
                RegresarLotesAlEstadoAnterior();//vuelte al estado anterior
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
                    Error = "TransferenciaStockBusiness, TransferToUbicaciones:"+ex.Message
                };
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        
        private static void updateEstadoLoteOnBdd(string estado, string lote, string codArticulo)
        {
            var sql = string.Format(@"
             update OBTN
              set ""Status""={0}
             where 
              ""DistNumber""='{1}'
              and ""ItemCode""='{2}'
            ", estado, lote,codArticulo);
            new BaseCore().Execute(sql);
        }
        private static void PonerLoteTemporalmenteComoLiberado(TsBodegaMsg me, string codEstadoOriginalLote)
        {
            RegistrarModificacionLote(me, codEstadoOriginalLote);
            updateEstadoLoteOnBdd(Convert.ToInt32(eEstadoLote.Liberado).ToString(), me.Lote, me.CodArticulo);

        }
        private static void RegistrarModificacionLote(TsBodegaMsg me, string codEstadoOriginalLote)
        {
            var sql = string.Format(@"
                insert into ""JB_MODIFICACION_ESTADO_LOTE""(
                 COD_ARTICULO,
                 LOTE,
                 COD_ESTADO_ORIGINAL,
                 FECHA
                ) 
                values('{0}','{1}',{2},CURRENT_TIMESTAMP)
            ", me.CodArticulo, me.Lote, codEstadoOriginalLote);
            new BaseCore().Execute(sql);
        }


        private static void RegresarLotesAlEstadoAnterior()
        {
            //consulto todos los lotes que fueron modificados
            var sql = string.Format(@"
                select 
                 COD_ARTICULO,
                 LOTE,
                 COD_ESTADO_ORIGINAL
                from JB_MODIFICACION_ESTADO_LOTE
            ");
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows)
            {
                var codArticulo = dr["COD_ARTICULO"].ToString();
                var lote = dr["LOTE"].ToString();
                var estadoOriginal = dr["COD_ESTADO_ORIGINAL"].ToString();
                updateEstadoLoteOnBdd(estadoOriginal, lote, codArticulo);
                BorrarRegistroActualizacion(codArticulo, lote);
            }
        }

        private static void BorrarRegistroActualizacion(string codArticulo, string lote)
        {
            var sql = string.Format(@"
                delete from JB_MODIFICACION_ESTADO_LOTE
                where 
                 COD_ARTICULO='{0}'
                 and LOTE='{1}'
            ", codArticulo, lote);
            new BaseCore().Execute(sql);
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
        
        

        

        private static void EliminarModificacionLote(TsBodegaMsg me)
        {
            throw new NotImplementedException();
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
