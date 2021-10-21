using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Sword, Gun, Bow
}

[System.Serializable]
public class Weapon : System.ICloneable {
    public string name, sprite, animator = "Animation/Weapon/Sword/Sword";
    public float EP = 1, CD;
    public int number = -1;
    public float angle = 0;
    public float distance, damage;
    public int bulletID;
    public WeaponType type;
    public object Clone() => MemberwiseClone();
}

// public class Sword : Weapon
// {
//     public float angle = 30.0f,
//         distance = 2.0f, damage = 1.0f;
// }

// public class Gun : Weapon
// {
//     public float angle = 0;
//     public int bulletNum = 1;
//     public Bullet bullet;
// }

public class WeaponController : MonoBehaviour
{
    public ObjectManager ObjectManager;
    public GameObject WeaponObject;
    public SpriteRenderer weaponSpriteRenderer;
    public Animator weaponAnimator;
    public List<Weapon> Inventory = new List<Weapon>();
    HealthSystem healthSystem;
    Vector2 position;
    public Vector2 direction;
    public string TargetTag;

    // Start is called before the first frame update
    void Awake()
    {
        ObjectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        healthSystem = GetComponent<HealthSystem>();
        // weaponSpriteRenderer = WeaponObject.GetComponent<SpriteRenderer>();
        // weaponAnimator = WeaponObject.GetComponent<Animator>();
        // Inventory.AddRange(
        //     new Weapon[]
        //     {
        //         new Sword() {
        //             name = "Super Sword",
        //             sprite = "Weapon/Swords/weapons_55",
        //             EP = 1, CD = 1,
        //             distance = 100f,
        //             angle = 45
        //         },
        //         new Sword() {
        //             name = "caidao",
        //             sprite = "Weapon/Swords/weapons_54",
        //             EP = 0
        //         },
        //         new Gun() {
        //             name = "Poison",
        //             sprite = "Weapon/Weapons/weapons_19",
        //             bullet = new Bullet() {
        //                 sprite = "Weapon/Bullets/bullet_15",
        //                 speed = 1000
        //             }
        //         }
        //     }
        // );
        // ChangeWeapon(0);
    }

    Weapon holding {get => Inventory.Count > 0 ? Inventory[holdingNum] : null;}
    int holdingNum;
    public void ChangeWeapon(int i = -1) {
        if (i >= Inventory.Count)   return;
        holdingNum = i == -1 ? (holdingNum + 1 >= Inventory.Count ? 0 : holdingNum + 1) : i;
        // if (holding.sprite != null)
        weaponSpriteRenderer.sprite = holding.sprite == null ? null : 
            Resources.Load<Sprite>(holding.sprite);
        if (holding.animator != null)
            weaponAnimator.runtimeAnimatorController = 
                Resources.Load<RuntimeAnimatorController>(holding.animator);
        else    WeaponObject.SetActive(false);
        usedTime = 0;
        beginTime = Time.time;
    }

    void DeleteWeapon(Weapon w) {
        if (w == holding) ChangeWeapon();
        Inventory.Remove(w);
    }

    // Update is called once per frame
    void Update()
    {
        position = transform.position;
        transform.localScale = new Vector3(direction.x > 0 ? 1 : -1, 1, 1);
        
        Debug.DrawRay(position, direction * 
            ((holding != null && holding.type == WeaponType.Sword) ? holding.distance : 5), Color.green);
    }

    float lastTime;
    public void Use() {
        if (holding.EP <= healthSystem.health.EP && lastTime + holding.CD < Time.fixedTime) {
            lastTime = Time.fixedTime;
            switch(holding.type) {
                case WeaponType.Gun:    UseGun();   break;
                case WeaponType.Sword:  UseSword(); break;
                case WeaponType.Bow:    UseBow();   break;
            }
            
        }   //TODO: Warning
    }

    public void EndUse() {
        switch(holding.type) {
            case WeaponType.Bow:    EndUseBow();    break;
        }
    }
    
    void FinishUse() {
        if (holding.number != -1 && --holding.number == 0)
            DeleteWeapon(holding);
        healthSystem.ModifyEP(-holding.EP);
    }

    float beginTime, usedTime;
    bool isUsing = false;
    void UseBow() {
        if (isUsing)    return;
        isUsing = true;
        beginTime = Time.time;
        weaponAnimator.SetBool("IsUsing", true);
    }

    void EndUseBow() {
        if (!isUsing)   return;
        weaponAnimator.SetBool("IsUsing", false);
        usedTime = Time.time - beginTime;
        ObjectManager.CreateBullet(
            position, direction, ObjectManager.bulletData[holding.bulletID], TargetTag, usedTime
        );
        isUsing = false;
        FinishUse();
    }

    void UseSword() {
        WeaponObject.GetComponent<Animator>().SetTrigger("HasUsed");
        var result = RaycastTest(position, direction,
            holding.angle, holding.distance, 50);
        foreach(Collider2D c in result) {
            c.GetComponent<IController>().GetHit(holding.damage, direction);
            Debug.Log(c.name);
        }
        FinishUse();
    }

    HashSet<Collider2D> RaycastTest(Vector2 position, Vector2 direction,
        float angle, float distance, int accuarcy) {
        HashSet<Collider2D> list = new HashSet<Collider2D>();
        direction = Quaternion.Euler(0, 0, angle) * direction;
        accuarcy = System.Math.Max(accuarcy, 2);
        
        RaycastHit2D[] result;
        for (int i = 0; i <= accuarcy; ++i) {
            Debug.DrawRay(position, direction * distance, Color.red, 0.2f);
            result = Physics2D.RaycastAll(position, direction, distance);
            foreach (RaycastHit2D r in result) {
                if (r.collider.CompareTag(TargetTag))
                    list.Add(r.collider);
            }
            direction = Quaternion.Euler(0, 0, -angle * 2 / accuarcy) * direction;
        }

        return list;
    }

    void UseGun() {
        ObjectManager.CreateBullet(
            position, direction, ObjectManager.bulletData[holding.bulletID], TargetTag, usedTime
        );
        FinishUse();
    }
}
