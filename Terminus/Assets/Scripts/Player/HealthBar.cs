using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
    [SerializeField] PlayerHealth playerHealth;

   
    public void UpdateHealthBar()
    {
        GameObject playerCharacter = GameObject.FindWithTag("Player");
        playerHealth = playerCharacter.GetComponent<PlayerHealth>();
        float currentHealth = playerHealth.CurrentHealth;
        float maxHealth = playerHealth.MaxHealth;
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
