using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KmHelper;
using Rnd = UnityEngine.Random;

public class Zendo : MonoBehaviour
{
    enum Color { Red, Yellow, Green }
    enum Symbol { Buddha, Lotus, Shrine }
    enum Orientation { Upright, Sideways, UpsideDown }

    public KMBombInfo Bomb;
    public KMSelectable Module;
    public KMSelectable[] Buttons;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private List<RulePart> _ruleParts;

    void Start()
    {
        _moduleId = _moduleIdCounter++;

        for (int i = 0; i < Buttons.Length; i++)
        {
            var j = i;
            Buttons[i].OnInteract += delegate () { PressButton(j); return false; };
        }

        _ruleParts = new List<RulePart>() {
            new RulePart() {
                Text = "all three",
                Children = new List<RulePart>() {
                    new RulePart() {
                        Text = "colors.",
                        Rule = new Rule() {
                            Text = "all three colors.",
                            Check = (Structure s) => {
                                return s.Pieces.Any(p => p.Color == Color.Green)
                                && s.Pieces.Any(p => p.Color == Color.Red)
                                && s.Pieces.Any(p => p.Color == Color.Yellow);
                            }
                        }
                    },
                    new RulePart() {
                        Text = "symbols.",
                        Rule = new Rule() {
                            Text = "all three symbols.",
                            Check = (Structure s) => {
                                return s.Pieces.Any(p => p.Symbol == Symbol.Buddha)
                                && s.Pieces.Any(p => p.Symbol == Symbol.Lotus)
                                && s.Pieces.Any(p => p.Symbol == Symbol.Shrine);
                            }
                        }
                    },
                    new RulePart() {
                        Text = "orientations.",
                        Rule = new Rule() {
                            Text = "all three orientations.",
                            Check = (Structure s) => {
                                return s.Pieces.Any(p => p.Orientation == Orientation.Upright)
                                && s.Pieces.Any(p => p.Orientation == Orientation.Sideways)
                                && s.Pieces.Any(p => p.Orientation == Orientation.UpsideDown);
                            }
                        }
                    },
                }
            },
            new RulePart()
            {
                Text = "at least one piece",
                Children = new List<RulePart>() {
                    new RulePart() {
                        Text = "with symbol",
                        Children = new List<RulePart>() {
                            new RulePart() {
                                Text = "Buddha.",
                                Rule = new Rule() {
                                    Text = "at least one Buddha.",
                                    Check = (Structure s) => {
                                        return s.Pieces.Any(p => p.Symbol == Symbol.Buddha);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private void PressButton(int i)
    {
        throw new NotImplementedException();
    }

    void Update()
    {

    }

    class Piece
    {
        public Color Color { get; set; }
        public Symbol Symbol { get; set; }
        public Orientation Orientation { get; set; }
    }

    class Structure
    {
        public List<Piece> Pieces { get; set; }
    }

    class Rule
    {
        public string Text { get; set; }
        public Func<Structure, bool> Check { get; set; }
    }

    class RulePart
    {
        public string Text { get; set; }
        public List<RulePart> Children { get; set; }
        public Rule Rule { get; set; }
    }
}
