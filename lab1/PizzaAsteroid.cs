using System.Globalization;
using System.Text.RegularExpressions;

namespace PizzaAsteroidApp
{
    public class PizzaAsteroid
    {
        private static int _createdCount;
        private static double _impactEnergyFactor;
        public const char Separator = ';';
        private const string DateFormat = "dd.MM.yyyy";
        private const string DefaultSauce = "Томатний Класик";

        private string _name = "Астероїд";
        private CrustType _crust;
        private double _diameterKm;
        private int _temperatureCelsius;
        private DateTime _discoveryDate;
        private string _sauceType = DefaultSauce;

        static PizzaAsteroid()
        {
            _createdCount = 0;
            _impactEnergyFactor = 1.75;
        }

        public static int CreatedCount
        {
            get => _createdCount;
        }

        public static double ImpactEnergyFactor
        {
            get => _impactEnergyFactor;
            set
            {
                if (double.IsNaN(value) || value < 0.1 || value > 100.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Коефіцієнт енергії удару має бути в межах від 0.1 до 100.0 Мт/км³.");
                _impactEnergyFactor = Math.Round(value, 3);
            }
        }

        public bool HasExtraCheese { get; set; }

        public string SauceType
        {
            get => _sauceType;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва соусу не може бути порожньою.", nameof(value));
                if (value.Contains(Separator))
                    throw new ArgumentException($"Назва соусу не може містити символ '{Separator}'.", nameof(value));
                _sauceType = value.Trim();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Назва астероїда не може бути порожньою.", nameof(value));

                string trimmed = value.Trim();
                if (trimmed.Length < 3 || trimmed.Length > 20)
                    throw new ArgumentException("Довжина назви має бути від 3 до 20 символів.", nameof(value));

                if (!Regex.IsMatch(trimmed, @"^[a-zA-Zа-яА-ЯіІїЇєЄґҐ0-9\s\-]+$"))
                    throw new ArgumentException("Назва містить неприпустимі спецсимволи.", nameof(value));

                _name = trimmed;
            }
        }

        public CrustType Crust
        {
            get => _crust;
            set
            {
                if (!Enum.IsDefined(typeof(CrustType), value))
                    throw new ArgumentException("Невідомий тип бортика.", nameof(value));
                _crust = value;
            }
        }

        public double DiameterKm
        {
            get => _diameterKm;
            set
            {
                if (double.IsNaN(value) || value < 0.1 || value > 1000.0)
                    throw new ArgumentOutOfRangeException(nameof(value), "Діаметр повинен бути в межах від 0.1 до 1000.0 км.");
                _diameterKm = Math.Round(value, 2);
            }
        }

        public int TemperatureCelsius
        {
            get => _temperatureCelsius;
            set
            {
                if (value < -273 || value > 500)
                    throw new ArgumentOutOfRangeException(nameof(value), "Температура повинна бути від -273°C до 500°C.");
                _temperatureCelsius = value;
            }
        }

        public DateTime DiscoveryDate
        {
            get => _discoveryDate;
            private set
            {
                DateTime minDate = new(1990, 1, 1);
                if (value < minDate || value > DateTime.Now.Date.AddDays(1))
                    throw new ArgumentOutOfRangeException(nameof(value), "Дата відкриття має бути в межах від 01.01.1990 до сьогодення.");
                _discoveryDate = value.Date;
            }
        }

        public double EstimatedVolumeKm3
        {
            get
            {
                double radius = _diameterKm / 2.0;
                double volume = (4.0 / 3.0) * Math.PI * Math.Pow(radius, 3);
                return volume < 0.01 ? Math.Round(volume, 5) : Math.Round(volume, 2);
            }
        }

        public PizzaAsteroid()
            : this("Астероїд", CrustType.Classic, 10.0, -50, false, DateTime.Now.Date)
        {
        }

        public PizzaAsteroid(string name, CrustType crust, double diameterKm)
            : this(name, crust, diameterKm, -50, false, DateTime.Now.Date)
        {
        }

        public PizzaAsteroid(string name, CrustType crust, double diameterKm, int temperatureCelsius, bool hasExtraCheese)
            : this(name, crust, diameterKm, temperatureCelsius, hasExtraCheese, DateTime.Now.Date)
        {
        }

