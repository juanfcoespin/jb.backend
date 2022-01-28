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

        public List<ReaccionesMsg> GetReacciones()
        {
            var ms = new List<ReaccionesMsg>();
            var sql = @"
                select 
                 FECHA_REGISTRO,
                 NOMBRES,
                 APELLIDOS,
                 SEXO,
                 RANGO_EDAD,
                 PESO_KG,
                 ALTURA_CM,
                 QUIEN_PADECIO_REACCION,
                 PADECE_OTRA_ENFERMEDAD
                from JBP_REACCIONES_MEDICAMENTOS
            ";
            var bc = new BaseCore();
            var dt = bc.GetDataTableByQuery(sql);
            foreach (DataRow dr in dt.Rows) {
                ms.Add(new ReaccionesMsg
                {
                    fechaRegistro = dr["FECHA_REGISTRO"].ToString(),
                    nombres = dr["NOMBRES"].ToString(),
                    apellidos = dr["APELLIDOS"].ToString(),
                    sexo = getSexo(dr["SEXO"].ToString()),
                    rangoEdad = dr["RANGO_EDAD"].ToString(),
                    pesoKg = bc.GetInt( dr["PESO_KG"].ToString()),
                    alturaCm = bc.GetInt( dr["ALTURA_CM"].ToString()),
                    quienPadecioReaccion = dr["QUIEN_PADECIO_REACCION"].ToString(),
                    padeceOtraEnfermedad = dr["PADECE_OTRA_ENFERMEDAD"].ToString()
                });
            }
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
            var ms = new List<string>();
            if (reacciones != null && reacciones.Count > 0)
            {
                reacciones.ForEach(reaccion =>
                {
                    try
                    {
                        var sql = string.Format(@"
                            insert into JBP_REACCIONES_MEDICAMENTOS(
                                FECHA_REGISTRO, NOMBRES, APELLIDOS, SEXO, RANGO_EDAD,
                                PESO_KG, ALTURA_CM, QUIEN_PADECIO_REACCION, PADECE_OTRA_ENFERMEDAD
                            )VALUES(
                                current_date, '{0}', '{1}',  '{2}', '{3}',   
                                {4}, {5}, '{6}', '{7}'
                            )
                        ",  reaccion.nombres, reaccion.apellidos, reaccion.sexo, reaccion.rangoEdad,
                         reaccion.pesoKg, reaccion.alturaCm, reaccion.quienPadecioReaccion, reaccion.padeceOtraEnfermedad
                        );
                        new BaseCore().Execute(sql);
                        ms.Add("ok");
                    }
                    catch (Exception e)
                    {
                        ms.Add(e.Message);
                    }
                });
            }
            return ms;
        }
        
        private void EnviarCorreoPago(PagoMsg pago)
        {
            string titulo = "Pago Recibido - " + pago.client;
            string msg = string.Empty;
            var bddName = BaseCore.GetBddName();
            msg += string.Format(@"
                <h2>{5}</h2><br>
                <b>Cliente:</b> {0} <br>
                <b>CodCliente:</b> {1} <br>
                <b>Monto Pagado:</b> USD {2} <br>
                <b>Base de datos:</b> {3} <br>
                <b>Comentario:</b><br>{4} <br><br>
            ", pago.client, pago.CodCliente, pago.totalPagado,bddName , pago.comment, titulo);
            msg += "<b>Facturas Pagadas:</b> <br>";
            msg += "<table>";
            msg += " <tr>";
            msg += "    <td style='border: solid 1px #000000'><b>Num Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Total Factura</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Saldo Vencido</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Pagado</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Saldo Pendiente</b></td>";
            msg += " </tr>";
            pago.facturasAPagar.ForEach(factura => {
                var saldoPendiente = factura.toPay - factura.pagado;
                msg += "<tr>";
                msg += "    <td style='border: solid 1px #000000'>" + factura.numDoc+"</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.total + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.toPay+"</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + factura.pagado + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + saldoPendiente.ToString("#.##") + "</td>";
                msg += "</tr>";
            });
            msg += "</table><br>";
            msg += "<b>Detalles del Pago:</b> <br>";
            msg += "<table>";
            msg += " <tr>";
            msg += "    <td style='border: solid 1px #000000'><b>Tipo Pago</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Monto</b></td>";
            msg += "    <td style='border: solid 1px #000000'><b>Banco</b></td>";
            msg += " </tr>";
            pago.tiposPago.ForEach(tp => {
                msg += "<tr>";
                msg += "    <td style='border: solid 1px #000000'>" + tp.tipoPago + "</td>";
                msg += "    <td style='border: solid 1px #000000'>USD " + tp.monto + "</td>";
                msg += "    <td style='border: solid 1px #000000'>" + tp.bancoTxt + "</td>";
                msg += "</tr>";
            });
            msg += "</table><br>";
            if (!string.IsNullOrEmpty(pago.photoComprobanteData))
            {
                msg += "<b>Comprobante Pago:</b> <br>";
                msg += string.Format(@"<img src=""data: image / png; base64, {0}""/>", pago.photoComprobanteData);
            }
            msg += "<div><i><b>Nota: </b>Los pagos detallados en este correo están sujetos a revisión del departamento de cobranzas de James Brown Pharma</div></i>";
            var destinatarios = conf.Default.correoPagos;
            //para que no se envíe al cliente en ambiente de pruebas
            if (!bddName.ToLower().Contains("prueba"))
            {
                var correoCliente = SocioNegocioBusiness.GetCorreoByCodigo(pago.CodCliente);
                if (correoCliente != null && TechTools.Utils.ValidacionUtils.EmailValid(correoCliente))
                    destinatarios += "; " + correoCliente;
            }
            
            this.EnviarPorCorreo(destinatarios, titulo, msg);
        }

        private static bool DuplicatePago(PagoMsg pago)
        {
            return false;
        }
        private static int GetNumFolio(string numDoc)
        {
            //001-010-000081691
            var matriz = numDoc.Split(new char[] { '-' });
            var ex = new Exception("Numero de documento de factura incorrecto: " + numDoc);
            if (matriz != null && matriz.Length > 0)
            {
                var folioStr = matriz[matriz.Length - 1];
                try
                {
                    return Convert.ToInt32(folioStr); //se quitan los 0s de la izq
                }
                catch
                {
                    throw ex;
                }
            }
            else
                throw ex;
        }
    }
}
