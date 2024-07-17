using System.ComponentModel.DataAnnotations;

namespace EmployeeSalaryManagement.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
    }
}
