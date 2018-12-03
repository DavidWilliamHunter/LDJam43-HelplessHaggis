using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaggisSpawner : CountDownTimer
{
    protected LevelManager levelManager;
    protected HaggisActionController haggisActionController;
    protected Animator anim;

    public int NumberToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        haggisActionController = levelManager.GetComponent<HaggisActionController>();
        if (!levelManager)
            Debug.LogError("What! No level manager?");
        countDownType = CountDownType.Number;
        countDownTypeParam1 = NumberToSpawn;
        AddOnFire(SpawnHaggis);

        anim = GetComponent<Animator>();
        if (anim)
            anim.speed = 0;
    }

    public void OnEnable()
    {
        anim = GetComponent<Animator>();
        if (anim)
            anim.speed = 0;
    }

    public void StartSpawning()
    {
        anim.StartPlayback();
        //anim.Play();
        anim.speed = 1.0f;
        SetActive(true);
    }

    void SpawnHaggis()
    {
        GameObject go = haggisActionController.InstantiateHaggis(transform);
        HaggisBehaviour hb = go.GetComponent<HaggisBehaviour>();
        if(hb)
        {
            hb.setActionController(haggisActionController);
        }
    }
}
