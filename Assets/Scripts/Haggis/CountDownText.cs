using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownText : CountDownTimer
{
    TextMesh textMesh;
    MeshRenderer textRenderer;

    // Start is called before the first frame update
    void Start()
    {
        TextMesh[] components = GetComponentsInChildren<TextMesh>();
        foreach(TextMesh component in components)
        {
            if(component.CompareTag("Text"))
            {
                textMesh = component;
                textRenderer = textMesh.GetComponent<MeshRenderer>();
            }
        }
    }
    public void OnEnable()
    {
        if(textRenderer)
            textRenderer.enabled = false;
    }

    public void StartCountDown(float startAt)
    {
        active = true;
        lastFireTime = Time.time;
        interval = startAt;
        countDownType = CountDownType.Once;
        textRenderer.enabled = true;
        pauseFactor = 0.0f;
        isPaused = false;
    }

    public float GetTimeRemaining()
    {
        return interval - (Time.time - lastFireTime);
    }

    // Update is called once per frame
    private new void Update()
    {
        if(active)
        {
            int remain = (int) Mathf.Floor(GetTimeRemaining());
            if (remain > 0)
                textMesh.text = remain.ToString();
            else
                textMesh.text = "!";

            Vector3 dir = textRenderer.transform.position - Camera.main.transform.position;
            dir.y = 0.0f;
            textRenderer.transform.LookAt(dir);
        }
        base.Update();
    }
}
