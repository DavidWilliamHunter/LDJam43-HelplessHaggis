using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenuUI : MonoBehaviour
{
    GameManager gameManager;
    Button PlayButton;
    Button QuitButton;

    List<Button> levelButtons;

    // Start is called before the first frame update
    void Start()
    {
        levelButtons = new List<Button>();
        gameManager = FindObjectOfType<GameManager>();
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach(Button button in buttons)
        {
            switch(button.name)
            {
                case "PlayButton":
                    PlayButton = button;
                    button.onClick.AddListener(gameManager.StartPlay);
                    break;
                case "QuitButton":
                    QuitButton = button;
                    button.onClick.AddListener(gameManager.QuitGame);
                    break;
                case "Level0":
                    levelButtons.Add(button);
                    button.onClick.AddListener(() => gameManager.StartLevel(0));
                    break;
                case "Level1":
                    levelButtons.Add(button);
                    button.onClick.AddListener(() => gameManager.StartLevel(1));
                    break;
                case "Level2":
                    levelButtons.Add(button);
                    button.onClick.AddListener(() => gameManager.StartLevel(2));
                    break;
                case "Level3":
                    levelButtons.Add(button);
                    button.onClick.AddListener(() => gameManager.StartLevel(3));
                    break;
                case "Level4":
                    levelButtons.Add(button);
                    button.onClick.AddListener(() => gameManager.StartLevel(4));
                    break;
                case "Level5":
                    levelButtons.Add(button);
                    button.onClick.AddListener(() => gameManager.StartLevel(5));
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
