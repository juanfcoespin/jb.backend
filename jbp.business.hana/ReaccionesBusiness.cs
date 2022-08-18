using jbp.msg.sap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.core.sapDiApi;
using TechTools.Core.Hana;
using System.Threading;
using System.Data;
using TechTools.Utils;



namespace jbp.business.hana
{
    public class ReaccionesBusiness: BaseBusiness
    {
        //para controlar la concurrencia
        public static readonly object control = new object();

        public List<string> Save(List<ReaccionesMsg> reacciones)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessReacciones(reacciones);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }

        public CatalogosReacciones GetCatalogos()
        {
            try
            {
                return new CatalogosReacciones {
                    quienPadecioReaccion = CatalogoBusiness.GetCatalogByName("quienPadecioReaccion"),
                    viaAdministracion = CatalogoBusiness.GetCatalogByName("viaAdministracion", true),
                    quePasoConMedicamento = CatalogoBusiness.GetCatalogByName("quePasoConMedicamento"),
                    rangoEdad = CatalogoBusiness.GetCatalogByName("rangoEdad"),
                    medicamentosConLotes = GetMedicamentosConLotes(),
                    reacciones = CatalogoBusiness.GetCatalogByName("reacciones"),
                    estadoPersonaAfectada = CatalogoBusiness.GetCatalogByName("estadoPersonaAfectada"),
                };
            }
            catch (Exception e)
            {
                return new CatalogosReacciones {
                    error = e.Message
                };
            }
        }
        public List<MedicamentoConLotesMsg> GetMedicamentosConLotes() { 
            var ms=new List<MedicamentoConLotesMsg>();
            var sql = @"
            select 
             ""CodArticulo"", 
             ""Articulo"" 
            from
             ""JbpVw_Articulos""
            where
             ""Linea"" = 'Humana'
             and ""TipoArticuloAbreviado"" = 'PT'
            order by
             ""Articulo"" 
            ";
            var bc = new BaseCore();
            var dtProductos=bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dtProductos.Rows) {
                ms.Add(new MedicamentoConLotesMsg
                {
                    codArticulo = dr["CodArticulo"].ToString(),
                    articulo = dr["Articulo"].ToString()
                }); 
            }
            ms.ForEach(articulo => {
                articulo.lotes = LoteBusiness.GetLotesByCodArticulo(articulo.codArticulo);
            });
            return ms;
        }
        public List<ReaccionesMsg> GetReacciones()
        {
            var ms = new List<ReaccionesMsg>();
            var sql = @"
                select 
                 T0.id,
                 T1.VALUE ""RangoEdad"",
                 T2.VALUE ""QuienPadecioEnfermedad"",
                 T0.FECHA_REGISTRO,
                 T0.NOMBRES,
                 T0.APELLIDOS,
                 T0.SEXO,
                 T0.PESO_KG,
                 T0.ALTURA_CM,
                 T0.PADECE_OTRA_ENFERMEDAD,
                 t0.NOTIFICADOR,
                 t0.NOTIFICADOR_MAIL,
                 t0.NOTIFICADOR_TELEFONO,
                 T0.OTRA_ENFERMEDAD,
                 T0.REACCIONES
                from JBP_REACCIONES T0 INNER JOIN
                    JB_CATALOG_VALUES T1 ON T1.ID = T0.ID_RANGO_EDAD INNER JOIN
                    JB_CATALOG_VALUES T2 ON T2.ID = T0.ID_QUIEN_PADECIO_REACCION
                order by
                    T0.FECHA_REGISTRO desc
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {

                var reaccion=new ReaccionesMsg
                {
                    id= bc.GetInt(dr["id"]),
                    rangoEdad = dr["RangoEdad"].ToString(),
                    quienPadecioEnfermedad = dr["QuienPadecioEnfermedad"].ToString(),
                    fechaRegistro = dr["FECHA_REGISTRO"].ToString(),
                    nombres = dr["NOMBRES"].ToString(),
                    apellidos = dr["APELLIDOS"].ToString(),
                    sexo = getSexo(dr["SEXO"].ToString()),
                    pesoKg = bc.GetInt( dr["PESO_KG"].ToString()),
                    alturaCm = bc.GetInt( dr["ALTURA_CM"].ToString()),
                    padeceOtraEnfermedad = bc.GetBoolean(dr["PADECE_OTRA_ENFERMEDAD"]),
                    notificador = dr["NOTIFICADOR"].ToString(),
                    notificadorMail = dr["NOTIFICADOR_MAIL"].ToString(),
                    notificadorTelefono = dr["NOTIFICADOR_TELEFONO"].ToString(),
                    otraEnfermedad = dr["OTRA_ENFERMEDAD"].ToString(),
                    reaccionesStr= getReaccionesSrt(dr["REACCIONES"].ToString(),bc),
                    
                };
                reaccion.idStr = "H-RAM-"+StringUtils.PonerCerosIzquierda(4, bc.GetInt(dr["id"]).ToString());
                reaccion.medicamentos = getMedicamentosByIdReaccion(reaccion.id, bc);
                reaccion.informacionesReaccion = getInfoReaccionesByIdReaccion(reaccion.id, bc);
                ms.Add(reaccion);
            }
            return ms;
        }
        private List<InfoReaccion> getInfoReaccionesByIdReaccion(int idReaccion, BaseCore bc)
        {
            var ms = new List<InfoReaccion>();
            var sql = string.Format(@"
                select 
                 T0.id,
               	 t1.value ESTADO_PERSONA_AFECTADA,
               	 to_char(T0.FECHA_INICIO,'yyyy-mm-dd') FECHA_INICIO,
               	 to_char(T0.FECHA_FIN,'yyyy-mm-dd') FECHA_FIN,
               	 T0.SIGUIO_TRATAMIENTO,
               	 T0.SINTOMAS,
               	 T0.TRATAMIENTO
                from JBP_REACCIONES_INFO T0 INNER JOIN
                    JB_CATALOG_VALUES T1 ON T1.ID = T0.ID_ESTADO_PERSONA_AFECTADA
                where
                 T0.ID_REACCION = {0}
            ", idReaccion);
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows)
            {
                ms.Add(new InfoReaccion
                {
                    id = bc.GetInt(dr["id"]),
                    idReaccion=idReaccion,
                    estadoPersonaAfectada = dr["ESTADO_PERSONA_AFECTADA"].ToString(),
                    fechaInicio = dr["FECHA_INICIO"].ToString(),
                    fechaFin = dr["FECHA_FIN"].ToString(),
                    siguioTratamiento = bc.GetBoolean(dr["SIGUIO_TRATAMIENTO"]),
                    sintomas = dr["SINTOMAS"].ToString(),
                    tratamiento = dr["TRATAMIENTO"].ToString()
                });
            }
            return ms;
        }
        private List<MedicamentoItem> getMedicamentosByIdReaccion(int idReaccion, BaseCore bc)
        {
            var ms = new List<MedicamentoItem>();
            var sql = string.Format(@"
                select 
                 T0.id,
                 T0.PARA_QUE_UTILIZO,
                 T1.VALUE ""ViaAdministracion"",
                 T2.VALUE ""QuePasoConElMedicamento"",
                 t3.""ItemName"" ""Medicamento"",
                 T0.LOTE,
                 T0.FECHA_VENCIMIENTO,
                 T0.CANTIDAD_FRECUENCIA,
                 to_char(T0.FECHA_UTILIZACION,'yyyy-mm-dd') ""FECHA_UTILIZACION"",
                 to_char(T0.CUANDO_DEJO_USAR,'yyyy-mm-dd') ""CUANDO_DEJO_USAR"",
                 T0.HA_VUELTO_REACCION,
                 T0.PARA_QUE_UTILIZO,
                 T0.POSOLOGIA
                from JBP_REACCIONES_MEDICAMENTOS T0 INNER JOIN
                    JB_CATALOG_VALUES T1 ON T1.ID = T0.ID_VIA_ADMINISTRACION INNER JOIN
                    JB_CATALOG_VALUES T2 ON t2.ID = T0.ID_QUE_PASO_CON_MEDICAMENTO INNER JOIN
                    OITM T3 ON T3.""ItemCode"" = t0.COD_ARTICULO
                where
                 T0.ID_REACCION = {0}
            ", idReaccion);
            var dt=bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new MedicamentoItem {
                    id = bc.GetInt(dr["id"]),
                    idReaccion=idReaccion,
                    viaAdministracion = dr["ViaAdministracion"].ToString(),
                    quePasoConMedicamento = dr["QuePasoConElMedicamento"].ToString(),
                    medicamento = dr["Medicamento"].ToString(),
                    lote = dr["LOTE"].ToString(),
                    fechaVencimiento = dr["FECHA_VENCIMIENTO"].ToString(),
                    cantidadFrecuencia = dr["CANTIDAD_FRECUENCIA"].ToString(),
                    fechaUtilizacion = dr["FECHA_UTILIZACION"].ToString(),
                    cuandoDejoUsar = dr["CUANDO_DEJO_USAR"].ToString(),
                    haVueltoReaccion = bc.GetBoolean(dr["HA_VUELTO_REACCION"]),
                    paraQueUtilizo = dr["PARA_QUE_UTILIZO"].ToString(),
                    posologia = dr["POSOLOGIA"].ToString()
                });
            }
            return ms;
        }
        private List<string> getReaccionesSrt(string me, BaseCore bc)
        {
            var ms=new List<string>();
            var arr = me.Split(new char[] { ',' }).ToList();
            arr.ForEach(idReaccion =>
            {
                var sql = string.Format(@"
                    select value from JB_CATALOG_VALUES
                    where id={0}
                ",idReaccion);
                ms.Add(bc.GetScalarByQuery(sql));
            });
            return ms;
        }
        private string getSexo(string sexo)
        {
            switch (sexo.ToLower()) {
                case "m":
                    return "Masculino";
                case "f":
                    return "Femenino";
            }
            return "No Definido";
        }
        private List<string> ProcessReacciones(List<ReaccionesMsg> reacciones)
        {
            /*
             de momento no se sabe como hacer transacciones en hana
             por lo que se emulará una
             */
            var bc=new BaseCore();
            var ms = new List<string>();
            if (reacciones != null && reacciones.Count > 0)
            {
                reacciones.ForEach(reaccion =>
                {
                    try
                    {
                        var sql = string.Format(@"
                            insert into JBP_REACCIONES(
                                ID_RANGO_EDAD,
                                ID_QUIEN_PADECIO_REACCION,
                                FECHA_REGISTRO,
                                NOMBRES, APELLIDOS, SEXO,
                                PESO_KG, ALTURA_CM, 
                                PADECE_OTRA_ENFERMEDAD,
                                NOTIFICADOR,
                                NOTIFICADOR_MAIL,
                                NOTIFICADOR_TELEFONO,
                                OTRA_ENFERMEDAD,
                                REACCIONES
                            )VALUES(
                                {0},
                                {1},
                                current_timestamp,
                                '{2}', '{3}',  '{4}',
                                {5}, {6},
                                {7},
                                '{8}',--NOTIFICADOR 
                                '{9}',
                                '{10}',
                                '{11}',
                                '{12}' --REACCIONES
                            )
                        ",  
                            reaccion.idRangoEdad,
                            reaccion.idQuienPadecioReaccion,
                            reaccion.nombres,reaccion.apellidos, reaccion.sexo,
                            reaccion.pesoKg, reaccion.alturaCm,
                            reaccion.padeceOtraEnfermedad,
                            reaccion.notificador,
                            reaccion.notificadorMail,
                            reaccion.notificadorTelefono,
                            reaccion.otraEnfermedad,
                            getReaccionesChecked(reaccion.reacciones)
                        );
                        bc.Execute(sql);
                        sql = "select max(ID) from JBP_REACCIONES";
                        var idReaccion = bc.GetIntScalarByQuery(sql);
                        try
                        {
                            reaccion.medicamentos.ForEach(medicamento => {
                                medicamento.idReaccion = idReaccion;
                                saveMedicamento(medicamento, bc);
                            });
                            reaccion.informacionesReaccion.ForEach(infoReaccion => {
                                infoReaccion.idReaccion = idReaccion;
                                saveInfoReaccion(infoReaccion, bc);
                            });
                            //throw new Exception("Error de prueba de rollback");
                            ms.Add("ok");
                        }
                        catch (Exception e) {
                            var error=rollback(idReaccion,bc);
                            error += e.Message;
                            ms.Add(error);
                        }
                    }
                    catch (Exception e)
                    {
                        ms.Add(e.Message);
                    }
                });
            }
            return ms;
        }

        private string rollback(int idReaccion, BaseCore bc)
        {
            try
            {
                var sql = string.Format(@"
                    DELETE FROM JBP_REACCIONES_MEDICAMENTOS            
                    WHERE ID_REACCION={0}
                ", idReaccion);
                bc.Execute(sql);
                sql = string.Format(@"
                    DELETE FROM JBP_REACCIONES_INFO            
                    WHERE ID_REACCION={0}
                ", idReaccion);
                bc.Execute(sql);
                sql = string.Format(@"
                    DELETE FROM JBP_REACCIONES
                    WHERE ID={0}
                ", idReaccion);
                bc.Execute(sql);
                return null;
            }
            catch(Exception e){
                return e.Message+ " ,";
            }
        }

        private void saveInfoReaccion(InfoReaccion infoReaccion, BaseCore bc)
        {
            string fechaFin = null;
            if (string.IsNullOrEmpty(infoReaccion.fechaFin))
            {
                fechaFin = "null";
            }
            else {
                fechaFin = string.Format("to_date('{0}','yyyy-mm-dd')",infoReaccion.fechaFin.Substring(0, 10));
            }

            var sql = string.Format(@"
                insert into JBP_REACCIONES_INFO(
                    ID_REACCION,
                    ID_ESTADO_PERSONA_AFECTADA,
                    FECHA_INICIO,
                    FECHA_FIN,
                    SIGUIO_TRATAMIENTO,
                    SINTOMAS,
                    TRATAMIENTO
                )values(
                    {0},
                    {1},
                    to_date('{2}','yyyy-mm-dd'),
                    {3},
                    {4},
                    '{5}',
                    '{6}'
                )
            ",
                infoReaccion.idReaccion,
                infoReaccion.idEstadoPersonaAfectada,
                infoReaccion.fechaInicio.Substring(0, 10),
                fechaFin,
                infoReaccion.siguioTratamiento,
                infoReaccion.sintomas,
                infoReaccion.tratamiento
            );
            bc.Execute(sql);
        }
        private void saveMedicamento(MedicamentoItem medicamento, BaseCore bc)
        {
            var sql = string.Format(@"
                insert into JBP_REACCIONES_MEDICAMENTOS(
                    ID_REACCION,
                    ID_VIA_ADMINISTRACION,
                    ID_QUE_PASO_CON_MEDICAMENTO,
                    COD_ARTICULO,
                    LOTE,
                    FECHA_VENCIMIENTO,
                    CANTIDAD_FRECUENCIA,
                    FECHA_UTILIZACION,
                    CUANDO_DEJO_USAR,
                    HA_VUELTO_REACCION,
                    PARA_QUE_UTILIZO,
                    POSOLOGIA
                )values(
                    {0},
                    {1},
                    {2},
                    '{3}',
                    '{4}',
                    '{5}',
                    '{6}',
                    to_date('{7}','yyyy-mm-dd'),
                    to_date('{8}','yyyy-mm-dd'),
                    {9},
                    '{10}',
                    '{11}'
                )
            ",
                medicamento.idReaccion,
                medicamento.codViaAdministracion,
                medicamento.idQuePasoConMedicamento,
                medicamento.codMedicamento,
                medicamento.lote,
                medicamento.fechaVencimiento,
                medicamento.cantidadFrecuencia,
                medicamento.fechaUtilizacion.Substring(0,10), //2022-03-15T10:52:00-05:00
                (medicamento.cuandoDejoUsar!=null) ? medicamento.cuandoDejoUsar.Substring(0, 10):null,
                medicamento.haVueltoReaccion,
                medicamento.paraQueUtilizo,
                medicamento.posologia
            );
            bc.Execute(sql);
        }
        private string getReaccionesChecked(List<ReaccionItem> reacciones)
        {
            var ms = "";
            var reaccionesSeleccionadas = reacciones.FindAll(r => r.selected);
            var i = 0;
            reaccionesSeleccionadas.ForEach(r =>
            {
                if (i > 0)
                    ms += ",";
                ms += r.id;
                i++;
            });
            //contiene los ids de las reacciones checkeadas
            return ms;
        }
    }
}
