using Login.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using System.Numerics;
using Dapper;
using Microsoft.AspNetCore.Authorization;

namespace Login.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CubeController : ControllerBase
    {

        private Vector3 _lastCubePosition;
        private readonly string _connectionString;


        public CubeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost("saveLocation")]
        [Authorize]
        public IActionResult SaveLocation([FromBody] CubeLocation location)
        {
            if (location == null)
            {
                return BadRequest("Location data is missing.");
            }

            var sqlQuery = @"
            INSERT INTO CubeLocation (X, Y, Z, SessionID)
            VALUES (@X, @Y, @Z, @SessionID);
        ";

            var parameters = new
            {
                X = location.X,
                Y = location.Y,
                Z = location.Z,
                SessionID = location.SessionID
            };

            // Execute the query using Dapper
            using (IDbConnection dbConnection = new SqlConnection(_connectionString))
            {
                dbConnection.Open();
                var rowsAffected = dbConnection.Execute(sqlQuery, parameters);

                if (rowsAffected > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cube location saved successfully!",
                        location = new { location.X, location.Y, location.Z }
                    });
                }
                else
                {
                    return StatusCode(500, new { success = false, message = "Error saving location to the database." });
                }
            }
        }
    }

   }



