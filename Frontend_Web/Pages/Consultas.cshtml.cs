using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Frontend_Web.Pages
{
    public class ConsultasModel : PageModel
    {
        //propiedades de enlace con HTML
        public ReporteFrontend ResultadoReporte { get; set; }

        public List<ResumenIvaFrontend> ListaIva { get; set; }
        public List<ResumenRangoFrontend> ListaRango { get; set; }

        public string MensajeError { get; set; }

        public void OnGet()
        {
        }

        //metodo al presionar "Generar Reporte"
        public async Task<IActionResult> OnPostReporteContribuyente(string nit, string tipo, string inicio, string fin)
        {
            using (HttpClient cliente = new HttpClient())
            {
                string urlApi = $"http://localhost:5030/ControladorConsultas/ConsultaDatos?nit={nit}&tipo={tipo}&inicio={inicio}&fin={fin}";
                
                try
                {
                    HttpResponseMessage respuesta = await cliente.GetAsync(urlApi);
                    
                    if (respuesta.IsSuccessStatusCode)
                    {
                        //se lee el json que nos devuelve la api
                        string jsonString = await respuesta.Content.ReadAsStringAsync();
                        
                        //para que ignore si las letras estan en mayuscula o minuscula
                        JsonSerializerOptions opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        
                        //convertimos el texto json a objeto local
                        ResultadoReporte = JsonSerializer.Deserialize<ReporteFrontend>(jsonString, opciones);
                    }
                }
                catch
                {
                    MensajeError = "Error de conexión con la API.";
                }
            }
            
            return Page();
        }

        //metodo para el resumen de iva
        public async Task<IActionResult> OnPostResumenIva(string fecha)
        {
            using (HttpClient cliente = new HttpClient())
            {
                string urlApi = $"http://localhost:5030/ControladorConsultas/ResumenIva?fecha={fecha}";
                try
                {
                    HttpResponseMessage respuesta = await cliente.GetAsync(urlApi);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        string jsonString = await respuesta.Content.ReadAsStringAsync();
                        JsonSerializerOptions opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        ListaIva = JsonSerializer.Deserialize<List<ResumenIvaFrontend>>(jsonString, opciones);
                    }
                }
                catch { MensajeError = "Error de conexión con la API."; }
            }
            return Page();
        }



        //metodo para el resumen de rango
        public async Task<IActionResult> OnPostResumenRango(string inicio, string fin, bool conIva)
        {
            using (HttpClient cliente = new HttpClient())
            {
                string urlApi = $"http://localhost:5030/ControladorConsultas/ResumenRango?inicio={inicio}&fin={fin}&conIva={conIva}";
                try
                {
                    HttpResponseMessage respuesta = await cliente.GetAsync(urlApi);
                    if (respuesta.IsSuccessStatusCode)
                    {
                        string jsonString = await respuesta.Content.ReadAsStringAsync();
                        JsonSerializerOptions opciones = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        ListaRango = JsonSerializer.Deserialize<List<ResumenRangoFrontend>>(jsonString, opciones);
                    }
                }
                catch { MensajeError = "Error de conexión con la API."; }
            }
            return Page();
        }
    }

    //clases del frontend para recibir los datos del reporte ordenados
    public class ReporteFrontend
    {
        public string Nit { get; set; }
        public double TotalFacturadoRango { get; set; }
        public List<DetalleFechaFrontend> DetallePorFecha { get; set; }
    }

    public class DetalleFechaFrontend
    {
        public string Fecha { get; set; }
        public int CantidadFacturas { get; set; }
        public double MontoTotal { get; set; }
    }

    //clase para el reporte de iva
    public class ResumenIvaFrontend
    {
        public string Nit { get; set; }
        public double IvaEmitido { get; set; }
        public double IvaRecibido { get; set; }
    }


    //clase para el reporte de rango
    public class ResumenRangoFrontend
    {
        public string Fecha { get; set; }
        public double Valor { get; set; }
    }
}