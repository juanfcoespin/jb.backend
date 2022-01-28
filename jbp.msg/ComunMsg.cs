using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace jbp.msg
{
    public class CamposValidar
    {
        public string Campo { get; set; }
        public string Msg { get; set; }
    }
    public class MensajeSalidaMsg {
        /// <summary>
        /// en esta variable se almacenan los errores producidos en el 
        /// servidor para notificar al cliente
        /// </summary>
        public string Error { get; set; }
    }
    public class TestMsg
    {
        public string Type { get; set; }
        public string Payload { get; set; }
    }
    public class BoolMs : MensajeSalidaMsg {
        public bool ms { get; set; }
    }
    public class RangoFechaMsg
    {
        public string Desde { get; set; }
        public string Hasta { get; set; }
        public RangoFechaMsg() { }
    }
    public class ListMS<T> : MensajeSalidaMsg {
        public List<T> List { get; set; }
        public ListMS(){
            this.List = new List<T>();
        }
    }
    public class ItemCombo {
        public int Id { get; set; }
        public string Cod { get; set; }
        public string Nombre { get; set; }
    }
    /// <summary>
    /// Este mensaje sirve para controlar el avance de procesos
    /// largos 
    /// </summary>
    public class ProcessCheckedMsg:MensajeSalidaMsg
    {
        /// <summary>
        /// Identificador del proceso que reside en memoria del servidor, es decir 
        /// no persiste.
        /// Este identificador llevará el id del arreglo por orden de llegada. Elemplo
        /// tengo almacenado en memoria 10 objetos y quiero procesar uno nuevo,
        /// el identificador del proceso sería 11.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// total de items por procesar
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// item actual procesado del total por procesar
        /// </summary>
        public int Current { get; set; }
    }
    public class StatusMsg {
        public string Date { get; set; }
        public string Msg { get; set; }
    }

}
