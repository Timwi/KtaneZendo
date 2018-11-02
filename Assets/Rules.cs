using System;
using System.Collections.Generic;
using System.Linq;
using Rnd = UnityEngine.Random;
using Assets;
using UnityEngine;

public partial class Zendo
{
    private enum RuleCount { Zero, AtLeastOne, AtLeastTwo, AllThree }
    private enum RuleProperty { FrontColor, FrontSymbol, BackColor, BackSymbol }
    private static Dictionary<RuleProperty?, string> PropertyNames = new Dictionary<RuleProperty?, string>()
    {
        { RuleProperty.FrontColor, "front color" },
        { RuleProperty.FrontSymbol, "front symbol" },
        { RuleProperty.BackColor, "back color" },
        { RuleProperty.BackSymbol, "back symbol" },
    };

    class Values
    {
        private const float FrontLightness = .25f;
        private const float BackLightness = .75f;

        public Dictionary<int, string> FrontSymbols = new Dictionary<int, string>();
        public Dictionary<int, string> BackSymbols = new Dictionary<int, string>();
        public List<Color> FrontColors = new List<Color>();
        public List<Color> BackColors = new List<Color>();
        public List<string> FrontColorNames = new List<string>();
        public List<string> BackColorNames = new List<string>();

        public Values(List<string> possibleFrontSymbols, Dictionary<string, Vector3?> possibleBackSymbols)
        {
            var frontSymbols = possibleFrontSymbols.Shuffle();
            var backSymbols = possibleBackSymbols.Keys.ToList().Shuffle();
            for (var i = 0; i < 3; i++) FrontSymbols.Add(i, frontSymbols[i]);
            for (var i = 0; i < 3; i++) BackSymbols.Add(i, backSymbols[i]);

            for (var i = 0; i < 2; i++)
            {
                int[] hues;
                do hues = new int[] { Rnd.Range(0, 360), Rnd.Range(0, 360), Rnd.Range(0, 360) };
                while (!Colors.EnoughDistance(hues));
                foreach (var hue in hues)
                {
                    if (i == 0)
                    {
                        FrontColorNames.Add(Colors.HueToColorName(hue));
                        FrontColors.Add(Colors.HslToColor(hue, 1f, FrontLightness));
                    }
                    else
                    {
                        BackColorNames.Add(Colors.HueToColorName(hue));
                        BackColors.Add(Colors.HslToColor(hue, 1f, BackLightness));
                    }
                }
            }
        }
    }

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
        private Values Values;
        public RuleCount? Count { get; set; }
        public RuleProperty? FirstProperty { get; set; }
        public RuleProperty? SecondProperty { get; set; }
        public bool NeedsSecondProperty { get; set; }
        public int? FirstVariant { get; set; }
        public int? SecondVariant { get; set; }
        public static Dictionary<string, string> FontAwesome;

        public Rule(Values values)
        {
            Values = values;
        }

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
            if (Count != RuleCount.AllThree)
            {
                FirstVariant = Rnd.Range(0, 3);
            }
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
                    return String.Format("Zero with {0}", PropertyToString(FirstProperty, FirstVariant, Values));
                case RuleCount.AtLeastOne:
                    if (SecondProperty == null)
                        return String.Format("At least one with {0}", PropertyToString(FirstProperty, FirstVariant, Values));
                    else
                        return String.Format("At least one with {0} and {1}", PropertyToString(FirstProperty, FirstVariant, Values), PropertyToString(SecondProperty, SecondVariant, Values));
                case RuleCount.AtLeastTwo:
                    return String.Format("At least two with {0}", PropertyToString(FirstProperty, FirstVariant, Values));
                case RuleCount.AllThree:
                    return String.Format("All three {0}s", PropertyNames[FirstProperty]);
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
            if (!Count.HasValue)
            {
                ClearButtons(ruleButtons);
                Count = (RuleCount)button;
                var optionalS = Count == RuleCount.AllThree ? "S" : "";
                ruleButtons[0].transform.Find("Text").GetComponent<TextMesh>().text = "FRONT\nCOLOR" + optionalS;
                ruleButtons[1].transform.Find("Text").GetComponent<TextMesh>().text = "FRONT\nSYMBOL" + optionalS;
                ruleButtons[2].transform.Find("Text").GetComponent<TextMesh>().text = "BACK\nCOLOR" + optionalS;
                ruleButtons[3].transform.Find("Text").GetComponent<TextMesh>().text = "BACK\nSYMBOL" + optionalS;
            }

