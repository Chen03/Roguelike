using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomData {
    public string roomPrefab;
    public int[] mobList;
}

[System.Serializable]
public struct Level {
    public string name;
    public int[] geneRooms;
    public int[] specRooms;
    public int beginRoomPrefab;
}

public class RoomGenerator : MonoBehaviour
{

    [Header("房间信息")]
    public int Total = 10;
    public GameObject roomPrefab;
    public int roomNumber;
    public Color startColor, endColor;
    public (int x, int y) center = (0, 0);
    public GameObject centerRoom;
    public ObjectManager objectManager;
    public Transform player;
    public List<Level> levelList = new List<Level>();

    [Header("位置控制")]
    public float xDis = 20, yDis = 10;
    
    HashSet<(int x, int y)> occupied = new HashSet<(int x, int y)>();
    List<Room> rooms = new List<Room>();
    // Start is called before the first frame update
    void Awake()
    {
        levelIter = levelList.GetEnumerator();
    }

    void Start() {
        NextLevel();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Level>.Enumerator levelIter;
    IEnumerator geneIter, specIter;
    Level cur;
    public void NextLevel() {
        if (levelIter.MoveNext()) {
            foreach (var r in rooms)    Destroy(r.gameObject);
            cur = levelIter.Current;
            geneIter = (IEnumerator)cur.geneRooms.GetEnumerator();
            specIter = (IEnumerator)cur.specRooms.GetEnumerator();
            spec = false;
            GenerateRoom(objectManager.roomData[cur.beginRoomPrefab], center);
            foreach(var r in rooms) {
                r.Setup();
            }
            player.position = new Vector3(center.x, center.y, 0);
        } else Debug.Log("Finished!");
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

    // int tot = 0, dis = 0;
    // int maxDis = 0;
    // Room furthestRoom;
    bool spec = false;
    Room GenerateRoom(RoomData roomData, (int x, int y) offset, bool cont = true) {
        // ++tot;
        Room thisRoom = Instantiate(
            Resources.Load<GameObject>(roomData.roomPrefab), 
            new Vector3(offset.x * xDis, offset.y * yDis, 0), Quaternion.identity)
            .GetComponent<Room>();
        rooms.Add(thisRoom);
        occupied.Add(offset);
        // if (maxDis < dis) {
        //     furthestRoom = thisRoom;
        //     maxDis = dis;
        // }
        (int x, int y) tmp = offset;
        bool[] fail = new bool[4];
        int id;
        if (cont)
        while (!(fail[0] && fail[1] && fail[2] && fail[3])) {
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
                    if (spec || !geneIter.MoveNext()) {
                        spec = true;
                        if (!specIter.MoveNext())    break;
                    }
                    GenerateRoom(objectManager.roomData[spec ? (int)specIter.Current : (int)geneIter.Current],
                        tmp, !spec).hasRoom[(id & 2) | ((id & 1) ^ 1)] = true;
                    thisRoom.hasRoom[id] = true;
                }
            }
        }
        // Debug.Log("x:" + tmp.x + "y:" + tmp.y);
        // return new Vector3(xDis * tmp.x, yDis * tmp.y, 0);
        // --dis;
        thisRoom.Init(roomData.mobList);
        return thisRoom;
    }

}