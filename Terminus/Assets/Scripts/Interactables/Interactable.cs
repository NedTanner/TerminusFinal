using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    private Animator animator;
    private InputAction interactAction;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Activate() // Custom interactables need to have an 'activate' trigger variable, activation animation, and setup, check door prefab Animator for reference
    {
        animator.SetTrigger("Activate");
    }
}
