using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{

    [Header("房间信息")]
    public GameObject roomPrefab;
    public int roomNumber;
    public Color startColor, endColor;

    [Header("位置控制")]
    public Transform generatorPoint;
    public (int x, int y) offset = (0, 0);
    public float xDis = 10, yDis = 10;
    
    HashSet<(int x, int y)> occupied = new HashSet<(int x, int y)>();
    List<Room> rooms = new List<Room>();
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 233; ++i)
            rooms.Add(Instantiate(roomPrefab, GetRoomPosition(), Quaternion.identity).GetComponent<Room>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 GetRoomPosition() {
        (int x, int y) tmp;
        do {
            tmp = Random.Range(0, 3) switch
            {
                0   =>  (offset.x + 1, offset.y),
                1   =>  (offset.x - 1, offset.y),
                2   =>  (offset.x, offset.y + 1),
                3   =>  (offset.x, offset.y - 1),
            };
        } while (occupied.Contains(tmp));
        occupied.Add(tmp);
        offset = tmp;
        // Debug.Log("x:" + tmp.x + "y:" + tmp.y);
        return new Vector3(xDis * tmp.x, yDis * tmp.y, 0);
    }
}
