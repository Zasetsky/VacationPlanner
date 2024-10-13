FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы csproj и восстанавливаем зависимости
COPY ["src/VacationPlanner.ConsoleApp/VacationPlanner.ConsoleApp.csproj", "src/VacationPlanner.ConsoleApp/"]
COPY ["src/VacationPlanner.Services/VacationPlanner.Services.csproj", "src/VacationPlanner.Services/"]
COPY ["src/VacationPlanner.Data/VacationPlanner.Data.csproj", "src/VacationPlanner.Data/"]
COPY ["src/VacationPlanner.Domain/VacationPlanner.Domain.csproj", "src/VacationPlanner.Domain/"]

RUN dotnet restore "src/VacationPlanner.ConsoleApp/VacationPlanner.ConsoleApp.csproj"

# Копируем остальные файлы и собираем проект
COPY . .
WORKDIR "/src/src/VacationPlanner.ConsoleApp"
RUN dotnet build "VacationPlanner.ConsoleApp.csproj" -c Release -o /app/build

# Публикуем приложение
FROM build AS publish
RUN dotnet publish "VacationPlanner.ConsoleApp.csproj" -c Release -o /app/publish

# Создаем финальный образ
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Выполняем миграции и запускаем приложение
ENTRYPOINT ["dotnet", "VacationPlanner.ConsoleApp.dll"]
