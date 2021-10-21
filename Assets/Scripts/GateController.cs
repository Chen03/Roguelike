using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateController : MonoBehaviour
{
    RoomGenerator roomGenerator;

    void Awake() {
        roomGenerator = GameObject.Find("RoomGenerator").GetComponent<RoomGenerator>();
    }

    void OnTriggerEnter2D(Collider2D col) {
        roomGenerator.NextLevel();
    }
}
