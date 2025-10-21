namespace MyVocaList.View.Components
{
    /// <summary>
    /// A typesafe "smart enum" for defining standard icon sizes.
    /// It's LINQ-ready and can hold more data than a standard enum.
    /// </summary>
    public sealed class StatefulIconSize
    {
        public string Name { get; }
        public double Dimension { get; }

        private StatefulIconSize(string name, double dimension)
        {
            Name = name;
            Dimension = dimension;
        }

        public override string ToString() => Name;

        // --- Static instances ---
        public static readonly StatefulIconSize Small = new("Small", 18);
        public static readonly StatefulIconSize Medium = new("Medium", 24);
        public static readonly StatefulIconSize Large = new("Large", 32);

        // --- List for LINQ usage ---
        public static readonly List<StatefulIconSize> All = new() { Small, Medium, Large };
    }
}
