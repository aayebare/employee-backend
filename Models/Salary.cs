using System.ComponentModel.DataAnnotations;

namespace EmployeeSalaryManagement.Models
{
    public class Salary
    {
        [Key]
        public int SalaryId { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }

        public Employee? Employee { get; set; }
    }
}
