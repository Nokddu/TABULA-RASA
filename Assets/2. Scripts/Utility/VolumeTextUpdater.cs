using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VolumeTextUpdater : MonoBehaviour
{
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(UpdateText);

        UpdateText(volumeSlider.value);
    }

    public void UpdateText(float value)
    {
        int percentage = Mathf.RoundToInt(value * 100);

        volumeText.text = percentage + "%";
    }
}