using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using parcial1.Data;

namespace parcial1.Pages.Productos
{
    public class AltaModel : PageModel
    {
        private readonly Db _db;
        public AltaModel(Db db) => _db = db;
        public List<Categoria> Categorias { get; set; } = new();
        public string? Mensaje { get; set; }

        [BindProperty, Required(ErrorMessage = "El nombre es obligatorio"), StringLength(100)]
        public string Nombre { get; set; } = "";
        [BindProperty, Required(ErrorMessage = "El precio es obligatorio"), Range(0.01, 10000, ErrorMessage = "El precio debe estar entre 0.01 y 10000")]
        public decimal Precio { get; set; }
        [BindProperty, Required(ErrorMessage = "La categoría es obligatoria")]
        public int IdCategoria { get; set; }

        public void OnGet() => Categorias = _db.Categorias();

        public void OnPost()
        {
            Categorias = _db.Categorias();
            if (!ModelState.IsValid) return;
            _db.Insertar(Nombre.Trim(),Precio, IdCategoria);
            Mensaje=$"Producto \"{Nombre.Trim()}\" agregado correctamente";
            ModelState.Clear();
            Nombre = ""; Precio= 0 ; IdCategoria = 0;


        }

    }
}
