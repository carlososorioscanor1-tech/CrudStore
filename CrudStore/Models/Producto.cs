using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRUDTablas.Models
{
    public class Producto
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public decimal Precio { get; set; }

        public int CategoriaId { get; set; }

        // PROPIEDAD DE NAVEGACIÓN CORREGIDA
        // Eliminamos la propiedad duplicada 'Categorias' y dejamos únicamente 'Categoria'
        // Además, le indicamos explícitamente cuál es su llave foránea para sanar el error anterior
        [ForeignKey("CategoriaId")]
        public virtual Categoria Categoria { get; set; } = null!;
    }
}
