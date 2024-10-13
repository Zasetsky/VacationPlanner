using Microsoft.EntityFrameworkCore;
using VacationPlanner.Domain;

namespace VacationPlanner.Data
{
    public class VacationPlannerDbContext : DbContext
    {
        public VacationPlannerDbContext(DbContextOptions<VacationPlannerDbContext> options)
            : base(options) { }

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Vacation> Vacations { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Заполнение данными
            modelBuilder
                .Entity<Employee>()
                .HasData(
                    new Employee
                    {
                        Id = 1,
                        FirstName = "Иван",
                        LastName = "Иванов",
                        MiddleName = "Иванович",
                    },
                    new Employee
                    {
                        Id = 2,
                        FirstName = "Петр",
                        LastName = "Петров",
                        MiddleName = "Петрович",
                    },
                    new Employee
                    {
                        Id = 3,
                        FirstName = "Юлия",
                        LastName = "Юлина",
                        MiddleName = "Юлиановна",
                    },
                    new Employee
                    {
                        Id = 4,
                        FirstName = "Сидор",
                        LastName = "Сидоров",
                        MiddleName = "Сидорович",
                    },
                    new Employee
                    {
                        Id = 5,
                        FirstName = "Павел",
                        LastName = "Павлов",
                        MiddleName = "Павлович",
                    },
                    new Employee
                    {
                        Id = 6,
                        FirstName = "Георг",
                        LastName = "Георгиев",
                        MiddleName = "Георгиевич",
                    }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
