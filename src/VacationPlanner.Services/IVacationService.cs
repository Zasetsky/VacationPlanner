using VacationPlanner.Domain;

namespace VacationPlanner.Services
{
    public interface IVacationService
    {
        Task PlanVacationsAsync();

        Task<List<Vacation>> GetVacationsForEmployeeAsync(int employeeId);

        Task<List<Vacation>> GetAllVacationsAsync();

        Task RecalculateVacationsAsync();
    }
}
