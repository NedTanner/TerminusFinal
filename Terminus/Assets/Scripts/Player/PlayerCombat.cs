using Player;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombat : MonoBehaviour
{
    private InputAction _attackAction;
    private PlayerMovement _playerMovement;

    [SerializeField] private Animator animator;

    private void Awake()
    {
        _attackAction = InputSystem.actions.FindAction("Attack");
        _playerMovement = GetComponent<PlayerMovement>();

        _attackAction.performed += _ => StartAttack();
    }

    void StartAttack()
    {
        
    }
}
