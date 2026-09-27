using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class DamageZone : MonoBehaviour

{
    PlayerPower playerPower;
    EnemyHealth enemyHealth;
    
    InputAction attackAction;
    public float finalDamage;

    private void Awake()
    {
        
        attackAction = InputSystem.actions.FindAction("Attack");
        
    }

    private void Start()
    {
        playerPower = GetComponent<PlayerPower>();
      // float modifiedDamage = playerPower.finalDamage;
        
    }

    private void Update()
    {
        //sattackDamage = modifiedDamage;
    }
    void OnTriggerStay(Collider other)
    {
        
     // attacks once it find an enemy and attack is performed
        if (attackAction.WasPressedThisFrame() && other.CompareTag("Enemy"))
        {

          //  attackDamage = finalDamage;
            
            enemyHealth = other.GetComponent<EnemyHealth>();
            enemyHealth.TakeDamage(finalDamage);
        }

    }
}
