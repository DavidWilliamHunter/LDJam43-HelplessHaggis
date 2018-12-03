using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPicker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CastMouseRay();
    }

    void CastMouseRay()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
        {
            if(hitInfo.transform.gameObject)
            {
                MouseListener mouseListener = hitInfo.transform.GetComponent<MouseListener>();
                if (mouseListener)
                    mouseListener.MouseOver(hitInfo);
            }
        }
    }
}
