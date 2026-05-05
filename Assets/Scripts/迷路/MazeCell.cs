using System.Collections.Generic;
using UnityEngine;

public class MazeCellModel
{
    public enum Wall { Top, Bottom, Left, Right }
    public bool visited = false;
    private Dictionary<Wall, bool> walls = new Dictionary<Wall, bool> {
        { Wall.Top, true },
        { Wall.Bottom, true },
        { Wall.Left, true },
        { Wall.Right, true }
    };

    public void RemoveWall(Wall wall)
    {
        walls[wall] = false;
    }

    public bool HasWall(Wall wall)
    {
        return walls[wall];
    }
}

public class MazeCell : MonoBehaviour
{
    [SerializeField] private GameObject[] wallAarray = new GameObject[] { };

    public void Setup(MazeCellModel mazeCellModel)
    {
        wallAarray[(int)MazeCellModel.Wall.Top].SetActive(mazeCellModel.HasWall(MazeCellModel.Wall.Top));

        for (int i = 0; i < (int)MazeCellModel.Wall.Right + 1; i++)
        {
            wallAarray[i].SetActive(mazeCellModel.HasWall((MazeCellModel.Wall)i));
        }
    }
}