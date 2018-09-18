using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KmHelper;
using Rnd = UnityEngine.Random;

public class Zendo : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMSelectable Module;
    public KMSelectable[] Buttons;

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
        { RuleEnum.AllThreeColors, new Rule() { Text = "all three colors", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Red)
                && c.Tiles.Any(t => t.Color == Color.White)
                && c.Tiles.Any(t => t.Color == Color.Yellow);
        } } },
        { RuleEnum.AllThreeSymbols, new Rule() { Text = "all three symbols", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Buddha)
                && c.Tiles.Any(t => t.Symbol == Symbol.Lotus)
                && c.Tiles.Any(t => t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AllThreeDirections, new Rule() { Text = "all three directions", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Up)
                && c.Tiles.Any(t => t.Direction == Direction.Left)
                && c.Tiles.Any(t => t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneBuddha, new Rule() { Text = "at least one Buddha", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneRedBuddha, new Rule() { Text = "at least one red Buddha", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Red && t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneWhiteBuddha, new Rule() { Text = "at least one white Buddha", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.White && t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneYellowBuddha, new Rule() { Text = "at least one yellow Buddha", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Yellow && t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneBuddhaPointingUp, new Rule() { Text = "at least one Buddha pointing up", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Up && t.Symbol == Symbol.Buddha);
        } } },
        { RuleEnum.AtLeastOneBuddhaPointingLeft, new Rule() { Text = "at least one Buddha pointing left", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Buddha && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneBuddhaPointingRight, new Rule() { Text = "at least one Buddha pointing right", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Buddha && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneLotus, new Rule() { Text = "at least one lotus", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneRedLotus, new Rule() { Text = "at least one red lotus", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Red && t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneWhiteLotus, new Rule() { Text = "at least one white lotus", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.White && t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneYellowLotus, new Rule() { Text = "at least one yellow lotus", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Yellow && t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneLotusPointingUp, new Rule() { Text = "at least one lotus pointing up", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Up && t.Symbol == Symbol.Lotus);
        } } },
        { RuleEnum.AtLeastOneLotusPointingLeft, new Rule() { Text = "at least one lotus pointint left", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Lotus && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneLotusPointingRight, new Rule() { Text = "at least one lotus pointing right", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Lotus && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneShrine, new Rule() { Text = "at least one shrine", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneRedShrine, new Rule() { Text = "at least one red shrine", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Red && t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneWhiteShrine, new Rule() { Text = "at least one white shrine", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.White && t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneYellowShrine, new Rule() { Text = "at least one yellow shrine", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Yellow && t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneShrinePointingUp, new Rule() { Text = "at least one shrine pointing up", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Up && t.Symbol == Symbol.Shrine);
        } } },
        { RuleEnum.AtLeastOneShrinePointingLeft, new Rule() { Text = "at least one shrine pointing left", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Shrine && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneShrinePointingRight, new Rule() { Text = "at least one shrine pointing right", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Symbol == Symbol.Shrine && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneRed, new Rule() { Text = "at least one red", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Red);
        } } },
        { RuleEnum.AtLeastOneRedPointingUp, new Rule() { Text = "at least one red pointing up", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Up && t.Color == Color.Red);
        } } },
        { RuleEnum.AtLeastOneRedPointingLeft, new Rule() { Text = "at least one red pointing left", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Red && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneRedPointingRight, new Rule() { Text = "at least one red pointing right", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Red && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneWhite, new Rule() { Text = "at least one white", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.White);
        } } },
        { RuleEnum.AtLeastOneWhitePointingUp, new Rule() { Text = "at least one white pointing up", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Up && t.Color == Color.White);
        } } },
        { RuleEnum.AtLeastOneWhitePointingLeft, new Rule() { Text = "at least one white pointing left", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.White && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneWhitePointingRight, new Rule() { Text = "at least one white pointing right", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.White && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOneYellow, new Rule() { Text = "at least one yellow", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Yellow);
        } } },
        { RuleEnum.AtLeastOneYellowPointingUp, new Rule() { Text = "at least one yellow pointing up", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Up && t.Color == Color.Yellow);
        } } },
        { RuleEnum.AtLeastOneYellowPointingLeft, new Rule() { Text = "at least one yellow pointing left", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Yellow && t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOneYellowPointingRight, new Rule() { Text = "at least one yellow pointing right", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Color == Color.Yellow && t.Direction == Direction.Right);
        } } },
        { RuleEnum.AtLeastOnePointingUp, new Rule() { Text = "at least one pointing up", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Up);
        } } },
        { RuleEnum.AtLeastOnePointingLeft, new Rule() { Text = "at least one pointing left", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Left);
        } } },
        { RuleEnum.AtLeastOnePointingRight, new Rule() { Text = "at least one pointing right", Check = (Configuration c) => {
            return c.Tiles.Any(t => t.Direction == Direction.Right);
        } } },
        { RuleEnum.ExactlyOneBuddha, new Rule() { Text = "exactly one Buddha", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Symbol == Symbol.Buddha) == 1;
        } } },
        { RuleEnum.ExactlyOneLotus, new Rule() { Text = "exactly one lotus", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Symbol == Symbol.Lotus) == 1;
        } } },
        { RuleEnum.ExactlyOneShrine, new Rule() { Text = "exactly one shrine", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Symbol == Symbol.Shrine) == 1;
        } } },
        { RuleEnum.ExactlyOneRed, new Rule() { Text = "exactly one red", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Color == Color.Red) == 1;
        } } },
        { RuleEnum.ExactlyOneWhite, new Rule() { Text = "exactly one white", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Color == Color.White) == 1;
        } } },
        { RuleEnum.ExactlyOneYellow, new Rule() { Text = "exactly one yellow", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Color == Color.Yellow) == 1;
        } } },
        { RuleEnum.ExactlyOnePointingUp, new Rule() { Text = "exactly one pointing up", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Direction == Direction.Up) == 1;
        } } },
        { RuleEnum.ExactlyOnePointingLeft, new Rule() { Text = "exactly one pointing left", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Direction == Direction.Left) == 1;
        } } },
        { RuleEnum.ExactlyOnePointingRight, new Rule() { Text = "exactly one pointing right", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Direction == Direction.Right) == 1;
        } } },
        { RuleEnum.ZeroBuddha, new Rule() { Text = "zero Buddha", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Symbol == Symbol.Buddha) == 0;
        } } },
        { RuleEnum.ZeroLotus, new Rule() { Text = "zero lotus", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Symbol == Symbol.Lotus) == 0;
        } } },
        { RuleEnum.ZeroShrine, new Rule() { Text = "zero shrine", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Symbol == Symbol.Shrine) == 0;
        } } },
        { RuleEnum.ZeroRed, new Rule() { Text = "zero red", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Color == Color.Red) == 0;
        } } },
        { RuleEnum.ZeroWhite, new Rule() { Text = "zero white", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Color == Color.White) == 0;
        } } },
        { RuleEnum.ZeroYellow, new Rule() { Text = "zero yellow", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Color == Color.Yellow) == 0;
        } } },
        { RuleEnum.ZeroPointingUp, new Rule() { Text = "zero pointing up", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Direction == Direction.Up) == 0;
        } } },
        { RuleEnum.ZeroPointingLeft, new Rule() { Text = "zero pointing left", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Direction == Direction.Left) == 0;
        } } },
        { RuleEnum.ZeroPointingRight, new Rule() { Text = "zero pointing right", Check = (Configuration c) => {
            return c.Tiles.Count(t => t.Direction == Direction.Right) == 0;
        } } },
    };

    private List<RulePart> _ruleParts;

    private enum Color { Red, White, Yellow }
    private enum Symbol { Buddha, Lotus, Shrine }
    private enum Direction { Up, Left, Right }

    void Start()
    {
        _moduleId = _moduleIdCounter++;

        for (int i = 0; i < Buttons.Length; i++)
        {
            var j = i;
            Buttons[i].OnInteract += delegate () { PressButton(j); return false; };
        }

        _ruleParts = new List<RulePart>() {
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
        };
    }

    private void PressButton(int i)
    {
        throw new NotImplementedException();
    }

    void Update()
    {

    }

    class Tile
    {
        public Color Color { get; set; }
        public Symbol Symbol { get; set; }
        public Direction Direction { get; set; }
    }

    class Configuration
    {
        public List<Tile> Tiles { get; set; }
    }

    class Rule
    {
        public string Text { get; set; }
        public Func<Configuration, bool> Check { get; set; }
    }

    class RulePart
    {
        public string Text { get; set; }
        public List<RulePart> Children { get; set; }
        public Rule Rule { get; set; }
    }
}
