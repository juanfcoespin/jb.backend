using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.business.hana;
using jbp.msg.sap;
using TechTools.Net;
using jbp.msg;

namespace EnvioCorreosClientes
{
    internal class Program
    {
        static void Main(string[] args)
        {
            EnviarCorreos();
        }

        private static void EnviarCorreos()
        {
            var clientesToSendMail = SocioNegocioBusiness.GetClientesToSendMail();
            if (!string.IsNullOrEmpty(clientesToSendMail.Error))
            {
                Console.WriteLine("Error al traer los clientes: " + clientesToSendMail.Error);
                return;
            }
            clientesToSendMail.Clientes.ForEach(cliente => {
                var mailMsg = new MailMsg {
                    IdCliente = cliente.Id,
                    Correo = cliente.Email,
                    Titulo = conf.Default.titulo,
                };
                mailMsg.Mensaje = String.Format(@"
                    <p>
                    Estimado <b>{0}</b><br><br>
                    Queremos presentarles nuestro nuevo logo institucional.<br><br>
                    Cordialmente<br>
                    <b>James Brown Pharma</b><br>
                    <img src='{1}' width='400'><br>
                    </p>
                ", cliente.Nombre, conf.Default.urlImagen);
                Console.WriteLine(String.Format("Enviando a {0} ({1})...", cliente.Nombre, cliente.Email));
                var files=new List<string>();
                files.Add(conf.Default.pathImagen);
                mailMsg.Enviado = MailUtils.Send(mailMsg.Correo,mailMsg.Titulo, mailMsg.Mensaje, files);
                SocioNegocioBusiness.SaveEmailToClient(mailMsg);
                Console.WriteLine(String.Format("Enviado: {0}",mailMsg.Enviado));
            });
            Console.WriteLine("Presione enter para finalizar");
            Console.ReadLine();
        }
    }
}
