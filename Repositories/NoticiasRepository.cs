using Microsoft.EntityFrameworkCore;
using System.Collections;
using WebNoticias.Models.Entities;
using WebNoticias.Models.ViewModels;
  
namespace WebNoticias.Repositories
{
    public class NoticiasRepository:Repository<Noticias> 
    {

        ProyectonoticiasContext context { get; }
        public NoticiasRepository(ProyectonoticiasContext context):base(context) 
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public override IEnumerable<Noticias> GetAll()
        {
            if (context != null && context.Noticias != null)
            {
                return context.Noticias.Include(n => n.IdCategoriaNavigation).OrderByDescending(n => n.FechaPublicacion);
            }
            else
            {
                // Puedes lanzar una excepción, devolver una lista vacía u otro comportamiento según tus necesidades.
                return Enumerable.Empty<Noticias>();
            }
        }
        public Noticias? GetById(int id) 
        {
            return context.Noticias.Find(id);  
        }
        public Noticias? GetId(int id) 
        {
            return context.Noticias.Include(n => n.IdCategoriaNavigation).FirstOrDefault(n => n.NoticiaId == id);
        }
        public IEnumerable<Noticias> GetNoticiaByTitulo(string noticia)
        {
            return context.Noticias
                .Where(x => x.Titulo == noticia)
                .OrderBy(x => x.Titulo);
        }
        public IEnumerable<Noticias> GetNoticiaByCategoria(string categoria)
        {
            return context.Noticias.Include(x => x.IdCategoriaNavigation)
                .Where(x => x.IdCategoriaNavigation != null && x.IdCategoriaNavigation.Nombre == categoria)
                .OrderBy(x => x.Titulo);      
        }                  
        public Noticias? GetNoticiaByTituloYCategoria(string titulo, string categoria)
        {
            //return context.Noticias
            //    .Include(n => n.IdCategoriaNavigation)
            //    .AsEnumerable()  // Traer datos a memoria
            //    .Where(n => n.Titulo.Equals(titulo, StringComparison.OrdinalIgnoreCase) &&
            //                n.IdCategoriaNavigation != null &&
            //                n.IdCategoriaNavigation.Nombre != null &&
            //                n.IdCategoriaNavigation.Nombre.Equals(categoria, StringComparison.OrdinalIgnoreCase))
            //    .OrderBy(n => n.Titulo)
            //    .ToList();

            if (categoria != null)
            {
                var noticia = context.Noticias.Include(n => n.IdCategoriaNavigation)
                    .Where(n => n.Titulo == titulo && n.IdCategoriaNavigation.Nombre == categoria)
                    .FirstOrDefault();
                if (noticia != null)
                {
                    return noticia;
                }
            }
            return null;
        } 
        public IEnumerable<Noticias> GetNoticiasPorCategorias(string categoria)
        {
            return context.Noticias
                .Where(x => x.IdCategoriaNavigation.Nombre == categoria)
                .OrderBy(x => x.Titulo); 
        }

        //public IEnumerable<Noticias> BuscarPorTermino(string termino)
        //{
        //    return context.Noticias
        //        .Where(x => x.Titulo.Contains(termino) || x.Contenido.Contains(termino))
        //        .OrderByDescending(x => x.FechaPublicacion);
        //}

        public IEnumerable<Noticias> GetMasVisitadas() 
        { 
            return context.Noticias.Include(x=>x.IdCategoriaNavigation)
            .Where(x=>x.Vistas>50)
            .Take(100)
            .OrderByDescending(x => x.Vistas);
        }
        public Noticias? GetByTitulo(string titulo)
        {
            return context.Noticias.Include(x => x.IdCategoriaNavigation).FirstOrDefault(x => x.Titulo == titulo);
        }
        
    }
}

    
