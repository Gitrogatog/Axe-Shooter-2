using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class LevelGenerator : MonoBehaviour
{
    public static LevelGenerator instance;
    enum gridSpace {empty, floor, wall};
    gridSpace[,] grid;
    Vector2 roomSizeWorldUnits = new Vector2(30,30);
    float worldUnitsInOneGridCell = 1;
    int roomHeight;
    int roomWidth;
    public MapSetup[] maps;
    public int currentMap = 0;

    public Vector2 playerStartPos;
    List<Vector2> allFloorCoords;
    Queue<Vector2> shuffledFloorCoords;
    public int seed;
    
    struct walker
    {
        public Vector2 dir;
        public Vector2 pos;
    }
    List<walker> walkers;
    [System.Serializable]
    public class MapSetup{ //maps[currentMap].
        public Vector2 roomSizeWorldUnits;
        public float chanceWalkerChangeDir = .5f, chanceWalkerSpawn = .05f;
        public float chanceWalkerDestroy = .05f;
        public int maxWalkers = 10;
        public float percentToFill = .2f;
    }
    
    float chanceWalkerChangeDir = .5f, chanceWalkerSpawn = .05f;
    float chanceWalkerDestroy = .05f;
    int maxWalkers = 10;
    float percentToFill = .2f;
    public GameObject floorObj;
    public GameObject wallObj;

    void Awake(){
        if(instance != null){
            Destroy(gameObject);
        }
        else{
            instance = this;
        }
    }

    public void CreateMap(int newMap){
        currentMap = newMap;
        if(currentMap >= maps.Length){
            currentMap = maps.Length - 1;
        }
        allFloorCoords = new List<Vector2>();
        Setup();
        CreateFloors();
        CreateWalls();
        //RemoveSingleWalls();
        SpawnLevel();
        AstarPath.active.Scan();
    }

    void Setup()
    {
        roomHeight = Mathf.RoundToInt(maps[currentMap].roomSizeWorldUnits.x / worldUnitsInOneGridCell);
        roomWidth = Mathf.RoundToInt(maps[currentMap].roomSizeWorldUnits.y / worldUnitsInOneGridCell);
        grid = new gridSpace[roomWidth, roomHeight];
        for (int x = 0; x < roomWidth-1; x++)
        {
            for (int y = 0; y < roomHeight-1; y++)
            {
                grid[x, y] = gridSpace.empty;
            }
        }
        walkers = new List<walker>();
        walker newWalker = new walker();
        newWalker.dir = RandomDirection();
        Vector2 spawnPos = new Vector2(Mathf.RoundToInt(roomWidth / 2.0f), Mathf.RoundToInt(roomHeight / 2.0f));
        newWalker.pos = spawnPos;
        walkers.Add(newWalker);
    }

    Vector2 RandomDirection()
    {
        int choice = Mathf.FloorToInt(Random.value * 3.99f);
        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            default:
                return Vector2.right;
        }
    }

    void CreateFloors()
    {
        
        int iterations = 0;
        do
        {
            foreach (walker myWalker in walkers)
            {
                grid[(int)myWalker.pos.x, (int)myWalker.pos.y] = gridSpace.floor;
            }
            int numberChecks = walkers.Count;
            for (int i = 0; i < numberChecks; i++)
            {
                if (Random.value < maps[currentMap].chanceWalkerDestroy && walkers.Count > 1)
                {
                    walkers.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < walkers.Count; i++)
            {
                if (Random.value < maps[currentMap].chanceWalkerChangeDir)
                {
                    walker thisWalker = walkers[i];
                    thisWalker.dir = RandomDirection();
                    walkers[i] = thisWalker;
                }
            }
            numberChecks = walkers.Count;
            for (int i = 0; i < numberChecks; i++)
            {
                if (Random.value < maps[currentMap].chanceWalkerSpawn && walkers.Count < maps[currentMap].maxWalkers)
                {
                    walker newWalker = new walker();
                    newWalker.dir = RandomDirection();
                    newWalker.pos = walkers[i].pos;
                    walkers.Add(newWalker);
                }
            }
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos += thisWalker.dir;
                walkers[i] = thisWalker;
            }
            for (int i = 0; i < walkers.Count; i++)
            {
                walker thisWalker = walkers[i];
                thisWalker.pos.x = Mathf.Clamp(thisWalker.pos.x, 1, roomWidth - 2);
                thisWalker.pos.y = Mathf.Clamp(thisWalker.pos.y, 1, roomHeight - 2);
                walkers[i] = thisWalker;
            }
            if ((float)NumberOfFloors() / (float)grid.Length > maps[currentMap].percentToFill)
            {
                break;
            }
            iterations++;
        } while (iterations < 100000);
    }

    int NumberOfFloors()
    {
        int count = 0;
        foreach (gridSpace space in grid)
        {
            if (space == gridSpace.floor)
            {
                count++;
            }
        }
        return count;
    }
    
    void SpawnLevel()
    {
        string holderName = "Generated Map";
        if(transform.Find(holderName)){
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < roomWidth; x++)
        {
            for (int y = 0; y < roomHeight; y++)
            {
                switch (grid[x, y])
                {
                    case gridSpace.empty:
                        break;
                    case gridSpace.floor:
                        Spawn(x, y, floorObj, mapHolder, true);
                        break;
                    case gridSpace.wall:
                        Spawn(x, y, wallObj, mapHolder);
                        break;
                }
            }
        }
        shuffledFloorCoords = new Queue<Vector2>(Utility.ShuffleArray(allFloorCoords.ToArray(), seed));
        mapHolder.position = new Vector3(mapHolder.position.x, mapHolder.position.y, 5);
    }

    void Spawn(float x, float y, GameObject toSpawn, Transform tileHolder, bool isFloor=false)
    {
        Vector2 offset = maps[currentMap].roomSizeWorldUnits / 2.0f;
        Vector2 spawnPos = new Vector2(x, y) * worldUnitsInOneGridCell - offset;
        GameObject tile = Instantiate(toSpawn, spawnPos, Quaternion.identity);
        tile.transform.parent = tileHolder;
        if(isFloor){
            allFloorCoords.Add(spawnPos);
        }
    }

    void CreateWalls()
    {
        for (int x = 0; x < roomWidth - 1; x++)
        {
            for (int y = 0; y < roomHeight-1; y++)
            {
                if (grid[x, y] == gridSpace.floor)
                {
                    if (grid[x, y + 1] == gridSpace.empty)
                    {
                        grid[x, y + 1] = gridSpace.wall;
                    }
                    if (grid[x, y - 1] == gridSpace.empty)
                    {
                        grid[x, y - 1] = gridSpace.wall;
                    }
                    if (grid[x + 1, y] == gridSpace.empty)
                    {
                        grid[x + 1, y] = gridSpace.wall;
                    }
                    if (grid[x - 1, y] == gridSpace.empty)
                    {
                        grid[x - 1, y] = gridSpace.wall;
                    }
                }
            }
        }
    }

    public Vector2 GetRandomFloorCoord(){
        return shuffledFloorCoords.Dequeue();
    }

    public bool FloorSpaceAvailable(){
        Debug.Log(shuffledFloorCoords.Count > 0);
        return shuffledFloorCoords.Count > 0;
    }

    public Vector2 GetRandFloorCoordNearPoint(Vector2 objectPos){
        Vector2[] array = Utility.ShuffleArray(allFloorCoords.ToArray(), Random.Range(1, 100));
        foreach(Vector2 tile in array){
            if((tile - objectPos).sqrMagnitude < 6f){
                return tile;
            }
        }
        return objectPos;
    }
}
