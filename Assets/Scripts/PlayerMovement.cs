using System;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public event Action<Vector2> OnMovement;
    
    [SerializeField] private MoveStats _moveStats;
    [SerializeField] private KeyCode sprintingKey =  KeyCode.LeftShift;
    
    [SerializeField] private float gravity = -15;
    [SerializeField] private LayerMask groundedLayers;
    [SerializeField] private Vector3 groundedOffset;

    [SerializeField] private CharacterController cc;
    
    [SerializeField] private RadioSystem radioSystem;
    
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private float interactDistance = 2f;
    [SerializeField] private LayerMask interactableLayer;
    
    private const string Horizontal = "Horizontal";
    private const string Vertical = "Vertical";

    private Vector2 _moveInput;
    private bool _sprinting;
    private Vector3 _playerVelocity;

    private float _currentMovementSpeed;
    
    private Collider[] groundedResults = new Collider[3];

    private void Update()
    {
        if (radioSystem != null && radioSystem.IsRadioActive)
        {
            _moveInput = Vector2.zero;
            OnMovement?.Invoke(Vector2.zero);
            
            if (Grounded() && _playerVelocity.y < 0)
            {
                _playerVelocity.y = 0f;
            }
            _playerVelocity.y += gravity * Time.deltaTime;
            cc.Move(_playerVelocity * Time.deltaTime);
        
            return;
        }
        
        _moveInput.x = Input.GetAxis(Horizontal);
        _moveInput.y = Input.GetAxis(Vertical);
    
        _sprinting = Input.GetKey(sprintingKey);

        if (Grounded() && _playerVelocity.y < 0)
        {
            _playerVelocity.y = 0f;
        }

        _playerVelocity.y += gravity * Time.deltaTime;

        _currentMovementSpeed = _sprinting ? _moveStats.SprintingSpeed :
            _moveInput.y > 0 ? _moveStats.WalkSpeed : _moveStats.WalkBackSpeed;

        if (_moveInput != Vector2.zero)
        {
            transform.Rotate(0, _moveInput.x * _moveStats.RotateSpeed * 50 * Time.deltaTime, 0);
            OnMovement?.Invoke(_moveInput);
        }
    
        cc.Move((_moveInput.y * _currentMovementSpeed * Time.deltaTime) * 
            Move(transform.forward) + (_playerVelocity * Time.deltaTime));
        
        
        if (Input.GetKeyDown(interactKey))
        {
            TryInteract();
        }
    }

    private Vector3 Move(Vector3 velocity)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out var hitInfo, 1.5f, groundedLayers))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustVel = slopeRotation * velocity;

            if (adjustVel.y < 0)
            {
                return adjustVel;
            }
            
        }
        return velocity;
    }

    private bool Grounded()
    {
        var hits = Physics.OverlapSphereNonAlloc(transform.position + groundedOffset,
            cc.radius, groundedResults, groundedLayers);

        if (hits > 0)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (cc == null) return;
        var color = Grounded() ? Color.green : Color.red;
        Gizmos.color =  color;
        Gizmos.DrawSphere(transform.position + groundedOffset, cc.radius);
    }
    
    private void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, interactDistance, interactableLayer))
        {
            DoorKeypad door = hit.collider.GetComponent<DoorKeypad>();
            if (door != null)
            {
                door.OpenKeypadUI();
            }
        }
    }
}

[Serializable]
public struct MoveStats
{
    public float WalkSpeed;
    public float SprintingSpeed;
    public float WalkBackSpeed;
    public float RotateSpeed;
}