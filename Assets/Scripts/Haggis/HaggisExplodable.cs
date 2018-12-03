using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HaggisExplodable : MonoBehaviour
{
    public Transform explosionEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Detonate()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);        // remove self.

    }
}
