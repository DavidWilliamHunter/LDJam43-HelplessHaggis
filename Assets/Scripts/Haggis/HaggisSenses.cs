using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

public class HaggisSenses : MonoBehaviour
{

    public int numberOfLookAheadRays = 4;

    public struct WorldAhead
    {
        public Vector3 position;  // ray point collision with object
        public Vector3 normal;    // surface normal at collision point
        public Collider other;     // object collided with
        public HaggisInteractable.InteractableType type;  // type of interaction
        public Vector3 lookAheadVector; // look ahead direction
        public bool isHit;              // true if hit. note position, normal, other and type are not defined if isHit is false;

        public WorldAhead(Vector3 lookAheadVector)
        {
            this.isHit = false;
            this.position = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
            this.normal = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
            this.other = null;
            this.type = HaggisInteractable.InteractableType.None;
            this.lookAheadVector = lookAheadVector;
        }

        public WorldAhead(Vector3 lookAheadVector, RaycastHit hit)
        {
            this.position = hit.point;
            this.normal = hit.normal;
            this.other = hit.collider;
            HaggisInteractable interactable = hit.collider.GetComponent<HaggisInteractable>();
            if (interactable)
            {
                switch (interactable.type)
                {
                    case HaggisInteractable.InteractableType.Ground:
                        this.type = HaggisInteractable.InteractableType.Ground;
                        break;
                    case HaggisInteractable.InteractableType.Wall:
                        this.type = HaggisInteractable.InteractableType.Wall;
                        break;
                    case HaggisInteractable.InteractableType.Hop:
                        this.type = HaggisInteractable.InteractableType.Hop;
                        break;
                    default:
                        this.type = HaggisInteractable.InteractableType.None;
                        break;
                }
            }
            else
                this.type = HaggisInteractable.InteractableType.None;
            this.isHit = true;
            this.lookAheadVector = lookAheadVector;

        }
    }

    // store a variable describing the world immediately ahead.
    public List<WorldAhead> worldAhead = new List<WorldAhead>();

    protected bool grounded = false;

    // Start is called before the first frame update
    void OnEnable()
    {
        ConstructLookAheadVectors(numberOfLookAheadRays);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateLookAhead();
    }

    private void ConstructLookAheadVectors(int count)
    {
        worldAhead.Clear();
        for (int i = 0; i < count; ++i)
        {
            Vector3 lookAhead = Vector3.forward * (((float)i) / 2.0f);
            lookAhead -= Vector3.up;
            lookAhead.Normalize();

            // create empty world ahead
            WorldAhead ahead = new WorldAhead(lookAhead);
            worldAhead.Add(ahead);

            Debug.DrawRay(transform.position, lookAhead, Color.yellow, 10.0f, false);
        }
    }

    private void UpdateLookAhead()
    {
        List<WorldAhead> newWorld = new List<WorldAhead>();
        foreach (WorldAhead ahead in worldAhead)
        {
            RaycastHit hitInfo;
            WorldAhead newAhead;
            if (Physics.Raycast(transform.position, ahead.lookAheadVector, out hitInfo))
            {
                Vector3 transformedLookAhead = transform.TransformDirection(ahead.lookAheadVector);

                Debug.DrawRay(transform.position, transformedLookAhead, Color.blue, 1.0f, false);

                newAhead = new WorldAhead(ahead.lookAheadVector, hitInfo);
            }
            else
                newAhead = new WorldAhead(ahead.lookAheadVector);

            newWorld.Add(newAhead);
        }
        worldAhead = newWorld;
    }

    public HaggisInteractable.InteractableType Below()
    {
        if (worldAhead.Count > 0)
        {
            if (worldAhead[0].isHit)
                return worldAhead[0].type;
            else
                return HaggisInteractable.InteractableType.None;
        }
        Debug.LogError("HaggisSenses:WorldAhead list is empty!", this);
        return HaggisInteractable.InteractableType.None;
    }

    // returns true if the obstacle type is detected ahead
    public bool IsAhead(HaggisInteractable.InteractableType type, int maxDist)
    {
        for (int i = 0; i < worldAhead.Count && i < maxDist; ++i)
        {
            WorldAhead ahead = worldAhead[i];
            if (ahead.isHit)
            {
                if (ahead.type == type)
                    return true;
            }
            else if (type == HaggisInteractable.InteractableType.None)
                return true;
        }
        return false;
    }

    // returns Where the first location of obstacle type is detected ahead. Lots of infs if there is not obstacle of type  detected ahead.
    Vector3 WhereAhead(HaggisInteractable.InteractableType type, int maxDist)
    {
        for (int i = 0; i < worldAhead.Count && i < maxDist; ++i)
        {
            WorldAhead ahead = worldAhead[i];
            if (ahead.isHit)
            {
                if (ahead.type == type)
                    return ahead.position;
            }
        }
        return new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    }

    public WorldAhead WhatAhead(HaggisInteractable.InteractableType type, int maxDist)
    {
        for (int i = 0; i < worldAhead.Count && i < maxDist; ++i)
        {
            WorldAhead ahead = worldAhead[i];
            if (ahead.isHit)
            {
                if (ahead.type == type)
                    return ahead;
            }
        }
        return new WorldAhead(new Vector3(0.0f, 0.0f, 0.0f));
    }

    public bool isGrounded()
    {
        return grounded;
    }

    // Detect what we are currently interacting with.
    private void OnCollisionEnter(Collision collision)
    {

    }

    private void OnCollisionStay(Collision collision)
    {
        HaggisInteractable interactable = collision.gameObject.GetComponent<HaggisInteractable>();
        if (interactable)
            if (interactable.type == HaggisInteractable.InteractableType.Ground)
            {
                grounded = true;
            }
    }

    private void OnCollisionExit(Collision collision)
    {
        HaggisInteractable interactable = collision.gameObject.GetComponent<HaggisInteractable>();
        if (interactable)
            if (interactable.type == HaggisInteractable.InteractableType.Ground)
            {
                grounded = false;
            }
    }
}
