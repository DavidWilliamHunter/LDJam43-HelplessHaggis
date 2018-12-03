using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    public float startTime;
    public float lastFireTime;
    public float interval;
    public int numberOfFires;
    public bool active = false;

    protected float pauseStart;
    protected float pauseFactor = 0.0f;
    protected bool isPaused;
        
    public enum CountDownType { Once, Loop, Number };
    public CountDownType countDownType;
    public int countDownTypeParam1;

    public delegate void OnFire();
    protected OnFire fireWatchers;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        lastFireTime = Time.time;
        numberOfFires = 0;
    }



    private void OnEnable()
    {
        lastFireTime = Time.time;
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if(levelManager)
        {
            levelManager.AddOnGameStateEnter(GameStateEntered);
        }
    }

    private void OnDisable()
    {
        lastFireTime = Time.time;
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager)
        {
            levelManager.RemoveOnGameStateEnter(GameStateEntered);
        }
        fireWatchers = null;
    }

    public void OnPause(bool pause)
    {
        if(pause)
        {
            isPaused = true;
            pauseStart = Time.time;
        } else
        {
            isPaused = false;
            pauseFactor = Time.time - pauseStart;
        }
    }
    public void OnApplicationPause(bool pause)
    {
        OnPause(pause);
    }

    public void AddOnFire(OnFire func)
    {
        fireWatchers += func;
    }

    public void SetActive(bool active)
    {
        if (this.active != active)
            OnPause(active);
        this.active = active;
        
    }

    // Update is called once per frame
    protected void Update()
    {
        if (active && (Time.time- lastFireTime) > (interval + pauseFactor))
        {
            pauseFactor = 0.0f;
            fireWatchers?.Invoke();
            lastFireTime = Time.time;
            numberOfFires++;

            switch (countDownType)
            {
                case CountDownType.Number:
                    countDownTypeParam1--;
                    if (countDownTypeParam1 <= 0)
                        SetActive(false);
                    break;
                case CountDownType.Once:
                    SetActive(false);
                    break;
            }
        }
    }

    void GameStateEntered(LevelManager.GameState newGameState, LevelManager.GameState oldGameState)
    {
        switch(newGameState)
        {
            case LevelManager.GameState.Paused:
                OnPause(true);
                break;
            case LevelManager.GameState.Playing:
                if(isPaused)
                    OnPause(false);
                SetActive(true);
                break;
            case LevelManager.GameState.Disabled:
            case LevelManager.GameState.Lost:
            case LevelManager.GameState.Won:
            case LevelManager.GameState.Pregame:
                SetActive(false);
                break;

        }        
    }

}
