using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KmHelper;
using Rnd = UnityEngine.Random;
using Assets;

public partial class Zendo : MonoBehaviour
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

    private Dictionary<string, Rule> _rules = new Dictionary<string, Rule>();
    private string[] _propertyNames = { "symbol color", "symbol", "pattern color", "pattern" };
    private Rule _masterRule;
    private Config _playerConfig;
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

        /*
        // Define all possible rules
        _rules = new Dictionary<string, Rule>();
        for (var firstProperty = 0; firstProperty < 4; firstProperty++)
        {
            _rules.Add(String.Format("all three {0}s", _propertyNames[firstProperty]), new Rule()
            {
                Check = (Config c) =>
                {
                    return c.Tiles.OfType<Tile>().Any(t => t.Properties[firstProperty] == 1)
                        && c.Tiles.OfType<Tile>().Any(t => t.Properties[firstProperty] == 2)
                        && c.Tiles.OfType<Tile>().Any(t => t.Properties[firstProperty] == 3);
                }
            });

            for (var firstVariant = 1; firstVariant <= 3; firstVariant++)
            {
                _rules.Add(String.Format("at least one with {0} {1}", _propertyNames[firstProperty], firstVariant), new Rule()
                {
                    Check = (Config c) =>
                    {
                        return c.Tiles.OfType<Tile>().Any(t => t.Properties[firstProperty] == firstVariant);
                    }
                });

                _rules.Add(String.Format("at least two with {0} {1}", _propertyNames[firstProperty], firstVariant), new Rule()
                {
                    Check = (Config c) =>
                    {
                        return c.Tiles.OfType<Tile>().Count(t => t.Properties[firstProperty] == firstVariant) > 1;
                    }
                });

                _rules.Add(String.Format("zero with {0} {1}", _propertyNames[firstProperty], firstVariant), new Rule()
                {
                    Check = (Config c) =>
                    {
                        return c.Tiles.OfType<Tile>().Count(t => t.Properties[firstProperty] == firstVariant) > 1;
                    }
                });

                for (var secondProperty = firstProperty + 1; secondProperty < 4; secondProperty++)
                {
                    for (var secondVariant = 1; secondVariant <= 3; secondVariant++)
                    {
                        _rules.Add(String.Format("at least one with {0} {1} and {2} {3}", _propertyNames[firstProperty], firstVariant, _propertyNames[secondProperty], secondVariant), new Rule()
                        {
                            Check = (Config c) =>
                            {
                                return c.Tiles.OfType<Tile>().Any(t => t.Properties[firstProperty] == firstVariant)
                                    && c.Tiles.OfType<Tile>().Any(t => t.Properties[secondProperty] == secondVariant);
                            }
                        });
                    }
                }
            }
        }*/

        // Pick random rule
        _masterRule = new Rule();
        _masterRule.PickRandom();
        _rules.ElementAt(Rnd.Range(0, _rules.Count)).Key;
        Debug.LogFormat("Random rule: {0}", _masterRule);

        // Search for random configuration that matches the rule
        do _playerConfig = RandomConfig();
        while (!_rules[_masterRule].Check(_playerConfig));
        _followsRule = _playerConfig;
        _quizzedConfigs.Add(_playerConfig.Clone());
        Debug.LogFormat("Config that matches the master rule: {0}", _playerConfig.ToString());

        // Search for random configuration that doesn't match the rule
        do _playerConfig = RandomConfig();
        while (_rules[_masterRule].Check(_playerConfig));
        _doesNotFollowRule = _playerConfig;
        _quizzedConfigs.Add(_playerConfig.Clone());
        Debug.LogFormat("Config that doesn't match the master rule: {0}", _playerConfig.ToString());

        // Some random guesses and a response to disprove
        for (var i = 0; i < 10; i++)
        {
            string guessedRule;
            do guessedRule = _rules.ElementAt(Rnd.Range(0, _rules.Count)).Key;
            while (guessedRule == _masterRule);
            Debug.LogFormat("Random guess: {0}", guessedRule);

            do _playerConfig = RandomConfig();
            while (_rules[_masterRule].Check(_playerConfig) == _rules[guessedRule].Check(_playerConfig));
            Debug.LogFormat("Config that disproves the guess: {0} {1}",
                _playerConfig.ToString(),
                (_rules[_masterRule].Check(_playerConfig)
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
            if (_playerConfig.Tiles[i] == null)
                _playerConfig.Tiles[i] = new Tile();
            else
                _playerConfig.Tiles[i] = null;
        }
        else
        {
            if (_playerConfig.Tiles[i] == null)
                _playerConfig.Tiles[i] = new Tile();
            _activeTile = i;
        }
        UpdateDisplay();
    }

    private void PressTileButton(int i)
    {
        if (_activeTile == -1) return;

        var tile = _playerConfig.Tiles[_activeTile];
        if (tile == null) return;

        /*        switch (i)
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
        */
        UpdateDisplay();
    }

    private void PressQuizButton(int i)
    {
        // If long press, just show one of the initial configs
        if (_isLongPress)
        {
            _playerConfig = (i == 0 ? _followsRule : _doesNotFollowRule);
            UpdateDisplay();
            return;
        }

        // Flash the button that was the actual answer
        var followsRule = _rules[_masterRule].Check(_playerConfig);
        StartCoroutine(FlashCaption(QuizButtons[followsRule ? 0 : 1].transform.Find("Text").GetComponent<TextMesh>()));

        // First check if this config has been quized before
        foreach (var config in _quizzedConfigs)
            if (_playerConfig.Equals(config)) return;
        _quizzedConfigs.Add(_playerConfig.Clone());

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
    {/*
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
        */
    }

    private Config RandomConfig()
    {

        Config config = new Config() { Tiles = new List<Tile>() { null, null, null, null } };
        /*
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
        */
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
                    for (var property = 0; property < 3; property++)
                    {
                        if (Tiles[i].Properties[property] != config.Tiles[i].Properties[property]) return false;
                    }
                }
            }
            return true;
        }

        override public string ToString()
        {
            return String.Join(", ", this.Tiles.Select(
            t => t is Tile
            ? String.Format(
                    "(symbol color {0}, symbol {1}, pattern color {2}, pattern {3})",
                    t.Properties[0],
                    t.Properties[1],
                    t.Properties[2],
                    t.Properties[3])
            : "(empty)"
            ).ToArray());
        }
    }
}
