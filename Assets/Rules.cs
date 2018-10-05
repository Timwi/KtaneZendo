using System;
using System.Collections.Generic;
using System.Linq;
using Rnd = UnityEngine.Random;

public partial class Zendo
{
    enum RuleCount { Zero, AtLeastOne, AtLeastTwo, AllThree }
    enum RuleProperty { SymbolColor, Symbol, PatternColor, Pattern }

    class Tile
    {
        public Dictionary<RuleProperty, int> Properties { get; set; }
    }

    class Rule : IEquatable<Rule>
    {
        public RuleCount Count { get; set; }
        public RuleProperty FirstProperty { get; set; }
        public RuleProperty SecondProperty { get; set; }
        public int FirstVariant { get; set; }
        public int SecondVariant { get; set; }

        public bool Check(Config c) {
            if (Count == RuleCount.AllThree)
            {
                return c.Tiles.OfType<Tile>().Any(t => t.Properties[this.FirstProperty] == 1)
                    && c.Tiles.OfType<Tile>().Any(t => t.Properties[this.FirstProperty] == 2)
                    && c.Tiles.OfType<Tile>().Any(t => t.Properties[this.FirstProperty] == 3);

            }
            return false;
        }

        public bool Equals(Rule other)
        {
            if (other == null) return false;

            return (
                Count == other.Count
                && FirstProperty == other.FirstProperty
                && SecondProperty == other.SecondProperty
                && FirstVariant == other.FirstVariant
                && SecondVariant == other.SecondVariant);
        }

        public void PickRandom()
        {
            this.Count = RandomEnumValue<RuleCount>();
            this.FirstProperty = RandomEnumValue<RuleProperty>();
            this.FirstVariant = Rnd.Range(1, 4);
            if (this.Count == RuleCount.AtLeastOne && Rnd.Range(0, 2) == 0)
            {
                do this.SecondProperty = RandomEnumValue<RuleProperty>();
                while (this.SecondProperty == this.FirstProperty);
                this.SecondVariant = Rnd.Range(1, 4);
            }
        }


    }

    public static T RandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Rnd.Range(0, values.Length));
    }
}
