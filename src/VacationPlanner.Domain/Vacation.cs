namespace VacationPlanner.Domain
{
    public class Vacation
    {
        public int Id { get; set; }

        public int EmployeeId { get; set; }

        public Employee? Employee { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
