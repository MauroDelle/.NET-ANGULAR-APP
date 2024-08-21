using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using webAPI.Models;
using Microsoft.SqlServer;
using System.Data.SqlClient;
using System.Collections.Generic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace webAPI.DataBase
{
    public class EmpleadoDataBase : IDataBase<Empleado>
    {
        private SqlConnection? _connection;

        public void Conectar()
        {
            // Ajusta la cadena de conexión para confiar en el certificado del servidor
            string connectionString = @"Server=localhost\SQLEXPRESS;Database=Comanda;Trusted_Connection=True;TrustServerCertificate=True;";

            _connection = new SqlConnection(connectionString);

            try
            {
                _connection.Open();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
                _connection = null;
            }
        }

        public void Desconectar()
        {
            if (_connection != null && _connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
        }

        public void Crear(Empleado obj)
        {
            obj.FechaAlta = DateTime.Now;
            Conectar();
            try
            {
                string query = "INSERT INTO Empleado (rol, nombre, baja, clave, fecha_alta, fecha_baja) " +
                               "VALUES (@Rol, @Nombre, @Baja, @Clave, @FechaAlta, @FechaBaja)";

                SqlCommand command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Rol", obj.Rol);
                command.Parameters.AddWithValue("@Nombre", obj.Nombre);
                command.Parameters.AddWithValue("@Baja", obj.Baja);
                command.Parameters.AddWithValue("@Clave", obj.Clave);
                command.Parameters.AddWithValue("@FechaAlta", obj.FechaAlta);
                command.Parameters.AddWithValue("@FechaBaja", obj.FechaBaja.HasValue ? (object)obj.FechaBaja.Value : DBNull.Value);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al crear empleado: {ex.Message}");
            }
            finally
            {
                Desconectar();
            }
        }

        public void Modificar(Empleado obj) { }

        public void Borrar(Empleado obj)
        {
            Conectar();
            try
            {
                // First, check if the employee is already deleted
                string checkQuery = "SELECT baja FROM Empleado WHERE id = @Id";
                SqlCommand checkCommand = new SqlCommand(checkQuery, _connection);
                checkCommand.Parameters.AddWithValue("@Id", obj.Id);

                bool isAlreadyDeleted = Convert.ToBoolean(checkCommand.ExecuteScalar());

                if (isAlreadyDeleted)
                {
                    Console.WriteLine($"Empleado con ID {obj.Id} ya está eliminado.");
                    return; // Exit the method if the employee is already deleted
                }

                // Proceed with soft deletion if not already deleted
                obj.Baja = true;
                obj.FechaBaja = DateTime.Now;

                string query = "UPDATE Empleado SET baja = @Baja, fecha_baja = @FechaBaja WHERE id = @Id";
                SqlCommand command = new SqlCommand(query, _connection);
                command.Parameters.AddWithValue("@Baja", obj.Baja);
                command.Parameters.AddWithValue("@FechaBaja", obj.FechaBaja.HasValue ? (object)obj.FechaBaja.Value : DBNull.Value);
                command.Parameters.AddWithValue("@Id", obj.Id);

                command.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Error al borrar empleado: {ex.Message}");
            }
            finally
            {
                Desconectar();
            }
        }

        public Empleado ObtenerPorId(int id)
        {
            Conectar();
            
            Empleado empleado = null;

            string query = "SELECT * FROM Empleado WHERE ID = @ID";

            SqlCommand command = new SqlCommand(query, _connection);    

            command.Parameters.AddWithValue("id", id);

            using(var reader = command.ExecuteReader())
            {
                if(reader.Read())
                {
                    empleado = new Empleado
                    {
                        Id = (int)reader["ID"],
                        Rol = reader["ROL"].ToString(),
                        Nombre = reader["NOMBRE"].ToString(),
                        Baja = (bool)reader["baja"],
                        Clave = reader["clave"].ToString(),
                        FechaAlta = (DateTime)reader["FECHA_ALTA"],
                        FechaBaja = reader["FECHA_BAJA"] != DBNull.Value ? (DateTime)reader["FECHA_BAJA"] : null
                    };
                }
            }

            Desconectar();

            return empleado;
        }

        public List<Empleado> ObtenerTodos()
        {
            Conectar();
            List<Empleado> empleados = new List<Empleado>();

            string query = "SELECT * FROM Empleado";
            SqlCommand command = new SqlCommand(query, _connection);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    empleados.Add(new Empleado
                    {
                        Id = (int)reader["id"],
                        Rol = reader["rol"].ToString(),
                        Nombre = reader["nombre"].ToString(),
                        Baja = (bool)reader["baja"],
                        Clave = reader["clave"].ToString(),
                        FechaAlta = (DateTime)reader["fecha_alta"],
                        // Conversión explícita para manejar DateTime?
                        FechaBaja = reader["fecha_baja"] != DBNull.Value ? (DateTime?)reader["fecha_baja"] : null
                    });
                }
            }

            Desconectar();
            return empleados;
        }


    }
}



