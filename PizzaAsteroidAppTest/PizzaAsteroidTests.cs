using System.Globalization;

namespace PizzaAsteroidApp.Tests
{
    [TestClass]
    public class PizzaAsteroidTests
    {
        private const double Delta = 1e-9;
        private const string DefaultSauce = "Томатний Класик";

        private PizzaAsteroid _asteroid = null!;
        private double _savedImpactFactor;
        private CultureInfo _savedCulture = null!;

        [TestInitialize]
        public void TestInitialize()
        {
            _savedImpactFactor = PizzaAsteroid.ImpactEnergyFactor;
            _savedCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            _asteroid = new PizzaAsteroid("Пепероні-X", CrustType.Thin, 12.5, -40, true,
                new DateTime(2021, 4, 12), "Барбекю");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            PizzaAsteroid.ImpactEnergyFactor = _savedImpactFactor;
            CultureInfo.CurrentCulture = _savedCulture;
            _asteroid = null!;
        }

        #region Конструктори

        [TestMethod]
        public void Constructor_Default_SetsDefaultValues()
        {
            // Arrange
            DateTime today = DateTime.Today;

            // Act
            var asteroid = new PizzaAsteroid();

            // Assert
            Assert.AreEqual("Астероїд", asteroid.Name);
            Assert.AreEqual(CrustType.Classic, asteroid.Crust);
            Assert.AreEqual(10.0, asteroid.DiameterKm, Delta);
            Assert.AreEqual(-50, asteroid.TemperatureCelsius);
            Assert.IsFalse(asteroid.HasExtraCheese);
            Assert.AreEqual(today, asteroid.DiscoveryDate);
            Assert.AreEqual(DefaultSauce, asteroid.SauceType);
        }

        [TestMethod]
        public void Constructor_ThreeParameters_SetsValuesAndDefaults()
        {
            // Arrange
            string name = "Церера";
            CrustType crust = CrustType.DeepDish;
            double diameter = 25.5;

            // Act
            var asteroid = new PizzaAsteroid(name, crust, diameter);

            // Assert
            Assert.AreEqual(name, asteroid.Name);
            Assert.AreEqual(crust, asteroid.Crust);
            Assert.AreEqual(diameter, asteroid.DiameterKm, Delta);
            Assert.AreEqual(-50, asteroid.TemperatureCelsius);
            Assert.IsFalse(asteroid.HasExtraCheese);
            Assert.AreEqual(DateTime.Today, asteroid.DiscoveryDate);
            Assert.AreEqual(DefaultSauce, asteroid.SauceType);
        }

        [TestMethod]
        public void Constructor_FiveParameters_SetsValuesAndDefaults()
        {
            // Arrange & Act
            var asteroid = new PizzaAsteroid("Веста", CrustType.CheeseStuffed, 40.0, 120, true);

            // Assert
            Assert.AreEqual("Веста", asteroid.Name);
            Assert.AreEqual(CrustType.CheeseStuffed, asteroid.Crust);
            Assert.AreEqual(40.0, asteroid.DiameterKm, Delta);
            Assert.AreEqual(120, asteroid.TemperatureCelsius);
            Assert.IsTrue(asteroid.HasExtraCheese);
            Assert.AreEqual(DateTime.Today, asteroid.DiscoveryDate);
            Assert.AreEqual(DefaultSauce, asteroid.SauceType);
        }

        [TestMethod]
        public void Constructor_AllParameters_SetsAllValues()
        {
            // Arrange
            var date = new DateTime(2015, 6, 30);

            // Act
            var asteroid = new PizzaAsteroid("Паллада", CrustType.Thin, 512.0, 300, true, date, "Песто");

            // Assert
            Assert.AreEqual("Паллада", asteroid.Name);
            Assert.AreEqual(CrustType.Thin, asteroid.Crust);
            Assert.AreEqual(512.0, asteroid.DiameterKm, Delta);
            Assert.AreEqual(300, asteroid.TemperatureCelsius);
            Assert.IsTrue(asteroid.HasExtraCheese);
            Assert.AreEqual(date, asteroid.DiscoveryDate);
            Assert.AreEqual("Песто", asteroid.SauceType);
        }

        [TestMethod]
        public void Constructor_SixParametersWithoutSauce_UsesDefaultSauce()
        {
            // Arrange & Act
            var asteroid = new PizzaAsteroid("Гігея", CrustType.Classic, 5.0, 0, false, new DateTime(2000, 1, 1));

            // Assert
            Assert.AreEqual(DefaultSauce, asteroid.SauceType);
        }

        [TestMethod]
        public void Constructor_ValidObject_IncrementsCreatedCountByOne()
        {
            // Arrange
            int countBefore = PizzaAsteroid.CreatedCount;

            // Act
            _ = new PizzaAsteroid("Юнона", CrustType.Thin, 3.0);

            // Assert
            Assert.AreEqual(countBefore + 1, PizzaAsteroid.CreatedCount);
        }

        [TestMethod]
        public void Constructor_InvalidDiameter_ThrowsAndDoesNotIncrementCreatedCount()
        {
            // Arrange
            int countBefore = PizzaAsteroid.CreatedCount;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => new PizzaAsteroid("Юнона", CrustType.Thin, 0.0));
            Assert.AreEqual(countBefore, PizzaAsteroid.CreatedCount);
        }

        [TestMethod]
        public void Constructor_InvalidName_ThrowsAndDoesNotIncrementCreatedCount()
        {
            // Arrange
            int countBefore = PizzaAsteroid.CreatedCount;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(
                () => new PizzaAsteroid("@@@", CrustType.Thin, 5.0));
            Assert.AreEqual(countBefore, PizzaAsteroid.CreatedCount);
        }

