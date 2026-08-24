using TMPro;
using UnityEngine;

public class RadioUI : MonoBehaviour
{
    [SerializeField] private RadioSystem radioSystem;
    [SerializeField] private TextMeshProUGUI freqText;
    
    [SerializeField] private GameObject radioPanel;

    private void OnEnable()
    {
        if (radioSystem != null)
        {
            radioSystem.OnRadioToggled += ToggleUI;
            radioSystem.OnFrequencyChanged += UpdateFrequencyText;
        }
    }

    private void OnDisable()
    {
        if (radioSystem != null)
        {
            radioSystem.OnRadioToggled -= ToggleUI;
            radioSystem.OnFrequencyChanged -= UpdateFrequencyText;
        }
    }

    private void ToggleUI(bool isActive)
    {
        if (radioPanel != null)
        {
            radioPanel.SetActive(isActive);
        }

        if (isActive && radioSystem != null)
        {
            UpdateFrequencyText(radioSystem.CurrentFrequency);
        }
    }

    private void UpdateFrequencyText(float currentFreq)
    {
        if (freqText != null)
        {
            freqText.text = currentFreq.ToString("F1") + " FM";
        }
    }
}