namespace WebNoticias.Areas.admin.Models
{
    public class CategoriaViewModel
    {
        public IEnumerable<CategoriaModel> Categorias { get; set; } = null!;

    }

    public class CategoriaModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int TotalNoticias { get; set; }
    }
}
