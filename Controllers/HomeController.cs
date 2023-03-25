using FerreteriaAlca.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System;
using System.Xml;

namespace FerreteriaAlca.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private string _fileName;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _fileName = "";
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> AnalizarXML(List<IFormFile> files)
        {

            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string[] directories = path.Split("\\");

            List<String> listaDatos = new List<String>();

            List<ProductViewModel> listaProductos = new List<ProductViewModel>();
            ArchivoViewModel datos = new ArchivoViewModel();

            foreach (var formFile in files)
            {
                _fileName = "C:\\" + directories[1] + "\\" + directories[2] + "\\Downloads\\" + formFile.FileName;
                XmlTextReader reader = null;

                try {
                    reader = new XmlTextReader(_fileName);
                    reader.WhitespaceHandling = WhitespaceHandling.None;

                    while (reader.Read())
                    {
                        if (reader.NodeType == XmlNodeType.Element)
                        {
                            if (reader.Name == "NumeroConsecutivo" || reader.Name == "TotalComprobante")
                            {
                                reader.Read();
                                listaDatos.Add(reader.Value);
                            }

                            if (reader.Name == "Emisor")
                            {
                                // Obtener el Nombre del proveedor
                                while (reader.Name != "Nombre") reader.Read();
                                reader.Read();
                                listaDatos.Add(reader.Value);
                            }

                            if (reader.Name == "LineaDetalle")
                            {
                                ProductViewModel producto = new ProductViewModel();

                                // Obtener el CABYS
                                while(reader.Name != "Codigo") reader.Read();
                                reader.Read();
                                producto.CodigoCabys = reader.Value;

                                // Obtener el codigo de producto
                                while (reader.Name != "Tipo") reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                reader.Read();
                                producto.Codigo = reader.Value;

                                // Obtener el codigo de producto
                                while (reader.Name != "Cantidad") reader.Read();
                                reader.Read();
                                string cantidad = reader.Value.Replace(".", ",");
                                producto.Cantidad = Convert.ToDouble(cantidad);

                                // Obtener el codigo de producto
                                while (reader.Name != "Detalle") reader.Read();
                                reader.Read();
                                producto.Descripcion = reader.Value;

                                // Obtener el codigo de producto
                                while (reader.Name != "PrecioUnitario") reader.Read();
                                reader.Read();
                                string costo = reader.Value.Replace(".", ",");
                                producto.Costo = Convert.ToDouble(costo);

                                // Obtener el codigo de producto
                                while (reader.Name != "MontoTotal") reader.Read();
                                reader.Read();
                                producto.MontoTotal = reader.Value;

                                reader.Read();
                                reader.Read();
                                if(reader.Name == "Descuento")
                                {
                                    reader.Read();
                                    reader.Read();
                                    string descuentoTotal = reader.Value.Replace(".", ",");
                                    var descuentoAplicado = Convert.ToDouble(descuentoTotal) / producto.Cantidad;
                                    producto.Costo = producto.Costo - descuentoAplicado;
                                }
                                producto.Costo = Math.Round(producto.Costo, 3, MidpointRounding.ToEven);
                                listaProductos.Add(producto);
                            }

                        }
                
                    }
                    datos.NombreProveedor = listaDatos[1];
                    datos.Consecutivo = listaDatos[0];
                    datos.MontoTotal = listaDatos[2];
                    datos.Productos = listaProductos;
                }
                finally
                {
                    if(reader == null)
                    {
                        reader.Close();
                    } 
                }
            }
            return View("Resultados", datos);
        }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

//long size = files.Sum(f => f.Length);
//var filePaths = new List<string>();

//_fileName = Directory.GetCurrentDirectory() + "\\UploadFiles\\" + formFile.FileName;

//if (formFile.Length > 0)
//{
//    // full path to file in temp location
//    var filePath = Path.Combine(Directory.GetCurrentDirectory() + "\\UploadFiles", formFile.FileName); //we are using Temp file name just for the example. Add your own file path.
//    filePaths.Add(filePath);
//    using (var stream = new FileStream(filePath, FileMode.Create))
//    {
//        await formFile.CopyToAsync(stream);
//    }
//}

// process uploaded files
// Don't rely on or trust the FileName property without validation.
//return Ok(new { count = files.Count, size, filePaths, listaDatos });

//switch (reader.NodeType)
//{
//    case XmlNodeType.Element:
//        if (reader.Name == "NumeroConsecutivo" || reader.Name == "NombreComercial" || reader.Name == "TotalComprobante")
//        {
//            listaDatos.Add(reader.Name);
//        }
//        cont++;
//        content += "<" + reader.Name + cont + ">";
//        break;
//    case XmlNodeType.Text:
//        cont++;
//        content += reader.Value + cont;
//        break;
//}