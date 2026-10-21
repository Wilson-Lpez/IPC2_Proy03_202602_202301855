using System.Collections.Generic;

namespace Backend_API.Models
{
    public class ReporteContribuyente
    {
        public string Nit {get;set;}
        public double TotalFacturadoRango {get;set;}
        public List<DetalleFecha> DetallePorFecha {get;set;} //lista para guardar el total de cada dia
    
        public ReporteContribuyente()
        {
            TotalFacturadoRango = 0;
            DetallePorFecha = new List<DetalleFecha>();
        }
    }


    public class DetalleFecha
    {
        public string Fecha{get;set;}
        public int CantidadFacturas{get;set;}
        public double MontoTotal {get;set;}
    }
}