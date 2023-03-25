namespace FerreteriaAlca.Models
{
    public class ArchivoViewModel
    {
        public string NombreProveedor { get; set; }
        public string Consecutivo { get; set; }
        public string MontoTotal { get; set; }
        public List<ProductViewModel> Productos { get; set; }
    }
}