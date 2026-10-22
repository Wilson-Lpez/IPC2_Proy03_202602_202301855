//Clase que recibe los datos de la factura
using System.Text.Json.Serialization;

namespace Backend_API.Models
{
    public class Factura
    {
        [JsonPropertyName("TIEMPO")]
        public string Tiempo {get;set;} //fecha y hora

        [JsonPropertyName("REFERENCIA")]
        public string Referencia {get;set;} //codigo del emisor

        [JsonPropertyName("NIT_EMISOR")]
        public string NitEmisor {get; set;} //nit de quien se emite

        [JsonPropertyName("NIT_RECEPTOR")]
        public string NitReceptor {get;set;} //nit de quien recibe

        [JsonPropertyName("VALOR")]
        public double Valor {get;set;} //El monto sin iva

        [JsonPropertyName("IVA")]
        public double Iva {get;set;} //El iba del producto

        [JsonPropertyName("TOTAL")]
        public double Total {get;set;}

    }
}