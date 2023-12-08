using Microsoft.EntityFrameworkCore;
using System.Collections;
using WebNoticias.Models.Entities;

namespace WebNoticias.Repositories
{
    public class CategoriasRepository:Repository<Categorias>
    {
        ProyectonoticiasContext context; 
        public CategoriasRepository(ProyectonoticiasContext context):base(context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override IEnumerable<Categorias> GetAll() 
        {
            if (context != null && context.Categorias != null)
            {
                return context.Categorias.Include(x => x.Noticias).OrderBy(x => x.Nombre).ToList();
            }
            else
            {
                // Puedes lanzar una excepción, devolver una lista vacía u otro comportamiento según tus necesidades.
                return Enumerable.Empty<Categorias>();
            }
        }
        public IEnumerable<Categorias> GetNoticiasByCategorias(string categoria)
        {
            return context.Categorias
                .Where(x => x.Noticias != null && x.Nombre == categoria)
                .OrderBy(x => x.Nombre);
        }
        public Categorias? GetByNoticia(string nombre)
        {
            return context.Categorias.FirstOrDefault(x => x.Nombre == nombre);
        }
    }
}