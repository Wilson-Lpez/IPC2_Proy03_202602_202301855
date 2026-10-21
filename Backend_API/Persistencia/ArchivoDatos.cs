//Clase que sirve para guardar y cargar informacion 
using Backend_API.Logica;

namespace Backend_API.Persistencia
{
    public static class ArchivoDatos
    {
        //instancias globales que pueden ser leidas por cualquier controlador
        public static GestorRtu GestorRtuGlobal = new GestorRtu();
        public static GestorFacturas GestorFacturasGlobal = new GestorFacturas();

        //metodo para vaciar datos
        public static void ReiniciarTodo()
        {
            GestorRtuGlobal = new GestorRtu();
            GestorFacturasGlobal = new GestorFacturas();
        }
    }
}