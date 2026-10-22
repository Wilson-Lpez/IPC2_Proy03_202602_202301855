//Clase para construir las respuestas JSON de los resumenes IVA y fechas
using System.Collections.Generic;

namespace Backend_API.Models
{
    //clase para el resultado del endpoint
    public class ResumenIvaNit
    {
        public string Nit { get; set; }
        public double IvaEmitido { get; set; }
        public double IvaRecibido { get; set; }
    }

    //clase para ordenar el resultado del endpoint
    public class ResumenRangoFecha
    {
        public string Fecha { get; set; }
        public double Valor { get; set; } //guarda el total o el valor sin iva
    }
}