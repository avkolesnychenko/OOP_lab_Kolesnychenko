using PizzaAsteroidApp;

namespace PizzaAsteroidApp.Tests
{
    [TestClass]
    public sealed class ProgramTests
    {
        private List<PizzaAsteroid> _list = null!;
        private PizzaAsteroid _alpha = null!;
        private PizzaAsteroid _beta = null!;
        private PizzaAsteroid _vega = null!;
        private double _savedFactor;

        private const string ValidLine = "Пепероні-X;Thin;12.5;-40;так;12.04.2021;Барбекю";

        [TestInitialize]
        public void Setup()
        {
            _savedFactor = PizzaAsteroid.ImpactEnergyFactor;

            _alpha = new PizzaAsteroid("Альфа", CrustType.Thin, 10.0, -50, true, new DateTime(2020, 1, 15), "Барбекю");
            _beta = new PizzaAsteroid("Бета", CrustType.Classic, 20.0, 0, false, new DateTime(2015, 6, 1));
            _vega = new PizzaAsteroid("Вега", CrustType.Thin, 5.5, 100, false, new DateTime(2010, 3, 3), "Часниковий");

            _list = [_alpha, _beta, _vega];
        }

        [TestCleanup]
        public void Cleanup()
        {
            PizzaAsteroid.ImpactEnergyFactor = _savedFactor;
            _list.Clear();
        }

        #region IsFull

        [TestMethod]
        [DataRow(3, true)]
        [DataRow(4, false)]
        [DataRow(1, true)]
        public void IsFull_VariousCapacity_ReturnsExpected(int capacity, bool expected)
        {
            // Arrange — список із 3 елементів (Setup)

            // Act
            bool result = Program.IsFull(_list, capacity);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(-5)]
        public void IsFull_NonPositiveCapacity_ThrowsArgumentOutOfRangeException(int capacity)
        {
            // Arrange — capacity з DataRow

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.IsFull(_list, capacity));
        }

