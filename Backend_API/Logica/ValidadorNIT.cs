//Clase que contiene la logica para validar si un nit es correcto o no
namespace Backend_API.Logica
{
    public class ValidadorNIT
    {
        public bool Validar(string nit) //validar si el nit es correcto
        {
            if (nit == null) //validar que no sea nulo
            {
                return false;
            }

            if (nit.Length < 2 || nit.Length > 21) //verificar que sea de un tamaño permitido
            {
                return false;
            }

            for (int i=0; i<nit.Length-1; i++) //verificar que los caracteres sean numeros, menos el ultimo
            {
                if (nit[i] < '0' || nit[i] > '9')
                {
                    return false;
                }
            }

            char verificarRecibido = nit[nit.Length-1]; //el ultimo digito verifica si a sido recibido
            if ((verificarRecibido < '0' || verificarRecibido > '9')
                && verificarRecibido != 'K'
                && verificarRecibido != 'k')
            {
                return false;
            }

            int suma = 0;
            int posicion = 2;

            for (int i=nit.Length-2; i >=0; i--)
            {
                int digito = int.Parse(nit[i].ToString());

                suma = suma + (digito*posicion);
                posicion = posicion + 1;
            }

            int residuo = suma % 11;
            int resta = 11 - residuo;
            int verificarCalculo = resta % 11;

            //si el resultado es 10, el verificador debe ser k
            if (verificarCalculo == 10)
            {
                if (verificarRecibido == 'K' || verificarRecibido == 'k')
                {
                    return true;
                }

                return false;
            }

            if (verificarRecibido == 'K' || verificarRecibido == 'k')
            {
                return false;
            }

            int verificadorNumerico = int.Parse(verificarRecibido.ToString());
            if (verificarCalculo == verificadorNumerico)
            {
                return true;
            }

            return false;
        }
    }
}