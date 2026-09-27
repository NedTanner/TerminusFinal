using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private float maxHealth = 10f;
    [SerializeField] HealthBar healthBar;
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
    void Start()
    {
        healAction = InputSystem.actions.FindAction("Heal");
        hurtAction = InputSystem.actions.FindAction("Hurt");
        currentHealth = maxHealth;
    }
    private void Update()
    {
        // purely for testing purposes lol
        if (healAction.WasPressedThisFrame())
        {
            HealDamage(testHeal);
        }
        if (hurtAction.WasPressedThisFrame())
        {
            TakeDamage(testHurt);
        }
    }
    public void ChangeHealth()
    {
       // update healthbar and death check
        print("reed " + currentHealth);
        healthBar = healthBar.GetComponent<HealthBar>();
        healthBar.UpdateHealthBar();
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
        if ( currentHealth <= 0)
        {
            Die();
        }

    }
   
   public void Die()
    { 
        // empty for now 
        print("R.I.P Reed.");
        GameObject playerCharacter = GameObject.FindWithTag("Player");
       // Destroy(playerCharacter);
    }
}
