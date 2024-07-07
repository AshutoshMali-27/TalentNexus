using BAL1.Models;
using DAL1;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.SqlClient;

namespace TalentNexus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {

        private readonly DapperContext _context;
        public StudentController(DapperContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("SetAllStudent")]
        public async Task<ActionResult> AddProduct(students objstudent)
        {
            bool isAdded = await AddProductAsync(objstudent);

            if (isAdded)
            {
                // Return a success response
                return Ok("Product added successfully.");
            }
            else
            {
                // Return an error response
                return BadRequest("Failed to add product.");
            }

        }
        private async Task<bool> AddProductAsync(students objstudent)
        {

            using (var connection = _context.CreateConnection())
            {
                using (var command = new SqlCommand("addstudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", objstudent.id);
                    command.Parameters.AddWithValue("@Name", objstudent.Name);
                    command.Parameters.AddWithValue("@Age", objstudent.Age);
                    command.Parameters.AddWithValue("@Gender", objstudent.Gender);

                    await connection.OpenAsync();
                    int i = await command.ExecuteNonQueryAsync();
                    connection.CloseAsync();
                    if (i > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
        }

        [HttpGet]
        [Route("GetAllStudent")]
        public async Task<ActionResult<IEnumerable<students>>> GetAllStudents()
        {
            var students = new List<students>();
            using (var connection = _context.CreateConnection())
            {
                using (var command = new SqlCommand("Sp_StudentDetails", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            students.Add(new students
                            {
                                id = reader.GetInt32("id"),
                                Name = reader.GetString("Name"),
                                Gender = reader.GetString("Gender"),
                                Age=reader.GetInt32("Age")
                            });
                        }
                    }
                }
            }
            return Ok(students);
        }

        [HttpGet]
        [Route("getAllStudentsBYID/{id}")]
        public async Task<ActionResult<students>> getAllStudentsBYID(int id)
    {
            students objstudent = null;
        using (var connection = _context.CreateConnection())
        {
            using (var command = new SqlCommand("Sp_StudentDetailsByID", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Id", id);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                            objstudent = new students
                            {
                            id = reader.GetInt32("id"),
                            Name = reader.GetString("Name"),
                            Age = reader.GetInt32("Age"),
                            Gender=reader.GetString("Gender")
                        };
                    }
                }
            }
        }
        if (objstudent == null)
        {
            return NotFound();
        }
        return Ok(objstudent);
    }

        [HttpPut]
        [Route("updateStudentsByID/{id}")]
        public async Task<IActionResult> UpdateStudentt(int id, students objstudent)
        {
            if (id != objstudent.id)
            {
                return BadRequest();
            }
            using (var connection = _context.CreateConnection())
            {
                using (var command = new SqlCommand("SP_UpdateStudents", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", objstudent.id);
                    command.Parameters.AddWithValue("@Name", objstudent.Name);
                    command.Parameters.AddWithValue("@Age", objstudent.Age);
                    command.Parameters.AddWithValue("@Gender", objstudent.Gender);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
            return NoContent();
        }

        [HttpDelete]
        [Route("DeleteStudentsByID/{id}")]
        public async Task<IActionResult> DeleteStudentsByID(int id)
        {
            using (var connection = _context.CreateConnection())
            {
                using (var command = new SqlCommand("SP_deleteStudent", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id", id);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
            return NoContent();
        }
    }


    
}
