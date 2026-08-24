using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 0.5f);
    }
}