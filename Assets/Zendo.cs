using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KmHelper;
using Rnd = UnityEngine.Random;
using Assets;

public class Zendo : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public Sprite[] Sprites;
    public KMSelectable[] Tiles;
    public KMSelectable[] TileButtons;
    public KMSelectable[] QuizButtons;
    public KMSelectable[] GuessButtons;
    public TextMesh GuessTokens;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _isButtonDown, _isLongPress;
    private Coroutine _buttonDownCoroutine;

    private Dictionary<string, Rule> _rules = new Dictionary<RuleEnum, Rule>();
    /*
    {
        { "all three symbol colors", new Rule() { Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == 1)
                && c.Tiles.OfType<Tile>().Any(t => t.Color == 2)
                && c.Tiles.OfType<Tile>().Any(t => t.Color == 3);
        } } },
        { "all three symbols", new Rule() { Check = (Config c) => {
            return c.Tiles.OfType<Tile>().OfType<Tile>().Any(t => t.Symbol == Symbol.Sym1)
                && c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym2)
                && c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym3);
        } } },
        { RuleEnum.AllThreePats, new Rule() { Text = "all three directions", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up)
                && c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Left)
                && c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneSym1, new Rule() { Text = "at least one Sym1", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym1);
        } } },
        { RuleEnum.AtLeastOneSymCol1Sym1, new Rule() { Text = "at least one SymCol1 Sym1", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol1 && t.Symbol == Symbol.Sym1);
        } } },
        { RuleEnum.AtLeastOneSymCol2Sym1, new Rule() { Text = "at least one SymCol2 Sym1", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol2 && t.Symbol == Symbol.Sym1);
        } } },
        { RuleEnum.AtLeastOneSymCol3Sym1, new Rule() { Text = "at least one SymCol3 Sym1", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol3 && t.Symbol == Symbol.Sym1);
        } } },
        { RuleEnum.AtLeastOneSym1Pat1, new Rule() { Text = "at least one Sym1 pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Symbol == Symbol.Sym1);
        } } },
        { RuleEnum.AtLeastOneSym1Pat2, new Rule() { Text = "at least one Sym1 pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym1 && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneSym1Pat3, new Rule() { Text = "at least one Sym1 pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym1 && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneSym2, new Rule() { Text = "at least one Sym2", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym2);
        } } },
        { RuleEnum.AtLeastOneSymCol1Sym2, new Rule() { Text = "at least one SymCol1 Sym2", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol1 && t.Symbol == Symbol.Sym2);
        } } },
        { RuleEnum.AtLeastOneSymCol2Sym2, new Rule() { Text = "at least one SymCol2 Sym2", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol2 && t.Symbol == Symbol.Sym2);
        } } },
        { RuleEnum.AtLeastOneSymCol3Sym2, new Rule() { Text = "at least one SymCol3 Sym2", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol3 && t.Symbol == Symbol.Sym2);
        } } },
        { RuleEnum.AtLeastOneSym2Pat1, new Rule() { Text = "at least one Sym2 pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Symbol == Symbol.Sym2);
        } } },
        { RuleEnum.AtLeastOneSym2Pat2, new Rule() { Text = "at least one Sym2 pointint left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym2 && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneSym2Pat3, new Rule() { Text = "at least one Sym2 pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym2 && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneSym3, new Rule() { Text = "at least one Sym3", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym3);
        } } },
        { RuleEnum.AtLeastOneSymCol1Sym3, new Rule() { Text = "at least one SymCol1 Sym3", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol1 && t.Symbol == Symbol.Sym3);
        } } },
        { RuleEnum.AtLeastOneSymCol2Sym3, new Rule() { Text = "at least one SymCol2 Sym3", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol2 && t.Symbol == Symbol.Sym3);
        } } },
        { RuleEnum.AtLeastOneSymCol3Sym3, new Rule() { Text = "at least one SymCol3 Sym3", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol3 && t.Symbol == Symbol.Sym3);
        } } },
        { RuleEnum.AtLeastOneSym3Pat1, new Rule() { Text = "at least one Sym3 pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Symbol == Symbol.Sym3);
        } } },
        { RuleEnum.AtLeastOneSym3Pat2, new Rule() { Text = "at least one Sym3 pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym3 && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneSym3Pat3, new Rule() { Text = "at least one Sym3 pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Sym3 && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneSymCol1, new Rule() { Text = "at least one SymCol1", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol1);
        } } },
        { RuleEnum.AtLeastOneSymCol1Pat1, new Rule() { Text = "at least one SymCol1 pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Color == Color.SymCol1);
        } } },
        { RuleEnum.AtLeastOneSymCol1Pat2, new Rule() { Text = "at least one SymCol1 pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol1 && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneSymCol1Pat3, new Rule() { Text = "at least one SymCol1 pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol1 && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneSymCol2, new Rule() { Text = "at least one SymCol2", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol2);
        } } },
        { RuleEnum.AtLeastOneSymCol2Pat1, new Rule() { Text = "at least one SymCol2 pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Color == Color.SymCol2);
        } } },
        { RuleEnum.AtLeastOneSymCol2Pat2, new Rule() { Text = "at least one SymCol2 pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol2 && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneSymCol2Pat3, new Rule() { Text = "at least one SymCol2 pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol2 && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneSymCol3, new Rule() { Text = "at least one SymCol3", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol3);
        } } },
        { RuleEnum.AtLeastOneSymCol3Pat1, new Rule() { Text = "at least one SymCol3 pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Color == Color.SymCol3);
        } } },
        { RuleEnum.AtLeastOneSymCol3Pat2, new Rule() { Text = "at least one SymCol3 pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol3 && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneSymCol3Pat3, new Rule() { Text = "at least one SymCol3 pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.SymCol3 && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOnePat1, new Rule() { Text = "at least one pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up);
        } } },
        { RuleEnum.AtLeastOnePat2, new Rule() { Text = "at least one pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOnePat3, new Rule() { Text = "at least one pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastTwoSym1, new Rule() { Text = "at least two Sym1", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Sym1) > 1;
        } } },
        { RuleEnum.AtLeastTwoSym2, new Rule() { Text = "at least two Sym2", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Sym2) > 1;
        } } },
        { RuleEnum.AtLeastTwoSym3, new Rule() { Text = "at least two Sym3", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Sym3) > 1;
        } } },
        { RuleEnum.AtLeastTwoSymCol1, new Rule() { Text = "at least two SymCol1", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.SymCol1) > 1;
        } } },
        { RuleEnum.AtLeastTwoSymCol2, new Rule() { Text = "at least two SymCol2", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.SymCol2) > 1;
        } } },
        { RuleEnum.AtLeastTwoSymCol3, new Rule() { Text = "at least two SymCol3", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.SymCol3) > 1;
        } } },
        { RuleEnum.AtLeastTwoPat1, new Rule() { Text = "at least two pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Up) > 1;
        } } },
        { RuleEnum.AtLeastTwoPat2, new Rule() { Text = "at least two pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Left) > 1;
        } } },
        { RuleEnum.AtLeastTwoPat3, new Rule() { Text = "at least two pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Right) > 1;
        } } },
        { RuleEnum.ZeroSym1, new Rule() { Text = "zero Sym1", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Sym1) == 0;
        } } },
        { RuleEnum.ZeroSym2, new Rule() { Text = "zero Sym2", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Sym2) == 0;
        } } },
        { RuleEnum.ZeroSym3, new Rule() { Text = "zero Sym3", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Sym3) == 0;
        } } },
        { RuleEnum.ZeroSymCol1, new Rule() { Text = "zero SymCol1", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.SymCol1) == 0;
        } } },
        { RuleEnum.ZeroSymCol2, new Rule() { Text = "zero SymCol2", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.SymCol2) == 0;
        } } },
        { RuleEnum.ZeroSymCol3, new Rule() { Text = "zero SymCol3", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.SymCol3) == 0;
        } } },
        { RuleEnum.ZeroPat1, new Rule() { Text = "zero pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Up) == 0;
        } } },
        { RuleEnum.ZeroPat2, new Rule() { Text = "zero pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Left) == 0;
        } } },
        { RuleEnum.ZeroPat3, new Rule() { Text = "zero pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Right) == 0;
        } } },
    };*/

    private enum Property { SymbolColor, Symbol, PatterColor, Pattern };
    private string[] _propertyNames = { "symbol color", "symbol", "pattern color", "pattern" };
    private RulePart _ruleTree;
    private RulePart _activeRulePart;
    private RuleEnum _masterRule;
    private Config _config;
    private Config _followsRule;
    private Config _doesNotFollowRule;
    private List<Config> _quizzedConfigs = new List<Config>();
    private KMSelectable _previousButton;
    private int _activeTile = -1;
    private int _guessTokens = 0;

    void Start()
    {
        _moduleId = _moduleIdCounter++;

        for (int i = 0; i < Tiles.Length; i++)
        {
            var j = i;
            Tiles[i].OnInteract += delegate () { _buttonDownCoroutine = StartCoroutine(HandleLongPress()); return false; };
            Tiles[i].OnInteractEnded += delegate () { HandleButtonUp(); PressTile(j); };
        }

        for (int i = 0; i < TileButtons.Length; i++)
        {
            var j = i;
            TileButtons[i].OnInteract += delegate () { _buttonDownCoroutine = StartCoroutine(HandleLongPress()); return false; };
            TileButtons[i].OnInteractEnded += delegate () { HandleButtonUp(); PressTileButton(j); };
        }

        for (int i = 0; i < QuizButtons.Length; i++)
        {
            var j = i;
            QuizButtons[i].OnInteract += delegate () { _buttonDownCoroutine = StartCoroutine(HandleLongPress()); return false; };
            QuizButtons[i].OnInteractEnded += delegate () { HandleButtonUp(); PressQuizButton(j); };
        }

        for (int i = 0; i < GuessButtons.Length; i++)
        {
            var j = i;
            GuessButtons[i].OnInteract += delegate () { _buttonDownCoroutine = StartCoroutine(HandleLongPress()); return false; };
            GuessButtons[i].OnInteractEnded += delegate () { HandleButtonUp(); PressGuessButton(j); };
        }

        _rules = new Dictionary<string, Rule>();

        for (var i = 0; i < 3; i++)
        {
            _rules.Add("all three " + _propertyNames[i] + "s", new Rule()
            {
                Check = (Config c) =>
                {
                    return c.Tiles.OfType<Tile>().Any(t => t.Property[i] == 1)
                        && c.Tiles.OfType<Tile>().Any(t => t.Property[i] == 2)
                        && c.Tiles.OfType<Tile>().Any(t => t.Property[i] == 3);
                }
            });
        }

        _ruleTree = new RulePart()
        {
            Children = new List<RulePart>() {
            new RulePart() { Text = "All three", Children = new List<RulePart>() {
                new RulePart() { Text = "colors.", Rule = RuleEnum.AllThreeSyms },
                new RulePart() { Text = "symbols.", Rule = RuleEnum.AllThreeSymCols },
                new RulePart() { Text = "directions.", Rule = RuleEnum.AllThreePats },
            } },
            new RulePart() { Text = "At least one tile", Children = new List<RulePart>() {
                new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                    new RulePart() { Text = "Sym1", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = RuleEnum.AtLeastOneSym1 },
                        new RulePart() { Text = "coloSymCol1", Children = new List<RulePart>() {
                            new RulePart() { Text = "SymCol1", Rule = RuleEnum.AtLeastOneSymCol1Sym1 },
                            new RulePart() { Text = "SymCol2", Rule = RuleEnum.AtLeastOneSymCol2Sym1 },
                            new RulePart() { Text = "SymCol3", Rule = RuleEnum.AtLeastOneSymCol3Sym1 },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = RuleEnum.AtLeastOneSym1Pat1 },
                            new RulePart() { Text = "left", Rule = RuleEnum.AtLeastOneSym1Pat2 },
                            new RulePart() { Text = "right", Rule = RuleEnum.AtLeastOneSym1Pat3 },
                        } },
                    } },
                    new RulePart() { Text = "Sym2", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = RuleEnum.AtLeastOneSym2 },
                        new RulePart() { Text = "coloSymCol1", Children = new List<RulePart>() {
                            new RulePart() { Text = "SymCol1", Rule = RuleEnum.AtLeastOneSymCol1Sym2 },
                            new RulePart() { Text = "SymCol2", Rule = RuleEnum.AtLeastOneSymCol2Sym2 },
                            new RulePart() { Text = "SymCol3", Rule = RuleEnum.AtLeastOneSymCol3Sym2 },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = RuleEnum.AtLeastOneSym2Pat1 },
                            new RulePart() { Text = "left", Rule = RuleEnum.AtLeastOneSym2Pat2 },
                            new RulePart() { Text = "right", Rule = RuleEnum.AtLeastOneSym2Pat3 },
                        } },
                    } },
                    new RulePart() { Text = "Sym3", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = RuleEnum.AtLeastOneSym3 },
                        new RulePart() { Text = "coloSymCol1", Children = new List<RulePart>() {
                            new RulePart() { Text = "SymCol1", Rule = RuleEnum.AtLeastOneSymCol1Sym3 },
                            new RulePart() { Text = "SymCol2", Rule = RuleEnum.AtLeastOneSymCol2Sym3 },
                            new RulePart() { Text = "SymCol3", Rule = RuleEnum.AtLeastOneSymCol3Sym3 },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = RuleEnum.AtLeastOneSym3Pat1 },
                            new RulePart() { Text = "left", Rule = RuleEnum.AtLeastOneSym3Pat2 },
                            new RulePart() { Text = "right", Rule = RuleEnum.AtLeastOneSym3Pat3 },
                        } },
                    } },
                } },
                new RulePart() { Text = "coloSymCol1", Children = new List<RulePart>() {
                    new RulePart() { Text = "SymCol1", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = RuleEnum.AtLeastOneSymCol1 },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Sym1", Rule = RuleEnum.AtLeastOneSymCol1Sym1 },
                            new RulePart() { Text = "Sym2", Rule = RuleEnum.AtLeastOneSymCol1Sym2 },
                            new RulePart() { Text = "Sym3", Rule = RuleEnum.AtLeastOneSymCol1Sym3 },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = RuleEnum.AtLeastOneSymCol1Pat1 },
                            new RulePart() { Text = "left", Rule = RuleEnum.AtLeastOneSymCol1Pat2 },
                            new RulePart() { Text = "right", Rule = RuleEnum.AtLeastOneSymCol1Pat3 },
                        } },
                    } },
                    new RulePart() { Text = "SymCol2", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = RuleEnum.AtLeastOneSymCol2 },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Sym1", Rule = RuleEnum.AtLeastOneSymCol2Sym1 },
                            new RulePart() { Text = "Sym2", Rule = RuleEnum.AtLeastOneSymCol2Sym2 },
                            new RulePart() { Text = "Sym3", Rule = RuleEnum.AtLeastOneSymCol2Sym3 },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = RuleEnum.AtLeastOneSymCol2Pat1 },
                            new RulePart() { Text = "left", Rule = RuleEnum.AtLeastOneSymCol2Pat2 },
                            new RulePart() { Text = "right", Rule = RuleEnum.AtLeastOneSymCol2Pat3 },
                        } },
                    } },
                    new RulePart() { Text = "SymCol3", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = RuleEnum.AtLeastOneSymCol3 },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Sym1", Rule = RuleEnum.AtLeastOneSymCol3Sym1 },
                            new RulePart() { Text = "Sym2", Rule = RuleEnum.AtLeastOneSymCol3Sym2 },
                            new RulePart() { Text = "Sym3", Rule = RuleEnum.AtLeastOneSymCol3Sym3 },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = RuleEnum.AtLeastOneSymCol3Pat1 },
                            new RulePart() { Text = "left", Rule = RuleEnum.AtLeastOneSymCol3Pat2 },
                            new RulePart() { Text = "right", Rule = RuleEnum.AtLeastOneSymCol3Pat3 },
                        } },
                    } },
                } },
                new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                    new RulePart() { Text = "up", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = RuleEnum.AtLeastOnePat1 },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Sym1", Rule = RuleEnum.AtLeastOneSym1Pat1 },
                            new RulePart() { Text = "Sym2", Rule = RuleEnum.AtLeastOneSym2Pat1 },
                            new RulePart() { Text = "Sym3", Rule = RuleEnum.AtLeastOneSym3Pat1 },
                        } },
                        new RulePart() { Text = "coloSymCol1", Children = new List<RulePart>() {
                            new RulePart() { Text = "SymCol1", Rule = RuleEnum.AtLeastOneSymCol1Pat1 },
                            new RulePart() { Text = "SymCol2", Rule = RuleEnum.AtLeastOneSymCol2Pat1 },
                            new RulePart() { Text = "SymCol3", Rule = RuleEnum.AtLeastOneSymCol3Pat1 },
                        } },
                    } },
                    new RulePart() { Text = "left", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = RuleEnum.AtLeastOnePat2 },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Sym1", Rule = RuleEnum.AtLeastOneSym1Pat2 },
                            new RulePart() { Text = "Sym2", Rule = RuleEnum.AtLeastOneSym2Pat2 },
                            new RulePart() { Text = "Sym3", Rule = RuleEnum.AtLeastOneSym3Pat2 },
                        } },
                        new RulePart() { Text = "coloSymCol1", Children = new List<RulePart>() {
                            new RulePart() { Text = "SymCol1", Rule = RuleEnum.AtLeastOneSymCol1Pat2 },
                            new RulePart() { Text = "SymCol2", Rule = RuleEnum.AtLeastOneSymCol2Pat2 },
                            new RulePart() { Text = "SymCol3", Rule = RuleEnum.AtLeastOneSymCol3Pat2 },
                        } },
                    } },
                    new RulePart() { Text = "right", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = RuleEnum.AtLeastOnePat3 },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Sym1", Rule = RuleEnum.AtLeastOneSym1Pat3 },
                            new RulePart() { Text = "Sym2", Rule = RuleEnum.AtLeastOneSym2Pat3 },
                            new RulePart() { Text = "Sym3", Rule = RuleEnum.AtLeastOneSym3Pat3 },
                        } },
                        new RulePart() { Text = "coloSymCol1", Children = new List<RulePart>() {
                            new RulePart() { Text = "SymCol1", Rule = RuleEnum.AtLeastOneSymCol1Pat3 },
                            new RulePart() { Text = "SymCol2", Rule = RuleEnum.AtLeastOneSymCol2Pat3 },
                            new RulePart() { Text = "SymCol3", Rule = RuleEnum.AtLeastOneSymCol3Pat3 },
                        } },
                    } },
                } },
            } },
            new RulePart() { Text = "at least two", Children = new List<RulePart>() {
                new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                    new RulePart() { Text = "Sym1", Rule = RuleEnum.AtLeastTwoSym1 },
                    new RulePart() { Text = "Sym2", Rule = RuleEnum.AtLeastTwoSym2 },
                    new RulePart() { Text = "Sym3", Rule = RuleEnum.AtLeastTwoSym3 },
                } },
                new RulePart() { Text = "coloSymCol1", Children = new List<RulePart>() {
                    new RulePart() { Text = "SymCol1", Rule = RuleEnum.AtLeastTwoSymCol1 },
                    new RulePart() { Text = "SymCol2", Rule = RuleEnum.AtLeastTwoSymCol2 },
                    new RulePart() { Text = "SymCol3", Rule = RuleEnum.AtLeastTwoSymCol3 },
                } },
                new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                    new RulePart() { Text = "up", Rule = RuleEnum.AtLeastTwoPat1 },
                    new RulePart() { Text = "left", Rule = RuleEnum.AtLeastTwoPat2 },
                    new RulePart() { Text = "right", Rule = RuleEnum.AtLeastTwoPat3 },
                } },
            } },
            new RulePart() { Text = "Zero tiles", Children = new List<RulePart>() {
                new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                    new RulePart() { Text = "Sym1", Rule = RuleEnum.ZeroSym1 },
                    new RulePart() { Text = "Sym2", Rule = RuleEnum.ZeroSym2 },
                    new RulePart() { Text = "Sym3", Rule = RuleEnum.ZeroSym3 },
                } },
                new RulePart() { Text = "coloSymCol1", Children = new List<RulePart>() {
                    new RulePart() { Text = "SymCol1", Rule = RuleEnum.ZeroSymCol1 },
                    new RulePart() { Text = "SymCol2", Rule = RuleEnum.ZeroSymCol2 },
                    new RulePart() { Text = "SymCol3", Rule = RuleEnum.ZeroSymCol3 },
                } },
                new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                    new RulePart() { Text = "up", Rule = RuleEnum.ZeroPat1 },
                    new RulePart() { Text = "left", Rule = RuleEnum.ZeroPat2 },
                    new RulePart() { Text = "right", Rule = RuleEnum.ZeroPat3 },
                } },
            } },
        }
        };
        _activeRulePart = _ruleTree;

        // Pick random rule
        _masterRule = (RuleEnum)Rnd.Range(0, Enum.GetValues(typeof(RuleEnum)).Length);
        Debug.Log("Random rule: " + _rules[_masterRule].Text);

        // Search for random configuration that matches the rule
        do _config = RandomConfig();
        while (!_rules[_masterRule].Check(_config));
        _followsRule = _config;
        _quizzedConfigs.Add(_config.Clone());
        Debug.Log("Config that matches the master rule: " + String.Join(", ", _config.Tiles.Select(
            t => t is Tile
            ? t.Color.ToString() + " " + t.Symbol.ToString() + " " + t.Direction.ToString()
            : "(empty)"
        ).ToArray()));

        // Search for random configuration that doesn't match the rule
        do _config = RandomConfig();
        while (_rules[_masterRule].Check(_config));
        _doesNotFollowRule = _config;
        _quizzedConfigs.Add(_config.Clone());
        Debug.Log("Config that doesn't match the master rule: " + String.Join(", ", _config.Tiles.Select(
            t => t is Tile
            ? t.Color.ToString() + " " + t.Symbol.ToString() + " " + t.Direction.ToString()
            : "(empty)"
        ).ToArray()));

        // Some random guesses and a response to disprove
        for (var i = 0; i < 10; i++)
        {
            RuleEnum guessedRule;
            do guessedRule = (RuleEnum)Rnd.Range(0, Enum.GetValues(typeof(RuleEnum)).Length);
            while (guessedRule == _masterRule);
            Debug.Log("Random guess: " + _rules[guessedRule].Text);

            do _config = RandomConfig();
            while (_rules[_masterRule].Check(_config) == _rules[guessedRule].Check(_config));
            Debug.Log("Config that disproves the guess: " + String.Join(", ", _config.Tiles.Select(
                t => t is Tile
                ? t.Color.ToString() + " " + t.Symbol.ToString() + " " + t.Direction.ToString()
                : "(empty)"
            ).ToArray()) + (
                _rules[_masterRule].Check(_config)
                ? ", because it matches the master rule, but not the guessed rule."
                : ", because it matches the guessed rule, but not the master rule.")
            );
        }

        UpdateDisplay();
    }

    private IEnumerator HandleLongPress()
    {
        _isButtonDown = true;
        _isLongPress = false;
        yield return new WaitForSeconds(.5f);
        _isLongPress = true;
    }

    private void HandleButtonUp()
    {
        _isButtonDown = false;

        if (_buttonDownCoroutine != null)
        {
            StopCoroutine(_buttonDownCoroutine);
            _buttonDownCoroutine = null;
        }
    }

    private void PressTile(int i)
    {
        if (_isLongPress)
        {
            if (_config.Tiles[i] == null)
                _config.Tiles[i] = new Tile();
            else
                _config.Tiles[i] = null;
        }
        else
        {
            if (_config.Tiles[i] == null)
                _config.Tiles[i] = new Tile();
            _activeTile = i;
        }
        UpdateDisplay();
    }

    private void PressTileButton(int i)
    {
        if (_activeTile == -1) return;

        var tile = _config.Tiles[_activeTile];
        if (tile == null) return;

        switch (i)
        {
            case 0:
                tile.Symbol = (Symbol)((int)(tile.Symbol + 1) % 3);
                break;
            case 1:
                tile.Color = (Color)((int)(tile.Color + 1) % 3);
                break;
            case 2:
                tile.Direction = (Direction)((int)(tile.Direction + 1) % 3);
                break;
        }

        UpdateDisplay();
    }

    private void PressQuizButton(int i)
    {
        // If long press, just show one of the initial configs
        if (_isLongPress)
        {
            _config = (i == 0 ? _followsRule : _doesNotFollowRule);
            UpdateDisplay();
            return;
        }

        // Flash the button that was the actual answer
        var followsRule = _rules[_masterRule].Check(_config);
        StartCoroutine(FlashCaption(QuizButtons[followsRule ? 0 : 1].transform.Find("Text").GetComponent<TextMesh>()));

        // First check if this config has been quized before
        foreach (var config in _quizzedConfigs)
            if (_config.Equals(config)) return;
        _quizzedConfigs.Add(_config.Clone());

        // Give guess token if guessed correctly
        if ((i == 0 && followsRule) || (i == 1 && !followsRule))
            _guessTokens++;

        // Update display
        UpdateDisplay();
    }

    private void PressGuessButton(int i)
    {
        // No guess tokens, return
        if (_guessTokens == 0) return;

        // Empty button, return
        if (i >= _activeRulePart.Children.Count) return;

        // One step further to define rule
        _activeRulePart = _activeRulePart.Children[i];

        // No child rule parts, rule is finished
        if (_activeRulePart.Children == null)
        {
            if (_activeRulePart.Rule == _masterRule)
            {
                Module.HandlePass();
            }
            else
            {
                _guessTokens--;
                _activeRulePart = _ruleTree;
            }
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        for (var i = 0; i < Tiles.Length; i++)
        {
            var sprite = Tiles[i].transform.Find("Sprite").gameObject;
            sprite.SetActive(false);
            var tile = _config.Tiles[i];
            if (tile is Tile)
            {
                sprite.GetComponent<SpriteRenderer>().sprite = Sprites[(int)tile.Color * 3 + (int)tile.Symbol];
                var rotation = tile.Direction == Direction.Left ? -90f : (tile.Direction == Direction.Right ? 90f : 0f);
                sprite.transform.localEulerAngles = new Vector3(90, rotation, 0);
                sprite.SetActive(true);
            }
        }

        GuessTokens.text = _guessTokens.ToString();

        foreach (var button in GuessButtons)
        {
            button.transform.Find("Text").GetComponent<TextMesh>().text = "";
        }

        if (_guessTokens > 0 && _activeRulePart.Children != null)
        {
            for (var i = 0; i < _activeRulePart.Children.Count; i++)
            {
                var text = _activeRulePart.Children[i].Text;
                if (_activeRulePart.Children[i].Children != null) text += " ...";
                GuessButtons[i].transform.Find("Text").GetComponent<TextMesh>().text = text;
            }
        }
    }

    private Config RandomConfig()
    {
        Config config = new Config() { Tiles = new List<Tile>() { null, null, null, null } };

        // Less chance on 1 or 4 tiles, more chance on 2 or 3 tiles
        var numTiles = (new List<int>() { 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 4 })[Rnd.Range(0, 12)];
        for (var i = 0; i < numTiles; i++)
        {
            config.Tiles[i] = new Tile()
            {
                Symbol = (Symbol)Rnd.Range(0, 3),
                SymbolColor = (Color)Rnd.Range(0, 3),
                Pattern = (Direction)Rnd.Range(0, 3)
            };
        }

        // Random position
        config.Tiles = config.Tiles.Shuffle();

        return config;
    }

    private IEnumerator FlashCaption(TextMesh text)
    {
        const float durationPerPing = .3f;
        var original = text.color;

        bool forward = true;
        for (var i = 0; i < 3; i++)
        {
            for (float time = 0f; time < durationPerPing; time += Time.deltaTime)
            {
                yield return null;

                float f = Mathf.SmoothStep(forward ? 0 : 1, forward ? 1 : 0, time / durationPerPing);
                text.color = new UnityEngine.Color(f, f, f);
            }
            forward = !forward;
        }
        text.color = original;
    }

    class Tile
    {
        public int[] Properties { get; set; }
        public Tile()
        {
            this.Properties = new int[] { 0, 0, 0, 0 };
        }
    }

    class Config
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
                    if (Tiles[i].Symbol != config.Tiles[i].Symbol) return false;
                    if (Tiles[i].Color != config.Tiles[i].Color) return false;
                    if (Tiles[i].Direction != config.Tiles[i].Direction) return false;
                }
            }
            return true;
        }
    }

    class Rule
    {
        public Func<Config, bool> Check { get; set; }
    }

    class RulePart
    {
        public string Text { get; set; }
        public List<RulePart> Children { get; set; }
        public string Rule { get; set; }
    }
}
