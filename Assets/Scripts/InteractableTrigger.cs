using UnityEngine;
using UnityEngine.Events;

public class InteractableTrigger : MonoBehaviour
{
    [Header("UI Message")]
    [SerializeField] private string interactMessage = "Presiona [E] para interactuar";

    [Header("Eventos al Interactuar")]
    public UnityEvent OnInteract;

    private bool _isPlayerInside = false;

    private void Update()
    {
        if (_isPlayerInside && Input.GetKeyDown(KeyCode.E))
        {
            InteractionUI.Instance?.HidePrompt();
            
            OnInteract?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = true;
            InteractionUI.Instance?.ShowPrompt(interactMessage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isPlayerInside = false;
            InteractionUI.Instance?.HidePrompt();
        }
    }
}