using System;
using UnityEngine;
using Unity.Cinemachine;

public class CamSwitcher : MonoBehaviour
{
    public Transform player;
    public CinemachineCamera activeCam;

    [Tooltip("Prioridad cuando el jugador entra al trigger")]
    public int priorityOnEnter = 20;
    
    [Tooltip("Prioridad cuando el jugador sale del trigger")]
    public int priorityOnExit = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeCam.Priority = priorityOnEnter;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeCam.Priority = priorityOnExit;
        }
    }
}