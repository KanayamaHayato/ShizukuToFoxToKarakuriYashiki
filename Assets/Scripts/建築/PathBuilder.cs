using System.Collections.Generic;
using UnityEngine;

public class PathBuilder : MonoBehaviour
{
    [SerializeField] private GameObject stonePrefab;  // 石畳プレハブ
    [SerializeField] private float spacing = 1f;      // 石畳の間隔
    [SerializeField] private Transform[] controlPoints; // コントロールポイント

    public void BuildPath()
    {
        // 既存の石畳を削除
        List<GameObject> tempList = new List<GameObject>();
        foreach (Transform child in transform)
            tempList.Add(child.gameObject);
        foreach (var go in tempList)
            DestroyImmediate(go);

        if (controlPoints.Length < 2) return;

        // コントロールポイント間を補間して石畳を配置
        float totalLength = 0f;
        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < controlPoints.Length - 1; i++)
        {
            Vector3 p0 = controlPoints[i].position;
            Vector3 p1 = controlPoints[i + 1].position;

            int steps = Mathf.CeilToInt(Vector3.Distance(p0, p1) / spacing);
            for (int j = 0; j < steps; j++)
            {
                float t = j / (float)steps;
                points.Add(Vector3.Lerp(p0, p1, t));
            }
        }
        points.Add(controlPoints[controlPoints.Length - 1].position);

        // 石畳を配置
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 pos = points[i];

            // 次のポイントの方向を向く
            Quaternion rot = Quaternion.identity;
            if (i < points.Count - 1) {
                Vector3 dir = points[i + 1] - pos;
                dir.y = 0f; // ★ Y軸を無視して水平方向だけ向く
                rot = Quaternion.LookRotation(dir);
            }

            Instantiate(stonePrefab, pos, rot, transform);
        }
    }

    public void ClearPath()
    {
        List<GameObject> tempList = new List<GameObject>();
        foreach (Transform child in transform)
            tempList.Add(child.gameObject);
        foreach (var go in tempList)
            DestroyImmediate(go);
    }
}