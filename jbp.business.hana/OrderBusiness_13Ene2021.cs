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


namespace jbp.business.hana
{
    public class OrderBusiness_13Ene2021
    {
        public static bool Busy;
        public static SapOrder sapOrder=new SapOrder();
        public OrderBusiness_13Ene2021()
        {
            Busy = false;

        }
        public List<string> SaveOrders(List<OrdenMsg> ordenes)
        {
            if (!Busy)
            {
                Busy = true;
                var ms = ProcessOrder(ordenes);
                Busy = false;
                return ms;
            }
            else{//espera máximo 2 minutos para que se desocupe de procesamientos anteriores
                var totalSecondsToWait = 120;
                var secondsElapsed = 0;
                while (secondsElapsed < totalSecondsToWait)
                {
                    var timeWait = 5; //wait for 5 seconds until process
                    Thread.Sleep(timeWait*1000); 
                    if (!Busy)
                    {
                        secondsElapsed = totalSecondsToWait; //to exit bucle
                        Busy = true;
                        var ms2 = ProcessOrder(ordenes);
                        Busy = false;
                        return ms2;
                    }
                    secondsElapsed += timeWait;
                }
                var ms3 = new List<string>();
                var errorMsg = "Servidor ocupado por favor intente mas tarde";
                ordenes.ForEach(o => ms3.Add(errorMsg));//ninguna de las ordenes se procesan
                return ms3;
            }
        }
        private List<string> ProcessOrder(List<OrdenMsg> ordenes)
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
                        order = GetOrdenSinCantidadesEnCero(order);
                        var resp = "";
                        if (DuplicateOrder(order))
                            resp = "Anteriormente ya se procesó esta orden!";
                        else
                        {
                            order.Vendedor = SocioNegocioBusiness.GetVendedorByCodSocioNegocio(order.CodCliente);
                            order.Lines.ForEach(line => {
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

        private OrdenMsg GetOrdenSinCantidadesEnCero(OrdenMsg order)
        {
            // se hace esta validación porque por alguna extraña razón
            // se filtran lineas con cantidad solicitada "0"
            var lineasConCantidad = new List<OrdenLinesMsg>();
            var existeCantSolEnCero = false;
            order.Lines.ForEach(line => {
                if (line.CantSolicitada > 0)
                    lineasConCantidad.Add(line);
                else
                    existeCantSolEnCero = true;
            });
            if (existeCantSolEnCero)
            {
                order.Lines.Clear();
                lineasConCantidad.ForEach(lineaConCantidad =>
                {
                    order.Lines.Add(lineaConCantidad);
                });
            }
            return order;
        }

        public bool DuplicateOrder(OrdenMsg order)
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

        private bool OrdersEquals(OrdenMsg order, OrdenMsg bddOrder)
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

        public List<OrdenMsg> GetOrdersByClientAndDate(string CodClient, DateTime orderDate)
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
    }
}
