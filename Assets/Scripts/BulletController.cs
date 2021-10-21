using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Bullet : System.ICloneable
{
    public string sprite;
    public float speed = 10, damage = 1, maxTime = 0;
    public object Clone() => MemberwiseClone();
}

public class BulletController : MonoBehaviour
{
    public ObjectManager objectManager;
    
    SpriteRenderer spriteRenderer;
    new BoxCollider2D collider;
    new Rigidbody2D rigidbody;
    Bullet bullet;
    string target;
    // Start is called before the first frame update
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidbody.velocity.magnitude > 1e-9)
            transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, rigidbody.velocity));
    }

    public void Init(Bullet bullet, Vector2 direction, string target, float usedTime) {
        this.bullet = bullet;
        this.target = target;
        if (bullet.maxTime > 1e-9)  bullet.damage *=
            System.Math.Min(usedTime, bullet.maxTime) / bullet.maxTime;
        spriteRenderer.sprite = 
            Resources.Load<Sprite>(bullet.sprite);
        rigidbody.AddForce(direction * bullet.speed);
        if (rigidbody.velocity.magnitude > 1e-9)
            transform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, rigidbody.velocity));
        collider.size = spriteRenderer.sprite.bounds.size * 0.75f;
    }

    //TODO
    void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag(target)) {
            col.GetComponent<IController>().GetHit(bullet.damage, 
                GetComponent<Rigidbody2D>().velocity.normalized
            );
        } else if (col.CompareTag("Wall")) {}
        else return;
        gameObject.SetActive(false);
        objectManager.BulletQueue.Enqueue(this);
    }

    void Recycle() {
    }
}
