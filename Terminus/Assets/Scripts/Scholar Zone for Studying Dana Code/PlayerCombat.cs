using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Player
{
    public class PlayerCombatOld : MonoBehaviour
    {
        private InputAction _attackAction;
        private PlayerMovement _playerMovement;
        private PlayerPower _playerPower;

        private float _powerDrawn;
        
        [SerializeField] private Animator animator;
        public bool Attacking { get; private set; }
        public WeaponBase CurrentWeapon;
        
        
        private void Awake()
        {
            _attackAction = InputSystem.actions.FindAction("Attack");
            _playerMovement = GetComponent<PlayerMovement>();
            _playerPower = GetComponent<PlayerPower>();
            
            _attackAction.performed += _ => StartAttack();
        }

        private bool CanAttack()
        {
            return !Attacking && _playerMovement.CurrentMovementState != PlayerMovementStates.Dodge;
        }
        
        public void ActivateDamage(bool activate)
        {
            CurrentWeapon.ActivateDamage(activate, _powerDrawn);
        }

        public void FinishAttack()
        {
            Attacking = false;
            _powerDrawn = 0;
            ActivateDamage(false);
            CurrentWeapon.PoseForAttack(false);
        }
        
        private void StartAttack()
        {
            if (!CanAttack()) return;
            Attacking = true;
            
            // we store the powerDrawn value so it can be passed in via ActivateDamage
            _powerDrawn = _playerPower.DrawUponPower(CurrentWeapon.RegenPerSec);
            
            if (animator != null)
            {
                CurrentWeapon.PoseForAttack(true);
                switch (CurrentWeapon.WeaponType)
                {
                    case WeaponTypeEnum.OneHanded:
                        animator.SetTrigger("DoOneSwing");
                        break;
                    case WeaponTypeEnum.TwoHanded:
                        animator.SetTrigger("DoTwoSwing");
                        break;
                }
            }
        }
    }
}