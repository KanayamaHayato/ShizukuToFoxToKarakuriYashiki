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

        for (int f = 0; f < floors; f++)
        {
            float floorY = f * floorHeight;
            float mazeY = floorY + 0.05f;

            // °¶¬
            GameObject floor = Instantiate(
                floorPrefab,
                new Vector3(
                    (width - 1) * cellScale / 2f,
                    floorY,
                    (height - 1) * cellScale / 2f
                ),
                Quaternion.identity,
                root
            );

            floor.name = $"Floor_{f + 1}";
            floor.transform.localScale = new Vector3(
                width * cellScale,
                0.05f,
                height * cellScale
            );

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mazes[f, x, y] = new MazeCellModel();
                }
            }

            GenerateMaze(f, 0, 0);

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

        //goalManager.CreateGoalAtDeadEnd(maze, width, height, cellScale, root);
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
}