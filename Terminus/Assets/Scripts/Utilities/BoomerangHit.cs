using Enemy;
using UnityEngine;

public class BoomerangHit : MonoBehaviour
{
    [SerializeField] private Boomerang boomerang;

    private void OnTriggerEnter(Collider other)
    {
        if (!boomerang.IsActive()) return;
        if (other.gameObject.layer != LayerMask.NameToLayer("Enemy")) return;
        /* evil dana code 
        var enemy = other.GetComponent<CubeEnemy>();
        if (enemy != null) enemy.Stun();

        var territorialEnemy = other.GetComponent<TerritorialCubeEnemy>();
        if (territorialEnemy != null) territorialEnemy.Stun(); */
    }
}