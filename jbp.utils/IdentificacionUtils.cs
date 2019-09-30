using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.utils
{
    public class IdentificacionUtils
    {
        /// <summary>
        /// Trae el código del tipo de identificación
        /// </summary>
        /// <param name="me">identificación, usualmente el RUC</param>
        /// <returns>Código del tipo de identificación</returns>
        public static string GetTipoIdentificacion(string me) {
            if (string.IsNullOrEmpty(me))
                return null;
            me = me.Replace("-", ""); //quita los guiones de la identificación.
            if (me.Equals("9999999999999"))
                return "07"; //consumidor final
            
            switch (me.Length) {
                case 10:
                    return "05"; //cedula
                case 11:
                    return "05"; //ruc
                case 13:
                    return "04";
                case 14:
                    return "04";
            }
            return "06"; //pasaporte
        }
    }
}
