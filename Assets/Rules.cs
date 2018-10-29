using System;
using System.Collections.Generic;
using System.Linq;
using Rnd = UnityEngine.Random;
using Assets;
using UnityEngine;

public partial class Zendo
{
    enum RuleCount { Zero, AtLeastOne, AtLeastTwo, AllThree }
    enum RuleProperty { FrontColor, FrontSymbol, BackColor, BackSymbol }

    class Tile
    {
        public Tile()
        {
            Properties = new Dictionary<RuleProperty?, int>()
            {
                { RuleProperty.FrontColor, 0 },
                { RuleProperty.FrontSymbol, 0 },
                { RuleProperty.BackColor, 0 },
                { RuleProperty.BackSymbol, 0 },
            };
        }
        public Dictionary<RuleProperty?, int> Properties { get; set; }
    }

    class Rule : IEquatable<Rule>
    {
        public RuleCount? Count { get; set; }
        public RuleProperty? FirstProperty { get; set; }
        public RuleProperty? SecondProperty { get; set; }
        public bool NeedsSecondProperty { get; set; }
        public int FirstVariant { get; set; }
        public int SecondVariant { get; set; }
        public static List<Color> Colors { get; set; }
        public static Dictionary<string, string> FontAwesome { get; set; }
        public static Dictionary<int, string> FrontSymbols = new Dictionary<int, string>();
        public static Dictionary<int, string> BackSymbols = new Dictionary<int, string>();

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
                    return c.Tiles.OfType<Tile>().Any(t => t.Properties[FirstProperty] == 0)
                        && c.Tiles.OfType<Tile>().Any(t => t.Properties[FirstProperty] == 1)
                        && c.Tiles.OfType<Tile>().Any(t => t.Properties[FirstProperty] == 2);
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
            FirstVariant = Rnd.Range(0, 3);
            if (Count == RuleCount.AtLeastOne && Rnd.Range(0, 2) == 0)
            {
                do SecondProperty = RandomEnumValue<RuleProperty>();
                while (SecondProperty == FirstProperty);
                SecondVariant = Rnd.Range(0, 3);
            }
        }

        override public string ToString()
        {
            switch (Count)
            {
                case RuleCount.Zero:
                    return String.Format("Zero with {0} {1}", FirstProperty, FirstVariant + 1);
                case RuleCount.AtLeastOne:
                    if (SecondProperty == null)
                        return String.Format("At least one with {0} {1}", FirstProperty, FirstVariant + 1);
                    else
                        return String.Format("At least one with {0} {1} and {2} {3}", FirstProperty, FirstVariant + 1, SecondProperty, SecondVariant + 1);
                case RuleCount.AtLeastTwo:
                    return String.Format("At least two with {0} {1}", FirstProperty, FirstVariant + 1);
                case RuleCount.AllThree:
                    return String.Format("All three {0}s", FirstProperty);
            }
            return "";
        }

        public void ClearButtons(KMSelectable[] ruleButtons)
        {
            foreach (var button in ruleButtons)
            {
                button.transform.Find("Text").GetComponent<TextMesh>().text = "";
                button.transform.Find("Front").GetComponent<TextMesh>().text = "";
                button.transform.Find("Back").GetComponent<TextMesh>().text = "";
            }
        }

        public void InitializeButtons(KMSelectable[] ruleButtons)
        {
            ClearButtons(ruleButtons);
            ruleButtons[0].transform.Find("Text").GetComponent<TextMesh>().text = "ZERO\nWITH";
            ruleButtons[1].transform.Find("Text").GetComponent<TextMesh>().text = "AT\nLEAST\nONE\nWITH";
            ruleButtons[2].transform.Find("Text").GetComponent<TextMesh>().text = "AT\nLEAST\nTWO\nWITH";
            ruleButtons[3].transform.Find("Text").GetComponent<TextMesh>().text = "ALL\nTHREE";
        }

