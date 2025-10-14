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
using jbp.msg;
using System.Security.Policy;
using System.Runtime.CompilerServices;
using TechTools.Utils;
using TechTools.Net;
using System.Net;

namespace jbp.business.hana
{
    public class TransferenciaStockBussiness:BaseBusiness
    {
        public static readonly object control = new object();
        public static SapTransferenciaStock sapTransferenciaStock = new SapTransferenciaStock();


        #region Desde solicitud de transferencia

        public static List<TsBodegaMsg> Map(ComponenteMsg me, string responsable, string ubicacionPesaje) { 
            var ms = new List<TsBodegaMsg>();
            me.Lotes.ForEach(lote => { 
                var obj=new TsBodegaMsg();
                obj.Responsable = responsable;
                obj.CodArticulo = me.CodArticulo;
                obj.Lote = lote.Lote;
                obj.ClientId = me.ClientId;
                lote.Ubicaciones.ForEach(ubicacion => {
                    obj.movimientos.Add(new MovimientoTsMsg { 
                        Cantidad=ubicacion.Cantidad,
                        CodBodegaDesde = me.BodegaOrigen,
                        UbicacionDesde = ubicacion.Ubicacion,
                        CodBodegaHasta = me.BodegaOrigen, //es la misma bodega de MP
                        UbicacionHasta = ubicacionPesaje
                    });
                });
                ms.Add(obj);
            });
            return ms;
        }
        public static string GetUbicacionPesajeFromBodegaMat(string codBodegaMat) {
            switch (codBodegaMat) { 
                case "MAT1":
                    return "MAT1-PSJ1";
                case "MAT2":
                    return "MAT2-PSJ2";
            }

            return null;
        }
        public static DocSapInsertadoMsg TransferFromPicking(TsFromPickingME me) {
            try
            {
                var ms = new DocSapInsertadoMsg();
                //se hace una transferencia de la ubicación origen a la ubicación de pesaje
                var ubicacionPesaje = GetUbicacionPesajeFromBodegaMat(me.CodBodegaMat);
                if(ubicacionPesaje == null)
                    throw new Exception("No se ha podido determinar la ubicación de pesaje para la bodega de MAT: " + me.CodBodegaMat);
                me.Componentes.ForEach(c =>
                {
                    SendMessageToClient(c.ClientId, "Procesando componente: " + c.CodArticulo, eMessageType.Warning);
                    var lotesConUbicaciones = Map(c, me.Responsable, ubicacionPesaje);
                    lotesConUbicaciones.ForEach(loteConUbicacion => { 
                        SendMessageToClient(c.ClientId, $"Procesando lote: {loteConUbicacion.Lote}" , eMessageType.Warning);
                        var loteMS = TransferToUbicaciones(loteConUbicacion);
                        loteConUbicacion.DocNumTS=loteMS.DocNum; 
                        if (!string.IsNullOrEmpty(loteMS.Error))
                        {
                            SendMessageToClient(c.ClientId, $"Error: {loteMS.Error}", eMessageType.Error);
                            ms.Error = loteMS.Error; //asigno el error al mensaje de respuesta
                        }
                        else {
                            SetLogLotesPesaje(loteConUbicacion, me);
                            var msg = $"Se transfirió satisfactoriamente con TS Nro:{loteMS.DocNum}";
                            loteConUbicacion.movimientos.ForEach(m => {
                                msg += $"{m.Cantidad} de {m.UbicacionDesde} a {m.UbicacionHasta} ";
                            } );
                            SendMessageToClient(c.ClientId, msg, eMessageType.Success);
                            ms.DocNum=loteMS.DocNum; //asigno el doc num de la ultima transferencia
                        }
                    });
                });
                return ms;
            }
            catch (Exception e)
            {
                SendMessageToClient(me.ClientId, e.StackTrace, eMessageType.Error);
                return new DocSapInsertadoMsg { Error = e.StackTrace };
            }
        }

