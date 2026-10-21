//Clase para procesar el RTU
using System.Text.Json.Serialization;

namespace Backend_API.Models
{
    public class RespuestaRtu
    {
        [JsonPropertyName("CONTRIBUYENYES NUEVOS")]
        public int ContribuyentesNuevos {get;set;}

        [JsonPropertyName("CONTRIBUYENTES ACTUALIZADOS")]
        public int ContribuyentesActualizados {get;set;}

        [JsonPropertyName("NITS VALIDOS")]
        public int NitsInvalidos {get;set;}

        public RespuestaRtu()
        {
            ContribuyentesNuevos = 0;
            ContribuyentesActualizados = 0;
            NitsInvalidos = 0;
        }
    }
}