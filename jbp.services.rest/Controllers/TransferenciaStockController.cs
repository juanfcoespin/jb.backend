using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using jbp.msg.sap;
using jbp.business.hana;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;


namespace jbp.services.rest.Controllers
{
    public class TransferenciaStockController : ApiController
    {
        //esta api utiliza el sistema de balanzas pesaje Espinoza - Paez
        // en el mensaje de entrada debe estar bodegas de origen y destino asi como los respectivos lotes 
        // en los artículos
        [HttpPost]
        [Route("api/transferenciaStock")]
        public DocSapInsertadoMsg TransferFromBalanzas([FromBody] TsBalanzasMsg me)
        {
            return TransferenciaStockBussiness.TransferFromBalanzas(me);
        }

        [HttpPost]
        [Route("api/tsUbicaciones")]
        public async Task<DocSapInsertadoMsg> TransferUbicaciones([FromBody] TsBodegaMsg me){
            return TransferenciaStockBussiness.TransferToUbicaciones(me);
            //pendiente de poner dns a service-layer.jbp
            try{
                using (var client = new HttpClient()){
                    var baseUrl = ConfigurationManager.AppSettings["urlServiceLayerSap"];
                    var apiKey = ConfigurationManager.AppSettings["apiKeyServiceLayerSap"];

                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Add("x-api-key", apiKey);
                    var response = await client.PostAsJsonAsync("tsToUbicaciones", me);

                    if (response.IsSuccessStatusCode)
                        return await response.Content.ReadAsAsync<DocSapInsertadoMsg>();
                    else{
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return new DocSapInsertadoMsg{
                            Error = $"Error en sfaservices ({(int)response.StatusCode} {response.ReasonPhrase}): {errorContent}"
                        };
                    }
                }
            }
            catch (Exception ex){
                return new DocSapInsertadoMsg{
                    Error = $"Error al comunicarse con sfaservices: {ex.Message}"
                };
            }
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
            return TransferenciaStockBussiness.TransferFromPicking(me);
        }
    }
}