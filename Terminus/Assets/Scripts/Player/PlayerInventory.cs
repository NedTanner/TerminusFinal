using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Weapons;

namespace Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        private PlayerCombat _playerCombat;
        [SerializeField] private List<WeaponBase> weapons;
        [SerializeField] private GameObject rightHand;
        [SerializeField] private GameObject leftHand;
        
        private int _currentWeapon;
        private bool _activelySwapping;
        
        private InputAction _nextWeaponAction;
        private InputAction _prevWeaponAction;

        private void Awake()
        {
            _playerCombat = GetComponent<PlayerCombat>();

            _nextWeaponAction = InputSystem.actions.FindAction("NextWeapon");
            _nextWeaponAction.performed += _ => NextWeapon();
            
            _prevWeaponAction = InputSystem.actions.FindAction("PreviousWeapon");
            _prevWeaponAction.performed += _ => PrevWeapon();
        }

        private WeaponBase CurrentWeaponPrefab => weapons[_currentWeapon];
        public WeaponBase CurrentWeapon { get; private set; }

        private void Start()
        {
            DoAnimation();
        }
        
        private void NextWeapon()
        {
            if (_activelySwapping || _playerCombat.Attacking) return;
            _activelySwapping = true;
            _currentWeapon = (_currentWeapon == (weapons.Count-1)) ? 0 : _currentWeapon+1;
            DoAnimation();
        }

        private void PrevWeapon()
        {
            if (_activelySwapping || _playerCombat.Attacking) return;
            _activelySwapping = true;
            _currentWeapon = _currentWeapon == 0 ? weapons.Count-1 : _currentWeapon-1;
            DoAnimation();
        }

        private void DoAnimation()
        {
            // commented out until animation is redone
            // the animation event is what should be calling EnableCurrentWeapon
            
            // if (_animator != null)
            // {
            //     _animator.SetTrigger("SwapWeapon");
            // }
            EnableCurrentWeapon();
        }
        
        public void EnableCurrentWeapon()
        {
            _activelySwapping = false;
            if(CurrentWeapon?.gameObject != null) Destroy(CurrentWeapon.gameObject);
            CurrentWeapon = Instantiate(CurrentWeaponPrefab, leftHand.transform);
            CurrentWeapon.PositionInHands(rightHand, leftHand);
            Debug.Log($"leftHand lossyScale: {leftHand.transform.lossyScale}");

            // set value in animator used to determine correct idle animation
            // base on CurrentWeapon.WeaponType
            if (animator != null)
            {
                switch (CurrentWeapon.WeaponType)
                {
                    case WeaponTypeEnum.TwoHanded:
                        animator.SetFloat("WeaponType", 1.0f);
                        break;
                    case WeaponTypeEnum.OneHanded:
                    case WeaponTypeEnum.Unarmed:
                    default:
                        animator.SetFloat("WeaponType", 0.0f);
                        break;
                }
            }
            
        }
        
    }
}
