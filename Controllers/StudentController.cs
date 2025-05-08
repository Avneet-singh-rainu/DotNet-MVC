using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Test.Models;

namespace Test.Controllers {
    public class StudentController : Controller {
        private readonly IConfiguration _configuration;

        // Constructor with dependency injection for IConfiguration
        public StudentController ( IConfiguration configuration ) {
            _configuration = configuration;
        }

        public JsonResult Login () {
            // Retrieve the connection string from the appsettings.json
            string connectionString = _configuration.GetConnectionString( "Database" );

            List<StudentModel> students = new List<StudentModel>();

            // Use a 'using' block to ensure the connection is closed and disposed properly
            using (var conn = new SqlConnection( connectionString )) {
                try {
                    // SQL query to retrieve students data
                    var query = "SELECT * FROM students";
                    SqlCommand cmd = new SqlCommand( query, conn );

                    // Open the connection
                    conn.Open();

                    // Execute the command and retrieve the data
                    var reader = cmd.ExecuteReader();

                    // Loop through the rows and fetch data
                    while (reader.Read()) {
                        // Assuming the student table has columns like 'Id' and 'Name'
                        var student = new StudentModel {
                            Id = reader.GetInt32( reader.GetOrdinal( "student_id" ) ),
                            Name = reader.GetString( reader.GetOrdinal( "name" ) ),
                            Age = reader.GetInt32( reader.GetOrdinal( "age" ) ),
                            Gender = reader.GetString( reader.GetOrdinal( "gender" ) )
                        };
                        students.Add( student );
                    }
                }
                catch (Exception ex) {
                    // Handle exceptions, such as connection issues or SQL errors
                    Console.WriteLine( $"Error: {ex.Message}" );
                }
            }


            foreach (var student in students) {
                Console.WriteLine( student );
            }
            // Return the students list to the view
            return Json( students );
        }
    }


}
