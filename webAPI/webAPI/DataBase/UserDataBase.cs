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
    public class UserDataBase
    {
        private  string _connectionString = @"Server=localhost\SQLEXPRESS;Database=Comanda;Trusted_Connection=True;TrustServerCertificate=True;";

        public User? GetUserByUsername(string username)
        {
            User? user = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT * FROM [User] WHERE Username = @Username";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Username", username);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        user = new User
                        {
                            Id = (int)reader["Id"],
                            Username = reader["Username"].ToString(),
                            Password = reader["Password"].ToString()
                        };
                    }
                }
            }

            return user;
        }
    }
}
