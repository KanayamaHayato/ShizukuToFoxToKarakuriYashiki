using UnityEngine;
using Cinemachine;
public class CameraZoom : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    private Cinemachine3rdPersonFollow follow;
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 6f;

    public bool isEnabled = true; // Åöí«â¡

    void Start()
    {
        follow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }
    void Update()
    {
        if (!isEnabled) return; // Åöí«â¡

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            follow.CameraDistance -= scroll * zoomSpeed;
            follow.CameraDistance = Mathf.Clamp(
                follow.CameraDistance,
                minDistance,
                maxDistance
            );
        }
    }
}