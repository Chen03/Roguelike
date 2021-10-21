using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitchIdle : MobState {
    public override void Enter() {
        navigator.speed = 2;
    }

    float lastTime = 0;
    public override void Update()
    {
        if (lastTime + 1.2 < Time.time) {   
            navigator.ChasePlayer(5f, 6f);
            lastTime = Time.time;
        }
    }

    static float attackTime = 0;
    public override System.Type Check()
    {
        if (senseInfo.alert > 8 && attackTime + 10 < Time.time) {
            attackTime = Time.time;
            return typeof(WitchAttack);
        }
        return null;
    }
}

public class WitchAttack : MobState {
    public override void Enter()
    {
        enterTime = Time.time;
        navigator.speed = 3;
    }

    float lastTime = 0;
    public override void Update()
    {
        if (lastTime + 0.6 < Time.time) {
            navigator.ChasePlayer(4f, 6f);
            lastTime = Time.time;
        }
        weapon.Use();
    }

    float enterTime;
    public override System.Type Check()
    {
        if (enterTime + 5f < Time.time) return typeof(WitchIdle);
        return null;
    }
}