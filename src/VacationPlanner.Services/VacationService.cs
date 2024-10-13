using Microsoft.EntityFrameworkCore;
using VacationPlanner.Data;
using VacationPlanner.Domain;

namespace VacationPlanner.Services
{
    public class VacationService : IVacationService
    {
        private readonly VacationPlannerDbContext _context;

        public VacationService(VacationPlannerDbContext context)
        {
            _context = context;
        }

        public async Task PlanVacationsAsync()
        {
            var employees = await _context.Employees.Include(e => e.Vacations).ToListAsync();
            var year = DateTime.UtcNow.Year;
            var random = new Random();

            foreach (var employee in employees)
            {
                // Получаем уже запланированные отпуска сотрудника на текущий год
                var existingVacations = await _context
                    .Vacations.Where(v => v.EmployeeId == employee.Id && v.StartDate.Year == year)
                    .ToListAsync();

                // Вычисляем использованные дни отпуска
                int usedDays = existingVacations.Sum(v => (v.EndDate - v.StartDate).Days + 1);
                int remainingDays = 28 - usedDays;

                if (remainingDays <= 0)
                {
                    // Сотрудник уже использовал все дни отпуска
                    continue;
                }

                while (remainingDays > 0)
                {
                    // Генерируем случайную дату начала отпуска
                    var randomStartDay = random.Next(1, 366);
                    var randomStartDate = new DateTime(year, 1, 1).AddDays(randomStartDay - 1);

                    // Проверяем, чтобы случайная дата была рабочим днем
                    if (!IsWorkingDay(randomStartDate))
                        continue;

                    // Выбираем случайную длину отпуска: 7 или 14 рабочих дней
                    int vacationLength = (remainingDays >= 14 && random.Next(0, 2) == 1) ? 14 : 7;

                    // Рассчитываем дату конца отпуска, исключая выходные дни
                    DateTime vacationEndDate = CalculateEndDate(randomStartDate, vacationLength);

                    // Создаем отпуск
                    var vacation = new Vacation
                    {
                        EmployeeId = employee.Id,
                        StartDate = DateTime.SpecifyKind(randomStartDate, DateTimeKind.Utc),
                        EndDate = DateTime.SpecifyKind(vacationEndDate, DateTimeKind.Utc),
                    };

                    // Проверяем, не пересекается ли новый отпуск с уже существующими
                    bool overlaps = existingVacations.Any(v =>
                        (v.StartDate <= vacation.EndDate && v.EndDate >= vacation.StartDate)
                    );

                    // Проверяем пересечения с уже запланированными отпусками в этом цикле
                    overlaps |= _context.Vacations.Any(v =>
                        v.EmployeeId == employee.Id
                        && (v.StartDate <= vacation.EndDate && v.EndDate >= vacation.StartDate)
                    );

                    if (!overlaps && vacation.EndDate.Year == year)
                    {
                        // Если нет пересечений, добавляем отпуск
                        _context.Vacations.Add(vacation);
                        existingVacations.Add(vacation);

                        remainingDays -= vacationLength;

                        if (remainingDays <= 0)
                            break;
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        private DateTime CalculateEndDate(DateTime startDate, int vacationLength)
        {
            DateTime endDate = startDate;
            int workDaysCount = 0;

            while (workDaysCount < vacationLength)
            {
                if (IsWorkingDay(endDate))
                {
                    workDaysCount++;
                }
                endDate = endDate.AddDays(1);
            }

            // Отнимаем один день, так как endDate на данный момент находится на следующий день после последнего рабочего дня
            return endDate.AddDays(-1);
        }

        public async Task<List<Vacation>> GetVacationsForEmployeeAsync(int employeeId)
        {
            return await _context
                .Vacations.Where(v => v.EmployeeId == employeeId)
                .OrderBy(v => v.StartDate)
                .ToListAsync();
        }

        public async Task<List<Vacation>> GetAllVacationsAsync()
        {
            return await _context
                .Vacations.Include(v => v.Employee)
                .OrderBy(v => v.StartDate)
                .ToListAsync();
        }

        public async Task RecalculateVacationsAsync()
        {
            // Удаляем все существующие отпуска
            var allVacations = await _context.Vacations.ToListAsync();
            _context.Vacations.RemoveRange(allVacations);
            await _context.SaveChangesAsync();

            // Планируем отпуска заново
            await PlanVacationsAsync();
        }

        private bool IsWorkingDay(DateTime date)
        {
            return date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday;
        }
    }
}
