using System;
using System.Collections.Generic;

namespace WebNoticias.Models.Entities;

public partial class Categorias
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Noticias> Noticias { get; set; } = new List<Noticias>();
}
