using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width, height;
    private System.Random random = new System.Random();

    [SerializeField] private Transform root;

    public int floors = 3;
    public float floorHeight = 10f;

    private MazeCellModel[,,] mazes;
    //í«â¡
    private GameObject[,,] roomObjects;

    [SerializeField] private GameObject floorPrefab;
    //ñ¿òHÇïîâÆÇ≤Ç∆Ç…äÆëSÉâÉìÉ_ÉÄÇ…Ç∑ÇÈÇÃÇ…ê›íË
    [SerializeField] private GameObject[] roomPrefabs;
    [SerializeField] private GameObject corridorPrefab;

    [SerializeField] private float roomSpacing = 20f;
    [SerializeField] private float corridorWidth = 4f;
    //äKíi
    [SerializeField] private GameObject stairRoomPrefabA;
    [SerializeField] private GameObject stairRoomPrefabB;

    

    public void ClearMaze()
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (Transform child in root)
        {
            tempList.Add(child.gameObject);
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            SafeDestroy(tempList[i]);
        }
    }

    public void GenerateMaze()
    {
        ClearMaze();

        mazes = new MazeCellModel[floors, width, height];
        roomObjects = new GameObject[floors, width, height];

        int stairX = random.Next(width);
        int stairY = random.Next(height);

        for (int f = 0; f < floors; f++)
        {
            float floorY = f * floorHeight;
            float mazeY = floorY + 0.05f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mazes[f, x, y] = new MazeCellModel();
                }
            }

            GenerateMaze(f, 0, 0);

            // ïîâÆÇëSïîêÊÇ…ê∂ê¨
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float posX = x * roomSpacing;
                    float posY = mazeY;
                    float posZ = y * roomSpacing;

                    GameObject roomPrefab;

                    if (x == stairX && y == stairY)
                    {
                        if (f == 0)
                        {
                            roomPrefab = stairRoomPrefabA;
                        }
                        else if (f == 1)
                        {
                            roomPrefab = stairRoomPrefabB;
                        }
                        else
                        {
                            roomPrefab = roomPrefabs[random.Next(roomPrefabs.Length)];
                        }
                    }
                    else
                    {
                        roomPrefab = roomPrefabs[random.Next(roomPrefabs.Length)];
                    }

                    GameObject room = Instantiate(
                        roomPrefab,
                        new Vector3(posX, posY, posZ),
                        Quaternion.identity,
                        root
                    );

                    room.name = $"Room_F{f + 1}_{x}-{y}";
                    roomObjects[f, x, y] = room;
                }
            }
            
            // DoorìØémÇí òHÇ≈ê⁄ë±

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    MazeCellModel cell = mazes[f, x, y];

                    GameObject currentRoom = roomObjects[f, x, y];

                    if (!cell.HasWall(MazeCellModel.Wall.Top) && y + 1 < height)
                    {
                        GameObject nextRoom = roomObjects[f, x, y + 1];

                        CreateCorridorBetweenDoors(
                            currentRoom.transform.Find("DoorTop"),
                            nextRoom.transform.Find("DoorBottom"),
                            $"Corridor_F{f + 1}_{x}-{y}_Top"
                        );
                        SafeDestroy(currentRoom.transform.Find("DoorTop").gameObject);
                        SafeDestroy(nextRoom.transform.Find("DoorBottom").gameObject);
                    }

                    if (!cell.HasWall(MazeCellModel.Wall.Right) && x + 1 < width)
                    {
                        GameObject nextRoom = roomObjects[f, x + 1, y];

                        CreateCorridorBetweenDoors(
                            currentRoom.transform.Find("DoorRight"),
                            nextRoom.transform.Find("DoorLeft"),
                            $"Corridor_F{f + 1}_{x}-{y}_Right"
                        );
                        SafeDestroy(currentRoom.transform.Find("DoorRight").gameObject);
                        SafeDestroy(nextRoom.transform.Find("DoorLeft").gameObject);
                    }
                }
            }
        }

        //goalManager.CreateGoalAtDeadEnd(maze, width, height, roomSpacing, root);
    }

    private void GenerateMaze(int floor, int x, int y)
    {
        MazeCellModel currentCell = mazes[floor, x, y];
        currentCell.visited = true;

        foreach (var direction in ShuffleDirections())
        {
            int newX = x + direction.Item1;
            int newY = y + direction.Item2;

            if (newX >= 0 && newY >= 0 && newX < width && newY < height)
            {
                MazeCellModel neighbourCell = mazes[floor, newX, newY];

                if (!neighbourCell.visited)
                {
                    neighbourCell.visited = true;
                    currentCell.RemoveWall(direction.Item3);
                    neighbourCell.RemoveWall(direction.Item4);
                    GenerateMaze(floor, newX, newY);
                }
            }
        }
    }

    private List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)> ShuffleDirections()
    {
        List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)> directions = new List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)> {
            (0, 1, MazeCellModel.Wall.Top, MazeCellModel.Wall.Bottom),
            (0, -1, MazeCellModel.Wall.Bottom, MazeCellModel.Wall.Top),
            (-1, 0, MazeCellModel.Wall.Left, MazeCellModel.Wall.Right),
            (1, 0, MazeCellModel.Wall.Right, MazeCellModel.Wall.Left)
        };
        for (int i = 0; i < directions.Count; i++)
        {
            var temp = directions[i];
            int randomIndex = random.Next(i, directions.Count);
            directions[i] = directions[randomIndex];
            directions[randomIndex] = temp;
        }
        return directions;
    }

    private void CreateCorridorBetweenDoors(Transform fromDoor, Transform toDoor, string corridorName)
    {
        if (fromDoor == null || toDoor == null)
        {
            Debug.LogWarning($"DoorÇ™å©Ç¬Ç©ÇËÇ‹ÇπÇÒ: {corridorName}");
            return;
        }

        Vector3 from = fromDoor.position;
        Vector3 to = toDoor.position;

        Vector3 center = (from + to) / 2;
        Vector3 direction = to - from;

        center.y -= 1f;
        GameObject corridor = Instantiate(
            corridorPrefab,
            center,
            Quaternion.identity,
            root
        );

        corridor.name = corridorName;

        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            corridor.transform.localScale = new Vector3(
                Mathf.Abs(direction.x),
                1f,
                corridorWidth
            );
        }
        else
        {
            corridor.transform.localScale = new Vector3(
                corridorWidth,
                1f,
                Mathf.Abs(direction.z)
            );
        }
    }

    private void SafeDestroy(GameObject obj)
    {
        if (obj == null) return;

        if (Application.isPlaying)
        {
            Destroy(obj);
        }
        else
        {
            DestroyImmediate(obj);
        }
    }
}