using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
   
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] EnemyHealthBar enemyHealthBar;
    private float currentHealth;

    float testHeal = 5f;
    float testHurt = 5f;
    InputAction healAction;
    InputAction hurtAction;

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
        healAction = InputSystem.actions.FindAction("Heal");
        hurtAction = InputSystem.actions.FindAction("Hurt");
        currentHealth = maxHealth;
        
    }

    private void Update()
    {
       // if (healAction.WasPressedThisFrame())
        {
       //     HealDamage(testHeal);
        }
       // if (hurtAction.WasPressedThisFrame())
        {
        //    TakeDamage(testHurt);
        }
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
