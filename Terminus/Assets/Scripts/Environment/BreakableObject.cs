using UnityEngine;
using Interfaces;
using Player;
using Enemy;

public class BreakableObject : MonoBehaviour, IDamageable
{
    [SerializeField] private GameObject gibObject; // Prefab for giblets. Need to rework for particle system. DON'T FORGET!
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private GameObject[] baseMeshes; // Include anything you want to disappear when the prop is broken. Base mesh is replaced by matching giblets.
    private int _currentHealth;
    private GameObject spawnedGibs;

    public int MaxHealth => maxHealth;
    public int CurrentHealth => _currentHealth;

    public void Start()
    {
        _currentHealth = maxHealth;
    }
    public bool IsAlive()
    {
        return CurrentHealth > 0;
    }

    public bool TakeDamage(int amount, PainTypes painType = PainTypes.Hit)
    {
        if (!IsAlive()) return false;

        _currentHealth -= amount;

        if (CurrentHealth <= 0)
        {
            DisableBaseMeshes();
            GetComponent<CapsuleCollider>().enabled = false;
            
            spawnedGibs = Instantiate(gibObject, transform.position, transform.rotation);
            CleanupTimer();
            return true;
        }

        return true;
    }

    private void CleanupTimer()
    {
        Destroy(spawnedGibs, 7f);
        Destroy(gameObject);
    }

    private void DisableBaseMeshes()
    {
        foreach (GameObject obj in baseMeshes)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
}
