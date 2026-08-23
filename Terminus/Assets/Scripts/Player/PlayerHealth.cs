using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{

    [SerializeField] float health = 10f;
    float maxHealth = 10f;
    public float CurrentHealth => health;
    float testHeal = 5f;
    float testHurt = -5f;
    InputAction healAction;
    InputAction hurtAction;


    public void ChangeHealth(float amount)
    {
        float oldHealth = CurrentHealth;
        health += amount;
        health = Mathf.Clamp(CurrentHealth, 0, maxHealth);
        print(health);

    }
     void Start()
    {
        healAction = InputSystem.actions.FindAction("Heal");
        hurtAction = InputSystem.actions.FindAction("Hurt");
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
}
