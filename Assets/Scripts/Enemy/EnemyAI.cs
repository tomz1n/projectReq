using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour, IDamageable
{
    public enum EnemyState { Patrol, Chase, Attack, Dead }

    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    
    [SerializeField] private float maxHealth = 50f;
    [SerializeField] private float attackDamage = 15f;
    [SerializeField] private float attackRate = 1.5f;
    
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float chaseSpeed = 3.5f;
    
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private LayerMask obstacleMask;
    
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private float waypointWaitTime = 2f;

    private EnemyState _currentState = EnemyState.Patrol;
    private float _currentHealth;
    private int _currentWaypointIndex;
    private float _nextAttackTime;
    private bool _isWaitingAtWaypoint;
    private bool _isAttacking;

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        _currentHealth = maxHealth;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            SetDestinationToWaypoint();
        }
    }

    private void Update()
    {
        if (_currentState == EnemyState.Dead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (!_isAttacking)
        {
            if (CanSeePlayer(distanceToPlayer))
            {
                if (distanceToPlayer <= attackRange)
                {
                    _currentState = EnemyState.Attack;
                }
                else
                {
                    _currentState = EnemyState.Chase;
                }
            }
            else if (_currentState == EnemyState.Chase)
            {
                _currentState = EnemyState.Patrol;
                SetDestinationToWaypoint();
            }
        }

        switch (_currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;

            case EnemyState.Attack:
                HandleAttack();
                break;
        }

        UpdateAnimator();
    }

    private void HandlePatrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0 || _isWaitingAtWaypoint || _isAttacking) return;

        agent.speed = patrolSpeed;
        agent.isStopped = false;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            StartCoroutine(WaitAtWaypointRoutine());
        }
    }

    private void HandleChase()
    {
        if (_isAttacking) return;

        agent.speed = chaseSpeed;
        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private void HandleAttack()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0;
        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        }

        if (Time.time >= _nextAttackTime && !_isAttacking)
        {
            _nextAttackTime = Time.time + attackRate;

            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        float speed = agent.velocity.magnitude;

        animator.SetBool("IsWalking", _currentState == EnemyState.Patrol && speed > 0.1f && !_isWaitingAtWaypoint);
        animator.SetBool("IsRunning", _currentState == EnemyState.Chase && speed > 0.1f);
    }
    

    public void OnAttackStart()
    {
        _isAttacking = true;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange + 0.5f)
        {
            IDamageable playerDamageable = player.GetComponent<IDamageable>();
            if (playerDamageable != null)
            {
                playerDamageable.TakeDamage(attackDamage);
            }

            Debug.Log($"<color=red>[ENEMIGO ATACO]</color> Realizó {attackDamage} de daño al jugador.");
        }
    }

    public void OnAttackEnd()
    {
        _isAttacking = false;

        if (_currentState != EnemyState.Dead)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer > attackRange)
            {
                agent.isStopped = false;
                _currentState = EnemyState.Chase;
            }
        }
    }

    private IEnumerator WaitAtWaypointRoutine()
    {
        _isWaitingAtWaypoint = true;
        agent.isStopped = true;

        yield return new WaitForSeconds(waypointWaitTime);

        _currentWaypointIndex = (_currentWaypointIndex + 1) % patrolPoints.Length;
        SetDestinationToWaypoint();

        _isWaitingAtWaypoint = false;
    }

    private void SetDestinationToWaypoint()
    {
        if (patrolPoints != null && patrolPoints.Length > 0 && patrolPoints[_currentWaypointIndex] != null)
        {
            agent.SetDestination(patrolPoints[_currentWaypointIndex].position);
        }
    }

    private bool CanSeePlayer(float distanceToPlayer)
    {
        if (distanceToPlayer > visionRange) return false;

        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 target = (player.position + Vector3.up * 1.2f) - origin;

        if (!Physics.Raycast(origin, target, distanceToPlayer, obstacleMask))
        {
            return true;
        }

        return false;
    }

    public void TakeDamage(float damage)
    {
        if (_currentState == EnemyState.Dead) return;

        _currentHealth -= damage;
        Debug.Log($"<color=orange>[ENEMIGO IMPACTADO]</color> Vida restante: {_currentHealth}");

        if (_currentState == EnemyState.Patrol)
        {
            _currentState = EnemyState.Chase;
        }

        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _currentState = EnemyState.Dead;
        Debug.Log("<color=black>[ENEMIGO MUERTO]</color>");

        agent.isStopped = true;
        agent.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        Destroy(gameObject, 3f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}