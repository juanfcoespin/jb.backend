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
    public class TransferenciaStockController : ApiController
    {
        //esta api utiliza el sistema de balanzas pesaje Espinoza - Paez
        // en el mensaje de entrada debe estar bodegas de orign y desitno asi como los respectivos lotes 
        // en los artículos
        [HttpPost]
        [Route("api/transferenciaStock")]
        public DocSapInsertadoMsg TransferFromBalanzas([FromBody] TsBalanzasMsg me)
        {
            return TransferenciaStockBussiness.TransferFromBalanzas(me);
        }

        [HttpPost]
        [Route("api/tsUbicaciones")]
        public DocSapInsertadoMsg TransferUbicaciones([FromBody] TsBodegaMsg me)
        {
            return TransferenciaStockBussiness.TransferToUbicaciones(me);
        }

        

        [HttpPost]
        [Route("api/tsFromPesajeToMat")]
        public DocSapInsertadoMsg TransferFromPesajeToMat([FromBody] TsFromPesajeToMatMsg me)
        {
            return TransferenciaStockBussiness.TransferFromPesajeToMat(me);
        }

        [HttpPost]
        [Route("api/tsFromST")]
        public DocSapInsertadoMsg TransferFromPicking([FromBody] TsFromPickingME me)
        {
            return TransferenciaStockBussiness.SaveFromST(me);
        }

        
        [HttpGet]
        [Route("api/trasferirCUAR")]
        public List<DocSapInsertadoMsg> trasferirCUAR()
        {
            var ms2 = new List<DocSapInsertadoMsg>();
            var lotesATransferir = TransferenciaStockBussiness.GetLotesCuarentena();
            lotesATransferir.ForEach(item =>
            {
                /*
                 identifico si es materia prima o producto terminado
                 - si el codArticulo empieza por 8 -> PT caso contrario es MP
                 - si es MP -> MAT (CUAR1, CUAR2 -> MAT1, MAT2)
                 - si es PT -> MAT (CUAR1, CUAR2 -> PT1, PT2)
                */
                var bodegaDestino = EsPT(item.CodArticulo) ? "PT" : "MAT";
                bodegaDestino += item.CodBodega == "CUAR1" ? "1" : "2";
                var me = new TsBalanzasMsg
                {
                    CodBodegaDesde = item.CodBodega,
                    CodBodegaHasta = bodegaDestino,
                    Lineas = new List<TsBalanzasLineaMsg> { new TsBalanzasLineaMsg {
                        CodArticulo=item.CodArticulo,
                        Lotes =new List<TsBalanzasLoteMsg>
                        {
                            new TsBalanzasLoteMsg
                            {
                                Cantidad= Convert.ToDouble(item.Cantidad),
                                Lote=item.Lote
                            }
                        }
                    } }
                };
                var ms=TransferenciaStockBussiness.TS_ConLotes(me);
                var msg = String.Format(" articulo {0} de bodega {1} a {2}, cant: {3}",
                            item.CodArticulo,
                            item.CodBodega,
                            bodegaDestino,
                            item.Cantidad
                        );
                if (string.IsNullOrEmpty(ms.Error))
                    Log(String.Format("TS Nro: {0} {1}", ms.DocNum, msg));
                else
                    Log(String.Format("{0} {1}", ms.Error, msg));
                ms2.Add(ms);
            });
            return ms2;
        }

        private  void Log(string msg)
        {
            var logMsg=new jbp.msg.LogMsg();
            logMsg.AppName = "trasferirCUAR";
            logMsg.UserName = "jespin";
            logMsg.Obs = msg;
            UserBusiness.Log(logMsg);
        }

        private  bool EsPT(string codArticulo)
        {
            return (codArticulo[0] == '8' || codArticulo[0] == '9');
        }
    }
}