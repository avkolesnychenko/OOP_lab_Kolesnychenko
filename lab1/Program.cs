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
                Console.WriteLine("6 – Продемонструвати static-методи");
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
                        Console.WriteLine($"Усього коректно створено об'єктів PizzaAsteroid (PizzaAsteroid.CreatedCount): {PizzaAsteroid.CreatedCount}");
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
                    case "6":
                        DemonstrateStaticMethods();
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
            Console.WriteLine("1 – Ввести дані вручну");
            Console.WriteLine("2 – Згенерувати випадковим конструктором");
            Console.WriteLine("3 – Створити за замовчуванням");
            Console.WriteLine("4 – Ввести рядком (TryParse)");
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

                        Console.WriteLine("[Конструктор]: Спрацював основний 7-параметричний конструктор PizzaAsteroid(name, crust, diameterKm, temp, cheese, date, sauce).");
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
                        Console.WriteLine("[Конструктор]: Спрацював конструктор без параметрів PizzaAsteroid() (через : this(...) викликав основний) разом з ініціалізатором об'єкта { Name, Crust, DiameterKm }.");
                        break;
                    case 2:
                        newObj = new PizzaAsteroid(genName, genCrust, genDiameter);
                        Console.WriteLine("[Конструктор]: Спрацював скорочений конструктор PizzaAsteroid(name, crust, diameterKm), який через : this(...) викликав основний 7-параметричний.");
                        break;
                    case 3:
                        newObj = new PizzaAsteroid(genName, genCrust, genDiameter, genTemp, genCheese);
                        Console.WriteLine("[Конструктор]: Спрацював 5-параметричний конструктор PizzaAsteroid(name, crust, diameterKm, temp, cheese), який через : this(...) викликав основний 7-параметричний.");
                        break;
                    default:
                        newObj = new PizzaAsteroid(genName, genCrust, genDiameter, genTemp, genCheese, genDate, "Грибний Соус");
                        Console.WriteLine("[Конструктор]: Спрацював основний 7-параметричний конструктор.");
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
                Console.WriteLine("[Конструктор]: Спрацював конструктор без параметрів PizzaAsteroid() разом з ініціалізатором об'єкта.");
                Console.WriteLine($"Успіх! Створено об'єкт '{defaultInit.Name}'.");
            }
            else if (mode == "4")
            {
                AddFromString();
            }
            else
            {
                Console.WriteLine("Помилка! Некоректний режим додавання.");
            }
        }

        private static void AddFromString()
        {
            PrintStringFormatHint();

            while (true)
            {
                Console.Write("Введіть рядок (порожній рядок — скасувати): ");
                string input = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Додавання скасовано.");
                    return;
                }

                if (PizzaAsteroid.TryParse(input, out PizzaAsteroid? parsed) && parsed != null)
                {
                    Asteroids.Add(parsed);
                    Console.WriteLine("[TryParse]: Рядок успішно перетворено на об'єкт.");
                    Console.WriteLine($"Успіх! Астероїд '{parsed.Name}' додано. ToString(): {parsed}");
                    return;
                }

                Console.WriteLine("[TryParse]: Не вдалося перетворити рядок на об'єкт (TryParse повернув false).");
                Console.WriteLine("Перевірте формат і значення полів. Щоб побачити точну причину, скористайтеся пунктом 6 → Parse.\n");
            }
        }

        private static void DemonstrateStaticMethods()
        {
            Console.WriteLine("Демонстрація static-членів класу PizzaAsteroid:");
            Console.WriteLine("1 – Static-властивість ImpactEnergyFactor (переглянути / змінити)");
            Console.WriteLine("2 – PizzaAsteroid.Parse(string)    (з повідомленням про помилку)");
            Console.WriteLine("3 – PizzaAsteroid.TryParse(string, out obj)");
            Console.WriteLine("4 – ToString() усіх об'єктів (формат, сумісний з Parse)");
            Console.WriteLine("5 – PizzaAsteroid.Merge(a, b, name) — злиття двох астероїдів");
            Console.WriteLine("6 – PizzaAsteroid.TotalVolume(list) — сумарний об'єм");
            Console.Write(">");
            string choice = Console.ReadLine()!.Trim();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    DemoImpactFactor();
                    break;
                case "2":
                    DemoParse();
                    break;
                case "3":
                    DemoTryParse();
                    break;
                case "4":
                    DemoToString();
                    break;
                case "5":
                    DemoMerge();
                    break;
                case "6":
                    DemoTotalVolume();
                    break;
                default:
                    Console.WriteLine("Помилка! Невідома опція.");
                    break;
            }
        }

        private static void DemoImpactFactor()
        {
            Console.WriteLine($"Поточне значення PizzaAsteroid.ImpactEnergyFactor = {PizzaAsteroid.ImpactEnergyFactor} Мт ТНТ / км³ (спільне для всіх астероїдів).");
            if (!ReadValidatedBool("Змінити значення? (1/так, 0/ні): "))
                return;

            double newValue = ReadDouble("Введіть новий коефіцієнт (0.1 – 100.0): ");
            try
            {
                double old = PizzaAsteroid.ImpactEnergyFactor;
                PizzaAsteroid.ImpactEnergyFactor = newValue;
                Console.WriteLine($"Успіх! ImpactEnergyFactor: {old} -> {PizzaAsteroid.ImpactEnergyFactor}.");
                Console.WriteLine("Зміна вплине на CollideWithTarget() УСІХ астероїдів одночасно.");

                if (Asteroids.Count > 0)
                    Console.WriteLine("Приклад: " + Asteroids[0].CollideWithTarget("Марс"));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[Помилка]: {ex.Message}");
            }
        }

        private static void DemoParse()
        {
            PrintStringFormatHint();
            Console.Write("Введіть рядок для Parse: ");
            string input = Console.ReadLine() ?? string.Empty;
            int countBefore = PizzaAsteroid.CreatedCount;

            try
            {
                PizzaAsteroid obj = PizzaAsteroid.Parse(input);
                Console.WriteLine("Parse успішний! Отримано об'єкт:");
                PrintTable([obj], "Результат Parse");

                if (Asteroids.Count < _maxCapacity && ReadValidatedBool("Додати цей об'єкт до списку? (1/так, 0/ні): "))
                {
                    Asteroids.Add(obj);
                    Console.WriteLine($"Об'єкт '{obj.Name}' додано до списку.");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"[FormatException]: {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[ArgumentOutOfRangeException]: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[{ex.GetType().Name}]: {ex.Message}");
            }

            Console.WriteLine($"CreatedCount: {countBefore} -> {PizzaAsteroid.CreatedCount}");
        }

        private static void DemoTryParse()
        {
            PrintStringFormatHint();
            Console.Write("Введіть рядок для TryParse: ");
            string input = Console.ReadLine() ?? string.Empty;

            bool ok = PizzaAsteroid.TryParse(input, out PizzaAsteroid? obj);
            Console.WriteLine($"TryParse повернув: {ok}");

            if (ok && obj != null)
            {
                PrintTable([obj], "Результат TryParse");
                if (Asteroids.Count < _maxCapacity && ReadValidatedBool("Додати цей об'єкт до списку? (1/так, 0/ні): "))
                {
                    Asteroids.Add(obj);
                    Console.WriteLine($"Об'єкт '{obj.Name}' додано до списку.");
                }
            }
            else
            {
                Console.WriteLine("obj = null. Виняток не виник — TryParse перехопив його всередині.");
            }
        }

        private static void DemoToString()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            Console.WriteLine("Результат ToString() для кожного об'єкта:");
            for (int i = 0; i < Asteroids.Count; i++)
                Console.WriteLine($"{i + 1}. {Asteroids[i]}");

            string original = Asteroids[0].ToString();
            bool roundTrip = PizzaAsteroid.TryParse(original, out PizzaAsteroid? copy) && copy!.ToString() == original;
            Console.WriteLine($"\nПеревірка сумісності ToString ↔ Parse для об'єкта №1: {(roundTrip ? "рядки збігаються ✔" : "НЕ збігаються ✘")}");
            Console.WriteLine("(Примітка: під час перевірки створено тимчасову копію, тому CreatedCount збільшився на 1.)");
        }

        private static void DemoMerge()
        {
            if (Asteroids.Count < 2)
            {
                Console.WriteLine("Для злиття потрібно щонайменше 2 об'єкти у списку.");
                return;
            }

            PrintTable(Asteroids, "Оберіть два астероїди для злиття");
            int i1 = ReadIntInRange($"Номер першого (1..{Asteroids.Count}): ", 1, Asteroids.Count);
            int i2 = ReadIntInRange($"Номер другого (1..{Asteroids.Count}): ", 1, Asteroids.Count);
            Console.Write("Назва нового астероїда: ");
            string newName = Console.ReadLine() ?? string.Empty;

            try
            {
                PizzaAsteroid a = Asteroids[i1 - 1];
                PizzaAsteroid b = Asteroids[i2 - 1];
                PizzaAsteroid merged = PizzaAsteroid.Merge(a, b, newName);

                Console.WriteLine($"Об'єм: {a.EstimatedVolumeKm3} + {b.EstimatedVolumeKm3} ≈ {merged.EstimatedVolumeKm3} км³");
                PrintTable([merged], "Результат Merge");

                if (ReadValidatedBool("Замінити два вихідні астероїди новим у списку? (1/так, 0/ні): "))
                {
                    Asteroids.Remove(a);
                    Asteroids.Remove(b);
                    Asteroids.Add(merged);
                    Console.WriteLine($"Успіх! '{a.Name}' і '{b.Name}' злито в '{merged.Name}'.");
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine($"[Помилка Merge]: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[Помилка Merge]: {ex.Message}");
            }
        }

        private static void DemoTotalVolume()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Список порожній, сумарний об'єм = 0 км³.");
                return;
            }

            double total = PizzaAsteroid.TotalVolume(Asteroids);
            Console.WriteLine($"PizzaAsteroid.TotalVolume(список з {Asteroids.Count} об'єктів) = {total} км³");
            Console.WriteLine($"Сумарна енергія, якщо всі впадуть на одну планету: {Math.Round(total * PizzaAsteroid.ImpactEnergyFactor, 2)} Мт ТНТ");
        }

        private static void PrintStringFormatHint()
        {
            Console.WriteLine("Формат рядка: Назва;Бортик;Діаметр;Температура;Сир;Дата[;Соус]");
            Console.WriteLine("  Бортик: Thin, CheeseStuffed, DeepDish, Classic або 1–4; Сир: так/ні; Дата: dd.MM.yyyy");
            Console.WriteLine("  Приклад: Пепероні-X;Thin;12.5;-40;так;12.04.2021;Барбекю");
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
                    int tempBefore = target.TemperatureCelsius;

                    try
                    {
                        if (choice == "1")
                        {
                            int deg = ReadInt("Введіть градуси нагріву (int): ");
                            target.HeatUp(deg);
                            Console.WriteLine("Викликано перевантаження: HeatUp(int degrees)");
                            Console.WriteLine($"Успіх! T: {tempBefore}°C -> {target.TemperatureCelsius}°C.");
                        }
                        else if (choice == "2")
                        {
                            int deg = ReadInt("Введіть градуси нагріву (int): ");
                            Console.Write("Введіть назву джерела (наприклад, 'Сонячний спалах'): ");
                            string src = Console.ReadLine()!;
                            string report = target.HeatUp(deg, src);
                            Console.WriteLine("Викликано перевантаження: HeatUp(int degrees, string heatSource)");
                            Console.WriteLine(report);
                            Console.WriteLine($"Успіх! T: {tempBefore}°C -> {target.TemperatureCelsius}°C.");
                        }
                        else if (choice == "3")
                        {
                            double factor = ReadDouble("Введіть коефіцієнт нагріву (1.0 < k <= 10.0, наприклад 1.5): ");
                            target.HeatUp(factor);
                            Console.WriteLine("Викликано перевантаження: HeatUp(double factor)");
                            Console.WriteLine($"Успіх! T: {tempBefore}°C -> {target.TemperatureCelsius}°C.");
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
    }
}