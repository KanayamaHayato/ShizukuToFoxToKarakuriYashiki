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

        maze = new MazeCellModel[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = new MazeCellModel();
            }
        }
        GenerateMaze(0, 0);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float posX = x * cellScale;
                float posZ = y * cellScale;

                MazeCell cell = Instantiate(
                    mazeCellPrefab,
                    new Vector3(posX, 0f, posZ),
                    Quaternion.identity,
                    root).GetComponent<MazeCell>();

                cell.transform.localScale *= cellScale;
                cell.name = $"{x}-{y}";
                cell.Setup(maze[x, y]);
            }
        }
    }

    private void GenerateMaze(int x, int y)
    {
        MazeCellModel currentCell = maze[x, y];
        currentCell.visited = true;

        foreach (var direction in ShuffleDirections())
        {
            int newX = x + direction.Item1;
            int newY = y + direction.Item2;
            if (newX >= 0 && newY >= 0 && newX < width && newY < height)
            {
                MazeCellModel neighbourCell = maze[newX, newY];
                if (!neighbourCell.visited)
                {
                    neighbourCell.visited = true;
                    currentCell.RemoveWall(direction.Item3);
                    neighbourCell.RemoveWall(direction.Item4);
                    GenerateMaze(newX, newY);
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