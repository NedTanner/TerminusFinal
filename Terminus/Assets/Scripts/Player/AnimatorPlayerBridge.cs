using UnityEngine;
using Player;

public class AnimatorPlayerBridge : MonoBehaviour
{
    // This piece of shit's only purpose is allow the animator, which is nested on Protoreed inside the player, to contact up the heirarchy
    // to PlayerMovement and PlayerCombat, which are mounted to the top of the player. It's dumb!

    [SerializeField] private PlayerCombat playerCombat;
    [SerializeField] private PlayerMovement playerMovement;

    public void ActivateDamageBridge()
    {
        playerCombat.ActivateDamage();
    }

    public void DisableDamageBridge()
    {
        playerCombat.DisableDamage();
    }

    public void ForwardForceBridge(float duration)
    {
        playerMovement.CallMovementEnumerator(duration);
    }
}
