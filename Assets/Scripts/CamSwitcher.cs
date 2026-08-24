using System;
using UnityEngine;
using Unity.Cinemachine;

public class CamSwitcher : MonoBehaviour
{
    public Transform player;
    public CinemachineCamera activeCam;
    
    public int priorityOnEnter = 20;
    
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