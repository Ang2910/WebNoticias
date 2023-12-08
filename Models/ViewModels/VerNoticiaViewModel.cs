using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebNoticias.Models.ViewModels
{
    public class VerNoticiaViewModel
    {  
        public uint Id { get; set; }
        public string Titulo { get; set; } = null!;
        public string Caption { get; set; } = null!;
        public string Contenido { get; set; } = null!;
        public DateOnly Fecha { get; set; }
        public string Categoría { get; set; } = null!;
        public ulong Vistas { get; set; }
        public IEnumerable<OtrasNoticiasModel>? OtrasNoticias { get; set; }
    }
    public class OtrasNoticiasModel
    {
        public uint Id { get; set; }
        public string Titulo { get; set; } = null!;
        public DateOnly Fecha { get; set; }
        public string Categoria { get; set; } = null!;

    }
}