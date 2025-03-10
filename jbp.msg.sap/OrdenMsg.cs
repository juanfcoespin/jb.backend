using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.msg.sap
{
    public class OrdenMsg
    {
        public string Cliente { get; set; }
        public string CodCliente { get; set; }
        public string Comentario { get; set; }
        public List<OrdenLinesMsg> Lines { get; set; }
        public int Id { get; set; }
        public string Vendedor { get; set; }

        public OrdenMsg()
        {
            this.Lines = new List<OrdenLinesMsg>();
        }
        public void AddLine(string codArticulo, int cantSol, int cantBon)
        {
            this.Lines.Add(
                new OrdenLinesMsg
                {
                    CodArticulo = codArticulo,
                    CantSolicitada = cantSol,
                    CantBonificacion = cantBon
                }
            );
        }
    }
    public class OrdenLinesMsg
    {
        public double price;

        public string CodArticulo { get; set; }
        public int CantSolicitada { get; set; }
        public int CantBonificacion { get; set; }
        public int CantBruta { 
            get { 
                return this.CantSolicitada + this.CantBonificacion;
            }
        }

        public string CodBodega { get; set; }
    }

    public class OrdenAppMsg
    {
        public string CodCliente { get; set; }
        public string Comentario { get; set; }
        public List<OrdenLinesAppMsg> Lines { get; set; }
        public string client { get; set; }
        public string key { get; set; }
        public string orderDate { get; set; }
        public bool synchronized { get; set; }
        public bool toSync { get; set; }
        public decimal total { get; set; }
        public decimal totalWithOutStock { get; set; }
        public decimal totalWithStock { get; set; }
        public OrdenAppMsg()
        {
            this.Lines = new List<OrdenLinesAppMsg>();
        }
    }
    public class OrdenLinesAppMsg
    {
        public int CantBonificacion { get; set; }
        public int CantSolicitada { get; set; }
        public string CodArticulo { get; set; }
        public string itemCssClass { get; set; }
        public decimal price { get; set; }
        public string productName { get; set; }
        public int stock { get; set; }
        public decimal subtotal
        {
            get {
                return this.CantSolicitada * this.price;
            }
        }
    }
}
