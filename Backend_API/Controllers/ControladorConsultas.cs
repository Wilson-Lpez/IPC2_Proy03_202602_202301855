//Clase que controla las consultas
using Microsoft.AspNetCore.Mvc;
using Backend_API.Models;
using Backend_API.Persistencia;
using System.Collections.Generic;
using System;

namespace Backend_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ControladorConsultas : ControllerBase
    {
        //primer endpoint que devuelve el reporte de un contribuyente en un rango de fechas
        [HttpGet("ConsultaDatos")]
        public ActionResult<ReporteContribuyente> ConsultaDatos(string nit, string tipo, string inicio, string fin)
        {
            ReporteContribuyente reporte = new ReporteContribuyente();
            reporte.Nit = nit;

            //facturas procesadas en memoria
            List<RegistroSolicitud> historial = ArchivoDatos.GestorFacturasGlobal.ObtenerHistorial();

            //convertimos los textos de inicio y fin a formato fecha de C# para poder compararlas con < y >
            DateTime fechaInicio = DateTime.ParseExact(inicio, "dd/MM/yyyy", null);
            DateTime fechaFin = DateTime.ParseExact(fin, "dd/MM/yyyy", null);

            //recorremos todas las facturas
            for (int i = 0; i < historial.Count; i++)
            {
                RegistroSolicitud actual = historial[i];

                //Solo importan las facturas sin errores
                if (actual.Aprobada == true)
                {
                    //bandera para saber si esta factura es de la persona que estamos buscando
                    bool coincideNit = false;
                    
                    if (tipo == "emitidas" && actual.Documento.NitEmisor == nit)
                    {
                        coincideNit = true;
                    }
                    else if (tipo == "recibidas" && actual.Documento.NitReceptor == nit)
                    {
                        coincideNit = true;
                    }

                    //si el nit coincide, se revisa la fecha
                    if (coincideNit == true)
                    {
                        //split por espacios para separar las palabras, la fecha exacta quedara en la posicion 1
                        string[] partesTiempo = actual.Documento.Tiempo.Split(' ');
                        string fechaFacturaTexto = partesTiempo[1]; 
                        
                        DateTime fechaFactura = DateTime.ParseExact(fechaFacturaTexto, "dd/MM/yyyy", null);

                        //revisar si la fecha de la factura esta dentro del rango
                        if (fechaFactura >= fechaInicio && fechaFactura <= fechaFin)
                        {
                            //se suma el gran total del reporte
                            reporte.TotalFacturadoRango = reporte.TotalFacturadoRango + actual.Documento.Total;

                            //se busca si ya tenemos esta fecha registrada en nuestra lista agrupada
                            int posicionFecha = -1;
                            for (int j = 0; j < reporte.DetallePorFecha.Count; j++)
                            {
                                if (reporte.DetallePorFecha[j].Fecha == fechaFacturaTexto)
                                {
                                    posicionFecha = j;
                                }
                            }

                            //si no la teniamo, creamos el detalle para ese dia
                            if (posicionFecha == -1)
                            {
                                DetalleFecha nuevoDetalle = new DetalleFecha();
                                nuevoDetalle.Fecha = fechaFacturaTexto;
                                nuevoDetalle.CantidadFacturas = 1;
                                nuevoDetalle.MontoTotal = actual.Documento.Total;
                                
                                reporte.DetallePorFecha.Add(nuevoDetalle);
                            }
                            else
                            {
                                //si ya existia la fecha, solo le sumamos los nuevos
                                reporte.DetallePorFecha[posicionFecha].CantidadFacturas = reporte.DetallePorFecha[posicionFecha].CantidadFacturas + 1;
                                reporte.DetallePorFecha[posicionFecha].MontoTotal = reporte.DetallePorFecha[posicionFecha].MontoTotal + actual.Documento.Total;
                            }
                        }
                    }
                }
            }

            return Ok(reporte);
        }

        //segundo endpoint, resumen de iva por fecha y nit
        [HttpGet("ResumenIva")]
        public ActionResult<List<ResumenIvaNit>> ResumenIva(string fecha)
        {
            List<ResumenIvaNit> listaResumen = new List<ResumenIvaNit>();
            List<RegistroSolicitud> historial = ArchivoDatos.GestorFacturasGlobal.ObtenerHistorial();

            for (int i = 0; i < historial.Count; i++)
            {
                RegistroSolicitud actual = historial[i];
                
                if (actual.Aprobada == true)
                {
                    string[] partesTiempo = actual.Documento.Tiempo.Split(' ');
                    string fechaFactura = partesTiempo[1];

                    //si las factura coincide
                    if (fechaFactura == fecha)
                    {
                        //sumar iva emitido al nit emisor
                        int posEmisor = -1;
                        for (int j = 0; j < listaResumen.Count; j++)
                        {
                            if (listaResumen[j].Nit == actual.Documento.NitEmisor)
                            {
                                posEmisor = j;
                            }
                        }

                        if (posEmisor == -1) //si no existe, se crea
                        {
                            ResumenIvaNit nuevoEmisor = new ResumenIvaNit();
                            nuevoEmisor.Nit = actual.Documento.NitEmisor;
                            nuevoEmisor.IvaEmitido = actual.Documento.Iva;
                            nuevoEmisor.IvaRecibido = 0;
                            listaResumen.Add(nuevoEmisor);
                        }
                        else //si ya existe, solo sumamos al acumulado
                        {
                            listaResumen[posEmisor].IvaEmitido = listaResumen[posEmisor].IvaEmitido + actual.Documento.Iva;
                        }

                        //sumar iva recibido al nit receptor
                        int posReceptor = -1;
                        for (int k = 0; k < listaResumen.Count; k++)
                        {
                            if (listaResumen[k].Nit == actual.Documento.NitReceptor)
                            {
                                posReceptor = k;
                            }
                        }

                        if (posReceptor == -1)
                        {
                            ResumenIvaNit nuevoReceptor = new ResumenIvaNit();
                            nuevoReceptor.Nit = actual.Documento.NitReceptor;
                            nuevoReceptor.IvaEmitido = 0;
                            nuevoReceptor.IvaRecibido = actual.Documento.Iva;
                            listaResumen.Add(nuevoReceptor);
                        }
                        else
                        {
                            listaResumen[posReceptor].IvaRecibido = listaResumen[posReceptor].IvaRecibido + actual.Documento.Iva;
                        }
                    }
                }
            }

            return Ok(listaResumen);
        }

        //tercer endpoint: resumen por rango de fechas
        [HttpGet("ResumenRango")]
        public ActionResult<List<ResumenRangoFecha>> ResumenRango(string inicio, string fin, bool conIva)
        {
            List<ResumenRangoFecha> listaResumen = new List<ResumenRangoFecha>();
            List<RegistroSolicitud> historial = ArchivoDatos.GestorFacturasGlobal.ObtenerHistorial();

            DateTime fechaInicio = DateTime.ParseExact(inicio, "dd/MM/yyyy", null);
            DateTime fechaFin = DateTime.ParseExact(fin, "dd/MM/yyyy", null);

            for (int i = 0; i < historial.Count; i++)
            {
                RegistroSolicitud actual = historial[i];
                
                if (actual.Aprobada == true)
                {
                    string[] partesTiempo = actual.Documento.Tiempo.Split(' ');
                    string fechaFacturaTexto = partesTiempo[1];
                    DateTime fechaFactura = DateTime.ParseExact(fechaFacturaTexto, "dd/MM/yyyy", null);

                    if (fechaFactura >= fechaInicio && fechaFactura <= fechaFin)
                    {
                        //determinamos que valor vamos a sumar segun lo que pidio el usuario
                        double valorASumar = 0;
                        if (conIva == true)
                        {
                            valorASumar = actual.Documento.Total;
                        }
                        else
                        {
                            valorASumar = actual.Documento.Valor;
                        }

                        int posFecha = -1;
                        for (int j = 0; j < listaResumen.Count; j++)
                        {
                            if (listaResumen[j].Fecha == fechaFacturaTexto)
                            {
                                posFecha = j;
                            }
                        }

                        if (posFecha == -1)
                        {
                            ResumenRangoFecha nuevo = new ResumenRangoFecha();
                            nuevo.Fecha = fechaFacturaTexto;
                            nuevo.Valor = valorASumar;
                            listaResumen.Add(nuevo);
                        }
                        else
                        {
                            listaResumen[posFecha].Valor = listaResumen[posFecha].Valor + valorASumar;
                        }
                    }
                }
            }

            return Ok(listaResumen);
        }

        //cuarto endpoint, que devuelve los datos especificos para armar las graficas
        [HttpGet("Grafica")]
        public ActionResult Grafica(string tipo, string fecha, string inicio, string fin)
        {
            // si se pide la grafica de iva
            if (tipo == "iva")
            {
                return ResumenIva(fecha).Result;
            }
            //si se pide la grafica por rango
            else if (tipo == "rango")
            {
                //true por defecto para graficar con el iva incluido
                return ResumenRango(inicio, fin, true).Result; 
            }
            
            return BadRequest("Tipo de grafica no valido");
        }
    }
}