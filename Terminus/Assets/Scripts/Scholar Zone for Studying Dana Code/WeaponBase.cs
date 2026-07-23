using System;
using UnityEngine;
using Enemy;
using Interfaces;
using Player;

namespace Weapons
{
    public enum WeaponTypeEnum
    {
        OneHanded,
        TwoHanded,
        Unarmed
    }
    
    public class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected int attackDamage = 2;
        [SerializeField] protected float regenPerSec = 1.0f;
        [SerializeField] protected string weaponName = "unnamed";
        [SerializeField] protected WeaponTypeEnum weaponType;
        [SerializeField] protected PainTypes painType;
        [SerializeField] protected float startDamageFrame;
        [SerializeField] protected float endDamageFrame;
        [SerializeField] protected BoxCollider damageArea;
        [SerializeField] protected GameObject impactEffect;
        
        private float _damageMod;
        
        public float RegenPerSec => regenPerSec;
        
        public float StartDamageFrame => startDamageFrame;
        public float EndDamageFrame => endDamageFrame;
        
        public int AttackDamage
        {
            get => attackDamage;
            private set => attackDamage = value;
        }

        public string WeaponName
        {
            get => weaponName;
            private set => weaponName = value;
        }

        public WeaponTypeEnum WeaponType
        {
            get => weaponType;
            private set => weaponType = value;
        }

        public virtual void PositionInHands(GameObject rightHand, GameObject leftHand)
        {

        }

        public void Start()
        {
            damageArea.enabled = false;
        }

        public virtual void ActivateDamage(bool activate, float power = 0)
        {
            damageArea.enabled = activate;
            _damageMod = activate? power: 0; //if damage is being turned off set power to 0

        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Damageable"))
            {
                // by default weapons always do at least 10% of their damage regardless of the passed
                // modifier
                
                int damage = (int)Math.Floor(attackDamage*Mathf.Clamp(_damageMod, 0.1f, 1f));
                bool didHurt = other.GetComponent<IDamageable>().TakeDamage(damage, painType);
                if (didHurt && impactEffect)
                {
                    Instantiate(impactEffect, transform.position, transform.rotation);
                }
            }
        }

        public virtual void PoseForAttack(bool forAttack)
        {
            
        }
        
    }
}