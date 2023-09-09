using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.msg.sap;
using jbp.business.hana;
using System.Threading;


namespace jbp.services.rest.Controllers
{
    public class BodegaController : ApiController
    {

        [HttpGet]
        [Route("api/bodega/getLotesConStockByCodArticulo/{codArticulo}")]
        public object GetLotesConStockByCodArticulo(string codArticulo)
        {
            return BodegaBusiness.GetLotesConStockByCodArticulo(codArticulo);
        }

        [HttpGet]
        [Route("api/bodega/getArticulosConStock")]
        public object GetArticulosConStock()
        {
            return BodegaBusiness.GetArticulosConStock();
        }

        [HttpGet]
        [Route("api/bodega/getSubnivelesAlmacen")]
        public List<SubNivelBodegaMsg> GetSubnivelesAlmacen()
        {
            return BodegaBusiness.GetSubnivelesAlmacen();
        }

        [HttpGet]
        [Route("api/bodega/getPedidosPorProveedor/{codProveedor}")]
        public PedidosPorProveedorMsg GetPedidosPorProveedor(string codProveedor)
        {
            return BodegaBusiness.GetPedidosPorProveedor(codProveedor);
        }

        [HttpGet]
        [Route("api/bodega/getEMPorProveedor/{codProveedor}")]
        public EMProveedorMsg GetEMPorProveedor(string codProveedor)
        {
            return BodegaBusiness.GetEMPorProveedor(codProveedor);
        }

        [HttpGet]
        [Route("api/bodega/getUbicacionesYDetArticuloPorLote/{lote}")]
        public object GetUbicacionesYDetArticuloPorLote(string lote)
        {
            return BodegaBusiness.GetUbicacionesYDetArticuloPorLote(lote);
        }

        [HttpGet]
        [Route("api/bodega/getBodegasConUbicaciones")]
        public BodegasMS GetBodegasConUbicaciones()
        {
            return BodegaBusiness.GetBodegasConUbicaciones();
        }
        [HttpGet]
        [Route("api/bodega/getBodegas")]
        public BodegasMS GetBodegas()
        {
            return BodegaBusiness.GetBodegas();
        }
        [HttpGet]
        [Route("api/bodega/getUbicaciones")]
        public UbicacionesMS GetUbicaciones()
        {
            return BodegaBusiness.GetUbicaciones();
        }
        [HttpGet]
        [Route("api/bodega/consultaubicacion/{ubicacion}")]
        public ContenidoUbicacionMS GetContenidoUbicacion(string ubicacion)
        {
            return BodegaBusiness.GetContenidoUbicacion(ubicacion);
        }
    }
}