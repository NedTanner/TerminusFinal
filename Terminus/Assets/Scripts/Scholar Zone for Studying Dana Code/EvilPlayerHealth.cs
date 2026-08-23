using Interfaces;
using UI;
using UnityEngine;

namespace Player
{
    public enum PainTypes
    {
        Hit,
        HeadHurts
    }

    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        private static readonly int IsDead = Animator.StringToHash("isDead");
        private static readonly int GotHitTrigger = Animator.StringToHash("Hit");

        private PlayerMovement _playerMovement;
        [SerializeField] private Animator animator;

        [SerializeField] private int maxHealth = 25;
        private int _currentHealth;

        private bool IsImmune()
        { return _playerMovement.isImmune; }

        private void Awake()
        {
            _playerMovement = GetComponent<PlayerMovement>();
        }

        private void Start()
        {
            CurrentHealth = maxHealth;
        }

        public int MaxHealth
        {
            get => maxHealth;
            set
            {
                maxHealth = value;
                _currentHealth = Mathf.Clamp(value, 0, maxHealth);
            }
        }

        public int CurrentHealth
        {
            get => _currentHealth;
            private set
            {
                _currentHealth = Mathf.Clamp(value, 0, maxHealth);
                DeathCheck();
            }
        }

        private void DeathCheck()
        {
            if (_currentHealth <= 0)
            {
                animator.SetBool(IsDead, true);
                GetComponentInChildren<SceneSelectorMenu>().ShowMenu(true);
            }
        }

        public bool TakeDamage(int amount, PainTypes painType = PainTypes.Hit)
        {
            // bail if immune
            if (IsImmune()) return false;

            // apply damage
            CurrentHealth -= amount;

            // bail if now dead
            if (_currentHealth <= 0) return true;

            // otherwise play appropriate animation
            switch (painType)
            {
                case PainTypes.Hit:
                    {
                        animator.SetTrigger(GotHitTrigger);
                        break;
                    }
            }

            return true;
        }

        public bool IsAlive()
        {
            return  CurrentHealth > 0;
        }


        public void Heal(int amount)
        {
            CurrentHealth += amount;
        }
    }
}

