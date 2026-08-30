using System;
using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Image healthBar;
    [SerializeField] EnemyHealth enemyHealth;
    [SerializeField] Camera camera;
    [SerializeField] Transform target;

    public void UpdateHealthBar()
    {
        
        float currentHealth = enemyHealth.CurrentHealth;
        float maxHealth = enemyHealth.MaxHealth;
        healthBar.fillAmount = currentHealth / maxHealth;
    }

    private void Update()
    {
        transform.rotation = camera.transform.rotation;
    }
}
