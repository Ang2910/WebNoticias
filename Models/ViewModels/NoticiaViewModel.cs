using System.Security.Policy;
using WebNoticias.Models.Entities;

namespace WebNoticias.Models.ViewModels
{
    public class NoticiaViewModel
    {
        public IEnumerable<NoticiasModel> ListaNoticias { get; set; } = null!;
        public IEnumerable<Noticias> Visitadas { get; set; } = null!;
        public bool MostrarNoticiasVisitadas { get; set; } = false;
       

        
    }

    public class NoticiasModel
    {
        public uint NoticiaId { get; set; }
        public string Titulo { get; set; } = null!;
        public DateOnly Fecha { get; set; }
        public string Categoría { get; set; } = null!;
    }
    public class NoticiaVisitadasModel
    {
        public int NoticiaId { get; set; }
        public string Nombre { get; set; } = "";
        public string Categoria { get; set; } = null!;
    }
    
} 