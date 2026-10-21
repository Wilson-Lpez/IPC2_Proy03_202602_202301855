using Backend_API.Models;
using System;
using System.Collections.Generic;

namespace Backend_API.Logica
{
    public class GestorFacturas
    {
        private List<RegistroSolicitud> historialSolicitudes; //lista para guardar todas las facturas procesadas tanto aprobadas y rechazadas
        
        private ValidadorNIT validadorNIT; //para validar el nit
        private int correlativoDiario; //contador para llevar el control del correlativo diario
        private string fechaCorrelativo; //guardamos la fecha en formato yyyyMMdd para saber cuando reiniciar el contador

        public GestorFacturas()
        {
            historialSolicitudes = new List<RegistroSolicitud>();
            validadorNIT = new ValidadorNIT();
            correlativoDiario = 1;
            
            //formato de tiempo
            fechaCorrelativo = DateTime.Now.ToString("yyyyMMdd");
        }

        //metodo que recibe la lista de facturas y el gestor del rtu para consultar si existen
        public RespuestaAutorizacion Procesar(List<Factura> peticiones, GestorRtu gestorRtu)
        {
            RespuestaAutorizacion respuesta = new RespuestaAutorizacion();

            if (peticiones == null)
            {
                return respuesta;
            }

            //revisar si cambiamos de dia para reiniciar el correlativo a 1
            string fechaHoy = DateTime.Now.ToString("yyyyMMdd");
            if (fechaHoy != fechaCorrelativo)
            {
                correlativoDiario = 1;
                fechaCorrelativo = fechaHoy;
            }

            //recorremos cada factura que viene en el json
            for (int i = 0; i < peticiones.Count; i++)
            {
                Factura actual = peticiones[i];
                RegistroSolicitud registro = new RegistroSolicitud(actual);

                //validar que los nits cumplan
                if (!validadorNIT.Validar(actual.NitEmisor))
                {
                    registro.ErrorNitEmisorInvalido = true;
                }
                if (!validadorNIT.Validar(actual.NitReceptor))
                {
                    registro.ErrorNitReceptorInvalido = true;
                }

                //validar que los nits existan en el rtu
                List<Contribuyente> listaRtu = gestorRtu.ObtenerContribuyentes();
                bool emisorExiste = false;
                bool receptorExiste = false;

                //recorremos el rtu para buscar los nits
                for (int j = 0; j < listaRtu.Count; j++)
                {
                    if (listaRtu[j].NIT == actual.NitEmisor)
                    {
                        emisorExiste = true;
                    }
                    if (listaRtu[j].NIT == actual.NitReceptor)
                    {
                        receptorExiste = true;
                    }
                }

                if (!emisorExiste)
                {
                    registro.ErrorNitEmisorInexistente = true;
                    respuesta.Resultado.RechazadasEmisorInexistente = respuesta.Resultado.RechazadasEmisorInexistente + 1;
                }
                if (!receptorExiste)
                {
                    registro.ErrorNitReceptorInexistente = true;
                    respuesta.Resultado.RechazadasReceptorInexistente = respuesta.Resultado.RechazadasReceptorInexistente + 1;
                }

                //validar calculos del iva y total
                //el iva se redondea a (valor * 0.12, 2)
                double ivaCalculado = Math.Round(actual.Valor * 0.12, 2);
                if (actual.Iva != ivaCalculado)
                {
                    registro.ErrorIvaMalCalculado = true;
                }

                //el total debe ser el valor mas el iva
                double totalCalculado = actual.Valor + actual.Iva;
                if (actual.Total != totalCalculado)
                {
                    registro.ErrorTotalMalCalculado = true;
                }

                //verificar que la referencia no este duplicada en el historial
                for (int k = 0; k < historialSolicitudes.Count; k++)
                {
                    if (historialSolicitudes[k].Documento.Referencia == actual.Referencia)
                    {
                        registro.ErrorReferenciaDuplicada = true;
                    }
                }

                //si no hay errores se aprueba
                if (!registro.ErrorNitEmisorInvalido && !registro.ErrorNitReceptorInvalido &&
                    !registro.ErrorNitEmisorInexistente && !registro.ErrorNitReceptorInexistente &&
                    !registro.ErrorIvaMalCalculado && !registro.ErrorTotalMalCalculado &&
                    !registro.ErrorReferenciaDuplicada)
                {
                    registro.Aprobada = true;
                    
                    //convertimos el correlativo a texto
                    string correlativoString = correlativoDiario.ToString();
                    
                    //ciclo para agregar los ceros a la izquierda hasta 8 posiciones
                    while (correlativoString.Length < 8)
                    {
                        correlativoString = "0" + correlativoString;
                    }

                    //se une la fecha con el correlativo de 8 digitos
                    string codigoTexto = fechaCorrelativo + correlativoString;
                    long codigoFinal = long.Parse(codigoTexto);
                    
                    registro.CodigoAprobacion = codigoTexto;

                    respuesta.Resultado.Aprobadas = respuesta.Resultado.Aprobadas + 1;
                    respuesta.Resultado.Detalles.Add(new DetalleAprobacion(actual.Referencia, codigoFinal));
                    
                    //aumentamos el correlativo para la siguiente factura aprobada
                    correlativoDiario = correlativoDiario + 1;
                }

                //se guarda el registro tanto aprobado como rechazado en el historial general
                historialSolicitudes.Add(registro);
            }

            return respuesta;
        }

        //metodo para devolver todo el historial y usarlo luego en los reportes
        public List<RegistroSolicitud> ObtenerHistorial()
        {
            return historialSolicitudes;
        }
    }
}