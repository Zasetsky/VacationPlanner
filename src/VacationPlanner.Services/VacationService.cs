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

            // Получаем все уже существующие отпуска для всех сотрудников в текущем году
            var allExistingVacations = await _context
                .Vacations.Where(v => v.StartDate.Year == year)
                .ToListAsync();

            foreach (var employee in employees)
            {
                // Получаем уже запланированные отпуска сотрудника на текущий год
                var existingVacations = allExistingVacations
                    .Where(v => v.EmployeeId == employee.Id)
                    .ToList();

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

                    // Проверяем разрыв между отпусками (должно быть минимум 1 месяц)
                    bool isValidGap = existingVacations.All(v =>
                        (randomStartDate - v.EndDate).Days >= 30
                        || (v.StartDate - randomStartDate).Days >= 30
                    );
                    if (!isValidGap)
                        continue;

                    // Выбираем случайную длину отпуска: 7 или 14 дней
                    int vacationLength = (remainingDays >= 14 && random.Next(0, 2) == 1) ? 14 : 7;

                    // Рассчитываем дату конца отпуска
                    DateTime vacationEndDate = randomStartDate.AddDays(vacationLength - 1);

                    // Создаем отпуск
                    var vacation = new Vacation
                    {
                        EmployeeId = employee.Id,
                        StartDate = DateTime.SpecifyKind(randomStartDate, DateTimeKind.Utc),
                        EndDate = DateTime.SpecifyKind(vacationEndDate, DateTimeKind.Utc),
                    };

                    // Проверяем пересечения с уже существующими отпусками сотрудника
                    bool overlapsWithOwn = existingVacations.Any(v =>
                        (v.StartDate <= vacation.EndDate && v.EndDate >= vacation.StartDate)
                    );

                    // Проверяем пересечения с отпусками других сотрудников
                    bool overlapsWithOthers = allExistingVacations.Any(v =>
                        v.EmployeeId != employee.Id
                        && (v.StartDate <= vacation.EndDate && v.EndDate >= vacation.StartDate)
                    );

                    if (!overlapsWithOwn && !overlapsWithOthers && vacation.EndDate.Year == year)
                    {
                        // Если нет пересечений, добавляем отпуск в локальные переменные
                        _context.Vacations.Add(vacation);
                        existingVacations.Add(vacation);
                        allExistingVacations.Add(vacation);

                        remainingDays -= vacationLength;

                        if (remainingDays <= 0)
                            break;
                    }
                }
            }

            await _context.SaveChangesAsync();
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
