using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    System.Reflection.Assembly assembly;
    // Start is called before the first frame update
    public ItemController ItemPrefab;
    public BulletController BulletPrefab;
    public MobController MobPrefab;

    public DataLibrary<Mob> mobData;
    public DataLibrary<Weapon> weaponData;
    public DataLibrary<Bullet> bulletData;
    public DataLibrary<Item> itemData;
    public DataLibrary<RoomData> roomData;
    void Awake() {
        foreach (var a in System.AppDomain.CurrentDomain.GetAssemblies()) {
            if (a.GetName().Name == "Assembly-CSharp") {
                assembly = a;
                break;
            }
        }

        mobData = new DataLibrary<Mob>("Data/Mob");
        weaponData = new DataLibrary<Weapon>("Data/Weapon");
        bulletData = new DataLibrary<Bullet>("Data/Bullet");
        itemData = new DataLibrary<Item>("Data/Item");
        roomData = new DataLibrary<RoomData>("Data/Room");
    }

    void Start()
    {

        // SpawnMob(Vector2.zero, mobData[0]);
        // SpawnMob(Vector2.zero, mobData[1]);
        // CreateItem(Vector2.up, itemData[0]);
        // CreateItem(Vector2.down, itemData[1]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Queue<ItemController> ItemQueue = new Queue<ItemController>();
    public ItemController CreateItem(Vector2 position, Item itemData) {
        ItemController item;
        if (ItemQueue.Count > 0) {
            item = ItemQueue.Dequeue();
            item.transform.position = position;
            item.gameObject.SetActive(true);
        }
        else {
            item = Instantiate(ItemPrefab, position, Quaternion.identity);
            item.objectManager = this;
        }
        item.Init(itemData);
        return item;
    }

    public Queue<BulletController> BulletQueue = new Queue<BulletController>();
    public void CreateBullet(Vector2 position, Vector2 direction, Bullet bulletData, string target, float usedTime) {
        BulletController bullet;
        if (BulletQueue.Count > 0) {
            bullet = BulletQueue.Dequeue();
            bullet.transform.position = position;
            bullet.gameObject.SetActive(true);
        } else {
            bullet = Instantiate(BulletPrefab, position, Quaternion.identity);
            bullet.objectManager = this;
        }
        bullet.Init(bulletData, direction, target, usedTime);
    }

    public Queue<MobController> MobQueue = new Queue<MobController>();
    public MobController SpawnMob(Vector2 position, Mob mobData) {
        MobController mob;
        if (MobQueue.Count > 0) {
            mob = MobQueue.Dequeue();
            mob.transform.position = position;
            mob.gameObject.SetActive(true);
        } else {
            mob = Instantiate(MobPrefab, position, Quaternion.identity);
            mob.objectManager = this;
            mob.assembly = assembly;
        }
        mob.Init(mobData);
        return mob;
    }
}


public class DataLibrary<T> {
    public List<T> data = new List<T>();

    public class DataWrapper {
        public List<T> data = new List<T>();
    }

    public DataLibrary(string dataFile) {
        TextAsset text = Resources.Load<TextAsset>(dataFile);
        data = JsonUtility.FromJson<DataWrapper>(text.text).data;
    }

    public T this[int i] => (data[i] is ICloneable) ? 
        (T)(data[i] as ICloneable).Clone() : data[i];
}