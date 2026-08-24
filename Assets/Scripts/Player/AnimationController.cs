using System;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private Animator animator;
    
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] footstepSounds;
    [SerializeField] private float stepVolume = 0.5f;
    
    [SerializeField] private WeaponHandler weaponHandler;
    
    [SerializeField] private float dampingTime = 0.1f;

    private const string Speed = "Speed";
    private const string Aiming = "IsAiming";
    
    private int _speedHashed;
    private int _aimingHashed;

    private float _targetSpeed;

    private void Awake()
    {
        _speedHashed = Animator.StringToHash(Speed);
        _aimingHashed = Animator.StringToHash(Aiming);
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
        
        if (weaponHandler != null)
        {
            animator.SetBool(_aimingHashed, weaponHandler.IsAiming);
        }
        
        animator.SetFloat(_speedHashed, _targetSpeed, dampingTime, Time.deltaTime);
    }

    public void PlayFootstepSound()
    {
        if (_targetSpeed == 0f && animator.GetFloat(_speedHashed) < 0.15f) return;

        if (footstepSounds == null || footstepSounds.Length == 0 || footstepAudioSource == null) return;

        float currentAnimSpeed = Mathf.Abs(animator.GetFloat(_speedHashed));
        
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