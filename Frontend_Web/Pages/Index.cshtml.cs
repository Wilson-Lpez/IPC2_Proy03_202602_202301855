using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Threading.Tasks;

namespace Frontend_Web.Pages
{
    public class IndexModel : PageModel
    {
        public string Mensaje { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostInicializar()
        {
            using (HttpClient cliente = new HttpClient())
            {
                string urlApi = "http://localhost:5030/ControladorSistema/Inicializar";
                
                try
                {
                    HttpResponseMessage respuesta = await cliente.DeleteAsync(urlApi);

                    if (respuesta.IsSuccessStatusCode)
                    {
                        Mensaje = "El sistema se ha inicializado correctamente. Todas las listas están vacías.";
                    }
                    else
                    {
                        Mensaje = "Hubo un error en la respuesta de la API.";
                    }
                }
                catch
                {
                    Mensaje = "No se pudo conectar. Asegúrate de que el Backend_API esté corriendo en el puerto 5000.";
                }
            }

            return Page();
        }
    }
}