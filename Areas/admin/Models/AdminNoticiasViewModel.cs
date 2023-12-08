namespace WebNoticias.Areas.admin.Models
{
    public class AdminNoticiasViewModel
    {
        public IEnumerable<AdminNotiModel> Noticias { get; set; }

        public IEnumerable<string> Categorias { get; set; }  
    }
    public class AdminNotiModel
    {
        public uint Id { get; set; }
        public string Titulo { get; set; } = null!;
        public DateOnly Fecha { get; set; }
        public string Contenido { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public ulong Visitas { get; set; }
        public string Caption { get; set; } = null!;
    }

}
