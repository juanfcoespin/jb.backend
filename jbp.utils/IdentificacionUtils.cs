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
        /// <summary>
        /// Quita los -60, -50, -01, etc (convenciones de protean para el RUC) a fin
        /// de enviar el documento financiero real al SRI
        /// Ejs:
        ///  1803281631-60 -> 1803281631
        ///  180328163-1 -> 1803281631
        /// </summary>
        /// <param name="me">Identificación, usualmente el ruc</param>
        /// <param name="tokensToRemove">Ej: "60,61,50,01,02,05,03,08"</param>
        /// <returns>el ruc sin -60,-50, etc</returns>
        public static string LimpiarIdentificacion(string me, string tokensToRemove) {
            var vector = tokensToRemove.Split(new char[] { ',' });
            if(vector!=null && vector.Length > 0)
            {
                foreach (var token in vector)
                    me = QuitarTokenRuc(me, "-" + token);
            }
            me = QuitarTokenRuc(me, "-");
            return me;
        }

        private static string QuitarTokenRuc(string ruc, string token)
        {
            if (ruc.Contains(token))
                ruc = ruc.Replace(token, "");
            return ruc;
        }
    }
}
