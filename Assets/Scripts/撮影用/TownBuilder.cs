using System.Collections.Generic;
using UnityEngine;

public class TownBuilder : MonoBehaviour
{
    [SerializeField] private Sprite[] smallHouseSprites;  // ¬‚³‚¢‰Æ
    [SerializeField] private Sprite[] largeHouseSprites;  // ƒ}ƒ“ƒVƒ‡ƒ“E‹“@

    //š Sprite‚ğ’¼Ú“o˜^
    [SerializeField] private int rowCount = 4;
    [SerializeField] private float rowSpacing = 3f;
    [SerializeField] private float streetLength = 100f;
    [SerializeField] private float randomGap = 1f;
    [SerializeField] private int seed = 0;

    void Start()
    {
        BuildTown();
    }

    public void BuildTown()
    {
        var random = new System.Random(seed);

        for (int row = 0; row < rowCount; row++)
        {
            float z = row * rowSpacing;
            float x = 0f;

            // š Œã‚ë‚Ì—ñ‚Ù‚Ç‘å‚«‚¢Œš•¨‚ğg‚¤
            bool useLarge = row >= rowCount - 2 && largeHouseSprites.Length > 0;
            Sprite[] pool = useLarge ? largeHouseSprites : smallHouseSprites;

            // š ‘å‚«‚¢Œš•¨‚ÍÅŒã‚É”z’u‚µ‚½ˆÊ’u‚ğ‹L˜^
            float lastLargeX = -999f;

            while (x < streetLength)
            {
                Sprite sprite;

                if (useLarge)
                {
                    // š ‘å‚«‚¢Œš•¨‚ÍŠÔŠu‚ğ‚ ‚¯‚é
                    if (x - lastLargeX < 20f)
                    {
                        // ŠÔŠu‚ª‘«‚è‚È‚¢ê‡‚Í¬‚³‚¢‰Æ‚Å–„‚ß‚é
                        if (smallHouseSprites.Length > 0)
                        {
                            sprite = smallHouseSprites[random.Next(smallHouseSprites.Length)];
                        }
                        else
                        {
                            x += 10f;
                            continue;
                        }
                    }
                    else
                    {
                        sprite = pool[random.Next(pool.Length)];
                        lastLargeX = x;
                    }
                }
                else
                {
                    sprite = pool[random.Next(pool.Length)];
                }

                float width = sprite.bounds.size.x;
                float height = sprite.bounds.size.y;

                GameObject obj = new GameObject($"House_{row}_{x}");
                obj.transform.parent = transform;

                SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
                sr.sprite = sprite;
                sr.sortingOrder = -row; // š Œã‚ë‚Ì—ñ‚Íè‘O‚æ‚èŒã‚ë‚É•`‰æ

                Vector3 pos = transform.position + new Vector3(x + width / 2f, 0f, z);
                obj.transform.position = pos;
                obj.transform.rotation = Quaternion.identity;

                float gap = useLarge
                    ? (float)random.NextDouble() * randomGap + 5f  // ‘å‚«‚¢Œš•¨‚ÍŒ„ŠÔL‚ß
                    : (float)random.NextDouble() * randomGap;

                x += width + gap;
            }
        }
    }
}