        [TestMethod]
        public void IsFull_NullList_ThrowsArgumentNullException()
        {
            // Arrange
            List<PizzaAsteroid> list = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Program.IsFull(list, 5));
        }

        #endregion

        #region TryAdd

        [TestMethod]
        public void TryAdd_ListNotFull_AddsToEndAndReturnsTrue()
        {
            // Arrange
            var asteroid = new PizzaAsteroid("Дельта", CrustType.DeepDish, 3.0);

            // Act
            bool result = Program.TryAdd(_list, asteroid, 10);

            // Assert
            Assert.IsTrue(result);
            Assert.HasCount(4, _list);
            Assert.AreSame(asteroid, _list[^1]);
        }

        [TestMethod]
        public void TryAdd_ListFull_ReturnsFalseAndListUnchanged()
        {
            // Arrange
            var asteroid = new PizzaAsteroid("Дельта", CrustType.DeepDish, 3.0);

            // Act
            bool result = Program.TryAdd(_list, asteroid, 3);

            // Assert
            Assert.IsFalse(result);
            Assert.HasCount(3, _list);
            CollectionAssert.DoesNotContain(_list, asteroid);
        }

        [TestMethod]
        public void TryAdd_SameObjectTwice_ThrowsArgumentException()
        {
            // Arrange — _alpha вже у списку

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.TryAdd(_list, _alpha, 10));
            Assert.HasCount(3, _list);
        }

        [TestMethod]
        public void TryAdd_NullAsteroid_ThrowsArgumentNullException()
        {
            // Arrange
            PizzaAsteroid asteroid = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Program.TryAdd(_list, asteroid, 10));
        }

        [TestMethod]
        public void TryAdd_EmptyListCapacityOne_AddsSingleItem()
        {
            // Arrange
            var empty = new List<PizzaAsteroid>();

            // Act
            bool first = Program.TryAdd(empty, _alpha, 1);
            bool second = Program.TryAdd(empty, _beta, 1);

            // Assert
            Assert.IsTrue(first);
            Assert.IsFalse(second);
            Assert.HasCount(1, empty);
        }

        #endregion

        #region TryAddFromString

        [TestMethod]
        public void TryAddFromString_ValidLine_AddsParsedObject()
        {
            // Arrange — ValidLine

            // Act
            bool result = Program.TryAddFromString(_list, ValidLine, 10, out PizzaAsteroid? added);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(added);
            Assert.AreEqual("Пепероні-X", added.Name);
            Assert.AreEqual(CrustType.Thin, added.Crust);
            Assert.AreSame(added, _list[^1]);
            Assert.HasCount(4, _list);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("Пепероні-X;Thin;12.5")]
        [DataRow("Пепероні-X;Pan;12.5;-40;так;12.04.2021")]
        [DataRow("Пепероні-X;Thin;abc;-40;так;12.04.2021")]
        [DataRow("Пепероні-X;Thin;5000;-40;так;12.04.2021")]
        [DataRow("Пепероні-X;Thin;12.5;-40;може;12.04.2021")]
        [DataRow("Пепероні-X;Thin;12.5;-40;так;2021-04-12")]
        [DataRow("@@@;Thin;12.5;-40;так;12.04.2021")]
        public void TryAddFromString_InvalidLine_ReturnsFalseAndListUnchanged(string input)
        {
            // Arrange — input з DataRow

            // Act
            bool result = Program.TryAddFromString(_list, input, 10, out PizzaAsteroid? added);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(added);
            Assert.HasCount(3, _list);
        }

        [TestMethod]
        public void TryAddFromString_NullInput_ReturnsFalse()
        {
            // Arrange
            string? input = null;

            // Act
            bool result = Program.TryAddFromString(_list, input, 10, out PizzaAsteroid? added);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(added);
        }

        [TestMethod]
        public void TryAddFromString_ListFull_ReturnsFalseWithoutCreatingObject()
        {
            // Arrange
            int countBefore = PizzaAsteroid.CreatedCount;

            // Act
            bool result = Program.TryAddFromString(_list, ValidLine, 3, out PizzaAsteroid? added);

            // Assert
            Assert.IsFalse(result);
            Assert.IsNull(added);
            Assert.AreEqual(countBefore, PizzaAsteroid.CreatedCount);
            Assert.HasCount(3, _list);
        }

        #endregion

        #region CreateDefault / CreateRandom

        [TestMethod]
        public void CreateDefault_ReturnsObjectWithExpectedValues()
        {
            // Arrange — немає вхідних даних

            // Act
            PizzaAsteroid result = Program.CreateDefault();

            // Assert
            Assert.AreEqual("Класичний-Базовий", result.Name);
            Assert.AreEqual(CrustType.CheeseStuffed, result.Crust);
            Assert.AreEqual("Часниковий", result.SauceType);
            Assert.AreEqual(50.0, result.DiameterKm);
            Assert.AreEqual(25, result.TemperatureCelsius);
            Assert.IsTrue(result.HasExtraCheese);
        }

        [TestMethod]
        public void CreateRandom_ManySeeds_AlwaysReturnsValidObject()
        {
            // Arrange
            var rand = new Random(12345);
            var usedConstructors = new HashSet<int>();

            for (int i = 0; i < 200; i++)
            {
                // Act
                PizzaAsteroid a = Program.CreateRandom(rand, out int ctor);
                usedConstructors.Add(ctor);

                // Assert
                Assert.IsTrue(ctor >= 1 && ctor <= 4);
                Assert.IsTrue(a.DiameterKm >= 1.0 && a.DiameterKm <= 96.0);
                Assert.IsTrue(Enum.IsDefined(typeof(CrustType), a.Crust));
                Assert.IsTrue(a.Name.Length >= 3 && a.Name.Length <= 20);
                Assert.IsTrue(a.DiscoveryDate <= DateTime.Today);
            }
            Assert.HasCount(4, usedConstructors, "За 200 спроб мали бути використані всі 4 конструктори.");
        }

        [TestMethod]
        public void CreateRandom_SameSeed_ReturnsSameData()
        {
            // Arrange
            var r1 = new Random(7);
            var r2 = new Random(7);

            // Act
            PizzaAsteroid a = Program.CreateRandom(r1, out int c1);
            PizzaAsteroid b = Program.CreateRandom(r2, out int c2);

            // Assert
            Assert.AreEqual(c1, c2);
            Assert.AreEqual(a.ToString(), b.ToString());
            Assert.AreNotSame(a, b);
        }

        [TestMethod]
        public void CreateRandom_NullRandom_ThrowsArgumentNullException()
        {
            // Arrange
            Random rand = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Program.CreateRandom(rand, out _));
        }

        [TestMethod]
        [DataRow(1, "без параметрів")]
        [DataRow(2, "скорочений")]
        [DataRow(3, "5-параметричний")]
        [DataRow(4, "7-параметричний")]
        public void GetConstructorDescription_ValidIndex_ContainsKeyword(int index, string keyword)
        {
            // Arrange — index, keyword з DataRow

            // Act
            string result = Program.GetConstructorDescription(index);

            // Assert
            StringAssert.Contains(result, keyword);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(5)]
        [DataRow(-1)]
        public void GetConstructorDescription_InvalidIndex_ThrowsArgumentOutOfRangeException(int index)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.GetConstructorDescription(index));
        }

        #endregion

        #region GetByNumber

        [TestMethod]
        [DataRow(1, "Альфа")]
        [DataRow(2, "Бета")]
        [DataRow(3, "Вега")]
        public void GetByNumber_ValidNumber_ReturnsCorrectObject(int number, string expectedName)
        {
            // Act
            PizzaAsteroid result = Program.GetByNumber(_list, number);

            // Assert
            Assert.AreEqual(expectedName, result.Name);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(4)]
        [DataRow(-1)]
        [DataRow(int.MaxValue)]
        public void GetByNumber_OutOfRange_ThrowsArgumentOutOfRangeException(int number)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.GetByNumber(_list, number));
        }

        #endregion

        #region FindByCrust

        [TestMethod]
        [DataRow(CrustType.Thin, 2)]
        [DataRow(CrustType.Classic, 1)]
        [DataRow(CrustType.DeepDish, 0)]
        [DataRow(CrustType.CheeseStuffed, 0)]
        public void FindByCrust_ExistingCrust_ReturnsMatchingCount(CrustType crust, int expectedCount)
        {
            // Act
            List<PizzaAsteroid> result = Program.FindByCrust(_list, crust);

            // Assert
            Assert.HasCount(expectedCount, result);
            Assert.IsTrue(result.TrueForAll(a => a.Crust == crust));
        }

        [TestMethod]
        public void FindByCrust_Thin_ReturnsObjectsInOriginalOrder()
        {
            // Arrange
            var expected = new List<PizzaAsteroid> { _alpha, _vega };

            // Act
            List<PizzaAsteroid> result = Program.FindByCrust(_list, CrustType.Thin);

            // Assert
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(5)]
        [DataRow(-3)]
        public void FindByCrust_UndefinedEnumValue_ThrowsArgumentException(int raw)
        {
            // Arrange
            CrustType crust = (CrustType)raw;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.FindByCrust(_list, crust));
        }

        [TestMethod]
        public void FindByCrust_EmptyList_ReturnsEmptyList()
        {
            // Arrange
            var empty = new List<PizzaAsteroid>();

            // Act
            List<PizzaAsteroid> result = Program.FindByCrust(empty, CrustType.Thin);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        #endregion

        #region FindByCheese

        [TestMethod]
        [DataRow(true, 1)]
        [DataRow(false, 2)]
        public void FindByCheese_ReturnsMatchingCount(bool cheese, int expectedCount)
        {
            // Act
            List<PizzaAsteroid> result = Program.FindByCheese(_list, cheese);

            // Assert
            Assert.HasCount(expectedCount, result);
            Assert.IsTrue(result.TrueForAll(a => a.HasExtraCheese == cheese));
        }

        [TestMethod]
        public void FindByCheese_ResultIsNewList_OriginalNotModified()
        {
            // Act
            List<PizzaAsteroid> result = Program.FindByCheese(_list, true);
            result.Clear();

            // Assert
            Assert.AreNotSame(_list, result);
            Assert.HasCount(3, _list);
        }

        #endregion

        #region FindByName / FindIndexByName / ExistsWithName

        [TestMethod]
        [DataRow("Бета")]
        [DataRow("бета")]
        [DataRow("  БЕТА  ")]
        public void FindByName_ExistingNameAnyCaseOrSpaces_ReturnsObject(string name)
        {
            // Act
            PizzaAsteroid? result = Program.FindByName(_list, name);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreSame(_beta, result);
        }

        [TestMethod]
        public void FindByName_MissingName_ReturnsNull()
        {
            // Act
            PizzaAsteroid? result = Program.FindByName(_list, "Омега");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindByName_PartialName_ReturnsNull()
        {
            // Act
            PizzaAsteroid? result = Program.FindByName(_list, "Бе");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow(null)]
        public void FindByName_EmptyName_ThrowsArgumentException(string? name)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.FindByName(_list, name!));
        }

        [TestMethod]
        public void FindByName_DuplicateNames_ReturnsFirst()
        {
            // Arrange
            var duplicate = new PizzaAsteroid("Альфа", CrustType.DeepDish, 1.0);
            _list.Add(duplicate);

            // Act
            PizzaAsteroid? result = Program.FindByName(_list, "Альфа");

            // Assert
            Assert.AreSame(_alpha, result);
        }

        [TestMethod]
        [DataRow("Альфа", 0)]
        [DataRow("Вега", 2)]
        [DataRow("Омега", -1)]
        public void FindIndexByName_ReturnsExpectedIndex(string name, int expected)
        {
            // Act
            int result = Program.FindIndexByName(_list, name);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void FindIndexByName_EmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.FindIndexByName(_list, " "));
        }

        [TestMethod]
        [DataRow("вега", true)]
        [DataRow("Омега", false)]
        public void ExistsWithName_ReturnsExpected(string name, bool expected)
        {
            // Act
            bool result = Program.ExistsWithName(_list, name);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ExistsWithName_EmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.ExistsWithName(_list, ""));
        }

        #endregion

        #region FindByDiameterRange

        [TestMethod]
        [DataRow(0.0, 1000.0, 3)]
        [DataRow(5.5, 10.0, 2)]
        [DataRow(10.0, 10.0, 1)]
        [DataRow(10.01, 19.99, 0)]
        [DataRow(20.0, 100.0, 1)]
        public void FindByDiameterRange_ValidRange_ReturnsInclusiveMatches(double min, double max, int expectedCount)
        {
            // Act
            List<PizzaAsteroid> result = Program.FindByDiameterRange(_list, min, max);

            // Assert
            Assert.HasCount(expectedCount, result);
            Assert.IsTrue(result.TrueForAll(a => a.DiameterKm >= min && a.DiameterKm <= max));
        }

        [TestMethod]
        public void FindByDiameterRange_MinGreaterThanMax_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.FindByDiameterRange(_list, 50.0, 10.0));
        }

        [TestMethod]
        [DataRow(-1.0, 10.0)]
        [DataRow(1.0, -10.0)]
        [DataRow(double.NaN, 10.0)]
        [DataRow(1.0, double.NaN)]
        public void FindByDiameterRange_NegativeOrNaN_ThrowsArgumentOutOfRangeException(double min, double max)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.FindByDiameterRange(_list, min, max));
        }

        #endregion

        #region RemoveAt

        [TestMethod]
        public void RemoveAt_MiddleNumber_RemovesAndShiftsElements()
        {
            // Act
            PizzaAsteroid removed = Program.RemoveAt(_list, 2);

            // Assert
            Assert.AreSame(_beta, removed);
            Assert.HasCount(2, _list);
            Assert.AreSame(_vega, _list[1]);
            CollectionAssert.DoesNotContain(_list, _beta);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(3)]
        public void RemoveAt_BoundaryNumbers_RemovesElement(int number)
        {
            // Arrange
            PizzaAsteroid expected = _list[number - 1];

            // Act
            PizzaAsteroid removed = Program.RemoveAt(_list, number);

            // Assert
            Assert.AreSame(expected, removed);
            Assert.HasCount(2, _list);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(4)]
        [DataRow(-10)]
        public void RemoveAt_InvalidNumber_ThrowsAndListUnchanged(int number)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.RemoveAt(_list, number));
            Assert.HasCount(3, _list);
        }

        [TestMethod]
        public void RemoveAt_EmptyList_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var empty = new List<PizzaAsteroid>();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.RemoveAt(empty, 1));
        }

        #endregion

        #region Remove

        [TestMethod]
        public void Remove_ExistingObject_ReturnsTrue()
        {
            // Act
            bool result = Program.Remove(_list, _vega);

            // Assert
            Assert.IsTrue(result);
            Assert.HasCount(2, _list);
            CollectionAssert.DoesNotContain(_list, _vega);
        }

        [TestMethod]
        public void Remove_ObjectWithSameDataButDifferentReference_ReturnsFalse()
        {
            // Arrange — копія з такими самими даними, але інше посилання
            PizzaAsteroid copy = PizzaAsteroid.Parse(_alpha.ToString());

            // Act
            bool result = Program.Remove(_list, copy);

            // Assert
            Assert.IsFalse(result);
            Assert.HasCount(3, _list);
        }

        [TestMethod]
        public void Remove_NullObject_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Program.Remove(_list, null!));
        }

        #endregion

        #region RemoveByName

        [TestMethod]
        public void RemoveByName_SeveralMatches_RemovesAllAndReturnsCount()
        {
            // Arrange
            _list.Add(new PizzaAsteroid("Альфа", CrustType.Classic, 2.0));
            _list.Add(new PizzaAsteroid("АЛЬФА", CrustType.Classic, 3.0));

            // Act
            int removed = Program.RemoveByName(_list, "альфа");

            // Assert
            Assert.AreEqual(3, removed);
            Assert.HasCount(2, _list);
            Assert.IsFalse(_list.Exists(a => a.Name.Equals("Альфа", StringComparison.OrdinalIgnoreCase)));
        }

        [TestMethod]
        public void RemoveByName_NoMatches_ReturnsZero()
        {
            // Act
            int removed = Program.RemoveByName(_list, "Омега");

            // Assert
            Assert.AreEqual(0, removed);
            Assert.HasCount(3, _list);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("  ")]
        [DataRow(null)]
        public void RemoveByName_EmptyName_ThrowsArgumentException(string? name)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.RemoveByName(_list, name!));
            Assert.HasCount(3, _list);
        }

        #endregion

        #region Clear

        [TestMethod]
        public void Clear_NonEmptyList_ReturnsCountAndEmptiesList()
        {
            // Act
            int removed = Program.Clear(_list);

            // Assert
            Assert.AreEqual(3, removed);
            Assert.IsEmpty(_list);
        }

        [TestMethod]
        public void Clear_EmptyList_ReturnsZero()
        {
            // Arrange
            var empty = new List<PizzaAsteroid>();

            // Act
            int removed = Program.Clear(empty);

            // Assert
            Assert.AreEqual(0, removed);
        }

        #endregion

        #region Sort

        [TestMethod]
        public void SortByDiameter_UnsortedList_SortsAscending()
        {
            // Arrange
            var expected = new List<PizzaAsteroid> { _vega, _alpha, _beta };

            // Act
            Program.SortByDiameter(_list);

            // Assert
            CollectionAssert.AreEqual(expected, _list);
        }

        [TestMethod]
        public void SortByName_ReversedList_SortsAlphabetically()
        {
            // Arrange
            _list.Reverse();
            var expected = new List<PizzaAsteroid> { _alpha, _beta, _vega };

            // Act
            Program.SortByName(_list);

            // Assert
            CollectionAssert.AreEqual(expected, _list);
        }

        [TestMethod]
        public void SortByName_EmptyList_DoesNotThrow()
        {
            // Arrange
            var empty = new List<PizzaAsteroid>();

            // Act
            Program.SortByName(empty);

            // Assert
            Assert.IsEmpty(empty);
        }

        [TestMethod]
        public void SortByDiameter_NullList_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Program.SortByDiameter(null!));
        }

        #endregion

        #region Merge / ReplaceWithMerged

        [TestMethod]
        public void Merge_TwoDifferentNumbers_ReturnsMergedWithoutChangingList()
        {
            // Act
            PizzaAsteroid merged = Program.Merge(_list, 1, 2, "Альфа-Бета");

            // Assert
            Assert.AreEqual("Альфа-Бета", merged.Name);
            Assert.AreEqual(20.8, merged.DiameterKm, 0.01);
            Assert.AreEqual(CrustType.Classic, merged.Crust);
            Assert.IsTrue(merged.HasExtraCheese);
            Assert.HasCount(3, _list);
            CollectionAssert.DoesNotContain(_list, merged);
        }

        [TestMethod]
        public void Merge_SameNumber_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.Merge(_list, 2, 2, "Подвійна"));
        }

        [TestMethod]
        [DataRow(0, 1)]
        [DataRow(1, 4)]
        public void Merge_InvalidNumber_ThrowsArgumentOutOfRangeException(int n1, int n2)
        {
            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => Program.Merge(_list, n1, n2, "Злиття"));
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("Аб")]
        [DataRow("Назва;з;роздільником")]
        public void Merge_InvalidNewName_ThrowsArgumentException(string newName)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.Merge(_list, 1, 2, newName));
        }

        [TestMethod]
        public void ReplaceWithMerged_ValidObjects_ReplacesTwoWithOneAtFirstPosition()
        {
            // Arrange
            PizzaAsteroid merged = Program.Merge(_list, 2, 3, "Бета-Вега");

            // Act
            Program.ReplaceWithMerged(_list, _beta, _vega, merged);

            // Assert
            Assert.HasCount(2, _list);
            Assert.AreSame(_alpha, _list[0]);
            Assert.AreSame(merged, _list[1]);
            CollectionAssert.DoesNotContain(_list, _beta);
            CollectionAssert.DoesNotContain(_list, _vega);
        }

        [TestMethod]
        public void ReplaceWithMerged_FirstObjectNotInList_ThrowsAndListUnchanged()
        {
            // Arrange — перший вихідний об'єкт відсутній у колекції
            var outsider = new PizzaAsteroid("Чужий", CrustType.Thin, 4.0);
            PizzaAsteroid merged = PizzaAsteroid.Merge(outsider, _beta, "Гібрид");

            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => Program.ReplaceWithMerged(_list, outsider, _beta, merged));
            Assert.HasCount(3, _list);
            Assert.AreSame(_beta, _list[1]);
        }

        [TestMethod]
        public void ReplaceWithMerged_ReversedOrder_InsertsAtSmallerIndex()
        {
            // Arrange
            PizzaAsteroid merged = Program.Merge(_list, 3, 1, "Вега-Альфа");

            // Act
            Program.ReplaceWithMerged(_list, _vega, _alpha, merged);

            // Assert
            Assert.AreSame(merged, _list[0]);
            Assert.AreSame(_beta, _list[1]);
        }

        [TestMethod]
        public void ReplaceWithMerged_ObjectNotInList_ThrowsAndListUnchanged()
        {
            // Arrange
            var outsider = new PizzaAsteroid("Чужий", CrustType.Thin, 4.0);
            PizzaAsteroid merged = PizzaAsteroid.Merge(_alpha, outsider, "Гібрид");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.ReplaceWithMerged(_list, _alpha, outsider, merged));
            Assert.HasCount(3, _list);
        }

        [TestMethod]
        public void ReplaceWithMerged_SameSourceObjects_ThrowsArgumentException()
        {
            // Arrange
            var merged = new PizzaAsteroid("Новий", CrustType.Thin, 4.0);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.ReplaceWithMerged(_list, _alpha, _alpha, merged));
        }

        [TestMethod]
        public void ReplaceWithMerged_MergedAlreadyInList_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.ReplaceWithMerged(_list, _alpha, _beta, _vega));
        }

        [TestMethod]
        public void ReplaceWithMerged_NullMerged_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Program.ReplaceWithMerged(_list, _alpha, _beta, null!));
        }

        #endregion

        #region CalculateTotalEnergy

        [TestMethod]
        public void CalculateTotalEnergy_FactorTwo_ReturnsDoubledVolume()
        {
            // Arrange — сумарний об'єм 523.6 + 4188.79 + 87.11 = 4799.5 км³
            PizzaAsteroid.ImpactEnergyFactor = 2.0;

            // Act
            double result = Program.CalculateTotalEnergy(_list);

            // Assert
            Assert.AreEqual(9599.0, result, 0.001);
        }

        [TestMethod]
        public void CalculateTotalEnergy_EmptyList_ReturnsZero()
        {
            // Act
            double result = Program.CalculateTotalEnergy(new List<PizzaAsteroid>());

            // Assert
            Assert.AreEqual(0.0, result);
        }

        [TestMethod]
        public void CalculateTotalEnergy_ListWithNull_ThrowsArgumentException()
        {
            // Arrange
            _list.Add(null!);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.CalculateTotalEnergy(_list));
        }

        #endregion

        #region GetToStringLines / IsRoundTripCompatible

        [TestMethod]
        public void GetToStringLines_ReturnsOneLinePerObjectInOrder()
        {
            // Act
            List<string> lines = Program.GetToStringLines(_list);

            // Assert
            Assert.HasCount(3, lines);
            Assert.AreEqual(_alpha.ToString(), lines[0]);
            Assert.AreEqual("Бета;Classic;20;0;ні;01.06.2015;Томатний Класик", lines[1]);
            StringAssert.StartsWith(lines[2], "Вега;Thin;5.5;100;ні;03.03.2010");
        }

        [TestMethod]
        public void IsRoundTripCompatible_AnyListObject_ReturnsTrue()
        {
            // Act & Assert
            foreach (PizzaAsteroid a in _list)
                Assert.IsTrue(Program.IsRoundTripCompatible(a));
        }

        [TestMethod]
        public void IsRoundTripCompatible_Null_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => Program.IsRoundTripCompatible(null!));
        }

        #endregion

        #region FormatTable

        [TestMethod]
        public void FormatTable_NonEmptyList_ContainsTitleHeaderAndAllRows()
        {
            // Act
            string table = Program.FormatTable(_list, "Тестова таблиця");
            string[] lines = table.Split(Environment.NewLine);

            // Assert
            Assert.HasCount(3 + 5, lines);
            Assert.AreEqual("Тестова таблиця", lines[0]);
            StringAssert.Contains(lines[2], "Назва");
            StringAssert.Contains(table, "Альфа");
            StringAssert.Contains(table, "Бета");
            StringAssert.Contains(table, "Вега");
        }

        [TestMethod]
        public void FormatTable_EmptyList_ReturnsNotFoundMessage()
        {
            // Act
            string result = Program.FormatTable(new List<PizzaAsteroid>(), "Порожньо");

            // Assert
            Assert.AreEqual("Жодного об'єкта не знайдено.", result);
        }

        [TestMethod]
        public void FormatTable_NullList_ReturnsNotFoundMessage()
        {
            // Act
            string result = Program.FormatTable(null!, "Null");

            // Assert
            Assert.AreEqual("Жодного об'єкта не знайдено.", result);
        }

        #endregion

        #region Перетворення введених рядків

        [TestMethod]
        [DataRow("42", 42)]
        [DataRow(" 7 ", 7)]
        [DataRow("-15", -15)]
        [DataRow("0", 0)]
        public void TryParseInt_ValidInput_ReturnsTrueAndValue(string input, int expected)
        {
            // Act
            bool ok = Program.TryParseInt(input, out int value);

            // Assert
            Assert.IsTrue(ok);
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("abc")]
        [DataRow("4.5")]
        [DataRow("99999999999")]
        [DataRow(null)]
        public void TryParseInt_InvalidInput_ReturnsFalse(string? input)
        {
            // Act
            bool ok = Program.TryParseInt(input, out _);

            // Assert
            Assert.IsFalse(ok);
        }

        [TestMethod]
        [DataRow("1", 1, 4, true)]
        [DataRow("4", 1, 4, true)]
        [DataRow("0", 1, 4, false)]
        [DataRow("5", 1, 4, false)]
        [DataRow("x", 1, 4, false)]
        public void TryParseIntInRange_ReturnsExpected(string input, int min, int max, bool expected)
        {
            // Act
            bool ok = Program.TryParseIntInRange(input, min, max, out int value);

            // Assert
            Assert.AreEqual(expected, ok);
            if (!ok)
                Assert.AreEqual(0, value);
        }

        [TestMethod]
        public void TryParseIntInRange_MinGreaterThanMax_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => Program.TryParseIntInRange("3", 5, 1, out _));
        }

        [TestMethod]
        [DataRow("12.5", 12.5)]
        [DataRow("12,5", 12.5)]
        [DataRow(" -3 ", -3.0)]
        [DataRow("1e3", 1000.0)]
        public void TryParseDouble_ValidInput_ReturnsTrueAndValue(string input, double expected)
        {
            // Act
            bool ok = Program.TryParseDouble(input, out double value);

            // Assert
            Assert.IsTrue(ok);
            Assert.AreEqual(expected, value, 1e-9);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("   ")]
        [DataRow("abc")]
        [DataRow("1.2.3")]
        [DataRow("NaN")]
        [DataRow("Infinity")]
        [DataRow(null)]
        public void TryParseDouble_InvalidInput_ReturnsFalse(string? input)
        {
            // Act
            bool ok = Program.TryParseDouble(input, out double value);

            // Assert
            Assert.IsFalse(ok);
            Assert.AreEqual(0.0, value);
        }

        [TestMethod]
        [DataRow("1", true)]
        [DataRow("так", true)]
        [DataRow(" ТАК ", true)]
        [DataRow("true", true)]
        [DataRow("0", false)]
        [DataRow("ні", false)]
        [DataRow("False", false)]
        public void TryParseBool_ValidInput_ReturnsTrueAndValue(string input, bool expected)
        {
            // Act
            bool ok = Program.TryParseBool(input, out bool value);

            // Assert
            Assert.IsTrue(ok);
            Assert.AreEqual(expected, value);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("2")]
        [DataRow("yes")]
        [DataRow("може")]
        [DataRow(null)]
        public void TryParseBool_InvalidInput_ReturnsFalse(string? input)
        {
            // Act
            bool ok = Program.TryParseBool(input, out _);

            // Assert
            Assert.IsFalse(ok);
        }

        [TestMethod]
        public void TryParseDate_ValidInput_ReturnsDate()
        {
            // Act
            bool ok = Program.TryParseDate("12.04.2021", out DateTime date);

            // Assert
            Assert.IsTrue(ok);
            Assert.AreEqual(new DateTime(2021, 4, 12), date);
        }

        [TestMethod]
        [DataRow("2021-04-12")]
        [DataRow("31.02.2021")]
        [DataRow("12/04/2021")]
        [DataRow("1.4.2021")]
        [DataRow("")]
        [DataRow(null)]
        public void TryParseDate_InvalidInput_ReturnsFalse(string? input)
        {
            // Act
            bool ok = Program.TryParseDate(input, out _);

            // Assert
            Assert.IsFalse(ok);
        }

        [TestMethod]
        [DataRow("1", CrustType.Thin)]
        [DataRow("2", CrustType.CheeseStuffed)]
        [DataRow("3", CrustType.DeepDish)]
        [DataRow("4", CrustType.Classic)]
        public void TryParseCrust_ValidNumber_ReturnsCrust(string input, CrustType expected)
        {
            // Act
            bool ok = Program.TryParseCrust(input, out CrustType crust);

            // Assert
            Assert.IsTrue(ok);
            Assert.AreEqual(expected, crust);
        }

        [TestMethod]
        [DataRow("0")]
        [DataRow("5")]
        [DataRow("Thin")]
        [DataRow("")]
        public void TryParseCrust_InvalidInput_ReturnsFalse(string input)
        {
            // Act
            bool ok = Program.TryParseCrust(input, out _);

            // Assert
            Assert.IsFalse(ok);
        }

        #endregion
    }
}