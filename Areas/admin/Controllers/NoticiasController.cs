using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using WebNoticias.Areas.admin.Models;
using WebNoticias.Models.Entities;
using WebNoticias.Repositories;
 
namespace WebNoticias.Areas.admin.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Area("admin")]
    public class NoticiasController : Controller
    {

        public CategoriasRepository CategoriasRepository { get; }

        public NoticiasRepository NoticiasRepository { get; }
            
        public IWebHostEnvironment Environment { get; }

        public NoticiasController(CategoriasRepository categoriasRepository, NoticiasRepository noticiasRepository, IWebHostEnvironment environment)
        {
            CategoriasRepository = categoriasRepository;
            NoticiasRepository = noticiasRepository;
            Environment = environment;

        }

        // Dentro de NoticiasController en NoticiasController.cs
        public IActionResult Index(string categoria)
        {
            AdminNoticiasViewModel vm = new();
            if (string.IsNullOrWhiteSpace(categoria) || categoria == "0")
            {
                // Obtener todas las categorías si no hay categoria seleccionada 
                vm.Categorias = CategoriasRepository.GetAll().Select(x => x.Nombre).AsEnumerable();

                // Mostrar todas las noticias
                vm.Noticias = NoticiasRepository.GetAll()
                              .Select(x => new AdminNotiModel
                              {
                                  Id = (uint)x.NoticiaId,
                                  Categoria = x.IdCategoriaNavigation.Nombre,
                                  Titulo = x.Titulo,
                                  Visitas = x.Vistas,
                                  Fecha = x.Fecha,
                                  Caption = x.Caption ?? ""
                              });
            }
            else
            {
                // Obtener todas las categorías
                vm.Categorias = CategoriasRepository.GetAll().Select(c => c.Nombre);

                // Filtrar noticias por categoría si se especifica
                vm.Noticias = NoticiasRepository.GetNoticiasPorCategorias(categoria)
                              .Select(x => new AdminNotiModel
                              {
                                  Id = (uint)x.NoticiaId,
                                  Categoria = x.IdCategoriaNavigation.Nombre,
                                  Titulo = x.Titulo,
                                  Visitas = x.Vistas,
                                  Fecha = x.Fecha,
                                  Caption = x.Caption ?? ""
                              });

            }

            return View(vm);
        }
            public IActionResult Agregar()
        {
            AdminAgregarViewModel vm = new(); 
            vm.Categorias = CategoriasRepository.GetAll().OrderBy(y => y.Nombre);
            return View(vm);
        }


        [HttpPost]
        public IActionResult Agregar(AdminAgregarViewModel vm)
        {
            vm.Categorias = CategoriasRepository.GetAll().OrderBy(c => c.Nombre);
            ModelState.Clear();     
            if (string.IsNullOrWhiteSpace(vm.Noticia.Titulo))
            {  
                ModelState.AddModelError(nameof(vm.Noticia.Titulo), "El Título es obligatorio.");
            } 
            if (vm.Noticia.Titulo.Contains('?') || vm.Noticia.Titulo.Contains('¿'))
            {
                ModelState.AddModelError(nameof(vm.Noticia.Contenido), "No se permite agregar preguntas en el titulo");
            }
            if (string.IsNullOrWhiteSpace(vm.Noticia.Contenido))
            {
                ModelState.AddModelError(nameof(vm.Noticia.Contenido), "El Contenido es obligatorio.");
            }
            if (vm.Noticia.Titulo.Length < 5 || vm.Noticia.Titulo.Length > 100)  
            {
                ModelState.AddModelError(nameof(vm.Noticia.Titulo), "La longitud del Título debe estar entre 5 y 100 caracteres.");
            }
            // Convertir DateTime.Now a DateOnly  
            DateOnly fechaActual = DateOnly.FromDateTime(DateTime.Now);   
            // Realizar la comparación
         
            if (vm.Archivo != null) 
            {
                // Genera un nombre único para el archivo usando el timestamp actual
                if (vm.Archivo.ContentType != "image/png")
                {
                    ModelState.AddModelError("", "Solo se permiten imagenes PNG.");
                }
                if (vm.Archivo.Length > 500 * 1024)
                {
                    ModelState.AddModelError("", "Solo se permiten archivos no mayores a 500kb");
                }
            }
      
                vm.Noticia.Fecha =  DateOnly.FromDateTime(DateTime.Now);
                vm.Noticia.FechaPublicacion = DateTime.Now;  

            if (ModelState.IsValid)
                {
                    NoticiasRepository.Insert(vm.Noticia);

                    if (vm.Archivo == null)
                    {
                        System.IO.File.Copy("wwwroot/logos/letra-w.png", $"wwwroot/Noticias/{vm.Noticia.NoticiaId}.png");
                    }     
                    else
                    {
                        System.IO.FileStream fs = System.IO.File.Create($"wwwroot/Noticias/{vm.Noticia.NoticiaId}.png");
                        vm.Archivo.CopyTo(fs);
                        fs.Close();
                    }

               

                return RedirectToAction("Index");
                }
             
                
            
			return View(vm); 
		}
        public IActionResult Editar(int id)
        {
            var noti = NoticiasRepository.GetById(id);

            if(noti == null)
            {
                return RedirectToAction("Index");
            }
            else 
            { 
            AdminAgregarViewModel vm = new AdminAgregarViewModel();
            vm.Noticia = noti;
             vm.Categorias = CategoriasRepository.GetAll().OrderBy(x => x.Nombre);
                return View(vm);
            }
           
        }
        [HttpPost]
        public IActionResult Editar(AdminAgregarViewModel vm)
        {
			ModelState.Clear();  
            if (string.IsNullOrWhiteSpace(vm.Noticia.Titulo))
            {
                ModelState.AddModelError(nameof(vm.Noticia.Titulo), "El Título es obligatorio.");
            }
            if (vm.Noticia.Titulo.Contains('?') || vm.Noticia.Titulo.Contains('¿'))
            {
                ModelState.AddModelError(nameof(vm.Noticia.Contenido), "No se permite agregar preguntas en el titulo");
            }
            if (string.IsNullOrWhiteSpace(vm.Noticia.Contenido))
            {
                ModelState.AddModelError(nameof(vm.Noticia.Contenido), "El Contenido es obligatorio.");
            }
            if (vm.Noticia.Titulo.Length < 5 || vm.Noticia.Titulo.Length > 100)
            {
                ModelState.AddModelError(nameof(vm.Noticia.Titulo), "La longitud del Título debe estar entre 5 y 100 caracteres.");
            }

            if (vm.Archivo != null)
			{
				if (vm.Archivo.ContentType != "image/png")
				{
					ModelState.AddModelError("", "Solo se  permiten imagenes PNG"); 
				}
				if (vm.Archivo.Length > 500 * 1024)
				{
					ModelState.AddModelError("", "Solo se permiten archivos no mayores  a 500kb");
				}
			}

			if (ModelState.IsValid)
			{
				var elemento = NoticiasRepository.GetById(vm.Noticia.NoticiaId);
				if (elemento == null)
				{
					return RedirectToAction("Index");
				}
				elemento.Titulo = vm.Noticia.Titulo;
				elemento.Caption = vm.Noticia.Caption;
				elemento.Fecha = vm.Noticia.Fecha;
                elemento.Contenido = vm.Noticia.Contenido;
				elemento.IdCategoria = vm.Noticia.IdCategoria;


				NoticiasRepository.Update(elemento);

				if (vm.Archivo != null)
				{
					System.IO.FileStream fs = System.IO.File.Create($"wwwroot/Noticias/{vm.Noticia.NoticiaId}.png");
					vm.Archivo.CopyTo(fs);
					fs.Close();
				}         
				return RedirectToAction("Index");
			}
			vm.Categorias = CategoriasRepository.GetAll().OrderBy(x => x.Nombre);

			return View(vm);

		}
        public IActionResult Eliminar(int id)
        {
            var noticia = NoticiasRepository.GetById(id);
            if(noticia == null)
            {
                RedirectToAction("Index");
            }
            return View(noticia);
        }
        [HttpPost]
        public IActionResult Eliminar(Noticias n)
        {
            var notic = NoticiasRepository.GetById(n.NoticiaId);
            if(notic == null)
            {
                return RedirectToAction("Index");
            }
            NoticiasRepository.Delete(notic);
            var imagen = $"wwwroot/Noticias/{n.NoticiaId}.png";
            if(System.IO.File.Exists(imagen))
            {
                System.IO.File.Delete(imagen);
            }
            return RedirectToAction("Index");
        }
    }

}

        
    


    


