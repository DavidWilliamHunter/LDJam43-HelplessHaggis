using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaggisInteractable : MonoBehaviour
{
    public enum InteractableType { Ground, Wall, Hop, Stop, None, Haggis };
    public InteractableType type;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void DoStepOn(HaggisBehaviour haggisBehaviour)
    {
        HaggisCommand[] commands = GetComponents<HaggisCommand>();
        foreach (HaggisCommand command in commands)
            command.DoStepOn(haggisBehaviour);
    }
}
