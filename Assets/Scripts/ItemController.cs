using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Potion, Sword
}

[System.Serializable]
public class Item
{
    public string sprite;
    public ItemType type;
    public int ID;
    public bool chasing;
}

public class ItemController : MonoBehaviour
{
    public ItemType ItemType;
    public int ID;
    public ObjectManager objectManager;
    public Transform player;

    SpriteRenderer spriteRenderer;
    new BoxCollider2D collider;
    new Rigidbody2D rigidbody;
    Navigator navigator;
    bool chasing;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        navigator = new Navigator(rigidbody, null);
        navigator.speed = 10;
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
        player = GameObject.Find("Player").transform;
        Init(objectManager.itemData[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (chasing) {
            navigator.Goto(player.position);
            navigator.Update();
        }
    }


    public void Init(Item item) {
        this.ItemType = item.type;
        this.ID = item.ID;
        this.chasing = item.chasing;
        spriteRenderer.sprite = Resources.Load<Sprite>(item.sprite);
        collider.size = spriteRenderer.sprite.bounds.size;
    }

    public void Pick() {
        gameObject.SetActive(false);
        objectManager.ItemQueue.Enqueue(this);
    }
}
