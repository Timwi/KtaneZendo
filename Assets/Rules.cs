using System;
using System.Collections.Generic;
using System.Linq;
using Rnd = UnityEngine.Random;
using Assets;

public partial class Zendo
{
    enum RuleCount { Zero, AtLeastOne, AtLeastTwo, AllThree }
    enum RuleProperty { SymbolColor, Symbol, PatternColor, Pattern }

    class Tile
    {
        public Tile()
        {
            Properties = new Dictionary<RuleProperty?, int>()
            {
                { RuleProperty.SymbolColor, 1 },
                { RuleProperty.Symbol, 1 },
                { RuleProperty.PatternColor, 1 },
                { RuleProperty.Pattern, 1 },
            };
        }
        public Dictionary<RuleProperty?, int> Properties { get; set; }
    }

    class Rule : IEquatable<Rule>
    {
        public RuleCount Count { get; set; }
        public RuleProperty FirstProperty { get; set; }
        public RuleProperty? SecondProperty { get; set; }
        public int FirstVariant { get; set; }
        public int SecondVariant { get; set; }

        public bool Check(Config c)
        {
            switch (Count)
            {
                case RuleCount.Zero:
                    return c.Tiles.OfType<Tile>().Count(t => t.Properties[FirstProperty] == FirstVariant) == 0;

                case RuleCount.AtLeastOne:
                    if (SecondProperty == null)
                        return c.Tiles.OfType<Tile>().Any(t => t.Properties[FirstProperty] == FirstVariant);
                    else
                        return c.Tiles.OfType<Tile>().Any(t => t.Properties[FirstProperty] == FirstVariant && t.Properties[SecondProperty] == SecondVariant);

                case RuleCount.AtLeastTwo:
                    return c.Tiles.OfType<Tile>().Count(t => t.Properties[FirstProperty] == FirstVariant) > 1;

                case RuleCount.AllThree:
                    return c.Tiles.OfType<Tile>().Any(t => t.Properties[FirstProperty] == 1)
                        && c.Tiles.OfType<Tile>().Any(t => t.Properties[FirstProperty] == 2)
                        && c.Tiles.OfType<Tile>().Any(t => t.Properties[FirstProperty] == 3);
            }
            return false;
        }

        public bool Equals(Rule other)
        {
            if (other == null) return false;

            return (Count == other.Count
                && FirstProperty == other.FirstProperty
                && SecondProperty == other.SecondProperty
                && FirstVariant == other.FirstVariant
                && SecondVariant == other.SecondVariant);
        }

        public void Randomize()
        {
            Count = RandomEnumValue<RuleCount>();
            FirstProperty = RandomEnumValue<RuleProperty>();
            FirstVariant = Rnd.Range(1, 4);
            if (Count == RuleCount.AtLeastOne && Rnd.Range(0, 2) == 0)
            {
                do SecondProperty = RandomEnumValue<RuleProperty>();
                while (SecondProperty == FirstProperty);
                SecondVariant = Rnd.Range(1, 4);
            }
        }

        override public string ToString()
        {
            switch (Count)
            {
                case RuleCount.Zero:
                    return String.Format("Zero with {0} {1}", FirstProperty, FirstVariant);
                case RuleCount.AtLeastOne:
                    if (SecondProperty == null)
                        return String.Format("At least one with {0} {1}", FirstProperty, FirstVariant);
                    else
                        return String.Format("At least one with {0} {1} and {2} {3}", FirstProperty, FirstVariant, SecondProperty, SecondVariant);
                case RuleCount.AtLeastTwo:
                    return String.Format("At least two with {0} {1}", FirstProperty, FirstVariant);
                case RuleCount.AllThree:
                    return String.Format("All three {0}s", FirstProperty);
            }
            return "";
        }
    }

    class Config : IEquatable<Config>
    {
        public List<Tile> Tiles { get; set; }

        public bool Equals(Config config)
        {
            for (var i = 0; i < Tiles.Count; i++)
            {
                if (Tiles[i] == null && config.Tiles[i] is Tile) return false;
                if (Tiles[i] is Tile && config.Tiles[i] == null) return false;
                if (Tiles[i] is Tile)
                {
                    foreach (var property in Tiles[i].Properties)
                    {
                        if (config.Tiles[i].Properties[property.Key] != property.Value) return false;
                    }
                }
            }
            return true;
        }

        public void Randomize()
        {
            Tiles = new List<Tile>() { null, null, null, null };

            // Less chance on 1 or 4 tiles, more chance on 2 or 3 tiles
            var numTiles = (new List<int>() { 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4 })[Rnd.Range(0, 12)];
            for (var i = 0; i < numTiles; i++)
            {
                Tiles[i] = new Tile()
                {
                    Properties = new Dictionary<RuleProperty?, int>()
                    {
                        { RuleProperty.SymbolColor, Rnd.Range(1, 4) },
                        { RuleProperty.Symbol, Rnd.Range(1, 4) },
                        { RuleProperty.PatternColor, Rnd.Range(1, 4) },
                        { RuleProperty.Pattern, Rnd.Range(1, 4) },
                    }
                };
            }

            // Random position
            Tiles = Tiles.Shuffle();
        }

        override public string ToString()
        {
            return String.Join("\n", Tiles.Select(
            t => t is Tile
            ? String.Format(
                    "symbol color {0}, symbol {1}, pattern color {2}, pattern {3}",
                    t.Properties[RuleProperty.SymbolColor],
                    t.Properties[RuleProperty.Symbol],
                    t.Properties[RuleProperty.PatternColor],
                    t.Properties[RuleProperty.Pattern])
            : "empty"
            ).ToArray());
        }
    }

    public static T RandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Rnd.Range(0, values.Length));
    }
}
