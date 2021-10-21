using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactive : MonoBehaviour {
    public abstract void Use();
}

public class PlayerController : MonoBehaviour, IController
{
    public float speed;
    public Camera Camera;
    Rigidbody2D rb;
    Vector2 movement, direction;

    HealthSystem healthSystem;
    WeaponController weapon;
    EventHandler Dead = (object o, EventArgs e) => {
        Debug.Log("233");
    };

    public Animator spriteAnimator;
    public ObjectManager objectManager;
    // Start is called before the first frame update
    void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
        healthSystem.HadDead += Dead;
        weapon = GetComponent<WeaponController>();
        weapon.TargetTag = "Mob";
        
        rb = GetComponent<Rigidbody2D>();

        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
    }

    void Start() {
        weapon.Inventory.Add(objectManager.weaponData[1]);
        weapon.ChangeWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        direction = (Camera.ScreenToWorldPoint(Input.mousePosition) - 
            transform.position);
        direction.Normalize();
        weapon.direction = direction;

        if (Input.GetMouseButton(0))        weapon.Use();
        if (Input.GetMouseButtonUp(0))      weapon.EndUse();

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            weapon.ChangeWeapon();

        if (Input.GetMouseButtonDown(1)) {
            foreach (Collider2D c in Physics2D.OverlapCircleAll(
                transform.position, 1f
            )) {
                if (c.CompareTag("Item"))
                    UseItem(c.GetComponent<ItemController>());
                if (c.CompareTag("Interactive"))
                    c.GetComponent<Interactive>().Use();

            }
        }
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Item"))
            UseItem(col.GetComponent<ItemController>());
    }

    void FixedUpdate() {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
        spriteAnimator.SetBool("IsRunning", movement.magnitude > 1e-9);
    }

    public void GetHit(float damage, Vector2 direction) {
        healthSystem.ModifyHP(-damage);
        rb.AddForce(direction);
    }

    void UseItem(ItemController item) {
        item.Pick();
        switch (item.ID) {
            case 0: healthSystem.ModifyHP(1);   break;
            case 1: healthSystem.ModifyEP(5);   break;
        }
    }
}
