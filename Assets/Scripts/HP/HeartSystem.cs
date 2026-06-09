using UnityEngine;

public class HeartSystem : MonoBehaviour
{
    public GameObject ofudaPrefab;
    public Transform ofudaParent;

    public int life = 5;
    public int maxLife = 7;

    private GameObject[] hearts;

    void Start()
    {
        hearts = new GameObject[maxLife];

        for (int i = 0; i < maxLife; i++)
        {
            GameObject obj = Instantiate(ofudaPrefab, ofudaParent);

            obj.name = "ofuda_" + (i + 1);

            RectTransform rect = obj.GetComponent<RectTransform>();

            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);

            rect.anchoredPosition = new Vector2(75 + (i * 70), -75);

            hearts[i] = obj;
        }

        UpdateHearts();
    }

    public void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(i < life);
        }
    }

    public void TakeDamage(int amount)
    {
        life -= amount;

        if (life < 0)
        {
            life = 0;
        }

        UpdateHearts();
    }

    public void Heal(int amount)
    {
        life += amount;

        if (life > maxLife)
        {
            life = maxLife;
        }

        UpdateHearts();
    }
}