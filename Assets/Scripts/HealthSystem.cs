using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct HealthData {
    public float HP;
    public float MaxHP;
    public float EP;
    public float MaxEP;
}

public class HealthSystem : MonoBehaviour
{
    [Header("数值")]
    public HealthData health;
    public event EventHandler HadDead;
    public event EventHandler BeingAttacked;
    public bool HasEP {get => health.EP != 0;}
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ModifyHP(float value) {
        health.HP = Math.Min(Math.Max(0, health.HP + value), health.MaxHP);
        if (value < 0)  BeingAttacked?.Invoke(this, EventArgs.Empty);
        if (health.HP == 0) HadDead?.Invoke(this, EventArgs.Empty);
    }

    public void ModifyEP(float value) {
        health.EP = Math.Min(health.MaxEP, Math.Max(0, health.EP + value));
    }
}
