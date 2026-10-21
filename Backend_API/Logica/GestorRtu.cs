
using Backend_API.Models;

namespace Backend_API.Logica
{
    //administra los contribuyentes del RTU
    public class GestorRtu
    {
        //Lista de contribuyentes
        private List<Contribuyente> contribuyentes;

        //Objeto para validar los NIT
        private ValidadorNIT validadorNIT;

        //Preparar la lista y el validador.
        public GestorRtu()
        {
            contribuyentes = new List<Contribuyente>();
            validadorNIT = new ValidadorNIT();
        }

        //Procesa los contribuyentes recibidos
        public RespuestaRtu Procesar(List<Contribuyente> recibidos)
        {
            RespuestaRtu respuesta = new RespuestaRtu();

            //Evita procesar una lista que no existe
            if (recibidos == null)
            {
                return respuesta;
            }

            //Recorremos todos los contribuyentes
            for (int i = 0; i < recibidos.Count; i++)
            {
                Contribuyente contribuyenteRecibido = recibidos[i];

                //Comprobar que el elemento y su NIT sean válidos
                if (contribuyenteRecibido == null ||
                    !validadorNIT.Validar(contribuyenteRecibido.NIT))
                {
                    respuesta.NitsInvalidos =
                        respuesta.NitsInvalidos + 1;
                }
                else
                {
                    int posicion = BuscarContribuyente(
                        contribuyenteRecibido.NIT
                    );

                    //Si no existe, se agrega como nuevo.
                    if (posicion == -1)
                    {
                        Contribuyente nuevoContribuyente =
                            new Contribuyente(contribuyenteRecibido.NIT, contribuyenteRecibido.Nombre);

                        contribuyentes.Add(nuevoContribuyente);

                        respuesta.ContribuyentesNuevos = respuesta.ContribuyentesNuevos + 1;
                    }
                    else
                    {
                        // Si ya existe, se actualiza su nombre.
                        contribuyentes[posicion].Nombre = contribuyenteRecibido.Nombre;

                        respuesta.ContribuyentesActualizados = respuesta.ContribuyentesActualizados + 1;
                    }
                }
            }

            return respuesta;
        }

        //Buscamos un contribuyente por su NIT.
        private int BuscarContribuyente(string nit)
        {
            for (int i = 0; i < contribuyentes.Count; i++)
            {
                if (contribuyentes[i].NIT == nit)
                {
                    return i;
                }
            }

            //El -1 indica que no fue encontrado.
            return -1;
        }

        //Devuelve la lista de contribuyentes registrados.
        public List<Contribuyente> ObtenerContribuyentes()
        {
            return contribuyentes;
        }
    }
}