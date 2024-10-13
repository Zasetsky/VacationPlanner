using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VacationPlanner.Data;
using VacationPlanner.Domain;
using VacationPlanner.Services;

namespace VacationPlanner.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();

            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<VacationPlannerDbContext>();

                // Автоматическое создание базы данных и таблиц
                bool dbCreated = await dbContext.Database.EnsureCreatedAsync();
                if (dbCreated)
                {
                    Console.WriteLine("База данных и таблицы успешно созданы.");
                }
                else
                {
                    Console.WriteLine("База данных уже существует.");
                }

                var vacationService = services.GetRequiredService<IVacationService>();

                // Планирование отпусков только если они не запланированы
                var existingVacations = await dbContext.Vacations.AnyAsync();
                if (!existingVacations)
                {
                    Console.WriteLine("Планирование отпусков...");
                    await vacationService.PlanVacationsAsync();
                    Console.WriteLine("Отпуска успешно спланированы.");
                }
                else
                {
                    Console.WriteLine("Отпуска уже запланированы.");
                }

                await ShowMenuAsync(vacationService);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }

            await host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(
                    (hostingContext, config) =>
                    {
                        config.AddJsonFile(
                            "appsettings.json",
                            optional: false,
                            reloadOnChange: true
                        );
                    }
                )
                .ConfigureServices(
                    (hostContext, services) =>
                    {
                        // Настройка DbContext
                        services.AddDbContext<VacationPlannerDbContext>(options =>
                            options.UseNpgsql(
                                hostContext.Configuration.GetConnectionString("DefaultConnection")
                            )
                        );

                        // Регистрация сервисов
                        services.AddScoped<IVacationService, VacationService>();
                    }
                );

        static async Task ShowMenuAsync(IVacationService vacationService)
        {
            while (true)
            {
                Console.WriteLine("\nВыберите действие:");
                Console.WriteLine("1. Добавить пользователя");
                Console.WriteLine("2. Вывести все отпуска");
                Console.WriteLine("3. Перерасчитать отпуска");
                Console.WriteLine("4. Выйти");
                Console.Write("Введите номер действия: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await AddEmployeeAsync(vacationService);
                        break;
                    case "2":
                        await DisplayAllVacationsAsync(vacationService);
                        break;
                    case "3":
                        await RecalculateVacationsAsync(vacationService);
                        break;
                    case "4":
                        Console.WriteLine("Выход из приложения.");
                        return;
                    default:
                        Console.WriteLine("Неверный ввод. Пожалуйста, попробуйте снова.");
                        break;
                }
            }
        }

        static async Task AddEmployeeAsync(IVacationService vacationService)
        {
            Console.WriteLine("\n--- Добавление Пользователя ---");
            Console.Write("Введите имя: ");
            var firstName = Console.ReadLine();

            Console.Write("Введите фамилию: ");
            var lastName = Console.ReadLine();

            Console.Write("Введите отчество: ");
            var middleName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                Console.WriteLine("Имя и фамилия обязательны для заполнения.");
                return;
            }

            var newEmployee = new Employee
            {
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName.Trim(),
            };

            var addedEmployee = await vacationService.AddEmployeeAsync(newEmployee);
            Console.WriteLine(
                $"\nПользователь добавлен: {addedEmployee.FirstName} {addedEmployee.LastName} {addedEmployee.MiddleName}"
            );

            var vacations = await vacationService.GetVacationsForEmployeeAsync(addedEmployee.Id);
            if (vacations.Any())
            {
                foreach (var vacation in vacations)
                {
                    Console.WriteLine(
                        $"Отпуск с {vacation.StartDate:dd.MM.yyyy} по {vacation.EndDate:dd.MM.yyyy}"
                    );
                }
            }
            else
            {
                Console.WriteLine("У этого пользователя еще не запланированы отпуска.");
            }
        }

        static async Task DisplayAllVacationsAsync(IVacationService vacationService)
        {
            Console.WriteLine("\n--- Все Отпуска ---");
            var allVacations = await vacationService.GetAllVacationsAsync();

            if (!allVacations.Any())
            {
                Console.WriteLine("Отпуска отсутствуют.");
                return;
            }

            foreach (var vacation in allVacations)
            {
                Console.WriteLine(
                    $"{vacation.Employee?.FirstName} {vacation.Employee?.LastName} {vacation.Employee?.MiddleName} "
                        + $"отпуск с {vacation.StartDate:dd.MM.yyyy} по {vacation.EndDate:dd.MM.yyyy}"
                );
            }
        }

        static async Task RecalculateVacationsAsync(IVacationService vacationService)
        {
            Console.WriteLine("\n--- Перерасчет Отпусков ---");
            await vacationService.RecalculateVacationsAsync();
            Console.WriteLine("Отпуска успешно перерасчитаны.");
        }
    }
}
