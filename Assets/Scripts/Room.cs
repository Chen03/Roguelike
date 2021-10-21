using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] wall = new GameObject[4];
    public bool[] hasRoom = new bool[4];
    public ObjectManager objectManager;
    public GameObject box;
    List<Mob> spawnList = new List<Mob>();

    // public event System.EventHandler RoomFinished;

    void Awake()
    {
        objectManager = GameObject.Find("ObjectManager").GetComponent<ObjectManager>();
    }

    public void Init(int[] list) {
        foreach (var i in list) spawnList.Add(objectManager.mobData[i]);
    }

    public void Setup() {
        for (int i = 0; i < 4; ++i)
            wall[i].SetActive(!hasRoom[i]);
    }

    // Update is called once per frame
    int killedCount = 0;
    void Update()
    {
        if (hasEntered && !finished && mobList.Count == killedCount) {
            finished = true;
            for (int i = 0; i < 4; ++i)
                wall[i].SetActive(!hasRoom[i]);
            Instantiate(box, transform.position, Quaternion.identity);
            // RoomFinished?.Invoke(this, System.EventArgs.Empty);
        }
    }

    bool hasEntered = false, finished = false;
    List<MobController> mobList = new List<MobController>();
    void OnTriggerEnter2D(Collider2D col) {
        if (hasEntered || !col.CompareTag("Player")) return;
        hasEntered = true;
        for (int i = 0; i < 4; ++i)
            wall[i].SetActive(true);

        MobController mobController;
        foreach (var m in spawnList) {
            mobController = objectManager.SpawnMob(transform.position + 
                new Vector3(Random.Range(-6.5f, 6.5f), Random.Range(-6.5f, 6.5f), 0), m);
            mobList.Add(mobController);
            mobController.healthSystem.HadDead += 
                (object sender, System.EventArgs e) => {
                    ++killedCount;};
        }
    }
}
