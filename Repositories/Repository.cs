using Microsoft.EntityFrameworkCore;
using WebNoticias.Models.Entities;

namespace WebNoticias.Repositories
{
    public class Repository<T> where T:class
    {  
        private readonly ProyectonoticiasContext Context; 
        public Repository(ProyectonoticiasContext context) 
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public virtual void Insert(T entity)
        {
            Context.Add(entity);
            Context.SaveChanges(); 
        }
        public virtual IEnumerable<T> GetAll() 
        { 
            return Context.Set<T>();
        }
        public virtual T? Get(object id)
        {
            if (id is uint uintId)   
            {
                // Convertir uint a int antes de buscar
                return Context.Find<T>((int)uintId);
            }

            // Manejar otros casos si es necesario...

            return null;
        }
        public virtual void Insert(Noticias entity)
        {
            // Asegúrate de que la propiedad 'Imagen' tenga un valor predeterminado
            // o asigna un valor por defecto si es posible.
            if (entity.Imagen == null) 
            {
                entity.Imagen = "/uploads/no_photo.png"; // Asigna la ruta de la imagen predeterminada
            }

            // Verifica que la categoría exista antes de intentar agregar la noticia
            var categoriaExistente = Context.Categorias.Find(entity.IdCategoria);
            if (categoriaExistente != null)
            {
                Context.Add(entity);
                Context.SaveChanges();
            }
            else
            {
                // Manejo de error: La categoría no existe
                // Puedes lanzar una excepción, loggear un mensaje de error, etc.
            }
        }

        public virtual void Update(T entity) 
        {
            Context.Update(entity); 
            Context.SaveChanges(); 
        }
        public virtual void Delete(T entity) 
        {
            Context.Remove(entity);
            Context.SaveChanges();
        }
        public virtual void Delete(object id) 
        {
            var entity = Get(id);
            if(entity != null) 
            {
                Delete(entity);
            }
        }
    }
}