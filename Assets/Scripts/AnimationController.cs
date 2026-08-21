using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    
    [SerializeField] private Animator animator;
    
    private const string speed = "Speed";

    private int speedHashed;

    private void Awake()
    {
        speedHashed = Animator.StringToHash(speed);
    }

    private void OnEnable()
    {
        playerMovement.OnMovement += OnMovement;
    }

    private void OnDisable()
    {
        playerMovement.OnMovement -= OnMovement;
    }

    private void OnMovement(Vector2 input)
    {
        animator.SetFloat(speedHashed, input.y);
    }
}
