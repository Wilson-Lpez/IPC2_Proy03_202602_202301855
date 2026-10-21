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
    }
}