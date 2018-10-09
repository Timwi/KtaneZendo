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
    private Config _playerConfig = new Config() { Tiles = new List<Tile>() { null, null, null, null } };
    private Config _followsRule;
    private Config _doesNotFollowRule;
    private List<Config> _quizzedConfigs = new List<Config>();
    private int _activeTile = -1;
    private int _guessTokens = 0;
    private Dictionary<int, string> _symbols = new Dictionary<int, string>();
    private Dictionary<int, string> _patterns = new Dictionary<int, string>();
    private List<Color> _colors;

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

        // Pick random symbols and patterns to use
        var symbols = _possibleSymbols.Shuffle();
        var patterns = _possiblePatterns.Shuffle();
        for (var i = 1; i <= 3; i++) _symbols.Add(i, symbols[i - 1]);
        for (var i = 1; i <= 3; i++) _patterns.Add(i, patterns[i - 1]);

        // Pick random colors
        var tries = 0;
        PickColors:
        tries++;
        _colors = new List<Color>();
        for (var i = 0; i < 6; i++)
            _colors.Add(new Color(Rnd.Range(0f, 1f), Rnd.Range(0f, 1f), Rnd.Range(0f, 1f)));
        _colors.Add(new Color(0f, 0f, 0f));
        _colors.Add(new Color(1f, 1f, 1f));
        for (var i = 0; i < _colors.Count; i++)
        {
            for (var j = i + 1; j < _colors.Count; j++)
            {
                var compare = ColorFormulas.DoFullCompare(
                    (int)(_colors[i].r * 255),
                    (int)(_colors[i].g * 255),
                    (int)(_colors[i].b * 255),
                    (int)(_colors[j].r * 255),
                    (int)(_colors[j].g * 255),
                    (int)(_colors[j].b * 255));
                if (compare < 40) goto PickColors;
            }
        }
        Debug.LogFormat("Tries: {0}", tries);

        // Pick random rule
        _masterRule = new Rule();
        _masterRule.Randomize();
        Debug.LogFormat("Random rule: {0}", _masterRule);

        // Search for random configuration that matches the rule
        _followsRule = new Config();
        do _followsRule.Randomize();
        while (!_masterRule.Check(_followsRule));
        _quizzedConfigs.Add(_followsRule.Clone());
        _playerConfig = _followsRule;
        Debug.LogFormat("Config that matches the master rule:\n{0}", _followsRule.ToString());

        // Search for random configuration that doesn't match the rule
        _doesNotFollowRule = new Config();
        do _doesNotFollowRule.Randomize();
        while (_masterRule.Check(_doesNotFollowRule));
        _quizzedConfigs.Add(_doesNotFollowRule.Clone());
        Debug.LogFormat("Config that doesn't match the master rule:\n{0}", _doesNotFollowRule.ToString());

        // Some random guesses and a response to disprove
        for (var i = 0; i < 10; i++)
        {
            var guessedRule = new Rule();
            do guessedRule.Randomize();
            while (guessedRule == _masterRule);
            Debug.LogFormat("Random guess: {0}", guessedRule);

            var config = new Config();
            do config.Randomize();
            while (_masterRule.Check(config) == guessedRule.Check(config));
            Debug.LogFormat("Config that disproves the guess:\n{0}\n{1}",
                config.ToString(),
                (_masterRule.Check(config)
                    ? "because it matches the master rule, but not the guessed rule."
                    : "because it matches the guessed rule, but not the master rule."));
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

        tile.Properties[(RuleProperty)i] += 1;
        if (tile.Properties[(RuleProperty)i] == 4)
            tile.Properties[(RuleProperty)i] = 1;

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
        var followsRule = _masterRule.Check(_playerConfig);
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
        /*
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
        */
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        for (var i = 0; i < Tiles.Length; i++)
        {
            var symbol = Tiles[i].transform.Find("Symbol").GetComponent<TextMesh>();
            var pattern = Tiles[i].transform.Find("Pattern").GetComponent<TextMesh>();
            var tile = _playerConfig.Tiles[i];

            if (tile is Tile)
            {
                symbol.text = _fontAwesome[_symbols[tile.Properties[RuleProperty.Symbol]]];
                symbol.color = _colors[tile.Properties[RuleProperty.SymbolColor] - 1];
                pattern.text = _fontAwesome[_patterns[tile.Properties[RuleProperty.Pattern]]];
                pattern.color = _colors[tile.Properties[RuleProperty.PatternColor] + 2];
            }
            else
            {
                symbol.text = "";
                pattern.text = "";
            }
        }

        GuessTokens.text = _guessTokens.ToString();

        foreach (var button in GuessButtons)
        {
            button.transform.Find("Text").GetComponent<TextMesh>().text = "";
        }

/*        if (_guessTokens > 0 && _activeRulePart.Children != null)
        {
            for (var i = 0; i < _activeRulePart.Children.Count; i++)
            {
                var text = _activeRulePart.Children[i].Text;
                if (_activeRulePart.Children[i].Children != null) text += " ...";
                GuessButtons[i].transform.Find("Text").GetComponent<TextMesh>().text = text;
            }
        }*/
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
}
