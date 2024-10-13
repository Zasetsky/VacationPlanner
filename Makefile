COMPOSE_FILE = ./docker/docker-compose.yml

.PHONY: build run up down logs all

# Собирает Docker образы
build:
	docker-compose -f $(COMPOSE_FILE) build

# Запускает сервисы (например, базу данных) в фоновом режиме
up:
	docker-compose -f $(COMPOSE_FILE) up -d db

# Запускает консольное приложение
run:
	docker-compose -f $(COMPOSE_FILE) run --rm app

# Останавливает и удаляет запущенные контейнеры
down:
	docker-compose -f $(COMPOSE_FILE) down

# Показывает логи всех сервисов
logs:
	docker-compose -f $(COMPOSE_FILE) logs -f

# Выполняет последовательность: сборка, запуск базы и запуск приложения
all: build up run
