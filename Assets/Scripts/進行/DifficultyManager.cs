using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty Current { get; private set; } = Difficulty.Normal;

    public int MazeWidth => Current == Difficulty.Hard ? 7 : 5;
    public int MazeHeight => Current == Difficulty.Hard ? 7 : 5;
    public int MazeFloors => Current == Difficulty.Easy ? 2 : 3;
    public int LanternCount => Current == Difficulty.Hard ? 7 : 5;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetDifficulty(Difficulty d) => Current = d;
}