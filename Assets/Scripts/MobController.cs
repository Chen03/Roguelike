using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Mob : ICloneable {
    public string name = "Mob", sprite, animator;
    public string defaultState, anyState;
    public string stateInfoClass;
    public HealthData health;
    public int weaponID;   //-1 to mark none
    public object Clone() => MemberwiseClone();
}

[Serializable]
public class MobSenseInfo {
    public float playerDistance;
    public bool playerIsVisible;
    public bool playerIsWalking;
    public bool mobBeingAttacked;
    public Vector2 playerPosition;
    public Vector2 playerDirection;
    public float alert;
}

public abstract class MobState {
    public GameObject gameObject;
    public Navigator navigator;
    public MobSenseInfo senseInfo;
    public MobStateInfo stateInfo;
    public WeaponController weapon;
    public void Init(GameObject gameObject, Navigator navigator,
        MobSenseInfo senseInfo, MobStateInfo stateInfo, WeaponController weapon) {
        this.gameObject = gameObject;
        this.navigator = navigator;
        this.senseInfo = senseInfo;
        this.stateInfo = stateInfo;
        this.weapon = weapon;
        Enter();
    }
    public abstract void Enter();
    public abstract void Update();
    public abstract Type Check();
}

public abstract class MobStateInfo {

}

public class MobController : MonoBehaviour, IController
{
    // Start is called before the first frame update
    public string Name;
    public HealthSystem healthSystem;
    MobState now, any;
    MobStateInfo stateInfo;
    // public Type defaultState, anyState;

    new Rigidbody2D rigidbody;
    new BoxCollider2D collider;
    public MobSenseInfo senseInfo;
    public Navigator navigator;
    WeaponController weapon;

    GameObject player;
    public ObjectManager objectManager;
    public SpriteRenderer spriteRenderer;
    public Animator spriteAnimator;
    public Assembly assembly;

    void Awake()
    {
        weapon = GetComponent<WeaponController>();
        weapon.TargetTag = "Player";
        player = GameObject.FindWithTag("Player");
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();
        navigator = new Navigator(rigidbody, senseInfo);
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.HadDead += (object o, EventArgs e) => Dead();
    }

    public void Init(Mob mob) {
        Name = mob.name;
        Vector3 ssize = spriteRenderer.sprite.bounds.size;
        spriteRenderer.sprite = Resources.Load<Sprite>(mob.sprite);
        spriteRenderer.transform.localPosition = new Vector3(ssize.x / 2, -ssize.y / 2, 0);
        collider.size = ssize;
        spriteAnimator.runtimeAnimatorController = Resources
            .Load<RuntimeAnimatorController>(mob.animator);
        healthSystem.health = mob.health;
        weapon.Inventory.Clear();
        if (mob.weaponID != -1) {
            weapon.Inventory.Add(objectManager.weaponData[mob.weaponID]);
            weapon.ChangeWeapon();
        }
        if (mob.stateInfoClass != null)
        stateInfo = (MobStateInfo)Activator.CreateInstance(assembly.GetType(mob.stateInfoClass));
        now = CreateNewState(mob.defaultState);
        if (mob.anyState != null)   any = CreateNewState(mob.anyState);
    }

    MobState CreateNewState(string type) => 
        CreateNewState(assembly.GetType(type));
    MobState CreateNewState(Type type) {
        object o = Activator.CreateInstance(type);
        MobState m = o as MobState;
        m.Init(gameObject, navigator, senseInfo, stateInfo, weapon);
        return m;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        SenseUpdate();
        // (senseInfo.playerIsVisible);

        weapon.direction = senseInfo.playerDirection;
        navigator.Update();
        Type type;
        if (any != null) {
            any.Update();
            type = any.Check();
            if (type != null)   now = CreateNewState(type);
        }
        if (now != null) {
            now.Update();
            type = now.Check();
            if (type != null)   now = CreateNewState(type);
        }
    }

