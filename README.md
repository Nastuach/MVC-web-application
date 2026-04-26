# Lab6 — Система для создания и прохождения тестов

Lab6 — учебный проект, представляющий собой систему для создания и прохождения тестов. Приложение позволяет:

- Создавать категории вопросов
- Управлять вопросами с множественными вариантами ответов
- Проводить тестирование с автоматической проверкой
- Экспортировать данные в JSON формат

---

## ✨ Функциональные возможности

### Управление категориями

- ➕ Создание категории (название, описание)
- ✏️ Редактирование категории
- 👁️ Просмотр деталей категории
- 🗑️ Удаление категории
- 📊 Статистика по категориям

### Управление вопросами

- ➕ Создание вопроса с указанием:
  - Текст вопроса
  - Комментарий/пояснение
  - Привязка к категории
  - Список правильных ответов
  - Список неправильных ответов
- ✏️ Редактирование вопроса
- 👁️ Детальный просмотр
- 🗑️ Удаление вопроса
- 📊 Статистика по вопросам

### Тестирование

- 🎯 Настройка параметров теста:
  - Выбор категории вопросов
  - Количество вопросов
- 📝 Прохождение теста с вводом ответов
- ✅ Автоматическая проверка ответов
- 📈 Отображение результатов (правильные/неправильные ответы)

### Экспорт данных

- 📤 Экспорт вопросов в JSON с опциями:
  - Ограничение количества записей
  - Сортировка (по тексту, комментарию, ID)
- 📅 Автоматическое именование файлов с временной меткой

---

## 🛠️ Технологический стек

| Компонент     | Технология                     |
|---------------|--------------------------------|
| Framework     | .NET 9.0                       |
| Web Framework | ASP.NET Core MVC               |
| ORM           | Entity Framework Core 9.0.4    |
| Database      | SQL Server (LocalDB)           |
| Frontend      | Bootstrap 5, jQuery Validation |
| Serialization | System.Text.Json               |

### Пакеты NuGet

```xml
- Microsoft.EntityFrameworkCore (9.0.4)
- Microsoft.EntityFrameworkCore.SqlServer (9.0.4)
- Microsoft.EntityFrameworkCore.Tools (9.0.4)
- Microsoft.VisualStudio.Web.CodeGeneration.Design (9.0.0)
```

---

## 🚀 Установка и запуск

### Предварительные требования

- .NET 9.0 SDK
- SQL Server LocalDB
- Visual Studio 2022 / VS Code / Rider

### Шаги установки

**1. Клонировать репозиторий**

```bash
git clone https://github.com/yourusername/Lab6.git
cd Lab6
```

**2. Настроить строку подключения** *(опционально)*

Отредактируйте `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=Lab6DB;Trusted_Connection=True;"
  }
}
```

**3. Применить миграции базы данных**

```bash
dotnet ef database update
```

> Или использовать встроенную миграцию при запуске — приложение автоматически применит миграции.

**4. Запустить приложение**

```bash
dotnet run
```

**5. Открыть браузер**

Перейти по адресу: `https://localhost:5001` или `http://localhost:5000`

---

## 📁 Структура проекта

```
Lab6/
├── Controllers/
│   ├── HomeController.cs          # Главная страница
│   ├── CategoryController.cs      # CRUD операции для категорий
│   └── QuestionController.cs      # CRUD операции для вопросов + тестирование
├── Models/
│   ├── Category.cs                # Модель категории
│   ├── Question.cs                # Модель вопроса (с JSON сериализацией)
│   ├── QuestionContext.cs         # DbContext
│   └── TestResultViewModel.cs     # ViewModel для результатов
├── Repositories/
│   ├── ICategoryRepository.cs     # Интерфейс репозитория категорий
│   ├── IQuestionRepository.cs     # Интерфейс репозитория вопросов
│   ├── EFCategoryRepository.cs    # Реализация для категорий
│   └── EFQuestionRepository.cs    # Реализация для вопросов
├── Views/
│   ├── Home/
│   │   └── Index.cshtml          # Главное меню
│   ├── Category/
│   │   ├── All.cshtml            # Список категорий
│   │   ├── Add.cshtml            # Создание категории
│   │   ├── Edit.cshtml           # Редактирование категории
│   │   ├── Details.cshtml        # Детали категории
│   │   ├── Remove.cshtml         # Удаление категории
│   │   └── Stat.cshtml           # Статистика категорий
│   └── Question/
│       ├── All.cshtml            # Список вопросов
│       ├── Add.cshtml            # Создание вопроса
│       ├── Edit.cshtml           # Редактирование вопроса
│       ├── Details.cshtml        # Детали вопроса
│       ├── Remove.cshtml         # Удаление вопроса
│       ├── Start.cshtml          # Настройки теста
│       ├── Test.cshtml           # Прохождение теста
│       ├── TestResult.cshtml     # Результаты теста
│       ├── Stat.cshtml           # Статистика вопросов
│       └── Export.cshtml         # Экспорт данных
├── Migrations/                   # EF Core миграции
├── wwwroot/                      # Статические файлы (CSS, JS)
├── Program.cs                    # Точка входа и конфигурация
├── appsettings.json              # Конфигурация приложения
└── Lab6.csproj                   # Файл проекта
```

