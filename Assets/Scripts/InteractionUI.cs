using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance;

    [SerializeField] private GameObject promptPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    private void Awake()
    {
        // Singleton basico para llamarlo facilmente desde cualquier trigger
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShowPrompt(string message)
    {
        if (promptText != null) promptText.text = message;
        if (promptPanel != null) promptPanel.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptPanel != null) promptPanel.SetActive(false);
    }
}