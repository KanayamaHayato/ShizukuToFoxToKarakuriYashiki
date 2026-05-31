using System.Collections.Generic;
using UnityEngine;

public class TownBuilder : MonoBehaviour
{
    [SerializeField] private Sprite[] smallHouseSprites;
    [SerializeField] private Sprite[] largeHouseSprites;
    [SerializeField] private Sprite fenceSprite;
    [SerializeField] private int rowCount = 4;
    [SerializeField] private float rowSpacing = 3f;
    [SerializeField] private float streetLength = 100f;
    [SerializeField] private float randomGap = 1f;
    [SerializeField] private int seed = 0;
    [SerializeField] private GameObject endWallPrefab;
    [SerializeField] private float endWallDepth = 20f;
    [SerializeField] private float endWallHeight = 5f;  // 壁の高さ
    [SerializeField] private float endWallWidth = 1f;   // 壁の厚み

    [SerializeField] private Material depthCubeMaterial; // ★ キューブのマテリアル
    [SerializeField] private float depthCubeWidthScale = 0.9f; // ★ 幅の調整（1より小さくすると縮む）
    [SerializeField] private float depthCubeThickness = 0.5f;  // ★ 厚みの調整

    public void ClearTown()
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (Transform child in transform)
            tempList.Add(child.gameObject);
        foreach (var go in tempList)
        {
            if (Application.isPlaying) Destroy(go);
            else DestroyImmediate(go);
        }
    }
    public void BuildTown()
    {
        var random = new System.Random(seed);

        // 塀を一番手前に連続配置
        if (fenceSprite != null)
        {
            float fenceWidth = fenceSprite.bounds.size.x;
            float fx = 0f;
            while (fx < streetLength)
            {
                PlaceSprite(fenceSprite, fx + fenceWidth / 2f, -0.01f, 10, false); // ★ falseを追加
                fx += fenceWidth;
            }
        }

        for (int row = 0; row < rowCount; row++)
        {
            float z = (row + 1) * rowSpacing;
            float x = 0f;

            bool useLarge = row >= rowCount - 2 && largeHouseSprites.Length > 0;
            Sprite[] pool = useLarge ? largeHouseSprites : smallHouseSprites;
            float lastLargeX = -999f;

            while (x < streetLength)
            {
                Sprite sprite;

                if (useLarge)
                {
                    if (x - lastLargeX < 20f)
                    {
                        if (smallHouseSprites.Length > 0)
                            sprite = smallHouseSprites[random.Next(smallHouseSprites.Length)];
                        else { x += 10f; continue; }
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
                if (x + width > streetLength) break;

                PlaceSprite(sprite, x + width / 2f, z, -row, !useLarge);

                float gap = useLarge
                    ? (float)random.NextDouble() * randomGap + 5f
                    : (float)random.NextDouble() * randomGap;
                x += width + gap;
            }
        }

        // 終点にZ方向の壁を生成（forループの外）
        if (endWallPrefab != null)
        {
            GameObject wall = Instantiate(endWallPrefab,
                transform.position + new Vector3(streetLength, endWallHeight / 2f, endWallDepth / 2f),
                Quaternion.identity, transform);
            wall.transform.localScale = new Vector3(endWallWidth, endWallHeight, endWallDepth);
        }
    }

    private void PlaceSprite(Sprite sprite, float x, float z, int sortOrder, bool addDepth = true)
    {
        // 親オブジェクト
        GameObject parent = new GameObject($"Obj_{x}_{z}");
        parent.transform.parent = transform;
        parent.transform.position = transform.position + new Vector3(x, 0f, z);
        parent.transform.rotation = Quaternion.identity;

        // Sprite
        GameObject spriteObj = new GameObject("Sprite");
        spriteObj.transform.parent = parent.transform;
        spriteObj.transform.localPosition = Vector3.zero;

        SpriteRenderer sr = spriteObj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = sortOrder;

        // ★ addDepthがtrueの時だけキューブを追加
        if (addDepth)
        {
            float width = sprite.bounds.size.x;

            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = parent.transform;
            cube.transform.localPosition = new Vector3(0f, sprite.bounds.center.y, depthCubeThickness / 2f + 0.01f);
            cube.transform.localScale = new Vector3(width * depthCubeWidthScale, sprite.bounds.size.y, depthCubeThickness);

            if (depthCubeMaterial != null)
            {
                var r = cube.GetComponent<Renderer>();
                r.material = depthCubeMaterial;
            }
        }
    }
}