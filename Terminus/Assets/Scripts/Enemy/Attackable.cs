using UnityEngine;

public class Attackable : MonoBehaviour
{
    [SerializeField] private Collider swingTrigger;

    private void Awake()
    {
        swingTrigger.enabled = false;
    }

    public void ActivateTrigger()
    {
        swingTrigger.enabled = true;
    }

    public void DisableTrigger()
    {
        swingTrigger.enabled = false;
    }
}
