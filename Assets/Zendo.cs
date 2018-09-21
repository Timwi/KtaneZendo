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
    public KMSelectable Module;
    public Sprite[] Sprites;
    public KMSelectable[] Tiles;
    public KMSelectable[] TileButtons;
    public KMSelectable[] QuizButtons;
    public KMSelectable[] GuessButtons;
    public TextMesh GuessTokens;

    private int _moduleId;
    private static int _moduleIdCounter = 1;

    private enum RuleEnum
    {
        AllThreeColors,
        AllThreeSymbols,
        AllThreeDirections,
        AtLeastOneBuddha,
        AtLeastOneRedBuddha,
        AtLeastOneWhiteBuddha,
        AtLeastOneYellowBuddha,
        AtLeastOneBuddhaPointingUp,
        AtLeastOneBuddhaPointingLeft,
        AtLeastOneBuddhaPointingRight,
        AtLeastOneLotus,
        AtLeastOneRedLotus,
        AtLeastOneWhiteLotus,
        AtLeastOneYellowLotus,
        AtLeastOneLotusPointingUp,
        AtLeastOneLotusPointingLeft,
        AtLeastOneLotusPointingRight,
        AtLeastOneShrine,
        AtLeastOneRedShrine,
        AtLeastOneWhiteShrine,
        AtLeastOneYellowShrine,
        AtLeastOneShrinePointingUp,
        AtLeastOneShrinePointingLeft,
        AtLeastOneShrinePointingRight,
        AtLeastOneRed,
        AtLeastOneRedPointingUp,
        AtLeastOneRedPointingLeft,
        AtLeastOneRedPointingRight,
        AtLeastOneWhite,
        AtLeastOneWhitePointingUp,
        AtLeastOneWhitePointingLeft,
        AtLeastOneWhitePointingRight,
        AtLeastOneYellow,
        AtLeastOneYellowPointingUp,
        AtLeastOneYellowPointingLeft,
        AtLeastOneYellowPointingRight,
        AtLeastOnePointingUp,
        AtLeastOnePointingLeft,
        AtLeastOnePointingRight,
        ExactlyOneBuddha,
        ExactlyOneLotus,
        ExactlyOneShrine,
        ExactlyOneRed,
        ExactlyOneWhite,
        ExactlyOneYellow,
        ExactlyOnePointingUp,
        ExactlyOnePointingLeft,
        ExactlyOnePointingRight,
        ZeroBuddha,
        ZeroLotus,
        ZeroShrine,
        ZeroRed,
        ZeroWhite,
        ZeroYellow,
        ZeroPointingUp,
        ZeroPointingLeft,
        ZeroPointingRight,
    }

    private Dictionary<RuleEnum, Rule> _rules = new Dictionary<RuleEnum, Rule>()
    {
        { RuleEnum.AllThreeColors, new Rule() { Text = "all three colors", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Red)
                && c.Tiles.OfType<Tile>().Any(t => t.Color == Color.White)
                && c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Yellow);
        } } },
        { RuleEnum.AllThreeSymbols, new Rule() { Text = "all three symbols", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().OfType<Tile>().Any(t => t.Symbol == Symbol.Buddha)
                && c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Lotus)
                && c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AllThreeDirections, new Rule() { Text = "all three directions", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up)
                && c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Left)
                && c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneBuddha, new Rule() { Text = "at least one Buddha", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneRedBuddha, new Rule() { Text = "at least one red Buddha", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Red && t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneWhiteBuddha, new Rule() { Text = "at least one white Buddha", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.White && t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneYellowBuddha, new Rule() { Text = "at least one yellow Buddha", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Yellow && t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneBuddhaPointingUp, new Rule() { Text = "at least one Buddha pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneBuddhaPointingLeft, new Rule() { Text = "at least one Buddha pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Buddha && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneBuddhaPointingRight, new Rule() { Text = "at least one Buddha pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Buddha && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneLotus, new Rule() { Text = "at least one lotus", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneRedLotus, new Rule() { Text = "at least one red lotus", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Red && t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneWhiteLotus, new Rule() { Text = "at least one white lotus", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.White && t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneYellowLotus, new Rule() { Text = "at least one yellow lotus", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Yellow && t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneLotusPointingUp, new Rule() { Text = "at least one lotus pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneLotusPointingLeft, new Rule() { Text = "at least one lotus pointint left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Lotus && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneLotusPointingRight, new Rule() { Text = "at least one lotus pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Lotus && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneShrine, new Rule() { Text = "at least one shrine", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneRedShrine, new Rule() { Text = "at least one red shrine", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Red && t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneWhiteShrine, new Rule() { Text = "at least one white shrine", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.White && t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneYellowShrine, new Rule() { Text = "at least one yellow shrine", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Yellow && t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneShrinePointingUp, new Rule() { Text = "at least one shrine pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneShrinePointingLeft, new Rule() { Text = "at least one shrine pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Shrine && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneShrinePointingRight, new Rule() { Text = "at least one shrine pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Symbol == Symbol.Shrine && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneRed, new Rule() { Text = "at least one red", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Red);
        } } },
        { RuleEnum.AtLeastOneRedPointingUp, new Rule() { Text = "at least one red pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Color == Color.Red);
        } } },
        { RuleEnum.AtLeastOneRedPointingLeft, new Rule() { Text = "at least one red pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Red && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneRedPointingRight, new Rule() { Text = "at least one red pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Red && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneWhite, new Rule() { Text = "at least one white", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.White);
        } } },
        { RuleEnum.AtLeastOneWhitePointingUp, new Rule() { Text = "at least one white pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Color == Color.White);
        } } },
        { RuleEnum.AtLeastOneWhitePointingLeft, new Rule() { Text = "at least one white pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.White && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneWhitePointingRight, new Rule() { Text = "at least one white pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.White && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneYellow, new Rule() { Text = "at least one yellow", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Yellow);
        } } },
        { RuleEnum.AtLeastOneYellowPointingUp, new Rule() { Text = "at least one yellow pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up && t.Color == Color.Yellow);
        } } },
        { RuleEnum.AtLeastOneYellowPointingLeft, new Rule() { Text = "at least one yellow pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Yellow && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneYellowPointingRight, new Rule() { Text = "at least one yellow pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Color == Color.Yellow && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOnePointingUp, new Rule() { Text = "at least one pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Up);
        } } },
        { RuleEnum.AtLeastOnePointingLeft, new Rule() { Text = "at least one pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOnePointingRight, new Rule() { Text = "at least one pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Any(t => t.Direction == Direction.Right);
        } } },
        { RuleEnum.ExactlyOneBuddha, new Rule() { Text = "exactly one Buddha", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Buddha) == 1;
        } } },
        { RuleEnum.ExactlyOneLotus, new Rule() { Text = "exactly one lotus", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Lotus) == 1;
        } } },
        { RuleEnum.ExactlyOneShrine, new Rule() { Text = "exactly one shrine", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Shrine) == 1;
        } } },
        { RuleEnum.ExactlyOneRed, new Rule() { Text = "exactly one red", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.Red) == 1;
        } } },
        { RuleEnum.ExactlyOneWhite, new Rule() { Text = "exactly one white", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.White) == 1;
        } } },
        { RuleEnum.ExactlyOneYellow, new Rule() { Text = "exactly one yellow", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.Yellow) == 1;
        } } },
        { RuleEnum.ExactlyOnePointingUp, new Rule() { Text = "exactly one pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Up) == 1;
        } } },
        { RuleEnum.ExactlyOnePointingLeft, new Rule() { Text = "exactly one pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Left) == 1;
        } } },
        { RuleEnum.ExactlyOnePointingRight, new Rule() { Text = "exactly one pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Right) == 1;
        } } },
        { RuleEnum.ZeroBuddha, new Rule() { Text = "zero Buddha", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Buddha) == 0;
        } } },
        { RuleEnum.ZeroLotus, new Rule() { Text = "zero lotus", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Lotus) == 0;
        } } },
        { RuleEnum.ZeroShrine, new Rule() { Text = "zero shrine", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Symbol == Symbol.Shrine) == 0;
        } } },
        { RuleEnum.ZeroRed, new Rule() { Text = "zero red", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.Red) == 0;
        } } },
        { RuleEnum.ZeroWhite, new Rule() { Text = "zero white", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.White) == 0;
        } } },
        { RuleEnum.ZeroYellow, new Rule() { Text = "zero yellow", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Color == Color.Yellow) == 0;
        } } },
        { RuleEnum.ZeroPointingUp, new Rule() { Text = "zero pointing up", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Up) == 0;
        } } },
        { RuleEnum.ZeroPointingLeft, new Rule() { Text = "zero pointing left", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Left) == 0;
        } } },
        { RuleEnum.ZeroPointingRight, new Rule() { Text = "zero pointing right", Check = (Config c) => {
            return c.Tiles.OfType<Tile>().Count(t => t.Direction == Direction.Right) == 0;
        } } },
    };

    private RulePart _ruleTree;

    private enum Color { Red, White, Yellow }
    private enum Symbol { Buddha, Lotus, Shrine }
    private enum Direction { Up, Right, Left }

    private RuleEnum _masterRule;
    private Config _config;
    private Config _followsRule;
    private Config _doesNotFollowRule;
    private List<Config> _quizedConfigs = new List<Config>();
    private RulePart _rulePart;
    private KMSelectable _previousButton;
    private int _activeTile = -1;
    private int _guessTokens = 0;

    void Start()
    {
        _moduleId = _moduleIdCounter++;

        for (int i = 0; i < Tiles.Length; i++)
        {
            var j = i;
            Tiles[i].OnInteract += delegate () { PressTile(j); return false; };
        }

        for (int i = 0; i < TileButtons.Length; i++)
        {
            var j = i;
            TileButtons[i].OnInteract += delegate () { PressTileButton(j); return false; };
        }

        for (int i = 0; i < QuizButtons.Length; i++)
        {
            var j = i;
            QuizButtons[i].OnInteract += delegate () { PressQuizButton(j); return false; };
        }

        for (int i = 0; i < GuessButtons.Length; i++)
        {
            var j = i;
            GuessButtons[i].OnInteract += delegate () { PressGuessButton(j); return false; };
        }

        _ruleTree = new RulePart() { Children = new List<RulePart>() {
            new RulePart() { Text = "all three", Children = new List<RulePart>() {
                new RulePart() { Text = "colors.", Rule = _rules[RuleEnum.AllThreeColors] },
                new RulePart() { Text = "symbols.", Rule = _rules[RuleEnum.AllThreeSymbols] },
                new RulePart() { Text = "directions.", Rule = _rules[RuleEnum.AllThreeDirections] },
            } },
            new RulePart() { Text = "at least one tile", Children = new List<RulePart>() {
                new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                    new RulePart() { Text = "Buddha", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = _rules[RuleEnum.AtLeastOneBuddha] },
                        new RulePart() { Text = "colored", Children = new List<RulePart>() {
                            new RulePart() { Text = "red", Rule = _rules[RuleEnum.AtLeastOneRedBuddha] },
                            new RulePart() { Text = "white", Rule = _rules[RuleEnum.AtLeastOneWhiteBuddha] },
                            new RulePart() { Text = "yellow", Rule = _rules[RuleEnum.AtLeastOneYellowBuddha] },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = _rules[RuleEnum.AtLeastOneBuddhaPointingUp] },
                            new RulePart() { Text = "left", Rule = _rules[RuleEnum.AtLeastOneBuddhaPointingLeft] },
                            new RulePart() { Text = "right", Rule = _rules[RuleEnum.AtLeastOneBuddhaPointingRight] },
                        } },
                    } },
                    new RulePart() { Text = "lotus", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = _rules[RuleEnum.AtLeastOneLotus] },
                        new RulePart() { Text = "colored", Children = new List<RulePart>() {
                            new RulePart() { Text = "red", Rule = _rules[RuleEnum.AtLeastOneRedLotus] },
                            new RulePart() { Text = "white", Rule = _rules[RuleEnum.AtLeastOneWhiteLotus] },
                            new RulePart() { Text = "yellow", Rule = _rules[RuleEnum.AtLeastOneYellowLotus] },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = _rules[RuleEnum.AtLeastOneLotusPointingUp] },
                            new RulePart() { Text = "left", Rule = _rules[RuleEnum.AtLeastOneLotusPointingLeft] },
                            new RulePart() { Text = "right", Rule = _rules[RuleEnum.AtLeastOneLotusPointingRight] },
                        } },
                    } },
                    new RulePart() { Text = "shrine", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = _rules[RuleEnum.AtLeastOneShrine] },
                        new RulePart() { Text = "colored", Children = new List<RulePart>() {
                            new RulePart() { Text = "red", Rule = _rules[RuleEnum.AtLeastOneRedShrine] },
                            new RulePart() { Text = "white", Rule = _rules[RuleEnum.AtLeastOneWhiteShrine] },
                            new RulePart() { Text = "yellow", Rule = _rules[RuleEnum.AtLeastOneYellowShrine] },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = _rules[RuleEnum.AtLeastOneShrinePointingUp] },
                            new RulePart() { Text = "left", Rule = _rules[RuleEnum.AtLeastOneShrinePointingLeft] },
                            new RulePart() { Text = "right", Rule = _rules[RuleEnum.AtLeastOneShrinePointingRight] },
                        } },
                    } },
                } },
                new RulePart() { Text = "colored", Children = new List<RulePart>() {
                    new RulePart() { Text = "red", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = _rules[RuleEnum.AtLeastOneRed] },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Buddha", Rule = _rules[RuleEnum.AtLeastOneRedBuddha] },
                            new RulePart() { Text = "lotus", Rule = _rules[RuleEnum.AtLeastOneRedLotus] },
                            new RulePart() { Text = "shrine", Rule = _rules[RuleEnum.AtLeastOneRedShrine] },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = _rules[RuleEnum.AtLeastOneRedPointingUp] },
                            new RulePart() { Text = "left", Rule = _rules[RuleEnum.AtLeastOneRedPointingLeft] },
                            new RulePart() { Text = "right", Rule = _rules[RuleEnum.AtLeastOneRedPointingRight] },
                        } },
                    } },
                    new RulePart() { Text = "white", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = _rules[RuleEnum.AtLeastOneWhite] },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Buddha", Rule = _rules[RuleEnum.AtLeastOneWhiteBuddha] },
                            new RulePart() { Text = "lotus", Rule = _rules[RuleEnum.AtLeastOneWhiteLotus] },
                            new RulePart() { Text = "shrine", Rule = _rules[RuleEnum.AtLeastOneWhiteShrine] },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = _rules[RuleEnum.AtLeastOneWhitePointingUp] },
                            new RulePart() { Text = "left", Rule = _rules[RuleEnum.AtLeastOneWhitePointingLeft] },
                            new RulePart() { Text = "right", Rule = _rules[RuleEnum.AtLeastOneWhitePointingRight] },
                        } },
                    } },
                    new RulePart() { Text = "yellow", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = _rules[RuleEnum.AtLeastOneYellow] },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Buddha", Rule = _rules[RuleEnum.AtLeastOneYellowBuddha] },
                            new RulePart() { Text = "lotus", Rule = _rules[RuleEnum.AtLeastOneYellowLotus] },
                            new RulePart() { Text = "shrine", Rule = _rules[RuleEnum.AtLeastOneYellowShrine] },
                        } },
                        new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                            new RulePart() { Text = "up", Rule = _rules[RuleEnum.AtLeastOneYellowPointingUp] },
                            new RulePart() { Text = "left", Rule = _rules[RuleEnum.AtLeastOneYellowPointingLeft] },
                            new RulePart() { Text = "right", Rule = _rules[RuleEnum.AtLeastOneYellowPointingRight] },
                        } },
                    } },
                } },
                new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                    new RulePart() { Text = "up", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = _rules[RuleEnum.AtLeastOnePointingUp] },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Buddha", Rule = _rules[RuleEnum.AtLeastOneBuddhaPointingUp] },
                            new RulePart() { Text = "lotus", Rule = _rules[RuleEnum.AtLeastOneLotusPointingUp] },
                            new RulePart() { Text = "shrine", Rule = _rules[RuleEnum.AtLeastOneShrinePointingUp] },
                        } },
                        new RulePart() { Text = "colored", Children = new List<RulePart>() {
                            new RulePart() { Text = "red", Rule = _rules[RuleEnum.AtLeastOneRedPointingUp] },
                            new RulePart() { Text = "white", Rule = _rules[RuleEnum.AtLeastOneWhitePointingUp] },
                            new RulePart() { Text = "yellow", Rule = _rules[RuleEnum.AtLeastOneYellowPointingUp] },
                        } },
                    } },
                    new RulePart() { Text = "left", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = _rules[RuleEnum.AtLeastOnePointingLeft] },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Buddha", Rule = _rules[RuleEnum.AtLeastOneBuddhaPointingLeft] },
                            new RulePart() { Text = "lotus", Rule = _rules[RuleEnum.AtLeastOneLotusPointingLeft] },
                            new RulePart() { Text = "shrine", Rule = _rules[RuleEnum.AtLeastOneShrinePointingLeft] },
                        } },
                        new RulePart() { Text = "colored", Children = new List<RulePart>() {
                            new RulePart() { Text = "red", Rule = _rules[RuleEnum.AtLeastOneRedPointingLeft] },
                            new RulePart() { Text = "white", Rule = _rules[RuleEnum.AtLeastOneWhitePointingLeft] },
                            new RulePart() { Text = "yellow", Rule = _rules[RuleEnum.AtLeastOneYellowPointingLeft] },
                        } },
                    } },
                    new RulePart() { Text = "right", Children = new List<RulePart>() {
                        new RulePart() { Text = "(done)", Rule = _rules[RuleEnum.AtLeastOnePointingRight] },
                        new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                            new RulePart() { Text = "Buddha", Rule = _rules[RuleEnum.AtLeastOneBuddhaPointingRight] },
                            new RulePart() { Text = "lotus", Rule = _rules[RuleEnum.AtLeastOneLotusPointingRight] },
                            new RulePart() { Text = "shrine", Rule = _rules[RuleEnum.AtLeastOneShrinePointingRight] },
                        } },
                        new RulePart() { Text = "colored", Children = new List<RulePart>() {
                            new RulePart() { Text = "red", Rule = _rules[RuleEnum.AtLeastOneRedPointingRight] },
                            new RulePart() { Text = "white", Rule = _rules[RuleEnum.AtLeastOneWhitePointingRight] },
                            new RulePart() { Text = "yellow", Rule = _rules[RuleEnum.AtLeastOneYellowPointingRight] },
                        } },
                    } },
                } },
            } },
            new RulePart() { Text = "exactly one tile", Children = new List<RulePart>() {
                new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                    new RulePart() { Text = "Buddha", Rule = _rules[RuleEnum.ExactlyOneBuddha] },
                    new RulePart() { Text = "lotus", Rule = _rules[RuleEnum.ExactlyOneLotus] },
                    new RulePart() { Text = "shrine", Rule = _rules[RuleEnum.ExactlyOneShrine] },
                } },
                new RulePart() { Text = "colored", Children = new List<RulePart>() {
                    new RulePart() { Text = "red", Rule = _rules[RuleEnum.ExactlyOneRed] },
                    new RulePart() { Text = "white", Rule = _rules[RuleEnum.ExactlyOneWhite] },
                    new RulePart() { Text = "yellow", Rule = _rules[RuleEnum.ExactlyOneYellow] },
                } },
                new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                    new RulePart() { Text = "up", Rule = _rules[RuleEnum.ExactlyOnePointingUp] },
                    new RulePart() { Text = "left", Rule = _rules[RuleEnum.ExactlyOnePointingLeft] },
                    new RulePart() { Text = "right", Rule = _rules[RuleEnum.ExactlyOnePointingRight] },
                } },
            } },
            new RulePart() { Text = "zero tiles", Children = new List<RulePart>() {
                new RulePart() { Text = "with symbol", Children = new List<RulePart>() {
                    new RulePart() { Text = "Buddha", Rule = _rules[RuleEnum.ZeroBuddha] },
                    new RulePart() { Text = "lotus", Rule = _rules[RuleEnum.ZeroLotus] },
                    new RulePart() { Text = "shrine", Rule = _rules[RuleEnum.ZeroShrine] },
                } },
                new RulePart() { Text = "colored", Children = new List<RulePart>() {
                    new RulePart() { Text = "red", Rule = _rules[RuleEnum.ZeroRed] },
                    new RulePart() { Text = "white", Rule = _rules[RuleEnum.ZeroWhite] },
                    new RulePart() { Text = "yellow", Rule = _rules[RuleEnum.ZeroYellow] },
                } },
                new RulePart() { Text = "pointing", Children = new List<RulePart>() {
                    new RulePart() { Text = "up", Rule = _rules[RuleEnum.ZeroPointingUp] },
                    new RulePart() { Text = "left", Rule = _rules[RuleEnum.ZeroPointingLeft] },
                    new RulePart() { Text = "right", Rule = _rules[RuleEnum.ZeroPointingRight] },
                } },
            } },
        } };

        // Pick random rule
        _masterRule = (RuleEnum)Rnd.Range(0, Enum.GetValues(typeof(RuleEnum)).Length);
        Debug.Log("Random rule: " + _rules[_masterRule].Text);

        // Search for random configuration that matches the rule
        do _config = RandomConfig();
        while (!_rules[_masterRule].Check(_config));
        _followsRule = _config;
        _quizedConfigs.Add(_config);
        Debug.Log("Config that matches the master rule: " + String.Join(", ", _config.Tiles.Select(
            t => t is Tile
            ? t.Color.ToString() + " " + t.Symbol.ToString() + " " + t.Direction.ToString()
            : "(empty)"
        ).ToArray()));

        // Search for random configuration that doesn't match the rule
        do _config = RandomConfig();
        while (_rules[_masterRule].Check(_config));
        _doesNotFollowRule = _config;
        _quizedConfigs.Add(_config);
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

    private bool RegisterDoubleClick(KMSelectable button)
    {
        if (_previousButton == button)
        {
            _previousButton = null;
            return true;
        }
        _previousButton = button;
        return false;
    }

    private void PressTile(int i)
    {
        if (RegisterDoubleClick(Tiles[i]))
        {
            if (_config.Tiles[i] == null)
                _config.Tiles[i] = new Tile();
            else
                _config.Tiles[i] = null;
        }
        else
        {
            _activeTile = i;
        }
        UpdateDisplay();
    }

    private void PressTileButton(int i)
    {
        RegisterDoubleClick(TileButtons[i]);

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
        // If double click, just show one of the initial configs
        if (RegisterDoubleClick(QuizButtons[i]))
        {
            _config = (i == 0 ? _followsRule : _doesNotFollowRule);
            UpdateDisplay();
            return;
        }

        // First check if this config has been quized before
        foreach (var config in _quizedConfigs)
        {
            if (_config.Equals(config)) return;
        }
        _quizedConfigs.Add(_config);

        // Check if the guessed correctly
        var followsRule = _rules[_masterRule].Check(_config);
        if ((i == 0 && followsRule) || (i == 1 && !followsRule))
        {
            _guessTokens++;
        }

        // Update display
        UpdateDisplay();
    }

    private void PressGuessButton(int i)
    {
        RegisterDoubleClick(GuessButtons[i]);
        throw new NotImplementedException();
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
                Color = (Color)Rnd.Range(0, 3),
                Direction = (Direction)Rnd.Range(0, 3)
            };
        }

        // Random position
        config.Tiles = config.Tiles.Shuffle();

        return config;
    }

    class Tile
    {
        public Color Color { get; set; }
        public Symbol Symbol { get; set; }
        public Direction Direction { get; set; }
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
        public string Text { get; set; }
        public Func<Config, bool> Check { get; set; }
    }

    class RulePart
    {
        public string Text { get; set; }
        public List<RulePart> Children { get; set; }
        public Rule Rule { get; set; }
    }
}
