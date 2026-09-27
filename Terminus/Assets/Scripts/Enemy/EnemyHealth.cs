using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
   
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] EnemyHealthBar enemyHealthBar;
    private float currentHealth;
    
    

    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = value;
            currentHealth = Mathf.Clamp(value, 0, maxHealth);
        }
    }
    public float CurrentHealth
    {
        get => currentHealth;
        private set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth);

        }
    }
    
    private void Start()
    {
        
        currentHealth = maxHealth;
        
    }

    
    public void ChangeHealth()
    {
        print("grunt " + currentHealth);
       
        enemyHealthBar.UpdateHealthBar();
        DeathCheck();

    }
    public void TakeDamage(float amount)
    {
        
        currentHealth -= amount; 
        currentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        ChangeHealth();

    }

    public void HealDamage(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        ChangeHealth();
    }
    public void DeathCheck()
    {
        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public void Die()
    {
        print("bye bye little grunt");

        
    }
}
