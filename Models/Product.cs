using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DefaultProject.Models
{
    public class Product
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal Precio { get; set; }
        public DateTime FechaCreacion { get; set; }

        public Guid UsuarioId { get; set; }
    }
}