        public PizzaAsteroid(string name, CrustType crust, double diameterKm, int temperatureCelsius, bool hasExtraCheese, DateTime discoveryDate, string sauce = DefaultSauce)
        {
            Name = name;
            Crust = crust;
            DiameterKm = diameterKm;
            TemperatureCelsius = temperatureCelsius;
            HasExtraCheese = hasExtraCheese;
            SauceType = sauce;
            SetDiscoveryDate(discoveryDate);

            _createdCount++;
        }

        public void SetDiscoveryDate(DateTime date)
        {
            DiscoveryDate = date;
        }

        public void HeatUp(int degrees)
        {
            if (degrees <= 0)
                throw new ArgumentOutOfRangeException(nameof(degrees), "Значення нагріву має бути додатним.");

            int targetTemp = TemperatureCelsius + degrees;
            TemperatureCelsius = ClampTemperature(targetTemp);
        }

        public string HeatUp(int degrees, string heatSource)
        {
            if (string.IsNullOrWhiteSpace(heatSource))
                throw new ArgumentException("Джерело тепла не може бути порожнім.", nameof(heatSource));

            HeatUp(degrees);
            return $"Астероїд '{Name}' нагріто джерелом '{heatSource.Trim()}' на {degrees}°C.";
        }

        public void HeatUp(double factor)
        {
            if (double.IsNaN(factor) || factor <= 1.0 || factor > 10.0)
                throw new ArgumentOutOfRangeException(nameof(factor), "Коефіцієнт нагріву повинен бути більшим за 1.0 і не більшим за 10.0.");

            int currentBase = TemperatureCelsius == 0 ? 20 : Math.Abs(TemperatureCelsius);
            int boost = Math.Max((int)Math.Round(currentBase * (factor - 1.0)), 5);
            HeatUp(boost);
        }

        public void Slice(int parts)
        {
            if (parts < 2)
                throw new ArgumentException("Кількість шматків має бути не менше 2.", nameof(parts));

            ApplySliceDivision(parts);
        }

        public string CollideWithTarget(string targetPlanet)
        {
            double impactEnergyMegatons = CalculateImpactEnergy();
            string energyStr = impactEnergyMegatons < 0.01
                ? impactEnergyMegatons.ToString("0.0000")
                : impactEnergyMegatons.ToString("0.00");

            return $"Астероїд '{Name}' зіткнувся з об'єктом '{targetPlanet}'. Енергія удару: {energyStr} Мт ТНТ. Соус розлетівся по орбіті!";
        }

        public override string ToString()
        {
            return string.Join(Separator,
                Name,
                Crust.ToString(),
                DiameterKm.ToString("0.##", CultureInfo.InvariantCulture),
                TemperatureCelsius.ToString(CultureInfo.InvariantCulture),
                HasExtraCheese ? "так" : "ні",
                DiscoveryDate.ToString(DateFormat, CultureInfo.InvariantCulture),
                SauceType);
        }

        public static PizzaAsteroid Parse(string s)
        {
            if (s == null)
                throw new ArgumentNullException(nameof(s), "Рядок для перетворення не може бути null.");
            if (string.IsNullOrWhiteSpace(s))
                throw new FormatException("Рядок для перетворення порожній.");

            string[] parts = s.Split(Separator);
            if (parts.Length < 6 || parts.Length > 7)
                throw new FormatException(
                    $"Очікується 6 або 7 полів, розділених '{Separator}' (Назва;Бортик;Діаметр;Температура;Сир;Дата[;Соус]), отримано: {parts.Length}.");

            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].Trim();

            string name = parts[0];
            CrustType crust = ParseCrust(parts[1]);
            double diameter = ParseDiameter(parts[2]);
            int temperature = ParseTemperature(parts[3]);
            bool cheese = ParseCheese(parts[4]);
            DateTime date = ParseDate(parts[5]);
            string sauce = parts.Length == 7 && parts[6].Length > 0 ? parts[6] : DefaultSauce;

            return new PizzaAsteroid(name, crust, diameter, temperature, cheese, date, sauce);
        }

