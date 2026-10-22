using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Frontend_Web.Pages
{
    public class EnviarModel : PageModel
    {
        public string MensajeResultado { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostSubirRtu(IFormFile archivoRtu)
        {
            if (archivoRtu != null)
            {
                using (StreamReader lector = new StreamReader(archivoRtu.OpenReadStream()))
                {
                    string contenidoJson = await lector.ReadToEndAsync();
                    MensajeResultado = await EnviarJsonAApi("http://localhost:5030/ControladorProcesar/Rtu", contenidoJson);
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostSubirFacturas(IFormFile archivoFacturas)
        {
            if (archivoFacturas != null)
            {
                using (StreamReader lector = new StreamReader(archivoFacturas.OpenReadStream()))
                {
                    string contenidoJson = await lector.ReadToEndAsync();
                    MensajeResultado = await EnviarJsonAApi("http://localhost:5030/ControladorProcesar/Facturas", contenidoJson);
                }
            }
            return Page();
        }

        private async Task<string> EnviarJsonAApi(string url, string json)
        {
            using (HttpClient cliente = new HttpClient())
            {
                StringContent contenidoHttp = new StringContent(json, Encoding.UTF8, "application/json");
                
                try
                {
                    HttpResponseMessage respuesta = await cliente.PostAsync(url, contenidoHttp);
                    string respuestaApi = await respuesta.Content.ReadAsStringAsync();
                    return respuestaApi;
                }
                catch
                {
                    return "Error: No se pudo conectar con el servidor Backend_API";
                }
            }
        }
    }
}