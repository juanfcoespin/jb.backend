using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using jbp.msg.sap;
using jbp.business.hana;

namespace MigrarCUAR
{
    internal class Program
    {
        static void Main(string[] args)
        {

            trasferirCUAR();
        }

        private static void trasferirCUAR() {
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
                var ms = TransferenciaStockBussiness.TS_ConLotes(me);
                if (!string.IsNullOrEmpty(ms.Error))
                    Log("Error: " + ms.Error);
                else
                    Log(String.Format("TS Nro: {0} articulo {1} de bodega {1} a {2}, cant: {3}",
                            ms.DocNum,
                            item.CodArticulo,
                            item.CodBodega,
                            bodegaDestino,
                            item.Cantidad
                        ));
            });

        }

        private static void Log(string msg)
        {
            
        }

        private static bool EsPT(string codArticulo)
        {
            return (codArticulo[0] == '8' || codArticulo[0] == '9');
        }
    }
}
