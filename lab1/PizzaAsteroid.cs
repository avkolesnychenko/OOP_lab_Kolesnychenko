using System.Text.RegularExpressions;

namespace PizzaAsteroidApp
{
    public class PizzaAsteroid
    {
        private string _name = "Астероїд";
        private CrustType _crust;
        private double _diameterKm;
        private int _temperatureCelsius;
        private DateTime _discoveryDate;

        public string SauceType { get; set; } = "Томатний Класик";

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

                if (!Regex.IsMatch(trimmed, @"^[a-zA-Zа-яА-ЯіІїЇєЄ0-9\s\-]+$"))
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
                if (value < 0.1 || value > 1000.0)
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

        public bool HasExtraCheese { get; set; }

        public DateTime DiscoveryDate
        {
            get => _discoveryDate;
            private set
            {
                DateTime minDate = new DateTime(1990, 1, 1);
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
        {
            Crust = CrustType.Classic;
            DiameterKm = 10.0;
            TemperatureCelsius = -50;
            DiscoveryDate = DateTime.Now.Date;
        }

        public PizzaAsteroid(string name, CrustType crust, double diameterKm, int temperatureCelsius, bool hasExtraCheese, DateTime discoveryDate, string sauce = "Томатний Класик")
        {
            Name = name;
            Crust = crust;
            DiameterKm = diameterKm;
            TemperatureCelsius = temperatureCelsius;
            HasExtraCheese = hasExtraCheese;
            SauceType = sauce;
            SetDiscoveryDate(discoveryDate);
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
            double energy = EstimatedVolumeKm3 * 1.75;
            return energy < 0.01 ? Math.Round(energy, 4) : Math.Round(energy, 2);
        }
    }
}