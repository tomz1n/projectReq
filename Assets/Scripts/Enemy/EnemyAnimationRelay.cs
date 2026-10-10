using UnityEngine;

public class EnemyAnimationRelay : MonoBehaviour
{
    private EnemyAI _enemyAI;

    private void Awake()
    {
        _enemyAI = GetComponentInParent<EnemyAI>();
    }

    public void OnAttackStart()
    {
        if (_enemyAI != null && _enemyAI.gameObject != null)
        {
            _enemyAI.OnAttackStart();
        }
    }

    public void OnAttackEnd()
    {
        if (_enemyAI != null && _enemyAI.gameObject != null)
        {
            _enemyAI.OnAttackEnd();
        }
    }
}