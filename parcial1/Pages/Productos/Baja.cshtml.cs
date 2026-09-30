using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using parcial1.Data;

namespace parcial1.Pages.Productos
{
    public class BajaModel : PageModel
    {
        private readonly Db _db;
        public BajaModel(Db db) => _db = db;
        public List<Producto> Productos { get; set; } = new();
        public string? Mensaje { get; set; }
        [BindProperty] public List<int> Seleccionados { get; set; } = new();

        public void OnGet() => Productos = _db.Productos();
        public void OnPost()
        {
            if (Seleccionados.Count == 0) Mensaje = "Selecciona al menos un producto para eliminar";
            else
            {
                _db.Eliminar(Seleccionados);
                Mensaje = $"Se eliminaron {Seleccionados.Count} producto/s";

            }
            Productos = _db.Productos();

        }

    }
}
