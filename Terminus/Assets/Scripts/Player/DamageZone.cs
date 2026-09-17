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
        GameObject enemy = GameObject.FindWithTag("Enemy");
        enemyHealth = enemy.GetComponent<EnemyHealth>();
    }

    void Update()
    {


    }
     void OnTriggerStay(Collider other)
    {
        
       
       

        if (attackAction.WasPressedThisFrame())
        {
            
            enemyHealth.TakeDamage(attackDamage);
        }

    }
}
