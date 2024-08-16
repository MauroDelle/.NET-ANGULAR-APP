
namespace webAPI.Models
{
    public class Empleado
    {
        public int Id { get; set; }
        public string Rol { get; set; }
        public string Nombre { get; set; }
        public bool Baja { get; set; }
        public string Clave { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }

        public Empleado() { }

    }
}
