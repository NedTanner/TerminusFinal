using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{

    [SerializeField] private float maxHealth = 10f;
    [SerializeField] HealthBar healthBar;
    private float currentHealth;
    float testHeal = 5f;
    float testHurt = -5f;
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
        if (healAction.WasPressedThisFrame())
        {
            ChangeHealth(testHeal);
        }
        if (hurtAction.WasPressedThisFrame())
        {
            ChangeHealth(testHurt);
        }
    }
    public void ChangeHealth(float amount)
    {
        float oldHealth = CurrentHealth;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        print(currentHealth);
        healthBar = healthBar.GetComponent<HealthBar>();
        healthBar.UpdateHealthBar();

    }
  

   
   
}