        public static bool TryParse(string s, out PizzaAsteroid? obj)
        {
            try
            {
                obj = Parse(s);
                return true;
            }
            catch (Exception ex) when (ex is FormatException || ex is ArgumentException)
            {
                obj = null;
                return false;
            }
        }

        public static PizzaAsteroid Merge(PizzaAsteroid a, PizzaAsteroid b, string newName)
        {
            ArgumentNullException.ThrowIfNull(a);
            ArgumentNullException.ThrowIfNull(b);
            if (ReferenceEquals(a, b))
                throw new ArgumentException("Неможливо злити астероїд сам із собою.", nameof(b));

            double totalVolume = VolumeOf(a.DiameterKm) + VolumeOf(b.DiameterKm);
            double newDiameter = 2.0 * Math.Cbrt(3.0 * totalVolume / (4.0 * Math.PI));

            PizzaAsteroid bigger = a.DiameterKm >= b.DiameterKm ? a : b;
            int avgTemp = (int)Math.Round((a.TemperatureCelsius + b.TemperatureCelsius) / 2.0);
            DateTime earliest = a.DiscoveryDate <= b.DiscoveryDate ? a.DiscoveryDate : b.DiscoveryDate;

            return new PizzaAsteroid(newName, bigger.Crust, newDiameter, avgTemp,
                a.HasExtraCheese || b.HasExtraCheese, earliest, bigger.SauceType);
        }

        public static double TotalVolume(IEnumerable<PizzaAsteroid> asteroids)
        {
            ArgumentNullException.ThrowIfNull(asteroids);
            double sum = 0;
            foreach (var a in asteroids)
                sum += a.EstimatedVolumeKm3;
            return Math.Round(sum, 2);
        }

        private int ClampTemperature(int temp)
        {
            return temp > 500 ? 500 : temp;
        }

        private void ApplySliceDivision(int parts)
        {
            double newDiameter = _diameterKm / Math.Sqrt(parts);
            DiameterKm = Math.Round(newDiameter, 2);
        }

        private double CalculateImpactEnergy()
        {
            double energy = EstimatedVolumeKm3 * ImpactEnergyFactor;
            return energy < 0.01 ? Math.Round(energy, 4) : Math.Round(energy, 2);
        }

        private static double VolumeOf(double diameter)
        {
            double r = diameter / 2.0;
            return 4.0 / 3.0 * Math.PI * r * r * r;
        }

        private static CrustType ParseCrust(string raw)
        {
            if (raw.Length == 0)
                throw new FormatException("Поле 'Бортик' порожнє.");

            if (int.TryParse(raw, out int num))
            {
                if (!Enum.IsDefined(typeof(CrustType), num))
                    throw new FormatException($"Поле 'Бортик': номер '{raw}' поза межами 1–4.");
                return (CrustType)num;
            }

            if (Enum.TryParse(raw, true, out CrustType crust) && Enum.IsDefined(typeof(CrustType), crust))
                return crust;

            throw new FormatException(
                $"Поле 'Бортик': невідоме значення '{raw}'. Допустимі: Thin, CheeseStuffed, DeepDish, Classic або 1–4.");
        }

        private static double ParseDiameter(string raw)
        {
            string normalized = raw.Replace(',', '.');
            if (!double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                throw new FormatException($"Поле 'Діаметр': '{raw}' не є дійсним числом.");
            return d;
        }

        private static int ParseTemperature(string raw)
        {
            if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int t))
                throw new FormatException($"Поле 'Температура': '{raw}' не є цілим числом.");
            return t;
        }

        private static bool ParseCheese(string raw)
        {
            switch (raw.ToLowerInvariant())
            {
                case "так": case "1": case "true": return true;
                case "ні": case "0": case "false": return false;
                default:
                    throw new FormatException($"Поле 'Сир': '{raw}' — очікується так/ні, 1/0 або true/false.");
            }
        }

        private static DateTime ParseDate(string raw)
        {
            if (!DateTime.TryParseExact(raw, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                throw new FormatException($"Поле 'Дата': '{raw}' не відповідає формату {DateFormat}.");
            return d;
        }
    }
}