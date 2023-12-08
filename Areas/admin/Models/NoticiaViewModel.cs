using System.Collections;
using WebNoticias.Models.Entities;

namespace WebNoticias.Areas.admin.Models
{
    public class NoticiaViewModel
    {

        public Noticias Noticia { get; set; } = null!;

        public IEnumerable<CategoriaModel> Categorias { get; set; } = null!;

        public IFormFile? Archivo { get; set; }

    }
}
