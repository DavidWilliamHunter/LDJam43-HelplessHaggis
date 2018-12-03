using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    protected string [] scenes =  { "EasyGoing", "Chasm" , "RingOfFire", "Boom", "AllTheThings", "SampleScene"};
    protected string menuScene = "MenuScene";

    public GameObject pauseMenu;
    public DisplayVictoryText victoryText;

    // Start is called before the first frame update

    enum GameState { MainMenu, PlayingLevel, Quiting };
    GameState gameState = GameState.MainMenu;

    private void Awake()
    {
        if(instance==null)
        {
            instance = this;
        } else if(instance!=this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

    }

    internal void StartPlay()
    {
        StartLevel(0);
    }

    internal void QuitGame()
    {
        Application.Quit();
    }

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    private void OnEnable()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.active)
                pauseMenu.SetActive(false);
            else
                pauseMenu.SetActive(true);
        }
    }

    void InitGame()
    {
        gameState = GameState.MainMenu;
        SceneManager.LoadScene(menuScene, LoadSceneMode.Single);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(menuScene));
    }

    public void GameOver(string message)
    {
        victoryText.DisplayMessage(message);
        StartCoroutine(GameOverCR());
    }

    IEnumerator GameOverCR()
    {
        yield return new WaitForSeconds(3.0f);
        victoryText.RemoveMessage();
        ReturnToMainMenu();
    }


    internal void ReturnToMainMenu()
    {
        SceneManager.LoadScene(menuScene, LoadSceneMode.Single);
        //SceneManager.SetActiveScene(SceneManager.GetSceneByName(menuScene));
    }

    public void StartLevel(int index)
    {
        if(index < 0 || index>= scenes.Length)
        {
            Debug.LogError("Invalid Scene");
        } else
        {
            
            SceneManager.LoadScene(scenes[index], LoadSceneMode.Single);
            //SceneManager.SetActiveScene(SceneManager.GetSceneByName("SampleScene"));
            //SceneManager.LoadScene(scenes[index], LoadSceneMode.Single);
            //SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenes[index]));
        }
    }
}
