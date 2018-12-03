using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseListener : MonoBehaviour
{
    public const int Left = 0;
    public const int Middle = 2;
    public const int Right = 1;

    // define our action delegates
    public delegate void OnMouseDown(RaycastHit hitInfo);
    public delegate void OnMouseUp(RaycastHit hitInfo);

    // create delegate managersl
    protected OnMouseDown mouseDownWatchers;
    protected OnMouseUp mouseUpWatchers;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddMouseDown(OnMouseDown func) => mouseDownWatchers += func;

    public void AddMouseUp(OnMouseUp func) => mouseUpWatchers += func;

    public void RemoveMouseDown(OnMouseDown func) => mouseDownWatchers -= func;

    public void RemoveMouseUp(OnMouseUp func) => mouseUpWatchers -= func;

    internal void MouseOver(RaycastHit hitInfo)
    {
        if (Input.GetMouseButtonDown(Left))
        {
            Debug.Log("On Mouse Down");
            mouseDownWatchers?.Invoke(hitInfo);
            Debug.Log("On Mouse Down watchers called.");
        }
        if (Input.GetMouseButtonUp(Left))
            mouseUpWatchers?.Invoke(hitInfo);
    }


}
