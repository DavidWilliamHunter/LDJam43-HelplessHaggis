using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaggisActionController : MonoBehaviour
{
    // expected number of haggises
    public int HaggisMax = 10;
    // Create and manage object pools
    ObjectPool haggisPool;
        public GameObject haggisPreFab;


    public const int Left = 0;
    public const int Middle = 2;
    public const int Right = 1;

    // define our action delegates
    public delegate void OnMouseDown(RaycastHit hitInfo);
    public delegate void OnMouseUp(RaycastHit hitInfo);

    // create delegate managersl
    protected OnMouseDown mouseDownWatchers;
    protected OnMouseUp mouseUpWatchers;

    public HaggisBehaviour.Action currentSelectedAction = HaggisBehaviour.Action.None;

    public Transform spawnPoint;
    public Transform spawnPrefab;
    protected Transform spawner = null;
    protected HaggisSpawner spawnerComponent = null;

    public int totalHaggises = 10;
    public int haggisSpawned = 0;
    public int haggisLost = 0;
    public int haggisSaved = 0;

    public int haggisTarget = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        haggisPool = new ObjectPool(HaggisMax)
        {
            prefab = haggisPreFab
        };
        haggisPool.PreInstantiate(HaggisMax);

        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager)
        {
            levelManager.AddOnGameStateEnter(GameStateEntered);
        }

        if (spawner)
        {
            Destroy(spawner);
        }
        spawnerComponent = null;
        spawner = null;

        CreateHaggisSpawner();
    }

    private void OnDisable()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        if (levelManager)
        {
            levelManager.RemoveOnGameStateEnter(GameStateEntered);
        }
        if (spawner)
        {
            Destroy(spawner);
        }
        spawnerComponent = null;
        spawner = null;
    }

private void OnDestroy()
{
    if (haggisPool != null)
    {
        haggisPool.DestroyObjects();
    }
}

internal void OnSelectHaggis(HaggisBehaviour haggisBehaviour, RaycastHit hitInfo)
    {
        Debug.LogFormat("Haggis Selected would have performed action : {0}", currentSelectedAction);
        if (haggisBehaviour.currentAction == HaggisBehaviour.Action.Walking)
        {
            switch (currentSelectedAction)
            {
                case HaggisBehaviour.Action.Blocking:
                    haggisBehaviour.SetCurrentAction(HaggisBehaviour.Action.Blocking);
                    break;
                case HaggisBehaviour.Action.Building:
                    haggisBehaviour.actionsRemaining = haggisBehaviour.numberOfBricks;
                    haggisBehaviour.StartBuilding();
                    haggisBehaviour.SetCurrentAction(HaggisBehaviour.Action.Building);
                    break;
                case HaggisBehaviour.Action.Bombing:
                    haggisBehaviour.SetCurrentAction(HaggisBehaviour.Action.Bombing);
                    break;
            }
        } else if(currentSelectedAction==HaggisBehaviour.Action.Bombing)
        {
            haggisBehaviour.SetCurrentAction(HaggisBehaviour.Action.Bombing);
        }
    }

    public bool SetCurrentSelectedAction(HaggisBehaviour.Action action)
    {
        switch (action)
        {
            case HaggisBehaviour.Action.Walking:
            case HaggisBehaviour.Action.Leaping:
            case HaggisBehaviour.Action.Falling:
            case HaggisBehaviour.Action.FastFalling:
            case HaggisBehaviour.Action.Dead:
            case HaggisBehaviour.Action.Hop:
            default:
                return false;
            case HaggisBehaviour.Action.Blocking:
            case HaggisBehaviour.Action.Building:
            case HaggisBehaviour.Action.Bombing:
            case HaggisBehaviour.Action.Exploding:
            case HaggisBehaviour.Action.None:
                currentSelectedAction = action;
                return true;
        }
    }

    void GameStateEntered(LevelManager.GameState newGameState, LevelManager.GameState oldGameState)
    {
        switch (newGameState)
        {
            case LevelManager.GameState.Paused:
                break;
            case LevelManager.GameState.Playing:
                spawnerComponent.countDownTypeParam1 = totalHaggises;
                spawnerComponent.StartSpawning();
                break;
            case LevelManager.GameState.Disabled:
            case LevelManager.GameState.Lost:
            case LevelManager.GameState.Won:
            case LevelManager.GameState.Pregame:
                break;

        }
    }

    private void CreateHaggisSpawner()
    {
        if(spawner)
        {
            Destroy(spawner);
            spawner = null;
        }
        spawner = Instantiate(spawnPrefab, spawnPoint);
        spawnerComponent = spawner.GetComponent<HaggisSpawner>();
        spawnerComponent.SetActive(false);
    }

    public GameObject InstantiateHaggis(Transform location)
    {
        GameObject go = haggisPool.Pop();
        HaggisBehaviour bh = go.GetComponent<HaggisBehaviour>();
        bh.setActionController(this);
        this.haggisSpawned++;
        //GameObject go = Instantiate(haggisPreFab);
        go.transform.position = location.position;
        go.transform.rotation = location.rotation;
        go.SetActive(true);
        return go;
    }

    internal void KillHaggis(HaggisBehaviour haggisBehaviour)
    {
        Debug.Log("Bye Bye");
        this.haggisLost++;
        GameObject t = haggisBehaviour.gameObject;
        t.SetActive(false);        // destroy self
        SetActiveAllChildren(t.transform, false);
    }

    internal void SaveHaggis(HaggisBehaviour haggisBehaviour)
    {
        Debug.Log("Bye Bye");
        this.haggisSaved++;
        GameObject t = haggisBehaviour.gameObject;
        t.SetActive(false);        // destroy self
        SetActiveAllChildren(t.transform, false);
    }

    public static void SetActiveAllChildren(Transform transform, bool value)
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(value);

            SetActiveAllChildren(child, value);
        }
    }

    public void DestroyAll()
    {
        foreach(GameObject haggis in haggisPool.objects)
        {
            if(haggis.activeInHierarchy)
            {
                HaggisBehaviour hb = haggis.GetComponent<HaggisBehaviour>();
                hb.SetCurrentAction(HaggisBehaviour.Action.Bombing);
            }
        }
    }

    public int HaggisInWorld()
    {
        return haggisSpawned - haggisSaved - haggisLost;
    }
    public bool IsFinished()
    {
        return HaggisInWorld() <= 0 && haggisSpawned >= totalHaggises ? true : false;
    }

    public bool IsWon()
    {
        return haggisSaved >= haggisTarget ? true : false;
    }

    public bool IsLost => IsFinished() && haggisSaved < haggisTarget;

}