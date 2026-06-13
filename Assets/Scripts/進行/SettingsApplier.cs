using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class SettingsApplier : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessVolume;

    void Start()
    {
        AudioListener.volume = PlayerPrefs.GetFloat("Volume", 1f);
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", 2));

        if (postProcessVolume.profile.TryGetSettings(out AutoExposure ae))
            ae.keyValue.value = PlayerPrefs.GetFloat("Brightness", 1f);
    }
}