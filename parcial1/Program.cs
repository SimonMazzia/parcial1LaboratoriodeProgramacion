using parcial1.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<Db>(new Db(builder.Configuration));


var app = builder.Build();
Db.Inicializar(builder.Configuration.GetConnectionString("Tienda")!);
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.Run();