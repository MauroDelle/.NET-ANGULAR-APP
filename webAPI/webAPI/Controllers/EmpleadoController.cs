using Microsoft.AspNetCore.Mvc;
using webAPI.DataBase;
using webAPI.Models;


namespace webAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmpleadoController : ControllerBase
    {
        private readonly ILogger<EmpleadoController> _logger;
        private readonly EmpleadoDataBase _empleadoDataBase;

        public EmpleadoController(ILogger<EmpleadoController> logger)
        {
            _logger = logger;
            _empleadoDataBase = new EmpleadoDataBase();
        }

        //EndPoint para obtener todos los empleados
        [HttpGet(Name = "ObtenerTodosLosEmpleados")]
        public IEnumerable<Empleado> ObtenerTodos()
        {
            return _empleadoDataBase.ObtenerTodos();
        }

        //EndPoint para obtener un empleado por ID
        [HttpGet("{id}")]
        public ActionResult<Empleado> ObtenerPorId(int id)
        {
            var empleado = _empleadoDataBase.ObtenerPorId(id);
            if(empleado == null)
            {
                return NotFound();  
            }
            return Ok(empleado);
        }

        [HttpPost(Name = "CrearEmpleado")]
        public IActionResult Crear([FromBody] Empleado nuevoEmpleado)
        {
            if (nuevoEmpleado == null)
            {
                return BadRequest("Empleado no puede ser nulo");
            }
            try
            {
                _empleadoDataBase.Crear(nuevoEmpleado);
                return CreatedAtAction(nameof(ObtenerTodos), new { id = nuevoEmpleado.Id }, nuevoEmpleado);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear empleado: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }


        [HttpDelete("{id}")]
        public IActionResult Borrar(int id)
        {
            // Retrieve the employee to check if it exists and is already deleted
            var empleado = _empleadoDataBase.ObtenerPorId(id);

            if (empleado == null)
            {
                return NotFound(new { message = "Empleado no encontrado" });
            }

            if (empleado.Baja)
            {
                return BadRequest(new { message = "Empleado ya ha sido eliminado" });
            }

            // Proceed with deletion
            _empleadoDataBase.Borrar(empleado);
            return Ok(new { message = "Empleado eliminado con éxito" });
        }



    }
}
