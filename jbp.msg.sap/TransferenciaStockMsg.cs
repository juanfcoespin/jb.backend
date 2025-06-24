using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class MovimientoPesajeMsg
    {
        public int IdLotePesaje { get; set; }
        public int DocNumTs { get; set; }
        public double Cantidad { get; set; }
        public string UbicacionDesde { get; set; }
        public string UbicacionHasta { get; set; }
        public string Lote { get; set; }
        public string CodArticulo { get; set; }
        public int DocNumOf { get; set; }
    }
    public class TsFromPickingME
    {
        public string ClientId;

        public string Responsable { get; set; }
        public List<ComponenteMsg> Componentes { get; set; }
        public int Id { get; set; }
        public string BodegaDestino { get; set; }
        public string BodegaOrigen { get; set; }
        public int NumST { get; set; }
        public int NumOF { get; set; }
        public string BodegaProd { get; set; }
        public string Comentario { get; set; }
        public string CodBodegaMat { get; set; }

        public TsFromPickingME()
        {
            this.Componentes = new List<ComponenteMsg>();
        }
    }
    public class ComponenteMsg
    {
        public string BodegaOrigen { get; set; }
        public string BodegaDestino { get; set; }
        public double CantidadEnviada { get; set; }
        public string CodArticulo { get; set; }
        public int LineNum { get; set; }
        public List<LoteComponenteMsg> Lotes { get; set; }
        public double CantidadRequerida { get; set; }
        public string ClientId { get; set; }

        public ComponenteMsg()
        {
            this.Lotes = new List<LoteComponenteMsg>();
        }
    }
    public class LoteComponenteMsg
    {
        public double CantidadEnviada { get; set; }
        public double CantidadReservada { get; set; }
        public string Lote { get; set; }
        public List<UbicacionCantidadMsg> Ubicaciones { get; set; }
        public LoteComponenteMsg()
        {
            this.Ubicaciones = new List<UbicacionCantidadMsg>();
        }

    }
    public class UbicacionCantidadMsg
    {
        public string Ubicacion { get; set; }
        public double Cantidad { get; set; }
        public int IdUbicacion { get; set; }
    }
    public class TransferenciaStockMsg
    {
        public static List<MovimientoPesajeMsg> Map(TsBodegaMsg loteConUbicacion, int numOf)
        {
            var ms=new List<MovimientoPesajeMsg>();
            loteConUbicacion.movimientos.ForEach(m =>
            {
                ms.Add(new MovimientoPesajeMsg
                {
                    
                    DocNumTs = loteConUbicacion.DocNumTS,
                    Cantidad = m.Cantidad,
                    UbicacionDesde = m.UbicacionDesde,
                    UbicacionHasta = m.UbicacionHasta,
                    Lote = loteConUbicacion.Lote,
                    CodArticulo = loteConUbicacion.CodArticulo,
                    DocNumOf = numOf
                });
            });
            return ms;
        }
        public static List<MovimientoPesajeMsg> Map(TsFromPickingME me, int docNumOf, int docNumTs)
        {
            var ms = new List<MovimientoPesajeMsg>();
            me.Componentes.ForEach(c => {
                c.Lotes.ForEach(l => {
                    l.Ubicaciones.ForEach(u => {
                        var obj = new MovimientoPesajeMsg();
                        obj.DocNumTs = docNumTs;
                        obj.DocNumOf = docNumOf;
                        obj.Cantidad = l.CantidadEnviada;
                        obj.UbicacionDesde = u.Ubicacion;
                        obj.UbicacionHasta=c.BodegaDestino;
                        obj.Lote = l.Lote;
                        obj.CodArticulo = c.CodArticulo;
                        ms.Add(obj);
                    });
                });
            });
            return ms;
        }
        public static TsBodegaMsg Map(TsFromPesajeToMatMsg me)
        {
            var ubicacionDesde = me.detalleLote.UbicacionPesaje;
            if (string.IsNullOrEmpty(ubicacionDesde))
                throw new Exception("No se ha podido determinar la ubicación de pesaje!!");
            var ms = new TsBodegaMsg();
            ms.CodArticulo = me.detalleLote.CodArticulo;
            ms.Lote = me.detalleLote.Lote;
            ms.Responsable = me.Responsable;
            ms.ClientId = me.ClientId;
            var movimiento = new MovimientoTsMsg
            {
                Cantidad = me.detalleLote.Cantidad,
                CodBodegaDesde = me.detalleLote.CodBodega,
                UbicacionDesde = ubicacionDesde,
                CodBodegaHasta = me.detalleLote.CodBodega,
                UbicacionHasta = me.UbicacionMatDestino

            };
            if(string.IsNullOrEmpty(movimiento.CodBodegaDesde))
                movimiento.CodBodegaDesde = GetCodBodegaFromUbicacion(movimiento.UbicacionDesde);
            if (string.IsNullOrEmpty(movimiento.CodBodegaHasta))
                movimiento.CodBodegaHasta = GetCodBodegaFromUbicacion(movimiento.UbicacionHasta);
            ms.movimientos.Add(movimiento);
            return ms;
        }

        private static string GetCodBodegaFromUbicacion(string ubicacion)
        {
            var matrix= ubicacion.Split('-');
            if(matrix.Length>0)
                return matrix[0];
            throw new Exception("No se ha podido determinar la bodega desde la ubicación: " + ubicacion);
        }
    }
}
