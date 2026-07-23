using System;
using UnityEngine;

namespace Player
{
    public class PlayerPower : MonoBehaviour
    {
        [SerializeField] private int maxPower = 10;
        private float _currentPower;
        private float _currentRegen = -1.0f;    // value of -1 means not regenerating
        private float _secsSinceLastRegen;
        private PlayerCombat _playerCombat;
        
        public float DrawUponPower(float regenRate)
        {
            // if we aren't regenerating currently (current regen is less than 0)
            // or regen rate is less than current, then set current rate to this one
            if (_currentRegen < 0 || regenRate < _currentRegen)
            {
                _currentRegen = regenRate;
            }
            
            // get our current power value & set CurrentPower to zero
            float powerToReturn = _currentPower/maxPower;
            _currentPower = 0;
            
            // return power value
            return powerToReturn;
        }

        private void Update()
        {
            // only if we are:
            //  - not attacking
            //  - have a regen rate > 0
            if (!_playerCombat.Attacking && _currentRegen > 0) RegenPower();
        }

        private void RegenPower()
        {
            // increase timer and if less than a second has 
            // accumulated then return;
            _secsSinceLastRegen += Time.deltaTime;
            if (_secsSinceLastRegen < 1) return;
            
            // at least a second has passed so reset the timer
            // and increase power
            _secsSinceLastRegen = 0;
            _currentPower += _currentRegen;
            
            // if we are at max power stop regenerating
            if (_currentPower >= maxPower)
            {
                _currentRegen = -1;
            }
        }
        
        private void Awake()
        {
            _playerCombat = GetComponent<PlayerCombat>();
            _currentPower = maxPower;
        }
        
        public int MaxPower => maxPower;
        public int CurrentPower => (int)Math.Ceiling(_currentPower);
    }
}
