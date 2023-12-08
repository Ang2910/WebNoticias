using WebNoticias.Models.Entities;

namespace WebNoticias.Areas.admin.Models
{
	public class AdminAgregarViewModel
	{

		public Noticias Noticia { get; set; } = null!;

		public IEnumerable<Categorias> Categorias { get; set; } = null!; 
		           
		public IFormFile? Archivo { get; set; }

		 
       
    }
}