        private static void SetLogLotesPesaje(TsBodegaMsg loteConUbicacion, TsFromPickingME me)
        {
            double cant = 0;
            loteConUbicacion.movimientos.ForEach(m => cant += m.Cantidad);
            SetCabeceraLogLotesPesaje(loteConUbicacion.Lote, loteConUbicacion.CodArticulo, me.Id, cant, me.NumOF);
            var movimientos = TransferenciaStockMsg.Map(loteConUbicacion, me.NumOF);
            movimientos.ForEach(m => {
                SetLogMovimientoPesaje(m);
            });
        }
        private static void SetCabeceraLogLotesPesaje(string lote, string codArticulo, int idSt, double cantAbiertaInsumo, int docNumOf) {
            var sql = string.Format(@"
                insert into JB_LOTES_PESAJE(LOTE, COD_ARTICULO, ID_ST, CANTIDAD, DOC_NUM_OF, FINALIZADO)
                values('{0}','{1}', {2}, {3}, {4}, 'N')
            ", lote, codArticulo, idSt, cantAbiertaInsumo.ToString().Replace(',', '.'), docNumOf);
            new BaseCore().Execute(sql);
        }

        private static void SetLogMovimientoPesaje(MovimientoPesajeMsg me) {
            
            me.IdLotePesaje = GetIdLotePesaje(me.Lote, me.CodArticulo, me.DocNumOf);
            //cuando se trata de una of en la que no se hizo el picking del componente porque 
            //previamente ya se mando el bulto de dicho insumo en otra orden se inserta una nueva cabecera
            if (me.IdLotePesaje == 0)
            {
                var obj=GetIdStYCantAbiertaInsumo(me.DocNumOf, me.CodArticulo);
                SetCabeceraLogLotesPesaje(me.Lote, me.CodArticulo, obj.IdSt, obj.CantAbiertaInsumo, me.DocNumOf);
                var bc = new BaseCore();
                var strIdLotePsj=bc.GetScalarByQuery("select max(ID) from JB_LOTES_PESAJE");
                me.IdLotePesaje = bc.GetInt(strIdLotePsj);

            }
            var sql = string.Format(@"
                insert into JB_MOVIMIENTOS_LOTE_PESAJE(ID_LOTE_PESAJE, DOC_NUM_TS, CANTIDAD, UBICACION_DESDE, UBICACION_HASTA)
                values({0}, {1}, {2}, '{3}', '{4}')
            ", me.IdLotePesaje, me.DocNumTs, me.Cantidad.ToString().Replace(',','.'), me.UbicacionDesde, me.UbicacionHasta);
            new BaseCore().Execute(sql);
        }
        public class IdStYCant
        {
            public int IdSt { get; internal set; }
            public double CantAbiertaInsumo { get; internal set; }
        }
        public static IdStYCant GetIdStYCantAbiertaInsumo(int docNumOf, string codArticulo) {
            var sql = string.Format(@"
            select
             t1.""Id"",
             t0.""CantidadAbierta""
            from ""JbpVw_SolicitudTrasladoLinea"" t0 inner join
             ""JbpVw_SolicitudTraslado"" t1 on t1.""Id""=t0.""IdSolicitudTraslado""
            where
             t1.""DocNumOrdenFabricacion""={0}
             and t0.""CodArticulo""='{1}'
            ", docNumOf, codArticulo);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0) {
                return new IdStYCant
                {
                    IdSt = bc.GetInt(dt.Rows[0]["Id"]),
                    CantAbiertaInsumo = bc.GetDouble(dt.Rows[0]["CantidadAbierta"])
                };
            }
            return null;
        }

        private static int GetIdLotePesaje(string lote, string codArticulo, int docNumOf) {
            var sql = string.Format(@"
                select top 1 ID from JB_LOTES_PESAJE
                where LOTE='{0}' and COD_ARTICULO='{1}' and DOC_NUM_OF={2}",
                lote, codArticulo, docNumOf);
            return new BaseCore().GetIntScalarByQuery(sql);
        }


        private static DocSapInsertadoMsg ProcessTSFromST(TsFromPickingME me, int numIntentos = 0, bool esHijo = false, SapTransferenciaStock sapTs = null)
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
            var idProceso = Thread.CurrentThread.ManagedThreadId+" OF:"+me.NumOF;
            SendMessageToClient(me.ClientId, "Entrado proceso: " + idProceso, eMessageType.Success);
            //semaforo.WaitAsync();
            var ms = new DocSapInsertadoMsg();
            if (sapTs == null)
                sapTs = new SapTransferenciaStock();
            try
            {
                ConectarASap(me.ClientId, sapTs);
                if (!esHijo)
                {
                    sapTs.StartTransaction();
                }
                var esStDePesaje = false;
                //por cada lote se envía un registro de componente
                me.Componentes.ForEach(c =>
                {
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
                    c.Lotes.ForEach(lote =>
                    {
                        if (!esHijo)
                        {
                            var msg2 = string.Format("Procesando lote {0}, cant enviada: {1}; cant. Reservada: {2}", lote.Lote, lote.CantidadEnviada, lote.CantidadReservada);
                            SendMessageToClient(me.ClientId, msg2, eMessageType.Warning);
                        }
                        lote.Ubicaciones.ForEach(u =>
                        {
                            u.IdUbicacion = BodegaBusiness.GetIdUbicacionByName(u.Ubicacion);
                        });
                        if (c.CantidadEnviada > c.CantidadRequerida)
                        {
                            //se libera el semáforo ya que "procesarReservasOtrasOF" llama a esta misma función (ProcessTSFromST)
                            procesarReservasOtrasOF(me, c, lote, sapTs);
                        }
                    });
                });
                me.Comentario = "ApiPesaje (Responsable: " + me.Responsable + " )";
                if (me.NumOF>0)
                    msg = string.Format("Creando TS para OF:{0} de {1}->{2}", me.NumOF, me.BodegaOrigen, me.BodegaDestino);
                else
                    msg = string.Format("Creando TS de {0}->{1}", me.BodegaOrigen, me.BodegaDestino);
                SendMessageToClient(me.ClientId, msg);

                //se hace la TS con la DiApi, hay que tener en cuenta que no soporta concurrencia
                ms = HacerTSConDiAPI(100,me, sapTs);

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
                    ReservarLotesComponentes(me, sapTs);
                }
                if (!esHijo)
                { //solo la transaccion inicial hace el commit
                    SendMessageToClient(me.ClientId, "Haciendo Commit (Finalizando transacción toma aprox 60seg por componente x favor espere)...", eMessageType.Warning);
                    sapTs.CommitTransaction();
                    SendMessageToClient(me.ClientId, "(Commit) Transacción Finalizada!", eMessageType.Success);
                }
                
                
            }
            catch (Exception e)
            {
                numIntentos = ManejarError(me, numIntentos, esHijo, ms, e);
            }
            finally
            {
                SendMessageToClient(me.ClientId, "Saliendo proceso: " + idProceso, eMessageType.Success);
            }
            return ms;
        }

