using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public float speed;

    HealthSystem healthSystem;
    EventHandler Dead = (object o, EventArgs e) => {
        Debug.Log("233");
    };
    // Start is called before the first frame update
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.HadDead += Dead;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
