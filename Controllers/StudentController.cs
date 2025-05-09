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

        public IActionResult Login () {
            // survives one request only
            // survives only the view
            ViewBag.name = "helloo";

            // survives two requests and one redirect
            // survives the view as well as the post request after login
            TempData["name"] = "helloo";
            return View();
        }

        [HttpPost]
        public IActionResult Login ( StudentModel student ) {
            // authenticate the student 
            // if successfull authentication the redirect to home
            var vdata = ViewBag.name;
            var tdata = TempData["name"];


            Console.WriteLine( vdata + ",,,,,,,,,,,,,,," );
            Console.WriteLine( tdata + ",,,,,,,,,,,,,,," );

            return RedirectToAction( "Home", "Student" );
        }


        public IActionResult Home () {
            return View();
        }



        public JsonResult LoadStudents () {
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

            return Json( students );
        }

    }
}
