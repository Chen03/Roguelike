using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : Interactive
{
    public ObjectManager objectManager;

    void Awake() {
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
    }
    
    public override void Use() {
        objectManager.CreateItem(transform.position, objectManager.itemData[0]);
        GameObject.Destroy(gameObject);
    }
}
