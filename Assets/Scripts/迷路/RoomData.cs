// RoomData.cs（新規）
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "Maze/RoomData")]
public class RoomData : ScriptableObject
{
    public string roomName;
    public GameObject prefab;

    [Range(0f, 1f)]
    public float weight = 1f; // 出現率の重み
}