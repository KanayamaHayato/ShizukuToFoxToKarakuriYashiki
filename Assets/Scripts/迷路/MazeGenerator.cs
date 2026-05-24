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
    private GameObject[,,] roomObjects;

    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private RoomData[] roomDataList;
    [SerializeField] private GameObject corridorPrefab;

    // ★追加: 通路の壁と天井
    [SerializeField] private GameObject corridorWallPrefab;
    [SerializeField] private GameObject corridorCeilingPrefab;
    [SerializeField] private float corridorWallHeight = 3f;

    [SerializeField] private float roomSpacing = 20f;
    [SerializeField] private float corridorWidth = 4f;

    // 階段
    [SerializeField] private GameObject stairRoomPrefabUp;   // 上り用（下の階に置く）
    [SerializeField] private GameObject stairRoomPrefabDown; // 下り用（上の階に置く）

    // ★追加: スタート部屋
    [SerializeField] private GameObject startRoomPrefab;

    // ★追加: 階段の個数
    [SerializeField][Range(1, 10)] private int stairsPerFloor = 1;

    // フィールドに追加
    [SerializeField] private LanternManager lanternManager;

    // フィールドに追加
    [SerializeField] private PlayerSpawner playerSpawner;

    [SerializeField] private bool useFixedSeed = false;
    [SerializeField] private int seed = 0;

    // フロアごとの階段座標リスト
    private List<Vector2Int>[] stairPositions;

    // ----------------------------------------

    public void ClearMaze()
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (Transform child in root)
            tempList.Add(child.gameObject);
        foreach (var go in tempList)
            SafeDestroy(go);
    }

    public void GenerateMaze()
    {
        ClearMaze();
        lanternManager.Reset();
        var roomCount = new Dictionary<string, int>();

        // シード値の設定
        random = useFixedSeed
            ? new System.Random(seed)
            : new System.Random();

        int actualSeed = useFixedSeed ? seed : random.Next();
        Debug.Log($"[MazeGenerator] Seed: {actualSeed}");

        mazes = new MazeCellModel[floors, width, height];
        roomObjects = new GameObject[floors, width, height];

        // ★ フロアごとの階段位置をランダムに決める（重複なし）
        stairPositions = new List<Vector2Int>[floors];
        for (int f = 0; f < floors; f++)
        {
            stairPositions[f] = new List<Vector2Int>();

            if (f < floors - 1) // 最上階には上り階段不要
            {
                int placed = 0;
                int tries = 0;
                while (placed < stairsPerFloor && tries < 1000)
                {
                    tries++;
                    var candidate = new Vector2Int(random.Next(width), random.Next(height));
                    if (!stairPositions[f].Contains(candidate))
                    {
                        stairPositions[f].Add(candidate);
                        placed++;
                    }
                }
            }
        }

        for (int f = 0; f < floors; f++)
        {
            float floorY = f * floorHeight;
            float mazeY = floorY + 0.05f;

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    mazes[f, x, y] = new MazeCellModel();

            GenerateMazeDFS(f, 0, 0);

            // ── 部屋を生成 ──────────────────────────────
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float posX = x * roomSpacing;
                    float posZ = y * roomSpacing;

                    GameObject prefab = ChooseRoomPrefab(f, x, y);

                    GameObject room = Instantiate(
                        prefab,
                        new Vector3(posX, mazeY, posZ),
                        Quaternion.identity,
                        root
                    );
                    room.name = $"Room_F{f + 1}_{x}-{y}";
                    roomObjects[f, x, y] = room;

                    // ★ プレハブ名でカウント
                    string prefabName = prefab.name;
                    if (!roomCount.ContainsKey(prefabName)) roomCount[prefabName] = 0;
                    roomCount[prefabName]++;

                    if (f == 0 && x == 0 && y == 0)
                        playerSpawner.Spawn(room);

                    ValidateRoomDoors(room, room.name);
                    lanternManager.RegisterLanternRoom(room);
                }
            }

            // ── 通路を生成 ──────────────────────────────
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    MazeCellModel cell = mazes[f, x, y];
                    GameObject currentRoom = roomObjects[f, x, y];

                    if (!cell.HasWall(MazeCellModel.Wall.Top) && y + 1 < height)
                    {
                        GameObject nextRoom = roomObjects[f, x, y + 1];
                        string name = $"Corridor_F{f + 1}_{x}-{y}_Top";

                        var doorTop = currentRoom.transform.Find("DoorTop");
                        var doorBottom = nextRoom.transform.Find("DoorBottom");

                        CreateCorridor(doorTop, doorBottom, name);
                        SafeDestroy(doorTop);
                        SafeDestroy(doorBottom);
                    }

                    if (!cell.HasWall(MazeCellModel.Wall.Right) && x + 1 < width)
                    {
                        GameObject nextRoom = roomObjects[f, x + 1, y];
                        string name = $"Corridor_F{f + 1}_{x}-{y}_Right";

                        var doorRight = currentRoom.transform.Find("DoorRight");
                        var doorLeft = nextRoom.transform.Find("DoorLeft");

                        CreateCorridor(doorRight, doorLeft, name);
                        SafeDestroy(doorRight);
                        SafeDestroy(doorLeft);
                    }
                }
            }
        }
        // フロアループが全部終わった後
        var sb = new System.Text.StringBuilder("[MazeGenerator] 部屋の内訳:\n");
        foreach (var kv in roomCount)
            sb.AppendLine($"  {kv.Key} : {kv.Value}個");

        Debug.Log(sb.ToString());
    }

    // ★ 部屋プレハブ選択
    private GameObject ChooseRoomPrefab(int f, int x, int y)
    {
        // (0,0) はスタート部屋（1階のみ）
        if (f == 0 && x == 0 && y == 0)
            return startRoomPrefab;

        var pos = new Vector2Int(x, y);

        // このフロアに上り階段を置く？
        if (f < floors - 1 && stairPositions[f].Contains(pos))
            return stairRoomPrefabUp;

        // 一つ下のフロアの同位置が上り階段なら、下り階段を置く
        if (f > 0 && stairPositions[f - 1].Contains(pos))
            return stairRoomPrefabDown;

        return PickWeightedRoom();
    }

    // MazeGenerator.cs に追加
    private GameObject PickWeightedRoom()
    {
        float totalWeight = 0f;
        foreach (var r in roomDataList)
            totalWeight += r.weight;

        float value = (float)random.NextDouble() * totalWeight;
        float cumulative = 0f;

        foreach (var r in roomDataList)
        {
            cumulative += r.weight;
            if (value <= cumulative)
                return r.prefab;
        }

        // フォールバック
        return roomDataList[roomDataList.Length - 1].prefab;
    }

    // ── DFS 迷路生成 ────────────────────────────────────
    private void GenerateMazeDFS(int floor, int startX, int startY)
    {
        // (x, y) をスタックで管理する
        var stack = new Stack<(int x, int y)>();

        mazes[floor, startX, startY].visited = true;
        stack.Push((startX, startY));

        while (stack.Count > 0)
        {
            var (x, y) = stack.Peek();
            bool moved = false;

            foreach (var dir in ShuffleDirections())
            {
                int nx = x + dir.Item1;
                int ny = y + dir.Item2;

                if (nx >= 0 && ny >= 0 && nx < width && ny < height
                    && !mazes[floor, nx, ny].visited)
                {
                    mazes[floor, nx, ny].visited = true;
                    mazes[floor, x, y].RemoveWall(dir.Item3);
                    mazes[floor, nx, ny].RemoveWall(dir.Item4);
                    stack.Push((nx, ny));
                    moved = true;
                    break; // 1方向進んだら while に戻る
                }
            }

            // どこにも進めなければ戻る（バックトラック）
            if (!moved)
                stack.Pop();
        }
    }

    private List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)> ShuffleDirections()
    {
        var dirs = new List<(int, int, MazeCellModel.Wall, MazeCellModel.Wall)> {
            ( 0,  1, MazeCellModel.Wall.Top,    MazeCellModel.Wall.Bottom),
            ( 0, -1, MazeCellModel.Wall.Bottom, MazeCellModel.Wall.Top),
            (-1,  0, MazeCellModel.Wall.Left,   MazeCellModel.Wall.Right),
            ( 1,  0, MazeCellModel.Wall.Right,  MazeCellModel.Wall.Left)
        };
        for (int i = 0; i < dirs.Count; i++)
        {
            int j = random.Next(i, dirs.Count);
            (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
        }
        return dirs;
    }

    // ── 通路生成（壁・天井付き）──────────────────────
    private void CreateCorridor(Transform fromDoor, Transform toDoor, string corridorName)
    {
        if (fromDoor == null || toDoor == null)
        {
            Debug.LogWarning($"Doorが見つかりません: {corridorName}");
            return;
        }

        Vector3 from = fromDoor.position;
        Vector3 to = toDoor.position;
        Vector3 center = (from + to) / 2f;
        Vector3 direction = to - from;
        float length = direction.magnitude;
        bool isX = Mathf.Abs(direction.x) > Mathf.Abs(direction.z);

        center.y -= 0.99f;

        // 床
        GameObject floor = Instantiate(corridorPrefab, center, Quaternion.identity, root);
        floor.name = corridorName + "_Floor";
        floor.transform.localScale = isX
            ? new Vector3(length, 1f, corridorWidth)
            : new Vector3(corridorWidth, 1f, length);

        // 天井
        if (corridorCeilingPrefab != null)
        {
            Vector3 ceilPos = center + Vector3.up * corridorWallHeight;
            GameObject ceil = Instantiate(corridorCeilingPrefab, ceilPos, Quaternion.identity, root);
            ceil.name = corridorName + "_Ceiling";
            ceil.transform.localScale = floor.transform.localScale;
        }

        // 壁（通路の両側）
        if (corridorWallPrefab != null)
        {
            Vector3 wallScale = isX
                ? new Vector3(length, corridorWallHeight, 1f)
                : new Vector3(1f, corridorWallHeight, length);

            Vector3 sideOffsetA = isX
                ? new Vector3(0f, corridorWallHeight / 2f, corridorWidth / 2f)
                : new Vector3(corridorWidth / 2f, corridorWallHeight / 2f, 0f);

            Vector3 sideOffsetB = isX
                ? new Vector3(0f, corridorWallHeight / 2f, -corridorWidth / 2f)
                : new Vector3(-corridorWidth / 2f, corridorWallHeight / 2f, 0f);

            GameObject wallA = Instantiate(corridorWallPrefab, center + sideOffsetA, Quaternion.identity, root);
            wallA.name = corridorName + "_WallA";
            wallA.transform.localScale = wallScale;

            GameObject wallB = Instantiate(corridorWallPrefab, center + sideOffsetB, Quaternion.identity, root);
            wallB.name = corridorName + "_WallB";
            wallB.transform.localScale = wallScale;
        }
    }

    // 生成時に全Doorの存在を検証するメソッドを追加
    private bool ValidateRoomDoors(GameObject room, string roomName)
    {
        string[] requiredDoors = { "DoorTop", "DoorBottom", "DoorLeft", "DoorRight" };
        bool allFound = true;

        foreach (var doorName in requiredDoors)
        {
            if (room.transform.Find(doorName) == null)
            {
                Debug.LogError(
                    $"[MazeGenerator] {roomName} に '{doorName}' がありません！" +
                    $"\nPrefab: {room.name}",
                    room  // ← クリックするとProjectウィンドウでそのプレハブが選択される
                );
                allFound = false;
            }
        }
        return allFound;
    }

    // ── ユーティリティ ───────────────────────────────
    private void SafeDestroy(GameObject obj)
    {
        if (obj == null) return;
        if (Application.isPlaying) Destroy(obj);
        else DestroyImmediate(obj);
    }

    private void SafeDestroy(Transform t)
    {
        if (t != null) SafeDestroy(t.gameObject);
    }

    // MazeGenerator.cs に追加
    private void OnDrawGizmos()
    {
        if (mazes == null || roomObjects == null) return;

        for (int f = 0; f < floors; f++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (mazes[f, x, y] == null) continue;

                    MazeCellModel cell = mazes[f, x, y];
                    GameObject room = roomObjects[f, x, y];
                    if (room == null) continue;

                    Vector3 pos = room.transform.position;

                    // 部屋の中心を球で表示（フロアごとに色変え）
                    Gizmos.color = f == 0 ? Color.green
                                 : f == 1 ? Color.yellow
                                 : Color.red;
                    Gizmos.DrawWireSphere(pos, 1f);

                    // 接続している方向に線を引く
                    Gizmos.color = Color.white;

                    if (!cell.HasWall(MazeCellModel.Wall.Top) && y + 1 < height && roomObjects[f, x, y + 1] != null)
                        Gizmos.DrawLine(pos, roomObjects[f, x, y + 1].transform.position);

                    if (!cell.HasWall(MazeCellModel.Wall.Right) && x + 1 < width && roomObjects[f, x + 1, y] != null)
                        Gizmos.DrawLine(pos, roomObjects[f, x + 1, y].transform.position);
                }
            }
        }
    }
}