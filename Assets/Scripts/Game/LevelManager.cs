using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    HaggisActionController actionController;

    UIManager ui;
    public enum GameState {  Pregame, Playing, Won, Lost, Paused, Disabled };
    public GameState gameState;

    // define our action delegates
    public delegate void OnGameStateEnter(GameState newAction, GameState oldAction);
    public delegate void OnGameStateExit(GameState currentAction, GameState newAction);

    // create delegate managers
    protected OnGameStateEnter gameStateEnterWatchers;
    protected OnGameStateExit gameStateExitWatchers;


    // Start is called before the first frame update
    void Start()
    {
        actionController = GetComponent<HaggisActionController>();
    }

    private void OnEnable()
    {
        SetCurrentGameState(GameState.Pregame);
    }

    private void OnDisable()
    {
        SetCurrentGameState(GameState.Disabled);
        gameStateEnterWatchers = null;
        gameStateExitWatchers = null;

    }



    // Update is called once per frame
    void Update()
    {
        if(actionController.IsFinished())
        {
            if (actionController.IsWon())
            {
                SetCurrentGameState(GameState.Won);
            }
            else
            {
                SetCurrentGameState(GameState.Lost);
            }
        }
    }

    public bool StartPlaying()
    {
        if (gameState == GameState.Pregame)
        {
            SetCurrentGameState(GameState.Playing);
            actionController.SetCurrentSelectedAction(HaggisBehaviour.Action.None);
            return true;
        }
        else
            return false;
    }


    // delegate watchers
    public void AddOnGameStateEnter(OnGameStateEnter func)
    {
        gameStateEnterWatchers += func;
    }

    public void AddOnGameStateExit(OnGameStateExit func)
    {
        gameStateExitWatchers += func;
    }

    public void RemoveOnGameStateEnter(OnGameStateEnter func)
    {
        gameStateEnterWatchers -= func;
    }

    public void RemoveOnGameStateExit(OnGameStateExit func)
    {
        gameStateExitWatchers -= func;
    }

    private void SetCurrentGameState(GameState act)
    {
        if (gameState != act)
        {
            gameStateExitWatchers?.Invoke(gameState, act);     // call functions watching for a state exit

            GameState oldAct = gameState;
            gameState = act;

            switch (act)
            {
                case GameState.Won:
                    {
                        GameManager gm = FindObjectOfType<GameManager>();
                        string text = string.Format("Well done! You have rescued {0} of {1} Haggii!", actionController.haggisSaved, actionController.haggisSpawned);
                        gm.GameOver(text);
                        break;
                    }
                case GameState.Lost:
                    {
                        GameManager gm = FindObjectOfType<GameManager>();
                        string text = string.Format("Bad luck! Poor skill? You have rescued {0} of {1} Haggii", actionController.haggisSaved, actionController.haggisSpawned);
                        gm.GameOver(text);
                    }
                    break;
            }
            //Debug.LogFormat("Called Enter Action from ID:{0}", GetInstanceID());
            gameStateEnterWatchers?.Invoke(act, oldAct);    // call functions watching for an enter state
        }
    }
}
