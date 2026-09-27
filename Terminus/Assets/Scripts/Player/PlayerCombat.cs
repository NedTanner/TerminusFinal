using Player;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private InputAction attackAction;
    private PlayerMovement playerMovement;
    private PlayerPower playerPower;

    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float bufferedAttackTimer = 0.2f;
    [SerializeField] public float weaponPower = 2f;
    [SerializeField] public float weaponRegen = 1f;
    

    [SerializeField] private Collider weaponCollider;

    public bool isAttacking { get; private set; }

    public int comboIndex;
    private float attackCooldownTimer;
    private bool bufferedAttack;
    private float bufferTimer;


    private void Awake()
    {
        attackAction = InputSystem.actions.FindAction("Attack");
        playerMovement = GetComponent<PlayerMovement>();
        playerPower = GetComponent<PlayerPower>(); 

        attackAction.performed += _ => AttackAttempt();
        comboIndex = 1;
    }

    private void Update()
    {
        if (attackCooldownTimer > 0) // Subtracts attack cooldown timer, simplez (compare the market reference).
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        if (bufferedAttack) // Subtracts buffer timer until 0. Means that an unsuccessful attack attempt is only cached for bufferTimer amount of time.
        {
            bufferTimer -= Time.deltaTime;

            if (bufferTimer <= 0)
            {
                bufferedAttack = false;
                
            }    
        }

        if (bufferedAttack == true && CanAttack())
        {
            bufferedAttack = false;
            Attack();
        }

        AnimatorStateInfo state = playerAnimator.GetCurrentAnimatorStateInfo(2); // Checks animation layer index of 2, the combat layer.

        if (state.IsTag("Attack")) // Checks if animation tagged with 'Attack' playing. REMINDER: Make sure to tag any player combat animations as 'Attack'.
        {
            isAttacking = state.normalizedTime < (1f - 0.3f); // Once animation 70% completed, allow attack cooldown logic to begin. Works with any animation.
        }

        else
        {
            isAttacking = false;
        }
    }
    private void AttackAttempt()
    {
        if (!CanAttack()) // If player clicks attack while actively attacking, caches input for a small time to feel more responsive.
        {
            
            bufferedAttack = true;
            bufferTimer = bufferedAttackTimer;
            return;
        }

        Attack();
    }

    private void Attack()
    {
        isAttacking = true;

        attackCooldownTimer = attackCooldown;

        playerAnimator.SetInteger("AnimIndex", comboIndex);
        playerAnimator.SetTrigger("Attack");

        comboIndex = comboIndex % 2 + 1; // REMEMBER THIS. If making longer movesets, change '2' to number that changes based on moveset length.

        playerPower.UsePower();

    }

    private bool CanAttack()
    {
        return attackCooldownTimer <= 0 && !isAttacking && playerMovement.CurrentMovementState != PlayerMovementStates.Dodge;
    }

    public void ActivateDamage()
    {
        weaponCollider.enabled = true;
    }

    public void DisableDamage()
    {
        weaponCollider.enabled = false;
    }
}
