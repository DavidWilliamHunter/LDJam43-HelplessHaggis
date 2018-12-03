using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaggisCommand : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void DoStepOn(HaggisBehaviour haggisBehaviour)
    {
        Debug.LogError("Do not attach base HaggisCommand to objects");
    }
}
