using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using jbp.core.sapDiApi;
using TechTools.Core.Hana;
using System.Threading;
using System.Data;
using System.ComponentModel;


namespace jbp.business.hana
{
    public class OrderBusiness
    {
        //para controlar la concurrencia
        public static readonly object control = new object();
        public static SapOrder sapOrder=new SapOrder();
        
        public static List<string> SaveOrders(List<OrdenMsg> ordenes)
        {
            Monitor.Enter(control);
            try
            {
                var ms = ProcessOrder(ordenes);
                return ms;
            }
            finally
            {
                Monitor.Exit(control);
            }
        }
        private static List<string> ProcessOrder(List<OrdenMsg> ordenes)
        {
            
            var ms = new List<string>();
            if (ordenes != null && ordenes.Count > 0)
            {
                if (sapOrder == null)
                    sapOrder = new SapOrder();
                if (!sapOrder.IsConected())
                {
                    sapOrder.Connect();//se conecta a sap
                }
                ordenes.ForEach(order =>
                {
                    try
                    {
                        var resp = "";
                        if (DuplicateOrder(order))
                            resp = "Anteriormente ya se procesó esta orden!";
                        else
                        {
                            order.Vendedor = SocioNegocioBusiness.GetVendedorByCodSocioNegocio(order.CodCliente);
                            order.Lines.ForEach(line => {
                                if (EsProductoVeterinaria(line.CodArticulo))
                                    line.CodBodega = "PICK2"; //es la bodega de despachos de veterinaria
                                line.price = SocioNegocioBusiness.GetPrecioByCodSocioNegocioCodArticulo(order.CodCliente, line.CodArticulo);
                            });

                            resp = sapOrder.Add(order);
                        }
                        ms.Add(resp);
                    }
                    catch (Exception e)
                    {
                        ms.Add(e.Message);
                    }
                });
            }
            return ms;
        }

        private static bool EsProductoVeterinaria(string codArticulo)
        {
            try
            {
                // Si los 3 dígitos iniciale del código están entre 800 y 802 es un producto de veterinaria
                var inicioCodigo = codArticulo.Substring(0, 3);
                int intInicioCodigo=Convert.ToInt32(inicioCodigo);
                return (intInicioCodigo >= 800 && intInicioCodigo <= 802);
            }
            catch
            {
                return false;
            }
            
        }

        public static bool DuplicateOrder(OrdenMsg order)
        {
            var bddOrders = GetOrdersByClientAndDate(order.CodCliente, DateTime.Now);
            if (bddOrders.Count == 0) //no hay ninguna orden de este cliente en esta fecha
                return false;
            
            foreach(var bddOrder in bddOrders)
            {
                if (OrdersEquals(order, bddOrder))
                    return true;
            }
            return false;
        }

        private static bool OrdersEquals(OrdenMsg order, OrdenMsg bddOrder)
        { 
            //se asume que que las 2 ordenes pertenecen al mismo cliente
            // y las dos ordenes son de la misma fecha
            foreach(var line in order.Lines)
            {
                var existLine = bddOrder.Lines.Find(p => 
                        p.CodArticulo.Equals(line.CodArticulo)
                        && p.CantBonificacion.Equals(line.CantBonificacion)
                        && p.CantSolicitada.Equals(line.CantSolicitada)
                    );
                if (existLine == null) //basta que no exista una línea
                    return false;       //las ordenes no son iguales
            }
            return true;//si contiene todas las lineas
        }

        public static List<OrdenMsg> GetOrdersByClientAndDate(string CodClient, DateTime orderDate)
        {
            var ms = new List<OrdenMsg>();
            var sql = string.Format(@"
                select
                 ""Id""
                from
                 ""JbpVw_OrdenVenta"" 
                where
                 ""CodCliente"" = '{0}'
                 and to_char(""Fecha"", 'yyyy-mm-dd') = '{1}'
            ",CodClient,orderDate.ToString("yyyy-MM-dd"));
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt!=null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var order = new OrdenMsg();
                    order.Id = bc.GetInt(dr["Id"]);
                    var sql2 = string.Format(@"
                        select
                            ""CodArticulo"",
                            ""CantSolicitada"",
                            ""CantBonificada""
                        from
                        ""JbpVw_OrdenVentaLinea""
                        where
                        ""IdOrdenVenta"" = {0}
                    ", order.Id);
                    var dt2 = bc.GetDataTableByQuery(sql2);
                    foreach(DataRow dr2 in dt2.Rows)
                    {
                        order.Lines.Add(
                            new OrdenLinesMsg()
                            {
                                CodArticulo = dr2["CodArticulo"].ToString(),
                                CantSolicitada=bc.GetInt(dr2["CantSolicitada"]),
                                CantBonificacion = bc.GetInt(dr2["CantBonificada"])
                            }
                        );
                    }
                    ms.Add(order);
                }
            }
            return ms;
        }
        public static List<OrdenAppMsg> GetOrdersByVendor(int codVendor)
        {
            var ms = new List<OrdenAppMsg>();
            var sql = string.Format(@"
                select
                 top 1000
                 t0.""CodCliente"",
                 t0.""Comentarios"",
                 t1.""Nombre"",
                 t0.""Id"",
                 to_char(t0.""Fecha"",'yyyy-mm-dd') ""Fecha"",
                 t0.""Total""   
                from 
	                ""JbpVw_OrdenVenta"" t0 inner join
	                ""JbpVw_SocioNegocio"" t1 on t1.""CodSocioNegocio""=t0.""CodCliente""
                where
                 t1.""CodVendedor""={0}
                 and t1.""Activo""='Y'
                order by t0.""Fecha"" desc
            ", codVendor);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var order = new OrdenAppMsg();
                    order.CodCliente = dr["CodCliente"].ToString();
                    order.Comentario = dr["Comentarios"].ToString();
                    order.client=dr["Nombre"].ToString();
                    order.key = dr["Id"].ToString();
                    order.orderDate = dr["Fecha"].ToString();
                    order.total = bc.GetDecimal(dr["Total"]);
                    order.Lines = GetOrdersLinesByIdOrder(order.key);
                    ms.Add(order);
                }
            }
            return ms;
        }

        private static List<OrdenLinesAppMsg> GetOrdersLinesByIdOrder(string idOrder)
        {
            var ms = new List<OrdenLinesAppMsg>();
            var sql = string.Format(@"
                select
                 ""CantBonificada"",
                 ""CantSolicitada"",
                 ""CodArticulo"",
                 ""Articulo"",
                 ""PrecioUnitario""
                from 
	                ""JbpVw_OrdenVentaLinea""
                where
                 ""IdOrdenVenta""={0}
            ", idOrder);
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    var orderLine = new OrdenLinesAppMsg();
                    orderLine.CantBonificacion = bc.GetInt(dr["CantBonificada"]);
                    orderLine.CantSolicitada = bc.GetInt(dr["CantSolicitada"]);
                    orderLine.CodArticulo = dr["CodArticulo"].ToString();
                    orderLine.productName = dr["Articulo"].ToString();
                    orderLine.price = bc.GetDecimal(dr["PrecioUnitario"]);
                    ms.Add(orderLine);
                }
            }
            return ms;
        }
    }
}
