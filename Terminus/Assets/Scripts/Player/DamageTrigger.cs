using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private bool canDamagePlayer;
    [SerializeField] private bool canDamageEnemy;
    public float damage;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDamagePlayer)
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();

            health.TakeDamage(damage);
        }

        if (other.CompareTag("Enemy") && canDamageEnemy)
        {
            EnemyHealth health = other.GetComponent<EnemyHealth>();

            health.TakeDamage(damage);
        }
    }
}
