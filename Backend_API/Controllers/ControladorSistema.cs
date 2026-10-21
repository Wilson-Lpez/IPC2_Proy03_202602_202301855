using Microsoft.AspNetCore.Mvc;
using Backend_API.Persistencia;

namespace Backend_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ControladorSistema : ControllerBase
    {
        //este es el endpoint para el boton de "inicializar" que esta en el frontend
        [HttpDelete("Inicializar")]
        public ActionResult Inicializar()
        {
            ArchivoDatos.ReiniciarTodo();
            return Ok(new { mensaje = "El sistema se ha regresado a su estado inicial, sin datos." });
        }
    }
}