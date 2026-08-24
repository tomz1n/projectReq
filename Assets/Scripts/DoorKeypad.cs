using System.Collections;
using UnityEngine;

public class DoorKeypad : MonoBehaviour
{
    public enum DoorType { Rotate, Slide }
    
    [SerializeField] private string correctCode = "4821";
    [SerializeField] private KeypadUI keypadUI;
    
    [SerializeField] private DoorType doorType = DoorType.Rotate;
    [SerializeField] private Transform doorTransform;
    [SerializeField] private float openSpeed = 2f;
    
    [SerializeField] private Vector3 openRotation = new Vector3(0, 90f, 0);
    [SerializeField] private Vector3 openOffset = new Vector3(0, 3f, 0);

    private bool _isUnlocked = false;
    private bool _playerInsideTrigger = false;

    private void Update()
    {
        if (_playerInsideTrigger && !_isUnlocked && Input.GetKeyDown(KeyCode.E))
        {
            if (keypadUI != null)
            {
                keypadUI.ShowKeypad(this);
            }
        }
    }

    public bool CheckCode(string inputCode)
    {
        if (inputCode == correctCode)
        {
            _isUnlocked = true;
            StartCoroutine(OpenDoorRoutine());
            return true;
        }

        return false;
    }

    private IEnumerator OpenDoorRoutine()
    {
        if (doorTransform == null) yield break;

        if (doorType == DoorType.Rotate)
        {
            Quaternion startRotation = doorTransform.localRotation;
            Quaternion targetRotation = startRotation * Quaternion.Euler(openRotation);
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                doorTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
                yield return null;
            }
        }
        else if (doorType == DoorType.Slide)
        {
            Vector3 startPosition = doorTransform.localPosition;
            Vector3 targetPosition = startPosition + openOffset;
            float t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime * openSpeed;
                doorTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
                yield return null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInsideTrigger = false;
            if (keypadUI != null) keypadUI.CloseKeypad();
        }
    }
    
    public void OpenKeypadUI()
    {
        if (_isUnlocked) return;

        if (keypadUI != null)
        {
            keypadUI.ShowKeypad(this);
        }
    }
}