using UnityEngine;
using UnityEngine.Events;

public class InteractableTrigger : MonoBehaviour
{
    [SerializeField] private string interactMessage = "Press [E] to interact";
    
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