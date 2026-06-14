using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private PostProcessVolume postProcessVolume;

    private AutoExposure autoExposure;

    void Start()
    {
        PlayerPrefs.DeleteAll();
        settingsPanel.SetActive(false);

        // OnValueChanged‚ðˆê’UŠO‚·
        volumeSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.onValueChanged.RemoveAllListeners();
        qualityDropdown.onValueChanged.RemoveAllListeners();

        // •Û‘¶‚³‚ê‚½’l‚ð“Ç‚Ýž‚Þ
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        qualityDropdown.value = PlayerPrefs.GetInt("Quality", 2);

        AudioListener.volume = volumeSlider.value;
        QualitySettings.SetQualityLevel(qualityDropdown.value);

        if (postProcessVolume != null)
        {
            if (postProcessVolume.profile.TryGetSettings(out autoExposure))
            {
                autoExposure.keyValue.value = brightnessSlider.value;
                Debug.Log("[Settings] AutoExposureŽæ“¾¬Œ÷");
            }
            else
                Debug.Log("[Settings] AutoExposureŽæ“¾Ž¸”s");
        }
        else
            Debug.LogError("[Settings] postProcessVolume‚ªNULL");

        // OnValueChanged‚ðÄ“o˜^
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    public void OpenSettings() => settingsPanel.SetActive(true);
    public void CloseSettings() => settingsPanel.SetActive(false);

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public void OnBrightnessChanged(float value)
    {
        if (autoExposure != null)
            autoExposure.keyValue.value = value;
        PlayerPrefs.SetFloat("Brightness", value);
        Debug.Log($"[Settings] –¾‚é‚³: {value}");
    }

    public void OnQualityChanged(int value)
    {
        QualitySettings.SetQualityLevel(value);
        PlayerPrefs.SetInt("Quality", value);
        Debug.Log($"[Settings] ‰æŽ¿: {QualitySettings.names[value]}");
    }
}