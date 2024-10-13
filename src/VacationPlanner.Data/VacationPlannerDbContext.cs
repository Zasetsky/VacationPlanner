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
                        FirstName = "Иванов",
                        LastName = "Иван",
                        MiddleName = "Иванович",
                    },
                    new Employee
                    {
                        Id = 2,
                        FirstName = "Петров",
                        LastName = "Петр",
                        MiddleName = "Петрович",
                    },
                    new Employee
                    {
                        Id = 3,
                        FirstName = "Юлина",
                        LastName = "Юлия",
                        MiddleName = "Юлиановна",
                    },
                    new Employee
                    {
                        Id = 4,
                        FirstName = "Сидоров",
                        LastName = "Сидор",
                        MiddleName = "Сидорович",
                    },
                    new Employee
                    {
                        Id = 5,
                        FirstName = "Павлов",
                        LastName = "Павел",
                        MiddleName = "Павлович",
                    },
                    new Employee
                    {
                        Id = 6,
                        FirstName = "Георгиев",
                        LastName = "Георг",
                        MiddleName = "Георгиевич",
                    }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
