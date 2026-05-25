using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;
    public GameObject player;
    public CinemachineBrain cinemachineBrain;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        player.GetComponent<StarterAssets.ThirdPersonController>().enabled = false;
        cinemachineBrain.enabled = false;
    }

    public void Retry()
    {
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
