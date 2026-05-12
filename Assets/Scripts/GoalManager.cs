using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    [SerializeField] private GameObject goalPrefab;
    private GameObject currentGoal;

    public void CreateGoalAtDeadEnd(
        MazeCellModel[,] maze,
        int width,
        int height,
        float cellScale,
        Transform root)
    {
        List<Vector3> goalPositions = new List<Vector3>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int wallCount = 0;

                if (maze[x, y].HasWall(MazeCellModel.Wall.Top)) wallCount++;
                if (maze[x, y].HasWall(MazeCellModel.Wall.Bottom)) wallCount++;
                if (maze[x, y].HasWall(MazeCellModel.Wall.Left)) wallCount++;
                if (maze[x, y].HasWall(MazeCellModel.Wall.Right)) wallCount++;

                if (wallCount == 3 && !(x == 0 && y == 0))
                {
                    float posX = x * cellScale;
                    float posZ = y * cellScale;

                    goalPositions.Add(
                        new Vector3(posX, 1.5f, posZ)
                    );
                }
            }
        }

        if (goalPositions.Count == 0)
        {
            Debug.LogWarning("Goal‚ð’u‚¯‚és‚«Ž~‚Ü‚è‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñ‚Å‚µ‚½");
            return;
        }

        Vector3 goalPos = goalPositions[Random.Range(0, goalPositions.Count)];

        if (currentGoal != null)
        {
            Destroy(currentGoal);
        }

        currentGoal = Instantiate(goalPrefab, goalPos, Quaternion.identity);
    }
}
