using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Maze"; // ゲームシーン名
    [SerializeField] private GameObject difficultyPanel; // 難易度選択パネル


    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void OnContinue()
    {
        Debug.Log($"[MainMenu] OnContinue呼ばれた HasSave:{SaveManager.Instance != null && SaveManager.Instance.HasSave} Instance:{SaveManager.Instance}");
        if (SaveManager.Instance != null && SaveManager.Instance.HasSave)
            SaveManager.Instance.Load();
    }
    public void OnNewGame()
    {
        difficultyPanel.SetActive(true);
    }

    public void OnSelectEasy()
    {
        DifficultyManager.Instance.SetDifficulty(DifficultyManager.Difficulty.Easy);
        SceneManager.LoadScene("SchoolToShrine");
    }

    public void OnSelectNormal()
    {
        DifficultyManager.Instance.SetDifficulty(DifficultyManager.Difficulty.Normal);
        SceneManager.LoadScene("SchoolToShrine");
    }

    public void OnSelectHard()
    {
        DifficultyManager.Instance.SetDifficulty(DifficultyManager.Difficulty.Hard);
        SceneManager.LoadScene("SchoolToShrine");
    }
}