using VacationPlanner.Domain;

namespace VacationPlanner.Services
{
    public interface IVacationService
    {
        Task PlanVacationsAsync();
        Task<List<Vacation>> GetVacationsForEmployeeAsync(int employeeId);
        Task<Employee> AddEmployeeAsync(Employee employee);
        Task<List<Vacation>> GetAllVacationsAsync();
        Task RecalculateVacationsAsync();
    }
}
