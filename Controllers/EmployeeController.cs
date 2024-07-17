using CsvHelper;
using CsvHelper.Configuration;
using EmployeeSalaryManagement.Data;
using EmployeeSalaryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeSalaryManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.ToListAsync();
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return employee;
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] Employee employee)
        {
            Console.WriteLine($"ID from route: {id}");
            Console.WriteLine($"EmployeeId from body: {employee.EmployeeId}");
            Console.WriteLine($"Employee Name: {employee.Name}");

            // Fetch the existing employee from the database
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound();
            }

            // Update employee fields (name, department, position)
            existingEmployee.Name = employee.Name;
            existingEmployee.Department = employee.Department;
            existingEmployee.Position = employee.Position;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var updatedEmployee = await _context.Employees.FindAsync(id);


            if (updatedEmployee == null)
            {
                return NotFound();
            }

            return Ok(updatedEmployee);
        }


        // POST: api/Employee
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmployee", new { id = employee.EmployeeId }, employee);
        }

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id);
        }

        // GET: api/Employee/export
        [HttpGet("export")]
        public async Task<IActionResult> ExportEmployeesToCsv()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync();

                var csvBytes = ExportEmployeesToCsvBytes(employees);

                Response.Headers.Append("Content-Disposition", "attachment; filename=employees.csv");

                return File(csvBytes, "text/csv");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exporting employees: {ex.Message}");
            }
        }

        private byte[] ExportEmployeesToCsvBytes(IEnumerable<Employee> employees)
        {
            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = (context) => true
            };

            using var csvWriter = new CsvWriter(streamWriter, csvConfig);

            csvWriter.WriteRecords(employees);

            streamWriter.Flush();
            return memoryStream.ToArray();
        }
    }
}