using UnityEngine;

public class VictoryZone : MonoBehaviour
{
    private bool _hasWon = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasWon) return;

        if (other.CompareTag("Player"))
        {
            _hasWon = true;

            MenuManager menu = FindObjectOfType<MenuManager>();
            if (menu != null)
            {
                menu.ShowVictory();
            }
        }
    }
}