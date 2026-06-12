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

    private List<Vector2Int>[] lanternPositions; // フロアごとの灯籠座標

    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private RoomData[] roomDataList;
    [SerializeField] private GameObject corridorPrefab;

    [SerializeField] private GameObject[] lanternRoomPrefabs; // 灯籠部屋プレハブ一覧
    [SerializeField][Range(0, 20)] private int lanternRoomCount = 3; // 灯籠部屋の個数

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

    private Vector2Int startPos;

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

    void Start()
    {
        GenerateMaze();
    }

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
        // ★追加: 事前配置の雫を削除してからSpawnで再生成させる
        playerSpawner.DestroyExisting();

        ClearMaze();
        lanternManager.Reset();
        var roomCount = new Dictionary<string, int>();

        // シード値の設定
        random = useFixedSeed
            ? new System.Random(seed)
            : new System.Random();

        int actualSeed = useFixedSeed ? seed : random.Next();
        Debug.Log($"[MazeGenerator] Seed: {actualSeed}");

        //スタート位置決め
        startPos = new Vector2Int(random.Next(width), random.Next(height));

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

        // ★ 灯籠部屋の位置を決める（全フロア合計でlanternRoomCount個）
        lanternPositions = new List<Vector2Int>[floors];
        for (int f = 0; f < floors; f++)
            lanternPositions[f] = new List<Vector2Int>();

        int totalPlaced = 0;
        int totalTries = 0;

        while (totalPlaced < lanternRoomCount && totalTries < 10000)
        {
            totalTries++;

            int f = random.Next(floors);
            var candidate = new Vector2Int(random.Next(width), random.Next(height));

            // スタート部屋は除外
            if (f == 0 && candidate == Vector2Int.zero) continue;

            // 階段部屋と被らないか確認
            bool stairConflict = false;
            for (int sf = 0; sf < floors; sf++)
            {
                if (stairPositions[sf].Contains(candidate))
                {
                    stairConflict = true;
                    break;
                }
            }
            if (stairConflict) continue;

            // 同じフロアで隣接していないか確認（上下左右）
            bool tooClose = false;
            foreach (var placedPos in lanternPositions[f])
            {
                int dx = Mathf.Abs(candidate.x - placedPos.x);
                int dy = Mathf.Abs(candidate.y - placedPos.y);
                if (dx + dy <= 1)
                {
                    tooClose = true;
                    break;
                }
            }
            if (tooClose) continue;

            // 同じフロアに既に置いてないか確認
            if (lanternPositions[f].Contains(candidate)) continue;

            lanternPositions[f].Add(candidate);
            totalPlaced++;
        }

        if (totalPlaced < lanternRoomCount)
            Debug.LogWarning($"[MazeGenerator] 灯籠部屋を {totalPlaced}/{lanternRoomCount} 個しか置けませんでした。迷路が小さすぎる可能性があります。");

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

                    // スポーンポイント
                    if (f == 0 && x == startPos.x && y == startPos.y)
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

                        // ★ 現在の部屋か隣の部屋どちらかがスタート部屋
                        bool isStart = f == 0 && (
                            (x == startPos.x && y == startPos.y) ||
                            (x == startPos.x && y + 1 == startPos.y)
                        );

                        CreateCorridor(doorTop, doorBottom, name, isStart);
                        SafeDestroy(doorTop);
                        SafeDestroy(doorBottom);
                    }

                    if (!cell.HasWall(MazeCellModel.Wall.Right) && x + 1 < width)
                    {
                        GameObject nextRoom = roomObjects[f, x + 1, y];
                        string name = $"Corridor_F{f + 1}_{x}-{y}_Right";

                        var doorRight = currentRoom.transform.Find("DoorRight");
                        var doorLeft = nextRoom.transform.Find("DoorLeft");

                        // ★ 現在の部屋か隣の部屋どちらかがスタート部屋
                        bool isStart = f == 0 && (
                            (x == startPos.x && y == startPos.y) ||
                            (x + 1 == startPos.x && y == startPos.y)
                        );

                        CreateCorridor(doorRight, doorLeft, name, isStart);
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

        // 迷路生成の最後に追加
        FindObjectOfType<ItemSpawnerManager>()?.SpawnAll();

        // 最下階の床（落下防止）
        GameObject bottomFloor = new GameObject("BottomFloor");
        bottomFloor.transform.parent = root;
        bottomFloor.transform.position = new Vector3(
            (width * roomSpacing) / 2f,
            0.04f,
            (height * roomSpacing) / 2f
        );

        var bfCol = bottomFloor.AddComponent<BoxCollider>();
        bfCol.size = new Vector3(width * roomSpacing + 20f, 1f, height * roomSpacing + 20f);

        // 見た目を追加
        var bfFilter = bottomFloor.AddComponent<MeshFilter>();
        bfFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        var bfRenderer = bottomFloor.AddComponent<MeshRenderer>();
        bfRenderer.material = corridorPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        bottomFloor.transform.localScale = new Vector3(width * roomSpacing + 20f, 1f, height * roomSpacing + 20f);

        // 最上階の天井
        GameObject topCeiling = new GameObject("TopCeiling");
        topCeiling.transform.parent = root;
        topCeiling.transform.position = new Vector3(
            (width * roomSpacing) / 2f,
            (floors) * floorHeight,
            (height * roomSpacing) / 2f
        );
        var tcCol = topCeiling.AddComponent<BoxCollider>();
        tcCol.size = new Vector3(width * roomSpacing + 20f, 1f, height * roomSpacing + 20f);

        // 見た目を追加
        var tcFilter = topCeiling.AddComponent<MeshFilter>();
        tcFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        var tcRenderer = topCeiling.AddComponent<MeshRenderer>();
        tcRenderer.material = corridorCeilingPrefab.GetComponent<MeshRenderer>().sharedMaterial;
        topCeiling.transform.localScale = new Vector3(width * roomSpacing + 20f, 1f, height * roomSpacing + 20f);

        Debug.Log(sb.ToString());

        var enemySpawnManager = FindObjectOfType<EnemySpawnManager>();
        if (enemySpawnManager != null)
        {
            var rooms = new List<GameObject>();
            for (int f = 0; f < floors; f++)
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        if (roomObjects[f, x, y] != null)
                            rooms.Add(roomObjects[f, x, y]);

            enemySpawnManager.RegisterRooms(rooms);
            enemySpawnManager.StartSpawning();
        }

        // GenerateMaze() のフロアループが全部終わった後に追加すること
        StaticBatchingUtility.Combine(root.gameObject);
        Debug.Log("[MazeGenerator] Static Batching 適用完了");
    }

    // ★ 部屋プレハブ選択
    private GameObject ChooseRoomPrefab(int f, int x, int y)
    {
        // スタート部屋
        if (f == 0 && x == startPos.x && y == startPos.y)
            return startRoomPrefab;

        var pos = new Vector2Int(x, y);

        // このフロアに上り階段を置く？
        if (f < floors - 1 && stairPositions[f].Contains(pos))
            return stairRoomPrefabUp;

        // 一つ下のフロアの同位置が上り階段なら、下り階段を置く
        if (f > 0 && stairPositions[f - 1].Contains(pos))
            return stairRoomPrefabDown;

        // ★ 灯籠部屋判定
        if (lanternRoomPrefabs != null && lanternRoomPrefabs.Length > 0
            && lanternPositions[f].Contains(pos))
            return lanternRoomPrefabs[random.Next(lanternRoomPrefabs.Length)];

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
    private void CreateCorridor(Transform fromDoor, Transform toDoor, string corridorName, bool isStartRoom = false)
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

        // ★ 天井高さは常に corridorWallHeight 固定（Inspectorで6に設定）
        float wallHeight = corridorWallHeight;

        // ★ 床のYだけスタート部屋かどうかで変える（扉のPosYを引いて床に合わせる）
        float floorY = Mathf.Min(fromDoor.position.y, toDoor.position.y);
        floorY -= isStartRoom ? 1f : 3f;
        center.y = floorY;

        // 床
        GameObject floor = Instantiate(corridorPrefab, center, Quaternion.identity, root);
        floor.name = corridorName + "_Floor";
        floor.transform.localScale = isX
            ? new Vector3(length, 1f, corridorWidth)
            : new Vector3(corridorWidth, 1f, length);

        // 天井
        if (corridorCeilingPrefab != null)
        {
            Vector3 ceilPos = center + Vector3.up * wallHeight;
            GameObject ceil = Instantiate(corridorCeilingPrefab, ceilPos, Quaternion.identity, root);
            ceil.name = corridorName + "_Ceiling";
            ceil.transform.localScale = floor.transform.localScale;
        }

        // 壁
        if (corridorWallPrefab != null)
        {
            Vector3 wallScale = isX
                ? new Vector3(length, wallHeight, 1f)
                : new Vector3(1f, wallHeight, length);

            Vector3 sideOffsetA = isX
                ? new Vector3(0f, wallHeight / 2f, corridorWidth / 2f)
                : new Vector3(corridorWidth / 2f, wallHeight / 2f, 0f);

            Vector3 sideOffsetB = isX
                ? new Vector3(0f, wallHeight / 2f, -corridorWidth / 2f)
                : new Vector3(-corridorWidth / 2f, wallHeight / 2f, 0f);

            GameObject wallA = Instantiate(corridorWallPrefab, center + sideOffsetA, Quaternion.identity, root);
            wallA.name = corridorName + "_WallA";
            wallA.transform.localScale = wallScale;

            GameObject wallB = Instantiate(corridorWallPrefab, center + sideOffsetB, Quaternion.identity, root);
            wallB.name = corridorName + "_WallB";
            wallB.transform.localScale = wallScale;
        }
        Debug.Log($"[Corridor] {corridorName} isStartRoom={isStartRoom} wallHeight={wallHeight}");
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
                    room
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
    public LanternManager GetLanternManager()
    {
        return lanternManager;
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

    // 隣の部屋を取得するメソッドを追加
    private GameObject[] GetNeighborRooms(int floor, int x, int y)
    {
        var neighbors = new System.Collections.Generic.List<GameObject>();
        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { 1, -1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];
            if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                if (roomObjects[floor, nx, ny] != null)
                    neighbors.Add(roomObjects[floor, nx, ny]);
        }
        return neighbors.ToArray();
    }
}