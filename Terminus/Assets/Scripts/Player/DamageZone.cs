using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class DamageZone : MonoBehaviour

{

    EnemyHealth enemyHealth;
    [SerializeField] private float attackDamage = 5f;
    InputAction attackAction;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        
    }

    void Update()
    {


    }
     void OnTriggerStay(Collider other)
    {
        
       
       

        if (attackAction.WasPressedThisFrame() && other.CompareTag("Enemy"))
        {
            enemyHealth = other.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(attackDamage);
        }

    }
}
