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
    private Rule _hiddenRule;
    private Rule _guessRule;
    private Config _playerConfig;
    private Config _currentConfig;
    private Config _followsRuleConfig;
    private Config _doesNotFollowRuleConfig;
    private bool _currentAnswerConfigFollowsRule = true;
    private List<Config> _quizzedConfigs = new List<Config>();
    private int _activeTile = -1;
    private bool _canGuess;
    private enum Mode { Question, Answer }
    private Mode _mode = Mode.Answer;
    private Values _values;

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

        // Pick random symbols and colors
        _values = new Values(_possibleFrontSymbols, _possibleBackSymbols);
        Debug.LogFormat("[Zendo #{0}] Front colors: {1}, {2}, {3}.", _moduleId, _values.FrontColorNames[0], _values.FrontColorNames[1], _values.FrontColorNames[2]);
        Debug.LogFormat("[Zendo #{0}] Front symbols: {1}, {2}, {3}.", _moduleId, _values.FrontSymbols[0], _values.FrontSymbols[1], _values.FrontSymbols[2]);
        Debug.LogFormat("[Zendo #{0}] Back colors: {1}, {2}, {3}.", _moduleId, _values.BackColorNames[0], _values.BackColorNames[1], _values.BackColorNames[2]);
        Debug.LogFormat("[Zendo #{0}] Back symbols: {1}, {2}, {3}.", _moduleId, _values.BackSymbols[0], _values.BackSymbols[1], _values.BackSymbols[2]);

        // Setup rule stuff
        _guessRule = new Rule(_values);
        _guessRule.ClearButtons(RuleButtons);
        Rule.FontAwesome = _fontAwesome;

        // Pick random rule
        _hiddenRule = new Rule(_values);
        _hiddenRule.Randomize();
        Debug.LogFormat("[Zendo #{0}] Hidden rule: {1}.", _moduleId, _hiddenRule);

        // Search for random configuration that matches the rule
        _playerConfig = new Config(_values) { Tiles = new List<Tile>() { null, null, null, null } };
        _followsRuleConfig = new Config(_values);
        do _followsRuleConfig.Randomize();
        while (!_hiddenRule.Check(_followsRuleConfig));
        _quizzedConfigs.Add(_followsRuleConfig.Clone());
        _currentConfig = _followsRuleConfig;
        Debug.LogFormat("[Zendo #{0}] Config that matches the hidden rule: {1}.", _moduleId, _followsRuleConfig);

        // Search for random configuration that doesn't match the rule
        _doesNotFollowRuleConfig = new Config(_values);
        do _doesNotFollowRuleConfig.Randomize();
        while (_hiddenRule.Check(_doesNotFollowRuleConfig));
        _quizzedConfigs.Add(_doesNotFollowRuleConfig.Clone());
        Debug.LogFormat("[Zendo #{0}] Config that doesn't match the hidden rule: {1}.", _moduleId, _doesNotFollowRuleConfig);

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
            var followsRule = _hiddenRule.Check(_currentConfig);
            StartCoroutine(FlashFollowButton(followsRule));

            Debug.LogFormat("[Zendo #{0}] Guessing config: {1}.", _moduleId, _currentConfig);
            Debug.LogFormat("[Zendo #{0}] You guessed {1}, {2} {3}.", _moduleId,
                i == 0 ? "it follows the rule" : "it doesn't follow the rule",
                (i == 0 && followsRule) || (i == 1 && !followsRule) ? "and indeed" : "but",
                followsRule ? "it does" : "it doesn't");

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
            Debug.LogFormat("[Zendo #{0}] Guessing rule: {1}.", _moduleId, _guessRule);
            if (_guessRule.Equals(_hiddenRule))
            {
                Debug.LogFormat("[Zendo #{0}] Correct! Module solved.", _moduleId);
                Module.HandlePass();
            }
            else
            {
                // Replace one of the answers with new prove
                var config = new Config(_values);
                do config.Randomize();
                while (_hiddenRule.Check(config) == _guessRule.Check(config));
                _quizzedConfigs.Add(config.Clone());
                Debug.LogFormat("[Zendo #{0}] That's incorrect. Config that disproves the guess: {1}.", _moduleId, config);
                Debug.LogFormat("[Zendo #{0}] {1}", _moduleId,
                    _hiddenRule.Check(config)
                    ? "This config matches the hidden rule, but not the guessed rule."
                    : "This config matches the guessed rule, but not the hidden rule.");
                if (_hiddenRule.Check(config))
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
                _guessRule = new Rule(_values);
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
        FollowButtons[0].transform.Find("Front").GetComponent<TextMesh>().text =
            _currentAnswerConfigFollowsRule && _mode == Mode.Answer
            ? _fontAwesome["check-circle"]
            : _fontAwesome["circle"];
        FollowButtons[1].transform.Find("Front").GetComponent<TextMesh>().text =
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
                frontSymbolText.text = _fontAwesome[_values.FrontSymbols[tile.Properties[RuleProperty.FrontSymbol]]];
                frontSymbolText.color = _values.FrontColors[tile.Properties[RuleProperty.FrontColor]];

                if (_possibleBackSymbols[_values.BackSymbols[tile.Properties[RuleProperty.BackSymbol]]] is Vector3)
                    backSymbolObj.localPosition = (Vector3)_possibleBackSymbols[_values.BackSymbols[tile.Properties[RuleProperty.BackSymbol]]];
                else
                    backSymbolObj.localPosition = new Vector3(0f, BackPosY, 0f);
                backSymbolText.text = _fontAwesome[_values.BackSymbols[tile.Properties[RuleProperty.BackSymbol]]];
                backSymbolText.color = _values.BackColors[tile.Properties[RuleProperty.BackColor]];
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
        var text = FollowButtons[followsRule ? 0 : 1].transform.Find("Front").GetComponent<TextMesh>();
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
