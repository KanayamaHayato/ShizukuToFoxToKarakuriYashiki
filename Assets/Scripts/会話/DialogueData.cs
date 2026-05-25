using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class Line
    {
        public string speakerName; // 話者名（空白なら名前欄非表示）
        [TextArea(2, 5)]
        public string text;        // セリフ
    }

    public Line[] lines;
}