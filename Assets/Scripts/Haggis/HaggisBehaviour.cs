using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaggisBehaviour : MonoBehaviour
{
    public enum Action { Walking, Hop, Blocking, Building, Bombing, Exploding, Leaping, Falling, FastFalling, Dead, None, Parashooting, Celebrating };
    public Action currentAction;

    public HaggisSenses senses;
    public CountDownText countDownText;
    public Rigidbody rigidBody;
    public Transform PraxisEffect;
    public float explosionRadius = 5.0f;
    public float explosionSpeed = 3.0f;
    public float explosionForce = 10.0f;

    public Transform BuildStepBlock;

    public int actionsRemaining;        // a countdown for repeated action

    protected Action repeatingActionType = Action.None;
    protected HaggisCommand orderedBy = null;

    // define our action delegates
    public delegate void OnActionEnter(Action newAction, Action oldAction);
    public delegate void OnActionExit(Action currentAction, Action newAction);

    // create delegate managers
    protected OnActionEnter actionEnterWatchers;
    protected OnActionExit actionExitWatchers;

    public float fallSpeed = 0.001f;
    public float fastFallSpeed = 5.0f;

    public int numberOfBricks = 5;

    HaggisActionController actionController;

    // Start is called before the first frame update
    void Start()
    {
        senses = GetComponent<HaggisSenses>();
        rigidBody = GetComponent<Rigidbody>();
        countDownText = GetComponent<CountDownText>();

        currentAction = Action.Falling;

    }

    private void OnEnable()
    {
        currentAction = Action.Falling;

        MouseListener mouseListener = GetComponent<MouseListener>();
        if (mouseListener)
            mouseListener.AddMouseDown(ClickedOn);

        repeatingActionType = Action.None;
        actionsRemaining = 0;
        orderedBy = null;
    }

    private void OnDisable()
    {
        MouseListener mouseListener = GetComponent<MouseListener>();
        if (mouseListener)
            mouseListener.RemoveMouseDown(ClickedOn);
        orderedBy = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAction != Action.Celebrating)
        {
            if (rigidBody.velocity.y < -fallSpeed && !IsFallingAction(currentAction))
                SetCurrentAction(Action.Falling);
            if (rigidBody.velocity.y < -fastFallSpeed && currentAction != Action.FastFalling)
                SetCurrentAction(Action.FastFalling);
            if (transform.position.y < -10.0f)
                SetCurrentAction(Action.Exploding);
        }
        switch (currentAction)
        {
            case Action.Walking:
                // walk forward until hit something.
                /*if(senses.IsAhead(HaggisSenses.InteractableType.Wall,3))
                {
                    HaggisSenses.WorldAhead details = senses.WhatAhead(HaggisSenses.InteractableType.Wall, 3);
                    HaggisMotion motionController = GetComponent<HaggisMotion>();
                    if (motionController)
                    {
                        motionController.BounceOff(details.position, details.normal);
                    }
                }*/
                if (senses.IsAhead(HaggisInteractable.InteractableType.Hop, 3))
                {
                    Debug.LogFormat("Detected Hop ahead: ID {0}", GetInstanceID());
                    SetCurrentAction(Action.Hop);
                }

                // if reach an edge perform a leap into the unknown.
                if (senses.IsAhead(HaggisInteractable.InteractableType.None, 2))
                {
                    Debug.Log("Leap!", this);
                    SetCurrentAction(Action.Leaping); // leap at unknown thing.
                }
                break;
            case Action.Exploding:
                {
                 
                }
                break;
        }

    }

    public void setActionController(HaggisActionController controller)
    {
        actionController = controller;
    }

    public void SetCurrentAction(Action act)
    {
        if (act != currentAction)
        {
            actionExitWatchers?.Invoke(currentAction, act);     // call functions watching for a state exit

            Action oldAct = currentAction;
            currentAction = act;

            // clear repeating action if state is a suitable type
            switch (act)
            {
                case Action.Blocking:
                case Action.Building:
                case Action.Bombing:
                case Action.Leaping:
                case Action.Dead:
                case Action.Parashooting:
                    repeatingActionType = Action.None;
                    break;
                case Action.Exploding:
                    DoExplode();
                    break;
            }
            switch (act)
            {
                case Action.Bombing:
                    countDownText.StartCountDown(4.0f);
                    countDownText.AddOnFire(BoomTime);
                    break;
                case Action.Building:
                    repeatingActionType = Action.Building;
                    break;
            }

            //Debug.LogFormat("Called Enter Action from ID:{0}", GetInstanceID());
            actionEnterWatchers?.Invoke(act, oldAct);    // call functions watching for an enter state
        }
    }


    private void BoomTime()
    {
        SetCurrentAction(Action.Exploding);
    }

    internal void DoDie()
    {
        SetCurrentAction(Action.Exploding);
        actionController.KillHaggis(this);
    }

    internal void DoSave()
    {
        SetCurrentAction(Action.Celebrating);
        StartCoroutine(DoSaveDance());
    }

    IEnumerator DoSaveDance()
    {
        rigidBody.velocity = Vector3.zero;
        for (int i = 0; i < 3; ++i)
        {
            rigidBody.AddRelativeForce(Vector3.up * 100.0f);
            yield return new WaitForSeconds(1.0f);
        }
        actionController.SaveHaggis(this);
        yield return null;
    }

    public void StartBuilding()
    {
        orderedBy = null;
        StartCoroutine(DoBuild(transform.position, transform.rotation));
        SetCurrentAction(Action.Building);
        actionsRemaining--;
    }

    internal void TryBuildStep(HaggisCommand haggisBuildMore, Vector3 position, Quaternion rotation)
    {
        if (actionsRemaining >= 0 && repeatingActionType == Action.Building)  // if we have any building actions remaining
        {
            if (orderedBy != haggisBuildMore)
            {
                orderedBy = haggisBuildMore;
                StartCoroutine(DoBuild(position, rotation));
                SetCurrentAction(Action.Building);
                actionsRemaining--;
            }
        }
    }

    // delegate watchers
    public void AddOnActionEnter(OnActionEnter func)
    {
        actionEnterWatchers += func;
    }

    public void AddOnActionExit(OnActionExit func)
    {
        actionExitWatchers += func;
    }


    // some helper functions
    public bool IsFallingAction(Action act) => act == Action.Falling || act == Action.FastFalling || act == Action.Leaping;

    public bool IsFalling() => IsFallingAction(currentAction);

    private void OnCollisionEnter(Collision collision)
    {
        HaggisInteractable interactable = collision.gameObject.GetComponent<HaggisInteractable>();
        if (interactable)
            if (interactable.type == HaggisInteractable.InteractableType.Ground && !DoingSomething())
            {
                if (currentAction == Action.FastFalling)
                    SetCurrentAction(Action.Exploding);
                else
                    SetCurrentAction(Action.Walking);
            }
            else if (interactable.type == HaggisInteractable.InteractableType.Hop)
            {
                // attempt to hop over or onto the object.
                SetCurrentAction(Action.Hop);
            }
    }

    private void OnCollisionStay(Collision collision)
    {
        HaggisInteractable interactable = collision.gameObject.GetComponent<HaggisInteractable>();
        if (interactable)
        {
            if (interactable.type == HaggisInteractable.InteractableType.Ground && !DoingSomething())
            {
                if (Vector3.Dot(rigidBody.velocity, Vector3.up) < 0.001f)
                    SetCurrentAction(Action.Walking);
            }
            interactable.DoStepOn(this);
        }
    }

    private bool DoingSomething()
    {
        switch (currentAction)
        {
            case Action.Walking:
            case Action.Hop:
            case Action.Falling:
            case Action.FastFalling:
            case Action.Leaping:
            case Action.None:
            case Action.Parashooting:
                return false;
            case Action.Blocking:
            case Action.Building:
            case Action.Bombing:
            case Action.Exploding:
            case Action.Dead:
            case Action.Celebrating:
            default:
                return true;
        }
    }

    private void ClickedOn(RaycastHit hitInfo) => actionController.OnSelectHaggis(this, hitInfo);

    private IEnumerator DoBuild(Vector3 position, Quaternion rotation)
    {
        Transform go = Instantiate(BuildStepBlock, position, rotation);
        yield return new WaitForSeconds(3);
        HaggisBehaviour hb = GetComponent<HaggisBehaviour>();
        hb.SetCurrentAction(HaggisBehaviour.Action.Walking);
    }

    private IEnumerator DoBuild(Transform transform)
    {
        Transform go = Instantiate(BuildStepBlock, transform.position, transform.rotation);
        yield return new WaitForSeconds(3);
        HaggisBehaviour hb = GetComponent<HaggisBehaviour>();
        hb.SetCurrentAction(HaggisBehaviour.Action.Walking);
    }

    private void DoExplode()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider obj in objects)
        {
            HaggisBehaviour hb = obj.GetComponent<HaggisBehaviour>();
            if (hb)
            {
                //hb.rigidBody.AddExplosionForce(explosionForce, transform.position, explosionRadius, 5.0f);
                Vector3 dir = hb.transform.position - transform.position;
                dir.Normalize();
                hb.rigidBody.AddForce(dir * explosionForce, ForceMode.Impulse);

            }

            HaggisExplodable he = obj.GetComponent<HaggisExplodable>();
            if (he)
            {
                he.Detonate();
            }
        }
        DoDie();
    }

    private IEnumerator DoPraxisEffect(Transform praxis)
    {
        praxis.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        for (float r = 0.0f; r < explosionRadius; r += explosionSpeed*Time.deltaTime)
        {
            Debug.Log(r);
            praxis.localScale = new Vector3(r, r, r);
            yield return null;
        }
        Destroy(praxis);
        DoDie();
    }
}