using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Security.Claims;
using WebNoticias.Helpers;
using WebNoticias.Models.Entities;
using WebNoticias.Models.ViewModels;
using WebNoticias.Repositories;

namespace WebNoticias.Controllers
{
    public class HomeController : Controller
    {
        ProyectonoticiasContext Context { get; set; }
        public NoticiasRepository NoticiasRepository { get; }

        private Repository<Usuario> UserRepository;
        public CategoriasRepository CategoriasRepository { get; }
        public HomeController(NoticiasRepository noticiasRepository, CategoriasRepository categoriasRepository, ProyectonoticiasContext context, Repository<Usuario> userRepos)
        {
            Context = context;
            NoticiasRepository = noticiasRepository;
            CategoriasRepository = categoriasRepository;
            UserRepository = userRepos;
        } 
        public IActionResult Index(string id)
        {
            if (id != null)
            {
                id = id.Replace('-', ' ');
            }
            if (HttpContext.User != null)    
            {
                if(HttpContext.User.Identity != null)
                {
                    if(!HttpContext.User.Identity.IsAuthenticated) 
                    {
                           return RedirectToAction("Login"); 
                    }
                       
                }      
            }  
            var noticias = NoticiasRepository.GetAll();
            if (!string.IsNullOrEmpty(id))
            {
                noticias = NoticiasRepository.GetNoticiaByCategoria(id);
            }
            var listaNoti = noticias.Select(y => new NoticiasModel
            {
                NoticiaId = (uint)y.NoticiaId,
                Titulo = y.Titulo ?? "",
                Fecha = y.Fecha,
                Categoría = y.IdCategoriaNavigation.Nombre ?? "Sin categoria"

            });
            var vm = new NoticiaViewModel
            {
                ListaNoticias = listaNoti,
                Visitadas = NoticiasRepository.GetMasVisitadas(),
                MostrarNoticiasVisitadas = string.IsNullOrEmpty(id)
            };
           

            return View(vm);
        }
        //[HttpGet]
        //public IActionResult Search(string searchTerm)
        //{
        //    // Lógica para realizar la búsqueda en el servidor
        //    var resultados = NoticiasRepository.BuscarPorTermino(searchTerm); 

        //    // Devolver una vista parcial con los resultados de la búsqueda
        //    return PartialView("_SearchResults", resultados);
        //}
        [HttpGet]
        public IActionResult NoticiaPorCategoria(string categoria)
        {
            var noticias = NoticiasRepository.GetNoticiasPorCategorias(categoria);

            var viewmodel = new NoticiaViewModel
            {
                ListaNoticias = noticias.Select(n => new NoticiasModel
                {

                    NoticiaId = (uint)n.NoticiaId,
                    Categoría = n.IdCategoriaNavigation?.Nombre ?? "",
                    Titulo = n.Titulo,
                    Fecha = n.Fecha

                })
            };
            return View("Index", viewmodel);
        }

        [Route("/VerNoticia/{categoria}/{id}")]
        public IActionResult VerNoticia(string categoria, string id)
        {
            id = id.Replace('-', ' ');
            categoria = categoria.Replace('-', ' ');
            var noticias = NoticiasRepository.GetNoticiaByTituloYCategoria(id, categoria);
            if (noticias == null)
            {
                return RedirectToAction("Index");
            }
            var noticia = noticias;
            noticia.Vistas++;
            Context.SaveChanges();
            VerNoticiaViewModel vm = new()
            {
                Id = (uint)noticia.NoticiaId,
                Titulo = noticia.Titulo,
                Caption = noticia.Caption ?? "Sin descripción", // Maneja el caso en que Caption sea nulo
                Contenido = noticia.Contenido,
                Fecha = noticia.Fecha,
                Categoría = noticia.IdCategoriaNavigation?.Nombre ?? "Sin categoría",

                Vistas = noticia.Vistas,
                OtrasNoticias = ObtenerNoticiasRandom(noticia)
            };
            return View(vm);
        }
        Random r = new();
        List<OtrasNoticiasModel> otralista { get; set; } = new();
        public IEnumerable<OtrasNoticiasModel> ObtenerNoticiasRandom(Noticias n)
        {
            //Lista sin Random
            var lista = NoticiasRepository.GetAll().Where(x => x.NoticiaId != n.NoticiaId).Take(3).Select(x => new OtrasNoticiasModel { Id = (uint)x.NoticiaId, Fecha = x.Fecha, Titulo = x.Titulo, Categoria = x.IdCategoriaNavigation?.Nombre ?? "" }).ToList();
            int numlista = lista.Count();
            if (numlista > 0)
            {
                for (int i = 0; i < numlista; i++)
                {
                    int rand = r.Next(0, numlista);
                    var on = lista[rand];
                    if (!otralista.Contains(on))
                    {
                        otralista.Add(on);
                    }
                    else
                    {
                        i--;
                    }
                }
                return otralista.AsEnumerable();
            }
            return null;
        }
        public IActionResult LogOut()
        {

            HttpContext.SignOutAsync();
              
            return RedirectToAction("Login", "Home");

        }
        public IActionResult Denied()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View(); 
        }
        [HttpPost] 
        public IActionResult Login(LoginViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Correo))
            {
                ModelState.AddModelError("", "Escriba el correo electronico del usuario");
            }
            if (string.IsNullOrWhiteSpace(vm.Contrasena))
            {
                ModelState.AddModelError("", "Escriba la contrasena del usuario");
            }

            if (ModelState.IsValid)
            {
               var user = UserRepository.GetAll().FirstOrDefault(x => x.Correo == vm.Correo && x.Contraseña == Encriptacion.StringToSHA512(vm.Contrasena));

                if (user == null)
                {
                    ModelState.AddModelError("", "Contrasena o correo electronico incorrectos.");
                }
                else 
                {
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim("Id", user.UsuarioId.ToString()));  //Nombre Clamin|Valor claim  (Todo string)
                    claims.Add(new Claim(ClaimTypes.Name, user.Nombre));
                    claims.Add(new Claim(ClaimTypes.Role, user.IdRol == 1 ? "Administrador" : "Cliente"));

                    ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme); //Se ponen los datos y se forma una credencial.
                    HttpContext.SignInAsync(new ClaimsPrincipal(identity), new AuthenticationProperties
                    {
                        IsPersistent = true,  //Login persistente 
                    });
                   if(user.IdRol==1) 
                    {
                   return RedirectToAction("Index", "Home",  new { area = "admin" });
                    }
                   else 
                    {
                        return RedirectToAction("Index");  
                    }     
                }
            }
            return View(vm);
        }
    }
    
}
