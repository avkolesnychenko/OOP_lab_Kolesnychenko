using System.Globalization;

namespace PizzaAsteroidApp
{
    internal class Program
    {
        private static readonly List<PizzaAsteroid> Asteroids = [];
        private static int _maxCapacity;

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            _maxCapacity = ReadPositiveInt("Введіть максимальну кількість астероїдів N (N > 0): ");

            bool running = true;
            while (running)
            {
                Console.WriteLine($"\nЗаповненість: {Asteroids.Count} / {_maxCapacity}");
                Console.WriteLine("1 – Додати об'єкт");
                Console.WriteLine("2 – Переглянути всі об'єкти");
                Console.WriteLine("3 – Знайти об'єкт");
                Console.WriteLine("4 – Продемонструвати поведінку");
                Console.WriteLine("5 – Видалити об'єкт");
                Console.WriteLine("0 – Вийти з програми");
                Console.Write(">");

                string ans = Console.ReadLine()!.Trim();
                Console.WriteLine();

                switch (ans)
                {
                    case "1":
                        AddAsteroidMenu();
                        break;
                    case "2":
                        PrintTable(Asteroids, "Список усіх піцца-астероїдів");
                        break;
                    case "3":
                        SearchAsteroids();
                        break;
                    case "4":
                        DemonstrateBehavior();
                        break;
                    case "5":
                        DeleteAsteroidMenu();
                        break;
                    case "0":
                        running = false;
                        Console.WriteLine("Робота програми завершена. Гарного космічного апетиту!");
                        break;
                    default:
                        Console.WriteLine("Помилка! Невідомий пункт меню. Спробуйте ще раз.");
                        break;
                }
            }
        }

        private static void AddAsteroidMenu()
        {
            if (Asteroids.Count >= _maxCapacity)
            {
                Console.WriteLine($"Досягнуто ліміт N = {_maxCapacity}. Неможливо додати новий об'єкт.");
                return;
            }

            Console.WriteLine("Режим додавання:");
            Console.WriteLine("1 - Ввести дані вручну");
            Console.WriteLine("2 - Згенерувати автоматично");
            Console.WriteLine("3 – Створити за замовчуванням (Default Constructor)");
            Console.Write(">");
            string mode = Console.ReadLine()!.Trim();

            if (mode == "1")
            {
                PizzaAsteroid asteroid = new();

                while (true)
                {
                    try
                    {
                        Console.Write("Введіть назву астероїда (3-20 симв., літери/цифри/-): ");
                        asteroid.Name = Console.ReadLine()!;
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"[Помилка властивості Name]: {ex.Message}");
                    }
                }

                while (true)
                {
                    try
                    {
                        Console.WriteLine("Оберіть бортик/тісто: 1 - Thin, 2 - CheeseStuffed, 3 - DeepDish, 4 - Classic");
                        Console.Write("Введіть номер (1-4): ");
                        if (int.TryParse(Console.ReadLine(), out int raw))
                        {
                            asteroid.Crust = (CrustType)raw;
                            break;
                        }
                        throw new FormatException("Введене значення має бути цілим числом.");
                    }
                    catch (Exception ex) when (ex is ArgumentException || ex is FormatException)
                    {
                        Console.WriteLine($"[Помилка властивості Crust]: {ex.Message}");
                    }
                }

                while (true)
                {
                    try
                    {
                        Console.Write("Введіть діаметр у км (0.1 .. 1000.0): ");
                        string raw = Console.ReadLine()!.Replace(',', '.');
                        if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                        {
                            asteroid.DiameterKm = val;
                            break;
                        }
                        throw new FormatException("Введене значення не є дійсним числом.");
                    }
                    catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException)
                    {
                        Console.WriteLine($"[Помилка властивості DiameterKm]: {ex.Message}");
                    }
                }

