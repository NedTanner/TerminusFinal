using Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class PlayerHUD : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        [SerializeField] private PlayerHealth  playerHealth;
        [SerializeField] private PlayerPower playerPower;
    
        private VisualElement _rootVe;
        
        private ProgressBar _healthBar;
        private ProgressBar _powerBar;

        private void Start()
        {
            _rootVe = GetComponent<UIDocument>().rootVisualElement;
            _healthBar = _rootVe.Q<ProgressBar>("healthBar");
            _powerBar = _rootVe.Q<ProgressBar>("powerBar");
        }

        // Update is called once per frame
       /* private void Update()
        {
            _healthBar.value = playerHealth.CurrentHealth;
            _healthBar.highValue = playerHealth.MaxHealth;
            _healthBar.lowValue = 0;
            
            _powerBar.value = playerPower.CurrentPower;
            _powerBar.highValue = playerPower.MaxPower;
            _powerBar.lowValue = 0;
        }*/
    }
}
