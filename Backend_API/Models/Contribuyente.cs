//Clase que representa a un contribuyente

namespace Backend_API.Models
{
    public class Contribuyente
    {
        public string NIT {get; set;} //Numero de identificación

        public string Nombre {get; set;} //Nombre del contribuyente

        public Contribuyente(string nit, string nombre) //constructor al crear un nuevo objeto
        {
            NIT = nit;
            Nombre = nombre;
        }
    }
}