using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using parcial1.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace parcial1.Pages.Productos;
public class ModificarModel : PageModel
{
    private readonly Db _db;
    public ModificarModel(Db db) => _db = db;
    public List<Producto> Productos { get; set; } = new();
    public List<Categoria> Categorias { get; set; } = new();
    public string? Mensaje { get; set; }
    public string? MensajeError { get; set; }
    [BindProperty(SupportsGet = true)] public int? Id { get; set; }
    [BindProperty, Required(ErrorMessage = "El nombre es obligatorio"),
    StringLength(100)]
    public string Nombre { get; set; } = "";
    [BindProperty, Range(0.01, 99999999, ErrorMessage = "Ingrese un precio válido")]
public decimal Precio { get; set; }
    [BindProperty, Range(1, int.MaxValue, ErrorMessage = "Seleccione una categoría")]
public int IdCategoria { get; set; }
    private void Cargar()
    {
        Productos = _db.Productos(); Categorias =
    _db.Categorias();
    }
    public void OnGet()
    {
        Cargar();
        if (Id is int id && _db.ProductoPorId(id) is Producto p)
        { Nombre = p.Nombre; Precio = p.Precio; IdCategoria = p.IdCategoria; }
    }
    public IActionResult OnPost()
    {
        Cargar();
        if (Id is not int id || _db.ProductoPorId(id) is not Producto original) return
        RedirectToPage();
        if (!ModelState.IsValid) return Page();
        // Validar que al menos un campo cambió

        var nombre = Nombre.Trim();
        if (nombre == original.Nombre && Precio == original.Precio && IdCategoria
        == original.IdCategoria)
        {
            MensajeError = "No modificaste ningún campo. Cambiá al menos uno antes de guardar.";
        return Page();
        }
        _db.Actualizar(id, nombre, Precio, IdCategoria);
        Productos = _db.Productos();
        Mensaje = "Producto actualizado correctamente.";
        return Page();
    }
}