            else if (!FirstProperty.HasValue)
            {
                ClearButtons(ruleButtons);
                FirstProperty = (RuleProperty)button;

                if (Count == RuleCount.AllThree) return true;

                if (FirstProperty == RuleProperty.FrontColor)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().text = FontAwesome["square-full"];
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().color = Values.FrontColors[i];
                    }
                }
                else if (FirstProperty == RuleProperty.FrontSymbol)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().text = FontAwesome[Values.FrontSymbols[i]];
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().color = new Color(1f, 1f, 1f);
                    }
                }
                else if (FirstProperty == RuleProperty.BackColor)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().text = FontAwesome["square-full"];
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().color = Values.BackColors[i];
                    }
                }
                else if (FirstProperty == RuleProperty.BackSymbol)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().text = FontAwesome[Values.BackSymbols[i]];
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().color = new Color(1f, 1f, 1f);
                    }
                }
            }

            else if (!FirstVariant.HasValue)
            {
                ClearButtons(ruleButtons);
                FirstVariant = button;

                if (Count != RuleCount.AtLeastOne) return true;

                ruleButtons[0].transform.Find("Text").GetComponent<TextMesh>().text = "(DONE)";
                ruleButtons[1].transform.Find("Text").GetComponent<TextMesh>().text = "AND";
            }

            else if (!NeedsSecondProperty)
            {
                if (button == 0) return true;
                if (button == 1)
                {
                    ClearButtons(ruleButtons);
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
            }

            else if (!SecondProperty.HasValue)
            {
                if (FirstProperty == (RuleProperty)button) return false;

                ClearButtons(ruleButtons);
                SecondProperty = (RuleProperty)button;
                if (SecondProperty == RuleProperty.FrontColor)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().text = FontAwesome["square-full"];
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().color = Values.FrontColors[i];
                    }
                }
                else if (SecondProperty == RuleProperty.FrontSymbol)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().text = FontAwesome[Values.FrontSymbols[i]];
                        ruleButtons[i].transform.Find("Front").GetComponent<TextMesh>().color = new Color(1f, 1f, 1f);
                    }
                }
                else if (SecondProperty == RuleProperty.BackColor)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().text = FontAwesome["square-full"];
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().color = Values.BackColors[i];
                    }
                }
                else if (SecondProperty == RuleProperty.BackSymbol)
                {
                    for (var i = 0; i < 3; i++)
                    {
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().text = FontAwesome[Values.BackSymbols[i]];
                        ruleButtons[i].transform.Find("Back").GetComponent<TextMesh>().color = new Color(1f, 1f, 1f);
                    }
                }
            }

            else if (!SecondVariant.HasValue)
            {
                ClearButtons(ruleButtons);
                SecondVariant = button;
                return true;
            }

            return false;
        }
    }

    class Config : IEquatable<Config>
    {
        private Values Values;

        public List<Tile> Tiles { get; set; }

        public Config(Values values)
        {
            Values = values;
        }

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
            return String.Join(", ", Tiles.Select(
            t => t is Tile
            ? String.Format(
                    "{0} {1} on {2} {3}",
                    Values.FrontColorNames[t.Properties[RuleProperty.FrontColor]],
                    Values.FrontSymbols[t.Properties[RuleProperty.FrontSymbol]],
                    Values.BackColorNames[t.Properties[RuleProperty.BackColor]],
                    Values.BackSymbols[t.Properties[RuleProperty.BackSymbol]])
            : "empty"
            ).ToArray());
        }
    }

    public static T RandomEnumValue<T>()
    {
        var values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(Rnd.Range(0, values.Length));
    }

    private static string PropertyToString(RuleProperty? property, int? variant, Values values)
    {
        var str = PropertyNames[property] + " ";
        switch (property)
        {
            case RuleProperty.FrontColor:
                str += values.FrontColorNames[(int)variant];
                break;
            case RuleProperty.FrontSymbol:
                str += values.FrontSymbols[(int)variant];
                break;
            case RuleProperty.BackColor:
                str += values.BackColorNames[(int)variant];
                break;
            case RuleProperty.BackSymbol:
                str += values.BackSymbols[(int)variant];
                break;
        }
        return str;
    }
}