---

## 🔌 API Эндпоинты

### Категории

| Метод | URL                                        | Описание               |
|-------|--------------------------------------------|------------------------|
| GET   | `/Category/All?n={count}&sort={field}`     | Список категорий       |
| GET   | `/Category/Add`                            | Форма создания         |
| POST  | `/Category/Add`                            | Создать категорию      |
| GET   | `/Category/Edit/{id}`                      | Форма редактирования   |
| POST  | `/Category/Edit/{id}`                      | Обновить категорию     |
| GET   | `/Category/Remove/{id}`                    | Подтверждение удаления |
| POST  | `/Category/Remove/{id}`                    | Удалить категорию      |
| GET   | `/Category/Details/{id}`                   | Детали категории       |
| GET   | `/Category/Stat`                           | Статистика             |
| GET   | `/Category/Export?n={count}&sort={field}`  | Экспорт JSON           |

### Вопросы

| Метод | URL                                            | Описание               |
|-------|------------------------------------------------|------------------------|
| GET   | `/Question/All?n={count}&sort={field}`         | Список вопросов        |
| GET   | `/Question/Add`                                | Форма создания         |
| POST  | `/Question/Add`                                | Создать вопрос         |
| GET   | `/Question/Edit/{id}`                          | Форма редактирования   |
| POST  | `/Question/Edit/{id}`                          | Обновить вопрос        |
| GET   | `/Question/Remove/{id}`                        | Подтверждение удаления |
| POST  | `/Question/Remove/{id}`                        | Удалить вопрос         |
| GET   | `/Question/Details/{id}`                       | Детали вопроса         |
| GET   | `/Question/Start`                              | Настройки теста        |
| GET   | `/Question/Test?n={count}&categoryId={id}`     | Начать тест            |
| POST  | `/Question/Test`                               | Отправить ответы       |
| GET   | `/Question/Stat`                               | Статистика             |
| GET   | `/Question/Export?n={count}&sort={field}`      | Экспорт JSON           |

---

## 🗄️ База данных

### Схема

**Таблица `Categories`**

```sql
CREATE TABLE Categories (
    Id          INT PRIMARY KEY IDENTITY,
    Title       NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL
);
```

**Таблица `Questions`**

```sql
CREATE TABLE Questions (
    Id         INT PRIMARY KEY IDENTITY,
    Text       NVARCHAR(MAX) NOT NULL,
    Answers    NVARCHAR(MAX) NOT NULL,   -- JSON строка
    BadAnswers NVARCHAR(MAX) NOT NULL,   -- JSON строка
    Comment    NVARCHAR(MAX) NOT NULL,
    CategoryId INT NOT NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id) ON DELETE CASCADE
);
```

### Хранение ответов

Правильные и неправильные ответы хранятся в формате JSON. Пример: `["Ответ 1","Ответ 2"]`. При работе с моделями автоматически десериализуются в `List<string>`.

---

## 📸 Скриншоты

*(Добавьте скриншоты вашего приложения)*

---

## 🔧 Устранение неполадок

**Ошибка подключения к базе данных**

```bash
dotnet ef database update
```

Убедитесь, что SQL Server LocalDB установлен и запущен.

**Ошибка миграции**

Удалите папку `Migrations` и создайте миграцию заново:

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Порт уже используется**

Измените порт в `launchSettings.json` или используйте:

```bash
dotnet run --urls=http://localhost:5005
```

**Ошибка компиляции**

Очистите и пересоберите проект:

```bash
dotnet clean
dotnet build
```
