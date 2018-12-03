using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaggisSave : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        HaggisBehaviour hb = other.GetComponent<HaggisBehaviour>();
        if(hb)
        {
            hb.DoSave();
        }
    }
}
