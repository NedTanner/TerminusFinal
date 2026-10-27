using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractArea : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private Interactable[] interactableList;
    private bool isOccupied = false;
    private InputAction interactAction;

    private void Awake()
    {
        interactAction = InputSystem.actions.FindAction("Interact");

        interactAction.performed += _ => enableCheck();
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isOccupied = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            isOccupied = false;
        }
    }

    private void enableCheck()
    {
        if (isOccupied)
        {
            foreach (Interactable interactable in interactableList) // Activates every interactable in list, can attach multiple doors to the same lever
            {
                interactable.Activate();

                continue;
            }
        }
    }
}
