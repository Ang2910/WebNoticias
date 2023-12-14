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
               
                return Context.Find<T>((int)uintId);
            }

          

            return null;
        }
        public virtual void Insert(Noticias entity)
        {
            
            if (entity.Imagen == null) 
            {
                entity.Imagen = "/uploads/no_photo.png"; 
            }

       
            var categoriaExistente = Context.Categorias.Find(entity.IdCategoria);
            if (categoriaExistente != null)
            {
                Context.Add(entity);
                Context.SaveChanges();
            }
            else
            {
                
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