                while (true)
                {
                    try
                    {
                        Console.Write("Введіть температуру в °C (-273 .. 500): ");
                        if (int.TryParse(Console.ReadLine(), out int val))
                        {
                            asteroid.TemperatureCelsius = val;
                            break;
                        }
                        throw new FormatException("Температура повинна бути цілим числом.");
                    }
                    catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException)
                    {
                        Console.WriteLine($"[Помилка властивості TemperatureCelsius]: {ex.Message}");
                    }
                }

                asteroid.HasExtraCheese = ReadValidatedBool("Чи є подвійний сир? (1/так - true, 0/ні - false): ");

                Console.Write("Введіть тип соусу (натисніть Enter для 'Томатний Класик'): ");
                string sauceInput = Console.ReadLine()!.Trim();
                if (!string.IsNullOrEmpty(sauceInput))
                {
                    asteroid.SauceType = sauceInput;
                }

                while (true)
                {
                    try
                    {
                        Console.Write("Введіть дату відкриття (dd.MM.yyyy, від 01.01.1990 до сьогодні): ");
                        string raw = Console.ReadLine()!.Trim();
                        if (DateTime.TryParseExact(raw, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                        {
                            asteroid.SetDiscoveryDate(d);
                            break;
                        }
                        throw new FormatException("Формат дати має бути dd.MM.yyyy (наприклад, 12.04.2021).");
                    }
                    catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException)
                    {
                        Console.WriteLine($"[Помилка дати відкриття]: {ex.Message}");
                    }
                }

                Asteroids.Add(asteroid);
                Console.WriteLine($"Успіх! Астероїд '{asteroid.Name}' успішно створено та додано!");
            }
            else if (mode == "2")
            {
                var rand = new Random();
                string[] names = { "Пепероні-X", "Квадро-Формаджо", "Карбонара-99", "Гаваї-Ультра", "Діавола-Prime" };
                string[] sauces = { "Барбекю", "Часниковий", "Песто", "Гострий Чилі", "Томатний Класик" };

                try
                {
                    PizzaAsteroid asteroid = new(
                        name: names[rand.Next(names.Length)] + "-" + rand.Next(10, 999),
                        crust: (CrustType)rand.Next(1, 5),
                        diameterKm: Math.Round(rand.NextDouble() * 99 + 1, 2),
                        temperatureCelsius: rand.Next(-200, 350),
                        hasExtraCheese: rand.Next(2) == 1,
                        discoveryDate: DateTime.Now.AddDays(-rand.Next(1, 5000)),
                        sauce: sauces[rand.Next(sauces.Length)]
                    );

                    Asteroids.Add(asteroid);
                    Console.WriteLine($"Успіх! Автоматично згенеровано та додано: '{asteroid.Name}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при автогенерації: {ex.Message}");
                }
            }
            else if (mode == "3")
            {
                PizzaAsteroid defaultAsteroid = new();
                Asteroids.Add(defaultAsteroid);

                Console.WriteLine("Успіх! Створено об'єкт за замовчуванням:");
                Console.WriteLine($"Назва: {defaultAsteroid.Name}");
                Console.WriteLine($"Бортик: {defaultAsteroid.Crust}");
                Console.WriteLine($"Діаметр: {defaultAsteroid.DiameterKm} км");
                Console.WriteLine($"Температура: {defaultAsteroid.TemperatureCelsius} °C");
                Console.WriteLine($"Соус: {defaultAsteroid.SauceType}");
                Console.WriteLine($"Об'єм: {defaultAsteroid.EstimatedVolumeKm3} км³");
            }
            else
            {
                Console.WriteLine("Помилка! Некоректний вибір режиму.");
            }
        }

        private static void DemonstrateBehavior()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Немає об'єктів для виклику методів.");
                return;
            }

            PrintTable(Asteroids, "Оберіть астероїд для демонстрації");
            Console.Write($"Введіть порядковий номер об'єкта (1..{Asteroids.Count}): ");
            if (!int.TryParse(Console.ReadLine(), out int idx) || idx < 1 || idx > Asteroids.Count)
            {
                Console.WriteLine("Помилка! Некоректний номер.");
                return;
            }

            PizzaAsteroid target = Asteroids[idx - 1];

            Console.WriteLine($"\nОберіть дію для астероїда '{target.Name}':");
            Console.WriteLine("1 – Нагріти астероїд (HeatUp)");
            Console.WriteLine("2 – Розрізати астероїд на шматки (Slice)");
            Console.WriteLine("3 – Створити зіткнення з планетою (CollideWithTarget)");
            Console.Write(">");

            switch (Console.ReadLine()!.Trim())
            {
                case "1":
                    Console.Write("На скільки градусів підняти температуру?: ");
                    if (int.TryParse(Console.ReadLine(), out int deg))
                    {
                        try
                        {
                            target.HeatUp(deg);
                            Console.WriteLine($"Успіх! Астероїд '{target.Name}' нагріто. Нова T = {target.TemperatureCelsius}°C.");
                        }
                        catch (ArgumentOutOfRangeException ex)
                        {
                            Console.WriteLine($"[Помилка]: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Помилка! Некоректне число.");
                    }
                    break;

                case "2":
                    Console.Write("На скільки частин розрізати (>= 2)?: ");
                    if (int.TryParse(Console.ReadLine(), out int slices))
                    {
                        try
                        {
                            target.Slice(slices);
                            Console.WriteLine($"Успіх! Астероїд '{target.Name}' розрізано на {slices} шматків.");
                            Console.WriteLine($"Новий діаметр частки: {target.DiameterKm} км, орієнтовний об'єм частки: {target.EstimatedVolumeKm3} км³.");
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Помилка! {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Помилка! Введіть ціле число.");
                    }
                    break;

                case "3":
                    Console.Write("Введіть назву планети або супутника: ");
                    string planet = Console.ReadLine()!.Trim();
                    Console.WriteLine(target.CollideWithTarget(string.IsNullOrWhiteSpace(planet) ? "Марс" : planet));
                    break;

                default:
                    Console.WriteLine("Помилка! Невідомий метод.");
                    break;
            }
        }

        private static void SearchAsteroids()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Список порожній, пошук неможливий.");
                return;
            }

            Console.WriteLine("Оберіть критерій пошуку:");
            Console.WriteLine("1 – За типом бортика (Crust)");
            Console.WriteLine("2 – За наявністю подвійного сиру (HasExtraCheese)");
            Console.Write(">");
            string subChoice = Console.ReadLine()!.Trim();

            List<PizzaAsteroid> results;

            if (subChoice == "1")
            {
                Console.WriteLine("Оберіть шуканий бортик: 1 - Thin, 2 - CheeseStuffed, 3 - DeepDish, 4 - Classic");
                if (int.TryParse(Console.ReadLine(), out int raw) && Enum.IsDefined(typeof(CrustType), raw))
                {
                    CrustType crust = (CrustType)raw;
                    results = Asteroids.FindAll(a => a.Crust == crust);
                }
                else
                {
                    Console.WriteLine("Помилка вибору бортика.");
                    return;
                }
            }
            else if (subChoice == "2")
            {
                bool cheese = ReadValidatedBool("Шукати з додатковим сиром? (1/так, 0/ні): ");
                results = Asteroids.FindAll(a => a.HasExtraCheese == cheese);
            }
            else
            {
                Console.WriteLine("Помилка! Невірний критерій пошуку.");
                return;
            }

            PrintTable(results, "Результати пошуку");
        }

        private static void DeleteAsteroidMenu()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Список порожній, видалення неможливе.");
                return;
            }

            Console.WriteLine("Спосіб видалення:");
            Console.WriteLine("1 – За порядковим номером у таблиці");
            Console.WriteLine("2 – За назвою (будуть видалені всі збіги)");
            Console.Write(">");
            string choice = Console.ReadLine()!.Trim();

            if (choice == "1")
            {
                PrintTable(Asteroids, "Поточний перелік астероїдів");
                Console.Write($"Введіть номер для видалення (1..{Asteroids.Count}): ");
                if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= Asteroids.Count)
                {
                    string removedName = Asteroids[index - 1].Name;
                    Asteroids.RemoveAt(index - 1);
                    Console.WriteLine($"Успіх! Астероїд '{removedName}' успішно видалено.");
                }
                else
                {
                    Console.WriteLine("Помилка! Некоректний порядковий номер.");
                }
            }
            else if (choice == "2")
            {
                Console.Write("Введіть назву для пошуку та видалення: ");
                string searchName = Console.ReadLine()!.Trim();

                int removedCount = Asteroids.RemoveAll(a => a.Name.Equals(searchName, StringComparison.OrdinalIgnoreCase));
                if (removedCount > 0)
                {
                    Console.WriteLine($"Успіх! Видалено об'єктів: {removedCount}.");
                }
                else
                {
                    Console.WriteLine($"Об'єктів з назвою '{searchName}' не знайдено.");
                }
            }
            else
            {
                Console.WriteLine("Помилка! Некоректний вибір.");
            }
        }
    }
}