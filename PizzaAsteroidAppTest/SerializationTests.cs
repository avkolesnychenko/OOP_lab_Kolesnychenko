using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PizzaAsteroidApp.Tests
{
    [TestClass]
    public sealed class SerializationTests
    {
        private const string DefaultSauce = "Томатний Класик";
        private const int BigCapacity = 100;

        private string _dir = null!;
        private List<PizzaAsteroid> _list = null!;
        private PizzaAsteroid _alpha = null!;
        private PizzaAsteroid _beta = null!;
        private PizzaAsteroid _vega = null!;

        [TestInitialize]
        public void Setup()
        {
            _dir = Path.Combine(Path.GetTempPath(), "PizzaLab7_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_dir);

            _alpha = new PizzaAsteroid("Альфа", CrustType.Thin, 10.0, -50, true, new DateTime(2020, 1, 15), "Барбекю");
            _beta = new PizzaAsteroid("Бета", CrustType.Classic, 20.0, 0, false, new DateTime(2015, 6, 1));
            _vega = new PizzaAsteroid("Вега", CrustType.DeepDish, 5.5, 100, false, new DateTime(2010, 3, 3), "Часниковий");
            _list = [_alpha, _beta, _vega];
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(_dir))
                Directory.Delete(_dir, true);
        }

        #region Допоміжні методи

        private string FilePath(string name) => Path.Combine(_dir, name);

        private string WriteCsv(params string[] lines)
        {
            string path = FilePath("test.csv");
            File.WriteAllLines(path, lines, Encoding.UTF8);
            return path;
        }

        private string WriteJson(string content)
        {
            string path = FilePath("test.json");
            File.WriteAllText(path, content, Encoding.UTF8);
            return path;
        }

        private static JsonObject ValidJsonElement() => new()
        {
            ["name"] = "Церера",
            ["crust"] = "Thin",
            ["diameter_km"] = 12.5,
            ["temperature_c"] = -40,
            ["extra_cheese"] = true,
            ["discovery_date"] = "2021-04-12T00:00:00",
            ["sauce"] = "Песто"
        };

        private static string JsonArrayOf(params JsonNode?[] items) => new JsonArray(items).ToJsonString();

        private static List<string> ToStrings(IEnumerable<PizzaAsteroid> items) => items.Select(a => a.ToString()).ToList();

        #endregion

        #region CsvHeader / IsCsvHeader / BuildFilePath

        [TestMethod]
        public void CsvHeader_HasSevenColumnsSeparatedBySeparator()
        {
            // Act
            string[] columns = Program.CsvHeader.Split(PizzaAsteroid.Separator);

            // Assert
            Assert.AreEqual(7, columns.Length);
            Assert.AreEqual("Name", columns[0]);
            Assert.AreEqual("SauceType", columns[6]);
        }

        [TestMethod]
        [DataRow("Name;Crust;DiameterKm;TemperatureCelsius;HasExtraCheese;DiscoveryDate;SauceType", true)]
        [DataRow("name ; crust ; diameterkm ; temperaturecelsius ; hasextracheese ; discoverydate ; saucetype", true)]
        [DataRow("Name;Crust;DiameterKm", false)]
        [DataRow("Альфа;Thin;10;-50;так;15.01.2020;Барбекю", false)]
        [DataRow("", false)]
        [DataRow(null, false)]
        public void IsCsvHeader_ReturnsExpected(string? line, bool expected)
        {
            // Act
            bool result = Program.IsCsvHeader(line);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(null, "asteroids.csv")]
        [DataRow("", "asteroids.csv")]
        [DataRow("   ", "asteroids.csv")]
        [DataRow("data", "data.csv")]
        [DataRow("data.csv", "data.csv")]
        [DataRow("DATA.CSV", "DATA.CSV")]
        [DataRow("data.txt", "data.txt.csv")]
        [DataRow("  \"my file\"  ", "my file.csv")]
        [DataRow("\"\"", "asteroids.csv")]
        [DataRow(" \" \" ", "asteroids.csv")]
        public void BuildFilePath_Csv_ReturnsExpected(string? input, string expected)
        {
            // Act
            string result = Program.BuildFilePath(input, Program.DefaultCsvFile, Program.CsvExtension);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("backup", "backup.json")]
        [DataRow("backup.json", "backup.json")]
        [DataRow("backup.csv", "backup.csv.json")]
        public void BuildFilePath_Json_ReturnsExpected(string input, string expected)
        {
            // Act
            string result = Program.BuildFilePath(input, Program.DefaultJsonFile, Program.JsonExtension);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow("", ".csv")]
        [DataRow("asteroids.csv", "")]
        [DataRow("asteroids.csv", "csv")]
        [DataRow("   ", ".csv")]
        [DataRow("asteroids.csv", "   ")]
        public void BuildFilePath_InvalidArguments_ThrowsArgumentException(string defaultName, string extension)
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Program.BuildFilePath("data", defaultName, extension));
        }

        #endregion

        #region GetDataDirectory / GetFiles / ResolveFileChoice

        [TestMethod]
        public void GetDataDirectory_CsprojInParentFolder_ReturnsDataInProjectFolder()
        {
            // Arrange
            string project = Path.Combine(_dir, "Project");
            string bin = Path.Combine(project, "bin", "Debug", "net9.0");
            Directory.CreateDirectory(bin);
            File.WriteAllText(Path.Combine(project, "App.csproj"), "<Project />");

            // Act
            string result = Program.GetDataDirectory(bin);

            // Assert
            Assert.AreEqual(Path.Combine(project, Program.DataFolderName), result);
        }

        [TestMethod]
        public void GetDataDirectory_CsprojInSameFolder_ReturnsDataInThatFolder()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_dir, "App.csproj"), "<Project />");

            // Act
            string result = Program.GetDataDirectory(_dir);

            // Assert
            Assert.AreEqual(Path.Combine(_dir, Program.DataFolderName), result);
        }

        [TestMethod]
        public void GetDataDirectory_NoCsproj_ReturnsDataNextToStartFolder()
        {
            // Arrange
            string start = Path.Combine(_dir, "app");
            Directory.CreateDirectory(start);

            // Act
            string result = Program.GetDataDirectory(start);

            // Assert
            Assert.AreEqual(Path.Combine(start, Program.DataFolderName), result);
        }

        [TestMethod]
        public void GetDataDirectory_StartFolderDoesNotExist_SearchesInParents()
        {
            // Arrange
            File.WriteAllText(Path.Combine(_dir, "App.csproj"), "<Project />");
            string missing = Path.Combine(_dir, "not", "created");

            // Act
            string result = Program.GetDataDirectory(missing);

            // Assert
            Assert.IsFalse(Directory.Exists(missing));
            Assert.AreEqual(Path.Combine(_dir, Program.DataFolderName), result);
        }

        [TestMethod]
        public void GetDataDirectory_StartFolderDoesNotExistAndNoCsproj_ReturnsDataInsideStartFolder()
        {
            // Arrange
            string missing = Path.Combine(_dir, "not", "created");

            // Act
            string result = Program.GetDataDirectory(missing);

            // Assert
            Assert.AreEqual(Path.Combine(missing, Program.DataFolderName), result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        public void GetDataDirectory_EmptyPath_ThrowsArgumentException(string start)
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Program.GetDataDirectory(start));
        }

        [TestMethod]
        public void GetFiles_MixedFiles_ReturnsOnlyMatchingExtensionSorted()
        {
            // Arrange
            foreach (string name in new[] { "b.csv", "A.CSV", "c.json", "d.txt", "e.csvx" })
                File.WriteAllText(FilePath(name), "");
            Directory.CreateDirectory(FilePath("folder.csv"));
            var expected = new List<string> { "A.CSV", "b.csv" };

            // Act
            List<string> csv = Program.GetFiles(_dir, ".csv");
            List<string> json = Program.GetFiles(_dir, ".json");

            // Assert
            CollectionAssert.AreEqual(expected, csv);
            CollectionAssert.AreEqual(new List<string> { "c.json" }, json);
        }

        [TestMethod]
        public void GetFiles_MissingDirectory_ReturnsEmptyList()
        {
            // Act
            List<string> result = Program.GetFiles(FilePath("absent"), ".csv");

            // Assert
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        [DataRow("", ".csv")]
        [DataRow("   ", ".csv")]
        [DataRow("dir", "csv")]
        [DataRow("dir", "")]
        [DataRow("dir", "   ")]
        public void GetFiles_InvalidArguments_ThrowsArgumentException(string directory, string extension)
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Program.GetFiles(directory, extension));
        }

        [TestMethod]
        [DataRow("1", "alpha.csv")]
        [DataRow(" 3 ", "gamma.csv")]
        [DataRow("4", "4.csv")]
        [DataRow("0", "0.csv")]
        [DataRow("beta", "beta.csv")]
        [DataRow("other.csv", "other.csv")]
        [DataRow("", "asteroids.csv")]
        [DataRow(null, "asteroids.csv")]
        public void ResolveFileChoice_ReturnsExpected(string? input, string expected)
        {
            // Arrange
            var files = new List<string> { "alpha.csv", "beta.csv", "gamma.csv" };

            // Act
            string result = Program.ResolveFileChoice(input, files, Program.DefaultCsvFile, Program.CsvExtension);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ResolveFileChoice_EmptyList_NumberIsTreatedAsName()
        {
            // Act
            string result = Program.ResolveFileChoice("1", [], Program.DefaultCsvFile, Program.CsvExtension);

            // Assert
            Assert.AreEqual("1.csv", result);
        }

        [TestMethod]
        public void ResolveFileChoice_NullList_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(
                () => Program.ResolveFileChoice("1", null!, Program.DefaultCsvFile, Program.CsvExtension));
        }

        #endregion

        #region SaveToCsv

        [TestMethod]
        public void SaveToCsv_ThreeObjects_WritesHeaderAndOneLinePerObject()
        {
            // Arrange
            string path = FilePath("out.csv");

            // Act
            int saved = Program.SaveToCsv(_list, path);
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);

            // Assert
            Assert.AreEqual(3, saved);
            Assert.AreEqual(4, lines.Length);
            Assert.AreEqual(Program.CsvHeader, lines[0]);
            Assert.AreEqual("Альфа;Thin;10;-50;так;15.01.2020;Барбекю", lines[1]);
            Assert.AreEqual(_beta.ToString(), lines[2]);
            Assert.AreEqual(_vega.ToString(), lines[3]);
        }

        [TestMethod]
        public void SaveToCsv_EmptyList_WritesOnlyHeader()
        {
            // Arrange
            string path = FilePath("empty.csv");

            // Act
            int saved = Program.SaveToCsv([], path);
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);

            // Assert
            Assert.AreEqual(0, saved);
            Assert.AreEqual(1, lines.Length);
            Assert.AreEqual(Program.CsvHeader, lines[0]);
        }

        [TestMethod]
        public void SaveToCsv_ExistingFile_IsOverwritten()
        {
            // Arrange
            string path = WriteCsv("старий вміст", "ще рядок", "і ще", "і ще", "і ще");

            // Act
            Program.SaveToCsv([_alpha], path);
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);

            // Assert
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual(_alpha.ToString(), lines[1]);
        }

        [TestMethod]
        public void SaveToCsv_DoesNotChangeList()
        {
            // Arrange
            List<string> before = ToStrings(_list);

            // Act
            Program.SaveToCsv(_list, FilePath("out.csv"));

            // Assert
            CollectionAssert.AreEqual(before, ToStrings(_list));
        }

        [TestMethod]
        public void SaveToCsv_NullList_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => Program.SaveToCsv(null!, FilePath("out.csv")));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void SaveToCsv_EmptyPath_ThrowsArgumentException(string? path)
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Program.SaveToCsv(_list, path!));
        }

        [TestMethod]
        public void SaveToCsv_ListWithNull_ThrowsAndFileNotCreated()
        {
            // Arrange
            string path = FilePath("out.csv");
            _list.Add(null!);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Program.SaveToCsv(_list, path));
            Assert.IsFalse(File.Exists(path));
        }

        #endregion

        #region LoadFromCsv

        [TestMethod]
        public void LoadFromCsv_AfterSave_RestoresSameObjects()
        {
            // Arrange
            string path = FilePath("roundtrip.csv");
            Program.SaveToCsv(_list, path);
            var target = new List<PizzaAsteroid>();

            // Act
            LoadResult result = Program.LoadFromCsv(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(3, result.Added);
            Assert.AreEqual(0, result.Invalid);
            Assert.AreEqual(0, result.OverCapacity);
            CollectionAssert.AreEqual(ToStrings(_list), ToStrings(target));
            Assert.AreNotSame(_alpha, target[0]);
        }

        [TestMethod]
        public void LoadFromCsv_NonEmptyList_AppendsToExistingObjects()
        {
            // Arrange
            string path = FilePath("data.csv");
            Program.SaveToCsv(_list, path);
            var existing = new PizzaAsteroid("Наявний", CrustType.Thin, 3.0);
            var target = new List<PizzaAsteroid> { existing };

            // Act
            LoadResult result = Program.LoadFromCsv(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(3, result.Added);
            Assert.AreEqual(4, target.Count);
            Assert.AreSame(existing, target[0]);
            Assert.AreEqual("Альфа", target[1].Name);
        }

        [TestMethod]
        [DataRow("Церера;Thin;5;10;ні")]                             // пропущене поле
        [DataRow("Церера;Thin;;10;ні;01.01.2000")]                   // порожній діаметр
        [DataRow(";Thin;5;10;ні;01.01.2000")]                        // порожня назва
        [DataRow("Церера;Thin;abc;10;ні;01.01.2000")]                // діаметр — не число
        [DataRow("Церера;Thin;5;12.5;ні;01.01.2000")]                // температура — не ціле
        [DataRow("Церера;Thin;5;тепло;ні;01.01.2000")]               // температура — текст
        [DataRow("Церера;Square;5;10;ні;01.01.2000")]                // невідомий бортик
        [DataRow("Церера;7;5;10;ні;01.01.2000")]                     // номер бортика поза межами
        [DataRow("Церера;Thin;5;10;може;01.01.2000")]                // сир — не bool
        [DataRow("Церера;Thin;5;10;ні;2000-01-01")]                  // невірний формат дати
        [DataRow("Церера;Thin;5;10;ні;31.02.2021")]                  // неіснуюча дата
        [DataRow("Церера;Thin;5000;10;ні;01.01.2000")]               // діаметр поза діапазоном
        [DataRow("Церера;Thin;5;-300;ні;01.01.2000")]                // температура поза діапазоном
        [DataRow("Церера;Thin;5;10;ні;01.01.1980")]                  // дата поза діапазоном
        [DataRow("@@@;Thin;5;10;ні;01.01.2000")]                     // спецсимволи в назві
        [DataRow("Церера;Thin;5;10;ні;01.01.2000;Песто;Зайве")]      // зайве поле
        [DataRow("Церера,Thin,5,10,ні,01.01.2000")]                  // інший роздільник
        public void LoadFromCsv_BrokenLine_IsSkipped(string brokenLine)
        {
            // Arrange
            string path = WriteCsv(Program.CsvHeader, _alpha.ToString(), brokenLine, _beta.ToString());
            var target = new List<PizzaAsteroid>();

            // Act
            LoadResult result = Program.LoadFromCsv(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(2, result.Added);
            Assert.AreEqual(1, result.Invalid);
            Assert.AreEqual(1, result.Errors.Count);
            StringAssert.StartsWith(result.Errors[0], "Рядок 3:");
            Assert.AreEqual(2, target.Count);
            Assert.AreEqual("Альфа", target[0].Name);
            Assert.AreEqual("Бета", target[1].Name);
        }

        [TestMethod]
        public void LoadFromCsv_MixedFile_ReportsCorrectCounts()
        {
            // Arrange
            string path = WriteCsv(
                Program.CsvHeader,
                "Церера;Thin;5;10;ні;01.01.2000;Песто",
                "Церера;Thin;abc;10;ні;01.01.2000",
                "",
                "Веста;Classic;7,5;-20;так;10.10.2010",
                "Паллада;Thin;5;10",
                "Гігея;DeepDish;5;10;ні;99.99.2000");
            var target = new List<PizzaAsteroid>();

            // Act
            LoadResult result = Program.LoadFromCsv(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(2, result.Added);
            Assert.AreEqual(3, result.Invalid);
            Assert.AreEqual(5, result.Total);
            Assert.AreEqual(3, result.Errors.Count);
            StringAssert.StartsWith(result.Errors[0], "Рядок 3:");
            StringAssert.Contains(result.Errors[0], "Діаметр");
            StringAssert.StartsWith(result.Errors[1], "Рядок 6:");
            StringAssert.StartsWith(result.Errors[2], "Рядок 7:");
            StringAssert.Contains(result.Errors[2], "Дата");
            Assert.AreEqual(7.5, target[1].DiameterKm, 1e-9);
            Assert.AreEqual(DefaultSauce, target[1].SauceType);
        }

        [TestMethod]
        public void LoadFromCsv_EmptyLines_AreIgnored()
        {
            // Arrange
            string path = WriteCsv("", Program.CsvHeader, "", _alpha.ToString(), "   ", "");

            // Act
            LoadResult result = Program.LoadFromCsv(new List<PizzaAsteroid>(), path, BigCapacity);

            // Assert
            Assert.AreEqual(1, result.Added);
            Assert.AreEqual(0, result.Invalid);
        }

        [TestMethod]
        public void LoadFromCsv_HeaderOnly_ReturnsZero()
        {
            // Arrange
            string path = WriteCsv(Program.CsvHeader);
            var target = new List<PizzaAsteroid>();

            // Act
            LoadResult result = Program.LoadFromCsv(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(0, target.Count);
        }

        [TestMethod]
        public void LoadFromCsv_EmptyFile_ReturnsZero()
        {
            // Arrange
            string path = WriteCsv();

            // Act
            LoadResult result = Program.LoadFromCsv(new List<PizzaAsteroid>(), path, BigCapacity);

            // Assert
            Assert.AreEqual(0, result.Total);
        }

        [TestMethod]
        public void LoadFromCsv_WithoutHeader_FirstLineIsParsedAsData()
        {
            // Arrange
            string path = WriteCsv(_alpha.ToString(), _beta.ToString());

            // Act
            LoadResult result = Program.LoadFromCsv(new List<PizzaAsteroid>(), path, BigCapacity);

            // Assert
            Assert.AreEqual(2, result.Added);
            Assert.AreEqual(0, result.Invalid);
        }

        [TestMethod]
        public void LoadFromCsv_HeaderInDifferentCaseAndSpaces_IsSkipped()
        {
            // Arrange
            string header = "name ; CRUST ; DiameterKm ; TemperatureCelsius ; HasExtraCheese ; DiscoveryDate ; SauceType";
            string path = WriteCsv(header, _alpha.ToString());

            // Act
            LoadResult result = Program.LoadFromCsv(new List<PizzaAsteroid>(), path, BigCapacity);

            // Assert
            Assert.AreEqual(1, result.Added);
            Assert.AreEqual(0, result.Invalid);
        }

        [TestMethod]
        public void LoadFromCsv_CapacityReached_RestAreCountedAsOverCapacity()
        {
            // Arrange — у _list 3 об'єкти, ліміт 4, у файлі 3 коректні рядки
            string path = WriteCsv(Program.CsvHeader, _alpha.ToString(), _beta.ToString(), _vega.ToString());

            // Act
            LoadResult result = Program.LoadFromCsv(_list, path, 4);

            // Assert
            Assert.AreEqual(1, result.Added);
            Assert.AreEqual(2, result.OverCapacity);
            Assert.AreEqual(4, _list.Count);
        }

        [TestMethod]
        public void LoadFromCsv_MissingFile_ThrowsFileNotFoundExceptionAndListUnchanged()
        {
            // Arrange
            string path = FilePath("absent.csv");

            // Act & Assert
            Assert.ThrowsExactly<FileNotFoundException>(() => Program.LoadFromCsv(_list, path, BigCapacity));
            Assert.AreEqual(3, _list.Count);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        public void LoadFromCsv_InvalidCapacity_ThrowsArgumentOutOfRangeException(int capacity)
        {
            // Arrange
            string path = WriteCsv(Program.CsvHeader);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Program.LoadFromCsv(_list, path, capacity));
        }

        [TestMethod]
        public void LoadFromCsv_NullList_ThrowsArgumentNullException()
        {
            // Arrange
            string path = WriteCsv(Program.CsvHeader);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => Program.LoadFromCsv(null!, path, BigCapacity));
        }

        #endregion

        #region SaveToJson

        [TestMethod]
        public void SaveToJson_ThreeObjects_WritesJsonArrayOfThree()
        {
            // Arrange
            string path = FilePath("out.json");

            // Act
            int saved = Program.SaveToJson(_list, path);
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));

            // Assert
            Assert.AreEqual(3, saved);
            Assert.AreEqual(JsonValueKind.Array, doc.RootElement.ValueKind);
            Assert.AreEqual(3, doc.RootElement.GetArrayLength());
        }

        [TestMethod]
        public void SaveToJson_UsesRenamedPropertiesAndOnlyMainOnes()
        {
            // Arrange
            string path = FilePath("out.json");
            var expected = new List<string>
                { "name", "crust", "diameter_km", "temperature_c", "extra_cheese", "discovery_date", "sauce" };

            // Act
            Program.SaveToJson(_list, path);
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
            List<string> actual = doc.RootElement[0].EnumerateObject().Select(p => p.Name).ToList();

            // Assert — порядок задано JsonPropertyOrder, EstimatedVolumeKm3 і static-властивості відсутні
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void SaveToJson_WritesExpectedValues()
        {
            // Arrange
            string path = FilePath("out.json");

            // Act
            Program.SaveToJson(_list, path);
            using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(path, Encoding.UTF8));
            JsonElement first = doc.RootElement[0];

            // Assert
            Assert.AreEqual("Альфа", first.GetProperty("name").GetString());
            Assert.AreEqual("Thin", first.GetProperty("crust").GetString());
            Assert.AreEqual(10.0, first.GetProperty("diameter_km").GetDouble(), 1e-9);
            Assert.AreEqual(-50, first.GetProperty("temperature_c").GetInt32());
            Assert.IsTrue(first.GetProperty("extra_cheese").GetBoolean());
            Assert.AreEqual("2020-01-15T00:00:00", first.GetProperty("discovery_date").GetString());
            Assert.AreEqual("Барбекю", first.GetProperty("sauce").GetString());
        }

        [TestMethod]
        public void SaveToJson_CyrillicIsNotEscaped()
        {
            // Arrange
            string path = FilePath("out.json");

            // Act
            Program.SaveToJson(_list, path);
            string text = File.ReadAllText(path, Encoding.UTF8);

            // Assert
            StringAssert.Contains(text, "\"Альфа\"");
            StringAssert.Contains(text, "\"Часниковий\"");
            Assert.IsFalse(text.Contains("\\u04"), "Кирилиця не повинна екрануватися як \\uXXXX.");
        }

        [TestMethod]
        public void SaveToJson_EmptyList_WritesEmptyArray()
        {
            // Arrange
            string path = FilePath("empty.json");

            // Act
            int saved = Program.SaveToJson([], path);

            // Assert
            Assert.AreEqual(0, saved);
            Assert.AreEqual("[]", File.ReadAllText(path, Encoding.UTF8).Trim());
        }

        [TestMethod]
        public void SaveToJson_NullList_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentNullException>(() => Program.SaveToJson(null!, FilePath("out.json")));
        }

        [TestMethod]
        public void SaveToJson_ListWithNull_ThrowsAndFileNotCreated()
        {
            // Arrange
            string path = FilePath("out.json");
            _list.Add(null!);

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Program.SaveToJson(_list, path));
            Assert.IsFalse(File.Exists(path));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        public void SaveToJson_EmptyPath_ThrowsArgumentException(string path)
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Program.SaveToJson(_list, path));
        }

        #endregion

        #region LoadFromJson

        [TestMethod]
        public void LoadFromJson_AfterSave_RestoresSameObjects()
        {
            // Arrange
            string path = FilePath("roundtrip.json");
            Program.SaveToJson(_list, path);
            var target = new List<PizzaAsteroid>();

            // Act
            LoadResult result = Program.LoadFromJson(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(3, result.Added);
            Assert.AreEqual(0, result.Invalid);
            CollectionAssert.AreEqual(ToStrings(_list), ToStrings(target));
            Assert.AreEqual(new DateTime(2020, 1, 15), target[0].DiscoveryDate);   // private set через [JsonInclude]
            Assert.AreNotSame(_alpha, target[0]);
        }

        [TestMethod]
        public void LoadFromJson_NonEmptyList_AppendsToExistingObjects()
        {
            // Arrange
            string path = FilePath("data.json");
            Program.SaveToJson(_list, path);
            var existing = new PizzaAsteroid("Наявний", CrustType.Thin, 3.0);
            var target = new List<PizzaAsteroid> { existing };

            // Act
            LoadResult result = Program.LoadFromJson(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(3, result.Added);
            Assert.AreEqual(4, target.Count);
            Assert.AreSame(existing, target[0]);
            Assert.AreEqual("Вега", target[3].Name);
        }

        [TestMethod]
        public void LoadFromJson_EmptyArray_ReturnsZero()
        {
            // Arrange
            string path = WriteJson("[]");

            // Act
            LoadResult result = Program.LoadFromJson(_list, path, BigCapacity);

            // Assert
            Assert.AreEqual(0, result.Total);
            Assert.AreEqual(3, _list.Count);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("[")]
        [DataRow("не json")]
        [DataRow("{\"name\":\"Церера\"}")]
        [DataRow("[1, 2, 3]")]
        [DataRow("[{\"name\":\"Церера\",\"crust\":\"Thin\",\"diameter_km\":5,}]")]
        public void LoadFromJson_MalformedJson_ThrowsFormatExceptionAndListUnchanged(string content)
        {
            // Arrange
            string path = WriteJson(content);

            // Act & Assert
            Assert.ThrowsExactly<FormatException>(() => Program.LoadFromJson(_list, path, BigCapacity));
            Assert.AreEqual(3, _list.Count);
        }

        [TestMethod]
        public void LoadFromJson_RootNull_ThrowsFormatException()
        {
            // Arrange
            string path = WriteJson("null");

            // Act
            var ex = Assert.ThrowsExactly<FormatException>(() => Program.LoadFromJson(_list, path, BigCapacity));

            // Assert
            StringAssert.Contains(ex.Message, "null");
            Assert.AreEqual(3, _list.Count);
        }

        [TestMethod]
        [DataRow("diameter_km", "5000")]
        [DataRow("diameter_km", "\"abc\"")]
        [DataRow("temperature_c", "-300")]
        [DataRow("temperature_c", "12.5")]
        [DataRow("name", "\"@@@\"")]
        [DataRow("name", "\"\"")]
        [DataRow("crust", "99")]
        [DataRow("crust", "\"Square\"")]
        [DataRow("extra_cheese", "\"може\"")]
        [DataRow("discovery_date", "\"2099-01-01T00:00:00\"")]
        [DataRow("discovery_date", "\"12.04.2021\"")]
        [DataRow("sauce", "\"Сир;Часник\"")]
        public void LoadFromJson_OneBadObject_WholeCollectionRejected(string property, string rawValue)
        {
            // Arrange — перший елемент коректний, другий містить «бите» значення
            JsonObject bad = ValidJsonElement();
            bad[property] = JsonNode.Parse(rawValue);
            string path = WriteJson(JsonArrayOf(ValidJsonElement(), bad));

            // Act & Assert
            Assert.ThrowsExactly<FormatException>(() => Program.LoadFromJson(_list, path, BigCapacity));
            Assert.AreEqual(3, _list.Count);
        }

        [TestMethod]
        [DataRow("name")]
        [DataRow("crust")]
        [DataRow("diameter_km")]
        public void LoadFromJson_MissingRequiredProperty_WholeCollectionRejected(string property)
        {
            // Arrange
            JsonObject bad = ValidJsonElement();
            bad.Remove(property);
            string path = WriteJson(JsonArrayOf(bad));

            // Act & Assert
            Assert.ThrowsExactly<FormatException>(() => Program.LoadFromJson(_list, path, BigCapacity));
            Assert.AreEqual(3, _list.Count);
        }

        [TestMethod]
        public void LoadFromJson_MissingOptionalProperties_UsesDefaults()
        {
            // Arrange
            JsonObject element = ValidJsonElement();
            element.Remove("sauce");
            element.Remove("extra_cheese");
            string path = WriteJson(JsonArrayOf(element));
            var target = new List<PizzaAsteroid>();

            // Act
            LoadResult result = Program.LoadFromJson(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(1, result.Added);
            Assert.AreEqual(DefaultSauce, target[0].SauceType);
            Assert.IsFalse(target[0].HasExtraCheese);
        }

        [TestMethod]
        [DataRow("3", CrustType.DeepDish)]
        [DataRow("\"classic\"", CrustType.Classic)]
        public void LoadFromJson_CrustAsNumberOrAnyCase_IsAccepted(string rawCrust, CrustType expected)
        {
            // Arrange
            JsonObject element = ValidJsonElement();
            element["crust"] = JsonNode.Parse(rawCrust);
            string path = WriteJson(JsonArrayOf(element));
            var target = new List<PizzaAsteroid>();

            // Act
            Program.LoadFromJson(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(expected, target[0].Crust);
        }

        [TestMethod]
        public void LoadFromJson_PropertyNamesInOtherCase_AreRecognized()
        {
            // Arrange
            string json = "[{\"NAME\":\"Церера\",\"Crust\":\"Thin\",\"Diameter_KM\":5,\"TEMPERATURE_C\":10}]";
            string path = WriteJson(json);
            var target = new List<PizzaAsteroid>();

            // Act
            Program.LoadFromJson(target, path, BigCapacity);

            // Assert
            Assert.AreEqual("Церера", target[0].Name);
            Assert.AreEqual(5.0, target[0].DiameterKm, 1e-9);
            Assert.AreEqual(10, target[0].TemperatureCelsius);
        }

        [TestMethod]
        public void LoadFromJson_UnknownProperty_IsIgnored()
        {
            // Arrange
            JsonObject element = ValidJsonElement();
            element["EstimatedVolumeKm3"] = 123.45;
            string path = WriteJson(JsonArrayOf(element));

            // Act
            LoadResult result = Program.LoadFromJson(new List<PizzaAsteroid>(), path, BigCapacity);

            // Assert
            Assert.AreEqual(1, result.Added);
        }

        [TestMethod]
        public void LoadFromJson_NullElement_IsSkipped()
        {
            // Arrange
            string path = WriteJson(JsonArrayOf(ValidJsonElement(), null));
            var target = new List<PizzaAsteroid>();

            // Act
            LoadResult result = Program.LoadFromJson(target, path, BigCapacity);

            // Assert
            Assert.AreEqual(1, result.Added);
            Assert.AreEqual(1, result.Invalid);
            StringAssert.StartsWith(result.Errors[0], "Елемент #2");
            Assert.AreEqual(1, target.Count);
        }

        [TestMethod]
        public void LoadFromJson_CapacityReached_RestAreCountedAsOverCapacity()
        {
            // Arrange — у _list 3 об'єкти, ліміт 4
            string path = WriteJson(JsonArrayOf(ValidJsonElement(), ValidJsonElement(), ValidJsonElement()));

            // Act
            LoadResult result = Program.LoadFromJson(_list, path, 4);

            // Assert
            Assert.AreEqual(1, result.Added);
            Assert.AreEqual(2, result.OverCapacity);
            Assert.AreEqual(4, _list.Count);
        }

        [TestMethod]
        public void LoadFromJson_MissingFile_ThrowsFileNotFoundException()
        {
            // Act & Assert
            Assert.ThrowsExactly<FileNotFoundException>(
                () => Program.LoadFromJson(_list, FilePath("absent.json"), BigCapacity));
            Assert.AreEqual(3, _list.Count);
        }

        [TestMethod]
        public void LoadFromJson_InvalidCapacity_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            string path = WriteJson("[]");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => Program.LoadFromJson(_list, path, 0));
        }

        #endregion

        #region CSV ↔ JSON

        [TestMethod]
        public void CsvAndJson_SameCollection_GiveSameObjects()
        {
            // Arrange
            string csv = FilePath("same.csv");
            string json = FilePath("same.json");
            Program.SaveToCsv(_list, csv);
            Program.SaveToJson(_list, json);
            var fromCsv = new List<PizzaAsteroid>();
            var fromJson = new List<PizzaAsteroid>();

            // Act
            Program.LoadFromCsv(fromCsv, csv, BigCapacity);
            Program.LoadFromJson(fromJson, json, BigCapacity);

            // Assert
            CollectionAssert.AreEqual(ToStrings(fromCsv), ToStrings(fromJson));
        }

        #endregion
    }
}