# VacationPlanner

VacationPlanner — это многослойное приложение на C# для планирования отпусков сотрудников с использованием базы данных и Docker.

## Структура проекта

Проект разделен на несколько слоев:

- **VacationPlanner.Domain**: Содержит бизнес-логику и модели данных.
- **VacationPlanner.Data**: Работа с базой данных, включая контекст базы данных и миграции Entity Framework.
- **VacationPlanner.Services**: Сервисный слой для управления отпусками, расчетами и логикой приложения.
- **VacationPlanner.ConsoleApp**: Консольное приложение, которое предоставляет интерфейс для взаимодействия с сервисами.

## Требования

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/)

### Установка Make на Windows

На Windows необходимо установить утилиту `make`, так как она по умолчанию отсутствует в системе. Для этого используйте пакетный менеджер [Chocolatey](https://chocolatey.org/install):

1. Откройте командную строку с правами администратора и выполните следующую команду для установки Chocolatey:

   ```bash
   @"%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe" -NoProfile -ExecutionPolicy Bypass -Command "Set-ExecutionPolicy Bypass -Scope Process -Force; [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072; iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))" && SET "PATH=%PATH%;%ALLUSERSPROFILE%\chocolatey\bin"
   ```
   
2. После установки Chocolatey выполните следующую команду для установки make:
   ```bash
   choco install make
   ```

## Как запустить

Для запуска приложения используется `Makefile`, который автоматизирует работу с Docker Compose.

### Шаги для запуска

1. Сначала клонируйте репозиторий:

   ```bash
   git clone https://github.com/your-repo/vacation-planner.git
   cd vacation-planner
   ```

2. Для сборки используйте команду:

   ```bash
   make all
   ```

## Примечание
Перед запуском убедитесь, что все зависимости установлены, и Docker корректно настроен в вашей системе.
