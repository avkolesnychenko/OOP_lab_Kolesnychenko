using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace PizzaAsteroidApp
{
    public class Program
    {
        private const string DateFormat = "dd.MM.yyyy";
        private const string DefaultSauce = "Томатний Класик";

        private static readonly string[] RandomNames =
            { "Пепероні-X", "Квадро-Формаджо", "Карбонара-99", "Гаваї-Ультра", "Діавола-Prime" };

        private static readonly List<PizzaAsteroid> Asteroids = [];
        private static int _maxCapacity;

        public static bool IsFull(List<PizzaAsteroid> list, int maxCapacity)
        {
            ArgumentNullException.ThrowIfNull(list);
            if (maxCapacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxCapacity), "Максимальна кількість об'єктів має бути більшою за нуль.");
            return list.Count >= maxCapacity;
        }

        public static bool TryAdd(List<PizzaAsteroid> list, PizzaAsteroid asteroid, int maxCapacity)
        {
            ArgumentNullException.ThrowIfNull(asteroid);
            if (IsFull(list, maxCapacity))
                return false;
            if (list.Contains(asteroid))
                throw new ArgumentException("Цей об'єкт уже є у списку.", nameof(asteroid));

            list.Add(asteroid);
            return true;
        }

        public static bool TryAddFromString(List<PizzaAsteroid> list, string? input, int maxCapacity, out PizzaAsteroid? added)
        {
            added = null;
            if (IsFull(list, maxCapacity))
                return false;

            if (input is null || !PizzaAsteroid.TryParse(input, out PizzaAsteroid? parsed) || parsed is null)
                return false;

            list.Add(parsed);
            added = parsed;
            return true;
        }

        public static PizzaAsteroid CreateDefault()
        {
            return new PizzaAsteroid
            {
                Name = "Класичний-Базовий",
                Crust = CrustType.CheeseStuffed,
                SauceType = "Часниковий",
                DiameterKm = 50.0,
                TemperatureCelsius = 25,
                HasExtraCheese = true
            };
        }

        public static PizzaAsteroid CreateRandom(Random rand, out int constructorIndex)
        {
            ArgumentNullException.ThrowIfNull(rand);

            string genName = RandomNames[rand.Next(RandomNames.Length)] + "-" + rand.Next(10, 999);
            CrustType genCrust = (CrustType)rand.Next(1, 5);
            double genDiameter = Math.Round(rand.NextDouble() * 95 + 1, 2);
            int genTemp = rand.Next(-100, 200);
            bool genCheese = rand.Next(2) == 1;
            DateTime genDate = DateTime.Today.AddDays(-rand.Next(10, 3000));

            constructorIndex = rand.Next(1, 5);
            return constructorIndex switch
            {
                1 => new PizzaAsteroid { Name = genName, Crust = genCrust, DiameterKm = genDiameter },
                2 => new PizzaAsteroid(genName, genCrust, genDiameter),
                3 => new PizzaAsteroid(genName, genCrust, genDiameter, genTemp, genCheese),
                _ => new PizzaAsteroid(genName, genCrust, genDiameter, genTemp, genCheese, genDate, "Грибний Соус")
            };
        }

        public static string GetConstructorDescription(int constructorIndex)
        {
            return constructorIndex switch
            {
                1 => "конструктор без параметрів PizzaAsteroid() (через : this(...) викликав основний) разом з ініціалізатором об'єкта { Name, Crust, DiameterKm }.",
                2 => "скорочений конструктор PizzaAsteroid(name, crust, diameterKm), який через : this(...) викликав основний 7-параметричний.",
                3 => "5-параметричний конструктор PizzaAsteroid(name, crust, diameterKm, temp, cheese), який через : this(...) викликав основний 7-параметричний.",
                4 => "основний 7-параметричний конструктор PizzaAsteroid(name, crust, diameterKm, temp, cheese, date, sauce).",
                _ => throw new ArgumentOutOfRangeException(nameof(constructorIndex), "Номер конструктора має бути від 1 до 4.")
            };
        }

        public static PizzaAsteroid GetByNumber(List<PizzaAsteroid> list, int number)
        {
            ArgumentNullException.ThrowIfNull(list);
            if (number < 1 || number > list.Count)
                throw new ArgumentOutOfRangeException(nameof(number), $"Номер має бути в межах від 1 до {list.Count}.");
            return list[number - 1];
        }

        public static List<PizzaAsteroid> FindByCrust(List<PizzaAsteroid> list, CrustType crust)
        {
            ArgumentNullException.ThrowIfNull(list);
            if (!Enum.IsDefined(typeof(CrustType), crust))
                throw new ArgumentException("Невідомий тип бортика.", nameof(crust));
            return list.FindAll(a => a.Crust == crust);
        }

        public static List<PizzaAsteroid> FindByCheese(List<PizzaAsteroid> list, bool hasExtraCheese)
        {
            ArgumentNullException.ThrowIfNull(list);
            return list.FindAll(a => a.HasExtraCheese == hasExtraCheese);
        }

        public static PizzaAsteroid? FindByName(List<PizzaAsteroid> list, string name)
        {
            ArgumentNullException.ThrowIfNull(list);
            string key = ValidateName(name);
            return list.Find(a => a.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public static int FindIndexByName(List<PizzaAsteroid> list, string name)
        {
            ArgumentNullException.ThrowIfNull(list);
            string key = ValidateName(name);
            return list.FindIndex(a => a.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public static bool ExistsWithName(List<PizzaAsteroid> list, string name)
        {
            ArgumentNullException.ThrowIfNull(list);
            string key = ValidateName(name);
            return list.Exists(a => a.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public static List<PizzaAsteroid> FindByDiameterRange(List<PizzaAsteroid> list, double minKm, double maxKm)
        {
            ArgumentNullException.ThrowIfNull(list);
            if (double.IsNaN(minKm) || minKm < 0)
                throw new ArgumentOutOfRangeException(nameof(minKm), "Мінімальний діаметр не може бути від'ємним.");
            if (double.IsNaN(maxKm) || maxKm < 0)
                throw new ArgumentOutOfRangeException(nameof(maxKm), "Максимальний діаметр не може бути від'ємним.");
            if (minKm > maxKm)
                throw new ArgumentException("Мінімальний діаметр не може перевищувати максимальний.", nameof(minKm));

            return list.FindAll(a => a.DiameterKm >= minKm && a.DiameterKm <= maxKm);
        }

        public static PizzaAsteroid RemoveAt(List<PizzaAsteroid> list, int number)
        {
            PizzaAsteroid removed = GetByNumber(list, number);
            list.RemoveAt(number - 1);
            return removed;
        }

        public static bool Remove(List<PizzaAsteroid> list, PizzaAsteroid asteroid)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(asteroid);
            return list.Remove(asteroid);
        }

        public static int RemoveByName(List<PizzaAsteroid> list, string name)
        {
            ArgumentNullException.ThrowIfNull(list);
            string key = ValidateName(name);
            return list.RemoveAll(a => a.Name.Equals(key, StringComparison.OrdinalIgnoreCase));
        }

        public static int Clear(List<PizzaAsteroid> list)
        {
            ArgumentNullException.ThrowIfNull(list);
            int count = list.Count;
            list.Clear();
            return count;
        }

        public static void SortByName(List<PizzaAsteroid> list)
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.CurrentCultureIgnoreCase));
        }

        public static void SortByDiameter(List<PizzaAsteroid> list)
        {
            ArgumentNullException.ThrowIfNull(list);
            list.Sort((x, y) => x.DiameterKm.CompareTo(y.DiameterKm));
        }

        public static PizzaAsteroid Merge(List<PizzaAsteroid> list, int number1, int number2, string newName)
        {
            PizzaAsteroid a = GetByNumber(list, number1);
            PizzaAsteroid b = GetByNumber(list, number2);
            return PizzaAsteroid.Merge(a, b, newName);
        }

        public static void ReplaceWithMerged(List<PizzaAsteroid> list, PizzaAsteroid a, PizzaAsteroid b, PizzaAsteroid merged)
        {
            ArgumentNullException.ThrowIfNull(list);
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            ArgumentNullException.ThrowIfNull(merged);
            if (ReferenceEquals(a, b))
                throw new ArgumentException("Вихідні астероїди мають бути різними об'єктами.", nameof(b));
            if (!list.Contains(a) || !list.Contains(b))
                throw new ArgumentException("Обидва вихідні астероїди мають бути у списку.");
            if (list.Contains(merged))
                throw new ArgumentException("Новий астероїд уже є у списку.", nameof(merged));

            int position = Math.Min(list.IndexOf(a), list.IndexOf(b));
            list.Remove(a);
            list.Remove(b);
            list.Insert(position, merged);
        }

        public static double CalculateTotalEnergy(List<PizzaAsteroid> list)
        {
            double totalVolume = PizzaAsteroid.TotalVolume(list);
            return Math.Round(totalVolume * PizzaAsteroid.ImpactEnergyFactor, 2);
        }

        public static List<string> GetToStringLines(List<PizzaAsteroid> list)
        {
            ArgumentNullException.ThrowIfNull(list);
            return list.ConvertAll(a => a.ToString());
        }

        public static bool IsRoundTripCompatible(PizzaAsteroid asteroid)
        {
            ArgumentNullException.ThrowIfNull(asteroid);
            string original = asteroid.ToString();
            PizzaAsteroid copy = PizzaAsteroid.Parse(original);
            return copy.ToString() == original;
        }

        public static string FormatTable(List<PizzaAsteroid> list, string title)
        {
            if (list is null || list.Count == 0)
                return "Жодного об'єкта не знайдено.";

            string separator = new('-', 135);
            List<string> lines =
            [
                title,
                separator,
                $"| {"#",-3} | {"Назва",-18} | {"Бортик",-14} | {"Соус",-16} | {"Діаметр",-9} | {"Об'єм (км³)",-12} | {"T (°C)",-7} | {"Сир+",-6} | {"Відкрито",-10} |",
                separator
            ];

            for (int i = 0; i < list.Count; i++)
            {
                PizzaAsteroid a = list[i];
                lines.Add($"| {i + 1,-3} | {a.Name,-18} | {a.Crust,-14} | {a.SauceType,-16} | {a.DiameterKm,-9:F2} | {a.EstimatedVolumeKm3,-12:F2} | {a.TemperatureCelsius,-7} | {(a.HasExtraCheese ? "Так" : "Ні"),-6} | {a.DiscoveryDate,-10:dd.MM.yyyy} |");
            }
            lines.Add(separator);

            return string.Join(Environment.NewLine, lines);
        }

        public static bool TryParseInt(string? input, out int value)
        {
            return int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
        }

        public static bool TryParseIntInRange(string? input, int min, int max, out int value)
        {
            if (min > max)
                throw new ArgumentException("Нижня межа не може перевищувати верхню.", nameof(min));
            if (TryParseInt(input, out value) && value >= min && value <= max)
                return true;
            value = 0;
            return false;
        }

        public static bool TryParseDouble(string? input, out double value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(input))
                return false;
            string normalized = input.Trim().Replace(',', '.');
            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed)
                && double.IsFinite(parsed))
            {
                value = parsed;
                return true;
            }
            return false;
        }

        public static bool TryParseBool(string? input, out bool value)
        {
            value = false;
            switch (input?.Trim().ToLowerInvariant())
            {
                case "1":
                case "так":
                case "true":
                    value = true;
                    return true;
                case "0":
                case "ні":
                case "false":
                    return true;
                default:
                    return false;
            }
        }

        public static bool TryParseDate(string? input, out DateTime value)
        {
            return DateTime.TryParseExact(input?.Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out value);
        }

        public static bool TryParseCrust(string? input, out CrustType crust)
        {
            crust = default;
            if (!TryParseIntInRange(input, 1, 4, out int number))
                return false;
            crust = (CrustType)number;
            return true;
        }

        private static string ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Назва для пошуку не може бути порожньою.", nameof(name));
            return name.Trim();
        }

        [ExcludeFromCodeCoverage]
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
                Console.WriteLine("7 – Сортувати список");
                Console.WriteLine("0 – Вийти з програми");
                Console.Write(">");

                string ans = ReadLine();
                Console.WriteLine();

                switch (ans)
                {
                    case "1": AddAsteroidMenu(); break;
                    case "2":
                        PrintTable(Asteroids, "Список усіх піцца-астероїдів");
                        Console.WriteLine($"Усього коректно створено об'єктів PizzaAsteroid (PizzaAsteroid.CreatedCount): {PizzaAsteroid.CreatedCount}");
                        break;
                    case "3": SearchAsteroids(); break;
                    case "4": DemonstrateBehavior(); break;
                    case "5": DeleteAsteroidMenu(); break;
                    case "6": DemonstrateStaticMethods(); break;
                    case "7": SortMenu(); break;
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

        [ExcludeFromCodeCoverage]
        private static void AddAsteroidMenu()
        {
            if (IsFull(Asteroids, _maxCapacity))
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

            switch (ReadLine())
            {
                case "1": AddManually(); break;
                case "2":
                    PizzaAsteroid random = CreateRandom(new Random(), out int ctorIndex);
                    TryAdd(Asteroids, random, _maxCapacity);
                    Console.WriteLine($"[Конструктор]: Спрацював {GetConstructorDescription(ctorIndex)}");
                    Console.WriteLine($"Успіх! Автоматично створено: '{random.Name}'.");
                    break;
                case "3":
                    PizzaAsteroid def = CreateDefault();
                    TryAdd(Asteroids, def, _maxCapacity);
                    Console.WriteLine("[Конструктор]: Спрацював конструктор без параметрів PizzaAsteroid() разом з ініціалізатором об'єкта.");
                    Console.WriteLine($"Успіх! Створено об'єкт '{def.Name}'.");
                    break;
                case "4": AddFromStringMenu(); break;
                default:
                    Console.WriteLine("Помилка! Некоректний режим додавання.");
                    break;
            }
        }

        [ExcludeFromCodeCoverage]
        private static void AddManually()
        {
            while (true)
            {
                try
                {
                    Console.Write("Введіть назву астероїда: ");
                    string name = ReadLine();

                    Console.WriteLine("Оберіть бортик: 1 - Thin, 2 - CheeseStuffed, 3 - DeepDish, 4 - Classic");
                    CrustType crust = (CrustType)ReadIntInRange("Номер (1-4): ", 1, 4);

                    double diameter = ReadDouble("Введіть діаметр у км: ");
                    int tempC = ReadInt("Введіть температуру в °C: ");
                    bool extraCheese = ReadBool("Чи є подвійний сир? (1/так, 0/ні): ");
                    DateTime date = ReadDate($"Введіть дату відкриття ({DateFormat}): ");

                    Console.Write($"Введіть назву соусу (Enter для '{DefaultSauce}'): ");
                    string sauceInput = ReadLine();
                    string sauce = string.IsNullOrWhiteSpace(sauceInput) ? DefaultSauce : sauceInput;

                    PizzaAsteroid asteroid = new(name, crust, diameter, tempC, extraCheese, date, sauce);
                    TryAdd(Asteroids, asteroid, _maxCapacity);

                    Console.WriteLine($"[Конструктор]: Спрацював {GetConstructorDescription(4)}");
                    Console.WriteLine($"Успіх! Астероїд '{asteroid.Name}' успішно створено!");
                    return;
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine($"[Помилка валідації]: {ex.Message}\nСпробуйте ввести дані заново.\n");
                }
            }
        }

        [ExcludeFromCodeCoverage]
        private static void AddFromStringMenu()
        {
            PrintStringFormatHint();

            while (true)
            {
                Console.Write("Введіть рядок (порожній рядок — скасувати): ");
                string input = ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Додавання скасовано.");
                    return;
                }

                if (TryAddFromString(Asteroids, input, _maxCapacity, out PizzaAsteroid? added) && added != null)
                {
                    Console.WriteLine("[TryParse]: Рядок успішно перетворено на об'єкт.");
                    Console.WriteLine($"Успіх! Астероїд '{added.Name}' додано. ToString(): {added}");
                    return;
                }

                Console.WriteLine("[TryParse]: Не вдалося перетворити рядок на об'єкт (TryParse повернув false).");
                Console.WriteLine("Перевірте формат і значення полів. Щоб побачити точну причину, скористайтеся пунктом 6 → Parse.\n");
            }
        }

        [ExcludeFromCodeCoverage]
        private static void SearchAsteroids()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Список порожній, пошук неможливий.");
                return;
            }

            Console.WriteLine("Оберіть критерій пошуку:");
            Console.WriteLine("1 – За типом бортика (FindAll)");
            Console.WriteLine("2 – За наявністю подвійного сиру (FindAll)");
            Console.WriteLine("3 – За назвою (Find, FindIndex)");
            Console.WriteLine("4 – За діапазоном діаметра (FindAll)");
            Console.Write(">");

            try
            {
                switch (ReadLine())
                {
                    case "1":
                        Console.WriteLine("Оберіть шуканий бортик: 1 - Thin, 2 - CheeseStuffed, 3 - DeepDish, 4 - Classic");
                        CrustType crust = (CrustType)ReadIntInRange("Номер: ", 1, 4);
                        PrintTable(FindByCrust(Asteroids, crust), "Результати пошуку");
                        break;
                    case "2":
                        bool cheese = ReadBool("Шукати з додатковим сиром? (1/так, 0/ні): ");
                        PrintTable(FindByCheese(Asteroids, cheese), "Результати пошуку");
                        break;
                    case "3":
                        Console.Write("Введіть назву: ");
                        string name = ReadLine();
                        PizzaAsteroid? found = FindByName(Asteroids, name);
                        if (found is null)
                        {
                            Console.WriteLine($"Об'єкт з назвою '{name}' не знайдено.");
                        }
                        else
                        {
                            Console.WriteLine($"Знайдено на позиції №{FindIndexByName(Asteroids, name) + 1}:");
                            PrintTable([found], "Результат пошуку");
                        }
                        break;
                    case "4":
                        double min = ReadDouble("Мінімальний діаметр, км: ");
                        double max = ReadDouble("Максимальний діаметр, км: ");
                        PrintTable(FindByDiameterRange(Asteroids, min, max), "Результати пошуку");
                        break;
                    default:
                        Console.WriteLine("Помилка вибору критерію.");
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[Помилка пошуку]: {ex.Message}");
            }
        }

        [ExcludeFromCodeCoverage]
        private static void DeleteAsteroidMenu()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Список порожній, видалення неможливе.");
                return;
            }

            Console.WriteLine("Спосіб видалення:");
            Console.WriteLine("1 – За порядковим номером у таблиці (RemoveAt)");
            Console.WriteLine("2 – За назвою, усі збіги (RemoveAll)");
            Console.WriteLine("3 – Очистити весь список (Clear)");
            Console.Write(">");

            try
            {
                switch (ReadLine())
                {
                    case "1":
                        PrintTable(Asteroids, "Поточний перелік астероїдів");
                        int number = ReadIntInRange($"Введіть номер для видалення (1..{Asteroids.Count}): ", 1, Asteroids.Count);
                        PizzaAsteroid removed = RemoveAt(Asteroids, number);
                        Console.WriteLine($"Успіх! Астероїд '{removed.Name}' успішно видалено.");
                        break;
                    case "2":
                        Console.Write("Введіть назву для пошуку та видалення: ");
                        string name = ReadLine();
                        int count = RemoveByName(Asteroids, name);
                        Console.WriteLine(count > 0 ? $"Успіх! Видалено об'єктів: {count}." : $"Об'єктів з назвою '{name}' не знайдено.");
                        break;
                    case "3":
                        if (ReadBool($"Видалити всі {Asteroids.Count} об'єкти? (1/так, 0/ні): "))
                            Console.WriteLine($"Успіх! Видалено об'єктів: {Clear(Asteroids)}.");
                        break;
                    default:
                        Console.WriteLine("Помилка вибору.");
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[Помилка видалення]: {ex.Message}");
            }
        }

        [ExcludeFromCodeCoverage]
        private static void SortMenu()
        {
            if (Asteroids.Count < 2)
            {
                Console.WriteLine("Для сортування потрібно щонайменше 2 об'єкти.");
                return;
            }

            Console.WriteLine("1 – Сортувати за назвою");
            Console.WriteLine("2 – Сортувати за діаметром");
            Console.Write(">");

            switch (ReadLine())
            {
                case "1": SortByName(Asteroids); break;
                case "2": SortByDiameter(Asteroids); break;
                default:
                    Console.WriteLine("Помилка вибору.");
                    return;
            }
            PrintTable(Asteroids, "Відсортований список");
        }

        [ExcludeFromCodeCoverage]
        private static void DemonstrateBehavior()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Немає об'єктів для виклику методів.");
                return;
            }

            PrintTable(Asteroids, "Оберіть астероїд для демонстрації");
            int number = ReadIntInRange($"Введіть порядковий номер об'єкта (1..{Asteroids.Count}): ", 1, Asteroids.Count);
            PizzaAsteroid target = GetByNumber(Asteroids, number);

            Console.WriteLine($"\nОберіть дію для астероїда '{target.Name}':");
            Console.WriteLine("1 – Нагріти астероїд (Демонстрація ПЕРЕВАНТАЖЕНИХ версій HeatUp)");
            Console.WriteLine("2 – Розрізати астероїд на шматки (Slice)");
            Console.WriteLine("3 – Створити зіткнення з планетою (CollideWithTarget)");
            Console.Write(">");

            try
            {
                switch (ReadLine())
                {
                    case "1": DemoHeatUp(target); break;
                    case "2":
                        int slices = ReadInt("На скільки частин розрізати (>= 2)?: ");
                        target.Slice(slices);
                        Console.WriteLine($"Успіх! Астероїд розрізано. Новий діаметр: {target.DiameterKm} км, об'єм: {target.EstimatedVolumeKm3} км³.");
                        break;
                    case "3":
                        Console.Write("Введіть назву планети або супутника: ");
                        string planet = ReadLine();
                        Console.WriteLine(target.CollideWithTarget(string.IsNullOrWhiteSpace(planet) ? "Марс" : planet));
                        break;
                    default:
                        Console.WriteLine("Помилка! Невідомий метод.");
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[Помилка]: {ex.Message}");
            }
        }

        [ExcludeFromCodeCoverage]
        private static void DemoHeatUp(PizzaAsteroid target)
        {
            Console.WriteLine("\nОберіть сигнатуру виклику HeatUp:");
            Console.WriteLine("1) HeatUp(int degrees)");
            Console.WriteLine("2) HeatUp(int degrees, string heatSource)");
            Console.WriteLine("3) HeatUp(double factor)");
            Console.Write(">");
            string choice = ReadLine();
            int tempBefore = target.TemperatureCelsius;

            switch (choice)
            {
                case "1":
                    target.HeatUp(ReadInt("Введіть градуси нагріву (int): "));
                    Console.WriteLine("Викликано перевантаження: HeatUp(int degrees)");
                    break;
                case "2":
                    int deg = ReadInt("Введіть градуси нагріву (int): ");
                    Console.Write("Введіть назву джерела (наприклад, 'Сонячний спалах'): ");
                    Console.WriteLine(target.HeatUp(deg, ReadLine()));
                    Console.WriteLine("Викликано перевантаження: HeatUp(int degrees, string heatSource)");
                    break;
                case "3":
                    target.HeatUp(ReadDouble("Введіть коефіцієнт нагріву (1.0 < k <= 10.0, наприклад 1.5): "));
                    Console.WriteLine("Викликано перевантаження: HeatUp(double factor)");
                    break;
                default:
                    Console.WriteLine("Помилка! Невідома опція.");
                    return;
            }
            Console.WriteLine($"Успіх! T: {tempBefore}°C -> {target.TemperatureCelsius}°C.");
        }

        [ExcludeFromCodeCoverage]
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
            string choice = ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1": DemoImpactFactor(); break;
                case "2": DemoParse(); break;
                case "3": DemoTryParse(); break;
                case "4": DemoToString(); break;
                case "5": DemoMerge(); break;
                case "6": DemoTotalVolume(); break;
                default: Console.WriteLine("Помилка! Невідома опція."); break;
            }
        }

        [ExcludeFromCodeCoverage]
        private static void DemoImpactFactor()
        {
            Console.WriteLine($"Поточне значення PizzaAsteroid.ImpactEnergyFactor = {PizzaAsteroid.ImpactEnergyFactor} Мт ТНТ / км³ (спільне для всіх астероїдів).");
            if (!ReadBool("Змінити значення? (1/так, 0/ні): "))
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

        [ExcludeFromCodeCoverage]
        private static void DemoParse()
        {
            PrintStringFormatHint();
            Console.Write("Введіть рядок для Parse: ");
            string input = ReadLine();
            int countBefore = PizzaAsteroid.CreatedCount;

            try
            {
                PizzaAsteroid obj = PizzaAsteroid.Parse(input);
                Console.WriteLine("Parse успішний! Отримано об'єкт:");
                PrintTable([obj], "Результат Parse");
                OfferToAdd(obj);
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"[FormatException]: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[{ex.GetType().Name}]: {ex.Message}");
            }

            Console.WriteLine($"CreatedCount: {countBefore} -> {PizzaAsteroid.CreatedCount}");
        }

        [ExcludeFromCodeCoverage]
        private static void DemoTryParse()
        {
            PrintStringFormatHint();
            Console.Write("Введіть рядок для TryParse: ");
            string input = ReadLine();

            bool ok = PizzaAsteroid.TryParse(input, out PizzaAsteroid? obj);
            Console.WriteLine($"TryParse повернув: {ok}");

            if (ok && obj != null)
            {
                PrintTable([obj], "Результат TryParse");
                OfferToAdd(obj);
            }
            else
            {
                Console.WriteLine("obj = null. Виняток не виник — TryParse перехопив його всередині.");
            }
        }

        [ExcludeFromCodeCoverage]
        private static void OfferToAdd(PizzaAsteroid obj)
        {
            if (IsFull(Asteroids, _maxCapacity))
                return;
            if (ReadBool("Додати цей об'єкт до списку? (1/так, 0/ні): ") && TryAdd(Asteroids, obj, _maxCapacity))
                Console.WriteLine($"Об'єкт '{obj.Name}' додано до списку.");
        }

        [ExcludeFromCodeCoverage]
        private static void DemoToString()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            Console.WriteLine("Результат ToString() для кожного об'єкта:");
            List<string> lines = GetToStringLines(Asteroids);
            for (int i = 0; i < lines.Count; i++)
                Console.WriteLine($"{i + 1}. {lines[i]}");

            bool roundTrip = IsRoundTripCompatible(Asteroids[0]);
            Console.WriteLine($"\nПеревірка сумісності ToString ↔ Parse для об'єкта №1: {(roundTrip ? "рядки збігаються ✔" : "НЕ збігаються ✘")}");
            Console.WriteLine("(Примітка: під час перевірки створено тимчасову копію, тому CreatedCount збільшився на 1.)");
        }

        [ExcludeFromCodeCoverage]
        private static void DemoMerge()
        {
            if (Asteroids.Count < 2)
            {
                Console.WriteLine("Для злиття потрібно щонайменше 2 об'єкти у списку.");
                return;
            }

            PrintTable(Asteroids, "Оберіть два астероїди для злиття");
            int n1 = ReadIntInRange($"Номер першого (1..{Asteroids.Count}): ", 1, Asteroids.Count);
            int n2 = ReadIntInRange($"Номер другого (1..{Asteroids.Count}): ", 1, Asteroids.Count);
            Console.Write("Назва нового астероїда: ");
            string newName = ReadLine();

            try
            {
                PizzaAsteroid a = GetByNumber(Asteroids, n1);
                PizzaAsteroid b = GetByNumber(Asteroids, n2);
                PizzaAsteroid merged = Merge(Asteroids, n1, n2, newName);

                Console.WriteLine($"Об'єм: {a.EstimatedVolumeKm3} + {b.EstimatedVolumeKm3} ≈ {merged.EstimatedVolumeKm3} км³");
                PrintTable([merged], "Результат Merge");

                if (ReadBool("Замінити два вихідні астероїди новим у списку? (1/так, 0/ні): "))
                {
                    ReplaceWithMerged(Asteroids, a, b, merged);
                    Console.WriteLine($"Успіх! '{a.Name}' і '{b.Name}' злито в '{merged.Name}'.");
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"[Помилка Merge]: {ex.Message}");
            }
        }

        [ExcludeFromCodeCoverage]
        private static void DemoTotalVolume()
        {
            if (Asteroids.Count == 0)
            {
                Console.WriteLine("Список порожній, сумарний об'єм = 0 км³.");
                return;
            }

            Console.WriteLine($"PizzaAsteroid.TotalVolume(список з {Asteroids.Count} об'єктів) = {PizzaAsteroid.TotalVolume(Asteroids)} км³");
            Console.WriteLine($"Сумарна енергія, якщо всі впадуть на одну планету: {CalculateTotalEnergy(Asteroids)} Мт ТНТ");
        }

        [ExcludeFromCodeCoverage]
        private static void PrintStringFormatHint()
        {
            Console.WriteLine("Формат рядка: Назва;Бортик;Діаметр;Температура;Сир;Дата[;Соус]");
            Console.WriteLine("  Бортик: Thin, CheeseStuffed, DeepDish, Classic або 1–4; Сир: так/ні; Дата: dd.MM.yyyy");
            Console.WriteLine("  Приклад: Пепероні-X;Thin;12.5;-40;так;12.04.2021;Барбекю");
        }

        [ExcludeFromCodeCoverage]
        private static void PrintTable(List<PizzaAsteroid> list, string title)
        {
            Console.WriteLine();
            Console.WriteLine(FormatTable(list, title));
        }

        [ExcludeFromCodeCoverage]
        private static string ReadLine() => (Console.ReadLine() ?? string.Empty).Trim();

        [ExcludeFromCodeCoverage]
        private static int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (TryParseInt(ReadLine(), out int val)) return val;
                Console.WriteLine("Помилка! Введіть коректне ціле число.");
            }
        }

        [ExcludeFromCodeCoverage]
        private static int ReadPositiveInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (TryParseIntInRange(ReadLine(), 1, int.MaxValue, out int val)) return val;
                Console.WriteLine("Помилка! Введіть ціле число, більше за нуль.");
            }
        }

        [ExcludeFromCodeCoverage]
        private static int ReadIntInRange(string prompt, int min, int max)
        {
            while (true)
            {
                Console.Write(prompt);
                if (TryParseIntInRange(ReadLine(), min, max, out int val)) return val;
                Console.WriteLine($"Помилка! Введіть ціле число в межах від {min} до {max}.");
            }
        }

        [ExcludeFromCodeCoverage]
        private static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (TryParseDouble(ReadLine(), out double val)) return val;
                Console.WriteLine("Помилка! Введіть коректне дійсне число.");
            }
        }

        [ExcludeFromCodeCoverage]
        private static DateTime ReadDate(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (TryParseDate(ReadLine(), out DateTime d)) return d;
                Console.WriteLine($"Помилка! Формат дати має бути {DateFormat} (наприклад, 12.04.2021).");
            }
        }

        [ExcludeFromCodeCoverage]
        private static bool ReadBool(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                if (TryParseBool(ReadLine(), out bool val)) return val;
                Console.WriteLine("Помилка! Введіть 1/так або 0/ні.");
            }
        }
    }
}