        public static readonly object _objTsFromSt = new object();
        private static DocSapInsertadoMsg HacerTSConDiAPI(int intentosTope, TsFromPickingME me, SapTransferenciaStock sapTs)
        {
            // para manejar concurrencia en esta función
            var ms=new DocSapInsertadoMsg();
            bool lockAdquirido = false;
            var numIntento=0;
            while (numIntento < intentosTope && !lockAdquirido)
            {
                
                lockAdquirido = Monitor.TryEnter(_objTsFromSt, 5000); // espera 5 segundos por si se está insertando otro registro
                if (!lockAdquirido)
                {
                    numIntento++;
                    SendMessageToClient(me.ClientId, $"Intentando Transferir por {numIntento} vez(ces), en espera que se desocupe SAP", eMessageType.Warning);
                    // Opcional: registrar intento fallido o esperar un poco antes del siguiente intento
                    Thread.Sleep(100); // pequeña pausa entre intentos
                }
            }

            if (lockAdquirido)
            {
                try
                {
                    ms = sapTs.AddFromSt(me);
                }
                finally
                {
                    Monitor.Exit(_objTsFromSt);
                    
                }
            }
            else
            {
                var msg=$"No se pudo adquirir el lock después de {numIntento} intentos.";
                SendMessageToClient(me.ClientId, msg, eMessageType.Error);
                return new DocSapInsertadoMsg { Error = msg };
            }
            return ms;
        }
        
        
        //sistema balanzas espinoza paez
        public static DocSapInsertadoMsg TransferFromBalanzas(TsBalanzasMsg me)
        {
            var ubicacionPesaje = GetUbicacionPesajeFromIpBalanza(me.IpBalanza);
            var ms = new DocSapInsertadoMsg();
            try
            {
                if (me == null || me.DocNumOF == 0)
                    throw new Exception("No se ha pasado como parámetro en número de orden de fabricación 'DocNumOF'");
                Monitor.Enter(control);
                if (sapTransferenciaStock == null)
                    sapTransferenciaStock = new SapTransferenciaStock();
                if (!sapTransferenciaStock.IsConected())
                    sapTransferenciaStock.Connect();//se conecta a sap
                SetCantidadPesadaByTS(me);
                SetAsociadosSTEnComponentes(me);
                var newMe = Map(me, ubicacionPesaje);
                ms = sapTransferenciaStock.AddFromSt(newMe);
                
                if (string.IsNullOrEmpty(ms.Error))
                {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                    CheckAllInsumosPesados(me.DocNumOF); //verifica si todos los componentes fueron pesados
                    var movimientos = TransferenciaStockMsg.Map(newMe, me.DocNumOF, ms.DocNum);
                    movimientos.ForEach(m => {
                        SetLogMovimientoPesaje(m);
                    });
                    FinalizarLogPesaje(me);
                }
                else
                {
                    SetCantidadPesadaByTS(me, true); //rollback
                    if (ms.Error.Contains("(-5002)"))
                        ms.Error += " La materia prima a fraccionarse no ha sido movida en SAP a Pesaje!!";
                }
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
            }
            finally {
                Monitor.Exit(control);
            }
            return ms;
        }