    void SenseUpdate() {
        senseInfo.playerPosition = player.transform.position;
        senseInfo.playerDistance = (senseInfo.playerPosition - rigidbody.position).magnitude;
        senseInfo.playerDirection = (senseInfo.playerPosition - rigidbody.position).normalized;
        senseInfo.playerIsVisible = false;
        foreach (var r in Physics2D.RaycastAll(rigidbody.position, senseInfo.playerDirection, 100)) {
            if (r.collider.CompareTag("Player")) {
                senseInfo.playerIsVisible = true;
                break;
            }
        }

        senseInfo.alert = (100 / senseInfo.playerDistance) * (senseInfo.playerIsVisible ? 3 : 1);
    }

    public void GetHit(float damage, Vector2 direction) {
        healthSystem.ModifyHP(-damage);
        senseInfo.mobBeingAttacked = true;
        spriteAnimator.SetTrigger("GetHit");
        // rigidbody.MovePosition(rigidbody.position + direction * 1000);
    }

    void Dead() {
        for (int i = 0; i < UnityEngine.Random.Range(0, 4); ++i)
        objectManager.CreateItem(transform.position + 
            new Vector3(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(-0.5f, 0.5f), 0), 
                objectManager.itemData[1]);
        gameObject.SetActive(false);
        objectManager.MobQueue.Enqueue(this);
    }
}

[Serializable]
public class Navigator {
    public float speed;
    public Rigidbody2D rigidbody;
    public MobSenseInfo senseInfo;
    public Vector2 destination;
    public bool stopped;

    public Navigator(Rigidbody2D rigidbody, MobSenseInfo senseInfo) {
        this.rigidbody = rigidbody;
        this.senseInfo = senseInfo;
    }

    public void Stop() => stopped = true;
    public void Goto(Vector2 d) {
        destination = d;
        stopped = false;
    }

    // public void ChasePlayer() => Goto(senseInfo.playerPosition);
    public void ChasePlayer(float minDis, float maxDis) {
        Goto((rigidbody.position - senseInfo.playerPosition).normalized * 
            UnityEngine.Random.Range(minDis, maxDis) + senseInfo.playerPosition
            + new Vector2(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f)));
    }
    // Goto(
    // ) );

    public void Update() {
        rigidbody.velocity = Vector2.zero;
        if (!stopped)
        rigidbody.MovePosition( rigidbody.position +
            (destination - rigidbody.position).normalized * speed * Time.fixedDeltaTime);
    }
}

public class AnyState : MobState {
    public override void Enter() {}
    public override void Update() {}
    public override Type Check()
    {
        // if (senseInfo.mobBeingAttacked) {
        //     senseInfo.mobBeingAttacked = false;
        //     return typeof(Gethit);
        // }
        return null;
    }
}

public class Idle : MobState {
    public override void Enter() {
        navigator.speed = 2;
    }

    float lastTime = 0;
    public override void Update()
    {
        if (lastTime + 1.2 < Time.time) {   
            navigator.ChasePlayer(0.5f, 2f);
            lastTime = Time.time;
        }
    }

    static float attackTime = 0;
    public override Type Check()
    {
        if (senseInfo.alert > 8 && attackTime + 5 < Time.time) {
            attackTime = Time.time;
            return typeof(Attack);
        }
        return null;
    }
}

public class Attack : MobState {
    public override void Enter()
    {
        enterTime = Time.time;
        navigator.speed = 3;
    }

    float lastTime = 0;
    public override void Update()
    {
        if (lastTime + 0.6 < Time.time) {
            navigator.ChasePlayer(0.5f, 1f);
            lastTime = Time.time;
        }
        weapon.Use();
    }

    float enterTime;
    public override Type Check()
    {
        if (enterTime + 1.5f < Time.time) return typeof(Idle);
        return null;
    }
}

public class Gethit : MobState {
    float lastTime;
    public override void Enter()
    {
        lastTime = Time.time;
        navigator.speed = 0;
    }

    public override void Update() {}


    public override Type Check()
    {
        if (lastTime + 1 < Time.time)   return typeof(Idle);
        return null;
    }
}