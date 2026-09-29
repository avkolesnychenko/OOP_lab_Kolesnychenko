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
            Console.WriteLine("2 - Згенерувати випадковим конструктором");
            Console.WriteLine("3 – Створити за замовчуванням");
            Console.Write(">");
            string mode = Console.ReadLine()!.Trim();

            if (mode == "1")
            {
                while (true)
                {
                    try
                    {
                        Console.Write("Введіть назву астероїда: ");
                        string name = Console.ReadLine()!;

                        Console.WriteLine("Оберіть бортик: 1 - Thin, 2 - CheeseStuffed, 3 - DeepDish, 4 - Classic");
                        CrustType crust = (CrustType)ReadIntInRange("Номер (1-4): ", 1, 4);

                        double diameter = ReadDouble("Введіть діаметр у км: ");
                        int tempC = ReadInt("Введіть температуру в °C: ");
                        bool extraCheese = ReadValidatedBool("Чи є подвійний сир? (1/так, 0/ні): ");
                        DateTime date = ReadDate("Введіть дату відкриття (dd.MM.yyyy): ");

                        Console.Write("Введіть назву соусу (Enter для 'Томатний Класик'): ");
                        string sauceInput = Console.ReadLine()!.Trim();
                        string sauce = string.IsNullOrWhiteSpace(sauceInput) ? "Томатний Класик" : sauceInput;

                        PizzaAsteroid asteroid = new(name, crust, diameter, tempC, extraCheese, date, sauce);
                        Asteroids.Add(asteroid);

                        Console.WriteLine("[Конструктор]: Спрацював повний конструктор PizzaAsteroid(name, crust, diameterKm, temp, cheese, date, sauce).");
                        Console.WriteLine($"Успіх! Астероїд '{asteroid.Name}' успішно створено!");
                        break;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        Console.WriteLine($"[Помилка валідації]: {ex.Message}\nСпробуйте ввести дані заново.\n");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"[Помилка валідації]: {ex.Message}\nСпробуйте ввести дані заново.\n");
                    }
                }
            }
            else if (mode == "2")
            {
                var rand = new Random();
                string[] names = { "Пепероні-X", "Квадро-Формаджо", "Карбонара-99", "Гаваї-Ультра", "Діавола-Prime" };
                string genName = names[rand.Next(names.Length)] + "-" + rand.Next(10, 999);
                CrustType genCrust = (CrustType)rand.Next(1, 5);
                double genDiameter = Math.Round(rand.NextDouble() * 95 + 1, 2);
                int genTemp = rand.Next(-100, 200);
                bool genCheese = rand.Next(2) == 1;
                DateTime genDate = DateTime.Now.Date.AddDays(-rand.Next(10, 3000));

                int ctorIndex = rand.Next(1, 5);
                PizzaAsteroid newObj;

                switch (ctorIndex)
                {
                    case 1:
                        newObj = new PizzaAsteroid
                        {
                            Name = genName,
                            Crust = genCrust,
                            DiameterKm = genDiameter
                        };
                        Console.WriteLine("[Конструктор]: Спрацював конструктор без параметрів PizzaAsteroid().");
                        break;
                    case 2:
                        newObj = new PizzaAsteroid(genName, genCrust, genDiameter);
                        Console.WriteLine("[Конструктор]: Спрацював скорочений конструктор PizzaAsteroid(name, crust, diameterKm).");
                        break;
                    case 3:
                        newObj = new PizzaAsteroid(genName, genCrust, genDiameter, genTemp, genCheese);
                        Console.WriteLine("[Конструктор]: Спрацював 5-параметричний конструктор через ланцюжок : this(...).");
                        break;
                    default:
                        newObj = new PizzaAsteroid(genName, genCrust, genDiameter, genTemp, genCheese, genDate, "Грибний Соус");
                        Console.WriteLine("[Конструктор]: Спрацював повний перевантажений конструктор.");
                        break;
                }

                Asteroids.Add(newObj);
                Console.WriteLine($"Успіх! Автоматично створено: '{newObj.Name}'.");
            }
            else if (mode == "3")
            {
                PizzaAsteroid defaultInit = new()
                {
                    Name = "Класичний-Базовий",
                    Crust = CrustType.CheeseStuffed,
                    SauceType = "Часниковий",
                    DiameterKm = 50.0,
                    TemperatureCelsius = 25,
                    HasExtraCheese = true
                };

                Asteroids.Add(defaultInit);
                Console.WriteLine("[Конструктор]: Спрацював конструктор без параметрів разом.");
                Console.WriteLine($"Успіх! Створено об'єкт '{defaultInit.Name}'.");
            }
            else
            {
                Console.WriteLine("Помилка! Некоректний режим додавання.");
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
            int idx = ReadIntInRange($"Введіть порядковий номер об'єкта (1..{Asteroids.Count}): ", 1, Asteroids.Count);
            PizzaAsteroid target = Asteroids[idx - 1];

            Console.WriteLine($"\nОберіть дію для астероїда '{target.Name}':");
            Console.WriteLine("1 – Нагріти астероїд (Демонстрація ПЕРЕВАНТАЖЕНИХ версій HeatUp)");
            Console.WriteLine("2 – Розрізати астероїд на шматки (Slice)");
            Console.WriteLine("3 – Створити зіткнення з планетою (CollideWithTarget)");
            Console.Write(">");

            switch (Console.ReadLine()!.Trim())
            {
                case "1":
                    Console.WriteLine("\nОберіть сигнатуру виклику HeatUp:");
                    Console.WriteLine("1) HeatUp(int degrees)");
                    Console.WriteLine("2) HeatUp(int degrees, string heatSource)");
                    Console.WriteLine("3) HeatUp(double factor)");
                    Console.Write(">");
                    string choice = Console.ReadLine()!.Trim();

                    try
                    {
                        if (choice == "1")
                        {
                            int deg = ReadInt("Введіть градуси нагріву (int): ");
                            target.HeatUp(deg);
                            Console.WriteLine($"[Результат HeatUp(int)]: {target.GetThermalReport()}");
                        }
                        else if (choice == "2")
                        {
                            int deg = ReadInt("Введіть градуси нагріву (int): ");
                            Console.Write("Введіть назву джерела (наприклад, 'Сонячний спалах'): ");
                            string src = Console.ReadLine()!;
                            target.HeatUp(deg, src);
                            Console.WriteLine($"[Результат HeatUp(int, string)]: {target.GetThermalReport()}");
                        }
                        else if (choice == "3")
                        {
                            double factor = ReadDouble("Введіть коефіцієнт нагріву (double > 1.0, наприклад 1.5): ");
                            target.HeatUp(factor);
                            Console.WriteLine($"[Результат HeatUp(double)]: {target.GetThermalReport()}");
                        }
                        else
                        {
                            Console.WriteLine("Помилка! Невідома опція.");
                        }
                    }
                    catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is ArgumentException)
                    {
                        Console.WriteLine($"[Помилка HeatUp]: {ex.Message}");
                    }
                    break;

                case "2":
                    int slices = ReadInt("На скільки частин розрізати (>= 2)?: ");
                    try
                    {
                        target.Slice(slices);
                        Console.WriteLine($"Успіх! Астероїд розрізано. Новий діаметр: {target.DiameterKm} км, об'єм: {target.EstimatedVolumeKm3} км³.");
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"[Помилка Slice]: {ex.Message}");
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
            string choice = Console.ReadLine()!.Trim();

            List<PizzaAsteroid> results;
            if (choice == "1")
            {
                Console.WriteLine("Оберіть шуканий бортик: 1 - Thin, 2 - CheeseStuffed, 3 - DeepDish, 4 - Classic");
                CrustType crust = (CrustType)ReadIntInRange("Номер: ", 1, 4);
                results = Asteroids.FindAll(a => a.Crust == crust);
            }
            else if (choice == "2")
            {
                bool cheese = ReadValidatedBool("Шукати з додатковим сиром? (1/так, 0/ні): ");
                results = Asteroids.FindAll(a => a.HasExtraCheese == cheese);
            }
            else
            {
                Console.WriteLine("Помилка вибору критерію.");
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
                int idx = ReadIntInRange($"Введіть номер для видалення (1..{Asteroids.Count}): ", 1, Asteroids.Count);
                string name = Asteroids[idx - 1].Name;
                Asteroids.RemoveAt(idx - 1);
                Console.WriteLine($"Успіх! Астероїд '{name}' успішно видалено.");
            }
            else if (choice == "2")
            {
                Console.Write("Введіть назву для пошуку та видалення: ");
                string name = Console.ReadLine()!.Trim();
                int count = Asteroids.RemoveAll(a => a.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                Console.WriteLine(count > 0 ? $"Успіх! Видалено об'єктів: {count}." : $"Об'єктів з назвою '{name}' не знайдено.");
            }
            else
            {
                Console.WriteLine("Помилка вибору.");
            }
        }

        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int val)) return val;
                Console.WriteLine("Помилка! Введіть коректне ціле число.");
            }
        }

        private static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                int val = ReadInt(prompt);
                if (val > 0) return val;
                Console.WriteLine("Помилка! Число має бути більшим за нуль.");
            }
        }

        private static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                int val = ReadInt(prompt);
                if (val >= min && val <= max) return val;
                Console.WriteLine($"Помилка! Число повинно бути в межах від {min} до {max}.");
            }
        }

        private static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string raw = Console.ReadLine()!.Replace(',', '.');
                if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double val)) return val;
                Console.WriteLine("Помилка! Введіть коректне дійсне число.");
            }
        }

        private static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string raw = Console.ReadLine()!.Trim();
                if (DateTime.TryParseExact(raw, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d)) return d;
                Console.WriteLine("Помилка! Формат дати має бути dd.MM.yyyy (наприклад, 12.04.2021).");
            }
        }

        private static bool ReadValidatedBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()!.Trim().ToLower();
                if (input is "1" or "так" or "true") return true;
                if (input is "0" or "ні" or "false") return false;
                Console.WriteLine("Помилка! Введіть 1/так або 0/ні.");
            }
        }

        private static void PrintTable(List<PizzaAsteroid> list, string title)
        {
            if (list == null || list.Count == 0)
            {
                Console.WriteLine("Жодного об'єкта не знайдено.");
                return;
            }

            Console.WriteLine($"\n{title}");
            Console.WriteLine(new string('-', 135));
            Console.WriteLine($"| {"#",-3} | {"Назва",-18} | {"Бортик",-14} | {"Соус",-16} | {"Діаметр",-9} | {"Об'єм (км³)",-12} | {"T (°C)",-7} | {"Сир+",-6} | {"Відкрито",-10} |");
            Console.WriteLine(new string('-', 135));

            for (int i = 0; i < list.Count; i++)
            {
                var a = list[i];
                Console.WriteLine($"| {i + 1,-3} | {a.Name,-18} | {a.Crust,-14} | {a.SauceType,-16} | {a.DiameterKm,-9:F2} | {a.EstimatedVolumeKm3,-12:F2} | {a.TemperatureCelsius,-7} | {(a.HasExtraCheese ? "Так" : "Ні"),-6} | {a.DiscoveryDate,-10:dd.MM.yyyy} |");
            }
            Console.WriteLine(new string('-', 135));
        }

        private static bool ReadValidatedBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()!.Trim().ToLower();
                if (input is "1" or "так" or "true") return true;
                if (input is "0" or "ні" or "false") return false;
                Console.WriteLine("Помилка! Введіть 1/так або 0/ні.");
            }
        }

        private static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (int.TryParse(Console.ReadLine(), out int n) && n > 0)
                {
                    return n;
                }
                Console.WriteLine("Помилка! Число повинно бути цілим і більшим за нуль.");
            }
        }

        private static void PrintTable(List<PizzaAsteroid> list, string title)
        {
            if (list == null || list.Count == 0)
            {
                Console.WriteLine("Жодного об'єкта не знайдено.");
                return;
            }

            Console.WriteLine($"\n{title}");
            Console.WriteLine(new string('-', 135));
            Console.WriteLine($"| {"#",-3} | {"Назва",-18} | {"Бортик",-14} | {"Соус",-16} | {"Діаметр",-9} | {"Об'єм (км³)",-12} | {"T (°C)",-7} | {"Сир+",-6} | {"Відкрито",-10} |");
            Console.WriteLine(new string('-', 135));

            for (int i = 0; i < list.Count; i++)
            {
                var a = list[i];
                Console.WriteLine($"| {i + 1,-3} | {a.Name,-18} | {a.Crust,-14} | {a.SauceType,-16} | {a.DiameterKm,-9:F2} | {a.EstimatedVolumeKm3,-12:F2} | {a.TemperatureCelsius,-7} | {(a.HasExtraCheese ? "Так" : "Ні"),-6} | {a.DiscoveryDate,-10:dd.MM.yyyy} |");
            }
            Console.WriteLine(new string('-', 135));
        }
    }
}