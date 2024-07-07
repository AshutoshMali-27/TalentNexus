using BAL1.Models;
using DAL1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;

namespace TalentNexus.Auth
{
    public class AuthService
    {
        //private readonly DapperContext _context;
        //public AuthService(DapperContext context)
        //{
        //    _context = context;
        //}



        private  string connectionString = "Data Source=LAPTOP-92R3ST6F\\SQLEXPRESS;Initial Catalog=StudentsDB;Integrated Security=True;";

        public users GetUser(string username, string password)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "SELECT * FROM Users WHERE Username = @Username AND Password = @Password";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new users
                            {
                                Id = reader.GetInt32(0),
                                Username = reader.GetString(1),
                                Password = reader.GetString(2),
                                Role = reader.GetString(3)
                            };
                        }
                    }
                }
            }
            return null;
        }


        public bool CreateUser(users user)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "INSERT INTO Users (Username, Password, Role) VALUES (@Username, @Password, @Role)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", user.Username);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@Role", user.Role);

                    return command.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
