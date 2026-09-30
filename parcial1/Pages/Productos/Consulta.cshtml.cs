using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using parcial1.Data; 


namespace parcial1.Pages.Productos
{
    public class ConsultaModel : PageModel
    {
        private readonly Db _db;
        public ConsultaModel(Db db) => _db = db;
        public List<Producto> Productos { get; set; } = new();
        public void OnGet() => Productos = _db.Productos();


    }
}