        private static string GetUbicacionPesajeFromIpBalanza(string ipBalanza)
        {
            //para hacer el mapero de las ips a las ubicaciones de pesaje
            //MailUtils.Send("jespin@jbp.com.ec", "IP Balanza: " + ipBalanza, "Se ha recibido una solicitud de pesaje desde la balanza con IP: " + ipBalanza);
            
            var ipsPifo= new List<string>() { "192.168.100.162" };
            foreach (var ip in ipsPifo)
            {
                if (ipBalanza == ip)
                    return "MAT1-PSJ1"; //ubicación de pesaje para la balanza de Pifo
            }
            ///Todo: agregar más ips de balanzas de puembo
            
            // por defecto retorna la ubicación de pesaje de pifo si la ip no está identificada
            return "MAT1-PSJ1";
        }

        private static void FinalizarLogPesaje(TsBalanzasMsg me)
        {
            var bc = new BaseCore();
            me.Lineas.ForEach(linea => {
                linea.Lotes.ForEach(lote => {
                    var sql = string.Format(@"
                    update JB_LOTES_PESAJE 
                    set FINALIZADO='Y'
                    where 
                        ID_ST={0}
                        AND LOTE='{1}'
                        AND COD_ARTICULO='{2}'
                    ", linea.IdSt, lote.Lote, linea.CodArticulo);
                    bc.Execute(sql);
                });
            });
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
                 and lower(t2.""Insumo"") not like '%agua%' -- el agua no se fracciona
                 and t0.""CantidadPesada"" < t0.""CantidadPlanificada""
                 and lower(t2.""UnidadMedida"") in ('kg', 'g', 'l')
                 and t1.""DocNum""={0}
            ", docNumOF);
            var insumosPorPesar = new BaseCore().GetIntScalarByQuery(sql);
            if (insumosPorPesar == 0) { // se fraccionaron todos los insumos
                SetOFPesada(docNumOF);
                EnviarCorreoFinalizacionPesaje(docNumOF);
            }
        }
        public static bool EnviarCorreoFinalizacionPesaje(int docNumOF)
        {
            string error = null;
            return TechTools.Net.MailUtils.Send(
                conf.Default.EmailPesaje,
                "Finalización de Pesaje de OF: " + docNumOF,
                "Se ha finalizado el pesaje de la OF: " + docNumOF + ". Por favor imprimir la asignación de materiales a PROD.",
                ref error
            );
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

        

        private static TsFromPickingME Map(TsBalanzasMsg me, string ubicacionPesaje)
        {
            var ms = new TsFromPickingME();
            ms.Responsable = "Sistema Balanzas Espinoza Paez";
            ms.BodegaDestino = me.CodBodegaHasta;
            ms.BodegaOrigen=me.CodBodegaDesde;
            ms.Id=me.IdST;
            me.Lineas.ForEach(line => {
                if(ms.Id==0)
                    ms.Id=line.IdSt;
                var cantidadEnviada = 0.0;
                var lotes = new List<LoteComponenteMsg>();
                line.Lotes.ForEach(lote => { 
                    cantidadEnviada += lote.Cantidad;
                    var ubicaciones = new List<UbicacionCantidadMsg>();
                    ubicaciones.Add(new UbicacionCantidadMsg { 
                        Ubicacion=ubicacionPesaje,
                        IdUbicacion = BodegaBusiness.GetIdUbicacionByName(ubicacionPesaje),
                        Cantidad = lote.Cantidad,
                    });
                    lotes.Add(new LoteComponenteMsg
                    {
                        Lote = lote.Lote,
                        CantidadEnviada = lote.Cantidad,
                        Ubicaciones = ubicaciones,
                    });
                });
                ms.Componentes.Add(new ComponenteMsg { 
                   BodegaOrigen=me.CodBodegaDesde,
                   BodegaDestino = me.CodBodegaHasta,
                   CantidadEnviada = cantidadEnviada,
                   CodArticulo = line.CodArticulo,
                   LineNum = line.LineNumST,
                   Lotes=lotes,
                });
            });
            return ms;
        }

        private static void SetAsociadosSTEnComponentes(TsBalanzasMsg me)
        {
            if (me == null || me.DocNumOF==0)
                return;
            var sql = string.Format(@"
                select 
                 ""CodInsumo"",
                 ""IdST"",
                 ""LineNumST""
                from 
                 ""JbVw_OFsConTSaPesaje""
                where
                 ""DocNum""={0}
            ", me.DocNumOF);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows)
            {
                me.Lineas.ForEach(linea => {
                    if (linea.CodArticulo == dr["CodInsumo"].ToString()) {
                        linea.IdSt = bc.GetInt(dr["IdST"]);
                        linea.LineNumST = bc.GetInt(dr["LineNumST"]);
                    }
                });
            }
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
                var newMe = TransferenciaStockMsg.Map(me);
                var ms=TransferToUbicaciones(newMe);
                if (string.IsNullOrEmpty(ms.Error)){
                    var movimientos = TransferenciaStockMsg.Map(newMe, me.detalleLote.DocNumOf);
                    movimientos.ForEach(m => {
                        m.DocNumTs = ms.DocNum;
                        SetLogMovimientoPesaje(m);
                    });
                }
                return ms;
            }
            catch (Exception e) {
                return new DocSapInsertadoMsg { Error = e.Message };
            }
            finally
            {
                Monitor.Exit(control);
            }
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

        private static void procesarReservasOtrasOF(TsFromPickingME me, ComponenteMsg c, LoteComponenteMsg lote, SapTransferenciaStock sapTs)
        {
            SendMessageToClient(me.ClientId, "Identificando reservas de otras órdenes de Fabricación para el lote: " + lote.Lote);
            var cantidadesReservadasPorLote = GetCantidadesReservadasPorLote(me, c.CodArticulo, lote.Lote);
            if (cantidadesReservadasPorLote.Count > 0)
            {
                var msg = string.Format("Se ha encontrado {0} STs de {1} a {2} en las que se han hecho reservas de este lote!", cantidadesReservadasPorLote.Count, me.BodegaOrigen, me.BodegaDestino);
                SendMessageToClient(me.ClientId, msg, eMessageType.Warning);
            }
            if (cantidadesReservadasPorLote.Count > 0){
                
                cantidadesReservadasPorLote.ForEach(crl =>
                {
                    //Si es una OF diferente de la original
                    if (crl.DocNumOF != me.NumOF && crl.Cantidad <= Math.Round(c.CantidadEnviada-c.CantidadRequerida,4))
                    {
                        var msg = string.Format("Reserva Identificada: OF:{0}, ST:{1}, Cant:{2}", crl.DocNumOF, crl.DocNumST, crl.Cantidad);
                        SendMessageToClient(me.ClientId, msg, eMessageType.Warning);
                        var otraST = GetOtraST(crl, c, lote, me);
                          
                        msg = string.Format("Otra ST Generada");
                        SendMessageToClient(me.ClientId, msg);
                        var ms = ProcessTSFromST(otraST, 0, true,sapTs);
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

            var ms = SolicitudTransferenciaBusiness.Save(stOFOriginal, 0, sapTransferenciaStock);
            if (!string.IsNullOrEmpty(ms.Error))
            {
                SendMessageToClient(me.ClientId, ms.Error, eMessageType.Error);
                throw new Exception(ms.Error);
            }
            else
                SendMessageToClient(me.ClientId, "Reserva completada Correctamente con Id:" + ms.Id);
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

        private static TsFromPickingME GetOtraST(CantidadesReservadasPorLoteMsg crl, ComponenteMsg componenteOriginal, LoteComponenteMsg loteMe, TsFromPickingME me)
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
            var ubicacionesOtraST = QuitarCantidadOtraReserva(loteMe, crl.Cantidad);
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
                    DocNumOF= bc.GetInt(dr["DocNumOrdenFabricacion"]),
                    Cantidad = bc.GetDouble(dr["Cantidad"]),
                    Lote = lote,
                    BodegaDestino = dr["BodegaDestino"].ToString(),
                    IdSolicitudTraslado = bc.GetInt(dr["IdSolicitudTraslado"]),
                    LineNum = bc.GetInt(dr["LineNum"]),
                });
            }
            
            return ms;
        }

        private static void ConectarASap(string ClientId, SapTransferenciaStock sapTs=null)
        {
            if(sapTs == null)
                sapTs = sapTransferenciaStock;
            if (sapTs == null)
                sapTs = new SapTransferenciaStock();

            if (!sapTs.IsConected())
            {
                SendMessageToClient(ClientId, "Conectando a Sap...");
                if (!sapTs.Connect()) // cuando no se puede conectar es por que el obj sap se inhibe
                {
                    SendMessageToClient(ClientId, "No se pudo conectar, reintentando conexión");
                    sapTs = null;
                    sapTs = new SapTransferenciaStock(); //se reinicia el objeto para hacer otro intento de conexión
                    if (!sapTs.Connect())
                    {
                        sapTs = null;
                        var error = "Alta concurrencia: Vuelva a intentar la sincronización en 1 minuto";
                        SendMessageToClient(ClientId, error, eMessageType.Warning);
                        throw new Exception(error);
                    }
                }
                SendMessageToClient(ClientId, "Conectando a Sap correctamente");
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
                //se notifica al cliente las transacciones de la DIapi de SAP
                sapTransferenciaStock.onNotififacationMessage += (msg =>
                {
                    SendMessageToClient(me.ClientId, msg, eMessageType.Info);
                });
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
                            SendMessageToClient(me.ClientId, "Estableciendo Ids de ubicaciones");
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
                SendMessageToClient(me.ClientId, "Trayendo estado del lote");
                var estadoLote = getEstadoLote(me);
                /*
                 solo se permiten hacer transferencias de stock en lotes liberados
                 Por esto para que desde bodega puedan mover los artículos temporalmente 
                 se cambia el estado del lote a liberado, luego se vuelve a poner el estado
                 anterior del lote
                */
                if (estadoLote != Convert.ToInt32(eEstadoLote.Liberado).ToString())
                {
                    SendMessageToClient(me.ClientId, "Poniendo temporalmente el lote como liberado");
                    PonerLoteTemporalmenteComoLiberado(me, estadoLote);
                }

                SendMessageToClient(me.ClientId, "Procesando Transferencia");

                ConectarASap(me.ClientId, sapTransferenciaStock);
                SendMessageToClient(me.ClientId, "Transfiriendo a ubicación destino...");
                var ms = sapTransferenciaStock.TranferirEntreUbicaciones(me);
                SendMessageToClient(me.ClientId, "Verificando consistencia de estado de lotes...");
                RegresarLotesAlEstadoAnterior();//vuelte al estado anterior
                if (string.IsNullOrEmpty(ms.Error))
                {
                    ms.DocNum = GetDocNumBYId(ms.Id);
                    SendMessageToClient(me.ClientId, "Transferencia Exitosa. Num SAP:" + ms.DocNum, eMessageType.Success);
                }
                else { 
                    throw new Exception(ms.Error);
                }
                return ms;
            }
            catch (Exception ex) {
                var error = "TransferenciaStockBusiness, TransferToUbicaciones:" + ex.Message;
                SendMessageToClient(me.ClientId, error, eMessageType.Error);
                return new DocSapInsertadoMsg
                {
                    Error = error
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
    }
}
