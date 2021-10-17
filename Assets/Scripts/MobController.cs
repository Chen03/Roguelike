using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobController : MonoBehaviour
{
    // Start is called before the first frame update
    HealthSystem healthSystem;
    void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.HadDead += (object o, EventArgs e) => {
            Debug.Log("QAQ");
            gameObject.SetActive(false);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
