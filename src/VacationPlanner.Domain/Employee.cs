namespace VacationPlanner.Domain
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        public ICollection<Vacation> Vacations { get; } = new List<Vacation>();
    }
}
