using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg;
using jbp.msg.sap;
using TechTools.Core.Hana;
using TechTools.Exceptions;
using System.Data;

namespace jbp.business.hana
{
    public class SocioNegocioBusiness
    {
        public static List<CarteraMsg> GetCarteraByRucPrincipalCliente(string rucPrincipal)
        {
            var ms = new List<CarteraMsg>();
            var sql = string.Format(@"
              select
                 t0.""DocNum"",
                 t1.""CodSocioNegocio"", 
                 t0.""TipoDocumento"",
                 t0.""Vendedor"",
                 t0.""NumFolio"",
                 to_char(t0.""FechaDocumento"", 'yyyy-mm-dd') ""FechaDocumento"",
                 t0.""TotalDocumento"",
                 to_char(t0.""FechaVencimiento"", 'yyyy-mm-dd') ""FechaVencimiento"",
                 t0.""DiasVencido"",
                 t0.""TotalPago"",
                 t0.""SaldoVencido"",
                 t0.""RangoDiasVencido"",
                 t0.""OrdenRango"",
                 t0.""OrdenTipoDocumento""
                from ""JbpVw_Cartera"" t0  inner join
                ""JbpVw_SocioNegocio"" t1 on t1.""CodSocioNegocio"" = t0.""CodCliente""
                where
                 t1.""RucPrincipal""='{0}'
                order by
                 t1.""CodSocioNegocio"",
                 t0.""OrdenRango""
            ",rucPrincipal);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new CarteraMsg
                    {
                        DocNum = bc.GetInt(dr["DocNum"]),
                        CodSocioNegocio = dr["CodSocioNegocio"].ToString(),
                        TipoDocumento = dr["TipoDocumento"].ToString(),
                        Vendedor = dr["Vendedor"].ToString(),
                        NumFolio = dr["NumFolio"].ToString(),
                        FechaDocumento = dr["FechaDocumento"].ToString(),
                        TotalDocumento = bc.GetDecimal(dr["TotalDocumento"]),
                        FechaVencimiento = dr["FechaVencimiento"].ToString(),
                        DiasVencido = bc.GetInt(dr["DiasVencido"]),
                        TotalPago = bc.GetDecimal(dr["TotalPago"]),
                        SaldoVencido = bc.GetDecimal(dr["SaldoVencido"]),
                        RangoDiasVencido = dr["RangoDiasVencido"].ToString(),
                        OrdenRango = bc.GetInt(dr["OrdenRango"]),
                        OrdenTipoDocumento = bc.GetInt(dr["OrdenTipoDocumento"])

                    });
                }
            }
            //se agregan pagos y retenciones a las facturas
            ms.ForEach(fact =>
            {
                if (fact.TipoDocumento == "Factura")
                {
                    fact.Retenciones = FacturaBusiness.GetRetencionesByDocNum(fact.DocNum);
                    fact.Pagos = FacturaBusiness.GetPagosByDocNum(fact.DocNum);
                }
            });
            return ms;
        }

        public static object getVentasYPuntosMesPorRucPrincipal(string rucPrincipal)
        {
            try
            {
                var sql = string.Format(@"
                    select 
                     sum(""montoFactura"") ""Ventas"",
                     sum(""puntos"") ""Puntos""
                    from
                     ""JbpVw_FacturasMasNCParticipantes""
                    where
                     ""RucPrincipal"" = '{0}'
                     and ""fechaFactura"" like '%' || to_char(current_date, 'mm/yyyy') || '%'
                ",rucPrincipal);
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);    
                if(dt.Rows.Count>0)
                    return new
                    {
                        ventas = bc.GetInt(dt.Rows[0]["Ventas"]),
                        puntos = bc.GetInt(dt.Rows[0]["Puntos"])
                    };
                return null;

            }
            catch (Exception e) {
                return new {
                    error="Error: " + e.Message
                };
            }
        }

        public static object GetProveedoresConFacturasReserva()
        {
            try
            {
                /*
                 Todos los proveedores que tengan asociadas facturas de reserva sin entrada de mercancía
                 */
                var ms=new { 
                    data=new List<object>()
                }; 
                
                var sql = @"
                   select 
	                distinct
	                  t0.""CodProveedor"",
                      t0.""Proveedor""
                    from
                     ""JbpVw_FacturasProveedores"" t0 inner join
                     ""JbpVw_FacturaProveedorLinea"" t1 on t1.""IdFactProveedor"" = t0.""Id"" left outer join
                     ""JbpVw_EntradaMercanciaLinea"" t2 on
                         t2.""BaseEntry"" = t0.""Id""
                         and t2.""IdTipoDocumento"" = 18--factura de reserva
                       and t2.""CodArticulo"" = t1.""CodArticulo""
                    where
                     t0.""FacturaDeReserva"" = 'Si'
                     and t0.""Cancelado"" = 'No'
                     and t0.""Estado"" = 'Abierto'
                     and t1.""Cantidad"" - ifnull(t2.""Cantidad"", 0) > 0-- CantPendiente por ingresar > 0
                ";
                var dt=new BaseCore().GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows) {
                    ms.data.Add(new { 
                     codProveedor=dr["CodProveedor"].ToString(),
                     name = dr["Proveedor"].ToString()
                    });
                }
                return ms;
            }
            catch (Exception e) {
                return new { error = e.Message };
            }
        }

        public static double GetPrecioByCodSocioNegocioCodArticulo(string codSocioNegocio, string codArticulo)
        {
            var sql = string.Format(@"
                select
                 top 1
                 ifnull(t1.""Precio"",0)
                from
                 ""JbpVw_SocioNegocio"" t0 left join
                 ""JbpVw_ListaPrecio"" t1 on t1.""Id"" = t0.""IdListaPrecio""
                where
                 t1.""CodArticulo"" = '{0}'
                 and t0.""CodSocioNegocio"" = '{1}'
            ",codArticulo, codSocioNegocio);
            var ms = new BaseCore().GetDoubleScalarByQuery(sql);
            return ms;
        }

        public static ClientesToSendMailMs GetClientesToSendMail()
        {
            var ms = new ClientesToSendMailMs();
            try
            {
                var sql = @"
                    select
                     TOP 500
                     t0.""Id"",
                     t0.""Nombre"",
                     t0.""Email""
                    from
                     ""JbpVw_SocioNegocio"" t0
                    where
                     t0.""CodTipoSocioNegocio"" = 'C'--cliente
                      and t0.""Activo"" = 'Y'
                      and t0.""Email"" is not null
                     --and t0.""CodSocioNegocio"" = 'C1803281631'
                     and t0.""Id"" not in(
                         select ID_CLIENTE
                        FROM JB_ENVIO_MAILS_CLIENTES
                        WHERE
                         TO_CHAR(FECHA_ENVIO, 'YYYY-MM-DD') in ('2022-05-19','2022-05-20')
                         and ENVIADO = TRUE
                     )
                ";
                var bc = new BaseCore();
                var dt = bc.GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows) {
                    var email=dr["Email"].ToString();
                    if (email.Contains(";") || email.Contains(",")) { //se trae el primer correo 
                        var arr=email.Split(new char[] { ';',','});
                        email=arr[0];
                    }
                    ms.Clientes.Add(new ClienteMsg { 
                        Id=bc.GetInt(dr["Id"]),
                        Nombre= dr["Nombre"].ToString(),
                        Email=email
                    });
                }
                
            }
            catch (Exception e)
            {
                ms.Error = e.Message;
            }
            return ms;
        }

        public static void SaveEmailToClient(MailMsg me)
        {
            var sql = string.Format(@"
                insert into JB_ENVIO_MAILS_CLIENTES(FECHA_ENVIO, TITULO, ID_CLIENTE, ENVIADO)
                values(current_timestamp, '{0}', {1}, {2})
            ",me.Titulo, me.IdCliente, me.Enviado);
            new BaseCore().Execute(sql);
        }

        internal static string GetVendedorByCodSocioNegocio(string codCliente)
        {
            var sql = string.Format(@"
                 select 
                  t1.""Vendedor""
                 from
                  ""JbpVw_SocioNegocio"" t0 inner join
                  ""JbpVw_Vendedores"" t1 on t1.""CodVendedor"" = t0.""CodVendedor""
                 where t0.""CodSocioNegocio"" = '{0}'
            ",codCliente);
            return new BaseCore().GetScalarByQuery(sql);
        }

        public static List<SocioNegocioItemMsg> GetProveedoresEM()
        {
            try
            {
                var ms = new List<SocioNegocioItemMsg>();
                //todos los proveedores con entradas de mercancia que tiene lotes con stock>0
                var sql = @"
                select
                 t0.""CodSocioNegocio"",
                 t0.""Nombre"",
                 t0.""Ruc""
                from ""JbpVw_SocioNegocio"" t0 inner join
                (
                 select
                  distinct(t1.""IdProveedor"") ""CodProveedor""
                 from
                  ""JbpVw_EntradaMercanciaLinea"" t0 inner join
                  ""JbpVw_EntradaMercancia"" t1 on t1.""Id"" = t0.""IdEntradaMercancia"" inner join
                  ""JbpVw_OperacionesLote"" t2 on t2.""IdDocBase"" = t1.""Id""
                     and t2.""BaseType"" = t1.""IdTipoDocumento""
                     and t2.""CodArticulo"" = t0.""CodArticulo"" inner join
                  ""JbpVw_Lotes"" t3 on t2.""Lote"" = t3.""Lote"" inner join
                  ""JbpVw_Articulos"" t4 on t4.""CodArticulo"" = t2.""CodArticulo""
                 where
                  t1.""Cancelado"" = 'N'
                  and t3.""Id"" in (--solo las entradas de mercancia que tengan lotes con stock
                     select
                      distinct(""IdLote"")
                     from
                      ""JbpVw_UbicacionPorLote""
                     where ""Cantidad"" > 0
                  )
                 )t1 on t1.""CodProveedor"" = t0.""CodSocioNegocio""
                 order by 2
            ";
                var dt = new BaseCore().GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    ms.Add(new SocioNegocioItemMsg
                    {
                        Codigo = dr["CodSocioNegocio"].ToString(),
                        Ruc = dr["Ruc"].ToString(),
                        Nombre = dr["Nombre"].ToString(),
                    });
                }
                return ms;
            }
            catch (Exception e)
            {
                var ms = new List<SocioNegocioItemMsg>();
                ms.Add(new SocioNegocioItemMsg
                {
                    error = e.Message
                });
                return ms;
            }
            

        }
        
        public static object GetProveedoresConPedidos()
        {
            try
            {
                var data = new List<object>();
                var sql = @"
                select
                 ""CodSocioNegocio"",
                 ""Nombre""
                 from ""JbpVw_SocioNegocio""
                where
                 ""CodTipoSocioNegocio"" = 'S'
                 and ""CodSocioNegocio"" in(
                     select
                     distinct(""CodProveedor"")
                    from
                     ""JbpVw_Pedidos""
                    where
                     lower(""Estado"") like '%abierto%'
                 )
                order by 2
            ";
                var dt = new BaseCore().GetDataTableByQuery(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    data.Add(new
                    {
                        codProveedor = dr["CodSocioNegocio"].ToString(),
                        name = dr["Nombre"].ToString()
                    });
                }
                return new { 
                    data=data
                };
            }
            catch (Exception e)
            {
                return new {
                    error = e.Message
                };
            }
            
        }

        public static List<SocioNegocioItemMsg> GetItemsBytoken(string token)
        {
            var ms = new List<SocioNegocioItemMsg>();
            var campos = new List<string>();
            campos.Add(@"""Ruc""");
            campos.Add(@"""Nombre""");
            campos.Add(@"""NombreComercial""");
          
            var searchCondition = new BaseCore().GetSearchCondition(true, campos, token);
            var sql = String.Format(@"
                select 
                 ""Ruc"",
                 case
                     when ""NombreComercial"" is null or ""NombreComercial"" = '' then ""Nombre""
                     else ""Nombre"" || ' - ' || ""NombreComercial""
                 end ""Nombre""
                from ""JbpVw_SocioNegocio""
                where
                 ""CodTipoSocioNegocio"" = 'C' {0}
            ", searchCondition);
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach(DataRow dr in dt.Rows)
            {
                ms.Add(new SocioNegocioItemMsg { 
                    Ruc=dr["Ruc"].ToString(),
                    Nombre = dr["Nombre"].ToString()
                });
            }
            return ms;
        }

        public static HabilitadoCanjearPuntosMS HabilitadoParaCangearPuntos(string ruc)
        {
            try
            {
                var ms = false;
                var sql = string.Format(@"
                    SELECT 
                     --t0.""Ruc"" ""RucSucursal"",
                     --t1.""Ruc"" ""RucPrincipal"",
                     top 1
                     t1.""EsElite""-- SI o NO para habilitar canje de puntos
                    FROM
                     ""JbpVw_SocioNegocio"" t0 inner join--es el secundario o sucursal
                     ""JbpVw_SocioNegocio"" t1 on t1.""Ruc"" = t0.""RucPrincipal""--es el principal
                    where
                     t0.""Ruc"" = '{0}'--sucursal 0 principal
                     and t1.""AplicaPuntos"" = 'SI' -- el principal participa en el plan puntos
                ", ruc);
                var resp = new BaseCore().GetScalarByQuery(sql);
                if (resp!=null && resp.Equals("SI"))
                    ms = true;
                return new HabilitadoCanjearPuntosMS() { 
                    CodResp=1,
                    Resp=ms
                };
            }
            catch(Exception e) {
                return new HabilitadoCanjearPuntosMS()
                {
                    CodResp = -500,
                    Resp = false
                };
            }
        }

        public static void ActualizarBanderaSincronizacionSocioNegocioPtk(string rucCliente, bool sincronizado)
        {
            try
            {
                
                var bc = new BaseCore();
                var sql = string.Format(@"
                    update OCRD
                    set ""U_JBP_SincronizadoConBddPromotick""={0}
                    where ""LicTradNum"" = '{1}'
                ",bc.GetBooleanSAP(sincronizado),rucCliente);
                bc.Execute(sql);
            }
            catch (Exception e)
            {
                e = ExceptionManager.GetDeepErrorMessage(e, ExceptionManager.eCapa.Business);
                throw e;
            }
        }

        internal static string GetCorreoByCodigo(string codCliente)
        {
            try
            {
                var bc = new BaseCore();
                var sql = string.Format(@"
                    select ""Email"" from ""JbpVw_SocioNegocio""
                    where
                        ""CodSocioNegocio"" = '{0}'
                ", codCliente);
                var correos = bc.GetScalarByQuery(sql);
                return GetUnSoloCorreo(correos);
            }
            catch
            {

                return null;
            }
        }

        /// <summary>
        /// trae un solo correo
        /// </summary>
        /// <param name="email">
        /// Ej: juan.espi@yahoo.com, jespin@jbp.com.ec
        /// Ej: juan.espi@yahoo.com; jespin@jbp.com.ec
        /// </param>
        /// <returns>juan.espi@yahoo.com</returns>
        public static string GetUnSoloCorreo(string emails)
        {
            if (!string.IsNullOrEmpty(emails)) {
                var matriz = emails.Split(new char[] { ',', ';', ' ' });
                if (matriz != null && matriz.Length > 0) {
                    foreach (string email in matriz) {
                        if (!string.IsNullOrEmpty(email))
                            return email;
                    }
                }
                    
            }
            return string.Empty;
        }

        public static List<ClientMsg> GetByCodVendedor(string codVendedor)
        {
            var ms = new List<ClientMsg>();
            var sql = @"
               select 
                ""CodSocioNegocio"", 
                ""Nombre"",
                ""NombreComercial"",
                ""Ruc"",
                ""RucPrincipal"",
                ""JbpFn_GetCumple""(""FechaCumpleaños"") ""FecCumple"",
                ""IdListaPrecio"",
                ""Telefono"",
                ""Celular"",
                ""MetaCompras"",
                ""PorcentajeDescuentoFinanciero"",
                ""EmailPtk""
               from
                ""JbpVw_SocioNegocio""
               where
                ""Activo""='Y'
                and ""CodTipoSocioNegocio""='C'
            ";
            if (!string.IsNullOrEmpty(codVendedor) && codVendedor!="0") { //no aplica para usuarios administradores
                sql += string.Format(@"
                    and ""CodVendedor"" = {0} 
                ",codVendedor);
            }
            sql+=@"   
               order by 2
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if(dt!=null && dt.Rows.Count > 0)
            {
                foreach(DataRow dr in dt.Rows)
                {
                    var codSocioNegocio = dr["CodSocioNegocio"].ToString();
                    ms.Add(new ClientMsg
                    {
                        id = codSocioNegocio,
                        name = dr["Nombre"].ToString(),
                        comercialName = dr["NombreComercial"].ToString(),
                        ruc= dr["Ruc"].ToString(),
                        rucPrincipal = dr["RucPrincipal"].ToString(),
                        birthDate = dr["FecCumple"].ToString(),
                        metaCompras = dr["MetaCompras"].ToString(),
                        priceListId = dr["IdListaPrecio"].ToString(),
                        telefono = dr["Telefono"].ToString(),
                        celular = dr["Celular"].ToString(),
                        directions = GetDirectionsById(codSocioNegocio),
                        contacts = GetContactsById(codSocioNegocio),
                        observations = GetVendorObservationsById(codSocioNegocio),
                        porcentajeDescuentoFinanciero = bc.GetDecimal(dr["PorcentajeDescuentoFinanciero"]),
                        emailPtk = dr["EmailPtk"].ToString(),
                    });
                }
            }
            return ms;
        }
        private static List<ClientDirectionMsg> GetDirectionsById(string codSocioNegocio)
        {
            var ms = new List<ClientDirectionMsg>();
            var sql = string.Format(@"
               select 
                distinct
                ""Ciudad"", 
                ""CalleYNumero""
               from ""JbpVw_DireccionesSN""
               where ""CodSocioNegocio"" = '{0}'
            ", codSocioNegocio);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {

                    ms.Add(new ClientDirectionMsg
                    {
                        city = dr["Ciudad"].ToString(),
                        direction = dr["CalleYNumero"].ToString()
                    });
                }
            }
            return ms;
        }
        private static List<ClientContactMsg> GetContactsById(string codSocioNegocio)
        {
            var ms = new List<ClientContactMsg>();
            var sql = string.Format(@"
               select 
                ""Contacto"", 
                ""Telefono"",
                ""Celular"",
                ""Email"",
                ""Direccion""
               from ""JbpVw_Contactos""
               where ""CodSocioNegocio"" = '{0}'
            ", codSocioNegocio);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {

                    ms.Add(new ClientContactMsg
                    {
                        name = dr["Contacto"].ToString(),
                        phone = dr["Telefono"].ToString(),
                        cellular = dr["Celular"].ToString(),
                        email = dr["Email"].ToString(),
                        direction = dr["Direccion"].ToString()
                    });
                }
            }
            return ms;
        }
        private static List<ClientVendorOnservationMsg> GetVendorObservationsById(string codSocioNegocio)
        {
            var ms = new List<ClientVendorOnservationMsg>();
            return ms;
        }
        internal List<string> GetRucsConRucPrincipalNull()
        {
            var ms = new List<string>();
            var sql = string.Format(@"
                select
                 ""Ruc""
                from
                 ""JbpVw_SocioNegocio""
                where
                 ""AplicaPuntos"" = 'SI'
                 and ""RucPrincipal"" is null
            ");
            var dt = new BaseCore().GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(dr["Ruc"].ToString());
            }
            return ms;
        }
        internal void RegistrarParticipanteComoSincronizado(string ruc)
        {
            var sql = string.Format(@"
                update OCRD
                 set ""U_JBP_SincronizadoConBddPromotick"" = 1 
                where 
                 ""LicTradNum"" = '{0}'
             ",ruc);
            new BaseCore().Execute(sql);
        }
    }
}
