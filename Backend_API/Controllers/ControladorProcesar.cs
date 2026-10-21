//Clase para las solicitudes de facturas y RTU
using Microsoft.AspNetCore.Mvc;
using Backend_API.Models;
using Backend_API.Persistencia;
using System.Collections.Generic;

namespace Backend_API.Controllers
{
    [ApiController]
    //la ruta base sera http://localhost:puerto/ControladorProcesar[cite: 7]
    [Route("[controller]")] 
    public class ControladorProcesar : ControllerBase
    {
        //este es el primer endpoint que recibe el json del rtu
        [HttpPost("Rtu")]
        public ActionResult<RespuestaRtu> ProcesarRtu([FromBody] List<Contribuyente> contribuyentes)
        {
            RespuestaRtu respuesta = ArchivoDatos.GestorRtuGlobal.Procesar(contribuyentes);
            return Ok(respuesta);
        }

        //segundo endpoint que recibe el json de las autorizaciones
        [HttpPost("Facturas")]
        public ActionResult<RespuestaAutorizacion> ProcesarFacturas([FromBody] List<Factura> facturas)
        {
            RespuestaAutorizacion respuesta = ArchivoDatos.GestorFacturasGlobal.Procesar(facturas, ArchivoDatos.GestorRtuGlobal);
            return Ok(respuesta);
        }
    }
}