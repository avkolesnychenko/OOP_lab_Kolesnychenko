namespace PizzaAsteroidApp
{
    /// <summary>
    /// Результат десеріалізації колекції з файлу (*.csv або *.json).
    /// </summary>
    public sealed class LoadResult
    {
        /// <summary>Кількість об'єктів, коректно десеріалізованих і доданих до списку.</summary>
        public int Added { get; internal set; }

        /// <summary>Кількість пропущених «битих» записів (рядків CSV або null-елементів JSON).</summary>
        public int Invalid { get; internal set; }

        /// <summary>Кількість коректних об'єктів, які не додано через досягнення ліміту N.</summary>
        public int OverCapacity { get; internal set; }

        /// <summary>Опис причин, з яких записи було пропущено.</summary>
        public List<string> Errors { get; } = [];

        /// <summary>Загальна кількість оброблених записів.</summary>
        public int Total => Added + Invalid + OverCapacity;
    }
}
