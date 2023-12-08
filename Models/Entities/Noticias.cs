using System;
using System.Collections.Generic;

namespace WebNoticias.Models.Entities;

public partial class Noticias
{
    public int NoticiaId { get; set; }

    public string Titulo { get; set; } = null!;

    public string? Caption { get; set; }

    public string Imagen { get; set; } = null!;

    public string Contenido { get; set; } = null!;

    public DateOnly Fecha { get; set; }

    public DateTime FechaPublicacion { get; set; }

    public ulong Vistas { get; set; }

    public int? AutorId { get; set; }

    public int IdCategoria { get; set; }

    public virtual Usuario? Autor { get; set; }

    public virtual Categorias IdCategoriaNavigation { get; set; } = null!;
}
