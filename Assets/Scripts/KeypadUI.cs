using TMPro;
using UnityEngine;

public class KeypadUI : MonoBehaviour
{
    [SerializeField] private GameObject keypadPanel;
    [SerializeField] private TextMeshProUGUI inputDisplay;
    [SerializeField] private PlayerMovement playerMovement;

    private string _currentInput = "";
    private DoorKeypad _currentDoor;

    public void ShowKeypad(DoorKeypad door)
    {
        _currentDoor = door;
        _currentInput = "";
        UpdateDisplay();
        keypadPanel.SetActive(true);

        if (playerMovement != null) playerMovement.enabled = false;
    }

    public void CloseKeypad()
    {
        keypadPanel.SetActive(false);
        _currentDoor = null;

        if (playerMovement != null) playerMovement.enabled = true;
    }

    public void PressNumber(string number)
    {
        if (_currentInput.Length < 4)
        {
            _currentInput += number;
            UpdateDisplay();
        }
    }

    public void PressClear()
    {
        _currentInput = "";
        UpdateDisplay();
    }

    public void PressEnter()
    {
        if (_currentDoor != null)
        {
            if (_currentDoor.CheckCode(_currentInput))
            {
                CloseKeypad();
            }
            else
            {
                _currentInput = "ERR";
                UpdateDisplay();
                Invoke(nameof(PressClear), 0.8f);
            }
        }
    }

    private void UpdateDisplay()
    {
        if (inputDisplay != null)
        {
            inputDisplay.text = _currentInput;
        }
    }
}