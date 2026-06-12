using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "Maze"; // ƒQ[ƒ€ƒV[ƒ“–¼

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }
}