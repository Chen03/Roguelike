using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{

    [Header("房间信息")]
    public int Total = 10;
    public GameObject roomPrefab;
    public int roomNumber;
    public Color startColor, endColor;
    public (int x, int y) center = (0, 0);
    public GameObject centerRoom;

    [Header("位置控制")]
    public float xDis = 20, yDis = 10;
    
    HashSet<(int x, int y)> occupied = new HashSet<(int x, int y)>();
    List<Room> rooms = new List<Room>();
    // Start is called before the first frame update
    void Awake()
    {
        rooms.Add(centerRoom.GetComponent<Room>());
        GenerateRoom(center);
        rooms[0].gameObject.GetComponent<SpriteRenderer>().color = startColor;
        furthestRoom.gameObject.GetComponent<SpriteRenderer>().color = endColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool IsPosVaild((int x, int y) pos) {
        return !occupied.Contains(pos) &&
            (
                (occupied.Contains((pos.x + 1, pos.y)) ? 1 : 0) +
                (occupied.Contains((pos.x - 1, pos.y)) ? 1 : 0) +
                (occupied.Contains((pos.x, pos.y + 1)) ? 1 : 0) +
                (occupied.Contains((pos.x, pos.y - 1)) ? 1 : 0) <= 1
            ) && (
                (Mathf.Pow(pos.x - center.x, 2) + Mathf.Pow(pos.y - center.y, 2))
                    <= Random.Range(5, 10)
            );
    }

    int tot = 0, dis = 0;
    int maxDis = 0;
    Room furthestRoom;
    Room GenerateRoom((int x, int y) offset) {
        ++tot; ++dis;
        Room thisRoom = offset == (0, 0) ? rooms[0] : 
            Instantiate(roomPrefab, new Vector3(offset.x * xDis, offset.y * yDis, 0), Quaternion.identity)
            .GetComponent<Room>();
        rooms.Add(thisRoom);
        occupied.Add(offset);
        if (maxDis < dis) {
            furthestRoom = thisRoom;
            maxDis = dis;
        }
        (int x, int y) tmp = offset;
        bool[] fail = new bool[4];
        int id;
        do {
            id = Random.Range(0, 4);
            if (!fail[id]) {
                tmp = id switch
                {
                    0   =>  (offset.x, offset.y + 1),
                    1   =>  (offset.x, offset.y - 1),
                    2   =>  (offset.x - 1, offset.y),
                    3   =>  (offset.x + 1, offset.y),
                };
                fail[id] = true;
                if (IsPosVaild(tmp)) {
                    GenerateRoom(tmp).hasRoom[(id & 2) | ((id & 1) ^ 1)] = true;
                    thisRoom.hasRoom[id] = true;
                }
            }
        } while (!(fail[0] && fail[1] && fail[2] && fail[3]) && tot <= Total);
        // Debug.Log("x:" + tmp.x + "y:" + tmp.y);
        // return new Vector3(xDis * tmp.x, yDis * tmp.y, 0);
        --dis;
        return thisRoom;
    }

}