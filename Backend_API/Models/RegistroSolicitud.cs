//clase que sirve para procesar y evaluar cada factura
namespace Backend_API.Models
{
    public class RegistroSolicitud
    {
        public Factura Documento { get; set; } //factura original
        public bool Aprobada { get; set; } //Para saber si paso las pruebas
        public string CodigoAprobacion { get; set; } //correlativo final si es aprobada
        
        //booleanos para saber los errores encontrados
        public bool ErrorNitEmisorInvalido { get; set; }
        public bool ErrorNitReceptorInvalido { get; set; }
        public bool ErrorNitEmisorInexistente { get; set; }
        public bool ErrorNitReceptorInexistente { get; set; }
        public bool ErrorIvaMalCalculado { get; set; }
        public bool ErrorTotalMalCalculado { get; set; }
        public bool ErrorReferenciaDuplicada { get; set; }

        //constructor que recibe la factura a evaluar
        public RegistroSolicitud(Factura documento)
        {
            Documento = documento;
            Aprobada = false;
            CodigoAprobacion = "";
            
            //iniciamos todos los errores en falso usando logica basica
            ErrorNitEmisorInvalido = false;
            ErrorNitReceptorInvalido = false;
            ErrorNitEmisorInexistente = false;
            ErrorNitReceptorInexistente = false;
            ErrorIvaMalCalculado = false;
            ErrorTotalMalCalculado = false;
            ErrorReferenciaDuplicada = false;
        }
    }
}