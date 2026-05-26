using UnityEngine;

public class ItemMenuUI : MonoBehaviour
{
    public void OnClickSlot(int index)
    {
        Debug.Log("Slot " + index + " clicked");
    }
}