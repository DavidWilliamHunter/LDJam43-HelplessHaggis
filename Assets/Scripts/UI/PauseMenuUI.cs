using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    GameManager gameManager;
    Button ResumeButton;
    Button MainMenuButton;
    Button QuitButton;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        Button[] buttons = transform.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            switch (button.name)
            {
                case "ResumeButton":
                    ResumeButton = button;
                    button.onClick.AddListener(ResumePlay);
                    break;
                case "QuitButton":
                    QuitButton = button;
                    button.onClick.AddListener(gameManager.QuitGame);
                    break;
                case "ReturnMenuButton":
                    QuitButton = button;
                    button.onClick.AddListener(gameManager.ReturnToMainMenu);
                    break;
            }
        }
    }

    private void ResumePlay()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }


}

