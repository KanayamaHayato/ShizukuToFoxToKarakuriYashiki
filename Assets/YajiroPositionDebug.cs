using UnityEngine;
public class YajiroPositionDebug : MonoBehaviour
{
    private Vector3 lastPos;
    void Update()
    {
        if (transform.position != lastPos)
        {
            Debug.Log($"[YajiroPos] à íuïœâª: {lastPos} Å® {transform.position}");
            lastPos = transform.position;
        }
    }
}