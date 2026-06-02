using UnityEngine;

public class VRMMaterialAdjuster : MonoBehaviour
{
    void Start()
    {
        Debug.Log("VRMMatAdjuster‹N“®");
        foreach (var renderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            Debug.Log($"renderer: {renderer.name}");
            foreach (var mat in renderer.sharedMaterials)
            {
                Debug.Log($"mat: {mat.name} shader: {mat.shader.name}");
                if (mat.shader.name == "VRM/MToon")
                {
                    mat.SetFloat("_ShadeToony", 0.5f);
                    mat.SetFloat("_LightColorAttenuation", 1f);
                    mat.SetColor("_Color", new Color(0.7f, 0.7f, 0.7f, 1f));
                }
            }
        }
    }
}