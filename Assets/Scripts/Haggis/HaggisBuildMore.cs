using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaggisBuildMore : HaggisCommand
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void DoStepOn(HaggisBehaviour haggisBehaviour)
    {            
        haggisBehaviour.TryBuildStep(this, this.transform.position + Vector3.up,haggisBehaviour.transform.rotation);
    }
}
