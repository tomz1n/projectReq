using System.Collections;
using TMPro;
using UnityEngine;

public class RadioUI : MonoBehaviour
{
    [SerializeField] private RadioSystem radioSystem;
    [SerializeField] private TextMeshProUGUI freqText;
    [SerializeField] private GameObject radioPanel;
    
    [SerializeField] private GameObject radioHUDContainer;
    [SerializeField] private TextMeshProUGUI titleHUDText;
    [SerializeField] private TextMeshProUGUI frequencyHUDText;
    [SerializeField] private TextMeshProUGUI keyHUDText;
    
    [SerializeField] private Color normalColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color signalColor = new Color(0.9f, 0.1f, 0.1f, 1f);
    [SerializeField] private float pulseSpeed = 4f;

    private Coroutine _pulseRoutine;
    private bool _inSignalZone = false;

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
        string formattedFreq = currentFreq.ToString("F1") + " FM";

        if (freqText != null)
        {
            freqText.text = formattedFreq;
        }

        if (frequencyHUDText != null)
        {
            frequencyHUDText.text = formattedFreq;
        }
    }

    public void UnlockRadioHUD(bool unlocked = true)
    {
        if (radioHUDContainer != null)
        {
            radioHUDContainer.SetActive(unlocked);
        }
        if (titleHUDText != null) titleHUDText.text = "Radio";
        if (keyHUDText != null) keyHUDText.text = "[Q]";
        
        SetSignalState(false);
    }

    public void SetSignalState(bool inSignal)
    {
        _inSignalZone = inSignal;

        if (_pulseRoutine != null)
        {
            StopCoroutine(_pulseRoutine);
        }

        if (_inSignalZone)
        {
            _pulseRoutine = StartCoroutine(PulseSignalRoutine());
        }
        else
        {
            ApplyColor(normalColor);
        }
    }

    private IEnumerator PulseSignalRoutine()
    {
        while (_inSignalZone)
        {
            float lerp = (Mathf.Sin(Time.unscaledTime * pulseSpeed) + 1f) / 2f;
            Color currentColor = Color.Lerp(normalColor, signalColor, lerp);
            
            ApplyColor(currentColor);
            yield return null;
        }

        ApplyColor(normalColor);
    }

    private void ApplyColor(Color targetColor)
    {
        if (frequencyHUDText != null) frequencyHUDText.color = targetColor;
        if (keyHUDText != null) keyHUDText.color = targetColor;
    }
}