using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HealthData {
    public float HP;
    public float EP;
}

public class HealthSystem : MonoBehaviour
{
    [Header("数值")]
    public HealthData health;
    public event EventHandler HadDead;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GetHit(float damage) {
        health.HP = Math.Max(0, health.HP - damage);
        if (health.HP == 0) HadDead?.Invoke(this, EventArgs.Empty);
    }
}
