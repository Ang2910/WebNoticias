using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebNoticias.Areas.admin.Models;
using WebNoticias.Models.Entities;
using WebNoticias.Repositories;

namespace WebNoticias.Areas.admin.Controllers
{
    [Authorize(Roles = "Administrador")] 
    [Area("admin")]
    public class CategoriasController : Controller
    {
        public CategoriasRepository Repository { get; }

        public NoticiasRepository NoticiasRepository { get; }
        public CategoriasController(CategoriasRepository repository, NoticiasRepository noticiasRepository)
        {
            Repository = repository;
            NoticiasRepository = noticiasRepository;
        }

        public IActionResult Index()
        {
            CategoriaViewModel vm = new();
            vm.Categorias = Repository.GetAll().Select(x => new CategoriaModel
            {
                Id = x.Id,
                Nombre = x.Nombre,
                TotalNoticias = x.Noticias.Count
            });


            return View(vm);
        }
        public IActionResult Agregar()
        {
            return View();
        }
         
        [HttpPost]
        public IActionResult Agregar(AdminAgregarCategoriaModel vm)
        {     
            if (string.IsNullOrWhiteSpace(vm.Nombre))
            {
                ModelState.AddModelError("", "Escribe el nombre de la categoria");
            }
            else if (vm.Nombre.Length > 50)
            {
                ModelState.AddModelError(nameof(vm.Nombre), "El nombre de la categoría no puede tener más de 50 caracteres.");
            }
            if (Repository.GetAll().Any(x => x.Nombre == vm.Nombre))
            {
                ModelState.AddModelError(nameof(vm.Nombre), "Ya existe una sección con este nombre.");
            }
            if (ModelState.IsValid)
            {
                var nuevaSeccion = new Categorias
                {
                    Nombre = vm.Nombre ?? ""
                };
                Repository.Insert(nuevaSeccion);
                return RedirectToAction("Index");
            }
            return View(vm);

        }

        public IActionResult Editar(uint id)
        {
            var categoria = Repository.Get(id);
            if (categoria == null)
            {
                return RedirectToAction("Index");
            }
            return View(categoria);
        }
        [HttpPost]
        public IActionResult Editar(Categorias c)
        {
            if (string.IsNullOrWhiteSpace(c.Nombre))
            {
                ModelState.AddModelError("", "Especifique el nombre de la sección");
            }
            if (string.IsNullOrWhiteSpace(c.Nombre))
            {
                ModelState.AddModelError(nameof(c.Nombre), "Escribir el nombre de la seccion.");
            }
            if (string.IsNullOrWhiteSpace(c.Nombre))
            {
                ModelState.AddModelError(nameof(c.Nombre), "El nombre de la categoría es obligatorio.");
            }
            if (c.Nombre.Contains("PalabraNoPermitida"))
            {
                ModelState.AddModelError(nameof(c.Nombre), "El nombre contiene palabras no permitidas.");
            }
            if (c.Nombre.Length > 50)
            {
                ModelState.AddModelError(nameof(c.Nombre), "El nombre de la categoría no puede tener más de 50 caracteres.");
            }

            if (ModelState.IsValid)
            {
                var categoria = Repository.Get((uint)c.Id);

                if (categoria == null)
                {
                    return RedirectToAction("Index");
                }

                categoria.Nombre = c.Nombre ?? "";
                Repository.Update(categoria);

                return RedirectToAction("Index");
            }

            // Si el modelo no es válido, regresa a la vista con el modelo para mostrar los errores.
            return View(c);
        }

        public IActionResult Eliminar(uint id)
        {
            var categoria = Repository.Get(id); 
            if (categoria == null)
            {
                return RedirectToAction("Index");
            }
            return View(categoria);
        }
        [HttpPost]
        public IActionResult Eliminar(Categorias c)
        {       
            // Obtén la categoría actual
            var categoria = Repository.Get((uint)c.Id);

            if (categoria == null)
            {
                return RedirectToAction("Index");
            }
            //foreach (var noticia in categoria.Noticias)
            //{
            //    NoticiasRepository.Delete(noticia);
            //}
            if (categoria.Noticias.Count == 0)
            {
                Repository.Delete(categoria);
                return RedirectToAction("Index");
            }
            return View(c);
        }    
    }
 }