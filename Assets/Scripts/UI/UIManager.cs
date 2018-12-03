using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class UIManager : MonoBehaviour
{
    Canvas canvas;

    Transform startButton;
    List<Transform> haggisActionButtons;
    Transform resignButton;

    const int blockerIndex = 0;
    const int bomberIndex = 1;
    const int builderIndex = 2;
    const int parashootIndex = 3;

    int currentHaggisAction = -1;

    Color disabledColor = Color.gray;
    Color enabledColor = Color.white;
    Color activatedColor = Color.red;

    enum ButtonState { Disabled, Enabled, Activated }

    public LevelManager levelManager;
    protected HaggisActionController haggisActionController;


    // Information boxes
    Text spawnedText;
    Text lostText;
    Text savedText;

    // Start is called before the first frame update
    void Start()
    {
        // Assuming the control is attached to the Hud UI canvas, find the appropriate buttons

    }

    private void OnEnable()
    {
        canvas = GetComponent<Canvas>();

        startButton = transform.Find("StartButton");
        Button b = startButton.GetComponent<Button>();
        b.onClick.AddListener(StartButtonClicked);

        haggisActionButtons = new List<Transform>(parashootIndex);
        RegisterHaggisActionButton("BlockerButton", blockerIndex);
        RegisterHaggisActionButton("BomberButton", bomberIndex);
        RegisterHaggisActionButton("BuilderButton", builderIndex);
        RegisterHaggisActionButton("ParashootButton", parashootIndex);

        resignButton = transform.Find("ResignButton");
        b = resignButton.GetComponent<Button>();
        b.onClick.AddListener(ResignButtonClicked);

        levelManager = FindObjectOfType<LevelManager>();
        if(levelManager)
        {
            levelManager.AddOnGameStateEnter(GameStateEntered);

            haggisActionController = levelManager.GetComponent<HaggisActionController>();
        }

        Transform panel = transform.Find("TopPanel");
        Transform go = panel.Find("SpawnedText");
        spawnedText = go.GetComponent<Text>();
        go = panel.Find("LostText");
        lostText = go.GetComponent<Text>();
        go = panel.Find("SavedText");
        savedText = go.GetComponent<Text>();
    }

    private void OnDisable()
    {
        if(levelManager)
        {
            levelManager.RemoveOnGameStateEnter(GameStateEntered);
            levelManager = null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(haggisActionController)
        {
            spawnedText.text = haggisActionController.haggisSpawned.ToString();
            lostText.text = haggisActionController.haggisLost.ToString();
            savedText.text = haggisActionController.haggisSaved.ToString();
        }
    }

    public void PreGameConfiguration()
    {
        SetStartButton(ButtonState.Enabled);
        SetHaggisActionButtons(ButtonState.Disabled);
        SetResignButton(ButtonState.Disabled);
        currentHaggisAction = -1;
        Canvas.ForceUpdateCanvases();
    }

    public void PlayGameConfiguration()
    {
        SetStartButton(ButtonState.Disabled);
        SetResignButton(ButtonState.Enabled);
        SetHaggisActionButtons(ButtonState.Enabled);
        Canvas.ForceUpdateCanvases();

    }

    private void SetHaggisActionActive(int index)
    {
        SetHaggisActionButtons(ButtonState.Enabled);
        for (int i = 0; i < haggisActionButtons.Count; ++i)
        {
            if (i == index)
                SetHaggisActionButton(index, ButtonState.Activated);
            else
                SetHaggisActionButton(index, ButtonState.Enabled);
        }
    }

    // some helper functions
    private void SetHaggisActionButtons(ButtonState state)
    {
        foreach(Transform go in haggisActionButtons)
        {
            Button b = go.GetComponent<Button>();
            if (b)
            {
                SetActionButton(b, state);
            }
        }
    }

    private void SetHaggisActionButton(int index, ButtonState state)
    {
        if (index >= 0 && index < haggisActionButtons.Count)
        {
            Button b = haggisActionButtons[index].GetComponent<Button>();
            if (b)
            {
                SetActionButton(b, state);
            }
        }   
    }

    private void SetActionButton(Button b, ButtonState state)
    {
        switch (state)
        {
            case ButtonState.Disabled:
                {
                    b.enabled = false;
                    ColorBlock block = ColorBlock.defaultColorBlock;
                    block.normalColor = disabledColor;
                    b.colors = block;
                    break;
                }
            case ButtonState.Enabled:
                {
                    b.enabled = false;
                    b.enabled = true;
                    ColorBlock block = ColorBlock.defaultColorBlock;
                    block.normalColor = enabledColor;
                    b.colors = block;
                    break;
                }
            case ButtonState.Activated:
                {
                    b.enabled = false;
                    b.enabled = true;
                    ColorBlock block = ColorBlock.defaultColorBlock;
                    block.normalColor = activatedColor;
                    b.colors = block;
                }
                break;
        }

    }

    private void RegisterHaggisActionButton(String name, int index)
    {
        Transform go = transform.Find(name);
        if(go)
        {
            Button b = go.GetComponent<Button>();
            b.onClick.AddListener(() => HaggisActionButtonClicked(index));
            haggisActionButtons.Add(go);
        }

    }

    void SetStartButton(ButtonState state)
    {
        Button b = startButton.GetComponent<Button>();
        if(b)
        {
            SetActionButton(b, state);
        }
    }

    void SetResignButton(ButtonState state)
    {
        Button b = resignButton.GetComponent<Button>();
        if(b)
        {
            SetActionButton(b,state);
        }
    }

    void StartButtonClicked()
    {
        if(levelManager)
        {
            levelManager.StartPlaying(); // don't do anything else here, it will phone back to say it is playing.
        }
    }

    void HaggisActionButtonClicked(int index)
    {
        Debug.LogFormat("Start HaggisActionButtonClicked {0} Clicked", index);
        currentHaggisAction = index ;

        if (haggisActionController.SetCurrentSelectedAction(IndexToAction(index)))
        {
            SetHaggisActionButtons(ButtonState.Enabled);
            SetHaggisActionButton(index, ButtonState.Activated);
            Canvas.ForceUpdateCanvases();
        }

        // set the current selected Action

    }

    void ResignButtonClicked()
    {
        haggisActionController.DestroyAll();
    }

    void GameStateEntered(LevelManager.GameState newGameState, LevelManager.GameState oldGameState)
    {
        switch(newGameState)
        {
            case LevelManager.GameState.Pregame:
                PreGameConfiguration();
                break;
            case LevelManager.GameState.Playing:
                PlayGameConfiguration();
                break;

        }
    }

    HaggisBehaviour.Action IndexToAction(int index)
    {
        switch(index)
        {
            case blockerIndex:
                return HaggisBehaviour.Action.Blocking;
            case bomberIndex:
                return HaggisBehaviour.Action.Bombing;
            case builderIndex:
                return HaggisBehaviour.Action.Building;
            case parashootIndex:
                return HaggisBehaviour.Action.Parashooting;
        }
        return HaggisBehaviour.Action.None;
    }
}
