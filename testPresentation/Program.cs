using System;
using System.Net;
using System.IO;
using System.Text;
using TechTools.Core.Hana;
using System.Data;

namespace testPresentation
{
    class Program
    {
        static void Main(string[] args)
        {
            var bc = new BaseCore();
            var sql = @"select * from EXX_EDOCS
                WHERE 
                ""RucEmisor"" like '%1803281631%'";

            var dt = bc.GetDataTableByQuery(sql, "FEX");
            /*sql = @"
                select top 3 ""FolioNum"" from OINV
            ";
            var dt = bc.GetDataTableByQuery(sql);*/
            foreach (DataRow dr in dt.Rows) {
                var numDoc = dr["SerieComprobante"].ToString();
                var xml = dr["ComprobanteXml"].ToString();
                Console.Write(xml);
            }
            /*
            var claveAcceso = "2807202107179246521400120010020000046240000462412";
            var ambiente = "2"; //produccion
            var xml = XMLDoc(claveAcceso, ambiente);
            Console.Write(xml);
            */
        }
        public static string XMLDoc(string claveAcceso, string tipoAmbiente)
        {
            string str = string.Empty;
            try
            {
                string requestUriString = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl";
                if (tipoAmbiente.Equals("1"))
                    requestUriString = "https://celcer.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl";
                else if (tipoAmbiente.Equals("2"))
                    requestUriString = "https://cel.sri.gob.ec/comprobantes-electronicos-ws/AutorizacionComprobantesOffline?wsdl";
                byte[] bytes = Encoding.ASCII.GetBytes("<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ec=\"http://ec.gob.sri.ws.autorizacion\">" + "<soapenv:Header/>" + "<soapenv:Body>" + "<ec:autorizacionComprobante>" + "<claveAccesoComprobante>" + claveAcceso + "</claveAccesoComprobante>" + "</ec:autorizacionComprobante>" + "</soapenv:Body>" + "</soapenv:Envelope>");
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString);
                httpWebRequest.Method = "POST";
                httpWebRequest.ContentLength = (long)bytes.Length;
                httpWebRequest.ContentType = "text/xml";
                Stream requestStream = httpWebRequest.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                    str = new StreamReader(response.GetResponseStream()).ReadToEnd();
                str = WebUtility.HtmlDecode(str);
                response.Close();
                if (str.IndexOf("?xml") - 1 > 0)
                {
                    int startIndex = str.IndexOf("?xml") - 1;
                    int num = str.LastIndexOf("\"?") + 3;
                    if (num == 2)
                        num = str.LastIndexOf('?') + 2;
                    str = str.Remove(startIndex, num - startIndex);
                    str = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + str;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return str;
        }
    }
}
