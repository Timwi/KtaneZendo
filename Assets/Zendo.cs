using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Rnd = UnityEngine.Random;
using Assets;

public partial class Zendo : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMBombModule Module;
    public Sprite[] Sprites;
    public KMSelectable[] Tiles;
    public KMSelectable[] TileButtons;
    public KMSelectable[] FollowButtons;
    public KMSelectable[] RuleButtons;
    public KMSelectable ModeButton;

    private int _moduleId;
    private static int _moduleIdCounter = 1;
    private bool _isButtonDown, _isLongPress;
    private Coroutine _buttonDownCoroutine;
    private string[] _propertyNames = { "front color", "front symbol", "back color", "back symbol" };
    private Rule _masterRule;
    private Rule _guessRule = new Rule();
    private Config _playerConfig = new Config() { Tiles = new List<Tile>() { null, null, null, null } };
    private Config _currentConfig;
    private Config _followsRuleConfig;
    private Config _doesNotFollowRuleConfig;
    private bool _currentAnswerConfigFollowsRule = true;
    private List<Config> _quizzedConfigs = new List<Config>();
    private int _activeTile = -1;
    private bool _canGuess;
    private Dictionary<int, string> _frontSymbols = new Dictionary<int, string>();
    private Dictionary<int, string> _backSymbols = new Dictionary<int, string>();
    private List<Color> _colors = new List<Color>();
    private enum Mode { Question, Answer }
    private Mode _mode = Mode.Answer;

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

        for (int i = 0; i < FollowButtons.Length; i++)
        {
            var j = i;
            FollowButtons[i].OnInteract += delegate () { _buttonDownCoroutine = StartCoroutine(HandleLongPress()); return false; };
            FollowButtons[i].OnInteractEnded += delegate () { HandleButtonUp(); PressFollowButton(j); };
        }

        for (int i = 0; i < RuleButtons.Length; i++)
        {
            var j = i;
            RuleButtons[i].OnInteract += delegate () { _buttonDownCoroutine = StartCoroutine(HandleLongPress()); return false; };
            RuleButtons[i].OnInteractEnded += delegate () { HandleButtonUp(); PressRuleButton(j); };
        }

        ModeButton.OnInteract += delegate () { _buttonDownCoroutine = StartCoroutine(HandleLongPress()); return false; };
        ModeButton.OnInteractEnded += delegate () { HandleButtonUp(); PressModeButton(); };

        // Pick random symbols
        var frontSymbols = _possibleFrontSymbols.Shuffle();
        var backSymbols = _possibleBackSymbols.Keys.ToList().Shuffle();
        for (var i = 0; i < 3; i++) _frontSymbols.Add(i, frontSymbols[i]);
        for (var i = 0; i < 3; i++) _backSymbols.Add(i, backSymbols[i]);
        Debug.LogFormat("Front symbols: {0}, {1}, {2}", _frontSymbols[0], _frontSymbols[1], _frontSymbols[2]);
        Debug.LogFormat("Back symbols: {0}, {1}, {2}", _backSymbols[0], _backSymbols[1], _backSymbols[2]);

        // Pick random colors
        var lightnesses = new float[] { .25f, .75f };
        foreach (var lightness in lightnesses)
        {
            int[] hues;
            do hues = new int[] { Rnd.Range(0, 360), Rnd.Range(0, 360), Rnd.Range(0, 360) };
            while (!Colors.EnoughDistance(hues));
            Debug.LogFormat("Hues: {0}, {1}, {2}", hues[0], hues[1], hues[2]);
            foreach (var hue in hues) _colors.Add(Colors.HslToColor(hue, 1f, lightness));
        }

        // Pick random rule
        _masterRule = new Rule();
        _masterRule.Randomize();
        Debug.LogFormat("Random rule: {0}", _masterRule);

        // Setup rule stuff
        _guessRule.ClearButtons(RuleButtons);
        Rule.Colors = _colors;
        Rule.FontAwesome = _fontAwesome;
        Rule.FrontSymbols = _frontSymbols;
        Rule.BackSymbols = _backSymbols;

        // Search for random configuration that matches the rule
        _followsRuleConfig = new Config();
        do _followsRuleConfig.Randomize();
        while (!_masterRule.Check(_followsRuleConfig));
        _quizzedConfigs.Add(_followsRuleConfig.Clone());
        _currentConfig = _followsRuleConfig;
        Debug.LogFormat("Config that matches the master rule:\n{0}", _followsRuleConfig.ToString());

        // Search for random configuration that doesn't match the rule
        _doesNotFollowRuleConfig = new Config();
        do _doesNotFollowRuleConfig.Randomize();
        while (_masterRule.Check(_doesNotFollowRuleConfig));
        _quizzedConfigs.Add(_doesNotFollowRuleConfig.Clone());
        Debug.LogFormat("Config that doesn't match the master rule:\n{0}", _doesNotFollowRuleConfig.ToString());

        //// Some random guesses and a response to disprove
        //for (var i = 0; i < 10; i++)
        //{
        //    var guessedRule = new Rule();
        //    do guessedRule.Randomize();
        //    while (guessedRule == _masterRule);
        //    Debug.LogFormat("Random guess: {0}", guessedRule);

        //    var config = new Config();
        //    do config.Randomize();
        //    while (_masterRule.Check(config) == guessedRule.Check(config));
        //    Debug.LogFormat("Config that disproves the guess:\n{0}\n{1}",
        //        config.ToString(),
        //        (_masterRule.Check(config)
        //            ? "because it matches the master rule, but not the guessed rule."
        //            : "because it matches the guessed rule, but not the master rule."));
        //}

        UpdateDisplay();
    }

    private void PressTile(int i)
    {
        if (_mode == Mode.Answer) return;

        if (_isLongPress)
        {
            if (_currentConfig.Tiles[i] == null)
                _currentConfig.Tiles[i] = new Tile();
            else
                _currentConfig.Tiles[i] = null;
        }
        else
        {
            if (_currentConfig.Tiles[i] == null)
                _currentConfig.Tiles[i] = new Tile();
            _activeTile = i;
        }
        UpdateDisplay();
    }

    private void PressTileButton(int i)
    {
        if (_mode == Mode.Answer) return;

        if (_activeTile == -1) return;

        var tile = _currentConfig.Tiles[_activeTile];
        if (tile == null) return;

        tile.Properties[(RuleProperty)i] += 1;
        if (tile.Properties[(RuleProperty)i] == 3)
            tile.Properties[(RuleProperty)i] = 0;

        UpdateDisplay();
    }

    private void PressFollowButton(int i)
    {
        // Answer mode?
        if (_mode == Mode.Answer)
        {
            // Long press?
            if (_isLongPress)
            {
                // Copy the follows / doesn't follow config to the player config
                _playerConfig = (i == 0 ? _followsRuleConfig : _doesNotFollowRuleConfig);
                _currentConfig = _playerConfig;
                _mode = Mode.Question;
            }

            // Normal press?
            else
            {
                // Show the latest answer that follows / doesn't follow the rule
                _currentConfig = (i == 0 ? _followsRuleConfig : _doesNotFollowRuleConfig);
                _currentAnswerConfigFollowsRule = (i == 0);
            }

        }

        // Question mode?
        else
        {
            // Flash the button that was the actual answer
            var followsRule = _masterRule.Check(_currentConfig);
            StartCoroutine(FlashFollowButton(followsRule));

            // First check if this config has been quizzed before
            foreach (var config in _quizzedConfigs)
                if (_currentConfig.Equals(config)) return;
            _quizzedConfigs.Add(_currentConfig.Clone());

            // Allow guessing rule if guessed correctly
            if ((i == 0 && followsRule) || (i == 1 && !followsRule))
            {
                _canGuess = true;
                _guessRule.InitializeButtons(RuleButtons);
            }
        }

        UpdateDisplay();
    }

    private void PressRuleButton(int i)
    {
        if (!_canGuess) return;

        var ruleComplete = _guessRule.PressButton(i, RuleButtons);
        if (ruleComplete)
        {
            if (_guessRule.Equals(_masterRule))
            {
                Module.HandlePass();
            }
            else
            {
                // Replace one of the answers with new prove
                var config = new Config();
                do config.Randomize();
                while (_masterRule.Check(config) == _guessRule.Check(config));
                Debug.LogFormat("Config that disproves the guess:\n{0}\n{1}",
                    config,
                    (_masterRule.Check(config)
                    ? "because it matches the master rule, but not the guessed rule."
                    : "because it matches the guessed rule, but not the master rule."));
                if (_masterRule.Check(config))
                {
                    _followsRuleConfig = config;
                    _currentAnswerConfigFollowsRule = true;

                }
                else
                {
                    _doesNotFollowRuleConfig = config;
                    _currentAnswerConfigFollowsRule = false;
                }

                // Show the new prove
                _mode = Mode.Answer;
                _playerConfig = _currentConfig;
                _currentConfig = _currentAnswerConfigFollowsRule ? _followsRuleConfig : _doesNotFollowRuleConfig;

                // Reset guess buttons
                _canGuess = false;
                _guessRule = new Rule();
                _guessRule.ClearButtons(RuleButtons);
            }
        }

        UpdateDisplay();
    }

    private void PressModeButton()
    {
        if (_mode == Mode.Answer)
        {
            _mode = Mode.Question;
            _currentConfig = _playerConfig;
        }
        else
        {
            _mode = Mode.Answer;
            _playerConfig = _currentConfig;
            _currentConfig = _currentAnswerConfigFollowsRule ? _followsRuleConfig : _doesNotFollowRuleConfig;
        }

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        ModeButton.transform.Find("Text").GetComponent<TextMesh>().text = _mode == Mode.Question ? "?" : "!";
        FollowButtons[0].transform.Find("Text").GetComponent<TextMesh>().text =
            _currentAnswerConfigFollowsRule && _mode == Mode.Answer
            ? _fontAwesome["check-circle"]
            : _fontAwesome["circle"];
        FollowButtons[1].transform.Find("Text").GetComponent<TextMesh>().text =
            !_currentAnswerConfigFollowsRule && _mode == Mode.Answer
            ? _fontAwesome["check-circle"]
            : _fontAwesome["circle"];

        for (var i = 0; i < Tiles.Length; i++)
        {
            var frontSymbolText = Tiles[i].transform.Find("Front").GetComponent<TextMesh>();
            var backSymbolObj = Tiles[i].transform.Find("Back");
            var backSymbolText = backSymbolObj.GetComponent<TextMesh>();
            var tile = _currentConfig.Tiles[i];

            if (tile is Tile)
            {
                frontSymbolText.text = _fontAwesome[_frontSymbols[tile.Properties[RuleProperty.FrontSymbol]]];
                frontSymbolText.color = _colors[tile.Properties[RuleProperty.FrontColor]];

                if (_possibleBackSymbols[_backSymbols[tile.Properties[RuleProperty.BackSymbol]]] is Vector3)
                    backSymbolObj.localPosition = (Vector3)_possibleBackSymbols[_backSymbols[tile.Properties[RuleProperty.BackSymbol]]];
                else
                    backSymbolObj.localPosition = new Vector3(0f, 0f, 0f);
                backSymbolText.text = _fontAwesome[_backSymbols[tile.Properties[RuleProperty.BackSymbol]]];
                backSymbolText.color = _colors[tile.Properties[RuleProperty.BackColor] + 3];
            }
            else
            {
                frontSymbolText.text = "";
                backSymbolText.text = "";
            }
        }
    }

    private IEnumerator FlashFollowButton(bool followsRule)
    {
        yield return null;
        var text = FollowButtons[followsRule ? 0 : 1].transform.Find("Text").GetComponent<TextMesh>();
        for (var i = 0; i < 6; i++)
        {
            text.text = (i % 2 == 0) ? _fontAwesome["check-circle"] : _fontAwesome["circle"];
            yield return new WaitForSeconds(.15f);
        }
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
}
