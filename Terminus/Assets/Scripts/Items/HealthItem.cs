using UnityEngine;

public class HealthItem : MonoBehaviour
{
    private float healAmount = 5f;
    PlayerHealth playerHealth;

   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            playerHealth.HealDamage(healAmount);
            
        }
       Destroy(gameObject);
    }
}
