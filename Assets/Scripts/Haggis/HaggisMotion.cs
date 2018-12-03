using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaggisMotion : MonoBehaviour
{
    HaggisBehaviour behaviour;

    Renderer rend;

    Rigidbody rigidBody;
    public ParticleSystem explosionEffect;



    public float walkSpeed = 1.0f;
    public float jumpPower = 5.0f;
    public Vector2 leapPower = new Vector2(2.5f, 7.5f);

    public float acc = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start Called");
        rend = GetComponent<Renderer>();
        rend.enabled = true;

        behaviour = GetComponent<HaggisBehaviour>();
        behaviour.AddOnActionEnter(func: OnActionEnter);

        rigidBody = GetComponent<Rigidbody>();
        rigidBody.maxAngularVelocity = 0.0f;

        /*ParticleSystem [] particleEffects = GetComponentsInChildren<ParticleSystem>(true);
        foreach(ParticleSystem p in particleEffects)
        {
            if (p.CompareTag("Explosion"))
                explosionEffect = p;
        }

        if (explosionEffect)
            explosionEffect.Stop(); 
        else
            Debug.LogError("ExplosionEffect not found!"); */

    }

    private void OnEnable()
    {
        rend = GetComponent<Renderer>();
        rend.enabled = true;
        HaggisActionController.SetActiveAllChildren(transform, true);
    }

    private void OnDisable()
    {
        if (rigidBody)
            rigidBody.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        switch (behaviour.currentAction)
        {
            case HaggisBehaviour.Action.Walking:
                {
                    Vector3 forward2D = rigidBody.velocity;
                    forward2D.y = 0.0f;
                    if (forward2D.magnitude < walkSpeed)
                    {
                        //rigidBody.AddRelativeForce(Vector3.forward * acc);
                        rigidBody.AddRelativeForce(Vector3.forward * walkSpeed, ForceMode.VelocityChange);
                        if (rigidBody.isKinematic)
                            rigidBody.isKinematic = false;
                    }
                    break;
                }
            case HaggisBehaviour.Action.Hop:
                {
                    Vector3 forward2D = rigidBody.velocity;
                    forward2D.y = 0.0f;
                    if (forward2D.magnitude < walkSpeed * 2.0f)
                        rigidBody.AddRelativeForce(Vector3.forward * acc, ForceMode.Acceleration);
                    break;
                }
            case HaggisBehaviour.Action.Blocking:
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
                rigidBody.isKinematic = true;
                break;
            case HaggisBehaviour.Action.Bombing:
            case HaggisBehaviour.Action.Building:
                rigidBody.velocity = Vector3.zero;
                rigidBody.angularVelocity = Vector3.zero;
                rigidBody.isKinematic = true;
                break;
            case HaggisBehaviour.Action.Exploding:
                //if (!explosionEffect.IsAlive())      // finished sploding.
                {

                }
                break;
            default:
                break;
        }


    }

    /*
     * Put Haggis actions here
     */

    public void HaggisHop(float power)
    {
        Debug.Log("Haggis Hop");
        rigidBody.velocity = Vector3.zero;
        rigidBody.AddRelativeForce(Vector3.up * power, ForceMode.Impulse);
        rigidBody.isKinematic = false;
    }

    public void HaggisLeap(float upPower, float forwardPower)
    {
        rigidBody.AddRelativeForce(Vector3.up * upPower, ForceMode.Impulse);
        rigidBody.AddRelativeForce(Vector3.forward * forwardPower, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        HaggisInteractable iteractable = collision.gameObject.GetComponent<HaggisInteractable>();
        if (iteractable)
            if (iteractable.type == HaggisInteractable.InteractableType.Wall)

            {
                // reflect haggis off object.
                if (collision.contacts.Length > 0)
                {
                    ContactPoint contact = collision.contacts[0];
                    BounceOff(contact.point, contact.normal);
                }

            }
            else if (iteractable.type == HaggisInteractable.InteractableType.Haggis)
            {
                if (collision.contacts.Length > 0)
                {
                    switch (behaviour.currentAction)
                    {
                        case HaggisBehaviour.Action.Walking:
                        case HaggisBehaviour.Action.Hop:
                        case HaggisBehaviour.Action.Leaping:
                            ReverseDirection();
                            break;
                    }
                }
            }
    }
    public void BounceOff(Vector3 point, Vector3 normal)
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 reflection = Vector3.Reflect(forward, normal);
        reflection.Normalize();
        Debug.DrawRay(point, -forward, Color.black, 10.0f, false);
        Debug.DrawRay(point, reflection, Color.red, 10.0f, false);
        Debug.DrawRay(point, normal, Color.green, 10.0f, false);
        /*if (Vector3.Dot(reflection, forward) < -0.5f)
        {
            reflection = Quaternion.Euler(0.0f, 90.0f, 0.0f) * forward;
            Debug.DrawRay(point, reflection, Color.red, 10.0f, false);
        }*/
        rigidBody.MoveRotation(Quaternion.LookRotation(reflection, transform.up));
        rigidBody.angularVelocity = new Vector3(0, 0, 0);
    }

    public void ReverseDirection()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 reflection = Quaternion.Euler(0.0f, 180.0f, 0.0f) * forward;
        rigidBody.MoveRotation(Quaternion.LookRotation(reflection, transform.up));
        rigidBody.angularVelocity = new Vector3(0, 0, 0);
    }

    private void OnActionEnter(HaggisBehaviour.Action newAction, HaggisBehaviour.Action oldAction)
    {
        //Debug.LogFormat("OnActionEnter called on ID: {0}", GetInstanceID());
        switch (newAction)
        {
            case HaggisBehaviour.Action.Hop:
                HaggisHop(jumpPower);
                break;
            case HaggisBehaviour.Action.Leaping:
                HaggisLeap(leapPower.x, leapPower.y);
                break;
            case HaggisBehaviour.Action.Bombing:
                rigidBody.velocity = Vector3.zero;
                break;
            case HaggisBehaviour.Action.Exploding:
                //if (explosionEffect)
                ParticleSystem ee = Instantiate(explosionEffect, transform.position, Quaternion.Euler(90.0f, 0.0f, 0.0f));
                if (ee)
                {
                    ee.Play();
                    Destroy(ee.gameObject, ee.startLifetime);
                }
                rend.enabled = false;
                break;
            case HaggisBehaviour.Action.Building:
                {
                    // seems to just play animation automatically                                       
                }
                break;
        }
    }





}
