using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject[] wall = new GameObject[4];
    public bool[] hasRoom = new bool[4];
    // public GameObject doorLeft, doorRight, doorUp, doorDown;
    // public bool roomLeft, roomRight, roomUp, roomDown;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 4; ++i)
            wall[i].SetActive(!hasRoom[i]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
