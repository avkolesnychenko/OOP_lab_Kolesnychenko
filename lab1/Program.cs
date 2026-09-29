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
            Console.WriteLine("2 - Згенерувати автоматично (випадковий конструктор)");
            Console.WriteLine("3 – Створити конструктором за замовчуванням з ініціалізатором");
            Console.Write(">");
            string mode = Console.ReadLine()!.Trim();

            if (mode == "1")
            {
                string name;
                while (true)
                {
                    try
                    {
                        Console.Write("Введіть назву астероїда (3-20 симв., літери/цифри/-): ");
                        string val = Console.ReadLine()!;
                        var temp = new PizzaAsteroid { Name = val };
                        name = temp.Name;
                        break;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"[Помилка назви]: {ex.Message}");
                    }
                }

                CrustType crust;
                while (true)
                {
                    Console.WriteLine("Оберіть бортик: 1 - Thin, 2 - CheeseStuffed, 3 - DeepDish, 4 - Classic");
                    Console.Write("Номер (1-4): ");
                    if (int.TryParse(Console.ReadLine(), out int raw) && Enum.IsDefined(typeof(CrustType), raw))
                    {
                        crust = (CrustType)raw;
                        break;
                    }
                    Console.WriteLine("Помилка! Введіть число від 1 до 4.");
                }

                double diameter;
                while (true)
                {
                    try
                    {
                        Console.Write("Введіть діаметр у км (0.1 .. 1000.0): ");
                        string raw = Console.ReadLine()!.Replace(',', '.');
                        if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                        {
                            var temp = new PizzaAsteroid { DiameterKm = val };
                            diameter = temp.DiameterKm;
                            break;
                        }
                        throw new FormatException("Введене значення не є числом.");
                    }
                    catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException)
                    {
                        Console.WriteLine($"[Помилка діаметра]: {ex.Message}");
                    }
                }

                int tempC;
                while (true)
                {
                    try
                    {
                        Console.Write("Введіть температуру в °C (-273 .. 500): ");
                        if (int.TryParse(Console.ReadLine(), out int val))
                        {
                            var temp = new PizzaAsteroid { TemperatureCelsius = val };
                            tempC = temp.TemperatureCelsius;
                            break;
                        }
                        throw new FormatException("Температура повинна бути цілим числом.");
                    }
                    catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException)
                    {
                        Console.WriteLine($"[Помилка температури]: {ex.Message}");
                    }
                }

                bool extraCheese = ReadValidatedBool("Чи є подвійний сир? (1/так, 0/ні): ");

                DateTime date;
                while (true)
                {
                    try
                    {
                        Console.Write("Введіть дату відкриття (dd.MM.yyyy, від 01.01.1990 до сьогодні): ");
                        string raw = Console.ReadLine()!.Trim();
                        if (DateTime.TryParseExact(raw, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                        {
                            var temp = new PizzaAsteroid();
                            temp.SetDiscoveryDate(d);
                            date = temp.DiscoveryDate;
                            break;
                        }
                        throw new FormatException("Формат дати має бути dd.MM.yyyy (наприклад, 12.04.2021).");
                    }
                    catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException)
                    {
                        Console.WriteLine($"[Помилка дати відкриття]: {ex.Message}");
                    }
                }

                Console.Write("Введіть назву соусу (Enter для 'Томатний Класик'): ");
                string sauceInput = Console.ReadLine()!.Trim();
                string sauce = string.IsNullOrWhiteSpace(sauceInput) ? "Томатний Класик" : sauceInput;

                PizzaAsteroid asteroid = new(name, crust, diameter, tempC, extraCheese, date, sauce);
                Asteroids.Add(asteroid);
                Console.WriteLine($"\n[Повідомлення конструктора]: Спрацював повний конструктор PizzaAsteroid(name, crust, diameterKm, temp, cheese, date, sauce).");
                Console.WriteLine($"Успіх! Астероїд '{asteroid.Name}' додано!");
            }
            else if (mode == "2")
            {
                var rand = new Random();
                string[] baseNames = { "Пепероні-X", "Квадро-Формаджо", "Карбонара-99", "Гаваї-Ультра", "Діавола-Prime" };
                string genName = baseNames[rand.Next(baseNames.Length)] + "-" + rand.Next(10, 999);
                CrustType genCrust = (CrustType)rand.Next(1, 5);
                double genDiameter = Math.Round(rand.NextDouble() * 90 + 5, 2);
                int genTemp = rand.Next(-150, 250);
                bool genCheese = rand.Next(2) == 1;
                DateTime genDate = DateTime.Now.Date.AddDays(-rand.Next(10, 4000));

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
                        Console.WriteLine("[Повідомлення конструктора]: Спрацював конструктор без параметрів PizzaAsteroid() з ініціалізатором об'єкта.");
                        break;
                    case 2:
                        newObj = new PizzaAsteroid(genName, genCrust, genDiameter);
                        Console.WriteLine("[Повідомлення конструктора]: Спрацював конструктор із 3 параметрами PizzaAsteroid(name, crust, diameterKm).");
                        break;
                    case 3:
                        newObj = new PizzaAsteroid(genName, genCrust, genDiameter, genTemp, genCheese);
                        Console.WriteLine("[Повідомлення конструктора]: Спрацював конструктор із 5 параметрами PizzaAsteroid(name, crust, diameter, temp, cheese) через ланцюжок this(...).");
                        break;
                    default:
                        newObj = new PizzaAsteroid(genName, genCrust, genDiameter, genTemp, genCheese, genDate, "Сирний Спешл");
                        Console.WriteLine("[Повідомлення конструктора]: Спрацював повний перевантажений конструктор із 7 параметрами.");
                        break;
                }

                Asteroids.Add(newObj);
                Console.WriteLine($"Успіх! Автоматично згенеровано та додано: '{newObj.Name}'.");
            }
            else if (mode == "3")
            {
                PizzaAsteroid initAsteroid = new PizzaAsteroid
                {
                    Name = "Класичний-Базовий",
                    Crust = CrustType.CheeseStuffed,
                    SauceType = "Часниковий Ранч",
                    DiameterKm = 42.5,
                    TemperatureCelsius = 60,
                    HasExtraCheese = true
                };

                Asteroids.Add(initAsteroid);
                Console.WriteLine("[Повідомлення конструктора]: Спрацював конструктор без параметрів PizzaAsteroid() разом із блоком ініціалізатора { ... }.");
                Console.WriteLine($"Успіх! Додано об'єкт '{initAsteroid.Name}'.");
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
            Console.WriteLine("1 – Нагріти астероїд (Демонстрація ПЕРЕВАНТАЖЕНИХ версій HeatUp)");
            Console.WriteLine("2 – Розрізати астероїд на шматки (Slice)");
            Console.WriteLine("3 – Створити зіткнення з планетою (CollideWithTarget)");
            Console.Write(">");

            switch (Console.ReadLine()!.Trim())
            {
                case "1":
                    Console.WriteLine("\nОберіть сигнатуру виклику перевантаженого методу HeatUp:");
                    Console.WriteLine("1) HeatUp(int degrees) — звичайний нагрів");
                    Console.WriteLine("2) HeatUp(int degrees, string heatSource) — нагрів із зазначенням космічного джерела");
                    Console.WriteLine("3) HeatUp(double factor) — інтенсивний імпульсний нагрів через множник");
                    Console.Write(">");
                    string heatChoice = Console.ReadLine()!.Trim();

                    if (heatChoice == "1")
                    {
                        Console.Write("Введіть градуси нагріву (int): ");
                        if (int.TryParse(Console.ReadLine(), out int deg)) target.HeatUp(deg);
                        else Console.WriteLine("Помилка! Некоректне значення.");
                    }
                    else if (heatChoice == "2")
                    {
                        Console.Write("Введіть градуси нагріву (int): ");
                        if (int.TryParse(Console.ReadLine(), out int deg))
                        {
                            Console.Write("Введіть назву джерела (наприклад, 'Спалах супернової', 'Лазер шатла'): ");
                            string src = Console.ReadLine()!;
                            target.HeatUp(deg, src);
                        }
                        else Console.WriteLine("Помилка! Некоректне значення.");
                    }
                    else if (heatChoice == "3")
                    {
                        Console.Write("Введіть коефіцієнт нагріву (double > 1.0, наприклад 1.5): ");
                        string raw = Console.ReadLine()!.Replace(',', '.');
                        if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double factor))
                        {
                            target.HeatUp(factor);
                        }
                        else Console.WriteLine("Помилка! Некоректне дійсне число.");
                    }
                    else
                    {
                        Console.WriteLine("Помилка! Невідома сигнатура методу.");
                    }
                    break;

                case "2":
                    Console.Write("На скільки частин розрізати (>= 2)?: ");
                    if (int.TryParse(Console.ReadLine(), out int slices))
                    {
                        try { target.Slice(slices); }
                        catch (ArgumentException ex) { Console.WriteLine($"Помилка! {ex.Message}"); }
                    }
                    else Console.WriteLine("Помилка! Введіть ціле число.");
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

        private static bool ReadValidatedBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()!.Trim().ToLower();
                if (input == "1" || input == "так" || input == "true") return true;
                if (input == "0" || input == "ні" || input == "false") return false;
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