
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
namespace parcial1.Data;
public record Categoria(int IdCategoria, string Descripcion);
public record Producto(int IdProducto, string Nombre, decimal Precio, int IdCategoria, string Categoria);

//acceso a datos 
public class Db
{
    private readonly string connectionString;
    private System.Data.SqlDbType idProducto;
    
    public Db (IConfiguration cfg) => connectionString = cfg.GetConnectionString("Tienda")!;
    //inicializacion creamos db, tablas y datos de ejempplo

    public static void Inicializar(string cs)
    {
        var b = new SqlConnectionStringBuilder(cs);
        var nombreBd = b.InitialCatalog;
        b.InitialCatalog = "master";
        using (var c = new SqlConnection(b.ConnectionString))
        {
            c.Open();
            using (var cmd = c.CreateCommand())
            {
                // c ya está abierto
                new SqlCommand($"IF DB_ID('{nombreBd}') IS NULL CREATE DATABASE [{nombreBd}]", c).ExecuteNonQuery();
            }
            using var con = new SqlConnection(cs);
            con.Open();
            const string sql = @"
IF OBJECT_ID('Categorias') IS NULL
CREATE TABLE Categorias(IdCategoria INT IDENTITY(1,1) PRIMARY KEY, Descripcion NVARCHAR(100) NOT NULL);
IF OBJECT_ID('Productos') IS NULL
CREATE TABLE Productos(IdProducto  INT IDENTITY(1,1) PRIMARY KEY, Nombre NVARCHAR(100) NOT NULL, Precio DECIMAL(10,2) NOT NULL, IdCategoria INT NOT NULL FOREIGN KEY REFERENCES Categorias(IdCategoria));
IF NOT EXISTS(SELECT 1 FROM Categorias) INSERT INTO Categorias(Descripcion) VALUES(N'Notebooks'),(N'Perifericos'),(N'Celulares');
IF NOT EXISTS(SELECT 1 FROM Productos) INSERT INTO Productos(Nombre, Precio, IdCategoria) VALUES('Notebook HP', 1000, 1),('Mouse Logitech', 50, 2),('Celular Samsung', 500, 3);";
            new SqlCommand(sql, con).ExecuteNonQuery();
        }
    }

    private List<T> Query<T>(string sql, Func<SqlDataReader, T> map, params SqlParameter[] ps)
    {
        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddRange(ps);
        con.Open();
        using var reader = cmd.ExecuteReader();
        var list = new List<T>();
        while (reader.Read())
            list.Add(map(reader));
        return list;
    }

    private void Execute(string sql, params SqlParameter[] ps)
    {
        using var con = new SqlConnection(connectionString);
        using var cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddRange(ps);
        con.Open();
        cmd.ExecuteNonQuery();
    }
    //consultas con join
    private const string Join= @"SELECT p.IdProducto, p.Nombre, p.Precio, c.IdCategoria, c.Descripcion FROM Productos p INNER JOIN Categorias c ON p.IdCategoria = c.IdCategoria";
    private static Producto MapProducto(SqlDataReader r) => new(r.GetInt32(0), r.GetString(1), r.GetDecimal(2), r.GetInt32(3), r.GetString(4));
    public List<Categoria> Categorias() => Query("SELECT IdCategoria, Descripcion FROM Categorias", r => new Categoria(r.GetInt32(0), r.GetString(1)));
    public List<Producto> Productos() => Query(Join, MapProducto);
    public Producto? ProductoPorId(int id) => Query($"{Join} WHERE p.IdProducto=@id", MapProducto, new SqlParameter("@id", id)).FirstOrDefault();

    //Alta , baja y modificacion de productos
    public void Insertar(string nombre, decimal precio, int idCategoria) => Execute("INSERT INTO Productos(Nombre, Precio, IdCategoria) VALUES(@nombre, @precio, @idCategoria)", new SqlParameter("@nombre", nombre), new SqlParameter("@precio", precio), new SqlParameter("@idCategoria", idCategoria));
    public void Actualizar(int idProducto, string nombre, decimal precio, int idCategoria) => Execute("UPDATE Productos SET Nombre=@nombre, Precio=@precio, IdCategoria=@idCategoria WHERE IdProducto=@idProducto", new SqlParameter("@idProducto", idProducto), new SqlParameter("@nombre", nombre), new SqlParameter("@precio", precio), new SqlParameter("@idCategoria", idCategoria));
    public void Eliminar(IEnumerable<int> ids) { 
        foreach (var id in ids)
         Execute("DELETE FROM Productos WHERE IdProducto=@idProducto", new SqlParameter("@idProducto", id)); 
    }
}


