using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpMenu : MonoBehaviour
{
    Canvas text;

    // Start is called before the first frame update
    void Start()
    {
        Canvas[] texts = transform.GetComponentsInChildren<Canvas>();
        foreach (Canvas item in texts)
        {
            switch (item.name)
            {
                case "HelpCanvas":
                    text = item;
                    break;
            }
        }
        //text.SetActive(false);
        text.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            //gameObject.SetActive(!gameObject.active);
            text.enabled = !text.enabled;
        }
    }
}
