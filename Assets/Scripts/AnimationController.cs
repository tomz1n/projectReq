using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;
    
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float stepVolume = 0.5f;
    
    [SerializeField] private float dampingTime = 0.1f;

    private const string speed = "Speed";
    private int speedHashed;

    private float _targetSpeed;

    private void Awake()
    {
        speedHashed = Animator.StringToHash(speed);
    }

    private void OnEnable()
    {
        if (playerMovement != null)
            playerMovement.OnMovement += OnMovement;
    }

    private void OnDisable()
    {
        if (playerMovement != null)
            playerMovement.OnMovement -= OnMovement;
    }

    private void OnMovement(Vector2 input)
    {
        if (input.y > 0)
        {
            _targetSpeed = Input.GetKey(KeyCode.LeftShift) ? 2f : 1f;
        }
        else if (input.y < 0)
        {
            _targetSpeed = -1f;
        }
    }

    private void Update()
    {
        if (animator == null) return;

        if (Input.GetAxisRaw("Vertical") == 0)
        {
            _targetSpeed = 0f;
        }
        
        animator.SetFloat(speedHashed, _targetSpeed, dampingTime, Time.deltaTime);
    }

    public void PlayFootstepSound()
    {
        if (_targetSpeed == 0f && animator.GetFloat(speedHashed) < 0.15f) return;

        if (footstepSounds == null || footstepSounds.Length == 0 || footstepAudioSource == null) return;

        float currentAnimSpeed = Mathf.Abs(animator.GetFloat(speedHashed));
        
        if (currentAnimSpeed > 1.2f)
        {
            footstepAudioSource.pitch = UnityEngine.Random.Range(1.1f, 1.25f);

            if (footstepAudioSource.isPlaying) footstepAudioSource.Stop(); 
        }
        else
        {
            footstepAudioSource.pitch = UnityEngine.Random.Range(0.85f, 1.05f);
        }

        AudioClip clip = footstepSounds[UnityEngine.Random.Range(0, footstepSounds.Length)];
        footstepAudioSource.PlayOneShot(clip, stepVolume);
    }
}