        public bool PressButton(int button, KMSelectable[] ruleButtons)
        {
            ClearButtons(ruleButtons);

            if (!Count.HasValue)
            {
                Count = (RuleCount)button;
                var optionalS = Count == RuleCount.AllThree ? "S" : "";
                ruleButtons[0].transform.Find("Text").GetComponent<TextMesh>().text = "FRONT\nCOLOR" + optionalS;
                ruleButtons[1].transform.Find("Text").GetComponent<TextMesh>().text = "FRONT\nSYMBOL" + optionalS;
                ruleButtons[2].transform.Find("Text").GetComponent<TextMesh>().text = "BACK\nCOLOR" + optionalS;
                ruleButtons[3].transform.Find("Text").GetComponent<TextMesh>().text = "BACK\nSYMBOL" + optionalS;
            }

            else if (!FirstProperty.HasValue)
            {
                FirstProperty = (RuleProperty)button;

                if (Count == RuleCount.AllThree)
                {
                    return true;
                }
                if (FirstProperty == RuleProperty.FrontColor)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().text = FontAwesome["square-full"];
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().color = Colors[i];
                    }
                }
                else if (FirstProperty == RuleProperty.FrontSymbol)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().text = FontAwesome[FrontSymbols[i]];
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().color = new Color(1f, 1f, 1f);
                    }
                }
                else if (FirstProperty == RuleProperty.BackColor)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().text = FontAwesome["square-full"];
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().color = Colors[i + 3];
                    }
                }
                else if (FirstProperty == RuleProperty.BackSymbol)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().text = FontAwesome[BackSymbols[i]];
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().color = new Color(1f, 1f, 1f);
                    }
                }
            }

            else if (FirstVariant == 0)
            {
                FirstVariant = button;

                if (Count != RuleCount.AtLeastOne)
                    return true;

                ruleButtons[0].transform.Find("Text").GetComponent<TextMesh>().text = "(DONE)";
                ruleButtons[1].transform.Find("Text").GetComponent<TextMesh>().text = "AND";
            }

            else if (!NeedsSecondProperty)
            {
                if (button == 0)
                    return true;

                NeedsSecondProperty = true;

                if (FirstProperty != RuleProperty.FrontColor)
                    ruleButtons[0].transform.Find("Text").GetComponent<TextMesh>().text = "FRONT\nCOLOR";
                if (FirstProperty != RuleProperty.FrontSymbol)
                    ruleButtons[1].transform.Find("Text").GetComponent<TextMesh>().text = "FRONT\nSYMBOL";
                if (FirstProperty != RuleProperty.BackColor)
                    ruleButtons[2].transform.Find("Text").GetComponent<TextMesh>().text = "BACK\nCOLOR";
                if (FirstProperty != RuleProperty.BackSymbol)
                    ruleButtons[3].transform.Find("Text").GetComponent<TextMesh>().text = "BACK\nSYMBOL";
            }

            else if (!SecondProperty.HasValue)
            {
                SecondProperty = (RuleProperty)button;

                //if (FirstProperty == SecondProperty) return false;

                if (SecondProperty == RuleProperty.FrontColor)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().text = FontAwesome["square-full"];
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().color = Colors[i];
                    }
                }
                else if (SecondProperty == RuleProperty.FrontSymbol)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().text = FontAwesome[FrontSymbols[i]];
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().color = new Color(1f, 1f, 1f);
                    }
                }
                else if (SecondProperty == RuleProperty.BackColor)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().text = FontAwesome["square-full"];
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().color = Colors[i + 3];
                    }
                }
                else if (SecondProperty == RuleProperty.BackSymbol)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().text = FontAwesome[BackSymbols[i]];
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().color = new Color(1f, 1f, 1f);
                    }
                }
            }

            else if (SecondVariant == 0)
            {
                SecondVariant = button;
                return true;
            }

            return false;
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
                        { RuleProperty.FrontColor, Rnd.Range(0, 3) },
                        { RuleProperty.FrontSymbol, Rnd.Range(0, 3) },
                        { RuleProperty.BackColor, Rnd.Range(0, 3) },
                        { RuleProperty.BackSymbol, Rnd.Range(0, 3) },
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
                    t.Properties[RuleProperty.FrontColor] + 1,
                    t.Properties[RuleProperty.FrontSymbol] + 1,
                    t.Properties[RuleProperty.BackColor] + 1,
                    t.Properties[RuleProperty.BackSymbol] + 1)
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
