//clase para la respuesta final de autorizaciones
using System.Text.Json.Serialization;

namespace Backend_API.Models
{
    public class RespuestaAutorizacion
    {
        [JsonPropertyName("resultadoAutorizaciones")]
        public ResultadoAutorizaciones Resultado { get; set; }

        public RespuestaAutorizacion()
        {
            //inicializamos el objeto
            Resultado = new ResultadoAutorizaciones();
        }
    }

    //clase interna para la estructura de los contadores
    public class ResultadoAutorizaciones
    {
        [JsonPropertyName("cantidadAutorizacionesAprobadas")]
        public int Aprobadas { get; set; }

        [JsonPropertyName("cantidadAutorizacionesRechazadasPorNitEmisorInexistente")]
        public int RechazadasEmisorInexistente { get; set; }

        [JsonPropertyName("cantidadAutorizacionesRechazadasPorNitReceptorInexistente")]
        public int RechazadasReceptorInexistente { get; set; }
        
        //lista para guardar el arreglo de referencias aprobadas y su codigo
        [JsonPropertyName("detalleAprobaciones")]
        public List<DetalleAprobacion> Detalles { get; set; }

        public ResultadoAutorizaciones()
        {
            //contadores
            Aprobadas = 0;
            RechazadasEmisorInexistente = 0;
            RechazadasReceptorInexistente = 0;
            Detalles = new List<DetalleAprobacion>();
        }
    }

    //clase para cada factura aprobada en la lista de detalles
    public class DetalleAprobacion
    {
        [JsonPropertyName("referencia")]
        public string Referencia { get; set; }

        //usamos long porque es un numero grande
        [JsonPropertyName("codigoAprobacion")]
        public long CodigoAprobacion { get; set; } 

        public DetalleAprobacion(string referencia, long codigo)
        {
            Referencia = referencia;
            CodigoAprobacion = codigo;
        }
    }
}