        [TestMethod]
        public void Constructor_TwoObjects_AreDifferentInstances()
        {
            // Arrange & Act
            var first = new PizzaAsteroid();
            var second = new PizzaAsteroid();

            // Assert
            Assert.AreNotSame(first, second);
            Assert.IsInstanceOfType(first, typeof(PizzaAsteroid));
        }

        #endregion

        #region Властивість Name

        [TestMethod]
        [DataRow("Абв")]
        [DataRow("Карбонара-99")]
        [DataRow("Гаваї Ультра")]
        [DataRow("Ґрунт-Їжак Є")]
        [DataRow("ABCDEFGHIJKLMNOPQRST")]
        public void Name_ValidValue_IsAssigned(string validName)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            _asteroid.Name = validName;

            // Assert
            Assert.AreEqual(validName, _asteroid.Name);
        }

        [TestMethod]
        public void Name_ValueWithSurroundingSpaces_IsTrimmed()
        {
            // Arrange
            string raw = "   Церера   ";

            // Act
            _asteroid.Name = raw;

            // Assert
            Assert.AreEqual("Церера", _asteroid.Name);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("\t")]
        public void Name_EmptyOrWhiteSpace_ThrowsArgumentException(string emptyName)
        {
            // Arrange
            string before = _asteroid.Name;

            // Act & Assert
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _asteroid.Name = emptyName);
            Assert.AreEqual("value", ex.ParamName);
            Assert.AreEqual(before, _asteroid.Name);
        }

        [TestMethod]
        public void Name_Null_ThrowsArgumentException()
        {
            // Arrange
            string? nullName = null;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _asteroid.Name = nullName!);
        }

        [TestMethod]
        [DataRow("Аб")]
        [DataRow("  Аб  ")]
        [DataRow("ABCDEFGHIJKLMNOPQRSTU")]
        public void Name_InvalidLength_ThrowsArgumentException(string badName)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _asteroid.Name = badName);

            // Assert
            StringAssert.Contains(ex.Message, "від 3 до 20 символів");
        }

        [TestMethod]
        [DataRow("Пепероні!")]
        [DataRow("Mars_1")]
        [DataRow("Name@1")]
        [DataRow("Пам'ять")]
        public void Name_SpecialCharacters_ThrowsArgumentException(string badName)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _asteroid.Name = badName);

            // Assert
            StringAssert.Contains(ex.Message, "спецсимволи");
            Assert.AreEqual("Пепероні-X", _asteroid.Name);
        }

        #endregion

        #region Властивість Crust

        [TestMethod]
        [DataRow(CrustType.Thin)]
        [DataRow(CrustType.CheeseStuffed)]
        [DataRow(CrustType.DeepDish)]
        [DataRow(CrustType.Classic)]
        public void Crust_DefinedValue_IsAssigned(CrustType crust)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            _asteroid.Crust = crust;

            // Assert
            Assert.AreEqual(crust, _asteroid.Crust);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(5)]
        [DataRow(99)]
        [DataRow(-1)]
        public void Crust_UndefinedValue_ThrowsArgumentException(int rawValue)
        {
            // Arrange
            CrustType undefined = (CrustType)rawValue;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _asteroid.Crust = undefined);
            Assert.AreEqual(CrustType.Thin, _asteroid.Crust);
        }

        #endregion

        #region Властивість DiameterKm

        [TestMethod]
        [DataRow(0.1)]
        [DataRow(1.0)]
        [DataRow(500.5)]
        [DataRow(1000.0)]
        public void DiameterKm_ValueInRange_IsAssigned(double diameter)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            _asteroid.DiameterKm = diameter;

            // Assert
            Assert.AreEqual(diameter, _asteroid.DiameterKm, Delta);
        }

        [TestMethod]
        [DataRow(12.3456, 12.35)]
        [DataRow(7.0049, 7.0)]
        [DataRow(999.991, 999.99)]
        public void DiameterKm_ManyDecimals_IsRoundedToTwoDigits(double input, double expected)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            _asteroid.DiameterKm = input;

            // Assert
            Assert.AreEqual(expected, _asteroid.DiameterKm, Delta);
        }

        [TestMethod]
        [DataRow(0.09)]
        [DataRow(0.0)]
        [DataRow(-5.0)]
        [DataRow(1000.01)]
        [DataRow(double.NaN)]
        [DataRow(double.PositiveInfinity)]
        [DataRow(double.NegativeInfinity)]
        public void DiameterKm_OutOfRange_ThrowsArgumentOutOfRangeException(double badDiameter)
        {
            // Arrange
            double before = _asteroid.DiameterKm;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _asteroid.DiameterKm = badDiameter);
            Assert.AreEqual(before, _asteroid.DiameterKm, Delta);
        }

        #endregion

        #region Властивість TemperatureCelsius

        [TestMethod]
        [DataRow(-273)]
        [DataRow(0)]
        [DataRow(25)]
        [DataRow(500)]
        public void TemperatureCelsius_ValueInRange_IsAssigned(int temperature)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            _asteroid.TemperatureCelsius = temperature;

            // Assert
            Assert.AreEqual(temperature, _asteroid.TemperatureCelsius);
        }

        [TestMethod]
        [DataRow(-274)]
        [DataRow(501)]
        [DataRow(int.MinValue)]
        [DataRow(int.MaxValue)]
        public void TemperatureCelsius_OutOfRange_ThrowsArgumentOutOfRangeException(int badTemperature)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _asteroid.TemperatureCelsius = badTemperature);
            Assert.AreEqual(-40, _asteroid.TemperatureCelsius);
        }

        #endregion

        #region Властивості SauceType та HasExtraCheese

        [TestMethod]
        public void SauceType_ValidValue_IsTrimmedAndAssigned()
        {
            // Arrange
            string raw = "  Песто  ";

            // Act
            _asteroid.SauceType = raw;

            // Assert
            Assert.AreEqual("Песто", _asteroid.SauceType);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void SauceType_EmptyOrWhiteSpace_ThrowsArgumentException(string badSauce)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _asteroid.SauceType = badSauce);
            Assert.AreEqual("Барбекю", _asteroid.SauceType);
        }

        [TestMethod]
        public void SauceType_ContainsSeparator_ThrowsArgumentException()
        {
            // Arrange
            string badSauce = $"Сир{PizzaAsteroid.Separator}Часник";

            // Act
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _asteroid.SauceType = badSauce);

            // Assert
            StringAssert.Contains(ex.Message, "';'");
        }

        [TestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void HasExtraCheese_SetValue_ReturnsSameValue(bool value)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            _asteroid.HasExtraCheese = value;

            // Assert
            Assert.AreEqual(value, _asteroid.HasExtraCheese);
        }

        [TestMethod]
        public void Separator_Constant_IsSemicolon()
        {
            // Arrange
            char expected = ';';

            // Act
            char actual = PizzaAsteroid.Separator;

            // Assert
            Assert.AreEqual(expected, actual);
        }

        #endregion

        #region Static-властивості ImpactEnergyFactor та CreatedCount

        [TestMethod]
        public void ImpactEnergyFactor_Initial_Is175()
        {
            // Arrange — static-конструктор уже виконався, TestCleanup відновлює значення

            // Act
            double factor = PizzaAsteroid.ImpactEnergyFactor;

            // Assert
            Assert.AreEqual(1.75, factor, Delta);
        }

        [TestMethod]
        [DataRow(0.1)]
        [DataRow(2.5)]
        [DataRow(100.0)]
        public void ImpactEnergyFactor_ValueInRange_IsAssigned(double factor)
        {
            // Arrange — початкове значення збережено в TestInitialize

            // Act
            PizzaAsteroid.ImpactEnergyFactor = factor;

            // Assert
            Assert.AreEqual(factor, PizzaAsteroid.ImpactEnergyFactor, Delta);
        }

        [TestMethod]
        public void ImpactEnergyFactor_ManyDecimals_IsRoundedToThreeDigits()
        {
            // Arrange
            double input = 1.23456;

            // Act
            PizzaAsteroid.ImpactEnergyFactor = input;

            // Assert
            Assert.AreEqual(1.235, PizzaAsteroid.ImpactEnergyFactor, Delta);
        }

        [TestMethod]
        [DataRow(0.09)]
        [DataRow(0.0)]
        [DataRow(-1.0)]
        [DataRow(100.1)]
        [DataRow(double.NaN)]
        public void ImpactEnergyFactor_OutOfRange_ThrowsArgumentOutOfRangeException(double badFactor)
        {
            // Arrange
            double before = PizzaAsteroid.ImpactEnergyFactor;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PizzaAsteroid.ImpactEnergyFactor = badFactor);
            Assert.AreEqual(before, PizzaAsteroid.ImpactEnergyFactor, Delta);
        }

        [TestMethod]
        public void CreatedCount_AfterTestInitialize_IsPositive()
        {
            // Arrange — у TestInitialize вже створено щонайменше один об'єкт

            // Act
            int count = PizzaAsteroid.CreatedCount;

            // Assert
            Assert.IsTrue(count >= 1);
        }

        #endregion

        #region DiscoveryDate / SetDiscoveryDate

        [TestMethod]
        [DataRow(1990, 1, 1)]
        [DataRow(2000, 2, 29)]
        [DataRow(2021, 4, 12)]
        public void SetDiscoveryDate_DateInRange_IsAssigned(int year, int month, int day)
        {
            // Arrange
            var date = new DateTime(year, month, day);

            // Act
            _asteroid.SetDiscoveryDate(date);

            // Assert
            Assert.AreEqual(date, _asteroid.DiscoveryDate);
        }

        [TestMethod]
        public void SetDiscoveryDate_Today_IsAssigned()
        {
            // Arrange
            DateTime today = DateTime.Today;

            // Act
            _asteroid.SetDiscoveryDate(today);

            // Assert
            Assert.AreEqual(today, _asteroid.DiscoveryDate);
        }

        [TestMethod]
        public void SetDiscoveryDate_CurrentMoment_IsAssignedWithoutTime()
        {
            // Arrange
            DateTime now = DateTime.Now;

            // Act
            _asteroid.SetDiscoveryDate(now);

            // Assert
            Assert.AreEqual(now.Date, _asteroid.DiscoveryDate);
        }

        [TestMethod]
        public void SetDiscoveryDate_DateWithTime_TimeIsDiscarded()
        {
            // Arrange
            var dateWithTime = new DateTime(2021, 4, 12, 15, 30, 45);

            // Act
            _asteroid.SetDiscoveryDate(dateWithTime);

            // Assert
            Assert.AreEqual(TimeSpan.Zero, _asteroid.DiscoveryDate.TimeOfDay);
            Assert.AreEqual(new DateTime(2021, 4, 12), _asteroid.DiscoveryDate);
        }

        [TestMethod]
        public void SetDiscoveryDate_BeforeMinDate_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var tooEarly = new DateTime(1989, 12, 31);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _asteroid.SetDiscoveryDate(tooEarly));
            Assert.AreEqual(new DateTime(2021, 4, 12), _asteroid.DiscoveryDate);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(30)]
        [DataRow(3650)]
        public void SetDiscoveryDate_FutureDate_ThrowsArgumentOutOfRangeException(int daysAhead)
        {
            // Arrange
            DateTime future = DateTime.Today.AddDays(daysAhead);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _asteroid.SetDiscoveryDate(future));
            Assert.AreEqual(new DateTime(2021, 4, 12), _asteroid.DiscoveryDate);
        }

        #endregion

        #region EstimatedVolumeKm3

        [TestMethod]
        [DataRow(10.0, 523.6)]
        [DataRow(2.0, 4.19)]
        [DataRow(12.5, 1022.65)]
        [DataRow(1000.0, 523598775.6)]
        public void EstimatedVolumeKm3_LargeVolume_IsRoundedToTwoDigits(double diameter, double expected)
        {
            // Arrange
            _asteroid.DiameterKm = diameter;

            // Act
            double volume = _asteroid.EstimatedVolumeKm3;

            // Assert
            Assert.AreEqual(expected, volume, 1e-6);
        }

        [TestMethod]
        [DataRow(0.1, 0.00052)]
        [DataRow(0.2, 0.00419)]
        public void EstimatedVolumeKm3_SmallVolume_IsRoundedToFiveDigits(double diameter, double expected)
        {
            // Arrange
            _asteroid.DiameterKm = diameter;

            // Act
            double volume = _asteroid.EstimatedVolumeKm3;

            // Assert
            Assert.AreEqual(expected, volume, Delta);
            Assert.IsTrue(volume < 0.01);
        }

        #endregion

        #region HeatUp(int)

        [TestMethod]
        [DataRow(30, -10)]
        [DataRow(1, -39)]
        [DataRow(540, 500)]
        public void HeatUp_PositiveDegrees_IncreasesTemperature(int degrees, int expected)
        {
            // Arrange — температура еталонного об'єкта -40°C

            // Act
            _asteroid.HeatUp(degrees);

            // Assert
            Assert.AreEqual(expected, _asteroid.TemperatureCelsius);
        }

        [TestMethod]
        public void HeatUp_DegreesAboveLimit_ClampsTo500()
        {
            // Arrange
            _asteroid.TemperatureCelsius = 450;

            // Act
            _asteroid.HeatUp(100);

            // Assert
            Assert.AreEqual(500, _asteroid.TemperatureCelsius);
        }

        [TestMethod]
        public void HeatUp_MaxIntDegrees_ClampsTo500WithoutOverflow()
        {
            // Arrange
            _asteroid.TemperatureCelsius = 100;

            // Act
            _asteroid.HeatUp(int.MaxValue);

            // Assert
            Assert.AreEqual(500, _asteroid.TemperatureCelsius);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-100)]
        public void HeatUp_NonPositiveDegrees_ThrowsArgumentOutOfRangeException(int degrees)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _asteroid.HeatUp(degrees));

            // Assert
            Assert.AreEqual("degrees", ex.ParamName);
            Assert.AreEqual(-40, _asteroid.TemperatureCelsius);
        }

        #endregion

        #region HeatUp(int, string)

        [TestMethod]
        public void HeatUpWithSource_ValidArguments_ReturnsReportAndHeats()
        {
            // Arrange
            string source = "  Сонячний спалах  ";

            // Act
            string report = _asteroid.HeatUp(30, source);

            // Assert
            Assert.AreEqual("Астероїд 'Пепероні-X' нагріто джерелом 'Сонячний спалах' на 30°C.", report);
            Assert.AreEqual(-10, _asteroid.TemperatureCelsius);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void HeatUpWithSource_EmptySource_ThrowsArgumentException(string source)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _asteroid.HeatUp(30, source));

            // Assert
            Assert.AreEqual("heatSource", ex.ParamName);
            Assert.AreEqual(-40, _asteroid.TemperatureCelsius);
        }

        [TestMethod]
        public void HeatUpWithSource_NullSource_ThrowsArgumentException()
        {
            // Arrange
            string? source = null;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _asteroid.HeatUp(30, source!));
        }

        [TestMethod]
        public void HeatUpWithSource_NonPositiveDegrees_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string source = "Вулкан";

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _asteroid.HeatUp(0, source));
            Assert.AreEqual(-40, _asteroid.TemperatureCelsius);
        }

        #endregion

        #region HeatUp(double)

        [TestMethod]
        [DataRow(1.5, -20)]   // база 40, приріст 40 * 0.5 = 20
        [DataRow(1.1, -35)]   // приріст 4 < 5, тому застосовується мінімум 5
        [DataRow(10.0, 320)]  // верхня межа коефіцієнта: приріст 40 * 9 = 360
        public void HeatUpFactor_ValidFactor_IncreasesTemperature(double factor, int expected)
        {
            // Arrange — температура еталонного об'єкта -40°C

            // Act
            _asteroid.HeatUp(factor);

            // Assert
            Assert.AreEqual(expected, _asteroid.TemperatureCelsius);
        }

        [TestMethod]
        public void HeatUpFactor_ZeroTemperature_UsesBaseOf20()
        {
            // Arrange
            _asteroid.TemperatureCelsius = 0;

            // Act
            _asteroid.HeatUp(2.0);

            // Assert
            Assert.AreEqual(20, _asteroid.TemperatureCelsius);
        }

        [TestMethod]
        public void HeatUpFactor_ResultAboveLimit_ClampsTo500()
        {
            // Arrange
            _asteroid.TemperatureCelsius = 300;

            // Act
            _asteroid.HeatUp(3.0);

            // Assert
            Assert.AreEqual(500, _asteroid.TemperatureCelsius);
        }

        [TestMethod]
        [DataRow(1.0)]
        [DataRow(0.5)]
        [DataRow(-2.0)]
        [DataRow(10.01)]
        [DataRow(double.NaN)]
        public void HeatUpFactor_InvalidFactor_ThrowsArgumentOutOfRangeException(double factor)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            var ex = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _asteroid.HeatUp(factor));

            // Assert
            Assert.AreEqual("factor", ex.ParamName);
            Assert.AreEqual(-40, _asteroid.TemperatureCelsius);
        }

        #endregion

        #region Slice

        [TestMethod]
        [DataRow(10.0, 2, 7.07)]
        [DataRow(12.5, 4, 6.25)]
        [DataRow(100.0, 100, 10.0)]
        public void Slice_ValidParts_DividesDiameterBySqrtOfParts(double diameter, int parts, double expected)
        {
            // Arrange
            _asteroid.DiameterKm = diameter;

            // Act
            _asteroid.Slice(parts);

            // Assert
            Assert.AreEqual(expected, _asteroid.DiameterKm, Delta);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(0)]
        [DataRow(-3)]
        public void Slice_LessThanTwoParts_ThrowsArgumentException(int parts)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _asteroid.Slice(parts));

            // Assert
            Assert.AreEqual("parts", ex.ParamName);
            Assert.AreEqual(12.5, _asteroid.DiameterKm, Delta);
        }

        [TestMethod]
        public void Slice_ResultTooSmall_ThrowsAndKeepsDiameter()
        {
            // Arrange
            _asteroid.DiameterKm = 0.5;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => _asteroid.Slice(100));
            Assert.AreEqual(0.5, _asteroid.DiameterKm, Delta);
        }

        #endregion

        #region CollideWithTarget

        [TestMethod]
        public void CollideWithTarget_LargeAsteroid_ReturnsReportWithTwoDecimals()
        {
            // Arrange
            var asteroid = new PizzaAsteroid("Церера", CrustType.Classic, 10.0);   // V = 523.6 км³

            // Act
            string report = asteroid.CollideWithTarget("Марс");

            // Assert
            Assert.AreEqual(
                "Астероїд 'Церера' зіткнувся з об'єктом 'Марс'. Енергія удару: 916.30 Мт ТНТ. Соус розлетівся по орбіті!",
                report);
        }

        [TestMethod]
        public void CollideWithTarget_TinyAsteroid_ReturnsEnergyWithFourDecimals()
        {
            // Arrange
            var asteroid = new PizzaAsteroid("Крихта", CrustType.Thin, 0.1);   // V = 0.00052 км³

            // Act
            string report = asteroid.CollideWithTarget("Місяць");

            // Assert
            StringAssert.Contains(report, "Енергія удару: 0.0009 Мт ТНТ");
        }

        [TestMethod]
        [DataRow(2.0, "1047.20")]
        [DataRow(0.5, "261.80")]
        public void CollideWithTarget_ChangedImpactFactor_AffectsEnergy(double factor, string expectedEnergy)
        {
            // Arrange
            var asteroid = new PizzaAsteroid("Церера", CrustType.Classic, 10.0);
            PizzaAsteroid.ImpactEnergyFactor = factor;

            // Act
            string report = asteroid.CollideWithTarget("Земля");

            // Assert
            StringAssert.Contains(report, $"Енергія удару: {expectedEnergy} Мт ТНТ");
        }

        [TestMethod]
        public void CollideWithTarget_TargetWithSpaces_IsTrimmed()
        {
            // Arrange
            string target = "   Юпітер   ";

            // Act
            string report = _asteroid.CollideWithTarget(target);

            // Assert
            StringAssert.StartsWith(report, "Астероїд 'Пепероні-X' зіткнувся з об'єктом 'Юпітер'.");
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void CollideWithTarget_EmptyTarget_ThrowsArgumentException(string target)
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            var ex = Assert.ThrowsExactly<ArgumentException>(() => _asteroid.CollideWithTarget(target));

            // Assert
            Assert.AreEqual("targetPlanet", ex.ParamName);
        }

        [TestMethod]
        public void CollideWithTarget_NullTarget_ThrowsArgumentException()
        {
            // Arrange
            string? target = null;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _asteroid.CollideWithTarget(target!));
        }

        #endregion

        #region ToString

        [TestMethod]
        public void ToString_FullObject_ReturnsSeparatedFields()
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            string result = _asteroid.ToString();

            // Assert
            Assert.AreEqual("Пепероні-X;Thin;12.5;-40;так;12.04.2021;Барбекю", result);
        }

        [TestMethod]
        public void ToString_WithoutExtraCheese_ContainsNi()
        {
            // Arrange
            var asteroid = new PizzaAsteroid("Церера", CrustType.DeepDish, 10.0, 0, false, new DateTime(2000, 1, 5));
            string[] expected = { "Церера", "DeepDish", "10", "0", "ні", "05.01.2000", DefaultSauce };

            // Act
            string[] parts = asteroid.ToString().Split(PizzaAsteroid.Separator);

            // Assert
            CollectionAssert.AreEqual(expected, parts);
        }

        [TestMethod]
        public void ToString_UkrainianCulture_UsesDotAsDecimalSeparator()
        {
            // Arrange
            CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

            // Act
            string result = _asteroid.ToString();

            // Assert
            StringAssert.Contains(result, ";12.5;");
            Assert.IsFalse(result.Contains("12,5"));
        }

        #endregion

        #region Parse

        [TestMethod]
        public void Parse_SevenFields_CreatesObjectWithAllValues()
        {
            // Arrange
            string input = "Пепероні-X;Thin;12.5;-40;так;12.04.2021;Барбекю";

            // Act
            PizzaAsteroid result = PizzaAsteroid.Parse(input);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Пепероні-X", result.Name);
            Assert.AreEqual(CrustType.Thin, result.Crust);
            Assert.AreEqual(12.5, result.DiameterKm, Delta);
            Assert.AreEqual(-40, result.TemperatureCelsius);
            Assert.IsTrue(result.HasExtraCheese);
            Assert.AreEqual(new DateTime(2021, 4, 12), result.DiscoveryDate);
            Assert.AreEqual("Барбекю", result.SauceType);
        }

        [TestMethod]
        [DataRow("Церера;Classic;5;10;ні;01.01.2000")]
        [DataRow("Церера;Classic;5;10;ні;01.01.2000;")]
        [DataRow("Церера;Classic;5;10;ні;01.01.2000;   ")]
        public void Parse_WithoutSauce_UsesDefaultSauce(string input)
        {
            // Arrange — вхідний рядок передано через DataRow

            // Act
            PizzaAsteroid result = PizzaAsteroid.Parse(input);

            // Assert
            Assert.AreEqual(DefaultSauce, result.SauceType);
        }

        [TestMethod]
        public void Parse_FieldsWithSpaces_AreTrimmed()
        {
            // Arrange
            string input = "  Церера ; Classic ; 5 ; 10 ; ні ; 01.01.2000 ; Песто ";

            // Act
            PizzaAsteroid result = PizzaAsteroid.Parse(input);

            // Assert
            Assert.AreEqual("Церера", result.Name);
            Assert.AreEqual(CrustType.Classic, result.Crust);
            Assert.AreEqual("Песто", result.SauceType);
        }

        [TestMethod]
        [DataRow("Thin", CrustType.Thin)]
        [DataRow("thin", CrustType.Thin)]
        [DataRow("DEEPDISH", CrustType.DeepDish)]
        [DataRow("CheeseStuffed", CrustType.CheeseStuffed)]
        [DataRow("1", CrustType.Thin)]
        [DataRow("2", CrustType.CheeseStuffed)]
        [DataRow("3", CrustType.DeepDish)]
        [DataRow("4", CrustType.Classic)]
        public void Parse_CrustByNameOrNumber_IsRecognized(string crustField, CrustType expected)
        {
            // Arrange
            string input = $"Церера;{crustField};5;10;ні;01.01.2000";

            // Act
            PizzaAsteroid result = PizzaAsteroid.Parse(input);

            // Assert
            Assert.AreEqual(expected, result.Crust);
        }

        [TestMethod]
        [DataRow("12.5", 12.5)]
        [DataRow("12,5", 12.5)]
        [DataRow("7", 7.0)]
        public void Parse_DiameterWithDotOrComma_IsRecognized(string diameterField, double expected)
        {
            // Arrange
            string input = $"Церера;Thin;{diameterField};10;ні;01.01.2000";

            // Act
            PizzaAsteroid result = PizzaAsteroid.Parse(input);

            // Assert
            Assert.AreEqual(expected, result.DiameterKm, Delta);
        }

        [TestMethod]
        [DataRow("так", true)]
        [DataRow("ТАК", true)]
        [DataRow("1", true)]
        [DataRow("true", true)]
        [DataRow("True", true)]
        [DataRow("ні", false)]
        [DataRow("НІ", false)]
        [DataRow("0", false)]
        [DataRow("false", false)]
        public void Parse_CheeseVariants_AreRecognized(string cheeseField, bool expected)
        {
            // Arrange
            string input = $"Церера;Thin;5;10;{cheeseField};01.01.2000";

            // Act
            PizzaAsteroid result = PizzaAsteroid.Parse(input);

            // Assert
            Assert.AreEqual(expected, result.HasExtraCheese);
        }

        [TestMethod]
        public void Parse_ValidString_IncrementsCreatedCount()
        {
            // Arrange
            int countBefore = PizzaAsteroid.CreatedCount;

            // Act
            _ = PizzaAsteroid.Parse("Церера;Thin;5;10;ні;01.01.2000");

            // Assert
            Assert.AreEqual(countBefore + 1, PizzaAsteroid.CreatedCount);
        }

        [TestMethod]
        public void Parse_ToStringResult_RoundTripGivesSameString()
        {
            // Arrange
            string original = _asteroid.ToString();

            // Act
            PizzaAsteroid copy = PizzaAsteroid.Parse(original);

            // Assert
            Assert.AreNotSame(_asteroid, copy);
            Assert.AreEqual(original, copy.ToString());
        }

        [TestMethod]
        public void Parse_Null_ThrowsArgumentNullException()
        {
            // Arrange
            string? input = null;

            // Act
            var ex = Assert.ThrowsExactly<ArgumentNullException>(() => PizzaAsteroid.Parse(input!));

            // Assert
            Assert.AreEqual("s", ex.ParamName);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("    ")]
        public void Parse_EmptyString_ThrowsFormatException(string input)
        {
            // Arrange — вхідний рядок передано через DataRow

            // Act & Assert
            Assert.ThrowsExactly<FormatException>(() => PizzaAsteroid.Parse(input));
        }

        [TestMethod]
        [DataRow("Церера")]
        [DataRow("Церера;Thin;5;10;ні")]
        [DataRow("Церера;Thin;5;10;ні;01.01.2000;Песто;Зайве")]
        public void Parse_WrongFieldCount_ThrowsFormatException(string input)
        {
            // Arrange — вхідний рядок передано через DataRow

            // Act
            var ex = Assert.ThrowsExactly<FormatException>(() => PizzaAsteroid.Parse(input));

            // Assert
            StringAssert.Contains(ex.Message, "Очікується 6 або 7 полів");
        }

        [TestMethod]
        [DataRow("Церера;;5;10;ні;01.01.2000", "порожнє")]
        [DataRow("Церера;0;5;10;ні;01.01.2000", "поза межами")]
        [DataRow("Церера;5;5;10;ні;01.01.2000", "поза межами")]
        [DataRow("Церера;Square;5;10;ні;01.01.2000", "невідоме значення")]
        [DataRow("Церера;Thin,Classic;5;10;ні;01.01.2000", "невідоме значення")]
        public void Parse_InvalidCrust_ThrowsFormatException(string input, string expectedMessagePart)
        {
            // Arrange — вхідний рядок передано через DataRow

            // Act
            var ex = Assert.ThrowsExactly<FormatException>(() => PizzaAsteroid.Parse(input));

            // Assert
            StringAssert.Contains(ex.Message, "Бортик");
            StringAssert.Contains(ex.Message, expectedMessagePart);
        }

        [TestMethod]
        [DataRow("Церера;Thin;abc;10;ні;01.01.2000", "Діаметр")]
        [DataRow("Церера;Thin;5;12.5;ні;01.01.2000", "Температура")]
        [DataRow("Церера;Thin;5;тепло;ні;01.01.2000", "Температура")]
        [DataRow("Церера;Thin;5;10;може;01.01.2000", "Сир")]
        [DataRow("Церера;Thin;5;10;ні;2000-01-01", "Дата")]
        [DataRow("Церера;Thin;5;10;ні;31.02.2021", "Дата")]
        public void Parse_InvalidFieldFormat_ThrowsFormatExceptionWithFieldName(string input, string fieldName)
        {
            // Arrange — вхідний рядок передано через DataRow

            // Act
            var ex = Assert.ThrowsExactly<FormatException>(() => PizzaAsteroid.Parse(input));

            // Assert
            StringAssert.Contains(ex.Message, fieldName);
        }

        [TestMethod]
        [DataRow("Церера;Thin;5000;10;ні;01.01.2000")]
        [DataRow("Церера;Thin;5;-300;ні;01.01.2000")]
        [DataRow("Церера;Thin;5;10;ні;01.01.1980")]
        public void Parse_ValueBreaksDomainRules_ThrowsArgumentOutOfRangeException(string input)
        {
            // Arrange
            int countBefore = PizzaAsteroid.CreatedCount;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PizzaAsteroid.Parse(input));
            Assert.AreEqual(countBefore, PizzaAsteroid.CreatedCount);
        }

        [TestMethod]
        public void Parse_InvalidName_ThrowsArgumentException()
        {
            // Arrange
            string input = "X;Thin;5;10;ні;01.01.2000";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => PizzaAsteroid.Parse(input));
        }

        #endregion

        #region TryParse

        [TestMethod]
        public void TryParse_ValidString_ReturnsTrueAndObject()
        {
            // Arrange
            string input = "Церера;DeepDish;20;15;так;10.10.2010;Песто";

            // Act
            bool success = PizzaAsteroid.TryParse(input, out PizzaAsteroid? result);

            // Assert
            Assert.IsTrue(success);
            Assert.IsNotNull(result);
            Assert.AreEqual(input, result.ToString());
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("Церера;Thin;5")]
        [DataRow("Церера;Square;5;10;ні;01.01.2000")]
        [DataRow("X;Thin;5;10;ні;01.01.2000")]
        [DataRow("Церера;Thin;5000;10;ні;01.01.2000")]
        public void TryParse_InvalidString_ReturnsFalseAndNull(string input)
        {
            // Arrange — вхідний рядок передано через DataRow

            // Act
            bool success = PizzaAsteroid.TryParse(input, out PizzaAsteroid? result);

            // Assert
            Assert.IsFalse(success);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void TryParse_Null_ReturnsFalseWithoutException()
        {
            // Arrange
            string? input = null;

            // Act
            bool success = PizzaAsteroid.TryParse(input!, out PizzaAsteroid? result);

            // Assert
            Assert.IsFalse(success);
            Assert.IsNull(result);
        }

        #endregion

        #region Merge

        [TestMethod]
        public void Merge_TwoAsteroids_CombinesPropertiesCorrectly()
        {
            // Arrange
            var a = new PizzaAsteroid("Альфа", CrustType.Thin, 10.0, -40, false, new DateTime(2010, 5, 1), "Песто");
            var b = new PizzaAsteroid("Бета", CrustType.DeepDish, 8.0, 20, true, new DateTime(2005, 3, 15), "Барбекю");

            // Act
            PizzaAsteroid merged = PizzaAsteroid.Merge(a, b, "Гамма");

            // Assert
            Assert.AreEqual("Гамма", merged.Name);
            Assert.AreEqual(11.48, merged.DiameterKm, Delta);       // r³ = 5³ + 4³ = 189
            Assert.AreEqual(CrustType.Thin, merged.Crust);           // від більшого (a)
            Assert.AreEqual("Песто", merged.SauceType);              // від більшого (a)
            Assert.AreEqual(-10, merged.TemperatureCelsius);         // (-40 + 20) / 2
            Assert.IsTrue(merged.HasExtraCheese);                    // false || true
            Assert.AreEqual(new DateTime(2005, 3, 15), merged.DiscoveryDate);
        }

        [TestMethod]
        public void Merge_TwoAsteroids_PreservesTotalVolume()
        {
            // Arrange
            var a = new PizzaAsteroid("Альфа", CrustType.Thin, 10.0);
            var b = new PizzaAsteroid("Бета", CrustType.Thin, 10.0);
            double expectedVolume = a.EstimatedVolumeKm3 + b.EstimatedVolumeKm3;

            // Act
            PizzaAsteroid merged = PizzaAsteroid.Merge(a, b, "Гамма");

            // Assert
            Assert.AreEqual(12.6, merged.DiameterKm, Delta);   // r³ = 125 + 125 = 250
            Assert.AreEqual(expectedVolume, merged.EstimatedVolumeKm3, expectedVolume * 0.01);
        }

        [TestMethod]
        public void Merge_SecondIsBigger_TakesCrustAndSauceFromSecond()
        {
            // Arrange
            var small = new PizzaAsteroid("Малий", CrustType.Thin, 2.0, 0, false, new DateTime(2020, 1, 1), "Песто");
            var big = new PizzaAsteroid("Великий", CrustType.Classic, 20.0, 0, false, new DateTime(2021, 1, 1), "Сирний");

            // Act
            PizzaAsteroid merged = PizzaAsteroid.Merge(small, big, "Злиток");

            // Assert
            Assert.AreEqual(CrustType.Classic, merged.Crust);
            Assert.AreEqual("Сирний", merged.SauceType);
            Assert.IsFalse(merged.HasExtraCheese);
            Assert.AreEqual(new DateTime(2020, 1, 1), merged.DiscoveryDate);
        }

        [TestMethod]
        public void Merge_EqualDiameters_TakesCrustFromFirst()
        {
            // Arrange
            var a = new PizzaAsteroid("Альфа", CrustType.DeepDish, 5.0, 10, true);
            var b = new PizzaAsteroid("Бета", CrustType.CheeseStuffed, 5.0, 30, false);

            // Act
            PizzaAsteroid merged = PizzaAsteroid.Merge(a, b, "Гамма");

            // Assert
            Assert.AreEqual(CrustType.DeepDish, merged.Crust);
            Assert.AreEqual(20, merged.TemperatureCelsius);
            Assert.IsTrue(merged.HasExtraCheese);   // true || false
        }

        [TestMethod]
        public void Merge_TwoAsteroids_ReturnsNewObjectAndKeepsOriginals()
        {
            // Arrange
            var a = new PizzaAsteroid("Альфа", CrustType.Thin, 10.0);
            var b = new PizzaAsteroid("Бета", CrustType.Thin, 8.0);
            int countBefore = PizzaAsteroid.CreatedCount;

            // Act
            PizzaAsteroid merged = PizzaAsteroid.Merge(a, b, "Гамма");

            // Assert
            Assert.AreNotSame(a, merged);
            Assert.AreNotSame(b, merged);
            Assert.AreEqual(10.0, a.DiameterKm, Delta);
            Assert.AreEqual(8.0, b.DiameterKm, Delta);
            Assert.AreEqual(countBefore + 1, PizzaAsteroid.CreatedCount);
        }

        [TestMethod]
        public void Merge_FirstIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            PizzaAsteroid? a = null;

            // Act
            var ex = Assert.ThrowsExactly<ArgumentNullException>(() => PizzaAsteroid.Merge(a!, _asteroid, "Гамма"));

            // Assert
            Assert.AreEqual("a", ex.ParamName);
        }

        [TestMethod]
        public void Merge_SecondIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            PizzaAsteroid? b = null;

            // Act
            var ex = Assert.ThrowsExactly<ArgumentNullException>(() => PizzaAsteroid.Merge(_asteroid, b!, "Гамма"));

            // Assert
            Assert.AreEqual("b", ex.ParamName);
        }

        [TestMethod]
        public void Merge_SameObject_ThrowsArgumentException()
        {
            // Arrange — еталонний об'єкт створено в TestInitialize

            // Act
            var ex = Assert.ThrowsExactly<ArgumentException>(() => PizzaAsteroid.Merge(_asteroid, _asteroid, "Гамма"));

            // Assert
            StringAssert.Contains(ex.Message, "сам із собою");
        }

        [TestMethod]
        public void Merge_InvalidNewName_ThrowsArgumentException()
        {
            // Arrange
            var other = new PizzaAsteroid("Бета", CrustType.Thin, 8.0);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => PizzaAsteroid.Merge(_asteroid, other, "X"));
        }

        [TestMethod]
        public void Merge_ResultTooBig_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var a = new PizzaAsteroid("Гігант-1", CrustType.Thin, 1000.0);
            var b = new PizzaAsteroid("Гігант-2", CrustType.Thin, 1000.0);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => PizzaAsteroid.Merge(a, b, "Гігант-3"));
        }

        #endregion

        #region TotalVolume

        [TestMethod]
        public void TotalVolume_SeveralAsteroids_ReturnsSumOfVolumes()
        {
            // Arrange
            var list = new List<PizzaAsteroid>
            {
                new("Альфа", CrustType.Thin, 10.0),   // 523.6
                new("Бета", CrustType.Thin, 2.0)      // 4.19
            };

            // Act
            double total = PizzaAsteroid.TotalVolume(list);

            // Assert
            Assert.AreEqual(527.79, total, Delta);
        }

        [TestMethod]
        public void TotalVolume_SingleAsteroid_EqualsItsVolume()
        {
            // Arrange
            var array = new[] { _asteroid };

            // Act
            double total = PizzaAsteroid.TotalVolume(array);

            // Assert
            Assert.AreEqual(_asteroid.EstimatedVolumeKm3, total, Delta);
        }

        [TestMethod]
        public void TotalVolume_EmptyCollection_ReturnsZero()
        {
            // Arrange
            var empty = new List<PizzaAsteroid>();

            // Act
            double total = PizzaAsteroid.TotalVolume(empty);

            // Assert
            Assert.AreEqual(0.0, total, Delta);
        }

        [TestMethod]
        public void TotalVolume_NullCollection_ThrowsArgumentNullException()
        {
            // Arrange
            List<PizzaAsteroid>? list = null;

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => PizzaAsteroid.TotalVolume(list!));
        }

        [TestMethod]
        public void TotalVolume_CollectionWithNullItem_ThrowsArgumentException()
        {
            // Arrange
            var list = new List<PizzaAsteroid> { _asteroid, null! };

            // Act
            var ex = Assert.ThrowsExactly<ArgumentException>(() => PizzaAsteroid.TotalVolume(list));

            // Assert
            Assert.AreEqual("asteroids", ex.ParamName);
        }

        #endregion
    }
}
