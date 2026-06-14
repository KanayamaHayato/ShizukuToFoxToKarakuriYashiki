using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) Save();
    }

    public void Save()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        PlayerPrefs.SetInt("SavedSeed", MazeGenerator.LastSeed);
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
        PlayerPrefs.SetInt("HasSave", 1);
        PlayerPrefs.Save();
        Debug.Log($"[Save] セーブ完了 確認HasSave:{PlayerPrefs.GetInt("HasSave", 0)}");
    }

    public bool HasSave => PlayerPrefs.GetInt("HasSave", 0) == 1;

    public int LoadSeed() => PlayerPrefs.GetInt("SavedSeed", 0);
    public Vector3 LoadPlayerPos() => new Vector3(
        PlayerPrefs.GetFloat("PlayerX"),
        PlayerPrefs.GetFloat("PlayerY"),
        PlayerPrefs.GetFloat("PlayerZ")
    );
    public void Load()
    {
        if (!HasSave) return;

        // FixedSeedをセットしてからシーンロード
        loadPending = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Maze");
    }

    public bool loadPending = false;
    public Vector3 pendingPlayerPos;

    public void ConsumePending()
    {
        loadPending = false;
    }
}