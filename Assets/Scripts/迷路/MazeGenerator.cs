using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public int width, height;
    private System.Random random = new System.Random();
    private MazeCellModel[,] maze;

    public GameObject mazeCellPrefab;
    [SerializeField] private Transform root;
    private float cellScale = 5f;

    [SerializeField] private GoalManager goalManager;

    public int floors = 3;
    public float floorHeight = 10f;

    private MazeCellModel[,,] mazes;

    [SerializeField] private GameObject floorPrefab;

    private Vector2Int[] holePositions;

    void Start()
    {
        GenerateMaze();
    }

    public void ClearMaze()
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (Transform child in root)
        {
            tempList.Add(child.gameObject);
        }
        for (int i = 0; i < tempList.Count; i++)
        {
            DestroyImmediate(tempList[i]);
        }
    }

    public void GenerateMaze()
    {
        ClearMaze();

        mazes = new MazeCellModel[floors, width, height];

        holePositions = new Vector2Int[floors];

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

            Vector2Int holePos = new Vector2Int(-1, -1);

            if (f == 1 || f == 2)
            {
                MazeCellModel[,] floorMaze = GetFloorMaze(f);
                holePos = goalManager.GetRandomDeadEnd(floorMaze, width, height);
            }

            holePositions[f] = holePos;

            // ƒZƒ‹‚²‚Æ‚É°¶¬
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (holePos.x == x && holePos.y == y)
                    {
                        continue; // ‚±‚±‚¾‚¯°‚ðì‚ç‚È‚¢ = ŒŠ
                    }

                    GameObject floor = Instantiate(
                        floorPrefab,
                        new Vector3(
                            x * cellScale,
                            floorY,
                            y * cellScale
                        ),
                        Quaternion.identity,
                        root
                    );

                    floor.name = $"Floor_F{f + 1}_{x}-{y}";
                    floor.transform.localScale = new Vector3(
                        cellScale,
                        0.05f,
                        cellScale
                    );
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float posX = x * cellScale;
                    float posY = mazeY;
                    float posZ = y * cellScale;

                    MazeCell cell = Instantiate(
                        mazeCellPrefab,
                        new Vector3(posX, posY, posZ),
                        Quaternion.identity,
                        root
                    ).GetComponent<MazeCell>();

                    cell.transform.localScale = new Vector3(cellScale, 2f, cellScale);
                    cell.name = $"Maze_F{f + 1}_{x}-{y}";
                    cell.Setup(mazes[f, x, y]);
                }
            }
        }

        for (int f = 1; f < floors; f++)
        {
            Vector2Int hole = holePositions[f];

            if (hole.x == -1) continue;

            float posX = hole.x * cellScale;
            float posZ = hole.y * cellScale;

            float posY = (f - 1) * floorHeight + 1f;

            goalManager.CreateGoal(
                new Vector3(posX, posY, posZ),
                root
            );
        }
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

    private MazeCellModel[,] GetFloorMaze(int floor)
    {
        MazeCellModel[,] floorMaze = new MazeCellModel[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                floorMaze[x, y] = mazes[floor, x, y];
            }
        }

        return floorMaze;
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
}